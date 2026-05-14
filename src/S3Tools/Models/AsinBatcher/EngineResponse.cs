using System.Runtime.Serialization;

namespace S3Tools
{
    /// <summary>
    /// Respuesta del motor de ASIN Batcher.
    /// Los nombres serializables se mantienen en snake_case por compatibilidad.
    /// </summary>
    [DataContract]
    internal sealed class EngineResponse
    {
        [DataMember(Name = "ok")]
        public bool Ok { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "traceback")]
        public string Traceback { get; set; }

        [DataMember(Name = "total")]
        public int? Total { get; set; }

        [DataMember(Name = "unique")]
        public int? Unique { get; set; }

        [DataMember(Name = "duplicates")]
        public int? Duplicates { get; set; }

        [DataMember(Name = "output_folder")]
        public string OutputFolder { get; set; }

        [DataMember(Name = "zip_path")]
        public string ZipPath { get; set; }

        [DataMember(Name = "csv_path")]
        public string CsvPath { get; set; }
    }
}
