# UberMaterialPropertyDrawer Tool

このファイルは、UberMaterialPropertyDrawerツールでの作業時にClaude Codeへのガイダンスを提供します。

## ツール概要

UberMaterialPropertyDrawerツールは、高度なマテリアルプロパティUI システムを提供するエディター拡張です。グループ化、カスタムコントロール、プロシージャルテクスチャ生成を含む洗練されたマテリアルインスペクター拡張を実現します。

## 主要機能

### 基本機能
- **プロパティグループ化**: マテリアルプロパティを折りたたみ可能なグループに整理
- **カスタム Enum ドローワー**: グループサポート付きの拡張enumプロパティドローワー
- **プロシージャルテクスチャ**: 組み込みグラデーションとカーブテクスチャ生成
- **ベクタープロパティドローワー**: カスタムVector2・Vector3プロパティコントロール
- **ネストグループ**: 階層的プロパティ整理のサポート
- **トグルグループ**: オン・オフ切り替え可能なグループ

### シェーダーでの使用方法
シェーダープロパティでカスタム属性を使用:
```hlsl
[Uber(GroupName)] _Property ("Display Name", Float) = 0
[Uber(GroupName, BeginGroup)] _GroupStart ("Group Name", Float) = 0
[Uber(GroupName, EndGroup)] _GroupEnd ("", Float) = 0
[Uber(GroupName, Enum, EnumTypeName)] _EnumProp ("Enum", Float) = 0
[Uber(GroupName, GradientTexture)] _GradTex ("Gradient", 2D) = "white" {}
[Uber(GroupName, CurveTexture)] _CurveTex ("Curve", 2D) = "white" {}
[Uber(GroupName, Vector2)] _Vec2 ("Vector2", Vector) = (0,0,0,0)
[Uber(GroupName, Vector3)] _Vec3 ("Vector3", Vector) = (0,0,0,0)
[Uber(GroupName, BeginToggleGroup)] _ToggleGroup ("Toggle Group", Float) = 0
[Uber(Init)] _Init ("", Float) = 0
```

## 実装詳細

### ファイル構造
- **UberDrawer.cs**: メインドローワーと委譲システム
- **BeginGroupDrawer.cs / EndGroupDrawer.cs**: グループ境界管理
- **BeginToggleGroupDrawer.cs**: トグル有効グループ
- **UberEnumDrawer.cs**: 型リフレクション付き拡張enum サポート
- **GradientTextureDrawer.cs / CurveTextureDrawer.cs**: プロシージャルテクスチャ生成
- **Vector2Drawer.cs / Vector3Drawer.cs**: カスタムベクタープロパティコントロール

### 中核クラス詳細

#### UberDrawer (メインディスパッチャー)
- **委譲システム**: 属性パラメータに基づく専用ドローワーへの委譲
- **状態管理**: 静的コレクションを使用したグループ状態とネスト管理
- **初期化処理**: `[Uber(Init)]` による状態リセット機能

#### グループ管理システム
- **BeginGroupDrawer**: フォールドアウトグループの開始処理
- **EndGroupDrawer**: グループ終了とインデント復元
- **BeginToggleGroupDrawer**: 切り替え可能グループの実装
- **ネストサポート**: スタックベースのネストグループ管理

#### プロシージャルテクスチャ
- **GradientTextureDrawer**: Gradient → Texture2D 変換とベイク
- **CurveTextureDrawer**: AnimationCurve → Texture2D 変換とベイク
- **サブアセット統合**: マテリアルのサブアセットとしてテクスチャ保存
- **形式オプション**: サイズと精度パラメータのサポート

## 技術的特徴

### 状態管理システム
```csharp
static Dictionary<string, bool> groupExpandedStates; // グループ展開状態
static Stack<int> indentLevelStack; // インデントレベルスタック  
static Dictionary<string, bool> groupToggleStates; // トグルグループ状態
```

### 型安全な Enum サポート
- **実行時型発見**: リフレクションによるEnum型の動的発見
- **名前空間サポート**: 完全修飾型名での型解決
- **エラーハンドリング**: 型が見つからない場合の適切なフォールバック

### プロシージャルテクスチャ統合
- **AssetDatabase.AddObjectToAsset()**: サブアセットとしての保存
- **自動更新**: プロパティ変更時の自動テクスチャ再生成  
- **メモリ効率**: 必要時のみテクスチャ作成

## 設定詳細

### 属性パラメータ
- **グループ名**: 第1パラメータでグループを指定
- **機能タイプ**: 第2パラメータで機能を指定（BeginGroup, EndGroup, Enum, etc.）
- **追加パラメータ**: 型名やサイズなどの追加設定

### グループ初期化
- **[Uber(Init)]**: マテリアル単位でのグループ状態リセット
- **静的状態**: 複数マテリアル間での状態共有
- **セッション永続**: エディターセッション中の状態保持

### テクスチャ生成設定
- **デフォルトサイズ**: 256x256ピクセル
- **形式**: RGBA32（アルファサポート）
- **保存場所**: マテリアルアセットのサブアセット

## 開発ノート

