using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class UberEnumDrawer : UberDrawerBase
    {
        private readonly GUIContent[] _names;
        private readonly int[] _values;
        private readonly string _errorLabel;

        public UberEnumDrawer(string groupName, string enumName) : base(groupName)
        {
            try
            {
                var loadedTypes = TypeCache.GetTypesDerivedFrom(typeof(Enum));
                var enumType = loadedTypes.FirstOrDefault(x => x.Name == enumName || x.FullName == enumName);
                var enumNames = Enum.GetNames(enumType);
                _names = new GUIContent[enumNames.Length];
                for (int i = 0; i < enumNames.Length; ++i)
                    _names[i] = new GUIContent(enumNames[i]);

                var enumVals = Enum.GetValues(enumType);
                _values = new int[enumVals.Length];
                for (var i = 0; i < enumVals.Length; ++i)
                    _values[i] = (int)enumVals.GetValue(i);
            }
            catch (Exception)
            {
                _errorLabel = $"Failed to create MaterialEnum, enum {enumName} not found";
                UberDrawerLogger.LogError(_errorLabel);
                _names = Array.Empty<GUIContent>();
                _values = Array.Empty<int>();
            }
        }

        // name,value,name,value,... pairs: explicit names & values
        public UberEnumDrawer(string groupName, string n1, float v1) : this(groupName, new[] { n1 }, new[] { v1 })
        {
        }

        public UberEnumDrawer(string groupName, string n1, float v1, string n2, float v2) : this(groupName,
            new[] { n1, n2 }, new[] { v1, v2 })
        {
        }

        public UberEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3) : this(
            groupName, new[] { n1, n2, n3 }, new[] { v1, v2, v3 })
        {
        }

        public UberEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4,
            float v4) : this(groupName, new[] { n1, n2, n3, n4 }, new[] { v1, v2, v3, v4 })
        {
        }

        public UberEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4,
            float v4, string n5, float v5) : this(groupName, new[] { n1, n2, n3, n4, n5 }, new[] { v1, v2, v3, v4, v5 })
        {
        }

        public UberEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4,
            float v4, string n5, float v5, string n6, float v6) : this(groupName, new[] { n1, n2, n3, n4, n5, n6 },
            new[] { v1, v2, v3, v4, v5, v6 })
        {
        }

        public UberEnumDrawer(string groupName, string n1, float v1, string n2, float v2, string n3, float v3, string n4,
            float v4, string n5, float v5, string n6, float v6, string n7, float v7) : this(groupName,
            new[] { n1, n2, n3, n4, n5, n6, n7 }, new[] { v1, v2, v3, v4, v5, v6, v7 })
        {
        }

        private UberEnumDrawer(string groupName, string[] enumNames, float[] vals) : base(groupName)
        {
            _names = new GUIContent[enumNames.Length];
            for (int i = 0; i < enumNames.Length; ++i)
                _names[i] = new GUIContent(enumNames[i]);

            _values = new int[vals.Length];
            for (int i = 0; i < vals.Length; ++i)
                _values[i] = (int)vals[i];
        }

        private static bool IsPropertyTypeSuitable(MaterialProperty prop)
        {
            return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range ||
                   prop.type == MaterialProperty.PropType.Int;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.SingleLineHeight, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;

            if (!IsPropertyTypeSuitable(prop))
            {
                GUIContent c = new GUIContent("Enum used on a non-float property: " + prop.name);
                EditorGUI.LabelField(position, c, EditorStyles.helpBox);
                return;
            }

            if (_names.Length == 0)
            {
                EditorGUI.LabelField(position, _errorLabel);
                return;
            }
            MaterialEditor.BeginProperty(position, prop);
            EditorGUI.showMixedValue = prop.hasMixedValue;
            var value = Util.GetInt(prop);
            var selectedIndex = -1;
            for (var index = 0; index < _values.Length; index++)
            {
                var i = _values[index];
                if (i == value)
                {
                    selectedIndex = index;
                    break;
                }
            }

            var selIndex = EditorGUI.Popup(position, label, selectedIndex, _names);
            EditorGUI.showMixedValue = false;
            Util.SetInt(prop, _values[selIndex]);
            MaterialEditor.EndProperty();
        }
    }
}
