using System;
using System.Linq;
using UnityEditor;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// グループ状態の直接的な操作を管理するクラス
    /// </summary>
    public static class GroupStateManager
    {
        /// <summary>
        /// インスペクタの描画開始。
        /// 指定されたグループデータに対して新しいパスを開始する。
        /// 現在のパスの状態を初期化し、以前にプッシュおよびポップされたレコードセットをクリアします。
        /// </summary>
        internal static void BeginPass(MaterialEditor editor)
        {
            if (editor == null) return;
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            renderState.PathStack.Clear();
            renderState.PushedProperties.Clear();
            renderState.PoppedProperties.Clear();
        }

        /// <summary>
        /// 同じ property 名で1パス中に二重 push/pop しないための記録。
        /// まだrecordされていなければrecordしてtrueを返す。
        /// 既にrecordされていればfalseを返す。
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="propNameKey">プロパティ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        /// <returns>記録に成功したかどうか</returns>
        internal static bool TryRecordPush(MaterialEditor editor, string propNameKey)
        {
            UberDrawerLogger.Log("TryRecordPush : " + propNameKey);
            if (editor == null || string.IsNullOrEmpty(propNameKey)) return false;
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            return renderState.PushedProperties.Add(propNameKey);
        }

        /// <summary>
        /// 同じ property 名で1パス中に二重 push/pop しないための記録。
        /// まだrecordされていなければrecordしてtrueを返す。
        /// 既にrecordされていればfalseを返す。
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="propNameKey">プロパティ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        /// <returns>記録に成功したかどうか</returns>
        internal static bool TryRecordPop(MaterialEditor editor, string propNameKey)
        {
            UberDrawerLogger.Log("TryRecordPop : " + propNameKey);
            if (editor == null || string.IsNullOrEmpty(propNameKey)) return false;
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            return renderState.PoppedProperties.Add(propNameKey);
        }

        /// <summary>
        /// 開閉状態の読み出し
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullなら、開状態として返却</param>
        /// <param name="groupPath">グループパス。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        /// <returns>開閉状態</returns>
        public static bool GetExpanded(ShaderGroupStateData data, string groupPath)
        {
            if (string.IsNullOrEmpty(groupPath)) return true;
            if (data == null) return true;
            if (data.ExpansionState.ExpandedByPath.TryGetValue(groupPath, out var expanded))
            {
                UberDrawerLogger.Log("GetExpanded : Existed " + groupPath + " : " + (expanded ? "開いている" : "閉じている"));
                return expanded;
            }

            const bool defaultValue = false;
            data.ExpansionState.ExpandedByPath.Add(groupPath, defaultValue);
            UberDrawerLogger.Log("GetExpanded : NOT Existed " + groupPath + " : " + (defaultValue　?　"開いている"　:　"閉じている"));
            return defaultValue;
        }

        /// <summary>
        /// 開閉状態の書き出し。親グループに依存しない開閉状態。
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullの場合、メソッドは実行せずに終了する。</param>
        /// <param name="groupPath">グループ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        /// <param name="state">開閉状態</param>
        internal static void SetExpanded(ShaderGroupStateData data, string groupPath, bool state)
        {
            if (string.IsNullOrEmpty(groupPath) || data == null) return;
            data.ExpansionState.ExpandedByPath[groupPath] = state;
            InspectorRepainter.RepaintAllInspector();
        }

        /// <summary>
        /// 描画中のプロパティのグループスタックの更新。
        /// グループ開始。
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="groupPath">グループ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        internal static void PushPath(MaterialEditor editor, string groupPath)
        {
            if (string.IsNullOrEmpty(groupPath) || editor== null) return;
            UberDrawerLogger.Log("Push : " + groupPath);
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            renderState.PathStack.Push(groupPath);
        }

        /// <summary>
        /// 描画中のプロパティのグループスタックの更新。
        /// グループの終了。
        /// </summary>
        /// <param name="editor"></param>
        /// <returns>グループ名。空文字列の場合、スタックが空であることを示す。</returns>
        internal static string PopPath(MaterialEditor editor)
        {
            if (editor == null) return string.Empty;
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            if (editor == null || renderState.PathStack.Count == 0)
            {
                UberDrawerLogger.LogWarning("Pop called on empty group stack.");
                return string.Empty;
            }

            var poppedPath = renderState.PathStack.Pop();
            UberDrawerLogger.Log("Pop  : " + poppedPath);
            return poppedPath;
        }

        internal static bool TryPeekGroup(MaterialEditor editor, out string groupPath)
        {
            if (editor == null)
            {
                groupPath = null;
                return false;
            }

            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            if (renderState.PathStack.Count == 0)
            {
                groupPath = string.Empty;
                return false;
            }

            groupPath = renderState.PathStack.Peek();
            return true;
        }

        internal static void ResetAll(ShaderGroupStateData data, MaterialEditor editor)
        {
            if (data != null) 
            {
                data.ExpansionState.ExpandedByPath.Clear();
            }

            if (editor != null)
            {
                var renderState = GroupTraversalStateCache.GetOrCreate(editor);
                renderState.PathStack.Clear();
                renderState.PushedProperties.Clear();
                renderState.PoppedProperties.Clear();
            }
        }
        
        public static string BuildPath(string parentPath, string groupName)
        {
            // A/B/Cの形にする
            // parentPathがnullや空文字列ならgroupNameをそのまま返差ないと/A/B/Cの形になり得る。
            if(string.IsNullOrEmpty(parentPath)) return groupName;
            return parentPath + "/" + groupName;
        }

        public static string GetParentPath(MaterialEditor editor)
        {
            if (editor == null) return string.Empty;
            
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            if (renderState.PathStack.Count <= 1) return string.Empty;
            return renderState.PathStack.ToArray()[1];
        }
        public static string GetCurrentPath(MaterialEditor editor)
        {
            if (editor == null) return string.Empty;
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            if (renderState.PathStack.Count <= 0) return string.Empty;
            return renderState.PathStack.Peek();
        }

        /// <summary>
        /// 今いるscope自体が全部開いているか
        /// </summary>
        /// <param name="data"></param>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static bool IsCurrentScopeVisible(ShaderGroupStateData data, MaterialEditor editor)
        {
            if (data == null) return false;
            if (editor == null) return false;
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            // あってる？
            foreach (var groupPath in renderState.PathStack)
            {
                if (!GetExpanded(data, groupPath))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 今からbeginするグループの親chainが全部開いているか
        /// </summary>
        /// <param name="data"></param>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static bool IsParentScopeVisible(ShaderGroupStateData data, MaterialEditor editor)
        {
            // あってる？
            if (data == null) return false;
            if (editor == null) return false;
            var renderState = GroupTraversalStateCache.GetOrCreate(editor);
            var groupPathArray = renderState.PathStack.Reverse().ToArray();
            for (var i = 0; i < groupPathArray.Length-1; i++)
            {
                if (!GetExpanded(data, groupPathArray[i])) return false;
            }
            return true;
        }
    }
}

