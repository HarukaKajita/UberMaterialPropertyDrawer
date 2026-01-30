## 概要
Texture 系 Drawer から UI 描画とアセット/ベイク処理を分離し、単一責務に整理する。UI は Drawer 側、アセット生成とベイクは共通ユーティリティへ移す。

## 関連ファイル
- issues/TextureDrawerMixedResponsibilities/issue.md

## 経緯
GradientTextureDrawer / CurveTextureDrawer が UI・AssetDatabase 操作・ベイク処理を同時に担っている。責務分離により変更影響を限定し、保守性を高める。

## タスクリスト
[ ] アセット/ベイク処理をまとめるユーティリティの設計（例: `TextureBakeUtility`）
[ ] `Assets/UberMaterialPropertyDrawer/Editor/GradientTextureDrawer.cs` から AssetDatabase 操作と Texture2D 生成/更新処理を抽出
[ ] `Assets/UberMaterialPropertyDrawer/Editor/CurveTextureDrawer.cs` から同様の処理を抽出
[ ] UI 描画部分は Drawer 側に残し、ユーティリティはデータ入出力（Curve/Gradient・設定値）だけ受ける形にする
[ ] 既存の ScriptableObject データ (`GradientTextureData` / `CurveTextureData`) はそのまま利用する
[ ] Unity Editor でテクスチャ再生成・Tiling/Offset・グループ開閉の挙動が変わらないことを確認する

## 備考
- 既存の引数仕様（ch/res/bit/accum）と生成テクスチャ名は維持する。
- DRY の共通化計画と統合できる可能性があるため、設計時に整合を取る。
