## 概要
重要度：55

UberDrawer がグループ状態管理、Drawer の選択/生成、描画の委譲、ログ制御を同時に担っており、SRP に反している。変更理由が複数に分散し、修正の影響範囲が広がる。

## 該当ファイル
- Assets/UberMaterialPropertyDrawer/Editor/UberDrawer.cs

### 関連ファイル
- Assets/UberMaterialPropertyDrawer/Editor/BeginGroupDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/EndGroupDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/BeginToggleGroupDrawer.cs
- Assets/UberMaterialPropertyDrawer/Editor/UberToggleDrawer.cs

## 内容
UberDrawer は以下の複数責務を抱えている。
- グループの展開状態・ネスト管理（GroupExpanded / GroupNest）
- Drawer の選択と生成（文字列による分岐）
- MaterialPropertyDrawer への描画委譲
- ログレベル管理（UberDrawerLogger）

これにより、グループ管理の仕様変更や Drawer 追加時に UberDrawer 本体を変更する必要があり、変更理由が複数存在する構造になっている。

### 不明点
- グループ管理や Drawer 登録を別コンポーネントへ分離する設計方針があるか。

## ゴール
- グループ状態管理、Drawer 生成/登録、描画委譲、ログ制御を分離し、それぞれ単一責務となる構成に整理する。

## 備考
- SRP（単一責任原則）の観点。
