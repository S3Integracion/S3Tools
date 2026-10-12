using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Xunit;

namespace S3Tools.Tests
{
    /// <summary>
    /// Verifica <see cref="SitemapJsonBuilder.BuildSitemapPayload"/>: paridad semántica con la
    /// salida del constructor anterior (fixtures) y robustez ante caracteres especiales.
    /// Las plantillas se leen directamente de <c>src/S3Tools/Resources</c>.
    /// </summary>
    public class SitemapJsonBuilderTests
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

            Assert.True(
                JsonNode.DeepEquals(JsonNode.Parse(payload), JsonNode.Parse(ReadFixture(fixtureFile))),
                "El resultado debe ser semánticamente igual a la salida de referencia del constructor anterior.");
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

        [Fact]
        public void BuildSitemapPayload_TemplateWithoutIdAndStartUrl_AddsBothAtTheStart()
        {
            var template = "{\"selectors\":[{\"id\":\"s\",\"type\":\"SelectorText\"}]}";

            var payload = SitemapJsonBuilder.BuildSitemapPayload(template, Title, Urls);

            var result = JsonNode.Parse(payload).AsObject();
            Assert.Equal(Title, (string)result["_id"]);
            Assert.Equal(Urls.Length, result["startUrl"].AsArray().Count);
            Assert.Equal(new[] { "_id", "startUrl", "selectors" }, result.Select(p => p.Key).ToArray());
        }

        [Fact]
        public void BuildSitemapPayload_StartUrlAsString_IsReplacedByArray()
        {
            var template = "{\"_id\":\"x\",\"startUrl\":\"https://old\",\"selectors\":[{\"id\":\"s\",\"type\":\"SelectorText\"}]}";

            var payload = SitemapJsonBuilder.BuildSitemapPayload(template, Title, Urls);

            var result = JsonNode.Parse(payload);
            var startUrl = Assert.IsType<JsonArray>(result["startUrl"]);
            Assert.Equal(Urls.Length, startUrl.Count);
            Assert.Equal(Urls[0], (string)startUrl[0]);
        }

        [Fact]
        public void BuildSitemapPayload_UrlWithQuoteBackslashTabAndUnicode_RoundTripsIdentically()
        {
            var tricky = "https://x.com/a\"b\\c\td/ñandú?q=1";
            var template = ReadTemplate("PlantillaSitemapsTiendas.json");

            var payload = SitemapJsonBuilder.BuildSitemapPayload(template, "t\"\\ñ", new[] { tricky });

            var result = JsonNode.Parse(payload);
            Assert.Equal(tricky, (string)result["startUrl"][0]);
            Assert.Equal("t\"\\ñ", (string)result["_id"]);
        }

        [Fact]
        public void BuildSitemapPayload_Ampersand_IsEmittedLiterally()
        {
            var payload = SitemapJsonBuilder.BuildSitemapPayload(
                ReadTemplate("PlantillaSitemapsTiendas.json"), Title, Urls);

            Assert.Contains("th=1&aod=1", payload);
            Assert.DoesNotContain("\\u0026", payload);
        }

        [Fact]
        public void BuildSitemapPayload_OutputIsSingleLine()
        {
            var payload = SitemapJsonBuilder.BuildSitemapPayload(
                ReadTemplate("PlantillaSitemapsTiendas.json"), Title, Urls);

            Assert.DoesNotContain("\n", payload);
        }

        [Fact]
        public void BuildSitemapPayload_TolerantTemplate_BomCommentsTrailingCommas()
        {
            var template = "﻿{\n// c\n\"_id\":\"x\",\"startUrl\":[],\"selectors\":[{\"id\":\"s\",\"type\":\"SelectorText\",},],}";

            var payload = SitemapJsonBuilder.BuildSitemapPayload(template, Title, Urls);

            Assert.Equal(Title, (string)JsonNode.Parse(payload)["_id"]);
        }
    }
}
