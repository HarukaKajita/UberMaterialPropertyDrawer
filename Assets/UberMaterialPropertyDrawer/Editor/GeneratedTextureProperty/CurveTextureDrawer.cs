using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal sealed class CurveTextureDrawer: GeneratedTextureDrawerBase<CurveData, CurveTextureOptions>
    {
        public CurveTextureDrawer(string groupName, params string[] args) : base(groupName, args)
        {
        }
        
        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.TexturePropertyHeight, editor);
        }

        protected override CurveTextureOptions ParseOptions(string[] args)
        {
            var chNum = 1;
            var res = 256;
            var bit = 8;
            var accum = false;
            if (args == null || args.Length == 0) return new CurveTextureOptions();
            foreach (var argStr in args)
            {
                if (argStr.StartsWith("ch")) chNum = int.Parse(argStr[2..]);
                else if (argStr.StartsWith("res")) res = int.Parse(argStr[3..]);
                else if (argStr.StartsWith("bit")) bit = int.Parse(argStr[3..]);
                else if (argStr == "accum") accum = true;
            }

            return new CurveTextureOptions(new GeneratedTextureOptions(res, chNum, bit == 16), accum);
        }

        protected override string GetDataName(string propName)
        {
            return propName + "_CurveData";
        }

        protected override string GetTextureName(string propName)
        {
            return propName + "_CurveTex";
        }

        protected override bool HasTextureOptionMismatch(Texture2D[] textures, CurveTextureOptions options)
        {
            foreach (var tex in textures)
            {
                var correctFormat = options.CommonOptions.ResolveTextureFormat();
                var res = options.CommonOptions.Resolution;
                if (tex.width != res || tex.height != 1 || tex.format != correctFormat)
                    return true;
            }
            return false;
        }

        protected override void DrawDetailFields(Rect position, SerializedObject so)
        {
            var curveRSp = so.FindProperty("curveR");
            var curveGSp = so.FindProperty("curveG");
            var curveBSp = so.FindProperty("curveB");
            var curveASp = so.FindProperty("curveA");
            EditorGUI.PropertyField(position, curveRSp, GUIContent.none);
            position.y += position.height;
            EditorGUI.PropertyField(position, curveGSp, GUIContent.none);
            position.y += position.height;
            EditorGUI.PropertyField(position, curveBSp, GUIContent.none);
            position.y += position.height;
            EditorGUI.PropertyField(position, curveASp, GUIContent.none);
            position.y += position.height;
            so.ApplyModifiedProperties();
        }

        protected override void Bake(CurveData data, ref Texture2D tex, CurveTextureOptions options, string texName)
        {
            var res = options.CommonOptions.Resolution;
            var accum = options.Accumulate;
            var useHalfTex = options.CommonOptions.UseHalfTexture;
            var format = options.CommonOptions.ResolveTextureFormat();
            data.BakeTo(ref tex, res, accum, useHalfTex, format, texName);
        }
    }
}
