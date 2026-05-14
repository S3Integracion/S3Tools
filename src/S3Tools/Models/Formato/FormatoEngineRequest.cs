using System.Runtime.Serialization;

namespace S3Tools
{
    /// <summary>Petición al motor de Formato (normalización de encabezados WebScraper).</summary>
    [DataContract]
    internal sealed class FormatoEngineRequest
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "input_files")]
        public string[] InputFiles { get; set; }

        /// <summary>"auto" / "tiendas" / "bbvs".</summary>
        [DataMember(Name = "template")]
        public string Template { get; set; }

        /// <summary>"underscore" (default) o "hyphen".</summary>
        [DataMember(Name = "header_format")]
        public string HeaderFormat { get; set; }
    }
}
