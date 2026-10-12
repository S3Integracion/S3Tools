using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using Xunit;

namespace S3Tools.Tests
{
    /// <summary>
    /// Fija el comportamiento actual de <see cref="SitemapJsonBuilder"/> antes de refactorizar.
    /// Las plantillas se leen directamente de <c>src/S3Tools/Resources</c>, equivalente a
    /// <see cref="SitemapJsonBuilder.LoadTemplateText"/> sin depender del directorio de salida.
    /// </summary>
    public class SitemapJsonBuilderCharacterizationTests
    {
        private const string Title = "ProductosTX_1";

        private static readonly string[] Urls =
        {
            "https://www.amazon.com.mx/dp/B0001ZZ5SG?th=1&aod=1",
            "https://www.amazon.com/dp/0805470859",
        };

        private static string ReadTemplate(string fileName)
        {
            return File.ReadAllText(TestPaths.Resource(fileName), new UTF8Encoding(false));
        }

        private static string ReadFixture(string fileName)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", fileName), new UTF8Encoding(false));
        }

        private static void AssertReplacesIdAndStartUrl(string templateFile)
        {
            var templateText = ReadTemplate(templateFile);

            var payload = SitemapJsonBuilder.BuildSitemapPayload(templateText, Title, Urls);

            var result = JsonNode.Parse(payload);
            var template = JsonNode.Parse(templateText);

            Assert.Equal(Title, (string)result["_id"]);

            var startUrl = result["startUrl"].AsArray();
            Assert.Equal(Urls.Length, startUrl.Count);
            for (var i = 0; i < Urls.Length; i++)
            {
                Assert.Equal(Urls[i], (string)startUrl[i]);
            }

            Assert.True(JsonNode.DeepEquals(template["selectors"], result["selectors"]),
                "Los selectores del resultado deben ser idénticos a los de la plantilla.");
        }

        private static void AssertMatchesFixture(string templateFile, string fixtureFile)
        {
            var payload = SitemapJsonBuilder.BuildSitemapPayload(ReadTemplate(templateFile), Title, Urls);

            Assert.Equal(ReadFixture(fixtureFile), payload);
        }

        [Fact]
        public void BuildSitemapPayload_ReplacesIdAndStartUrl_Tiendas()
        {
            AssertReplacesIdAndStartUrl("PlantillaSitemapsTiendas.json");
        }

        [Fact]
        public void BuildSitemapPayload_ReplacesIdAndStartUrl_Bbvs()
        {
            AssertReplacesIdAndStartUrl("PlantillaSitemapsBBvs.json");
        }

        [Fact]
        public void BuildSitemapPayload_MatchesReferenceFixture_Tiendas()
        {
            AssertMatchesFixture("PlantillaSitemapsTiendas.json", "expected-tiendas.json");
        }

        [Fact]
        public void BuildSitemapPayload_MatchesReferenceFixture_Bbvs()
        {
            AssertMatchesFixture("PlantillaSitemapsBBvs.json", "expected-bbvs.json");
        }
    }
}
