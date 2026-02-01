using System.Collections.Generic;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class GroupData
    {
        public Material material;   // マテリアル
        public Hash128 depHashAtSet;// キャッシュ生成時の依存ハッシュ
        internal Dictionary<string, bool> GroupExpanded { get; } = new();
        internal Stack<string> GroupNest { get; } = new();
        internal int CurrentPassId;
        internal HashSet<string> PushedInPass { get; } = new();
        internal HashSet<string> PoppedInPass { get; } = new();
    }
}