### 拡張ポイント
- **新しいドローワータイプ**: UberDrawer.OnGUI()に新しい分岐追加
- **カスタムグループタイプ**: 新しいグループドローワーの実装
- **テクスチャ形式**: 新しいプロシージャルテクスチャタイプの追加

### パフォーマンス考慮
- **静的コレクション**: グループ状態のエディター間共有
- **遅延テクスチャ生成**: 必要時のみテクスチャ作成
- **効率的な再描画**: 変更時のみUI更新

### エラーハンドリング
- **型解決失敗**: 存在しないEnum型でのフォールバック
- **循環参照**: グループネストでの循環検出
- **アセット作成エラー**: テクスチャ生成失敗時の適切な処理

### 互換性
- **Unity バージョン**: Unity 2022.3+ での動作確認
- **シェーダー統合**: カスタム属性のシェーダー互換性
- **マテリアル形式**: 標準マテリアルとの完全互換性

### 使用場面
- **複雑なシェーダー**: 大量プロパティを持つシェーダーのUI整理
- **アーティストワークフロー**: 直感的なマテリアル編集インターフェース
- **プロシージャル生成**: リアルタイムテクスチャ生成ワークフロー

### 制限事項
- **エディター専用**: ランタイムでは動作しない
- **マテリアル依存**: マテリアルアセットが必要
- **静的状態**: エディター再起動でグループ状態リセット

### ベストプラクティス
- **グループ設計**: 論理的なプロパティグループ化
- **命名規則**: 明確なグループ名とプロパティ名
- **初期化配置**: マテリアル上部での[Uber(Init)]配置

### 今後の機能
- **永続状態**: グループ状態の永続化
- **テーマサポート**: UI テーマのカスタマイズ
- **プリセット**: マテリアル設定プリセット機能

### トラブルシューティング
- **グループ状態異常**: [Uber(Init)]による状態リセット
- **テクスチャ生成失敗**: アセット権限とディスク容量確認
- **型解決エラー**: Enum型名とアセンブリ参照確認

## 現状の課題

### 重要度: Critical（緊急）
- **静的状態破損**: グローバル静的辞書が異なるマテリアル間で破損
  - **影響**: 複数マテリアル編集時の状態混在とクラッシュリスク
  - **改善提案**: マテリアル固有キーまたはインスタンスベース状態管理

- **AssetDatabase破損**: 適切なシーケンスなしの複数ImportAsset呼び出し
  - **影響**: アセットデータベース不整合とプロジェクト破損
  - **改善提案**: バッチ処理とアトミック操作の実装

### 重要度: High（高）
- **メモリリーク**: サブアセットが適切にクリーンアップされない
  - **影響**: 長時間使用でメモリ不足とパフォーマンス低下
  - **改善提案**: 適切なライフサイクル管理と自動削除機能

- **パフォーマンス**: GUI更新のたびにテクスチャベイク処理
  - **影響**: マテリアルインスペクター操作の重大な遅延
  - **改善提案**: 変更検出とキャッシュ機能

- **スレッドセーフティ**: 同期なしでの静的コレクションアクセス
  - **影響**: マルチスレッド環境でのデータ競合とクラッシュ
  - **改善提案**: ロック機構またはスレッドローカル実装

### 重要度: Medium（中）
- **デバッグスパム**: 本番コードに残されたDebug.Log呼び出し
  - **影響**: コンソールログの汚染とパフォーマンス低下
  - **改善提案**: コンディショナルコンパイルまたは完全削除

- **エラー回復**: テクスチャ操作失敗時にマテリアルが不正状態
  - **影響**: 一部テクスチャが生成されずUI操作不能
  - **改善提案**: ロールバック機能と状態検証

### 具体的な改善コード例

