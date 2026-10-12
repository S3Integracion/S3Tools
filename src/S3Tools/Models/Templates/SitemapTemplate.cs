using System;
using System.Collections.Generic;
using System.Linq;

namespace S3Tools
{
    /// <summary>
    /// Plantilla de sitemap de WebScraper administrada por el usuario. Es inmutable: cualquier
    /// cambio produce una instancia nueva (ver el repositorio de plantillas).
    /// </summary>
    internal sealed class SitemapTemplate
    {
        public SitemapTemplate(
            string id,
            string name,
            string description,
            string builtInKey,
            IReadOnlyList<string> stores,
            DateTime createdUtc,
            DateTime updatedUtc,
            string sitemapJson)
        {
            Id = id;
            Name = name;
            Description = description ?? string.Empty;
            BuiltInKey = builtInKey;
            // Copia defensiva: la plantilla es inmutable aunque el llamador conserve y modifique su lista.
            Stores = stores == null ? Array.Empty<string>() : stores.ToArray();
            CreatedUtc = ToUtc(createdUtc);
            UpdatedUtc = ToUtc(updatedUtc);
            SitemapJson = sitemapJson;
        }

        /// <summary>Identificador inmutable (<c>^[a-z0-9]{1,64}$</c>).</summary>
        public string Id { get; }

        public string Name { get; }

        public string Description { get; }

        /// <summary>Clave de la plantilla de fábrica de origen (<c>tiendas</c> o <c>bbvs</c>); <c>null</c> si la creó el usuario.</summary>
        public string BuiltInKey { get; }

        /// <summary>Tiendas vinculadas a esta plantilla (cada tienda pertenece a lo sumo a una).</summary>
        public IReadOnlyList<string> Stores { get; }

        public DateTime CreatedUtc { get; }

        public DateTime UpdatedUtc { get; }

        /// <summary>Contenido del sitemap como texto JSON.</summary>
        public string SitemapJson { get; }

        public bool IsBuiltIn
        {
            get { return BuiltInKey != null; }
        }

        private static DateTime ToUtc(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Utc:
                    return value;
                case DateTimeKind.Local:
                    return value.ToUniversalTime();
                default:
                    return DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }
    }

    /// <summary>Datos editables de una plantilla: entrada para crear o actualizar desde el repositorio.</summary>
    internal sealed class SitemapTemplateDraft
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IReadOnlyList<string> Stores { get; set; }

        public string SitemapJson { get; set; }
    }

    /// <summary>Resultado de cargar el almacén de plantillas.</summary>
    internal sealed class StoreLoadResult
    {
        public StoreLoadResult(IReadOnlyList<SitemapTemplate> templates, string warning, bool isReadOnly)
        {
            Templates = templates;
            Warning = warning;
            IsReadOnly = isReadOnly;
        }

        public IReadOnlyList<SitemapTemplate> Templates { get; }

        /// <summary>Resumen de las correcciones o recuperaciones aplicadas al cargar; <c>null</c> si la carga fue limpia.</summary>
        public string Warning { get; }

        /// <summary>Verdadero cuando el archivo no se puede modificar (versión de esquema futura o archivo inaccesible).</summary>
        public bool IsReadOnly { get; }
    }
}
