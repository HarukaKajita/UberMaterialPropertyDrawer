using System;
using System.Collections.Generic;
using UnityEditor;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// シェーダーソースファイルが更新されたタイミングで、マテリアルプロパティのグループの階層情報や開閉状態のキャッシュを破棄するプロセス。
    /// シェーダーの更新によってグループ階層が変化する可能性があるため、既存のキャッシュと不整合を起こさないようにキャッシュを破棄する。
    /// </summary>
    public class ShaderSourceChangeWatcher : AssetPostprocessor
    {
        static readonly HashSet<string> k_WatchExts = new(StringComparer.OrdinalIgnoreCase)
        {
            ".shader",
            ".shadergraph",
            ".hlsl",
            ".cginc"
            
            // ".compute",
            // ".glslinc"
        };

        static bool s_scheduled;

        static bool IsWatched(string path)
            => k_WatchExts.Contains(System.IO.Path.GetExtension(path));

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload)
        {
            bool touched = false;

            foreach (var p in importedAssets) { if (IsWatched(p)) { touched = true; break; } }
            if (!touched) foreach (var p in deletedAssets) { if (IsWatched(p)) { touched = true; break; } }
            if (!touched) foreach (var p in movedAssets)   { if (IsWatched(p)) { touched = true; break; } }

            if (!touched) return;

            // 連打されるので、1フレーム後にまとめて1回だけ
            if (s_scheduled) return;
            s_scheduled = true;

            EditorApplication.delayCall += () =>
            {
                s_scheduled = false;
                GroupDataCache.InvalidateByDependencyHash(invalidateNonAssetMaterials: true);
            };
        }
    }
}
