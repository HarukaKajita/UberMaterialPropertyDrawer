using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal class GradientData : ScriptableObject
    {
        // don't rename this field. It is used by GradientTextureDrawer.cs as SerializedProperty.
        public Gradient gradient = new Gradient();
        
        public void BakeTo(ref Texture2D tex, int resolution, TextureFormat format, string texName)
        {
            if (tex == null)
            {
                tex = new Texture2D(resolution, 1, format, true, true);
                tex.name = texName;
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
