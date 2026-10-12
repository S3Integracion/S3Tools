using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace S3Tools
{
    /// <summary>
    /// Valida el contenido JSON de una plantilla de sitemap de WebScraper. Los errores bloquean
    /// el guardado; las advertencias solo informan. La lectura es tolerante: acepta BOM inicial,
    /// comentarios y comas finales, y <see cref="Normalize"/> produce el JSON limpio.
    /// </summary>
    internal static class SitemapTemplateValidator
    {
        /// <summary>Longitud máxima (en caracteres) aceptada para el texto de una plantilla.</summary>
        public const int MaxTemplateChars = 1_000_000;

        private const string ElementClickType = "SelectorElementClick";
        private const string RootSelectorId = "_root";

        private static readonly JsonDocumentOptions TolerantOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            AllowDuplicateProperties = false,
        };

        private static readonly JsonSerializerOptions IndentedOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// Interpreta <paramref name="json"/> de forma tolerante (sin BOM inicial, con comentarios y
        /// comas finales). Lanza <see cref="JsonException"/> si el texto no es JSON válido.
        /// </summary>
        internal static JsonNode ParseTolerant(string json)
        {
            if (json.Length > 0 && json[0] == '﻿')
            {
                json = json.Substring(1);
            }

            return JsonNode.Parse(json, null, TolerantOptions);
        }

        /// <summary>Serializa el sitemap como JSON indentado, sin escapar caracteres como <c>&amp;</c> o <c>ñ</c>.</summary>
        public static string Normalize(JsonObject sitemap)
        {
            return sitemap.ToJsonString(IndentedOptions);
        }

        /// <summary>Valida el texto de una plantilla y devuelve los hallazgos y el resumen de selectores.</summary>
        public static TemplateValidationResult Validate(string json)
        {
            var issues = new List<TemplateIssue>();
            var selectors = new List<SelectorSummary>();

            if (string.IsNullOrWhiteSpace(json))
            {
                issues.Add(Error("La plantilla está vacía."));
                return new TemplateValidationResult(issues, selectors, null);
            }

            if (json.Length > MaxTemplateChars)
            {
                issues.Add(Error("La plantilla excede el máximo de " + MaxTemplateChars.ToString("N0") + " caracteres."));
                return new TemplateValidationResult(issues, selectors, null);
            }

            JsonNode root;
            try
            {
                root = ParseTolerant(json);
            }
            catch (JsonException ex)
            {
                var line = ex.LineNumber.HasValue ? ex.LineNumber.Value + 1 : (long?)null;
                var column = ex.BytePositionInLine.HasValue ? ex.BytePositionInLine.Value + 1 : (long?)null;
                issues.Add(new TemplateIssue(TemplateIssueSeverity.Error, "JSON inválido: " + ex.Message, line, column));
                return new TemplateValidationResult(issues, selectors, null);
            }

            var sitemap = root as JsonObject;
            if (sitemap == null)
            {
                issues.Add(Error("La raíz de la plantilla debe ser un objeto JSON."));
                return new TemplateValidationResult(issues, selectors, null);
            }

            ValidateTopLevelFields(sitemap, issues);
            ValidateSelectors(sitemap, issues, selectors);
            return new TemplateValidationResult(issues, selectors, sitemap);
        }

        private static void ValidateTopLevelFields(JsonObject sitemap, List<TemplateIssue> issues)
        {
            if (!sitemap.TryGetPropertyValue("_id", out var id) || id == null)
            {
                issues.Add(Warning("Falta \"_id\"; se agregará al generar el sitemap."));
            }
            else if (!IsString(id))
            {
                issues.Add(Error("\"_id\" debe ser una cadena de texto."));
            }

            if (!sitemap.TryGetPropertyValue("startUrl", out var startUrl) || startUrl == null)
            {
                issues.Add(Warning("Falta \"startUrl\"; se agregará al generar el sitemap."));
            }
            else if (!IsString(startUrl) && !IsStringArray(startUrl))
            {
                issues.Add(Error("\"startUrl\" debe ser una cadena o un arreglo de cadenas."));
            }
        }

        private static void ValidateSelectors(JsonObject sitemap, List<TemplateIssue> issues, List<SelectorSummary> summaries)
        {
            if (!sitemap.TryGetPropertyValue("selectors", out var selectorsNode) || selectorsNode == null)
            {
                issues.Add(Error("Falta \"selectors\"."));
                return;
            }

            var array = selectorsNode as JsonArray;
            if (array == null)
            {
                issues.Add(Error("\"selectors\" debe ser un arreglo."));
                return;
            }

            if (array.Count == 0)
            {
                issues.Add(Error("\"selectors\" no puede estar vacío."));
                return;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            var reportedDuplicates = new HashSet<string>(StringComparer.Ordinal);
            var parentRefs = new List<KeyValuePair<string, string>>();
            var hasNonClickSelector = false;

            for (var i = 0; i < array.Count; i++)
            {
                var position = i + 1;
                var obj = array[i] as JsonObject;
                if (obj == null)
                {
                    issues.Add(Error("El selector #" + position + " debe ser un objeto."));
                    continue;
                }

                var id = ReadString(obj, "id");
                if (string.IsNullOrEmpty(id))
                {
                    issues.Add(Error("El selector #" + position + " debe tener un \"id\" de texto no vacío."));
                }
                else if (!ids.Add(id) && reportedDuplicates.Add(id))
                {
                    issues.Add(Error("El id de selector \"" + id + "\" está duplicado."));
                }

                var type = ReadString(obj, "type");
                if (string.IsNullOrEmpty(type))
                {
                    issues.Add(Error("El selector " + Describe(id, position) + " debe tener un \"type\" de texto no vacío."));
                }
                else if (!string.Equals(type, ElementClickType, StringComparison.Ordinal))
                {
                    hasNonClickSelector = true;
                }

                var parents = new List<string>();
                if (obj.TryGetPropertyValue("parentSelectors", out var parentsNode) && parentsNode != null)
                {
                    var parentsArray = parentsNode as JsonArray;
                    if (parentsArray == null)
                    {
                        issues.Add(Error("\"parentSelectors\" del selector " + Describe(id, position) + " debe ser un arreglo."));
                    }
                    else
                    {
                        foreach (var parent in parentsArray)
                        {
                            if (IsString(parent))
                            {
                                var parentId = parent.GetValue<string>();
                                parents.Add(parentId);
                                parentRefs.Add(new KeyValuePair<string, string>(Describe(id, position), parentId));
                            }
                            else
                            {
                                issues.Add(Error("\"parentSelectors\" del selector " + Describe(id, position) + " solo admite cadenas."));
                            }
                        }
                    }
                }

                summaries.Add(new SelectorSummary(
                    id ?? string.Empty,
                    type ?? string.Empty,
                    ReadString(obj, "selector") ?? string.Empty,
                    obj.TryGetPropertyValue("multiple", out var multiple) && IsTrue(multiple),
                    parents));
            }

            foreach (var reference in parentRefs)
            {
                if (reference.Value != RootSelectorId && !ids.Contains(reference.Value))
                {
                    issues.Add(Warning("El selector " + reference.Key + " referencia el padre inexistente \"" + reference.Value + "\"."));
                }
            }

            if (!hasNonClickSelector)
            {
                issues.Add(Warning("No hay ningún selector distinto de " + ElementClickType + "; Formato no podrá autodetectar la plantilla."));
            }
        }

        private static string Describe(string id, int position)
        {
            return string.IsNullOrEmpty(id) ? "#" + position : "\"" + id + "\"";
        }

        private static string ReadString(JsonObject obj, string name)
        {
            return obj.TryGetPropertyValue(name, out var node) && IsString(node) ? node.GetValue<string>() : null;
        }

        private static bool IsString(JsonNode node)
        {
            return node is JsonValue value && value.TryGetValue<string>(out _);
        }

        private static bool IsTrue(JsonNode node)
        {
            return node is JsonValue value && value.TryGetValue<bool>(out var flag) && flag;
        }

        private static bool IsStringArray(JsonNode node)
        {
            var array = node as JsonArray;
            if (array == null)
            {
                return false;
            }

            foreach (var item in array)
            {
                if (!IsString(item))
                {
                    return false;
                }
            }

            return true;
        }

        private static TemplateIssue Error(string message)
        {
            return new TemplateIssue(TemplateIssueSeverity.Error, message);
        }

        private static TemplateIssue Warning(string message)
        {
            return new TemplateIssue(TemplateIssueSeverity.Warning, message);
        }
    }
}