```csharp
// 静的状態破損修正
public class UberDrawer : MaterialPropertyDrawer
{
    // マテリアル固有の状態管理
    private static readonly Dictionary<(Material, string), bool> GroupExpanded = 
        new Dictionary<(Material, string), bool>();
    private static readonly Dictionary<Material, Stack<string>> GroupNests = 
        new Dictionary<Material, Stack<string>>();
    private static readonly object StateLock = new object();
    
    internal static bool GetGroupExpanded(Material material, string groupName)
    {
        lock (StateLock)
        {
            return GroupExpanded.TryGetValue((material, groupName), out var expanded) ? expanded : false;
        }
    }
    
    internal static void SetGroupExpanded(Material material, string groupName, bool expanded)
    {
        lock (StateLock)
        {
            GroupExpanded[(material, groupName)] = expanded;
        }
    }
    
    internal static void PushGroup(Material material, string groupName)
    {
        lock (StateLock)
        {
            if (!GroupNests.TryGetValue(material, out var stack))
            {
                stack = new Stack<string>();
                GroupNests[material] = stack;
            }
            stack.Push(groupName);
        }
    }
    
    internal static string PopGroup(Material material)
    {
        lock (StateLock)
        {
            if (GroupNests.TryGetValue(material, out var stack) && stack.Count > 0)
            {
                return stack.Pop();
            }
            return null;
        }
    }
    
    // 状態初期化
    [MenuItem("Tools/Uber Material/Clear State")]
    public static void ClearAllState()
    {
        lock (StateLock)
        {
            GroupExpanded.Clear();
            GroupNests.Clear();
        }
        Debug.Log("Uber Material状態をクリアしました");
    }
}

// AssetDatabase操作修正
public class GradientTextureDrawer : MaterialPropertyDrawer
{
    private static readonly Dictionary<Material, List<UnityEngine.Object>> PendingSubAssets = 
        new Dictionary<Material, List<UnityEngine.Object>>();
        
    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        Material mat = editor.target as Material;
        var data = GetGradientData(prop);
        
        EditorGUI.BeginChangeCheck();
        
        // UI描画...
        DrawGradientField(position, data, label);
        
        if (EditorGUI.EndChangeCheck())
        {
            // バッチ処理でテクスチャ作成
            EnqueueTextureCreation(mat, prop, data);
        }
    }
    
    private void EnqueueTextureCreation(Material material, MaterialProperty prop, GradientTextureData data)
    {
        if (!PendingSubAssets.TryGetValue(material, out var pendingList))
        {
            pendingList = new List<UnityEngine.Object>();
            PendingSubAssets[material] = pendingList;
        }
        
        // 遅延実行でバッチ処理
        EditorApplication.delayCall += () => ProcessPendingTextures(material);
    }
    
    private static void ProcessPendingTextures(Material material)
    {
        if (!PendingSubAssets.TryGetValue(material, out var pendingList) || pendingList.Count == 0)
            return;
            
        try
        {
            string path = AssetDatabase.GetAssetPath(material);
            if (string.IsNullOrEmpty(path)) return;
            
            AssetDatabase.StartAssetEditing();
            
            foreach (var asset in pendingList)
            {
                if (asset != null)
                {
                    AssetDatabase.AddObjectToAsset(asset, material);
                    EditorUtility.SetDirty(asset);
                }
            }
            
            EditorUtility.SetDirty(material);
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"テクスチャ作成バッチ処理エラー: {ex.Message}");
            
            // 失敗時のクリーンアップ
            foreach (var asset in pendingList)
            {
                if (asset != null) DestroyImmediate(asset);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(material));
            
            // 処理済みリストクリア
            PendingSubAssets.Remove(material);
        }
    }
    
    // リソース管理改善
    private Texture2D CreateTexture(GradientTextureData data, string textureName)
    {
        Texture2D texture = null;
        try
        {
            texture = new Texture2D(data.textureSize, 1, TextureFormat.RGBA32, false);
            texture.name = textureName;
            texture.hideFlags = HideFlags.HideInHierarchy;
            
            // テクスチャデータ生成
            Color[] colors = new Color[data.textureSize];
            for (int i = 0; i < data.textureSize; i++)
            {
                float t = (float)i / (data.textureSize - 1);
                colors[i] = data.gradient.Evaluate(t);
            }
            
            texture.SetPixels(colors);
            texture.Apply(false);
            
            return texture;
        }
        catch (Exception ex)
        {
            Debug.LogError($"テクスチャ作成エラー: {ex.Message}");
            if (texture != null) DestroyImmediate(texture);
            return null;
        }
    }
}

// パフォーマンス改善（キャッシュ機能）
public class TextureCache
{
    private struct CacheKey
    {
        public Material material;
        public string propertyName;
        public int dataHash;
        
        public override bool Equals(object obj)
        {
            return obj is CacheKey other && 
                   material == other.material && 
                   propertyName == other.propertyName && 
                   dataHash == other.dataHash;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(material, propertyName, dataHash);
        }
    }
    
    private static readonly Dictionary<CacheKey, Texture2D> TextureCache = 
        new Dictionary<CacheKey, Texture2D>();
    
    public static Texture2D GetOrCreateTexture(Material material, string propertyName, 
                                              GradientTextureData data, Func<Texture2D> creator)
    {
        var key = new CacheKey 
        { 
            material = material, 
            propertyName = propertyName, 
            dataHash = data.GetHashCode() 
        };
        
        if (TextureCache.TryGetValue(key, out var cached) && cached != null)
        {
            return cached;
        }
        
        var newTexture = creator();
        if (newTexture != null)
        {
            TextureCache[key] = newTexture;
        }
        
        return newTexture;
    }
    
    [MenuItem("Tools/Uber Material/Clear Texture Cache")]
    public static void ClearCache()
    {
        foreach (var texture in TextureCache.Values)
        {
            if (texture != null) DestroyImmediate(texture);
        }
        TextureCache.Clear();
        Debug.Log("テクスチャキャッシュをクリアしました");
    }
}

// デバッグログ除去（コンディショナルコンパイル）
public static class UberDebug
{
    [System.Diagnostics.Conditional("UBER_DEBUG")]
    public static void Log(string message)
    {
        Debug.Log($"[Uber] {message}");
    }
    
    [System.Diagnostics.Conditional("UBER_DEBUG")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning($"[Uber] {message}");
    }
}
```