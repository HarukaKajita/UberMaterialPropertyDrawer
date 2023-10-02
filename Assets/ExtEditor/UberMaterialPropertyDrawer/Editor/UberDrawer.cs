using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer.Editor
{
    public class UberDrawer : MaterialPropertyDrawer
    {
        private readonly string _groupName = null;
        private readonly string _drawer = null;
        private readonly string _arg0 = null;
        private static readonly Dictionary<string, bool> GroupExpanded = new Dictionary<string, bool>();
        private readonly MaterialPropertyDrawer _propertyDrawer;
        public UberDrawer() { }

        public UberDrawer(string groupName)
        {
            this._groupName = groupName;
            this._propertyDrawer = null;
        }

        public UberDrawer(string groupName, string drawer)
        {
            this._groupName = groupName;
            this._drawer = drawer;
            this._propertyDrawer = null;
            if (drawer == "BeginToggleGroup")
                this._propertyDrawer = new BeginToggleGroupDrawer(groupName, this);
            else if (drawer == "EndToggleGroup")
                this._propertyDrawer = new EndToggleGroupDrawer(groupName, this);
            else if (drawer == "BeginGroup")
                this._propertyDrawer = new BeginGroupDrawer(groupName, this);
        }

        public UberDrawer(string groupName, string drawer, string arg0)
        {
            this._groupName = groupName;
            this._drawer = drawer;
            this._arg0 = arg0;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            // var debugStr = MakeDebugArgString(prop.name);
            if(_propertyDrawer != null)
                _propertyDrawer.OnGUI(position, prop,label,editor);
            else
            {
                // editor.TexturePropertySingleLine(label, prop);
                if (GroupExpanded.TryGetValue(_groupName, out var expanded))
                {
                    if (expanded)
                        editor.DefaultShaderProperty(prop, ObjectNames.NicifyVariableName(prop.name));
                }
                else
                {
                    //対応するグループが見つからない場合
                    editor.DefaultShaderProperty(prop, ObjectNames.NicifyVariableName(prop.name));
                }   
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (_propertyDrawer != null) return _propertyDrawer.GetPropertyHeight(prop, label, editor);
            
            const int baseValue = 0; //base.GetPropertyHeight(prop, label, editor);
            if (GroupExpanded.TryGetValue(_groupName, out var expanded))
                return expanded ? baseValue : 0;
            
            return baseValue;//この場合別の処理をしても良いかも
        }

        string MakeDebugArgString(string propName)
        {
            var debugStr = propName ?? "EMPTY";
            debugStr += " : ";
            debugStr += _groupName ?? "EMPTY";
            debugStr += ",";
            debugStr += _drawer ?? "EMPTY";
            debugStr += ",";
            debugStr += _arg0 ?? "EMPTY";
            return debugStr;
        }
        
        internal static bool GetGroupExpanded(string groupName)
        {
            if (GroupExpanded.TryGetValue(groupName, out var expanded))
            {
                Debug.Log("GetGroupExpanded : Existed " + groupName + " : " + expanded);
                return expanded;
            }
            else
            {
                var defaultValue = false;
                GroupExpanded.Add(groupName, defaultValue);
                Debug.Log("GetGroupExpanded : NOT Existed " + groupName + " : " + defaultValue);
                return defaultValue;
            }
        }

        internal static void SetGroupExpanded(string groupName, bool state)
        {
            GroupExpanded[groupName] = state;
        }
    }
}