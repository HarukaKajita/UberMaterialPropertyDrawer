using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static readonly Dictionary<string, ConstructorInfo> DrawerConstructors = new(StringComparer.Ordinal);
        private static bool _registryInitialized = false;
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

        // (groupName, drawer, string, float, string, float, ...)
        public UberDrawer(string groupName, string drawer,string kw1,float id1) : this(groupName, drawer, new[] { kw1 }, new []{ id1 }) {}
        public UberDrawer(string groupName, string drawer,string kw1,float id1,string kw2,float id2) : this(groupName, drawer, new[] { kw1,kw2 }, new []{ id1,id2 }) {}
        public UberDrawer(string groupName, string drawer,string kw1,float id1,string kw2,float id2,string kw3,float id3) : this(groupName, drawer, new[] { kw1,kw2,kw3 }, new []{ id1,id2,id3 }) {}
        public UberDrawer(string groupName, string drawer,string kw1,float id1,string kw2,float id2,string kw3,float id3,string kw4,float id4) : this(groupName, drawer, new[] { kw1,kw2,kw3,kw4 }, new []{ id1,id2,id3,id4 }) {}
        public UberDrawer(string groupName, string drawer,string kw1,float id1,string kw2,float id2,string kw3,float id3,string kw4,float id4,string kw5,float id5) : this(groupName, drawer, new[] { kw1,kw2,kw3,kw4,kw5 }, new []{ id1,id2,id3,id4,id5 }) {}
        public UberDrawer(string groupName, string drawer,string kw1,float id1,string kw2,float id2,string kw3,float id3,string kw4,float id4,string kw5,float id5,string kw6,float id6) : this(groupName, drawer, new[] { kw1,kw2,kw3,kw4,kw5,kw6 }, new []{ id1,id2,id3,id4,id5,id6 }) {}
        public UberDrawer(string groupName, string drawer,string kw1,float id1,string kw2,float id2,string kw3,float id3,string kw4,float id4,string kw5,float id5,string kw6,float id6,string kw7,float id7) : this(groupName, drawer, new[] { kw1,kw2,kw3,kw4,kw5,kw6,kw7 }, new []{ id1,id2,id3,id4,id5,id6,id7 }) {}
        public UberDrawer(string groupName, string drawer, params string[] args)
        {
            this._groupName = groupName;
            this._drawer = drawer;
            var context = CreateContext(groupName, drawer, args, null, null);
            this._propertyDrawer = CreateDrawer(context);
        }
        
        public UberDrawer(string groupName, string drawer, string[] enumNames, float[] vals)
        {
            this._groupName = groupName;
            this._drawer = drawer;
            var context = CreateContext(groupName, drawer, Array.Empty<string>(), enumNames, vals);
            this._propertyDrawer = CreateDrawer(context);
        }

        private static UberDrawerContext CreateContext(
            string groupName,
            string drawerKey,
            string[] args,
            string[] enumNames,
            float[] enumValues)
        {
            var hasParent = GroupNest.TryPeek(out var parentGroup);
            parentGroup = hasParent ? parentGroup : string.Empty;
            var groupStack = GroupStr();
            return new UberDrawerContext(groupName, drawerKey, args, enumNames, enumValues, parentGroup, groupStack);
        }

        private static MaterialPropertyDrawer CreateDrawer(UberDrawerContext context)
        {
            if (string.IsNullOrEmpty(context.DrawerKey))
                return null;

            if (context.DrawerKey == "ResetGroup")
            {
                GroupExpanded.Clear();
                GroupNest.Clear();
                return null;
            }

            EnsureRegistry();
            if (!DrawerConstructors.TryGetValue(context.DrawerKey, out var constructor))
                return null;

            try
            {
                return (MaterialPropertyDrawer)constructor.Invoke(new object[] { context });
            }
            catch (System.Exception ex)
            {
                UberDrawerLogger.LogError($"Failed to create drawer for key '{context.DrawerKey}': {ex.Message}");
                return null;
            }
        }

        private static void EnsureRegistry()
        {
            if (_registryInitialized)
                return;

            _registryInitialized = true;
            foreach (var type in TypeCache.GetTypesWithAttribute<DrawerKeyAttribute>())
            {
                var attribute = (DrawerKeyAttribute)Attribute.GetCustomAttribute(type, typeof(DrawerKeyAttribute));
                if (attribute == null)
                    continue;

                if (!typeof(MaterialPropertyDrawer).IsAssignableFrom(type))
                {
                    UberDrawerLogger.LogError($"DrawerKeyAttribute is applied to non-drawer type: {type.FullName}");
                    continue;
                }

                var key = attribute.Key;
                if (string.IsNullOrEmpty(key))
                {
                    UberDrawerLogger.LogError($"Drawer key is empty on type: {type.FullName}");
                    continue;
                }

                var constructor = type.GetConstructor(new[] { typeof(UberDrawerContext) });
                if (constructor == null)
                {
                    UberDrawerLogger.LogError($"Drawer type '{type.FullName}' lacks UberDrawerContext constructor");
                    continue;
                }

                if (DrawerConstructors.TryGetValue(key, out var existing))
                {
                    UberDrawerLogger.LogError(
                        $"Drawer key '{key}' is duplicated: {existing.DeclaringType?.FullName} and {type.FullName}");
                    continue;
                }

                DrawerConstructors.Add(key, constructor);
            }
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
        public static bool GetGroupExpanded(string groupName)
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
