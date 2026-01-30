## 概要
重要度：40
GradientTextureDrawer / CurveTextureDrawer が UI 描画、アセット検索・生成、テクスチャベイク処理を同時に担っており、SRP を満たしていない。変更理由が複数に分散している。

## 該当ファイル
- Assets/UberMaterialPropertyDrawer/Editor/GradientTextureDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/CurveTextureDrawer.cs

### 関連ファイル
- Assets/UberMaterialPropertyDrawer/Editor/GradientTextureData.cs
- Assets/UberMaterialPropertyDrawer/Editor/CurveTextureData.cs
- Assets/UberMaterialPropertyDrawer/Editor/GUIHelper.cs

## 内容
両 Drawer が以下の責務を同時に担っている。
- UI 描画（GradientField / CurveField、Texture/TilingOffset のレイアウト）
- AssetDatabase を使った ScriptableObject/Texture2D の検索・生成
- Texture2D のベイク処理と再生成条件判定

これらは変更理由が異なるため、責務分離しないと変更影響が広がる。

### 不明点
- テクスチャ生成/アセット管理を専用ユーティリティに切り出す方針があるか。

## ゴール
- UI 描画とアセット/ベイク処理を分離し、各クラスが単一責務となる構成に整理する。

## 備考
- SRP（単一責任原則）の観点。
