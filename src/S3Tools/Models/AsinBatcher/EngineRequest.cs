using System.Runtime.Serialization;

namespace S3Tools
{
    /// <summary>
    /// Petición que llega al motor de ASIN Batcher.
    /// Los nombres serializables (<see cref="DataMemberAttribute.Name"/>) usan snake_case
    /// para preservar compatibilidad histórica con el formato original del motor.
    /// </summary>
    [DataContract]
    internal sealed class EngineRequest
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "input_path")]
        public string InputPath { get; set; }

        [DataMember(Name = "output_dir")]
        public string OutputDir { get; set; }

        [DataMember(Name = "market")]
        public string Market { get; set; }

        [DataMember(Name = "store")]
        public string Store { get; set; }

        [DataMember(Name = "order")]
        public string Order { get; set; }

        [DataMember(Name = "batches")]
        public int? Batches { get; set; }

        [DataMember(Name = "zip_output")]
        public bool? ZipOutput { get; set; }

        [DataMember(Name = "show_seller_on_open")]
        public bool? ShowSellerOnOpen { get; set; }

        [DataMember(Name = "file_label")]
        public string FileLabel { get; set; }

        [DataMember(Name = "name_prefix_1")]
        public string NamePrefix1 { get; set; }

        [DataMember(Name = "name_prefix_2")]
        public string NamePrefix2 { get; set; }

        [DataMember(Name = "store_name")]
        public string StoreName { get; set; }
    }
}
