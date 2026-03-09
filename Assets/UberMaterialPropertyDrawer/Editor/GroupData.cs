using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// Shaderに対応して保持されるデータ。
    /// プロパティ描画時のグループの開閉状態。
    /// </summary>
    public class GroupData
    {
        // キャッシュの所有者と無効か判定用のハッシュ値
        public Shader Shader; 
        public Hash128 DepHashAtSet;// キャッシュ生成時の依存ハッシュ
        
        // 開閉状態
        internal readonly GroupExpansionState ExpansionState = new();
    }
}
