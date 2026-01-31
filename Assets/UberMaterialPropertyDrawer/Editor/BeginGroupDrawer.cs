using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class BeginGroupDrawer : UberDrawerBase
    {
        private readonly int _indentNum = 0;
        private readonly string _parentGroup = "";
        private string _memo;

        public BeginGroupDrawer(string groupName) : base(groupName)
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
            if (ParentIsClosed()) return GUIHelper.ClosedHeight;
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
            var bgRect = new Rect(position.x + indentOffset, position.y, position.width - indentOffset,
                style.fixedHeight);
            GUI.Box(bgRect, "", style); // Background.
            var interactiveRect = new Rect(bgRect.x, bgRect.y, bgRect.width, bgRect.height);
            var e = Event.current;
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(new Rect(position.x + 2 + indentOffset, position.y, 18, 18), false, false,
                    expanded, false);
            }

            if (e.type == EventType.MouseDown && interactiveRect.Contains(e.mousePosition))
            {
                expanded = !expanded;
                e.Use();
            }

            EditorGUI.LabelField(
                new Rect(position.x + 18, position.y, 300, 18),
                GroupName + ":" + _memo,
                EditorStyles.boldLabel);

            return expanded;
        }
    }
}
