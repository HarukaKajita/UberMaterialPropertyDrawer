# Uber Material Property Drawer (高機能マテリアルプロパティドロワー)

## 概要

Uber Material Property Drawerは、Unityで高度にカスタマイズされ整理されたマテリアルインスペクターを作成するための包括的なシステムです。デフォルトのフラットなプロパティリストの代わりに、このツールを使用すると、シェーダーの作成者はプロパティを折りたたみ可能なセクションにグループ化したり、トグル可能な機能グループを作成したり、列挙型やベクター用のカスタムドロワーを使用したり、プロパティのネストされた階層を管理したりすることができます。

このシステムは、シェーダーファイルのプロパティ属性に特定の構文を使用して呼び出される`MaterialPropertyDrawer`スクリプトのコレクションとして実装されています。

## 主な機能とコンポーネント

このシステムは主に`UberDrawer`クラスによって駆動され、シェーダーのマテリアルプロパティの`[Uber(...)]`属性で提供される引数に基づいて、さまざまな描画動作のディスパッチャーとして機能します。

-   **折りたたみ可能なグループ:**
    -   `[Uber(GroupName, BeginGroup)]`: インスペクターに`GroupName`というタイトルの折りたたみ可能なセクションを開始します。
    -   `[Uber(GroupName, EndGroup)]`: セクションの終わりを示します。
    -   このグループ内のプロパティ（同様に`[Uber(GroupName)]`でタグ付けされている）は、グループが展開されている場合にのみ表示されます。
-   **トグル可能なグループ:**
    -   `[Uber(GroupName, BeginToggleGroup)]`: `BeginGroup`に似ていますが、グループヘッダーにトグルチェックボックスが含まれます。このチェックボックスはシェーダー内のfloat/integerプロパティ（通常は0または1）を制御し、このグループ内のプロパティによって制御される機能を有効/無効にすることができます。グループも折りたたみ可能です。
-   **列挙型ドロップダウン:**
    -   `[Uber(GroupName, Enum, EnumTypeName)]`: floatまたはintegerのシェーダープロパティをドロップダウンメニューとして描画します。
    -   `EnumTypeName`は、プロジェクトで定義されたC#の列挙型の名前（例: `"UnityEngine.Rendering.BlendMode"`やカスタム列挙型）である必要があります。ドロワーはリフレクションを使用してドロップダウンを生成します。
-   **カスタムベクタードロワー:**
    -   `[Uber(GroupName, Vector2)]`: `Vector2`プロパティ用のコンパクトなドロワーを提供します。
    -   `[Uber(GroupName, Vector3)]`: `Vector3`プロパティ用のコンパクトなドロワーを提供します。
    -   これらはグループ内での一貫したレイアウトを保証します。
-   **カーブテクスチャドロワー:**
    -   `[Uber(GroupName, CurveTexture, size,mode,precision)]`: 最大4つの`AnimationCurve`を入力し、テクスチャにベイクしてマテリアルのサブアセットとして保持します。`size`はテクスチャ幅、`mode`は`value`または`cumulative`、`precision`は`8bit`または`half`を指定します。
-   **グラデーションテクスチャドロワー:**
    -   `[Uber(GroupName, GradientTexture, size,precision)]`: `Gradient`を入力し、テクスチャにベイクしてマテリアルのサブアセットとして保持します。`size`はテクスチャ幅、`precision`は`8bit`または`half`を指定します。
-   **プロパティの可視性:**
    -   `[Uber(GroupName)]`でタグ付けされた標準のマテリアルプロパティは、デフォルトのプロパティドロワーを使用して描画されますが、`GroupName`（およびネストされている場合はすべての親グループ）が展開されている場合にのみ表示されます。
-   **ネスティング:** グループを他のグループ内にネストして、より深い階層を作成できます。
-   **状態の初期化:**
    -   `[Uber(_, Init)]`: 通常、シェーダーの`Properties`ブロックの最初にあるダミープロパティに配置される特別なコマンド。すべてのグループの展開/折りたたみ状態をリセットします。グループ名`_`はこのユーティリティの慣例です。

## シェーダーでの使用方法

Uber Material Property Drawerシステムを使用するには、シェーダープロパティを`[Uber(...)]`属性で装飾します。

**1. 初期化 (推奨):**
シェーダーが選択または再コンパイルされたときに折りたたみ状態がリセットされるように、`Properties`ブロックの最初にダミープロパティにこれを配置します。
```shaderlab
Properties
{
    [Uber(_, Init)] _Init ("Editor State Initializer", Int) = 0 // グループ名 '_' は慣例
    // ... 残りのプロパティ
}
```

**2. 基本的な折りたたみ可能グループの作成:**
```shaderlab
    [Uber(MySection, BeginGroup)] _MySectionFoldout ("My Section Settings", Int) = 0 // グループヘッダー用のダミープロパティ
    [Uber(MySection)] _MyColor ("My Color", Color) = (1,1,1,1)
    [Uber(MySection)] _MyFloat ("My Float", Float) = 1.0
    [Uber(MySection, EndGroup)] _MySectionEnd ("End My Section", Int) = 0 // ダミープロパティ
```
-   `MySection`はこのグループの一意の名前です。
-   プロパティ`_MyColor`と`_MyFloat`は、「My Section Settings」が展開されている場合にのみ表示されます。

