using Xunit;

namespace S3Tools.Tests
{
    /// <summary>
    /// Fija el comportamiento actual de la autodetección de plantilla en Formato.
    /// <see cref="TemplateResolver.DetectTemplate"/> resuelve las plantillas por nombre de archivo desde
    /// el directorio base / actual, que llegan al directorio de salida vía ProjectReference.
    /// </summary>
    public class TemplateResolverCharacterizationTests
    {
        [Fact]
        public void DetectTemplate_HeadersFromBbvs_ReturnsBbvs()
        {
            var headers = new[] { "web_scraper_order", "web_scraper_start_url", "precio_bb", "vendedor", "shipping" };

            Assert.Equal("bbvs", TemplateResolver.DetectTemplate(headers));
        }

        [Fact]
        public void DetectTemplate_HeadersFromTiendas_ReturnsTiendas()
        {
            var headers = new[] { "costo_detailpage", "shipsfrom_detailpage" };

            Assert.Equal("tiendas", TemplateResolver.DetectTemplate(headers));
        }

        [Fact]
        public void DetectTemplate_NoMatch_ReturnsTiendas()
        {
            var headers = new[] { "foo", "bar" };

            Assert.Equal("tiendas", TemplateResolver.DetectTemplate(headers));
        }
    }
}
