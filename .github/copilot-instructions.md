# Custom Coding Instructions

## プロジェクト概要
このプロジェクトは .NET 9 を使用した C# プロジェクトで、以下のコンポーネントを含みます：
- PrimeService: 素数判定ライブラリ
- RazorPagesProject: ASP.NET Core Razor Pages Webアプリケーション
- 各種テストプロジェクト（Unit Tests, Integration Tests, E2E Tests, Playwright Tests）

## コーディング規約

### 1. 全般的な規約
- **言語バージョン**: C# 最新機能を活用（.NET 9 対応）
- **命名規則**: PascalCase（クラス、メソッド、プロパティ）、camelCase（ローカル変数、フィールド）
- **ファイル構成**: 1クラス1ファイルの原則
- **using文**: ファイルスコープのusingディレクティブを優先使用
- **null許容参照型**: 有効化し、適切に使用する

### 2. コード品質
- **可読性**: 意図が明確になるような変数名・メソッド名を使用
- **SOLID原則**: 特に単一責任の原則とオープン・クローズの原則を重視
- **DRY原則**: コードの重複を避ける
- **例外処理**: 適切な例外型を使用し、ユーザーフレンドリーなエラーメッセージを提供

### 3. テスト関連
- **テストフレームワーク**: xUnit を使用
- **テスト命名**: `MethodName_Scenario_ExpectedBehavior` 形式
- **AAA パターン**: Arrange, Act, Assert の構造を明確に分離
- **Theory と InlineData**: パラメータ化テストを積極的に活用
- **カバレッジ**: 主要なビジネスロジックは100%を目指す

### 4. ASP.NET Core / Razor Pages 固有
- **依存性注入**: コンストラクタインジェクションを基本とする
- **設定管理**: appsettings.json とオプションパターンを使用
- **セキュリティ**: ASP.NET Core Identity を適切に実装
- **HTTPクライアント**: HttpClientFactory パターンを使用
- **ページモデル**: ロジックはPage Modelに集約し、ViewからUIロジックを分離

### 5. データベース関連
- **Entity Framework Core**: Code First アプローチ
- **Migration**: 適切なマイグレーション命名とスクリプト管理
- **SQLite**: 開発環境での使用（本番環境では適切なDBを選択）

### 6. パフォーマンス
- **非同期プログラミング**: I/O操作は async/await を使用
- **メモリ効率**: IEnumerable<T> や Span<T> を適切に活用
- **文字列処理**: StringBuilder や string interpolation を適切に使い分け

### 7. セキュリティ
- **認証・認可**: ASP.NET Core Identity の機能をフル活用
- **HTTPS**: 本番環境では必須
- **XSS対策**: Razor Pages の自動エスケープ機能を活用
- **CSRF対策**: 適切なAntiForgeryToken の使用

### 8. ドキュメント
- **XML コメント**: パブリック API には適切な文書化
- **README**: 各プロジェクトの目的と使用方法を明記
- **CHANGELOG**: 主要な変更履歴を記録

### 9. CI/CD
- **GitHub Actions**: 推奨CI/CDプラットフォーム
- **自動テスト**: プルリクエスト時の自動実行
- **コードカバレッジ**: レポートの自動生成と可視化

### 10. エラーハンドリング
- **例外の使い分け**: 
  - ArgumentException: 引数が無効な場合
  - InvalidOperationException: オブジェクトの状態が無効な場合
  - NotImplementedException: 未実装機能
- **ログ**: 構造化ログ（Serilog 推奨）
- **ユーザーエラー**: わかりやすいエラーメッセージとガイダンス

## プロジェクト固有のガイドライン

### PrimeService
- 数学的アルゴリズムは効率性を重視
- エッジケース（負数、0、1）の適切な処理
- 大きな数値に対するパフォーマンス考慮

### RazorPagesProject
- レスポンシブデザインの実装
- アクセシビリティの考慮
- SEO対応（適切なメタタグ、構造化データ）

### テストプロジェクト
- Unit Tests: ビジネスロジックの詳細テスト
- Integration Tests: API エンドポイントのテスト
- E2E Tests: ユーザーシナリオのテスト

## 推奨ツールとライブラリ
- **フォーマッター**: EditorConfig の設定に従う
- **ログ**: Microsoft.Extensions.Logging または Serilog

## 禁止事項
- `var` の過度な使用（型が明確でない場合）
- マジックナンバーの使用
- グローバル変数の使用
- 未処理の例外の放置
- パスワードやシークレットのハードコード

## レビューガイドライン
- 必ず日本語でレビューコメントを記載
