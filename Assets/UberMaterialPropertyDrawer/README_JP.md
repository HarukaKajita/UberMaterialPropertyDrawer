# UberMaterialPropertyDrawer

このドキュメントは日本語版 README です。英語版は `README.md` を参照してください。

## 概要
UberMaterialPropertyDrawer は、ShaderLab のマテリアルプロパティに対してグループ化やカスタム表示を追加する Unity Editor 拡張です。Foldout グループやトグル制御のセクション、ベクトル/列挙/テクスチャ生成（カーブ・グラデーション）などを提供し、Inspector の視認性と操作性を高めます。

## 要件
- Unity 2022.3 LTS（本プロジェクトは 2022.3.22f1 を使用）。
- Editor 専用パッケージのため、スクリプトは `Assets/UberMaterialPropertyDrawer/Editor/` に配置されています。

## インストール（UPM / Git）
Package Manager で Git URL を指定して追加します。
```
git@github.com:HarukaKajita/UberMaterialPropertyDrawer.git?path=Assets/UberMaterialPropertyDrawer
```
プライベートリポジトリの場合は、SSH キーや認証情報の設定が必要です。

## 使い方（ShaderLab）
Shader の `Properties` ブロックで Drawer クラス名を直接使います。
基本構文は以下です。
```
[DrawerName(GroupName, optionalArgs...)]
```
`GroupName` はグループ名、`optionalArgs` は追加設定です。

例:
```
[BeginGroup(MyGroup)] _GroupHeader("My Group", Float) = 0
[UberToggle(MyGroup)] _Enable("Enable", Float) = 0
[Vector2(MyGroup)] _Scale("Scale", Vector) = (1, 1, 0, 0)
[CurveTexture(MyGroup, ch4, res256, bit16)] _CurveTex("Curve", 2D) = "white" {}
[GradientTexture(MyGroup, res256)] _GradientTex("Gradient", 2D) = "white" {}
[EndGroup(MyGroup)] _GroupEnd("End", Float) = 0
```

### InitGroup デコレータ
`[InitGroupDecorator]` はグループ状態を初期化します。他の Drawer と併記できます。
```
[InitGroupDecorator][BeginGroup(MyGroup)] _GroupHeader("My Group", Float) = 0
```

### 対応する Drawer
- `BeginGroup` / `EndGroup`: Foldout グループの開始/終了。
- `BeginToggleGroup`: Float/Int プロパティのトグルで開閉するグループ。
- `UberToggle`: Float/Int プロパティ用のトグル UI。
- `Vector2` / `Vector3`: ラベル配置を調整したベクトル入力。
- `UberEnum`: Enum のポップアップ。型名指定または `名前, 値` の明示指定に対応。
- `CurveTexture`: カーブからテクスチャを生成してプロパティに割り当て。
- `GradientTexture`: グラデーションからテクスチャを生成してプロパティに割り当て。

型名指定の例:
```
[UberEnum(MyGroup, MyEnumType)] _Mode("Mode", Float) = 0
```

明示指定の例:
```
[UberEnum(MyGroup, Low, 0, High, 1)] _Mode("Mode", Float) = 0
```

### Curve/Gradient の引数
以下は任意で組み合わせ可能です。
- `resN`（例: `res256`）: テクスチャ解像度。
- `bit16`: 16-bit フォーマットを使用。
- `chN`（例: `ch1`, `ch4`）: チャンネル数（カーブ用）。
- `accum`: X 方向に加算的にカーブ値を積算（カーブ用）。

### グループ名の省略（グループ名のみの Drawer）
`Vector2` / `Vector3` / `UberToggle` はグループ名を省略できます。
- グループ外で使う場合: 常に表示（通常の Drawer と同じ挙動）
- グループ内で使う場合: 親グループの Foldout に従って表示/非表示が切り替わります

### 旧 `Uber` Drawer
`[Uber(GroupName)]` は従来の簡易グルーピング用として残しています。追加引数は無視され、通常の ShaderProperty 描画で表示されます。

## 仕組み
- グループの開閉状態はグループ名ごとに保持されます。
- カーブ/グラデーションのデータはマテリアルのサブアセットとして保存されます。
- 生成されたテクスチャもサブアセットとして保存され、プロパティに自動設定されます。

## Tips
- `BeginToggleGroup` と `UberToggle` は Float/Int プロパティで使用してください。
- 関連するプロパティは `BeginGroup`/`EndGroup` で囲うと見やすくなります。
- 反応しない場合はグループ名の一致（大文字小文字含む）を確認してください。
