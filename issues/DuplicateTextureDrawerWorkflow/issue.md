## 概要
重要度：50

GradientTextureDrawer と CurveTextureDrawer に共通ロジックが多数あり、DRY に反している。修正の二重化と挙動の不一致リスクがある。

## 該当ファイル
- Assets/UberMaterialPropertyDrawer/Editor/GradientTextureDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/CurveTextureDrawer.cs

### 関連ファイル
- Assets/UberMaterialPropertyDrawer/Editor/GradientTextureData.cs
- Assets/UberMaterialPropertyDrawer/Editor/CurveTextureData.cs
- Assets/UberMaterialPropertyDrawer/Editor/GUIHelper.cs

## 内容
以下の処理が両クラスでほぼ同じ実装になっている。
- args のパース（ch/res/bit）
- テクスチャ設定変更検知（解像度・フォーマット）
- テクスチャ生成/再初期化と Asset への追加
- レイアウト計算（ラベル、テクスチャ、Tiling/Offset）

共通化されていないため、修正や改善が片側だけに入る可能性が高く、将来的なバグの温床になる。

### 不明点
なし（共通化して良いとの回答あり）。

## ゴール
- 共通ロジックをユーティリティまたは共通基底クラスに集約し、Drawer 固有部分（CurveField/GradientField など）だけを個別実装にする。

## 備考
- DRY の観点。共通化により修正箇所の一元化と挙動の一貫性が期待できる。
- Curve/Gradient の差分要件はなし（共通化可能）。
