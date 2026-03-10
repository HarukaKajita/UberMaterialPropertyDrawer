using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
	/// <summary>
	/// MaterialのプロタゴラスのTDataから作成されたTextureの組を保持するクラス
	/// </summary>
	/// <typeparam name="TData"></typeparam>
	internal sealed class GeneratedTextureBinding<TData> where TData : ScriptableObject
	{
		public Material Material { get; }
		public TData Data { get; }
		public Texture2D Texture { get; }
		
		public GeneratedTextureBinding(Material material, TData data, Texture2D texture)
		{
			Material = material;
			Data = data;
			Texture = texture;
		}
	}
}