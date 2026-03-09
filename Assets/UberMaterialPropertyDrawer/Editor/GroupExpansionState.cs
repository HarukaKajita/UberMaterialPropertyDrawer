using System.Collections.Generic;

namespace ExtEditor.UberMaterialPropertyDrawer
{
	/// <summary>
	/// グループ自身の開閉状態を保持するクラス
	/// </summary>
	public class GroupExpansionState
	{
		// groupPath -> 開閉状態　の辞書
		internal readonly Dictionary<string, bool> ExpandedByPath = new();
	}
}
