using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// Materialのサブアセットの探索、生成、保存を管理するクラス
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    internal sealed class MaterialSubAssetStore<TData> where TData : GeneratedTextureDataBase
    {
        private readonly GeneratedTextureAssetCoordinator _assetCoordinator = new();

        public GeneratedTextureBinding<TData>[] EnsureBindings(Material[] materials, string propertyName, string dataAssetName, string textureAssetName)
        {
            if (materials == null || materials.Length == 0) return Array.Empty<GeneratedTextureBinding<TData>>();
            return materials.Select(mat => EnsureBindings(mat, propertyName, dataAssetName, textureAssetName)).ToArray();
        }

        public GeneratedTextureBinding<TData> EnsureBindings(Material mat, string propertyName,
            string dataAssetName, string textureAssetName)
        {
            if (mat == null)
                throw new ArgumentNullException(nameof(mat));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName cannot be null or empty", nameof(propertyName));
            if (string.IsNullOrEmpty(dataAssetName))
                throw new ArgumentException("dataAssetName cannot be null or empty", nameof(dataAssetName));
            if (string.IsNullOrEmpty(textureAssetName))
                throw new ArgumentException("textureAssetName cannot be null or empty", nameof(textureAssetName));

            var requiresSave = false;
            var data = FetchDataAsset(mat, dataAssetName);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<TData>();
                data.name = dataAssetName;
                AssetDatabase.AddObjectToAsset(data, mat);
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            var subAssetTexture = Util.FetchSubAssetTexture(mat, textureAssetName);
            var normalizationResult = _assetCoordinator.Normalize(mat, data, subAssetTexture, propertyName, textureAssetName);
            if (!normalizationResult.IsValid)
                throw new InvalidOperationException(normalizationResult.Warning ?? "Generated texture normalization failed.");

            var materialTexture = mat.GetTexture(propertyName) as Texture2D;
            if (normalizationResult.MetadataChanged || normalizationResult.SettingsChanged || normalizationResult.VisibilityChanged)
            {
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            if (subAssetTexture != null && normalizationResult.VisibilityChanged)
            {
                EditorUtility.SetDirty(subAssetTexture);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            if (normalizationResult.RequiresTextureRecreation)
            {
                if (subAssetTexture != null)
                {
                    if (materialTexture == subAssetTexture)
                    {
                        mat.SetTexture(propertyName, null);
                        EditorUtility.SetDirty(mat);
                    }

                    Object.DestroyImmediate(subAssetTexture, true);
                    requiresSave = true;
                }

                subAssetTexture = CreateGeneratedTexture(textureAssetName, data);
                AssetDatabase.AddObjectToAsset(subAssetTexture, mat);
                EditorUtility.SetDirty(subAssetTexture);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            if (subAssetTexture != null && data.ApplyStoredTextureSettings(subAssetTexture))
            {
                EditorUtility.SetDirty(subAssetTexture);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            if (materialTexture != subAssetTexture)
            {
                mat.SetTexture(propertyName, subAssetTexture);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            if (requiresSave)
                Util.DelaySaveAsset(mat);

            return new GeneratedTextureBinding<TData>(mat, data, subAssetTexture);
        }

        private static TData FetchDataAsset(Material mat, string dataName)
        {
            return Util.FetchSubAssets(mat).OfType<TData>().FirstOrDefault(a => a.name == dataName);
        }

        private static Texture2D CreateGeneratedTexture(string textureAssetName, GeneratedTextureDataBase data)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false, data.UsesLinearColorSpace)
            {
                name = textureAssetName,
                hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector
            };
            data.ApplyStoredTextureSettings(texture);
            return texture;
        }
    }
}