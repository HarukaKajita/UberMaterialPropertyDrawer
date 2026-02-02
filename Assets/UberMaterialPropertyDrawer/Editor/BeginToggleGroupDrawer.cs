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

        private static bool ParentIsClosed(GroupData data, string parentGroup)
        {
            if (string.IsNullOrEmpty(parentGroup)) return false;
            return !UberGroupState.GetGroupExpanded(data, parentGroup);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            var data = GetGroupData(editor);
            if (TryBeginGroup(data, prop))
                BeginGroupScope(editor);
            var parentGroup = GetParentGroup(editor,  GroupName);
            var parentIsClosed = ParentIsClosed(data, parentGroup);
            UberDrawerLogger.Log(GroupName + "のParent" + parentGroup + "は" + (parentIsClosed ? "閉じてる" : "開いている"));
            return parentIsClosed ? GUIHelper.ClosedHeight : GUIHelper.GroupHeaderHeight;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            UberDrawerLogger.Log("OnGUI Begin : " + GroupName);
            var data = GetGroupData(editor);
            if (TryBeginGroup(data, prop))
                BeginGroupScope(editor);
            var parentGroup = GetParentGroup(editor, GroupName);
            var parentIsClosed = ParentIsClosed(data, parentGroup);
            _memo = $" parent:{parentGroup}";
            UberDrawerLogger.Log(GroupName + "のParent" + parentGroup + "は" + (parentIsClosed ? "閉じてる" : "開いている"));
            var newState = false;
            if (!parentIsClosed) newState = BeginPanel(position, editor, prop, UberGroupState.GetGroupExpanded(data, GroupName));
            EditorGUI.indentLevel++;
            UberGroupState.SetGroupExpanded(data, GroupName, newState);
            
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

        private static bool TryBeginGroup(GroupData data, MaterialProperty prop)
        {
            return UberGroupState.TryRecordPush(data, prop?.name);
        }
    }
}
