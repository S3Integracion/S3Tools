using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace S3Tools.Tests
{
    public class SitemapTemplateRepositoryTests : IDisposable
    {
        private const string ValidSitemap =
            "{\"_id\":\"x\",\"startUrl\":[\"https://a.com\"],\"selectors\":[" +
            "{\"id\":\"title\",\"type\":\"SelectorText\",\"parentSelectors\":[\"_root\"],\"selector\":\"h1\",\"multiple\":false}]}";

        private readonly string _dir;
        private readonly string _file;

        public SitemapTemplateRepositoryTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "S3ToolsTests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            _file = Path.Combine(_dir, "sitemap-templates.json");
        }

        public void Dispose()
        {
            if (Directory.Exists(_dir))
            {
                Directory.Delete(_dir, true);
            }
        }

        private SitemapTemplateRepository NewRepo()
        {
            return new SitemapTemplateRepository(_file);
        }

        private static SitemapTemplateDraft Draft(string name, string sitemap = ValidSitemap, params string[] stores)
        {
            return new SitemapTemplateDraft
            {
                Name = name,
                Description = "desc",
                Stores = stores,
                SitemapJson = sitemap,
            };
        }

        private static string Errors(TemplateSaveResult result)
        {
            return string.Join("|", result.Issues.Where(i => i.Severity == TemplateIssueSeverity.Error).Select(i => i.Message));
        }

        private static string Warnings(TemplateSaveResult result)
        {
            return string.Join("|", result.Issues.Where(i => i.Severity == TemplateIssueSeverity.Warning).Select(i => i.Message));
        }

        [Fact]
        public void Constructor_DoesNoIo_UntilFirstAccess()
        {
            var repo = NewRepo();

            Assert.False(File.Exists(_file));
            Assert.Equal(2, repo.GetAll().Count);
            Assert.True(File.Exists(_file));
        }

        [Fact]
        public void Create_Valid_PersistsAndRaisesChanged()
        {
            var repo = NewRepo();
            var raised = 0;
            repo.Changed += (s, e) => raised++;

            var result = repo.Create(Draft("  Mi plantilla  ", ValidSitemap, "altinor"));

            Assert.True(result.Ok);
            Assert.Equal(1, raised);
            Assert.Equal("Mi plantilla", result.Template.Name);
            Assert.Equal(new[] { "Altinor" }, result.Template.Stores);
            Assert.Null(result.Template.BuiltInKey);
            Assert.Matches("^[a-z0-9]{32}$", result.Template.Id);

            var reloaded = NewRepo().Find(result.Template.Id);
            Assert.NotNull(reloaded);
            Assert.Equal("Mi plantilla", reloaded.Name);
            Assert.Equal(new[] { "Altinor" }, reloaded.Stores);
        }

        [Fact]
        public void Create_StoresCopyOfCallerStores()
        {
            var repo = NewRepo();
            var stores = new List<string> { "Altinor" };
            var draft = Draft("Copia");
            draft.Stores = stores;

            var result = repo.Create(draft);
            stores.Add("ProductosTX");

            Assert.Equal(new[] { "Altinor" }, result.Template.Stores);
        }

        [Fact]
        public void Create_PersistsNormalizedSitemap_AndReportsValidatorWarnings()
        {
            var repo = NewRepo();
            var result = repo.Create(Draft("Tolerante", "﻿{\"selectors\":[{\"id\":\"a\",\"type\":\"SelectorText\",/*c*/},]}"));

            Assert.True(result.Ok);
            Assert.Contains("_id", Warnings(result));
            Assert.DoesNotContain("/*", result.Template.SitemapJson);
            Assert.StartsWith("{", result.Template.SitemapJson);
        }

        [Fact]
        public void Create_DuplicateNameIgnoringCase_Fails()
        {
            var repo = NewRepo();

            var result = repo.Create(Draft("TIENDAS (normal)"));

            Assert.False(result.Ok);
            Assert.Null(result.Template);
            Assert.Equal("Ya existe una plantilla llamada «TIENDAS (normal)».", Errors(result));
            Assert.Equal(2, repo.GetAll().Count);
        }

        [Fact]
        public void Create_NameEmptyOrOver60_Fails()
        {
            var repo = NewRepo();

            var empty = repo.Create(Draft("   "));
            var tooLong = repo.Create(Draft(new string('a', 61)));
            var maxOk = repo.Create(Draft(new string('b', 60)));

            Assert.False(empty.Ok);
            Assert.Equal("El nombre es obligatorio.", Errors(empty));
            Assert.False(tooLong.Ok);
            Assert.Equal("El nombre no puede superar 60 caracteres.", Errors(tooLong));
            Assert.True(maxOk.Ok);
        }

        [Fact]
        public void Create_DescriptionOver200_Fails()
        {
            var draft = Draft("Larga");
            draft.Description = new string('d', 201);

            var result = NewRepo().Create(draft);

            Assert.False(result.Ok);
            Assert.Equal("La descripción no puede superar 200 caracteres.", Errors(result));
        }

        [Fact]
        public void Create_InvalidJson_FailsWithValidatorIssues()
        {
            var repo = NewRepo();

            var result = repo.Create(Draft("Rota", "{\"selectors\": [ }"));

            Assert.False(result.Ok);
            var issue = Assert.Single(result.Issues, i => i.Severity == TemplateIssueSeverity.Error);
            Assert.StartsWith("JSON inválido", issue.Message);
            Assert.NotNull(issue.Line);
            Assert.Equal(2, repo.GetAll().Count);
        }

        [Fact]
        public void Create_UnknownStore_Fails()
        {
            var result = NewRepo().Create(Draft("Tienda rara", ValidSitemap, "NoExiste"));

            Assert.False(result.Ok);
            Assert.Equal("La tienda «NoExiste» no existe en el catálogo.", Errors(result));
        }

        [Fact]
        public void Update_MovesStoreFromOtherTemplate_ReturnsWarning()
        {
            var repo = NewRepo();
            var custom = repo.Create(Draft("Custom")).Template;

            var result = repo.Update(custom.Id, Draft("Custom", ValidSitemap, "altinor"));

            Assert.True(result.Ok);
            Assert.Equal(new[] { "Altinor" }, result.Template.Stores);
            Assert.Equal("La tienda Altinor se desvinculó de «Tiendas (Normal)».", Warnings(result));
            Assert.DoesNotContain("Altinor", repo.Find("tiendas").Stores);
            Assert.Equal(custom.Id, repo.FindByStore("ALTINOR").Id);

            var reloaded = NewRepo();
            Assert.DoesNotContain("Altinor", reloaded.Find("tiendas").Stores);
            Assert.Equal(custom.Id, reloaded.FindByStore("altinor").Id);
        }

        [Fact]
        public void Update_KeepsIdAndCreatedUtc_BumpsUpdatedUtc()
        {
            var repo = NewRepo();
            var created = repo.Create(Draft("Original")).Template;

            var result = repo.Update(created.Id, Draft("Renombrada"));

            Assert.True(result.Ok);
            Assert.Equal(created.Id, result.Template.Id);
            Assert.Equal(created.CreatedUtc, result.Template.CreatedUtc);
            Assert.True(result.Template.UpdatedUtc > created.UpdatedUtc);
            Assert.Equal("Renombrada", repo.Find(created.Id).Name);
            Assert.Equal(created.Id, repo.GetAll()[2].Id);
        }

        [Fact]
        public void Update_KeepsBuiltInKey_AndAllowsOwnNameChangeCasing()
        {
            var repo = NewRepo();

            var result = repo.Update("tiendas", Draft("TIENDAS (NORMAL)"));

            Assert.True(result.Ok);
            Assert.Equal("tiendas", result.Template.BuiltInKey);
            Assert.Equal("TIENDAS (NORMAL)", result.Template.Name);
        }

        [Fact]
        public void Update_DuplicateOfOtherName_Fails()
        {
            var repo = NewRepo();

            var result = repo.Update("tiendas", Draft("bbvs (nube)"));

            Assert.False(result.Ok);
            Assert.Equal("Ya existe una plantilla llamada «bbvs (nube)».", Errors(result));
        }

        [Fact]
        public void Update_MissingId_Fails()
        {
            var result = NewRepo().Update("noexiste", Draft("X"));

            Assert.False(result.Ok);
            Assert.Equal("La plantilla no existe.", Errors(result));
        }

        [Fact]
        public void Delete_LastTemplate_Fails()
        {
            var repo = NewRepo();
            Assert.True(repo.Delete("bbvs").Ok);

            var result = repo.Delete("tiendas");

            Assert.False(result.Ok);
            Assert.Equal("No se puede eliminar la última plantilla.", Errors(result));
            Assert.Single(repo.GetAll());
        }

        [Fact]
        public void Delete_BuiltIn_Succeeds()
        {
            var repo = NewRepo();
            var raised = 0;
            repo.Changed += (s, e) => raised++;

            var result = repo.Delete("tiendas");

            Assert.True(result.Ok);
            Assert.Equal("tiendas", result.Template.Id);
            Assert.Null(repo.Find("tiendas"));
            Assert.Null(NewRepo().Find("tiendas"));
            Assert.Equal(1, raised);
        }

        [Fact]
        public void Delete_MissingId_Fails()
        {
            var result = NewRepo().Delete("noexiste");

            Assert.False(result.Ok);
            Assert.Equal("La plantilla no existe.", Errors(result));
        }

        [Fact]
        public void RestoreDefaults_RecreatesDeletedBuiltIn_ResetsEditedBuiltIn_KeepsCustom()
        {
            var repo = NewRepo();
            var custom = repo.Create(Draft("Custom", ValidSitemap, "BBvsBB2")).Template;
            repo.Delete("tiendas");
            repo.Update("bbvs", Draft("Mi BBvs", ValidSitemap, "BBvs_Template"));
            var factoryBbvs = BuiltInSitemapTemplates.GetSitemapJson("bbvs");

            var result = repo.RestoreDefaults();

            Assert.True(result.Ok);
            var tiendas = repo.Find("tiendas");
            Assert.NotNull(tiendas);
            Assert.Equal("Tiendas (Normal)", tiendas.Name);
            Assert.Equal(StoreCatalog.StoresLeft, tiendas.Stores);
            Assert.Equal("tiendas", tiendas.BuiltInKey);

            var bbvs = repo.Find("bbvs");
            Assert.Equal("BBvs (Nube)", bbvs.Name);
            Assert.Equal(
                SitemapTemplateValidator.Normalize(SitemapTemplateValidator.Validate(factoryBbvs).Sitemap),
                bbvs.SitemapJson);

            var keptCustom = repo.Find(custom.Id);
            Assert.Equal("Custom", keptCustom.Name);
            Assert.Equal(custom.SitemapJson, keptCustom.SitemapJson);
            Assert.Equal(new[] { "BBvsBB2" }, keptCustom.Stores);

            var reloaded = NewRepo();
            Assert.Equal(3, reloaded.GetAll().Count);
            Assert.Equal("Tiendas (Normal)", reloaded.Find("tiendas").Name);
        }

        [Fact]
        public void RestoreDefaults_DoesNotStealStoreLinkedToCustom()
        {
            var repo = NewRepo();
            var custom = repo.Create(Draft("Custom")).Template;
            repo.Update(custom.Id, Draft("Custom", ValidSitemap, "Altinor"));
            repo.Delete("tiendas");

            var result = repo.RestoreDefaults();

            Assert.True(result.Ok);
            Assert.Equal(custom.Id, repo.FindByStore("Altinor").Id);
            var tiendas = repo.Find("tiendas");
            Assert.Equal(new[] { "ProductosTX", "Holaproducto", "HervazTrade" }, tiendas.Stores);
        }

        [Fact]
        public void RestoreDefaults_FactoryNameTakenByCustom_UsesSuffix()
        {
            var repo = NewRepo();
            repo.Update("tiendas", Draft("Renombrada"));
            repo.Create(Draft("Tiendas (Normal)"));

            var result = repo.RestoreDefaults();

            Assert.True(result.Ok);
            Assert.Equal("Tiendas (Normal) (2)", repo.Find("tiendas").Name);
        }

        [Fact]
        public void RestoreDefaults_RecreatedBuiltIn_ReusesKeyAsId()
        {
            var repo = NewRepo();
            repo.Delete("tiendas");

            repo.RestoreDefaults();

            Assert.Equal("tiendas", repo.GetAll().Single(t => t.BuiltInKey == "tiendas").Id);
        }

        [Fact]
        public void RestoreDefaults_KeyIdTakenByCustom_UsesGuidId()
        {
            const string entry = "{\"id\":\"tiendas\",\"name\":\"Propia\",\"description\":\"\",\"builtInKey\":null,\"stores\":[],\"sitemap\":" + ValidSitemap + "}";
            File.WriteAllText(_file, "{\"schemaVersion\":1,\"templates\":[" + entry + "]}");
            var repo = NewRepo();

            var result = repo.RestoreDefaults();

            Assert.True(result.Ok);
            Assert.Equal("Propia", repo.Find("tiendas").Name);
            var restored = repo.GetAll().Single(t => t.BuiltInKey == "tiendas");
            Assert.Matches("^[a-z0-9]{32}$", restored.Id);
            Assert.NotNull(repo.Find("bbvs"));
        }

        [Fact]
        public void RestoreDefaults_RaisesChanged()
        {
            var repo = NewRepo();
            var raised = 0;
            repo.Changed += (s, e) => raised++;

            repo.RestoreDefaults();

            Assert.Equal(1, raised);
        }

        [Fact]
        public void Duplicate_GeneratesUniqueCopyNames()
        {
            var repo = NewRepo();

            var first = repo.Duplicate("tiendas");
            var second = repo.Duplicate("tiendas");
            var third = repo.Duplicate("tiendas");

            Assert.Equal("Tiendas (Normal) (copia)", first.Template.Name);
            Assert.Equal("Tiendas (Normal) (copia 2)", second.Template.Name);
            Assert.Equal("Tiendas (Normal) (copia 3)", third.Template.Name);
            Assert.Empty(first.Template.Stores);
            Assert.Null(first.Template.BuiltInKey);
            Assert.NotEqual("tiendas", first.Template.Id);
            Assert.Equal(repo.Find("tiendas").SitemapJson, first.Template.SitemapJson);
        }

        [Fact]
        public void Duplicate_LongName_StaysWithin60Chars()
        {
            var repo = NewRepo();
            var longName = new string('n', 60);
            var created = repo.Create(Draft(longName)).Template;

            var copy = repo.Duplicate(created.Id);

            Assert.True(copy.Ok);
            Assert.True(copy.Template.Name.Length <= 60);
            Assert.EndsWith(" (copia)", copy.Template.Name);
        }

        [Fact]
        public void Duplicate_MissingId_Fails()
        {
            var result = NewRepo().Duplicate("noexiste");

            Assert.False(result.Ok);
            Assert.Equal("La plantilla no existe.", Errors(result));
        }

        [Fact]
        public void Import_UsesFileNameAndDisambiguates()
        {
            var repo = NewRepo();

            var first = repo.Import(ValidSitemap, "Mi archivo");
            var second = repo.Import(ValidSitemap, "mi archivo");

            Assert.True(first.Ok);
            Assert.Equal("Mi archivo", first.Template.Name);
            Assert.Equal("mi archivo (2)", second.Template.Name);
            Assert.Empty(first.Template.Stores);
            Assert.Null(first.Template.BuiltInKey);
            Assert.Equal(4, NewRepo().GetAll().Count);
        }

        [Fact]
        public void Import_BlankSuggestedName_FallsBackToJsonIdThenDefault()
        {
            var repo = NewRepo();

            var fromId = repo.Import(ValidSitemap, "  ");
            var noId = repo.Import("{\"selectors\":[{\"id\":\"a\",\"type\":\"SelectorText\"}]}", null);

            Assert.Equal("x", fromId.Template.Name);
            Assert.True(noId.Ok);
            Assert.Equal("Plantilla importada", noId.Template.Name);
        }

        [Fact]
        public void Import_LongName_IsCutTo60()
        {
            var result = NewRepo().Import(ValidSitemap, new string('z', 100));
            var second = NewRepo().Import(ValidSitemap, new string('z', 100));

            Assert.Equal(60, result.Template.Name.Length);
            Assert.True(second.Template.Name.Length <= 60);
        }

        [Fact]
        public void Import_LongNameCollision_StillFits60()
        {
            var repo = NewRepo();
            var name = new string('z', 100);
            var first = repo.Import(ValidSitemap, name);
            var second = repo.Import(ValidSitemap, name);

            Assert.Equal(60, first.Template.Name.Length);
            Assert.True(second.Ok);
            Assert.True(second.Template.Name.Length <= 60);
            Assert.EndsWith(" (2)", second.Template.Name);
        }

        [Fact]
        public void Import_InvalidJson_Fails()
        {
            var result = NewRepo().Import("no es json", "x");

            Assert.False(result.Ok);
            Assert.StartsWith("JSON inválido", Errors(result));
        }

        [Fact]
        public void Mutation_WhenSaveThrows_StateUnchanged()
        {
            var repo = NewRepo();
            var before = repo.GetAll();
            var raised = 0;
            repo.Changed += (s, e) => raised++;

            TemplateSaveResult result;
            using (new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                result = repo.Create(Draft("No se guarda"));
            }

            Assert.False(result.Ok);
            Assert.StartsWith("No se pudo guardar: ", Errors(result));
            Assert.Equal(before.Select(t => t.Id), repo.GetAll().Select(t => t.Id));
            Assert.Equal(0, raised);
            Assert.False(File.Exists(_file + ".tmp"));

            // Una vez liberado el archivo la misma operación funciona.
            Assert.True(repo.Create(Draft("No se guarda")).Ok);
        }

        [Fact]
        public void ReadOnly_FutureSchema_RejectsEveryMutation()
        {
            File.WriteAllText(_file, "{\"schemaVersion\":99,\"templates\":[]}");
            var repo = NewRepo();
            const string message = "El archivo de plantillas fue creado por una versión más reciente de S3Tools.";

            Assert.True(repo.IsReadOnly);
            Assert.Equal(StoreReadOnlyReason.FutureSchema, repo.ReadOnlyReason);
            Assert.NotNull(repo.LoadWarning);
            Assert.Equal(message, Errors(repo.Create(Draft("A"))));
            Assert.Equal(message, Errors(repo.Update("tiendas", Draft("A"))));
            Assert.Equal(message, Errors(repo.Delete("tiendas")));
            Assert.Equal(message, Errors(repo.Duplicate("tiendas")));
            Assert.Equal(message, Errors(repo.Import(ValidSitemap, "A")));
            Assert.Equal(message, Errors(repo.RestoreDefaults()));
            Assert.Equal("{\"schemaVersion\":99,\"templates\":[]}", File.ReadAllText(_file));
        }

        [Fact]
        public void ReadOnly_IoErrorOnLoad_ReportsIoCause()
        {
            NewRepo().GetAll();
            var before = File.ReadAllText(_file);
            SitemapTemplateRepository repo;
            TemplateSaveResult result;
            using (new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                repo = NewRepo();
                Assert.True(repo.IsReadOnly);
                result = repo.Create(Draft("No debe guardarse"));
            }

            Assert.Equal(StoreReadOnlyReason.IoError, repo.ReadOnlyReason);
            Assert.False(result.Ok);
            Assert.Equal("No se pudo leer el archivo de plantillas. Los cambios están deshabilitados hasta reiniciar S3Tools.", Errors(result));
            Assert.Equal(before, File.ReadAllText(_file));
        }

        [Fact]
        public void RestoreDefaults_OnFreshDefaults_DoesNotDuplicateStores()
        {
            var repo = NewRepo();

            var result = repo.RestoreDefaults();

            Assert.True(result.Ok);
            Assert.Equal(StoreCatalog.StoresLeft, repo.Find("tiendas").Stores);
            Assert.Equal(StoreCatalog.StoresRight, repo.Find("bbvs").Stores);

            var reloaded = NewRepo();
            Assert.Null(reloaded.LoadWarning);
            Assert.Equal(StoreCatalog.StoresLeft, reloaded.Find("tiendas").Stores);
            Assert.Equal(StoreCatalog.StoresRight, reloaded.Find("bbvs").Stores);
        }

        [Fact]
        public void LoadWarning_ExposedWhenFileCorrupt()
        {
            File.WriteAllText(_file, "esto no es json");

            var repo = NewRepo();

            Assert.NotNull(repo.LoadWarning);
            Assert.False(repo.IsReadOnly);
            Assert.Equal(2, repo.GetAll().Count);
        }

        [Fact]
        public void GetAll_ReturnsSnapshotCopy()
        {
            var repo = NewRepo();
            var snapshot = repo.GetAll();

            repo.Create(Draft("Nueva"));

            Assert.Equal(2, snapshot.Count);
            Assert.Equal(3, repo.GetAll().Count);
        }

        [Fact]
        public void Find_UnknownOrNull_ReturnsNull()
        {
            var repo = NewRepo();

            Assert.Null(repo.Find("noexiste"));
            Assert.Null(repo.Find(null));
            Assert.Null(repo.FindByStore("NoExiste"));
            Assert.Null(repo.FindByStore(null));
            Assert.Equal("tiendas", repo.FindByStore("productostx").Id);
        }

        [Fact]
        public void ParallelCreates_AreSerializedAndAllPersisted()
        {
            var repo = NewRepo();
            var results = new TemplateSaveResult[20];

            Parallel.For(0, results.Length, i => results[i] = repo.Create(Draft("Paralela " + i)));

            Assert.All(results, r => Assert.True(r.Ok, Errors(r)));
            Assert.Equal(22, repo.GetAll().Count);
            Assert.Equal(22, NewRepo().GetAll().Count);
        }
    }
}
