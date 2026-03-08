using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public static class UberDrawerLogger
    {
        static UberDrawerLogger()
        {
            _debugLevel = DebugLevel.Warning;
        }
        // プロパティの描画プロセスをDebug.Logで出力してデバッグする時用の制御
        public static DebugLevel _debugLevel;

        public enum DebugLevel
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
}
