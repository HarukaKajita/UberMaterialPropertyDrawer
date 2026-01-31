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
        private readonly string _groupName = string.Empty;

        public UberDrawer(string groupName)
        {
            _groupName = groupName ?? string.Empty;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!UberGroupState.GetGroupExpanded(_groupName))
                return;

            // 無限ループしてエディタがクラッシュするのでDefaultShaderPropertyを使用する
            // editor.ShaderProperty(position, prop, ObjectNames.NicifyVariableName(prop.name));
            editor.DefaultShaderProperty(position, prop, prop.displayName);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!UberGroupState.GetGroupExpanded(_groupName))
                return -2;
            return MaterialEditor.GetDefaultPropertyHeight(prop);
        }
    }
}
