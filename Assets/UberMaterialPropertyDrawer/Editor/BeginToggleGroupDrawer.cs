using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class BeginToggleGroupDrawer : UberDrawerBase
    {
        private string _memo;

        public BeginToggleGroupDrawer(string groupName) : base(groupName)
        {
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            var data = GetGroupData(editor);
            var beginGroup = TryBeginGroup(editor, prop);
            if (beginGroup) BeginGroupScope(editor);
            
            var parentPath         = UberGroupState.GetParentPath(editor);
            var parentVisible = UberGroupState.IsParentScopeVisible(data, editor);
            var groupPath = UberGroupState.GetCurrentPath(editor);
            
            UberDrawerLogger.Log($"{GetType().Name}({GroupName}).GetPropertyHeight()");
            UberDrawerLogger.Log($"\t{nameof(beginGroup)}:{beginGroup}");
            UberDrawerLogger.Log($"\t{nameof(parentPath)}:{parentPath}");
            UberDrawerLogger.Log($"\t{nameof(groupPath)}:{groupPath}");
            UberDrawerLogger.Log($"\t{nameof(parentVisible)}:{parentVisible}");
            UberDrawerLogger.Log(GroupName + "のParent" + parentPath + "は" + (parentVisible ? "可視" : "不可視"));
            return GroupVisibility.CanShowHeader(data, editor) ? GUIHelper.GroupHeaderHeight : GUIHelper.ClosedHeight;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            var data = GetGroupData(editor);
            var beginGroup = TryBeginGroup(editor, prop);
            if (beginGroup) BeginGroupScope(editor);
            
            var parentPath         = UberGroupState.GetParentPath(editor);
            var parentVisible = UberGroupState.IsParentScopeVisible(data, editor);
            var groupPath = UberGroupState.GetCurrentPath(editor);
            
            UberDrawerLogger.Log($"{GetType().Name}({GroupName}).OnGUI()");
            UberDrawerLogger.Log($"\t{nameof(beginGroup)}:{beginGroup}");
            UberDrawerLogger.Log($"\t{nameof(parentPath)}:{parentPath}");
            UberDrawerLogger.Log($"\t{nameof(groupPath)}:{groupPath}");
            UberDrawerLogger.Log($"\t{nameof(parentVisible)}:{parentVisible}");
            _memo = $" parent:{parentPath}";
            UberDrawerLogger.Log(GroupName + "のParent" + parentPath + "は" + (parentVisible ? "可視" : "不可視"));
            var state = UberGroupState.GetExpanded(data, groupPath);
            if (GroupVisibility.CanShowHeader(data, editor))
            {
                var newState = BeginPanel(position, editor, prop, state);
                if(state != newState)
                    UberGroupState.SetExpanded(data, groupPath, newState);
                EditorGUI.indentLevel++;
            }
        }

        private bool BeginPanel(Rect position, MaterialEditor editor, MaterialProperty prop, bool expanded)
        {
            UberDrawerLogger.Log($"BeginPanel : {GroupName} - {_memo}");
            MaterialEditor.BeginProperty(position,prop);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            
            var style = new GUIStyle("ShurikenModuleTitle");
            style.border = new RectOffset(7, 7, 4, 4); // Background edge tweaks.
            style.fixedHeight = GUIHelper.GroupHeaderHeight; // Background height.
            position.y += GUIHelper.GroupHeaderTopPadding;
            var bgRect = EditorGUI.IndentedRect(position);
            bgRect.height = style.fixedHeight;
            GUI.Box(bgRect, "", style); // background
            
            var toggleState = Util.GetAsBool(prop);
            var buttonWidth = 18;
            var buttonLeftMargin = 2;
            var toggleRect = position;
            toggleRect.x += buttonLeftMargin;
            toggleRect.width = toggleRect.height = buttonWidth;
            // EditorGUIは自動で内部でインデントを自動処理する。
            // xを増やして、その分幅を狭くすることでインデントを表現する。
            // 幅が狭くなる影響でトグルのインタラクション領域がなくなって機能しなくなる。
            // インデントで狭くなる分の幅を予め余分に加えておくことで、幅が狭くなる影響を防ぐ。
            toggleRect.width += EditorGUI.indentLevel * GUIHelper.IndentWidth;
            toggleState = EditorGUI.Toggle(toggleRect, toggleState);

            var labelRect = toggleRect;
            labelRect.x += buttonWidth + buttonLeftMargin;
            labelRect.width = 300;
            labelRect.height = 18;
            EditorGUI.LabelField(labelRect, GroupName + ":" + _memo, EditorStyles.boldLabel);

            var interactiveRect = bgRect;
            interactiveRect.x += buttonWidth + buttonLeftMargin;
            interactiveRect.width -= buttonWidth + buttonLeftMargin;
            var e = Event.current;
            if (e.type == EventType.MouseDown && interactiveRect.Contains(e.mousePosition))
            {
                expanded = !expanded;
                e.Use();
            }
            
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                editor.RegisterPropertyChangeUndo(prop.name);
                Util.SetBool(prop, toggleState);
            }
            MaterialEditor.EndProperty();
            
            return expanded;
        }

        private static bool TryBeginGroup(MaterialEditor editor, MaterialProperty prop)
        {
            return UberGroupState.TryRecordPush(editor, prop?.name);
        }
    }
}
