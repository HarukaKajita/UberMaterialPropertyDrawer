using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class BeginGroupDrawer : MaterialPropertyDrawer
    {
        private readonly string _groupName = "";
        private readonly int _indentNum = 0;
        private readonly string _parentGroup = "";
        private string _memo;

        public BeginGroupDrawer(string groupName, string parentGroupName = "")
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
            Debug.Log("GetPropertyHeight Begin : " + _groupName);
            if (ParentIsFolded()) return -2;
            return 22 + 2; //調整した方が良いかも？
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            Debug.Log("OnGUI Begin : " + _groupName);
            var newState = false;
            if (!ParentIsFolded()) newState = BeginPanel(position, prop, UberDrawer.GetGroupExpanded(_groupName));
            EditorGUI.indentLevel++;
            UberDrawer.SetGroupExpanded(_groupName, newState);
        }

        private bool BeginPanel(Rect position, MaterialProperty prop, bool expanded)
        {
            Debug.Log("BeginPanel " + _groupName);
            var style = new GUIStyle("ShurikenModuleTitle");
            style.border = new RectOffset(7, 7, 4, 4); //背景の淵が何故か変わる
            style.fixedHeight = 22; //背景の高さ
            position.y += 6;
            const int indentSize = 15;
            var indentOffset = indentSize * _indentNum;
            var bgRect = new Rect(position.x + indentOffset, position.y, position.width - indentOffset,
                style.fixedHeight);
            GUI.Box(bgRect, "", style); //背景
            var interactiveRect = new Rect(bgRect.x, bgRect.y, bgRect.width, bgRect.height);
            var e = Event.current;
            // var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            var x0 = position.x + 2 + indentOffset;
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

            // prop.floatValue = EditorGUI.Toggle(new Rect(position.x+2, position.y+1, 18, 18), prop.floatValue == 1.0) ? 1 : 0;
            // var x1 = position.x+18;
            // _memo = _indentNum.ToString() + "position.x=" + x0 +"->"+x1;
            EditorGUI.LabelField(
                new Rect(position.x + 18, position.y, 300, 18),
                this._groupName + ":" + _memo,
                EditorStyles.boldLabel);

            return expanded;
        }
    }
}
