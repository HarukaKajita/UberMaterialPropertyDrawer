using ExtEditor.UberMaterialPropertyDrawer.Editor;
using UnityEditor;
using UnityEngine;

public class Vector2Drawer : MaterialPropertyDrawer
{
    private readonly string _groupName = "";
    
    public Vector2Drawer(string groupName)
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

        if (UberDrawer.GetGroupExpanded(_groupName))
        {
            var propName = ObjectNames.NicifyVariableName(label.text);
            var labelWidth = position.width *0.45f;
            var valueWidth = position.width - labelWidth;
            var tmp_labelWidth = EditorGUIUtility.labelWidth;
            var tmp_fieldWidth = EditorGUIUtility.fieldWidth;
            
            // EditorGUIUtility.wideMode = true;
            // EditorGUIUtility.labelWidth = labelWidth;
            // EditorGUIUtility.fieldWidth = 0;
            
            var labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            var valueRect = new Rect(position.x + labelWidth, position.y, valueWidth, position.height);
            valueRect.xMax = position.xMax;
            EditorGUI.DrawRect(position, Color.green);
            EditorGUI.DrawRect(labelRect, Color.red);
            EditorGUI.DrawRect(valueRect, Color.blue);
            EditorGUI.LabelField(labelRect, propName);
            // EditorGUIUtility.labelWidth = 0;
            // EditorGUIUtility.fieldWidth = valueWidth;
            prop.vectorValue = EditorGUI.Vector2Field(valueRect, GUIContent.none, prop.vectorValue);
            editor.VectorProperty(prop, propName);
            // prop.vectorValue = EditorGUI.Vector2Field(position, propName, prop.vectorValue);
            EditorGUIUtility.labelWidth = tmp_labelWidth;
            EditorGUIUtility.fieldWidth = tmp_fieldWidth;
            EditorGUIUtility.wideMode = false;
            

        }
    }
}
