namespace S3Tools
{
    /// <summary>
    /// Catálogo canónico de tiendas y mercados soportados.
    /// La fuente de verdad de los nombres usados por la UI y los motores.
    /// </summary>
    internal static class StoreCatalog
    {
        /// <summary>Tiendas mostradas en la columna izquierda de los selectores.</summary>
        public static readonly string[] StoresLeft =
        {
            "ProductosTX",
            "Holaproducto",
            "Altinor",
            "HervazTrade",
        };

        /// <summary>Tiendas mostradas en la columna derecha (BBvs).</summary>
        public static readonly string[] StoresRight =
        {
            "BBvs_Template",
            "BBvsBB2_2da",
            "BBvsBB2",
        };

        /// <summary>Mercados Amazon soportados.</summary>
        public static readonly string[] Markets = { "MX", "US" };

        /// <summary>Modos de ordenamiento de ASINs (en español por compatibilidad histórica).</summary>
        public static readonly string[] OrderChoices = { "Ordenado", "Inverso", "Aleatorio" };
    }
}
