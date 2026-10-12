using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace S3Tools
{
    /// <summary>Gravedad de un hallazgo de validación: los errores bloquean el guardado, las advertencias no.</summary>
    internal enum TemplateIssueSeverity
    {
        Error,
        Warning,
    }

    /// <summary>Hallazgo de validación de una plantilla, con posición opcional (base 1).</summary>
    internal sealed class TemplateIssue
    {
        public TemplateIssue(TemplateIssueSeverity severity, string message, long? line = null, long? column = null)
        {
            Severity = severity;
            Message = message;
            Line = line;
            Column = column;
        }

        public TemplateIssueSeverity Severity { get; }

        public string Message { get; }

        /// <summary>Línea (base 1) donde ocurrió el problema, si se conoce.</summary>
        public long? Line { get; }

        /// <summary>Columna (base 1) donde ocurrió el problema, si se conoce.</summary>
        public long? Column { get; }
    }

    /// <summary>Resumen de un selector de la plantilla para mostrarlo en la UI.</summary>
    internal sealed class SelectorSummary
    {
        public SelectorSummary(string id, string type, string selector, bool multiple, IReadOnlyList<string> parentSelectors)
        {
            Id = id;
            Type = type;
            Selector = selector;
            Multiple = multiple;
            ParentSelectors = parentSelectors;
        }

        public string Id { get; }

        public string Type { get; }

        public string Selector { get; }

        public bool Multiple { get; }

        public IReadOnlyList<string> ParentSelectors { get; }
    }

    /// <summary>Resultado de validar el contenido de una plantilla.</summary>
    internal sealed class TemplateValidationResult
    {
        public TemplateValidationResult(
            IReadOnlyList<TemplateIssue> issues,
            IReadOnlyList<SelectorSummary> selectors,
            JsonObject sitemap)
        {
            Issues = issues;
            Selectors = selectors;
            Sitemap = sitemap;
        }

        /// <summary>Verdadero cuando no hay ningún hallazgo de tipo <see cref="TemplateIssueSeverity.Error"/>.</summary>
        public bool IsValid
        {
            get
            {
                foreach (var issue in Issues)
                {
                    if (issue.Severity == TemplateIssueSeverity.Error)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public IReadOnlyList<TemplateIssue> Issues { get; }

        public IReadOnlyList<SelectorSummary> Selectors { get; }

        /// <summary>Objeto sitemap analizado; <c>null</c> si el texto no se pudo interpretar como objeto JSON.</summary>
        public JsonObject Sitemap { get; }
    }
}
