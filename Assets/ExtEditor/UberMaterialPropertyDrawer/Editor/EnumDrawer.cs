
using System;
using System.Linq;
using ExtEditor.UberMaterialPropertyDrawer.Editor;
using UnityEditor;
using UnityEngine;

public class EnumDrawer : MaterialPropertyDrawer
{
    private readonly string _groupName = "";
    private readonly GUIContent[] names;
    private readonly float[] values;
    public EnumDrawer(string groupName, string enumName)
    {
        this._groupName = groupName;
        
        var loadedTypes = TypeCache.GetTypesDerivedFrom(typeof(Enum));
        try
        {
            Debug.Log(loadedTypes.Count + enumName);
            var enumType = loadedTypes.FirstOrDefault(x => x.Name == enumName || x.FullName == enumName);
            var enumNames = Enum.GetNames(enumType);
            this.names = new GUIContent[enumNames.Length];
            for (int i = 0; i < enumNames.Length; ++i)
                this.names[i] = new GUIContent(enumNames[i]);
            var enumVals = Enum.GetValues(enumType);
            values = new float[enumVals.Length];
            for (var i = 0; i < enumVals.Length; ++i)
                values[i] = (int)enumVals.GetValue(i);//Arrayの要素をfloatにするとこける
            
        }
        catch (Exception)
        {
            Debug.LogWarningFormat("Failed to create MaterialEnum, enum {0} not found", enumName);
            throw;
        }
    }
    
    public EnumDrawer(string groupName, string[] enumNames, float[] vals)
    {
        this._groupName = groupName;
        
        this.names = new GUIContent[enumNames.Length];
        for (var i = 0; i < enumNames.Length; ++i)
            this.names[i] = new GUIContent(enumNames[i]);

        values = new float[vals.Length];
        for (var i = 0; i < vals.Length; ++i)
            values[i] = (float)vals[i];
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
        if (!UberDrawer.GetGroupExpanded(_groupName))return;
        
        var propName = ObjectNames.NicifyVariableName(label.text);
        EditorGUI.showMixedValue = prop.hasMixedValue;
        var value = prop.floatValue;
        var selectedIndex = -1;
        for (var index = 0; index < values.Length; index++)
        {
            var i = values[index];
            if (i == value)
            {
                selectedIndex = index;
                break;
            }
        }
        var selIndex = EditorGUI.Popup(position, label, selectedIndex, names);
        EditorGUI.showMixedValue = false;
        // prop.intValue = values[selIndex];
        prop.floatValue = (float)values[selIndex];
    }
}
