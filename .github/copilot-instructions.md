# Custom Coding Instructions

## プロジェクト概要
このプロジェクトは .NET 9 を使用した C# プロジェクトで、以下のコンポーネントを含みます：
* PrimeService: 素数判定ライブラリ
* RazorPagesProject: ASP.NET Core Razor Pages Webアプリケーション
* 各種テストプロジェクト（Unit Tests, Integration Tests, E2E Tests, Playwright Tests）

## コーディング規約

### 1. 全般的な規約
* **言語バージョン**: C# 最新機能を活用（.NET 9 対応）
* **命名規則**: PascalCase（クラス、メソッド、プロパティ）、camelCase（ローカル変数、フィールド）
* **ファイル構成**: 1クラス1ファイルの原則
* **using文**: ファイルスコープのusingディレクティブを優先使用
* **null許容参照型**: 有効化し、適切に使用する

### 2. コード品質
* **可読性**: 意図が明確になるような変数名・メソッド名を使用
* **SOLID原則**: 特に単一責任の原則とオープン・クローズの原則を重視
* **DRY原則**: コードの重複を避ける
* **例外処理**: 適切な例外型を使用し、ユーザーフレンドリーなエラーメッセージを提供

### 3. テスト関連
* **テストフレームワーク**: xUnit を使用
* **テスト命名**: `MethodName_Scenario_ExpectedBehavior` 形式
* **AAA パターン**: Arrange, Act, Assert の構造を明確に分離
* **Theory と InlineData**: パラメータ化テストを積極的に活用
* **カバレッジ**: 主要なビジネスロジックは現実的な目標として80～90%を目指し、リソースとバランスを考慮する
* **待機機構**: Thread.Sleepの代わりに明示的な待機機構（Explicit Wait）を使用してテストの安定性を向上させる
  * ✅ 推奨: `await Task.Delay()`, `TaskCompletionSource`, `CancellationToken`による待機
  * ❌ 非推奨: `Thread.Sleep()`による固定時間待機

#### E2E テスト固有のガイドライン
* **セレクター戦略**: 安定性と保守性を重視したセレクター選択
  * ✅ **最優先**: `By.Id()` - HTMLのID属性（最も安定）
  * ✅ **次点**: `By.CssSelector("form#formId button[type='submit']")` - ID + 要素タイプの組み合わせ
  * ✅ **許可**: `By.XPath("//button[@type='submit' and contains(., 'テキスト')]")` - 属性 + テキスト内容
  * ❌ **非推奨**: 複雑なCSSセレクター、クラス名のみに依存、XPathでの階層指定
* **Page Object パターン**: 
  * 明示的な待機を使用した要素プロパティ: `CreateWait().Until(driver => driver.FindElement(By.Id("elementId")))`
  * 複雑なフォールバック機構は避け、シンプルで直接的なセレクターを使用
  * DOM要素の存在確認による待機条件（ページソースの文字列検索より安定）
* **要素操作**: 
  * クリックイベント: `((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element)` で相互作用の問題を回避
  * 要素の存在確認: `driver.FindElements(By.Id("elementId")).Any()` パターンを使用

### 4. ASP.NET Core / Razor Pages 固有
* **依存性注入**: コンストラクタインジェクションを基本とする
* **設定管理**: appsettings.json とオプションパターンを使用
* **セキュリティ**: ASP.NET Core Identity を適切に実装
* **HTTPクライアント**: HttpClientFactory パターンを使用
* **ページモデル**: ロジックはPage Modelに集約し、ViewからUIロジックを分離

### 5. データベース関連
* **Entity Framework Core**: Code First アプローチ
* **Migration**: 適切なマイグレーション命名とスクリプト管理
* **SQLite**: 開発環境での使用（本番環境では適切なDBを選択）

### 6. パフォーマンス
* **非同期プログラミング**: I/O操作は async/await を使用
* **メモリ効率**: IEnumerable<T> や Span<T> を適切に活用
* **文字列処理**: StringBuilder や string interpolation を適切に使い分け

### 7. セキュリティ
* **認証・認可**: ASP.NET Core Identity の機能をフル活用
* **HTTPS**: 本番環境では必須
* **XSS対策**: Razor Pages の自動エスケープ機能を活用
* **CSRF対策**: 適切なAntiForgeryToken の使用

