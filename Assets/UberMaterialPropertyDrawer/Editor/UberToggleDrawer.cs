using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class UberToggleDrawer : UberDrawerBase
    {
        private int _dimention = 1;
        public UberToggleDrawer() : base()
        {
        }

        public UberToggleDrawer(string groupName) : base(groupName)
        {
        }
        
        public UberToggleDrawer(string groupName, float dim) : base(groupName)
        {
            _dimention = (int)Math.Clamp(dim,1,4);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.SingleLineHeight, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;

            var propName = label.text;
            if (_dimention == 1)
            {
                MaterialEditor.BeginProperty(position, prop);
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = prop.hasMixedValue;

                
                var toggleValue = Util.GetAsBool(prop);
                toggleValue = EditorGUI.Toggle(position, propName, toggleValue);    
            
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    editor.RegisterPropertyChangeUndo(propName);
                    Util.SetBool(prop, toggleValue);
                }
                MaterialEditor.EndProperty();
            }
            else
            {
                var tmpWidth = EditorGUIUtility.labelWidth;
                var labelWidth = position.width;
                var toggleLabelWidth = 15;
                var toggleWidth = 20;
                var spaceWidth = 5;
                var fieldWidth = (toggleLabelWidth + toggleWidth + spaceWidth) * 4;
                var leftMargin = 15f;
                labelWidth -= fieldWidth + leftMargin;

                
                EditorGUIUtility.labelWidth = labelWidth;
                var labelRect = new Rect(position.x, position.y, labelWidth , position.height);
                EditorGUI.PrefixLabel(labelRect, label);
                
                var toggleRect = new Rect(
                    position.x + labelWidth,
                    position.y,
                    toggleLabelWidth+toggleWidth+spaceWidth,
                    position.height);
                
                var materials = prop.targets.Cast<Material>().ToArray();
                
                var componentLabel = new[] { "X", "Y", "Z", "W" };
                for (var dim = 0; dim < _dimention; dim++)
                {
                    EditorGUI.BeginChangeCheck();
                    var vectors = materials.Select(m=>m.GetVector(propName)).ToArray();
                    var floats = vectors.Select(v => v[dim]).ToArray();
                    var bools = floats.Select(f => f != 0).ToArray();
                    var headElementBool = bools[0];
                    var hasMixedValue = bools.Any(b => b != headElementBool);
                    EditorGUI.showMixedValue = hasMixedValue;

                    var toggleValue = headElementBool;
                    EditorGUI.LabelField(toggleRect,componentLabel[dim]);
                    toggleRect.x += toggleLabelWidth;
                    toggleValue = EditorGUI.Toggle(toggleRect,"", headElementBool);
                    toggleRect.x += toggleWidth;
                    toggleRect.x += spaceWidth;
                    
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        editor.RegisterPropertyChangeUndo(propName);
                        foreach (var mat in materials)
                        {
                            var v = mat.GetVector(propName);
                            v[dim] = toggleValue ? 1f : 0f;
                            mat.SetVector(propName, v);
                        }
                    }
                    
                    EditorGUIUtility.labelWidth = tmpWidth;
                }
            }
        }
    }
}
