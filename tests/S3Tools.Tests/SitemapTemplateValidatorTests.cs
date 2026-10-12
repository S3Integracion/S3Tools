using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Xunit;

namespace S3Tools.Tests
{
    public class SitemapTemplateValidatorTests
    {
        private static string ReadTemplate(string fileName)
        {
            return File.ReadAllText(TestPaths.Resource(fileName), new UTF8Encoding(false));
        }

        private static int CountSelectors(string text)
        {
            return JsonNode.Parse(text)["selectors"].AsArray().Count;
        }

        private static int Count(TemplateValidationResult result, TemplateIssueSeverity severity)
        {
            return result.Issues.Count(i => i.Severity == severity);
        }

        [Theory]
        [InlineData("PlantillaSitemapsTiendas.json")]
        [InlineData("PlantillaSitemapsBBvs.json")]
        public void Validate_FactoryTemplates_AreValid(string fileName)
        {
            var text = ReadTemplate(fileName);

            var result = SitemapTemplateValidator.Validate(text);

            Assert.True(result.IsValid, string.Join("; ", result.Issues.Select(i => i.Message)));
            Assert.Equal(CountSelectors(text), result.Selectors.Count);
            Assert.NotNull(result.Sitemap);
        }

        [Fact]
        public void Validate_SelectorSummary_ExposesFields()
        {
            var json = "{\"_id\":\"a\",\"startUrl\":[],\"selectors\":["
                + "{\"id\":\"root\",\"type\":\"SelectorText\",\"selector\":\"h1\",\"multiple\":true,\"parentSelectors\":[\"_root\"]},"
                + "{\"id\":\"child\",\"type\":\"SelectorText\",\"selector\":\"p\",\"multiple\":false,\"parentSelectors\":[\"root\"]}]}";

            var result = SitemapTemplateValidator.Validate(json);

            Assert.True(result.IsValid);
            var first = result.Selectors[0];
            Assert.Equal("root", first.Id);
            Assert.Equal("SelectorText", first.Type);
            Assert.Equal("h1", first.Selector);
            Assert.True(first.Multiple);
            Assert.Equal(new[] { "_root" }, first.ParentSelectors);
            Assert.Equal(new[] { "root" }, result.Selectors[1].ParentSelectors);
            Assert.Equal(0, Count(result, TemplateIssueSeverity.Warning));
        }

        [Fact]
        public void Validate_Empty_IsError()
        {
            Assert.False(SitemapTemplateValidator.Validate("").IsValid);
            Assert.False(SitemapTemplateValidator.Validate("   \n ").IsValid);
            Assert.False(SitemapTemplateValidator.Validate(null).IsValid);
            Assert.Null(SitemapTemplateValidator.Validate("").Sitemap);
        }

        [Fact]
        public void Validate_OpenBrace_IsErrorAtLineOne()
        {
            var result = SitemapTemplateValidator.Validate("{");

            Assert.False(result.IsValid);
            var error = Assert.Single(result.Issues, i => i.Severity == TemplateIssueSeverity.Error);
            Assert.Equal(1, error.Line);
            Assert.StartsWith("JSON inválido: ", error.Message);
            Assert.Null(result.Sitemap);
        }

        [Fact]
        public void Validate_InvalidJsonOnLaterLine_ReportsLineAndColumn()
        {
            var result = SitemapTemplateValidator.Validate("{\n  \"selectors\": [ x ]\n}");

            var error = Assert.Single(result.Issues);
            Assert.Equal(2, error.Line);
            Assert.True(error.Column >= 1);
        }

        [Fact]
        public void Validate_RootArray_IsError()
        {
            var result = SitemapTemplateValidator.Validate("[]");

            Assert.False(result.IsValid);
            Assert.Null(result.Sitemap);
        }

        [Theory]
        [InlineData("{\"selectors\":[]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[],\"selectors\":{}}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[],\"selectors\":[1]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[],\"selectors\":[{\"id\":\"\",\"type\":\"SelectorText\"}]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[],\"selectors\":[{\"id\":5,\"type\":\"SelectorText\"}]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[],\"selectors\":[{\"id\":\"x\",\"type\":\"\"}]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[],\"selectors\":[{\"id\":\"x\"}]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[],\"selectors\":[{\"id\":\"x\",\"type\":\"SelectorText\",\"parentSelectors\":\"_root\"}]}")]
        [InlineData("{\"_id\":5,\"startUrl\":[],\"selectors\":[{\"id\":\"x\",\"type\":\"SelectorText\"}]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":5,\"selectors\":[{\"id\":\"x\",\"type\":\"SelectorText\"}]}")]
        [InlineData("{\"_id\":\"a\",\"startUrl\":[1],\"selectors\":[{\"id\":\"x\",\"type\":\"SelectorText\"}]}")]
        public void Validate_StructuralProblems_AreErrors(string json)
        {
            var result = SitemapTemplateValidator.Validate(json);

            Assert.False(result.IsValid);
            Assert.True(Count(result, TemplateIssueSeverity.Error) >= 1);
        }

