using System;
using System.Runtime.Serialization;

namespace S3Tools
{
    /// <summary>Respuesta del motor de Sitemap.</summary>
    [DataContract]
    internal sealed class SitemapEngineResponse
    {
        [DataMember(Name = "ok")]
        public bool Ok { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "traceback")]
        public string Traceback { get; set; }

        [DataMember(Name = "output_folder")]
        public string OutputFolder { get; set; }

        [DataMember(Name = "zip_path")]
        public string ZipPath { get; set; }

        [DataMember(Name = "output_files")]
        public string[] OutputFiles { get; set; } = Array.Empty<string>();
    }
}
