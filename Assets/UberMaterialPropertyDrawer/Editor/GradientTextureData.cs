using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal class GradientTextureData : ScriptableObject
    {
        public Gradient gradient = new Gradient();
        [HideInInspector] public Texture2D texture;
        
        public void BakeTexture(int resolution, TextureFormat format, string texName)
        {
            var tex = texture;
            if (tex == null)
            {
                tex = new Texture2D(resolution, 1, format, true, true);
                tex.name = texName;
                texture = tex;
            }
            else if (tex.width != resolution || tex.height != 1 || tex.format != format)
                tex.Reinitialize(resolution, 1, format, true);

            var colors = new Color[resolution];
            for (var i = 0; i < resolution; i++)
            {
                var t = (float)i / (resolution - 1);
                colors[i] = gradient.Evaluate(t);
            }
            tex.SetPixels(colors);
            tex.Apply();
        }
    }
}
