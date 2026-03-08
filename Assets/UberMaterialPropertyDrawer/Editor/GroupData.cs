using System.Collections.Generic;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// マテリアルに対応して保持されるデータ
    /// プロパティ描画時のグループの階層状態、グループの開閉状態、描画時の冗長な操作防止用の変数を持つ
    /// </summary>
    public class GroupData
    {
        // キャッシュの所有者と無効か判定用のハッシュ値
        public Material material;   // マテリアル
        public Hash128 depHashAtSet;// キャッシュ生成時の依存ハッシュ
        
        // open state key:groupName, value:bool
        internal Dictionary<string, bool> GroupExpanded { get; } = new();
        
        // 現在の描画スタック。描画サイクル中で今どのグループ内を走査しているかという一時状態。
        internal Stack<string> GroupNest { get; } = new();
        internal int CurrentPassId;
        
        // 同じプロパティに対してGEtPropertyHeightとOnGUIの両方からpush/popされるのを防ぐための記録
        internal HashSet<string> PushedInPass { get; } = new();
        internal HashSet<string> PoppedInPass { get; } = new();
    }
}
