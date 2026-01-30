## 概要
Gradient/Curve の Drawer で重複している処理を共通化し、描画ロジックの差分（GradientField/CurveField と蓄積処理など）だけを各クラスに残す。共通化により修正漏れと挙動不一致を防ぐ。

## 関連ファイル
- issues/DuplicateTextureDrawerWorkflow/issue.md

## 経緯
既存の GradientTextureDrawer と CurveTextureDrawer には、引数パース、テクスチャ設定検知、Texture2D の生成/更新、Asset 付随処理、レイアウト計算が重複している。差分要件はないため、共通化して DRY に寄せる方針。

## タスクリスト
- [ ] 共有化方針を決定（共通基底クラス or static helper）し、影響範囲を整理する
- [ ] args パース（ch/res/bit）を共通化するヘルパーを作成する（例: GUIHelper もしくは新規クラス）
- [ ] テクスチャ設定変更検知と PickCorrectTextureFormat を共通化する
- [ ] サブアセット取得/生成（ScriptableObject・Texture2D）処理を共通化する
- [ ] Gradient/Curve 固有の UI 部分（GradientField/CurveField、accum など）を各 Drawer 側へ残す
- [ ] GradientTextureDrawer と CurveTextureDrawer から重複コードを削除し共通実装を利用する
- [ ] 既存挙動が変わらないことを Unity Editor で手動確認する（テクスチャ再生成、Tiling/Offset、グループ開閉）

## 備考
- 既存の public API/属性指定（引数文字列）を維持する。
- ScriptableObject のデータ型は現状のまま利用する。
