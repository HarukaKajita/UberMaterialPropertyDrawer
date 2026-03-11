using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// shader毎にグループの開閉状態をキャッシュするクラス。
    /// </summary>
    public static class GroupDataCache
    {
        private static readonly Dictionary<Shader, ShaderGroupStateData> Cache = new();

        /// <summary>
        /// Inspector描画タイミングでキャッシュが無ければ呼ぶ想定
        /// </summary>
        private static void Set(Shader shader, ShaderGroupStateData groupData)
        {
            if (!shader || groupData == null) return;

            var path = AssetDatabase.GetAssetPath(shader);
            groupData.OwnerShader = shader;

            if (!string.IsNullOrEmpty(path))
                groupData.DependencyHashAtCacheTime = AssetDatabase.GetAssetDependencyHash(path); // 依存込みハッシュ
            else
                groupData.DependencyHashAtCacheTime = default; // 非アセット(シーン内生成等)

            Cache[shader] = groupData;
        }

        public static ShaderGroupStateData GetOrCreate(Shader shader)
        {
            if (!shader) return null;
            if (Cache.TryGetValue(shader, out var groupData)) return groupData;

            groupData = new ShaderGroupStateData();
            Set(shader, groupData);
            return groupData;
        }

        public static bool TryGet(Shader shader, out ShaderGroupStateData groupData)
        {
            if (!shader)
            {
                groupData = null;
                return false;
            }
            return Cache.TryGetValue(shader, out groupData);
        }

        public static void Clear(Shader shader)
        {
            if (!shader) return;
            Cache.Remove(shader);
        }

        public static void ClearAll() => Cache.Clear();

        /// <summary>
        /// シェーダー系ファイルが更新されたタイミングで呼ぶ。
        /// 依存ハッシュ差分が出たシェーダーをキャッシュから破棄する。
        /// </summary>
        public static void InvalidateByDependencyHash(bool invalidateNonAssetMaterials = true)
        {
            if (Cache.Count == 0) return;

            // Dictionaryを回しながらRemoveしない（スナップショット取る）
            var keys = new List<Shader>(Cache.Keys);

            foreach (var shader in keys)
            {
                // 偽装null/破棄済み掃除
                if (!shader)
                {
                    Cache.Remove(shader);
                    continue;
                }

                var data = Cache[shader];

                // 非アセット（AssetDatabase管理外）
                var materialAssetPath = AssetDatabase.GetAssetPath(data.OwnerShader);
                if (string.IsNullOrEmpty(materialAssetPath))
                {
                    if (invalidateNonAssetMaterials)
                        Cache.Remove(shader);
                    continue;
                }

                var newHash = AssetDatabase.GetAssetDependencyHash(materialAssetPath);
                if (newHash != data.DependencyHashAtCacheTime)
                    Cache.Remove(shader);
            }
        }
    }
}


