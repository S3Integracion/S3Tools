using System.Collections.Generic;
using System.Runtime.Serialization;

namespace S3Tools
{
    /// <summary>Respuesta del motor de Formato.</summary>
    [DataContract]
    internal sealed class FormatoEngineResponse
    {
        [DataMember(Name = "ok")]
        public bool Ok { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "traceback")]
        public string Traceback { get; set; }

        [DataMember(Name = "updated_files")]
        public string[] UpdatedFiles { get; set; }

        /// <summary>Conteo de archivos por plantilla detectada (claves: "tiendas", "bbvs"...).</summary>
        [DataMember(Name = "template_counts")]
        public Dictionary<string, int> TemplateCounts { get; set; }
    }
}
