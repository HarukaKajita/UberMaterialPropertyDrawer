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
            var indentNum = UberGroupState.GetIndentLevel(data);
            if (TryBeginGroup(data, prop))
                BeginGroupScope(editor);
            var parentGroup = GetParentGroup(editor, GroupName);
            var parentIsClosed = ParentIsClosed(data, parentGroup);
            _memo = $" parent:{parentGroup}";
            UberDrawerLogger.Log(GroupName + "のParent" + parentGroup + "は" + (parentIsClosed ? "閉じてる" : "開いている"));
            var newState = false;
            if (!parentIsClosed) newState = BeginPanel(position, prop, UberGroupState.GetGroupExpanded(data, GroupName), indentNum);
            EditorGUI.indentLevel++;
            UberGroupState.SetGroupExpanded(data, GroupName, newState);
            
        }

        private bool BeginPanel(Rect position, MaterialProperty prop, bool expanded, int indentNum)
        {
            UberDrawerLogger.Log($"BeginPanel : {GroupName} - {_memo}");
            var style = new GUIStyle("ShurikenModuleTitle");
            style.border = new RectOffset(7, 7, 4, 4); // Background edge tweaks.
            style.fixedHeight = GUIHelper.GroupHeaderHeight; // Background height.
            position.y += GUIHelper.GroupHeaderTopPadding;
            var indentOffset = GUIHelper.IndentWidth * indentNum;
            var bgRect = new Rect(position.x + indentOffset, position.y, position.width - indentOffset, style.fixedHeight);
            GUI.Box(bgRect, "", style);
            var buttonWidth = 18;
            var buttonLeftMargin = 2;

            var toggleState = Util.GetAsBool(prop);
            toggleState = GUI.Toggle(new Rect(bgRect.x + 2, position.y + 0.5f, buttonWidth, 18), toggleState, "");
            Util.SetBool(prop, toggleState);

            EditorGUI.LabelField(new Rect(position.x + 20f, position.y, 300, 18), GroupName + ":" + _memo, EditorStyles.boldLabel);

            var interactiveRect = bgRect;
            interactiveRect.x += buttonWidth + buttonLeftMargin;
            interactiveRect.width -= buttonWidth + buttonLeftMargin;
            var e = Event.current;
            if (e.type == EventType.MouseDown && interactiveRect.Contains(e.mousePosition))
            {
                expanded = !expanded;
                e.Use();
            }
            return expanded;
        }

        private static bool TryBeginGroup(GroupData data, MaterialProperty prop)
        {
            return UberGroupState.TryRecordPush(data, prop?.name);
        }
    }
}
