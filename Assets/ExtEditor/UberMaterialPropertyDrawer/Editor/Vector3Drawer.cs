
using ExtEditor.UberMaterialPropertyDrawer.Editor;
using UnityEditor;
using UnityEngine;

public class Vector3Drawer : MaterialPropertyDrawer
{
    private readonly string _groupName = "";
    
    public Vector3Drawer(string groupName)
    {
        this._groupName = groupName;
    }
    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        if (UberDrawer.GetGroupExpanded(_groupName))
            return EditorGUIUtility.singleLineHeight;
        else
            return 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        if (!UberDrawer.GetGroupExpanded(_groupName)) return;
        
        var propName = ObjectNames.NicifyVariableName(label.text);
        var totalIndentSize = EditorGUI.indentLevel * 15;
        var labelWidth = position.width *0.3f;
        var valueWidth = position.width - labelWidth + totalIndentSize;
        var tmp_labelWidth = EditorGUIUtility.labelWidth;
        var tmp_fieldWidth = EditorGUIUtility.fieldWidth;
        var valueX = position.x + labelWidth - totalIndentSize;
            
        var labelRect = new Rect(position.x, position.y, labelWidth, position.height);
        var valueRect = new Rect(valueX, position.y, valueWidth, position.height);
        // EditorGUI.DrawRect(position, Color.green);
        // EditorGUI.DrawRect(labelRect, Color.red);
        // EditorGUI.DrawRect(valueRect, Color.blue);
        EditorGUI.LabelField(labelRect, propName);
        prop.vectorValue = EditorGUI.Vector3Field(valueRect, GUIContent.none, prop.vectorValue);
        EditorGUIUtility.labelWidth = tmp_labelWidth;
        EditorGUIUtility.fieldWidth = tmp_fieldWidth;
        EditorGUIUtility.wideMode = false;
    }
}
