using UnityEditor;

namespace ExtEditor.UberMaterialPropertyDrawer
{
	public static class GroupVisibility
	{
		/// <summary>
		/// MaterialPropertyDrawerを表示するかどうかを決定します。
		/// </summary>
		/// <param name="data">グループデータ。</param>
		/// <param name="editor">マテリアルエディタ。</param>
		/// <param name="groupName">グループ名。</param>
		/// <returns>コンテンツを表示する場合は true 。それ以外は false 。</returns>
		public static bool CanShowContent(ShaderGroupStateData data, MaterialEditor editor, string groupName)
		{
			var currentPath = GroupStateManager.GetCurrentPath(editor);
			var hasScope = !string.IsNullOrEmpty(currentPath);
			var hasGroupName = !string.IsNullOrEmpty(groupName);

			if (!hasScope && !hasGroupName) return true;
			if ( hasScope && !hasGroupName) return GroupStateManager.IsCurrentScopeVisible(data, editor);
			if (!hasScope &&  hasGroupName)
			{
				UberDrawerLogger.LogWarning($"Group [{groupName}] is not scoped with BeginGroup/EndGroup. fix shader property attribute.");
				return false;
			}
			
			var currentGroupMatched = currentPath == groupName || currentPath.EndsWith("/"+groupName);
			if (!currentGroupMatched)
			{
				UberDrawerLogger.LogError($"Group [{groupName}] is written in path [{currentPath}]. fix shader property attribute.");
				return false;
			}
			return GroupStateManager.IsCurrentScopeVisible(data, editor);
		}

		/// <summary>
		/// グループのヘッダーを表示するかどうかを決定します。
		/// </summary>
		/// <param name="data">グループデータ。</param>
		/// <param name="editor">マテリアルエディタ。</param>
		/// <returns>ヘッダーを表示する場合は true 。それ以外は false 。</returns>
		public static bool CanShowHeader(ShaderGroupStateData data, MaterialEditor editor)
		{
			return GroupStateManager.IsParentScopeVisible(data, editor);
		}
	}
}
