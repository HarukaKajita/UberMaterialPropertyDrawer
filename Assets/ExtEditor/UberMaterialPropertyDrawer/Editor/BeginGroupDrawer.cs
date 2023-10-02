using ExtEditor.UberMaterialPropertyDrawer.Editor;
using UnityEditor;
using UnityEngine;

public class BeginGroupDrawer : MaterialPropertyDrawer
{
    private UberDrawer _parentUberDrawer = null;
    private readonly string _groupName = "";
    public BeginGroupDrawer(string groupName, UberDrawer parent)
    {
        this._groupName = groupName;
        this._parentUberDrawer = parent;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        // return base.GetPropertyHeight(prop, label, editor);
        return 22 + 2;//調整した方が良いかも？
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        var newState = BeginPanel(position, prop, UberDrawer.GetGroupExpanded(_groupName));
        UberDrawer.SetGroupExpanded(_groupName, newState);
    }
    private bool BeginPanel( Rect position, MaterialProperty prop, bool expanded)
    {
        // EditorGUI.indentLevel = 0;
            
        var style = new GUIStyle("ShurikenModuleTitle");
        style.border = new RectOffset(7, 7, 4, 4);//背景の淵が何故か変わる
        style.fixedHeight = 22;//背景の高さ
        position.y += 6;
        var bgRect = new Rect(position);
        bgRect.height = 22;
        GUI.Box(bgRect, "", style);//背景
        var interactiveRect = new Rect(bgRect.x + 20, bgRect.y, bgRect.width, bgRect.height);
        var e = Event.current;
        // var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint) {
            EditorStyles.foldout.Draw(new Rect(position.x+2, position.y, 18, 18), false, false, expanded, false);
        }
        if (e.type == EventType.MouseDown && interactiveRect.Contains(e.mousePosition)) {
            expanded = !expanded;
            e.Use();
        }
            
        // prop.floatValue = EditorGUI.Toggle(new Rect(position.x+2, position.y+1, 18, 18), prop.floatValue == 1.0) ? 1 : 0;
        EditorGUI.LabelField(new Rect(position.x + 20f, position.y, 300, 18), this._groupName, EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        return expanded;
    }
}
