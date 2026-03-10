using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ExtEditor.UberMaterialPropertyDrawer
{
	/// <summary>
	/// Materialのサブアセットの探索、生成、保存を管理するクラス
	/// </summary>
	/// <typeparam name="TData"></typeparam>
	internal sealed class MaterialSubAssetStore<TData> where TData : ScriptableObject
	{
		public GeneratedTextureBinding<TData>[] EnsureBindings(Material[] materials, string propertyName, string dataAssetName, string textureAssetName)
		{
			if (materials == null || materials.Length == 0) return null;
			return materials.Select(mat => EnsureBindings(mat, propertyName, dataAssetName, textureAssetName)).ToArray();
		}

		/// <summary>
		/// 探索・不足次作成・整合・保存予約
		/// </summary>
		/// <param name="mat"></param>
		/// <param name="propertyName"></param>
		/// <param name="dataAssetName"></param>
		/// <param name="textureAssetName"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
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
			
			// テクスチャ化するデータそのものがない場合は作成してサブアセット化
			var data = FetchDataAsset(mat, dataAssetName);
			if (data == null)
			{
				data = ScriptableObject.CreateInstance<TData>();
				data.name = dataAssetName;
				data.hideFlags |= HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(data, mat);
				EditorUtility.SetDirty(data);
				EditorUtility.SetDirty(mat);
				Util.DelaySaveAsset(mat);
			}
			// Textureが初期化されているか確認。subAssetのテクスチャとMaterialのテクスチャが一致する状態にする
			var subAssetTexture = Util.FetchSubAssetTexture(mat, textureAssetName);
			var materialTexture = mat.GetTexture(propertyName);
			if (subAssetTexture == null || subAssetTexture != materialTexture)
			{
				if (subAssetTexture != null)
				{
					mat.SetTexture(propertyName, subAssetTexture);
					EditorUtility.SetDirty(mat);
				}
				else
				{
					// SubAssetTextureがない場合は作成してサブアセット化
					var texture = new Texture2D(1,1,TextureFormat.RGBA32, false, true);
					texture.name = textureAssetName;
					texture.hideFlags |= HideFlags.HideInHierarchy;
					AssetDatabase.AddObjectToAsset(texture, mat);
					EditorUtility.SetDirty(texture);
					EditorUtility.SetDirty(mat);
					Util.DelaySaveAsset(mat);
					subAssetTexture = texture;
					mat.SetTexture(propertyName, subAssetTexture);
				}
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
	}
}