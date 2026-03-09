using System.Collections.Generic;

namespace ExtEditor.UberMaterialPropertyDrawer
{
	/// <summary>
	/// 現在のインスペクタ描画パスでどのグループの中を走査中かの追跡をするクラス
	/// </summary>
	public class GroupRenderState
	{
		internal readonly Stack<string> PathStack = new();
		// 同じプロパティに対してGetPropertyHeightとOnGUIの両方からpush/popされるのを防ぐための記録
		internal readonly HashSet<string> PushedProperties = new();
		internal readonly HashSet<string> PoppedProperties = new();
	}
}
