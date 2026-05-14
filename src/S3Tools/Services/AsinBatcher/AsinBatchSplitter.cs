using System;
using System.Collections.Generic;
using System.Linq;

namespace S3Tools
{
    /// <summary>
    /// Ordena y divide listas de ASINs en lotes equilibrados.
    /// </summary>
    internal static class AsinBatchSplitter
    {
        private static readonly string[] AllStores = MergeStores();

        private static string[] MergeStores()
        {
            var left  = StoreCatalog.StoresLeft;
            var right = StoreCatalog.StoresRight;
            var merged = new string[left.Length + right.Length];
            System.Array.Copy(left,  0, merged, 0,            left.Length);
            System.Array.Copy(right, 0, merged, left.Length,  right.Length);
            return merged;
        }

        /// <summary>
        /// Aplica el modo de ordenamiento solicitado: "Ordenado" (ascendente),
        /// "Inverso" (descendente) o "Aleatorio" (shuffle Fisher–Yates).
        /// </summary>
        public static List<string> ReorderAsins(IEnumerable<string> uniques, string mode)
        {
            var normalized = (mode ?? string.Empty).ToLowerInvariant();

            if (normalized == "inverso")
            {
                return uniques.OrderByDescending(x => x, StringComparer.Ordinal).ToList();
            }

            if (normalized == "aleatorio")
            {
                var shuffled = uniques.ToList();
                var rng = new Random();
                for (var i = shuffled.Count - 1; i > 0; i--)
                {
                    var j = rng.Next(i + 1);
                    (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
                }
                return shuffled;
            }

            return uniques.OrderBy(x => x, StringComparer.Ordinal).ToList();
        }

        /// <summary>
        /// Divide la lista en exactamente <paramref name="batches"/> lotes balanceados.
        /// El primer remainder de lotes recibe un elemento extra.
        /// </summary>
        public static List<List<string>> SplitInBatches(IList<string> items, int batches)
        {
            if (batches <= 1)
            {
                return new List<List<string>> { items.ToList() };
            }

            var n = items.Count;
            if (n == 0)
            {
                var empty = new List<List<string>>();
                for (var i = 0; i < batches; i++)
                {
                    empty.Add(new List<string>());
                }
                return empty;
            }

            var baseSize = n / batches;
            var remainder = n % batches;
            var outList = new List<List<string>>();
            var start = 0;

            for (var i = 0; i < batches; i++)
            {
                var count = baseSize + (i < remainder ? 1 : 0);
                outList.Add(items.Skip(start).Take(count).ToList());
                start += count;
            }
            return outList;
        }

        /// <summary>Devuelve la tienda válida de <see cref="AllStores"/>, o la primera por defecto.</summary>
        public static string ComputeStoreFromSelection(string selectedStore)
        {
            return AllStores.Contains(selectedStore) ? selectedStore : AllStores[0];
        }
    }
}
