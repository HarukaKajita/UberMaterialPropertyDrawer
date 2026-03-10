using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// Holds the generated data and texture bound to a material.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    internal sealed class GeneratedTextureBinding<TData> where TData : GeneratedTextureDataBase
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
