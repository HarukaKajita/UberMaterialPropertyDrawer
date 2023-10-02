using ExtEditor.UberMaterialPropertyDrawer.Editor;
using UnityEditor;
using UnityEngine;

public class EndToggleGroupDrawer : MaterialPropertyDrawer
{
    private UberDrawer _parentUberDrawer = null;
    private readonly string _groupName = "";
    public EndToggleGroupDrawer(string groupName, UberDrawer parent)
    {
        this._groupName = groupName;
        this._parentUberDrawer = parent;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        // return base.GetPropertyHeight(prop, label, editor);
        return 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        EndPanel();
    }

    private void EndPanel()
    {
        EditorGUI.indentLevel -= 1;
    }
}
