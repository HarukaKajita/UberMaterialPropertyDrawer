## 概要
Drawer 追加時に UberDrawer を編集しなくても拡張できるよう、登録テーブル/ファクトリ方式に移行する。OCP を満たす拡張方法を用意する。

## 関連ファイル
- issues/UberDrawerClosedForExtension/issue.md

## 経緯
現在は UberDrawer のコンストラクタで drawer 文字列に応じた new 分岐をしており、追加のたびに UberDrawer を修正している。拡張点を外部化して変更不要にする。

## タスクリスト
[ ] 登録方式を決める（例: `Dictionary<string, Func<...>>` のレジストリ）
[ ] `Assets/UberMaterialPropertyDrawer/Editor/UberDrawer.cs` に登録 API（`RegisterDrawer(string key, Func<...>)` 等）を設計
[ ] 既存の drawer 文字列（BeginGroup/Vector2/CurveTexture 等）を登録する初期化処理を追加
[ ] UberDrawer の分岐生成コードをレジストリ参照に置き換える
[ ] 新 Drawer を追加する場合の登録箇所を決める（静的初期化 or 明示登録）
[ ] Unity Editor で既存の全 Drawer が従来通り動作することを確認する

## 備考
- 登録 API を public にするか internal にするかは運用方針に合わせる。
- 既存の文字列キーは互換性のため維持する。
