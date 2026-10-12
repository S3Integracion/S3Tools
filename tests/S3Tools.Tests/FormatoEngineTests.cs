using System;
using System.IO;
using System.Text;
using Xunit;

namespace S3Tools.Tests
{
    /// <summary>Verifica que <see cref="FormatoEngine"/> tome las plantillas del repositorio.</summary>
    public class FormatoEngineTests : IDisposable
    {
        private const string CustomSitemap =
            "{\"_id\":\"x\",\"startUrl\":[],\"selectors\":[" +
            "{\"id\":\"mi_columna_unica\",\"type\":\"SelectorText\",\"parentSelectors\":[\"_root\"],\"selector\":\"span\",\"multiple\":false}]}";

        private readonly string _dir;
        private readonly SitemapTemplateRepository _repo;

        public FormatoEngineTests()
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

        private string WriteCsv(string headerLine)
        {
            var path = Path.Combine(_dir, "datos.csv");
            File.WriteAllText(path, headerLine + "\n1,http://a,x\n", new UTF8Encoding(false));
            return path;
        }

        private static FormatoEngineRequest Request(string file, string template)
        {
            return new FormatoEngineRequest
            {
                Action = "process",
                InputFiles = new[] { file },
                Template = template,
                HeaderFormat = "underscore",
            };
        }

        [Fact]
        public void FormatoEngine_Auto_ReportsCountsByTemplateName()
        {
            var file = WriteCsv("web-scraper-order,web-scraper-start-url,precio_bb");

            var response = FormatoEngine.Handle(Request(file, "auto"), _repo);

            Assert.True(response.Ok, response.Error);
            Assert.Equal(1, response.TemplateCounts["BBvs (Nube)"]);
            Assert.StartsWith("web_scraper_order,web_scraper_start_url,precio_bb", File.ReadAllText(file));
        }

        [Fact]
        public void FormatoEngine_ExplicitCustomTemplate_ReportsItsName()
        {
            var created = _repo.Create(new SitemapTemplateDraft { Name = "Mi plantilla", SitemapJson = CustomSitemap, Stores = new string[0] });
            Assert.True(created.Ok);
            var file = WriteCsv("a,b,mi_columna_unica");

            var response = FormatoEngine.Handle(Request(file, created.Template.Id), _repo);

            Assert.True(response.Ok, response.Error);
            Assert.Equal(1, response.TemplateCounts["Mi plantilla"]);
        }

        [Fact]
        public void FormatoEngine_DeletedTemplateId_ReturnsExactMessageAndLeavesFileUntouched()
        {
            var created = _repo.Create(new SitemapTemplateDraft { Name = "Temporal", SitemapJson = CustomSitemap, Stores = new string[0] });
            Assert.True(_repo.Delete(created.Template.Id).Ok);
            var file = WriteCsv("a,b,c");

            var response = FormatoEngine.Handle(Request(file, created.Template.Id), _repo);

            Assert.False(response.Ok);
            Assert.Equal("La plantilla seleccionada ya no existe.", response.Error);
            Assert.StartsWith("a,b,c", File.ReadAllText(file));
        }
    }
}
