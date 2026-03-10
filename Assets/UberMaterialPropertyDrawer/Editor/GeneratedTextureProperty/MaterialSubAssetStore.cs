using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// Materialのサブアセットの探索、生成、保存を管理するクラス
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    internal sealed class MaterialSubAssetStore<TData> where TData : GeneratedTextureDataBase
    {
        public GeneratedTextureBinding<TData>[] EnsureBindings(Material[] materials, string propertyName, string dataAssetName, string textureAssetName)
        {
            if (materials == null || materials.Length == 0) return null;
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
                ApplyGeneratedAssetVisibility(data);
                AssetDatabase.AddObjectToAsset(data, mat);
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            if (data.SyncMetadata(propertyName, textureAssetName))
            {
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            if (ApplyGeneratedAssetVisibility(data))
            {
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            var subAssetTexture = Util.FetchSubAssetTexture(mat, textureAssetName);
            var materialTexture = mat.GetTexture(propertyName);
            if (subAssetTexture == null || subAssetTexture != materialTexture)
            {
                if (subAssetTexture != null)
                {
                    mat.SetTexture(propertyName, subAssetTexture);
                    EditorUtility.SetDirty(mat);
                    requiresSave = true;
                }
                else
                {
                    var texture = new Texture2D(1,1,TextureFormat.RGBA32, false, true);
                    texture.name = textureAssetName;
                    ApplyGeneratedAssetVisibility(texture);
                    AssetDatabase.AddObjectToAsset(texture, mat);
                    EditorUtility.SetDirty(texture);
                    EditorUtility.SetDirty(mat);
                    subAssetTexture = texture;
                    mat.SetTexture(propertyName, subAssetTexture);
                    requiresSave = true;
                }
            }

            if (subAssetTexture != null && ApplyGeneratedAssetVisibility(subAssetTexture))
            {
                EditorUtility.SetDirty(subAssetTexture);
                EditorUtility.SetDirty(mat);
                requiresSave = true;
            }

            if (requiresSave)
            {
                Util.DelaySaveAsset(mat);
            }

            var binding = new GeneratedTextureBinding<TData>(mat, data, subAssetTexture);
            return binding;
        }
        
        private TData FetchDataAsset(Material mat, string dataName)
        {
            var subAssets = Util.FetchSubAssets(mat);
            var data = subAssets.OfType<TData>().FirstOrDefault(a => a.name == dataName);
            return data;
        }

        private static bool ApplyGeneratedAssetVisibility(UnityEngine.Object asset)
        {
            const HideFlags generatedAssetHideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            var nextHideFlags = asset.hideFlags | generatedAssetHideFlags;
            if (nextHideFlags == asset.hideFlags) return false;

            asset.hideFlags = nextHideFlags;
            return true;
        }
    }
}
