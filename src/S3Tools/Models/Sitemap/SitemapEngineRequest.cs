using System.Runtime.Serialization;

namespace S3Tools
{
    /// <summary>Petición al motor de Sitemap.</summary>
    [DataContract]
    internal sealed class SitemapEngineRequest
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "input_files")]
        public string[] InputFiles { get; set; }

        [DataMember(Name = "output_dir")]
        public string OutputDir { get; set; }

        [DataMember(Name = "base_name")]
        public string BaseName { get; set; }

        [DataMember(Name = "store")]
        public string Store { get; set; }

        [DataMember(Name = "zip_output")]
        public bool? ZipOutput { get; set; }

        [DataMember(Name = "template_mode")]
        public string TemplateMode { get; set; }

        [DataMember(Name = "name_prefix_1")]
        public string NamePrefix1 { get; set; }

        [DataMember(Name = "name_prefix_2")]
        public string NamePrefix2 { get; set; }

        [DataMember(Name = "store_name")]
        public string StoreName { get; set; }
    }
}
