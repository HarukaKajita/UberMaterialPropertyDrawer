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
        public static void CleanupMaterial(Material mat, CleanupMode mode)
        {
            if (mat == null) return;

            var subAssets = Util.FetchSubAssets(mat).Where(asset => asset != null).ToList();
            var generatedDataAssets = subAssets.OfType<GeneratedTextureDataBase>().ToArray();
            if (generatedDataAssets.Length == 0) return;

            var hasShaderDefinitions = GeneratedTextureShaderDefinitionReader.TryReadGeneratedPropertyKinds(
                mat.shader,
                out var generatedPropertyKinds);

            var materialChanged = false;

            foreach (var generatedData in generatedDataAssets)
            {
                if (generatedData == null) continue;

                if (!TryEnsureMetadata(mat, generatedData, out var metadataChanged))
                    continue;

                materialChanged |= metadataChanged;

                var generatedTexture = FindGeneratedTexture(subAssets, generatedData.GeneratedTextureName);
                if (generatedTexture != null && EnsureGeneratedAssetVisibility(generatedTexture))
                {
                    EditorUtility.SetDirty(generatedTexture);
                    materialChanged = true;
                }

                if (!ShouldDeleteGeneratedAssets(mat, generatedData, generatedTexture, hasShaderDefinitions, generatedPropertyKinds))
                    continue;

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

        private static bool TryEnsureMetadata(Material mat, GeneratedTextureDataBase generatedData, out bool metadataChanged)
        {
            metadataChanged = false;
            if (generatedData == null) return false;

            if (!generatedData.HasCompleteMetadata)
            {
                if (!generatedData.TryBackfillMetadata(out var warning))
                {
                    UberDrawerLogger.LogWarning($"{warning} Material: {mat.name}");
                    return false;
                }

                EditorUtility.SetDirty(generatedData);
                metadataChanged = true;
            }

            if (EnsureGeneratedAssetVisibility(generatedData))
            {
                EditorUtility.SetDirty(generatedData);
                metadataChanged = true;
            }

            return true;
        }

        private static bool ShouldDeleteGeneratedAssets(
            Material mat,
            GeneratedTextureDataBase generatedData,
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
            GeneratedTextureDataBase generatedData,
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
            return Util.FetchSubAssets(mat).OfType<GeneratedTextureDataBase>().Any();
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

        private static bool EnsureGeneratedAssetVisibility(Object asset)
        {
            if (asset == null) return false;

            const HideFlags generatedAssetHideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            var nextHideFlags = asset.hideFlags | generatedAssetHideFlags;
            if (nextHideFlags == asset.hideFlags) return false;

            asset.hideFlags = nextHideFlags;
            return true;
        }
    }
}

