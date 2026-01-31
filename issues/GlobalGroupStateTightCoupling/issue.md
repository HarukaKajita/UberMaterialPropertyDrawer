## 概要
重要度：65

UberGroupState が static なグローバル状態として実装され、各 Drawer がそれに直接依存しています。これはDIPに反し、状態管理の差し替えやテスト、複数マテリアル/インスペクターでの分離が困難になります。

## 該当ファイル
- Assets/UberMaterialPropertyDrawer/Editor/UberGroupState.cs
- Assets/UberMaterialPropertyDrawer/Editor/UberDrawerBase.cs
- Assets/UberMaterialPropertyDrawer/Editor/BeginGroupDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/BeginToggleGroupDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/EndGroupDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/UberDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/InitGroupDecorator.cs

### 関連ファイル
- Assets/UberMaterialPropertyDrawer/Editor/GUIHelper.cs

## 内容
GroupExpanded と GroupNest が static で保持され、全 Drawer がそれを直接参照/更新しています。これにより、状態のスコープ（マテリアル単位/インスペクター単位/セッション単位）を変更したい場合に広範囲の修正が必要になり、拡張性が低下します。また、テスト時に状態の初期化・隔離ができず、依存が強い設計になっています。

### 不明点
- グループ状態は「マテリアルごと」に保持すべき想定ですか、それとも「エディターセッション全体」で共有する想定ですか？

## ゴール
- Drawer からは抽象（例: IGroupState や Context）を介して状態を取得する。
- 状態のスコープ変更が局所的な実装差し替えで済む。

## 備考
- UnityEditor では MaterialEditor.userData 等に状態を持たせる設計も可能です。