        [Fact]
        public void Validate_DuplicateSelectorId_IsErrorMentioningId()
        {
            var json = "{\"_id\":\"a\",\"startUrl\":[],\"selectors\":["
                + "{\"id\":\"dup\",\"type\":\"SelectorText\"},{\"id\":\"dup\",\"type\":\"SelectorText\"}]}";

            var result = SitemapTemplateValidator.Validate(json);

            Assert.False(result.IsValid);
            Assert.Contains(result.Issues, i => i.Severity == TemplateIssueSeverity.Error && i.Message.Contains("dup"));
        }

        [Fact]
        public void Validate_SelectorIdsDifferingByCase_AreNotDuplicates()
        {
            var json = "{\"_id\":\"a\",\"startUrl\":[],\"selectors\":["
                + "{\"id\":\"Dup\",\"type\":\"SelectorText\"},{\"id\":\"dup\",\"type\":\"SelectorText\"}]}";

            Assert.True(SitemapTemplateValidator.Validate(json).IsValid);
        }

        [Fact]
        public void Validate_BomCommentAndTrailingComma_IsValid()
        {
            var json = "﻿{\n// x\n\"_id\":\"a\",\"startUrl\":[\"u\",],\"selectors\":["
                + "{\"id\":\"s\",\"type\":\"SelectorText\",},],}";

            var result = SitemapTemplateValidator.Validate(json);

            Assert.True(result.IsValid, string.Join("; ", result.Issues.Select(i => i.Message)));
            Assert.Single(result.Selectors);
        }

        [Fact]
        public void Validate_MissingIdAndStartUrl_IsValidWithTwoWarnings()
        {
            var json = "{\"selectors\":[{\"id\":\"s\",\"type\":\"SelectorText\"}]}";

            var result = SitemapTemplateValidator.Validate(json);

            Assert.True(result.IsValid);
            Assert.Equal(2, Count(result, TemplateIssueSeverity.Warning));
            Assert.Equal(0, Count(result, TemplateIssueSeverity.Error));
        }

        [Fact]
        public void Validate_StartUrlAsString_IsAccepted()
        {
            var json = "{\"_id\":\"a\",\"startUrl\":\"https://x\",\"selectors\":[{\"id\":\"s\",\"type\":\"SelectorText\"}]}";

            var result = SitemapTemplateValidator.Validate(json);

            Assert.True(result.IsValid);
            Assert.Equal(0, Count(result, TemplateIssueSeverity.Warning));
        }

        [Fact]
        public void Validate_UnknownParentSelector_IsWarning()
        {
            var json = "{\"_id\":\"a\",\"startUrl\":[],\"selectors\":["
                + "{\"id\":\"s\",\"type\":\"SelectorText\",\"parentSelectors\":[\"noexiste\"]}]}";

            var result = SitemapTemplateValidator.Validate(json);

            Assert.True(result.IsValid);
            Assert.Contains(result.Issues, i => i.Severity == TemplateIssueSeverity.Warning && i.Message.Contains("noexiste"));
        }

        [Fact]
        public void Validate_OnlyElementClickSelectors_WarnsAboutAutodetection()
        {
            var json = "{\"_id\":\"a\",\"startUrl\":[],\"selectors\":["
                + "{\"id\":\"c\",\"type\":\"SelectorElementClick\",\"parentSelectors\":[\"_root\"]}]}";

            var result = SitemapTemplateValidator.Validate(json);

            Assert.True(result.IsValid);
            Assert.Equal(1, Count(result, TemplateIssueSeverity.Warning));
        }

        [Fact]
        public void Validate_TextLongerThanMax_IsError()
        {
            var text = new string(' ', SitemapTemplateValidator.MaxTemplateChars + 1);

            var result = SitemapTemplateValidator.Validate(text);

            Assert.False(result.IsValid);
            Assert.Null(result.Sitemap);
        }

        [Fact]
        public void Validate_DuplicatePropertyNames_DoesNotThrow()
        {
            var result = SitemapTemplateValidator.Validate("{\"selectors\":[],\"selectors\":[]}");

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Normalize_ProducesIndentedJsonThatRoundTrips()
        {
            var text = ReadTemplate("PlantillaSitemapsTiendas.json");
            var sitemap = SitemapTemplateValidator.Validate(text).Sitemap;

            var normalized = SitemapTemplateValidator.Normalize(sitemap);

            Assert.Contains("\n", normalized);
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(text), JsonNode.Parse(normalized)));
            Assert.True(SitemapTemplateValidator.Validate(normalized).IsValid);
        }

        [Fact]
        public void Normalize_KeepsUnicodeAndAmpersandLiteral()
        {
            var sitemap = (JsonObject)JsonNode.Parse("{\"selectors\":[{\"id\":\"ñ&\",\"type\":\"T\"}]}");

            var normalized = SitemapTemplateValidator.Normalize(sitemap);

            Assert.Contains("ñ&", normalized);
        }
    }
}
