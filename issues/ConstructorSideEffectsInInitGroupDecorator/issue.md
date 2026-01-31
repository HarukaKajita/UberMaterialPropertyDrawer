## 概要
重要度：60

InitGroupDecorator のコンストラクタで UberGroupState.ResetAll() を実行しており、インスタンス生成時にグローバル副作用が発生します。MaterialPropertyDrawer の利用者は「描画時にのみ副作用が起きる」前提で扱うため、LSPの観点で置換可能性が下がります。

## 該当ファイル
- Assets/UberMaterialPropertyDrawer/Editor/InitGroupDecorator.cs

### 関連ファイル
- Assets/UberMaterialPropertyDrawer/Editor/UberGroupState.cs

## 内容
Unity はシェーダープロパティの解析時に Drawer を生成するため、生成タイミングや回数は制御できません。そのためコンストラクタで状態をリセットすると、意図しないタイミングで全グループ状態が初期化される可能性があります。結果として、描画順やキャッシュの挙動に依存した不安定な設計になります。

### 不明点
- ResetAll は「描画開始時に1回」だけ実行したい意図でしょうか？それとも「特定プロパティが評価されるたび」に実行したい意図でしょうか？

## ゴール
- 副作用のある処理は OnGUI 等の明示的なライフサイクル内に移動される。
- Drawer の生成順に依存しないグループ初期化が実現されている。

## 備考
- 初期化トリガーを明示する仕組み（専用の Init 呼び出しやコンテキスト初期化）に分離すると安全です。
