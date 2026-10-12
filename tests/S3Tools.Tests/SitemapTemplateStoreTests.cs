using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Xunit;

namespace S3Tools.Tests
{
    public class SitemapTemplateStoreTests : IDisposable
    {
        private const string ValidSitemap =
            "{\"_id\":\"x\",\"startUrl\":[\"https://a.com\"],\"selectors\":[" +
            "{\"id\":\"title\",\"type\":\"SelectorText\",\"parentSelectors\":[\"_root\"],\"selector\":\"h1\",\"multiple\":false}]}";

        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        private readonly string _dir;
        private readonly string _file;

        public SitemapTemplateStoreTests()
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

        private SitemapTemplateStore NewStore()
        {
            return new SitemapTemplateStore(_file);
        }

        private static string Entry(string id, string name, string stores = "[]", string sitemap = ValidSitemap, string builtInKey = null)
        {
            return "{\"id\":\"" + id + "\",\"name\":\"" + name + "\",\"description\":\"d\",\"builtInKey\":" +
                   (builtInKey == null ? "null" : "\"" + builtInKey + "\"") +
                   ",\"stores\":" + stores +
                   ",\"createdUtc\":\"2026-10-12T00:00:00Z\",\"updatedUtc\":\"2026-10-12T00:00:00Z\",\"sitemap\":" + sitemap + "}";
        }

        private static void WriteFile(string path, int schemaVersion, params string[] entries)
        {
            File.WriteAllText(
                path,
                "{\"schemaVersion\":" + schemaVersion + ",\"templates\":[" + string.Join(",", entries) + "]}",
                Utf8NoBom);
        }

        [Fact]
        public void Load_NoFile_SeedsDefaultsAndPersists()
        {
            var result = NewStore().Load();

            Assert.Null(result.Warning);
            Assert.False(result.IsReadOnly);
            Assert.Equal(new[] { "tiendas", "bbvs" }, result.Templates.Select(t => t.Id).ToArray());
            Assert.Equal("Tiendas (Normal)", result.Templates[0].Name);
            Assert.Equal("BBvs (Nube)", result.Templates[1].Name);
            Assert.True(File.Exists(_file));
            Assert.Equal(2, NewStore().Load().Templates.Count);
        }

        [Fact]
        public void Load_NoFile_CreatesMissingDirectory()
        {
            var nested = Path.Combine(_dir, "a", "b", "sitemap-templates.json");

            var result = new SitemapTemplateStore(nested).Load();

            Assert.Equal(2, result.Templates.Count);
            Assert.True(File.Exists(nested));
        }

        [Fact]
        public void SaveThenLoad_RoundTripsAllFields()
        {
            var created = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc).AddTicks(1234567);
            var updated = new DateTime(2026, 6, 7, 8, 9, 10, DateTimeKind.Utc);
            var original = new SitemapTemplate(
                "abc123", "Mi plantilla ñ & <b>", "Descripción", null,
                new[] { "BBvs_Template", "BBvsBB2" }, created, updated,
                "{ \"_id\": \"a&b\", \"startUrl\": [\"https://a.com/?x=1&y=2\"], \"selectors\": [" +
                "{\"id\":\"t\",\"type\":\"SelectorText\",\"parentSelectors\":[\"_root\"],\"selector\":\"h1\",\"multiple\":false}] }");
            var builtIn = BuiltInSitemapTemplates.CreateDefaults(created)[0];
            var store = NewStore();

            store.Save(new[] { builtIn, original });
            var loaded = store.Load();

