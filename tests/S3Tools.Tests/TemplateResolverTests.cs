using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace S3Tools.Tests
{
    /// <summary>
    /// Verifica <see cref="TemplateResolver"/> contra listas de plantillas en memoria
    /// (las de fábrica embebidas más plantillas personalizadas); no depende de archivos en disco.
    /// </summary>
    public class TemplateResolverTests
    {
        private static readonly DateTime T0 = new DateTime(2026, 10, 12, 0, 0, 0, DateTimeKind.Utc);

        private static IReadOnlyList<SitemapTemplate> Defaults()
        {
            return BuiltInSitemapTemplates.CreateDefaults(T0);
        }

        private static string NewId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static SitemapTemplate Custom(string id, DateTime updatedUtc, params string[] selectorIds)
        {
            var selectors = string.Join(",", selectorIds.Select(
                s => "{\"id\":\"" + s + "\",\"type\":\"SelectorText\",\"parentSelectors\":[\"_root\"],\"selector\":\"h1\",\"multiple\":false}"));
            var json = "{\"_id\":\"x\",\"startUrl\":[],\"selectors\":[" + selectors + "]}";
            return new SitemapTemplate(id, "Custom " + id, string.Empty, null, new string[0], T0, updatedUtc, json);
        }

        private static IReadOnlyList<SitemapTemplate> With(params SitemapTemplate[] extra)
        {
            return Defaults().Concat(extra).ToArray();
        }

        [Fact]
        public void DetectTemplate_HeadersFromBbvs_ReturnsBbvs()
        {
            var headers = new[] { "web_scraper_order", "web_scraper_start_url", "precio_bb", "vendedor", "shipping" };

            Assert.Equal("bbvs", TemplateResolver.DetectTemplate(headers, Defaults()));
        }

        [Fact]
        public void DetectTemplate_HeadersFromTiendas_ReturnsTiendas()
        {
            var headers = new[] { "costo_detailpage", "shipsfrom_detailpage" };

            Assert.Equal("tiendas", TemplateResolver.DetectTemplate(headers, Defaults()));
        }

        [Fact]
        public void DetectTemplate_NoMatch_ReturnsTiendas()
        {
            var headers = new[] { "foo", "bar" };

            Assert.Equal("tiendas", TemplateResolver.DetectTemplate(headers, Defaults()));
        }

        [Fact]
        public void DetectTemplate_CustomTemplateWithUniqueSelectors_IsDetected()
        {
            var id = NewId();
            var templates = With(Custom(id, T0, "unique_alpha_" + id, "unique_beta_" + id));

            var detected = TemplateResolver.DetectTemplate(
                new[] { "web_scraper_order", "unique_alpha_" + id, "UNIQUE-BETA-" + id }, templates);

            Assert.Equal(id, detected);
        }

        [Fact]
        public void DetectTemplate_TieAboveZero_FirstInListWins()
        {
            var first = NewId();
            var second = NewId();
            var shared = "shared_" + first;
            var templates = new[]
            {
                Custom(first, T0, shared),
                Custom(second, T0, shared),
            };

            Assert.Equal(first, TemplateResolver.DetectTemplate(new[] { shared }, templates));
        }

        [Fact]
        public void DetectTemplate_AfterTemplateEdited_UsesNewSelectors()
        {
            var id = NewId();
            var alpha = "alpha_" + id;
            var beta = "beta_" + id;
            var before = With(Custom(id, T0, alpha));
            var after = With(Custom(id, T0.AddSeconds(1), beta));

            Assert.Equal(id, TemplateResolver.DetectTemplate(new[] { alpha }, before));

            // Mismo id, UpdatedUtc distinto: la caché no debe devolver los encabezados viejos.
            Assert.Equal("tiendas", TemplateResolver.DetectTemplate(new[] { alpha }, after));
            Assert.Equal(id, TemplateResolver.DetectTemplate(new[] { beta }, after));
        }

        [Fact]
        public void DetectTemplate_TiendasDeleted_FallsBackToFirst()
        {
            var id = NewId();
            var bbvs = Defaults().Single(t => t.Id == "bbvs");
            var templates = new[] { bbvs, Custom(id, T0, "only_" + id) };

            Assert.Equal("bbvs", TemplateResolver.DetectTemplate(new[] { "foo", "bar" }, templates));
        }

        [Fact]
        public void DetectTemplate_ElementClickSelectors_AreNotExpectedHeaders()
        {
            var id = NewId();
            var json = "{\"_id\":\"x\",\"startUrl\":[],\"selectors\":[" +
                       "{\"id\":\"click_" + id + "\",\"type\":\"SelectorElementClick\",\"parentSelectors\":[\"_root\"],\"selector\":\"a\",\"multiple\":false}]}";
            var template = new SitemapTemplate(id, "Click", string.Empty, null, new string[0], T0, T0, json);

            Assert.Equal("tiendas", TemplateResolver.DetectTemplate(new[] { "click_" + id }, With(template)));
        }

        [Fact]
        public void ResolveTemplate_Auto_Detects()
        {
            var headers = new[] { "precio_bb", "vendedor" };

            Assert.Equal("bbvs", TemplateResolver.ResolveTemplate("auto", headers, Defaults()));
        }

        [Fact]
        public void ResolveTemplate_ExplicitId_ReturnsThatId()
        {
            var headers = new[] { "precio_bb", "vendedor" };

            Assert.Equal("tiendas", TemplateResolver.ResolveTemplate("tiendas", headers, Defaults()));
        }

        [Fact]
        public void ResolveTemplate_UnknownId_Throws()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => TemplateResolver.ResolveTemplate("borrada", new[] { "foo" }, Defaults()));

            Assert.Equal("La plantilla seleccionada ya no existe.", ex.Message);
        }
    }
}
