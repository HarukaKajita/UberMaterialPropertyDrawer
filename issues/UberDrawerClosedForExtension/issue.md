## 概要
重要度：45
UberDrawer が文字列分岐で Drawer を生成しており、Drawer 追加のたびに UberDrawer を修正する必要がある。OCP に反しており拡張時の変更コストが高い。

## 該当ファイル
- Assets/UberMaterialPropertyDrawer/Editor/UberDrawer.cs

### 関連ファイル
- Assets/UberMaterialPropertyDrawer/Editor/Vector2Drawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/Vector3Drawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/GradientTextureDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/CurveTextureDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/UberEnumDrawer.cs

## 内容
UberDrawer のコンストラクタ内で drawer 文字列に応じて具体クラスを new しているため、
新しい Drawer を追加するたびに UberDrawer を変更する必要がある。
拡張を既存コード変更なしで行えず、OCP を満たさない。

### 不明点
- Drawer の登録方式（辞書/ファクトリ/属性ベース）への移行方針があるか。

## ゴール
- UberDrawer 自体を変更せずに Drawer を追加できる仕組み（登録テーブルやファクトリ等）を導入する。

## 備考
- OCP（開放/閉鎖原則）の観点。