**3. トグル可能なグループの作成:**
`BeginToggleGroup`に関連付けられたプロパティがトグルとして機能します（オフの場合は0、オンの場合は1）。
```shaderlab
    [Uber(MyFeature, BeginToggleGroup)] _EnableMyFeature ("Enable My Feature", Float) = 1.0 // このプロパティがトグル
    [Uber(MyFeature)] _FeatureTexture ("Feature Texture", 2D) = "white" {}
    [Uber(MyFeature)] _FeatureIntensity ("Feature Intensity", Range(0, 5)) = 1.0
    [Uber(MyFeature, EndGrup)] _MyFeatureEnd ("End My Feature", Int) = 0
```
-   「Enable My Feature」ヘッダーには`_EnableMyFeature`に連動するチェックボックスが表示されます。
-   `_FeatureTexture`と`_FeatureIntensity`は、「Enable My Feature」が展開されている場合にのみ表示されます。シェーダーでのこれらの使用可能性は、通常`_EnableMyFeature`の値に依存します。

**4. 列挙型ドロップダウンの使用:**
C#の列挙型が定義されていることを確認してください。例:
```csharp
// C#スクリプト内 (Editorフォルダ内である必要はありません)
public enum MyBlendMode { Alpha, Additive, Multiply }
```
シェーダー内:
```shaderlab
    [Uber(Effects, BeginGroup)] _EffectsFoldout ("Effects Settings", Int) = 0
    [Uber(Effects, Enum, MyBlendMode)] _CurrentBlendMode ("Blend Mode", Float) = 0 // デフォルトは Alpha (0)
    [Uber(Effects, EndGroup)] _EffectsEnd ("End Effects", Int) = 0
```
-   `_CurrentBlendMode`プロパティは、「Alpha」、「Additive」、「Multiply」オプションを持つドロップダウンとして表示されます。

**5. カスタムベクタードロワーの使用:**
```shaderlab
    [Uber(Transform, BeginGroup)] _TransformFoldout ("Transform Settings", Int) = 0
    [Uber(Transform, Vector2)] _Offset ("UV Offset", Vector) = (0,0,0,0) // Vector2 は x,y を使用
    [Uber(Transform, Vector3)] _Scale ("Local Scale", Vector) = (1,1,1,0) // Vector3 は x,y,z を使用
    [Uber(Transform, EndGroup)] _TransformEnd ("End Transform", Int) = 0
```

**6. カーブのベイク:**
```shaderlab
    [Uber(Curves, CurveTexture, 256,cumulative,half)] _CurveTex ("Curve Texture", 2D) = "white" {}
```

**7. グラデーションのベイク:**
```shaderlab
    [Uber(Curves, GradientTexture, 256,half)] _GradientTex ("Gradient Texture", 2D) = "white" {}
```

**8. グループのネスティング:**
アクティブなグループ内にもう一つの`BeginGroup`を定義するだけです。
```shaderlab
    [Uber(Parent, BeginGroup)] _ParentFoldout ("Parent Group", Int) = 0
    [Uber(Parent)] _ParentProp ("Parent Property", Float) = 0.0

        [Uber(Child, BeginGroup)] _ChildFoldout ("Child Group (inside Parent)", Int) = 0
        [Uber(Child)] _ChildProp ("Child Property", Color) = (1,0,0,1)
        [Uber(Child, EndGroup)] _ChildEnd ("End Child", Int) = 0

    [Uber(Parent, EndGroup)] _ParentEnd ("End Parent", Int) = 0
```
-   「Child Group」は「Parent Group」が展開されている場合にのみ表示されます。`_ChildProp`は両方が展開されている場合にのみ表示されます。

**重要な注意点:**
-   **グループ名:** グループ名は重要であり、展開/折りたたみ状態を追跡するために使用されます。一意で説明的な名前を選択してください。
-   **ダミープロパティ:** `BeginGroup`、`EndGroup`、および`Init`ドロワーは、通常、シェーダー内のダミーのintegerまたはfloatプロパティ（例: `_MySectionFoldout`、`_Init`）にアタッチされます。これらのダミープロパティの実際の値は通常ドロワーロジックでは使用されませんが、プロパティ自体がドロワー属性のアンカーとして機能します。`BeginToggleGroup`の場合、それがアタッチされているプロパティはトグル状態のために*使用されます*。
-   **順序の重要性:** `EndGroup`呼び出しは、対応する`BeginGroup`または`BeginToggleGroup`と一致する必要があります。システムは内部的にスタックを使用するため、不一致はエラーや予期しないUIの動作を引き起こす可能性があります。
-   **シェーダーのコンパイル:** これらの属性への変更は、マテリアルインスペクターが更新されるためにシェーダーが再コンパイルされる必要があります。

このシステムは、複雑なマテリアルに対してクリーンでユーザーフレンドリー、かつ高度に整理されたインターフェースを作成するための堅牢なツールキットを提供します。
