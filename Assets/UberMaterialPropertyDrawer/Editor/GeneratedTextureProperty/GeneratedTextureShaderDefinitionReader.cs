using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal static class GeneratedTextureShaderDefinitionReader
    {
        private static readonly Regex GeneratedPropertyRegex = new(
            @"(?:\[[^\]]+\]\s*)*\[(?<drawer>CurveTexture|GradientTexture)\s*\([^\]]*\)\](?:\s*\[[^\]]+\])*\s*(?<prop>[_A-Za-z][_A-Za-z0-9]*)\s*\(",
            RegexOptions.Multiline | RegexOptions.Compiled);

        public static bool TryReadGeneratedPropertyKinds(Shader shader, out Dictionary<string, string> propertyKinds)
        {
            propertyKinds = null;
            if (shader == null) return false;

            var shaderPath = AssetDatabase.GetAssetPath(shader);
            if (string.IsNullOrEmpty(shaderPath)) return false;
            if (!shaderPath.EndsWith(".shader", StringComparison.OrdinalIgnoreCase)) return false;
            if (!File.Exists(shaderPath)) return false;

            var shaderSource = File.ReadAllText(shaderPath);
            if (!TryExtractPropertiesBlock(shaderSource, out var propertiesBlock)) return false;

            propertyKinds = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (Match match in GeneratedPropertyRegex.Matches(propertiesBlock))
            {
                var propertyName = match.Groups["prop"].Value;
                var generatorKind = match.Groups["drawer"].Value;
                if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(generatorKind)) continue;
                propertyKinds[propertyName] = generatorKind;
            }

            return true;
        }

        private static bool TryExtractPropertiesBlock(string shaderSource, out string propertiesBlock)
        {
            propertiesBlock = null;
            if (string.IsNullOrEmpty(shaderSource)) return false;

            var propertiesIndex = shaderSource.IndexOf("Properties", StringComparison.Ordinal);
            if (propertiesIndex < 0) return false;

            var braceStart = shaderSource.IndexOf('{', propertiesIndex);
            if (braceStart < 0) return false;

            var depth = 0;
            for (var i = braceStart; i < shaderSource.Length; i++)
            {
                var ch = shaderSource[i];
                if (ch == '{')
                {
                    depth++;
                    continue;
                }

                if (ch != '}') continue;
                depth--;
                if (depth != 0) continue;

                propertiesBlock = shaderSource.Substring(braceStart + 1, i - braceStart - 1);
                return true;
            }

            return false;
        }
    }
}
