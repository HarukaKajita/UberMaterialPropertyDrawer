using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FBXScaleAutomationExtension
{
    [CustomEditor(typeof(AutoScaleExclusionSetting))]
    public class AutoScaleExclusionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var scriptableObject = (AutoScaleExclusionSetting)target;
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("exclusiveModelsPath"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("removed"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("additional"), true);
            EditorGUI.EndDisabledGroup();
            if(GUILayout.Button("Reimport diff", GUILayout.Height(40)))
            {
                scriptableObject.Reimport();
            }
            
            if(GUILayout.Button("Revert diff", GUILayout.Height(40)))
            {
                scriptableObject.Revert();
            }
         
        }
    }
}
