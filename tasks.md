# タスク
未着手、保留中のタスクを記載するファイル。

### ルール
- ルールを守れ。
- タスクを起票する際は`## タスク名`でタイトルを付ける。
- タスクを起票する際は、`### 目次`にタスクを追加する。
- タスクを起票する際は、コンテキストを失っても記載されている内容を確認すれば、タスクの内容が理解できるように必要な情報は全て記載する。
- タスクが完了したら、本文から該当タスクを削除する。
- タスクが完了したら、`### 目次`からタスクを削除する。

### 目次
- [01 基盤クラスと generated texture 系の大規模リネーム](#01-基盤クラスと-generated-texture-系の大規模リネーム)
- [02 GroupState 系の中核クラス名を整理](#02-groupstate-系の中核クラス名を整理)
- [03 utility クラスを責務単位に分割](#03-utility-クラスを責務単位に分割)
- [04 レイアウト helper と inspector repaint API の命名統一](#04-レイアウト-helper-と-inspector-repaint-api-の命名統一)
- [05 可視性判定と generated texture API の命名調整](#05-可視性判定と-generated-texture-api-の命名調整)
- [06 数値・オプション関連の命名とスペルミスを修正](#06-数値オプション関連の命名とスペルミスを修正)
- [07 公開ドキュメントとサンプルの命名を実装方針へ合わせる](#07-公開ドキュメントとサンプルの命名を実装方針へ合わせる)

## 01 基盤クラスと generated texture 系の大規模リネーム
- 目的：
    - generated texture 周辺で意味が曖昧な基底クラス名を先に確定し、後続の型参照更新の基準を作る。
- 背景：
    - `GeneratedTextureDataBase` は `database` と誤読されやすく、以後の議論で確定した `GeneratedTextureDataAssetBase` に改める方針になっている。
    - この型は `CurveData`、`GradientData`、`GeneratedTextureBinding<TData>`、`GeneratedTextureDrawerBase<TData, TOption>`、`MaterialSubAssetStore<TData>` など複数クラスの型制約や引数型として広く使われている。
- 対応内容
    - `GeneratedTextureDataBase` を `GeneratedTextureDataAssetBase` に改名する。
    - 関連する継承、ジェネリック制約、メソッド引数、ローカル変数の型名参照を一括更新する。
    - 型名変更後も generated texture 系の責務境界が維持されていることを確認する。
- 完了条件
    - `GeneratedTextureDataBase` という型名参照がコードベースから消えている。
    - generated texture 系のコードが `GeneratedTextureDataAssetBase` を前提に一貫した名前になっている。

## 02 GroupState 系の中核クラス名を整理
- 目的：
    - グループ状態管理の中心概念を具体的な名前へ寄せ、状態保持と描画走査状態の違いをコード上で明確にする。
- 背景：
    - 方針として `GroupData` は `ShaderGroupStateData`、`UberGroupState` は `GroupStateManager`、`GroupRenderState` は `GroupTraversalState`、`GroupRenderStateCache` は `GroupTraversalStateCache` へ変更することが確定している。
    - これらは相互参照が多く、途中で utility 分割や細部の改名に入るより先に核となる概念名を確定した方が安全。
- 対応内容
    - `GroupData` を `ShaderGroupStateData` に改名する。
    - `GroupData.Shader` を `OwnerShader`、`GroupData.DepHashAtSet` を `DependencyHashAtCacheTime` に改名する。
    - `UberGroupState` を `GroupStateManager` に改名する。
    - `GroupRenderState` を `GroupTraversalState`、`GroupRenderStateCache` を `GroupTraversalStateCache` に改名する。
    - 関連するファイル名、型名、参照名を一括更新する。
- 完了条件
    - GroupState 系の中心クラス名が確定方針どおりに更新されている。
    - `GroupData`、`UberGroupState`、`GroupRenderState`、`GroupRenderStateCache` という旧名参照が残っていない。

## 03 utility クラスを責務単位に分割
- 目的：
    - `Util` の責務を分解し、MaterialProperty 操作と Material asset 操作を別クラスへ明示的に分ける。
- 背景：
    - `Util` は property 値変換、ラベル生成、sub-asset 探索、保存遅延まで抱えており、名前が責務を隠している。
    - 中核クラスのリネーム後に着手することで、どの helper がどの層に属するかを整理しやすくなる。
- 対応内容
    - `Util` を廃止する。
    - property 値変換とラベル生成を `MaterialPropertyUtility` に移す。
    - sub-asset 探索と保存遅延を `MaterialAssetUtility` に移す。
    - 呼び出し元を責務に応じて適切な utility へ差し替える。
- 完了条件
    - `Util` クラスが削除されている。
    - 既存メソッドが `MaterialPropertyUtility` と `MaterialAssetUtility` に責務分割されている。

## 04 レイアウト helper と inspector repaint API の命名統一
- 目的：
    - GUI レイアウト関連と inspector 再描画 API の命名を、責務が分かる形へ揃える。
- 背景：
    - 方針として `GUIHelper` は `DrawerLayoutUtility`、`RepaintAllInspector` は `RepaintAllInspectors` に変更する。
    - utility 分割のあとに行うことで、`DrawerLayoutUtility` と `MaterialPropertyUtility` / `MaterialAssetUtility` の役割分担が見えやすくなる。
- 対応内容
    - `GUIHelper` を `DrawerLayoutUtility` に改名する。
    - `InspectorRepainter.RepaintAllInspector` を `RepaintAllInspectors` に改名する。
    - 参照箇所を全て更新する。
- 完了条件
    - レイアウト helper と repaint API の命名が方針どおりに統一されている。
    - `GUIHelper` と `RepaintAllInspector` の旧名参照が残っていない。

## 05 可視性判定と generated texture API の命名調整
- 目的：
    - 主要 API の呼び出し意図が読み取れるように、bool 判定名と単数複数 API 名を整理する。
- 背景：
    - 方針として `IsVisibleDrawer` は `IsDrawerVisible`、`MaterialSubAssetStore` の `EnsureBindings` オーバーロードは単数複数で分けることが確定している。
    - utility と中核クラスの命名が固まった後なら、API の読みやすさに集中して変更できる。
- 対応内容
    - `UberDrawerBase.IsVisibleDrawer` を `IsDrawerVisible` に改名する。
    - `MaterialSubAssetStore` の複数版メソッドを `EnsureBindings`、単数版メソッドを `EnsureBinding` に分ける。
    - 単数複数 API の呼び出し元を対応する名前へ更新する。
- 完了条件
    - bool 判定メソッドが `IsDrawerVisible` へ統一されている。
    - `MaterialSubAssetStore` の単数複数 API が名前で区別できる。

## 06 数値・オプション関連の命名とスペルミスを修正
- 目的：
    - 機械的に修正可能な命名とスペルミスをまとめて片付け、レビューコストを下げる。
- 背景：
    - 方針として `CommonOptions` は `TextureOptions`、`min` / `max` は `_minValue` / `_maxValue`、ローカル変数は `channelCount` / `resolution` / `bitDepth` / `accumulate`、スペルミスは修正することが確定している。
    - ここは意味変更を伴わない変更が多いため、主要 API のリネーム後にまとめて行うのが効率的。
- 対応内容
    - `CurveTextureOptions.CommonOptions` を `TextureOptions` に改名する。
    - `UberIntRangeDrawer` の private field `min`, `max` を `_minValue`, `_maxValue` に改名する。
    - `UberToggleDrawer` の `_dimention` を `_dimension` に修正する。
    - `DrawerLayoutUtility` 内の `TillingOffsetPropertyHeight` を `TilingOffsetPropertyHeight` に修正する。
    - `CurveTextureDrawer` と `GradientTextureDrawer` の省略ローカル変数を `channelCount`, `resolution`, `bitDepth`, `accumulate` へ改名する。
- 完了条件
    - このタスクで対象とした命名・スペルミスが全て修正されている。
    - 省略名やスペルミスによる旧名参照が残っていない。

## 07 公開ドキュメントとサンプルの命名を実装方針へ合わせる
- 目的：
    - README とサンプル shader を、確定した命名方針に合わせて利用者向けに整合させる。
- 背景：
    - 方針として公開説明は `Vector(dim)` に統一し、`InitGroupDecorator` は `ResetGroupStateDecorator` に合わせる。
    - サンプル shader の誤記は単純修正でよいと決まっている。
    - 実装名の変更が先に終わってからでないと、公開ドキュメント側で再修正が発生する。
- 対応内容
    - README 英日版の `Vector2` / `Vector3` 表記を `Vector(dim)` に統一する。
    - README とサンプルの `InitGroupDecorator` 表記を `ResetGroupStateDecorator` に更新する。
    - `Test_UberProps.shader` の `_BeginGroundChild`、`_EndGroundChild`、`_EndSomGroup`、`Unverlay` などの誤記を単純修正する。
    - 実装と README とサンプルの公開名が一致していることを確認する。
- 完了条件
    - README とサンプルが実装済みの命名方針と一致している。
    - サンプル shader の既知の誤記が解消されている。
