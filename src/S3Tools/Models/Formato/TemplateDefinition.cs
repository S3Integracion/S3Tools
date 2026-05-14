using System.Collections.Generic;
using System.Runtime.Serialization;

namespace S3Tools
{
    /// <summary>Modelo simplificado de una plantilla de WebScraper para detectar encabezados.</summary>
    [DataContract]
    internal sealed class TemplateDefinition
    {
        [DataMember(Name = "selectors")]
        public List<TemplateSelector> Selectors { get; set; }
    }

    /// <summary>Selector individual dentro de una plantilla WebScraper.</summary>
    [DataContract]
    internal sealed class TemplateSelector
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
