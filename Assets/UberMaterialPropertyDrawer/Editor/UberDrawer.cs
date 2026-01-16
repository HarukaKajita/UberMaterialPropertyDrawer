using System;
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

        public UberDrawer(string groupName) : this(groupName, null, Array.Empty<string>()) {}
        public UberDrawer(string groupName, string drawer) : this(groupName, drawer, Array.Empty<string>()) {}
        public UberDrawer(string groupName, string drawer, string kw1) : this(groupName, drawer, new[] { kw1 }) {}
        public UberDrawer(string groupName, string drawer, string kw1, string kw2) : this(groupName, drawer, new[] { kw1, kw2 }) {}
        public UberDrawer(string groupName, string drawer, string kw1, string kw2, string kw3) : this(groupName, drawer, new[] { kw1, kw2, kw3 }) {}
        public UberDrawer(string groupName, string drawer, string kw1, string kw2, string kw3, string kw4) : this(groupName, drawer, new[] { kw1, kw2, kw3, kw4 }) {}
        public UberDrawer(string groupName, string drawer, string kw1, string kw2, string kw3, string kw4, string kw5) : this(groupName, drawer, new[] { kw1, kw2, kw3, kw4, kw5 }) {}
        public UberDrawer(string groupName, string drawer, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6) : this(groupName, drawer, new[] { kw1, kw2, kw3, kw4, kw5, kw6 }) {}
        public UberDrawer(string groupName, string drawer, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7) : this(groupName, drawer, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7 }) {}
        public UberDrawer(string groupName, string drawer, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8) : this(groupName, drawer, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7, kw8 }) {}
        public UberDrawer(string groupName, string drawer, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8, string kw9) : this(groupName, drawer, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7, kw8, kw9 }) {}

        public UberDrawer(string groupName, string drawer, params string[] args)
        {
            this._groupName = groupName;
            this._drawer = drawer;
            this._propertyDrawer = null;
            var existParent = GroupNest.TryPeek(out var parentGroup);
            parentGroup = existParent ? parentGroup : "";
            if (drawer == "BeginToggleGroup") this._propertyDrawer = new BeginToggleGroupDrawer(groupName, parentGroup);
            else if (drawer == "EndGroup")    this._propertyDrawer = new EndGroupDrawer(groupName, GroupStr());
            else if (drawer == "BeginGroup")  this._propertyDrawer = new BeginGroupDrawer(groupName, parentGroup);
            else if (drawer == "Vector2")     this._propertyDrawer = new Vector2Drawer(groupName);
            else if (drawer == "Vector3")     this._propertyDrawer = new Vector3Drawer(groupName);
            else if (drawer == "ToggleUI")    this._propertyDrawer = new UberToggleDrawer(groupName);
            else if (drawer == "CurveTexture") this._propertyDrawer = new CurveTextureDrawer(groupName, args);
            else if (drawer == "GradientTexture") this._propertyDrawer = new GradientTextureDrawer(groupName, args);
            else if (drawer == "Enum")
            {
                // 要素数が1ならEnumのクラス名指定
                if(args.Length == 1)
                    this._propertyDrawer = new UberEnumDrawer(groupName, args[0]);
                else //要素数が2以上（2の倍数）なら名前と値の組を直接記述している
                    this._propertyDrawer = new UberEnumDrawer(groupName, args);
            }
            else if (drawer == "ResetGroup")        
            {
                GroupExpanded.Clear();
                GroupNest.Clear();
            }
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