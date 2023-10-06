
using System;
using System.Linq;
using ExtEditor.UberMaterialPropertyDrawer.Editor;
using UnityEditor;
using UnityEngine;

public class EnumDrawer : MaterialPropertyDrawer
{
    private readonly string _groupName = "";
    private readonly GUIContent[] names;
    private readonly int[] values;
    public EnumDrawer(string groupName, string enumName)
    {
        this._groupName = groupName;
        
        var loadedTypes = TypeCache.GetTypesDerivedFrom(typeof(Enum));
        try
        {
            var enumType = loadedTypes.FirstOrDefault(x => x.Name == enumName || x.FullName == enumName);
            var enumNames = Enum.GetNames(enumType);
            this.names = new GUIContent[enumNames.Length];
            for (int i = 0; i < enumNames.Length; ++i)
                this.names[i] = new GUIContent(enumNames[i]);

            var enumVals = Enum.GetValues(enumType);
            values = new int[enumVals.Length];
            for (var i = 0; i < enumVals.Length; ++i)
                values[i] = (int)enumVals.GetValue(i);
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
        for (int i = 0; i < enumNames.Length; ++i)
            this.names[i] = new GUIContent(enumNames[i]);

        values = new int[vals.Length];
        for (int i = 0; i < vals.Length; ++i)
            values[i] = (int)vals[i];
    }
    static bool IsPropertyTypeSuitable(MaterialProperty prop)
    {
        return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range || prop.type == MaterialProperty.PropType.Int;
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
        
        if (!IsPropertyTypeSuitable(prop))
        {
            GUIContent c = new GUIContent("Enum used on a non-float property: " + prop.name);
            EditorGUI.LabelField(position, c, EditorStyles.helpBox);
            return;
        }

        if (prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range)
        {
            EditorGUI.showMixedValue = prop.hasMixedValue;

            var value = (int)prop.floatValue;
            int selectedIndex = -1;
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
            prop.floatValue = (float)values[selIndex];
        }
        else
        {
            EditorGUI.showMixedValue = prop.hasMixedValue;

            var value = prop.intValue;
            int selectedIndex = -1;
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
            prop.intValue = values[selIndex];
        }
    }
}
