using System.Runtime.CompilerServices;
using UnityEditor;

namespace ExtEditor.UberMaterialPropertyDrawer
{
	/// <summary>
	/// MaterialEditor毎にグループの開閉状態をキャッシュするクラス。
	/// </summary>
	public static class GroupRenderStateCache
	{
		private static readonly ConditionalWeakTable<MaterialEditor, GroupRenderState> Cache = new();

		public static GroupRenderState GetOrCreate(MaterialEditor editor)
		{
			if (Cache.TryGetValue(editor, out var renderState)) return renderState;
			renderState = new GroupRenderState();
			Cache.Add(editor, renderState);
			return renderState;
		}
	}
}