### 8. ドキュメント
* **XML コメント**: パブリック API には適切な文書化
* **README**: 各プロジェクトの目的と使用方法を明記
* **CHANGELOG**: 主要な変更履歴を記録
* **コメント**: ソースコードを見れば明らかなことはコメントに書かない
  * ❌ 悪い例: `.AddViewLocalization() // View Localizationを追加`
  * ✅ 良い例: ビジネスロジックの意図や複雑なアルゴリズムの説明のみ

### 9. CI/CD
* **GitHub Actions**: 推奨CI/CDプラットフォーム
* **自動テスト**: プルリクエスト時の自動実行
* **コードカバレッジ**: レポートの自動生成と可視化

### 10. エラーハンドリング
* **例外の使い分け**: 
  * ArgumentException: 引数が無効な場合
  * InvalidOperationException: オブジェクトの状態が無効な場合
  * NotImplementedException: 未実装機能
* **ログ**: 構造化ログ（Serilog 推奨）
* **ユーザーエラー**: わかりやすいエラーメッセージとガイダンス

## プロジェクト固有のガイドライン

### PrimeService
* 数学的アルゴリズムは効率性を重視
* エッジケース（負数、0、1）の適切な処理
* 大きな数値に対するパフォーマンス考慮

### RazorPagesProject
* レスポンシブデザインの実装
* アクセシビリティの考慮
* SEO対応（適切なメタタグ、構造化データ）

#### 起動とテスト方法
**起動コマンド:**
```powershell
cd .\src\RazorPagesProject\
dotnet run
```

**テスト用URL:**
* メインページ: http://localhost:5016/
* GitHubプロフィールページ: http://localhost:5016/GitHubProfile
* HTTPS版: https://localhost:7072/

**注意事項:**
* 開発者証明書が信頼されていない場合は、HTTPSの警告が表示される場合があります
* アプリケーションの停止は `Ctrl+C` で行います
* SQLiteデータベースが自動的に初期化されます

### テストプロジェクト
* **Unit Tests**: ビジネスロジックの詳細テスト
* **Integration Tests**: API エンドポイントのテスト
* **E2E Tests**: ユーザーシナリオのテスト
  * IDベースのセレクターを最優先で使用
  * Page Object パターンでの明示的な待機実装
  * DOM要素の存在確認による安定した待機条件
  * JavaScriptベースの要素操作で相互作用の問題を回避

## 推奨ツールとライブラリ
* **フォーマッター**: EditorConfig の設定に従う
* **ログ**: Microsoft.Extensions.Logging または Serilog

## 禁止事項
* `var` の過度な使用（型が明確でない場合）
* マジックナンバーの使用
* グローバル変数の使用
* 未処理の例外の放置
* パスワードやシークレットのハードコード
* 自明な内容の不要なコメント（メソッド名や型名を繰り返すだけのコメント）
* テストでの`Thread.Sleep()`の使用（明示的な待機機構を使用すること）
* **E2E テストでの非推奨パターン**:
  * 複雑なフォールバック機構を持つセレクター（try-catch の多重ネスト）
  * ページソースの文字列検索による待機条件
  * クラス名のみに依存したセレクター
  * 階層的なXPathセレクター（DOM構造に依存）

## レビューガイドライン
* レビューコメントは必ず日本語と英語を併記すること
    * 例: `このコードは可読性が低いです。` / `This code is hard to read.`

## Coding Agent 固有のガイドライン
* **UI変更タスク**: 画面の変更を伴う場合は、以下の手順を必須とする
  1. 変更前のスクリーンショットを Playwright MCP Server で取得
  2. コード変更を実行
  3. 変更後のスクリーンショットを Playwright MCP Server で取得
  4. 取得したスクリーンショットをプルリクエストにコメントとして投稿
  5. 必要に応じて、異なるブラウザや画面サイズでの検証も実施
* **スクリーンショット命名規則**: `{feature-name}_before.png` / `{feature-name}_after.png` の形式を使用
* **検証対象**: デスクトップ（1920x1080）、タブレット（768x1024）、モバイル（375x667）の3つの画面サイズでの確認を推奨
* **UI変更時のスクリーンショット取得**: 画面の変更を伴うタスクでは、必ず Playwright MCP Server で変更箇所のスクリーンショットを取得してプルリクエストにコメントすること
  * 変更前後の比較スクリーンショットを含める
  * レスポンシブデザインの場合は、異なる画面サイズでのスクリーンショットも取得する
  * アクセシビリティに関わる変更の場合は、キーボードナビゲーションやスクリーンリーダー対応の確認も行う
