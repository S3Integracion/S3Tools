using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Xunit;

namespace S3Tools.Tests
{
    /// <summary>Verifica que <see cref="SitemapEngine"/> lea las plantillas del repositorio y no lance excepciones.</summary>
    public class SitemapEngineTests : IDisposable
    {
        private const string DeletedMessage = "La plantilla seleccionada ya no existe. Elige otra en la sección Plantilla.";

        private const string CustomSitemap =
            "{\"_id\":\"x\",\"startUrl\":[],\"selectors\":[" +
            "{\"id\":\"mi_selector_unico\",\"type\":\"SelectorText\",\"parentSelectors\":[\"_root\"],\"selector\":\"span.unico\",\"multiple\":false}]}";

        private readonly string _dir;
        private readonly SitemapTemplateRepository _repo;

        public SitemapEngineTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "S3ToolsTests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            _repo = new SitemapTemplateRepository(Path.Combine(_dir, "sitemap-templates.json"));
        }

        public void Dispose()
        {
            if (Directory.Exists(_dir))
            {
                Directory.Delete(_dir, true);
            }
        }

        private string WriteUrlsFile()
        {
            var path = Path.Combine(_dir, "urls.txt");
            File.WriteAllText(path, "https://www.amazon.com/dp/B0001ZZ5SG\nhttps://www.amazon.com/dp/0805470859\n", new UTF8Encoding(false));
            return path;
        }

        private SitemapEngineRequest Request(string templateId, string outputDir = null)
        {
            return new SitemapEngineRequest
            {
                Action = "process",
                InputFiles = new[] { WriteUrlsFile() },
                OutputDir = outputDir ?? Path.Combine(_dir, "out"),
                StoreName = "TiendaX",
                Store = "TiendaX",
                ZipOutput = false,
                TemplateId = templateId,
            };
        }

        [Fact]
        public void SitemapEngine_WithCustomTemplate_WritesItsSelectors()
        {
            var created = _repo.Create(new SitemapTemplateDraft { Name = "Mia", SitemapJson = CustomSitemap, Stores = new string[0] });
            Assert.True(created.Ok);

            var response = SitemapEngine.Handle(Request(created.Template.Id), _repo);

            Assert.True(response.Ok, response.Error);
            var file = Assert.Single(response.OutputFiles);
            var json = JsonNode.Parse(File.ReadAllText(file, new UTF8Encoding(false)));
            Assert.Equal("TiendaX", (string)json["_id"]);
            Assert.Equal(2, json["startUrl"].AsArray().Count);
            var selectorIds = json["selectors"].AsArray().Select(s => (string)s["id"]).ToArray();
            Assert.Equal(new[] { "mi_selector_unico" }, selectorIds);
        }

        [Fact]
        public void SitemapEngine_FactoryTemplate_WritesFactorySelectors()
        {
            var response = SitemapEngine.Handle(Request("bbvs"), _repo);

            Assert.True(response.Ok, response.Error);
            var json = JsonNode.Parse(File.ReadAllText(response.OutputFiles.Single(), new UTF8Encoding(false)));
            Assert.Contains("precio_bb", json["selectors"].AsArray().Select(s => (string)s["id"]));
        }

        [Theory]
        [InlineData("noexiste")]
        [InlineData("")]
        [InlineData(null)]
        public void SitemapEngine_DeletedTemplateId_ReturnsError(string templateId)
        {
            var response = SitemapEngine.Handle(Request(templateId), _repo);

            Assert.False(response.Ok);
            Assert.Equal(DeletedMessage, response.Error);
            Assert.False(Directory.Exists(Path.Combine(_dir, "out")));
        }

        [Fact]
        public void SitemapEngine_TemplateDeletedAfterCreate_ReturnsError()
        {
            var created = _repo.Create(new SitemapTemplateDraft { Name = "Temporal", SitemapJson = CustomSitemap, Stores = new string[0] });
            Assert.True(_repo.Delete(created.Template.Id).Ok);

            var response = SitemapEngine.Handle(Request(created.Template.Id), _repo);

            Assert.False(response.Ok);
            Assert.Equal(DeletedMessage, response.Error);
        }

        [Fact]
        public void SitemapEngine_OutputDirIsAFile_ReturnsErrorInsteadOfThrowing()
        {
            var blocker = Path.Combine(_dir, "blocker");
            File.WriteAllText(blocker, "x");

            var response = SitemapEngine.Handle(Request("tiendas", blocker), _repo);

            Assert.False(response.Ok);
            Assert.False(string.IsNullOrWhiteSpace(response.Error));
            Assert.Contains("Tiendas (Normal)", response.Error);
        }
    }
}
