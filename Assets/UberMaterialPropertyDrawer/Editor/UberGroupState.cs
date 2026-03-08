using System;
using System.Linq;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// グループ状態の直接的な操作を管理するクラス
    /// </summary>
    public static class UberGroupState
    {
        /// <summary>
        /// インスペクタの描画開始。
        /// 指定されたグループデータに対して新しいパスを開始する。
        /// 現在のパスの状態を初期化し、以前にプッシュおよびポップされたレコードセットをクリアします。
        /// </summary>
        /// <param name="data">現在のパス状態とグループ固有情報を保持するグループデータオブジェクト。nullの場合、メソッドは実行せずに終了する。</param>
        internal static void BeginPass(GroupData data)
        {
            if (data == null) return;
            data.CurrentPassId++;
            data.PushedInPass.Clear();
            data.PoppedInPass.Clear();
        }

        /// <summary>
        /// 同じ property 名で1パス中に二重 push/pop しないための記録。
        /// まだrecordされていなければrecordしてtrueを返す。
        /// 既にrecordされていればfalseを返す。
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullの場合、メソッドは実行せずに終了する。</param>
        /// <param name="propNameKey">プロパティ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        /// <returns>記録に成功したかどうか</returns>
        internal static bool TryRecordPush(GroupData data, string propNameKey)
        {
            UberDrawerLogger.Log("TryRecordPush : " + propNameKey);
            if (data == null || string.IsNullOrEmpty(propNameKey)) return false;
            return data.PushedInPass.Add(propNameKey);
        }

        /// <summary>
        /// 同じ property 名で1パス中に二重 push/pop しないための記録。
        /// まだrecordされていなければrecordしてtrueを返す。
        /// 既にrecordされていればfalseを返す。
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullの場合、メソッドは実行せずに終了する。</param>
        /// <param name="propNameKey">プロパティ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        /// <returns>記録に成功したかどうか</returns>
        internal static bool TryRecordPop(GroupData data, string propNameKey)
        {
            UberDrawerLogger.Log("TryRecordPop : " + propNameKey);
            if (data == null || string.IsNullOrEmpty(propNameKey)) return false;
            return data.PoppedInPass.Add(propNameKey);
        }

        /// <summary>
        /// 開閉状態の読み出し
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullなら、開状態として返却</param>
        /// <param name="groupName">グループ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        /// <returns>開閉状態</returns>
        public static bool GetGroupExpanded(GroupData data, string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return true;
            if (data == null) return true;
            if (data.GroupExpanded.TryGetValue(groupName, out var expanded))
            {
                UberDrawerLogger.Log("GetGroupExpanded : Existed " + groupName + " : " + (expanded ? "開いている" : "閉じている"));
                return expanded;
            }

            var defaultValue = false;
            data.GroupExpanded.Add(groupName, defaultValue);
            UberDrawerLogger.Log("GetGroupExpanded : NOT Existed " + groupName + " : " + (defaultValue　?　"開いている"　:　"閉じている"));
            return defaultValue;
        }

        /// <summary>
        /// 開閉状態の書き出し
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullの場合、メソッドは実行せずに終了する。</param>
        /// <param name="groupName">グループ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        /// <param name="state">開閉状態</param>
        internal static void SetGroupExpanded(GroupData data, string groupName, bool state)
        {
            if (string.IsNullOrEmpty(groupName) || data == null) return;
            data.GroupExpanded[groupName] = state;
        }

        /// <summary>
        /// 描画中のプロパティのグループスタックの更新。
        /// グループ開始。
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullの場合、メソッドは実行せずに終了する。</param>
        /// <param name="groupName">グループ名。nullまたは空文字列の場合、メソッドは実行せずに終了する。</param>
        internal static void PushGroup(GroupData data, string groupName)
        {
            if (string.IsNullOrEmpty(groupName) || data == null) return;
            UberDrawerLogger.Log("Push : " + groupName);
            data.GroupNest.Push(groupName);
        }

        /// <summary>
        /// 描画中のプロパティのグループスタックの更新。
        /// グループの終了。
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullの場合、メソッドは実行せずに終了する。</param>
        /// <returns>グループ名。空文字列の場合、スタックが空であることを示す。</returns>
        internal static string PopGroup(GroupData data)
        {
            if (data == null || data.GroupNest.Count == 0)
            {
                UberDrawerLogger.LogWarning("Pop called on empty group stack.");
                return string.Empty;
            }

            var popGroup = data.GroupNest.Pop();
            UberDrawerLogger.Log("Pop  : " + popGroup);
            return popGroup;
        }

        internal static bool TryPeekGroup(GroupData data, out string groupName)
        {
            if (data == null || data.GroupNest.Count == 0)
            {
                groupName = string.Empty;
                return false;
            }

            groupName = data.GroupNest.Peek();
            return true;
        }

        internal static int GetGroupIndentLevel(GroupData data)
        {
            return data == null ? 0 : Math.Max(0,data.GroupNest.Count-1);
        }

        internal static void ResetAll(GroupData data)
        {
            if (data == null) return;
            data.GroupExpanded.Clear();
            data.GroupNest.Clear();
            data.PushedInPass.Clear();
            data.PoppedInPass.Clear();
            data.CurrentPassId = 0;
        }
        
        internal static void ResetNest(GroupData data)
        {
            if (data == null) return;
            data.GroupNest.Clear();
        }
        
        /// <summary>
        /// 親グループのどれかが閉じられているかどうかを確認する。
        /// </summary>
        /// <param name="data">グループデータオブジェクト。nullの場合、メソッドは実行せずに終了する。</param>
        /// <param name="indentNum">確認する親グループの深さ。0は現在のグループを指す。</param>
        /// <returns>親グループのどれかが閉じられている場合はtrue、それ以外はfalse。</returns>
        internal static bool ParentGroupIsClosed(GroupData data, int indentNum)
        {
            if (data == null) return false;
            UberDrawerLogger.Log("indentNum : " + indentNum);
            UberDrawerLogger.Log("GroupNest : " + data.GroupNest.Count);
            var groupArray = data.GroupNest.Reverse().ToArray();
            if (data.GroupNest.Count < indentNum) return false;
            UberDrawerLogger.Log("Parents : " + string.Join(", ", groupArray));
            for (var i = 0; i < indentNum; i++)
            {
                var parentalGroup = groupArray[i];
                UberDrawerLogger.Log("Parent " + parentalGroup + " -> " + (GetGroupExpanded(data, parentalGroup) ? "opened" : "closed"));
                if (!GetGroupExpanded(data, parentalGroup)) return true;
            }
            return false;
        }
    }
}
