## 概要
UberDrawer に集中している責務を分割し、グループ状態管理・Drawer 登録/生成・描画委譲・ログ制御を独立コンポーネントに分離する。変更理由を限定し、SRP を満たす構成に整理する。

## 関連ファイル
- issues/UberDrawerMultipleResponsibilities/issue.md

## 経緯
現在の UberDrawer は複数責務を同居させており、グループ管理の変更や Drawer 追加などの理由で同一クラスを編集する必要がある。責務分離によって変更影響を小さくし、保守性を高める。

## タスクリスト
- [ ] グループ状態管理の責務を切り出す方針を決める（例: `GroupStateStore` / `GroupStack`）
- [ ] `Assets/UberMaterialPropertyDrawer/Editor/UberDrawer.cs` から GroupExpanded/GroupNest 関連を新規クラスへ移す
- [ ] Drawer 生成ロジックをファクトリ/レジストリへ分離する方針を決める（別 issue の OCP 計画と整合）
- [ ] UberDrawer の描画委譲は単純なルーティングに限定する
- [ ] `UberDrawerLogger` を独立クラスに分離する（ファイル分割 or 同ファイル内の独立クラス化）
- [ ] 既存の public API（コンストラクタ・引数）と挙動が維持されることを Unity Editor で確認する

## 備考
- OCP 対応（Drawer 登録/生成）と併せて作業すると手戻りを減らせる。
- 追加クラスの配置は `Assets/UberMaterialPropertyDrawer/Editor/` 配下に統一する。
