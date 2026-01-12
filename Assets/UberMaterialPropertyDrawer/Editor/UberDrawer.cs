using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal static class UberDrawerLogger
    {
        static UberDrawerLogger()
        {
            _debugLevel = DebugLevel.Warning;
        }
        // プロパティの描画プロセスをDebug.Logで出力してデバッグする時用の制御
        private static readonly DebugLevel _debugLevel;
        enum DebugLevel
        {
            None,
            Error,
            Warning,
            Log
        }
        public static void Log(string message)
        {
            if(_debugLevel >= DebugLevel.Log)
                Debug.Log(message);
        }
        public static void LogWarning(string message)
        {
            if (_debugLevel >= DebugLevel.Warning)
                Debug.LogWarning(message);
        }
        public static void LogError(string message)
        {
            if (_debugLevel >= DebugLevel.Error)
                Debug.LogError(message);
        }
    }
    public class UberDrawer : MaterialPropertyDrawer
    {
        private readonly string _groupName = null;
        private readonly string _drawer = null;
        private readonly string _arg0 = null;
        private static readonly Dictionary<string, bool> GroupExpanded = new();
        private static readonly Stack<string> GroupNest = new();
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
            var success = GroupNest.TryPeek(out var parentGroup);
            parentGroup = success ? parentGroup : "";
            if (drawer == "BeginToggleGroup") this._propertyDrawer = new BeginToggleGroupDrawer(groupName, parentGroup);
            else if (drawer == "EndGroup")    this._propertyDrawer = new EndGroupDrawer(groupName, GroupStr());
            else if (drawer == "BeginGroup")  this._propertyDrawer = new BeginGroupDrawer(groupName, parentGroup);
            else if (drawer == "Vector2")     this._propertyDrawer = new Vector2Drawer(groupName);
            else if (drawer == "Vector3")     this._propertyDrawer = new Vector3Drawer(groupName);
            else if (drawer == "CurveTexture") this._propertyDrawer = new CurveTextureDrawer();
            else if (drawer == "GradientTexture") this._propertyDrawer = new GradientTextureDrawer();
            else if (drawer == "ResetGroup")        
            {
                GroupExpanded.Clear();
                GroupNest.Clear();
            }
        }

        public UberDrawer(string groupName, string drawer, string arg0)
        {
            this._groupName = groupName;
            this._drawer = drawer;
            this._arg0 = arg0;
            this._propertyDrawer = null;
            if (drawer == "Enum") this._propertyDrawer = new UberEnumDrawer(groupName, arg0);
            else if (drawer == "CurveTexture") this._propertyDrawer = new CurveTextureDrawer(arg0);
            else if (drawer == "GradientTexture") this._propertyDrawer = new GradientTextureDrawer(arg0);
        }
        
        public UberDrawer(string groupName, string drawer, string[] enumNames, float[] vals)
        {
            this._groupName = groupName;
            this._drawer = drawer;
            if (drawer == "Enum") this._propertyDrawer = new UberEnumDrawer(groupName, enumNames, vals);
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
                return expanded ? baseValue : -2;
            
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
        
        // グループが開かれた描画状態になっているかどうかのboolを返す
        internal static bool GetGroupExpanded(string groupName)
        {
            if (GroupExpanded.TryGetValue(groupName, out var expanded))
            {
                UberDrawerLogger.Log("GetGroupExpanded : Existed " + groupName + " : " + expanded);
                return expanded;
            }
            else
            {
                var defaultValue = false;
                GroupExpanded.Add(groupName, defaultValue);
                UberDrawerLogger.Log("GetGroupExpanded : NOT Existed " + groupName + " : " + defaultValue);
                return defaultValue;
            }
        }
        // グループの開閉状態を設定する
        internal static void SetGroupExpanded(string groupName, bool state)
        {
            GroupExpanded[groupName] = state;
        }

        public static void PushGroup(string groupName)
        {
            UberDrawerLogger.Log("Push : " + groupName);
            GroupNest.Push(groupName);
        }
        internal static string PopGroup()
        {
            var popGroup = GroupNest.Pop();
            UberDrawerLogger.Log("Pop  : " + popGroup);
            return popGroup;
        }

        internal static int GetGroupIntentLevel()
        {
            return GroupNest.Count;
        }

        private static string GroupStr()
        {
            return string.Join(", ", GroupNest);
        }
        internal static bool ParentGroupIsFolded(int indentNum)
        {
            UberDrawerLogger.Log("indentNum : " + indentNum);
            UberDrawerLogger.Log("GroupNest : " + GroupNest.Count);
            var groupArray = GroupNest.Reverse().ToArray();
            if (GroupNest.Count < indentNum) return false;
            UberDrawerLogger.Log("Parents : " + string.Join(", ", groupArray));
            for (var i = 0; i < indentNum; i++)
            {
                var parentalGroup = groupArray[i];
                UberDrawerLogger.Log("Parent " + parentalGroup + " -> " + (GetGroupExpanded(parentalGroup) ? "expanded" : "folded"));
                if (!GetGroupExpanded(parentalGroup)) return true;
            }
            return false;
        }
    }
}