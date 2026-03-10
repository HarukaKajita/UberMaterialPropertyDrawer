using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal sealed class GradientTextureDrawer: GeneratedTextureDrawerBase<GradientData, GeneratedTextureOptions>
    {
        public GradientTextureDrawer(string groupName, params string[] args) : base(groupName, args)
        {
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.TexturePropertyHeight, editor);
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
            EditorGUI.PropertyField(position, gradientSp, GUIContent.none);
            position.y += position.height;
            so.ApplyModifiedProperties();
        }

        protected override void Bake(GradientData data, ref Texture2D tex, GeneratedTextureOptions options, string texName)
        {
            data.BakeTo(ref tex, options.Resolution, options.ResolveTextureFormat(), texName);
        }
    }
}
