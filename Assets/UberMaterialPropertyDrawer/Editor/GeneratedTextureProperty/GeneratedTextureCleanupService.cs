using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal static class GeneratedTextureCleanupService
    {
        private static readonly GeneratedTextureAssetCoordinator AssetCoordinator = new();

        public static void CleanupMaterial(Material mat, CleanupMode mode)
        {
            if (mat == null) return;

            var subAssets = Util.FetchSubAssets(mat).Where(asset => asset != null).ToList();
            var generatedDataAssets = subAssets.OfType<GeneratedTextureDataAssetBase>().ToArray();
            if (generatedDataAssets.Length == 0) return;

            var hasShaderDefinitions = GeneratedTextureShaderDefinitionReader.TryReadGeneratedPropertyKinds(
                mat.shader,
                out var generatedPropertyKinds);

            var materialChanged = false;

            foreach (var generatedData in generatedDataAssets)
            {
                if (generatedData == null) continue;

                var generatedTexture = FindGeneratedTexture(subAssets, generatedData.GeneratedTextureName);
                if (mode == CleanupMode.Interactive)
                {
                    if (!ShouldDeleteGeneratedAssetsInteractive(mat, generatedData, generatedTexture, hasShaderDefinitions, generatedPropertyKinds))
                        continue;
                }
                else
                {
                    var normalizationResult = AssetCoordinator.Normalize(mat, generatedData, generatedTexture);
                    if (!normalizationResult.IsValid)
                    {
                        UberDrawerLogger.LogWarning($"{normalizationResult.Warning} Material: {mat.name}");
                        continue;
                    }

                    if (normalizationResult.HasAnyChange)
                    {
                        EditorUtility.SetDirty(generatedData);
                        if (generatedTexture != null)
                            EditorUtility.SetDirty(generatedTexture);
                        materialChanged = true;
                    }

                    if (!ShouldDeleteGeneratedAssetsNormalized(mat, generatedData, generatedTexture, hasShaderDefinitions, generatedPropertyKinds))
                        continue;
                }

                DisconnectGeneratedTextureReference(mat, generatedData, generatedTexture);

                if (generatedTexture != null)
                {
                    DestroyGeneratedAsset(generatedTexture, mode);
                    subAssets.Remove(generatedTexture);
                }

                DestroyGeneratedAsset(generatedData, mode);
                subAssets.Remove(generatedData);
                materialChanged = true;
            }

            if (!materialChanged) return;

            EditorUtility.SetDirty(mat);
            Util.DelaySaveAsset(mat);
        }

        public static void CleanupAllAffectedMaterials(IEnumerable<Shader> shaders, CleanupMode mode)
        {
            HashSet<Shader> shaderSet = null;
            if (shaders != null)
                shaderSet = new HashSet<Shader>(shaders.Where(shader => shader != null));

            foreach (var guid in AssetDatabase.FindAssets("t:Material"))
            {
                var materialPath = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (mat == null) continue;
                if (shaderSet is { Count: > 0 } && !shaderSet.Contains(mat.shader)) continue;
                if (!ContainsGeneratedAssets(mat)) continue;

                CleanupMaterial(mat, mode);
            }
        }

        private static bool ShouldDeleteGeneratedAssetsInteractive(
            Material mat,
            GeneratedTextureDataAssetBase generatedData,
            Texture2D generatedTexture,
            bool hasShaderDefinitions,
            IReadOnlyDictionary<string, string> generatedPropertyKinds)
        {
            var sourcePropertyName = generatedData.SourcePropertyName;
            if (string.IsNullOrEmpty(sourcePropertyName)) return false;
            if (!mat.HasProperty(sourcePropertyName)) return true;
            if (generatedTexture == null) return true;

            var currentTexture = mat.GetTexture(sourcePropertyName);
            if (currentTexture != generatedTexture) return true;

            if (!hasShaderDefinitions) return false;
            if (string.IsNullOrEmpty(generatedData.GeneratorKind)) return false;
            if (!generatedPropertyKinds.TryGetValue(sourcePropertyName, out var generatorKind)) return true;
            return !string.Equals(generatorKind, generatedData.GeneratorKind, StringComparison.Ordinal);
        }

        private static bool ShouldDeleteGeneratedAssetsNormalized(
            Material mat,
            GeneratedTextureDataAssetBase generatedData,
            Texture2D generatedTexture,
            bool hasShaderDefinitions,
            IReadOnlyDictionary<string, string> generatedPropertyKinds)
        {
            if (string.IsNullOrEmpty(generatedData.SourcePropertyName)) return false;
            if (!mat.HasProperty(generatedData.SourcePropertyName)) return true;

            if (hasShaderDefinitions)
            {
                if (!generatedPropertyKinds.TryGetValue(generatedData.SourcePropertyName, out var generatorKind))
                    return true;

                if (!string.Equals(generatorKind, generatedData.GeneratorKind, StringComparison.Ordinal))
                    return true;
            }

            if (generatedTexture == null) return true;
            var currentTexture = mat.GetTexture(generatedData.SourcePropertyName);
            return currentTexture != generatedTexture;
        }

        private static void DisconnectGeneratedTextureReference(
            Material mat,
            GeneratedTextureDataAssetBase generatedData,
            Texture2D generatedTexture)
        {
            if (string.IsNullOrEmpty(generatedData.SourcePropertyName)) return;
            if (!mat.HasProperty(generatedData.SourcePropertyName)) return;

            var currentTexture = mat.GetTexture(generatedData.SourcePropertyName);
            if (currentTexture != generatedTexture) return;

            mat.SetTexture(generatedData.SourcePropertyName, null);
        }

        private static void DestroyGeneratedAsset(Object asset, CleanupMode mode)
        {
            if (asset == null) return;

            if (mode == CleanupMode.Interactive)
            {
                Undo.DestroyObjectImmediate(asset);
                return;
            }

            Object.DestroyImmediate(asset, true);
        }

        private static bool ContainsGeneratedAssets(Material mat)
        {
            return Util.FetchSubAssets(mat).OfType<GeneratedTextureDataAssetBase>().Any();
        }

        private static Texture2D FindGeneratedTexture(IEnumerable<Object> subAssets, string textureName)
        {
            if (string.IsNullOrEmpty(textureName)) return null;

            foreach (var texture in subAssets.OfType<Texture2D>())
            {
                if (texture == null) continue;
                if (texture.name == textureName) return texture;
            }

            return null;
        }
    }
}