            Assert.Null(loaded.Warning);
            Assert.Equal(2, loaded.Templates.Count);
            var t = loaded.Templates.Single(x => x.Id == "abc123");
            Assert.Equal(original.Name, t.Name);
            Assert.Equal(original.Description, t.Description);
            Assert.Null(t.BuiltInKey);
            Assert.False(t.IsBuiltIn);
            Assert.Equal(original.Stores, t.Stores);
            Assert.Equal(created, t.CreatedUtc);
            Assert.Equal(DateTimeKind.Utc, t.CreatedUtc.Kind);
            Assert.Equal(updated, t.UpdatedUtc);
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(original.SitemapJson), JsonNode.Parse(t.SitemapJson)));
            var b = loaded.Templates.Single(x => x.Id == "tiendas");
            Assert.True(b.IsBuiltIn);
            Assert.Equal("tiendas", b.BuiltInKey);
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(builtIn.SitemapJson), JsonNode.Parse(b.SitemapJson)));
        }

        [Fact]
        public void Save_PersistsSitemapAsJsonObject()
        {
            NewStore().Save(BuiltInSitemapTemplates.CreateDefaults(DateTime.UtcNow));

            var root = JsonNode.Parse(File.ReadAllText(_file, Utf8NoBom));
            Assert.Equal(1, root["schemaVersion"].GetValue<int>());
            Assert.IsType<JsonObject>(root["templates"][0]["sitemap"]);
        }

        [Fact]
        public void Save_WritesUtf8WithoutBom_AndLeavesNoTmp()
        {
            var store = NewStore();
            var templates = BuiltInSitemapTemplates.CreateDefaults(DateTime.UtcNow);

            store.Save(templates);
            store.Save(templates);

            var bytes = File.ReadAllBytes(_file);
            Assert.False(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
            Assert.False(File.Exists(_file + ".tmp"));
            Assert.True(File.Exists(_file + ".bak"));
            Assert.Contains("\n", Utf8NoBom.GetString(bytes));
        }

        [Fact]
        public void Load_CorruptMain_ValidBak_RecoversFromBak()
        {
            WriteFile(_file + ".bak", 1, Entry("good", "Buena"));
            File.WriteAllText(_file, "{ esto no es json", Utf8NoBom);
            var store = NewStore();

            var result = store.Load();

            Assert.NotNull(result.Warning);
            Assert.False(result.IsReadOnly);
            Assert.Equal(new[] { "good" }, result.Templates.Select(t => t.Id).ToArray());

            // El contenido recuperado se vuelve a guardar como archivo principal.
            var again = NewStore().Load();
            Assert.Null(again.Warning);
            Assert.Equal(new[] { "good" }, again.Templates.Select(t => t.Id).ToArray());
        }

        [Fact]
        public void Load_MainMissing_ValidBak_RecoversFromBak()
        {
            WriteFile(_file + ".bak", 1, Entry("good", "Buena"));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Equal("good", Assert.Single(result.Templates).Id);
            Assert.True(File.Exists(_file));
        }

        [Fact]
        public void Load_CorruptMainAndBak_RenamesCorruptAndReseeds()
        {
            File.WriteAllText(_file, "basura", Utf8NoBom);
            File.WriteAllText(_file + ".bak", "{ tambien basura", Utf8NoBom);

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Single(Directory.GetFiles(_dir, "sitemap-templates.corrupt-*.json"));
            Assert.Equal(new[] { "tiendas", "bbvs" }, result.Templates.Select(t => t.Id).ToArray());
            Assert.Equal(2, NewStore().Load().Templates.Count);
        }

        [Fact]
        public void Load_CorruptMain_NoBak_RenamesCorruptAndReseeds()
        {
            File.WriteAllText(_file, "[]", Utf8NoBom);

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Single(Directory.GetFiles(_dir, "sitemap-templates.corrupt-*.json"));
            Assert.Equal(2, result.Templates.Count);
        }

        [Fact]
        public void Load_EntryWithInvalidSitemap_IsDroppedWithWarning()
        {
            WriteFile(_file, 1, Entry("good", "Buena"), Entry("bad", "Mala", sitemap: "{\"selectors\":[]}"));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Equal("good", Assert.Single(result.Templates).Id);
        }

        [Theory]
        [InlineData("BAD")]
        [InlineData("has-dash")]
        [InlineData("")]
        public void Load_EntryWithInvalidId_IsDropped(string badId)
        {
            WriteFile(_file, 1, Entry("good", "Buena"), Entry(badId, "Otra"));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Equal("good", Assert.Single(result.Templates).Id);
        }

        [Fact]
        public void Load_EntryWithEmptyName_IsDropped()
        {
            WriteFile(_file, 1, Entry("good", "Buena"), Entry("noname", "   "));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Equal("good", Assert.Single(result.Templates).Id);
        }

        [Fact]
        public void Load_DuplicateIds_KeepsFirst()
        {
            WriteFile(_file, 1, Entry("same", "Primera"), Entry("same", "Segunda"));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Equal("Primera", Assert.Single(result.Templates).Name);
        }

        [Fact]
        public void Load_DuplicateNames_SecondGetsSuffix()
        {
            WriteFile(_file, 1, Entry("a", "X"), Entry("b", "x"), Entry("c", "X"));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Equal(new[] { "X", "x (2)", "X (3)" }, result.Templates.Select(t => t.Name).ToArray());
        }

        [Fact]
        public void Load_StoreLinkedTwice_KeepsFirst()
        {
            WriteFile(_file, 1,
                Entry("a", "A", stores: "[\"Altinor\",\"ProductosTX\"]"),
                Entry("b", "B", stores: "[\"altinor\",\"HervazTrade\"]"));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Equal(new[] { "Altinor", "ProductosTX" }, result.Templates[0].Stores.ToArray());
            Assert.Equal(new[] { "HervazTrade" }, result.Templates[1].Stores.ToArray());
        }

        [Fact]
        public void Load_UnknownBuiltInKey_IsTreatedAsNull()
        {
            WriteFile(_file, 1, Entry("a", "A", builtInKey: "otra"));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Null(Assert.Single(result.Templates).BuiltInKey);
        }

        [Fact]
        public void Load_AllEntriesInvalid_Reseeds()
        {
            WriteFile(_file, 1, Entry("bad", "Mala", sitemap: "{}"), Entry("BAD", "Peor"));

            var result = NewStore().Load();

            Assert.NotNull(result.Warning);
            Assert.Equal(new[] { "tiendas", "bbvs" }, result.Templates.Select(t => t.Id).ToArray());
            Assert.Null(NewStore().Load().Warning);
        }

        [Fact]
        public void Load_FutureSchemaVersion_IsReadOnlyAndFileUntouched()
        {
            WriteFile(_file, 2, Entry("fut", "Futura"), Entry("bad", "Mala", sitemap: "{}"));
            var before = File.ReadAllBytes(_file);

            var result = NewStore().Load();

            Assert.True(result.IsReadOnly);
            Assert.NotNull(result.Warning);
            Assert.Equal("fut", Assert.Single(result.Templates).Id);
            Assert.Equal(before, File.ReadAllBytes(_file));
            Assert.False(File.Exists(_file + ".bak"));
            Assert.False(File.Exists(_file + ".tmp"));
        }

        [Fact]
        public void Load_FutureSchemaVersion_Unparseable_ReturnsDefaultsInMemory()
        {
            File.WriteAllText(_file, "{\"schemaVersion\":9,\"templates\":\"raro\"}", Utf8NoBom);
            var before = File.ReadAllBytes(_file);

            var result = NewStore().Load();

            Assert.True(result.IsReadOnly);
            Assert.NotNull(result.Warning);
            Assert.Equal(2, result.Templates.Count);
            Assert.Equal(before, File.ReadAllBytes(_file));
            Assert.Empty(Directory.GetFiles(_dir, "sitemap-templates.corrupt-*.json"));
        }

        [Fact]
        public void Load_TolerantOfBomCommentsAndTrailingCommas()
        {
            var text = "﻿// editado a mano\n{\"schemaVersion\":1,\"templates\":[" + Entry("a", "A") + ",]}";
            File.WriteAllText(_file, text, Utf8NoBom);

            var result = NewStore().Load();

            Assert.Null(result.Warning);
            Assert.Equal("a", Assert.Single(result.Templates).Id);
        }

        [Theory]
        [InlineData("tiendas")]
        [InlineData("bbvs")]
        public void BuiltIns_ResourcesExistAndValidate(string key)
        {
            var json = BuiltInSitemapTemplates.GetSitemapJson(key);

            var validation = SitemapTemplateValidator.Validate(json);

            Assert.True(validation.IsValid);
        }

        [Fact]
        public void BuiltIns_CreateDefaults_UsesStoreCatalog()
        {
            var defaults = BuiltInSitemapTemplates.CreateDefaults(DateTime.UtcNow);

            Assert.Equal(StoreCatalog.StoresLeft, defaults.Single(t => t.Id == "tiendas").Stores.ToArray());
            Assert.Equal(StoreCatalog.StoresRight, defaults.Single(t => t.Id == "bbvs").Stores.ToArray());
            Assert.All(defaults, t => Assert.True(t.IsBuiltIn));
        }

        [Fact]
        public void BuiltIns_GetSitemapJson_UnknownKey_Throws()
        {
            Assert.Throws<ArgumentException>(() => BuiltInSitemapTemplates.GetSitemapJson("nope"));
        }
    }
}
