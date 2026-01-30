## 概要
Vector2/Vector3 の Drawer で重複しているレイアウト計算と描画フローを共通化し、入力欄数の差分だけを各クラスに残す。

## 関連ファイル
- issues/DuplicateVectorDrawerLayout/issue.md

## 経緯
Vector2Drawer と Vector3Drawer はレイアウト計算と描画フローが同一で、差分は Vector2Field/Vector3Field の入力欄数のみ。共通化により DRY を満たす。

## タスクリスト
[ ] 共有化方針を決定（共通メソッド or 抽象基底クラス）し、最小の差分抽出方法を決める
[ ] 共通のレイアウト計算ロジックをヘルパーへ移す（labelWidth/valueWidth/indent 等）
[ ] Vector2/Vector3 の描画差分を delegate/virtual メソッド等で分離する
[ ] Vector2Drawer/Vector3Drawer を共通ロジック利用に置き換える
[ ] 既存挙動が変わらないことを Unity Editor で手動確認する

## 備考
- 入力欄数以外の挙動は同一で良い。
- 既存の groupName の扱いは維持する。
