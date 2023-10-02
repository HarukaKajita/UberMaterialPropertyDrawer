using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer.Editor
{
    public class UberDrawer : MaterialPropertyDrawer
    {
        private string groupName = null;
        private string drawer = null;
        private string arg0 = null;
        private static Dictionary<string, bool> groupExpanded = new Dictionary<string, bool>();
        private static bool state = false;
        public UberDrawer() { }

        public UberDrawer(string groupName)
        {
            this.groupName = groupName;
        }

        public UberDrawer(string groupName, string drawer)
        {
            this.groupName = groupName;
            this.drawer = drawer;
        }

        public UberDrawer(string groupName, string drawer, string arg0)
        {
            this.groupName = groupName;
            this.drawer = drawer;
            this.arg0 = arg0;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            var debugStr = MakeDebugArgString(prop.name);
            // Debug.Log(debugStr);
            if (drawer == "BeginToggleGroup")
            {
                groupExpanded[groupName] = BeginPanel(position, prop, GetGroupExpanded(groupName));
            }
            else if (drawer == "EndToggleGroup")
            {
                EndPanel();
            }
            else
            {
                // editor.TexturePropertySingleLine(label, prop);
                // Debug.Log(debugStr);
                // EditorGUI.LabelField(position, debugStr, EditorStyles.label);
                if (groupExpanded.TryGetValue(groupName, out var expanded))
                {
                    if (expanded)
                    {
                        // EditorGUI.indentLevel++;
                        // EditorGUI.LabelField(position, debugStr, EditorStyles.label);
                        editor.DefaultShaderProperty(prop, ObjectNames.NicifyVariableName(prop.name));
                        // EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    //対応するグループが見つからない場合
                    // EditorGUI.LabelField(position, debugStr, EditorStyles.label);
                    editor.DefaultShaderProperty(prop, ObjectNames.NicifyVariableName(prop.name));
                }   
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            var baseValue = 0;//base.GetPropertyHeight(prop, label, editor);
            if (drawer == "EndToggleGroup") return 0;
            if (drawer == "BeginToggleGroup") return 22 + 4;
            if (groupExpanded.TryGetValue(groupName, out var expanded))
            {
                return expanded ? baseValue : 0;
            }
            return baseValue;//この場合別の処理をしても良いかも
        }

        string MakeDebugArgString(string propName)
        {
            var debugStr = propName ?? "EMPTY";
            debugStr += " : ";
            debugStr += groupName ?? "EMPTY";
            debugStr += ",";
            debugStr += drawer ?? "EMPTY";
            debugStr += ",";
            debugStr += arg0 ?? "EMPTY";
            return debugStr;
        }

        private bool BeginPanel( Rect position, MaterialProperty prop, bool expanded)
        {
            EditorGUI.indentLevel = 0;
            
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
            // if (e.type == EventType.Repaint) {
            //     // EditorStyles.foldout.Draw(toggleRect, false, false, expanded, false);
            // }
            if (e.type == EventType.MouseDown && interactiveRect.Contains(e.mousePosition)) {
                expanded = !expanded;
                e.Use();
            }
            
            prop.floatValue = EditorGUI.Toggle(new Rect(position.x+2, position.y+1, 18, 18), prop.floatValue == 1.0) ? 1 : 0;
            EditorGUI.LabelField(new Rect(position.x + 20f, position.y, 300, 18), this.groupName, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            return expanded;
        }
        public void EndPanel()
        {
            EditorGUI.indentLevel -= 1;
        }


        private static bool GetGroupExpanded(string groupName)
        {
            if (groupExpanded.TryGetValue(groupName, out var expanded))
            {
                Debug.Log("GetGroupExpanded : Existed " + groupName + " : " + expanded);
                return expanded;
            }
            else
            {
                var defaultValue = true;
                groupExpanded.Add(groupName, defaultValue);
                Debug.Log("GetGroupExpanded : NOT Existed " + groupName + " : " + defaultValue);
                return defaultValue;
            }
        }

    }
}