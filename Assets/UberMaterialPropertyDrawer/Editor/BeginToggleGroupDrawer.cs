using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class BeginToggleGroupDrawer : MaterialPropertyDrawer
    {
        private readonly string _groupName = "";
        private readonly int _indentNum = 0;
        private readonly string _parentGroup = "";
        private readonly string _memo;

        public BeginToggleGroupDrawer(string groupName, string parentGroupName = "")
        {
            this._groupName = groupName;
            this._parentGroup = parentGroupName;
            this._indentNum = UberDrawer.GetGroupIntentLevel();
            UberDrawer.PushGroup(groupName);
        }

        private bool ParentIsFolded()
        {
            if (string.IsNullOrEmpty(_parentGroup)) return false;
            return !UberDrawer.GetGroupExpanded(_parentGroup);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            var parentIsFolded = ParentIsFolded();
            UberDrawerLogger.Log("GetPropertyHeight Begin : " + _groupName);
            UberDrawerLogger.Log(_groupName + "のParent" + _parentGroup + "は" + (parentIsFolded ? "閉じてる" : "開いている"));
            if (parentIsFolded) return -2;
            return 22 + 2; //調整した方が良いかも？
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            UberDrawerLogger.Log("OnGUI Begin : " + _groupName);
            var newState = false;
            if (!ParentIsFolded()) newState = BeginPanel(position, prop, UberDrawer.GetGroupExpanded(_groupName));
            EditorGUI.indentLevel++;
            UberDrawer.SetGroupExpanded(_groupName, newState);
        }

        private bool BeginPanel(Rect position, MaterialProperty prop, bool expanded)
        {
            UberDrawerLogger.Log("BeginPanel " + _groupName);
            var style = new GUIStyle("ShurikenModuleTitle");
            style.border = new RectOffset(7, 7, 4, 4); //背景の淵が何故か変わる
            style.fixedHeight = 22; //背景の高さ
            position.y += 6;
            var indentOffset = GUIHelper.IndentWidth * _indentNum;
            var bgRect = new Rect(position.x + indentOffset, position.y, position.width - indentOffset, style.fixedHeight);
            GUI.Box(bgRect, "", style); //背景
            var buttonWidth = 18;
            var buttonLeftMargin = 2;
            var toggleState = prop.intValue != 0;
            
            if(prop.type == MaterialProperty.PropType.Int)
                toggleState = prop.intValue != 0;
            else if(prop.type == MaterialProperty.PropType.Float)
                toggleState = prop.floatValue != 0;
            
            // toggleState = EditorGUI.Toggle(new Rect(bgRect.x + 2, position.y + 0.5f, buttonWidth, 18), toggleState);
            toggleState = GUI.Toggle(new Rect(bgRect.x + 2, position.y + 0.5f, buttonWidth, 18), toggleState, "");
            
            if(prop.type == MaterialProperty.PropType.Int)
                prop.intValue = toggleState ? 1 : 0;
            else if(prop.type == MaterialProperty.PropType.Float)
                prop.floatValue = toggleState ? 1 : 0;
            
            // Group Name Label
            EditorGUI.LabelField(new Rect(position.x + 20f, position.y, 300, 18), this._groupName + ":" + _memo, EditorStyles.boldLabel);
            
            // Clickable Area
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
