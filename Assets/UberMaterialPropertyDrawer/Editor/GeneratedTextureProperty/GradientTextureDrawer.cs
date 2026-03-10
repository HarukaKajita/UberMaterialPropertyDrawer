using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal sealed class GradientTextureDrawer: GeneratedTextureDrawerBase<GradientData, GeneratedTextureOptions>
    {
        protected override int DetailLineCount => 1;

        public GradientTextureDrawer(string groupName, params string[] args) : base(groupName, args)
        {
        }
        
        protected override GeneratedTextureOptions ParseOptions(string[] args)
        {
            var chNum = 1;
            var res = 256;
            var bit = 8;
            if (args == null || args.Length == 0) return new GeneratedTextureOptions();
            foreach (var argStr in args)
            {
                if (argStr.StartsWith("ch")) chNum = int.Parse(argStr[2..]);
                else if (argStr.StartsWith("res")) res = int.Parse(argStr[3..]);
                else if (argStr.StartsWith("bit")) bit = int.Parse(argStr[3..]);
            }

            return new GeneratedTextureOptions(res, chNum, bit == 16);
        }

        protected override string GetDataName(string propName)
        {
            return propName + "_GradientData";
        }

        protected override string GetTextureName(string propName)
        {
            return propName + "_GradientTex";
        }

        protected override bool HasTextureOptionMismatch(Texture2D[] textures, GeneratedTextureOptions options)
        {
            foreach (var tex in textures)
            {
                if (tex == null) return true;

                var correctFormat = options.ResolveTextureFormat();
                var res = options.Resolution;
                if (tex.width != res || tex.height != 1 || tex.format != correctFormat)
                    return true;
            }
            return false;
        }

        protected override void DrawDetailFields(Rect position, SerializedObject so)
        {
            var gradientSp = so.FindProperty("gradient");
            var lineRect = new Rect(position.x, position.y, position.width, GUIHelper.SingleLineHeight);
            EditorGUI.PropertyField(lineRect, gradientSp, GUIContent.none);
        }

        protected override void Bake(GradientData data, ref Texture2D tex, GeneratedTextureOptions options, string texName)
        {
            data.BakeTo(ref tex, options.Resolution, options.ResolveTextureFormat(), texName, data.UsesLinearColorSpace);
        }
    }
}
