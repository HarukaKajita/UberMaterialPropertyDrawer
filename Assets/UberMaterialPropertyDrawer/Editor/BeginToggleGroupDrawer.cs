using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class BeginToggleGroupDrawer : UberDrawerBase
    {
        private readonly int _indentNum = 0;
        private readonly string _parentGroup = "";
        private readonly string _memo;

        public BeginToggleGroupDrawer(string groupName) : base(groupName)
        {
            _indentNum = IndentLevel;
            _parentGroup = TryGetParentGroup(out var parentGroup) ? parentGroup : string.Empty;
            BeginGroupScope();
        }

        private bool ParentIsClosed()
        {
            if (string.IsNullOrEmpty(_parentGroup)) return false;
            return !UberGroupState.GetGroupExpanded(_parentGroup);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            var parentIsClosed = ParentIsClosed();
            UberDrawerLogger.Log("GetPropertyHeight Begin : " + GroupName);
            UberDrawerLogger.Log(GroupName + "のParent" + _parentGroup + "は" + (parentIsClosed ? "閉じてる" : "開いている"));
            if (parentIsClosed) return GUIHelper.ClosedHeight;
            return GUIHelper.GroupHeaderHeight;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            UberDrawerLogger.Log("OnGUI Begin : " + GroupName);
            var newState = false;
            if (!ParentIsClosed()) newState = BeginPanel(position, prop, UberGroupState.GetGroupExpanded(GroupName));
            EditorGUI.indentLevel++;
            UberGroupState.SetGroupExpanded(GroupName, newState);
        }

        private bool BeginPanel(Rect position, MaterialProperty prop, bool expanded)
        {
            UberDrawerLogger.Log("BeginPanel " + GroupName);
            var style = new GUIStyle("ShurikenModuleTitle");
            style.border = new RectOffset(7, 7, 4, 4); // Background edge tweaks.
            style.fixedHeight = GUIHelper.GroupHeaderHeight; // Background height.
            position.y += GUIHelper.GroupHeaderTopPadding;
            var indentOffset = GUIHelper.IndentWidth * _indentNum;
            var bgRect = new Rect(position.x + indentOffset, position.y, position.width - indentOffset, style.fixedHeight);
            GUI.Box(bgRect, "", style);
            var buttonWidth = 18;
            var buttonLeftMargin = 2;

            var toggleState = Util.ToBool(prop);
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
    }
}
