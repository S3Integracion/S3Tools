using System;

namespace S3Tools
{
    /// <summary>
    /// Construye URLs de detalle de producto de Amazon (US / MX) a partir de un ASIN.
    /// </summary>
    internal static class AsinUrlBuilder
    {
        private const string UrlUs = "https://www.amazon.com/dp/";
        private const string UrlMx = "https://www.amazon.com.mx/dp/";
        private const string ThSuffix = "?th=1";
        private const string SellerOnOpenSuffix = "&aod=1";

        /// <summary>Construye la URL Amazon para un ASIN.</summary>
        /// <param name="asin">Identificador de producto (10 chars alfanuméricos).</param>
        /// <param name="market">"US" o "MX" (default MX).</param>
        /// <param name="showSellerOnOpen">Anexa el parámetro <c>aod=1</c> que abre el panel de vendedores.</param>
        public static string ToUrl(string asin, string market, bool showSellerOnOpen)
        {
            var baseUrl = string.Equals(market, "US", StringComparison.OrdinalIgnoreCase) ? UrlUs : UrlMx;
            var url = baseUrl + asin + ThSuffix;
            if (showSellerOnOpen)
            {
                url += SellerOnOpenSuffix;
            }
            return url;
        }
    }
}
