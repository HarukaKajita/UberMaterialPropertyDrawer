## 概要
Drawer 追加時に UberDrawer を編集しなくても拡張できるよう、属性 + TypeCache による自動登録方式に移行する。OCP を満たす拡張方法を用意する。

## 関連ファイル
- issues/UberDrawerClosedForExtension/issue.md

## 経緯
現在は UberDrawer のコンストラクタで drawer 文字列に応じた new 分岐をしており、追加のたびに UberDrawer を修正している。拡張点を外部化して変更不要にする。

## タスクリスト
- [ ] Drawer 登録用の属性（例: `DrawerKeyAttribute(string key)`）を追加する
- [ ] TypeCache を使って属性付き Drawer を列挙し、キー→型の辞書を一度だけ構築する
- [ ] キー重複があった場合はエラーとして検出し、登録を拒否する
- [ ] UberDrawer の文字列分岐生成コードを、登録辞書参照に置き換える
- [ ] 既存 Drawer 全てに属性を付与して移行する（既存キーは維持）
- [ ] Unity Editor で既存の全 Drawer が従来通り動作することを確認する

## 備考
- 新しい Drawer はクラス追加 + 属性付与だけで利用可能にする。
- キーの重複は許容しない（エラー）。
- 既存の文字列キーは互換性のため維持する。
