using System.Runtime.CompilerServices;
using UnityEditor;

namespace ExtEditor.UberMaterialPropertyDrawer
{
	/// <summary>
	/// MaterialEditor毎にグループの開閉状態をキャッシュするクラス。
	/// </summary>
	public static class GroupTraversalStateCache
	{
		private static readonly ConditionalWeakTable<MaterialEditor, GroupTraversalState> Cache = new();

		public static GroupTraversalState GetOrCreate(MaterialEditor editor)
		{
			if (Cache.TryGetValue(editor, out var renderState)) return renderState;
			renderState = new GroupTraversalState();
			Cache.Add(editor, renderState);
			return renderState;
		}
	}
}
