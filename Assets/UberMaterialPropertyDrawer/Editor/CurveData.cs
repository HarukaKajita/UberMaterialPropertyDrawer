using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal class CurveData : ScriptableObject
    {
        // don't rename this field. It is used by CurveTextureDrawer.cs as SerializedProperty
        public AnimationCurve curveR = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveG = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveB = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveA = AnimationCurve.Linear(0, 0, 1, 1);

        public void BakeTo(ref Texture2D tex, int resolution, bool accumulate, bool useHalfTexture, TextureFormat format, string texName)
        {
            if (tex == null)
            {
                tex = new Texture2D(resolution, 1, format, true, true);
                tex.name = texName;
            }
            else if (tex.width != resolution || tex.height != 1 || tex.format != format)
                tex.Reinitialize(resolution, 1, format, true);

            float accR = 0, accG = 0, accB = 0, accA = 0;
            var colors = new Color[resolution];
            for (var i = 0; i < resolution; i++)
            {
                var t = (float)i / (resolution - 1);
                var r = curveR.Evaluate(t);
                var g = curveG.Evaluate(t);
                var b = curveB.Evaluate(t);
                var a = curveA.Evaluate(t);
                if (accumulate)
                {
                    accR += r; r = accR;
                    accG += g; g = accG;
                    accB += b; b = accB;
                    accA += a; a = accA;
                }
                if (!useHalfTexture)
                {
                    r = Mathf.Clamp01(r);
                    g = Mathf.Clamp01(g);
                    b = Mathf.Clamp01(b);
                    a = Mathf.Clamp01(a);
                }
                colors[i] = new Color(r, g, b, a);
            }
            tex.SetPixels(colors);
            tex.Apply();
        }
    }
}
