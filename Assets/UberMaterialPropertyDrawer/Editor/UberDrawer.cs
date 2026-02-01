using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class UberDrawer : MaterialPropertyDrawer
    {
        private readonly string _groupName = string.Empty;

        public UberDrawer(string groupName)
        {
            _groupName = groupName ?? string.Empty;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            var data = GroupDataCache.GetOrCreate(UberDrawerBase.GetTargetMaterial(editor));
            if (!UberGroupState.GetGroupExpanded(data, _groupName))
                return;

            MaterialEditor.BeginProperty(position, prop);
            // 無限ループしてエディタがクラッシュするのでDefaultShaderPropertyを使用する
            // editor.ShaderProperty(position, prop, ObjectNames.NicifyVariableName(prop.name));
            editor.DefaultShaderProperty(position, prop, prop.displayName);
            MaterialEditor.EndProperty();
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            var data = GroupDataCache.GetOrCreate(UberDrawerBase.GetTargetMaterial(editor));
            if (!UberGroupState.GetGroupExpanded(data, _groupName))
                return GUIHelper.ClosedHeight;
            return MaterialEditor.GetDefaultPropertyHeight(prop);
        }
    }
}
