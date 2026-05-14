using System.Collections.Generic;

namespace S3Tools
{
    /// <summary>
    /// Resultado de extraer ASINs de un archivo: lista de únicos preservando el orden
    /// de aparición y lista de duplicados con repeticiones.
    /// </summary>
    internal sealed class ExtractionResult
    {
        public ExtractionResult(List<string> uniques, List<string> duplicates)
        {
            Uniques = uniques;
            Duplicates = duplicates;
        }

        public List<string> Uniques { get; }
        public List<string> Duplicates { get; }
    }
}
