## 概要
重要度：35
Vector2/Vector3 の描画ロジックがほぼ重複しており、修正のたびに複数ファイルを触る必要がある。DRY に反しており、片方だけ修正される不整合リスクがある。

## 該当ファイル
- Assets/UberMaterialPropertyDrawer/Editor/Vector2Drawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/Vector3Drawer.cs

### 関連ファイル
- Assets/UberMaterialPropertyDrawer/Editor/GUIHelper.cs

## 内容
Vector2Drawer と Vector3Drawer はレイアウト計算（labelWidth/fieldWidth/indent など）と描画フローがほぼ同一で、差分は Vector2Field/Vector3Field の呼び出しのみ。今後 UI 変更やバグ修正が入ると、双方で同じ修正を繰り返す必要があり、修正漏れの原因になる。
ユーザー回答では、差分は入力欄が 2 つ/3 つになる点のみで、それ以外の挙動は共通で良いとのこと。

### 不明点
なし（入力欄数の違いのみ、他挙動は共通で良いとの回答あり）。

## ゴール
- 共通処理をヘルパー関数または共通基底クラスへ集約し、差分部分だけを各 Drawer で実装できる状態。

## 備考
- DRY の観点。UI の一貫性維持と保守コスト削減に有効。
