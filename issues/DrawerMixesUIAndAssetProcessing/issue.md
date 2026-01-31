## 概要
重要度：70

GradientTextureDrawer と CurveTextureDrawer が UI描画、アセット管理、テクスチャ生成を一つのクラスにまとめており、変更理由が複数に跨っているためSRPを満たしません。UIの見た目変更、保存方式変更、テクスチャ生成仕様変更が相互に影響し、保守性・テスト容易性が低下します。

## 該当ファイル
- Assets/UberMaterialPropertyDrawer/Editor/GradientTextureDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/CurveTextureDrawer.cs

### 関連ファイル
- Assets/UberMaterialPropertyDrawer/Editor/GradientTextureData.cs
- Assets/UberMaterialPropertyDrawer/Editor/CurveTextureData.cs
- Assets/UberMaterialPropertyDrawer/Editor/GUIHelper.cs

## 内容
OnGUI 内で UI レイアウト、AssetDatabase を使ったサブアセットの検索/生成、BakeTexture によるピクセル生成を同時に扱っています。これにより、例えば保存方法やベイク仕様を変更したい場合でも UI ロジックに手を入れる必要が生じ、影響範囲が広がります。さらに Gradient と Curve でほぼ同種の資産操作・ベイク処理が重複しており、修正点の散在が発生しています。

### 不明点
- 将来的にテクスチャ生成や保存方式の差し替え（例: 外部アセット保存やキャッシュ化）を想定していますか？

## ゴール
- Drawer は UI の入力/表示に専念し、テクスチャ生成・アセット管理は専用クラスに分離されている。
- ベイクや保存の変更が Drawer の修正なしで差し替え可能になっている。

## 備考
- 共通化できる処理（アセット検索/生成、サイズ・フォーマット判定など）をサービス化すると重複を減らせます。
