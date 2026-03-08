using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// Provides a caching mechanism for storing and managing <see cref="GroupData"/> objects
    /// associated with <see cref="Material"/> instances.
    /// </summary>
    /// <remarks>
    /// This class is utilized to optimize operations related to material property drawing and
    /// shader dependency management. It ensures that the relevant group data for materials
    /// are readily available within the editor, avoiding repetitive computations.
    /// </remarks>
    public static class GroupDataCache
    {
        static readonly Dictionary<Material, GroupData> s_cache = new();

        /// <summary>
        /// Inspector描画タイミングでキャッシュが無ければ呼ぶ想定
        /// </summary>
        public static void Set(Material mat, GroupData groupData)
        {
            if (!mat || groupData == null) return;

            var path = AssetDatabase.GetAssetPath(mat);
            groupData.material = mat;

            if (!string.IsNullOrEmpty(path))
                groupData.depHashAtSet = AssetDatabase.GetAssetDependencyHash(path); // 依存込みハッシュ
            else
                groupData.depHashAtSet = default; // 非アセット(シーン内生成等)

            s_cache[mat] = groupData;
        }

        public static GroupData GetOrCreate(Material mat)
        {
            if (!mat) return null;
            if (s_cache.TryGetValue(mat, out var groupData)) return groupData;

            groupData = new GroupData();
            Set(mat, groupData);
            return groupData;
        }

        public static bool TryGet(Material mat, out GroupData groupData)
        {
            if (!mat)
            {
                groupData = null;
                return false;
            }
            return s_cache.TryGetValue(mat, out groupData);
        }

        public static void Clear(Material mat)
        {
            if (!mat) return;
            s_cache.Remove(mat);
        }

        public static void ClearAll() => s_cache.Clear();

        /// <summary>
        /// シェーダー系ファイルが更新されたタイミングで呼ぶ。
        /// 依存ハッシュ差分が出たマテリアルをキャッシュから破棄する。
        /// </summary>
        public static void InvalidateByDependencyHash(bool invalidateNonAssetMaterials = true)
        {
            if (s_cache.Count == 0) return;

            // Dictionaryを回しながらRemoveしない（スナップショット取る）
            var keys = new List<Material>(s_cache.Keys);

            foreach (var mat in keys)
            {
                // 偽装null/破棄済み掃除
                if (!mat)
                {
                    s_cache.Remove(mat);
                    continue;
                }

                var data = s_cache[mat];

                // 非アセット（AssetDatabase管理外）
                var materialAssetPath = AssetDatabase.GetAssetPath(data.material);
                if (string.IsNullOrEmpty(materialAssetPath))
                {
                    if (invalidateNonAssetMaterials)
                        s_cache.Remove(mat);
                    continue;
                }

                var newHash = AssetDatabase.GetAssetDependencyHash(materialAssetPath);
                if (newHash != data.depHashAtSet)
                    s_cache.Remove(mat);
            }
        }
    }
}
