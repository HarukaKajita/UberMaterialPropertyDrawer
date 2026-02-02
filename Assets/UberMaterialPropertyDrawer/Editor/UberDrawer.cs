using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class UberDrawer : UberDrawerBase
    {
        public UberDrawer() : base()
        {
        }
        public UberDrawer(string groupName) : base(groupName)
        {
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;

            MaterialEditor.BeginProperty(position, prop);
            var tmp = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = position.width * 0.5f;
            var labelWithTooltip = Util.MakeLabelWithTooltip(prop);
            // 無限ループしてエディタがクラッシュするのでDefaultShaderPropertyを使用する
            // editor.ShaderProperty(position, prop, ObjectNames.NicifyVariableName(prop.name));
            var rect = EditorGUI.PrefixLabel(position, labelWithTooltip);
            editor.DefaultShaderProperty(rect, prop, "");
            
            EditorGUIUtility.labelWidth = tmp;
            MaterialEditor.EndProperty();
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            if (!IsVisibleInGroup(editor)) return GUIHelper.ClosedHeight;
            // var data = GroupDataCache.GetOrCreate(GetTargetMaterial(editor));
            // if (!UberGroupState.GetGroupExpanded(data, GroupName))
            //     return GUIHelper.ClosedHeight;
            return MaterialEditor.GetDefaultPropertyHeight(prop);
        }
    }
}
