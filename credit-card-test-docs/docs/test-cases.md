# クレジットカード決済 テストケース設計（初版）

**注意:** 以下はCodexによるソース調査・テスト実装前の**設計案**です。対象フォームは候補であり、メソッド名、DBテーブル・列、実行可能性、既存テスト基盤は調査後に確定します。実施結果を捏造しないこと。

## テスト実行の前提
- 外部決済APIは必ずモック／スタブ化し、実通信を禁止する。
- DBは隔離されたテストDB、またはDBアクセス層のモックを使用する。本番DBへの接続・更新禁止。
- 決済API呼出し回数と引数、DBの事前・事後状態、履歴、画面表示を観測する。
- 同時実行・タイムアウト・クラッシュ・DB例外を注入できない場合は`BLOCKED_ENV`または`REVIEW_ONLY`とし、`PASS`にしない。
- 単体/結合/UIのテストレベルは実装調査後に付与する。
- **テストを通すために本番コード・受け入れ条件を書き換えない。**
- `REQ-CC-020`と`REQ-CC-033`の詳細仕様は未定。関連ケースを勝手に合格/不合格にしない。

## テストケース一覧

| テストID | 対応要件 | 検証内容 | 対象フォーム候補 | テストデータ/障害注入 | 期待結果 |
|---|---|---|---|---|---|
| TC-CC-001 | REQ-CC-001 | 締め時点の支払方法・締区分と決済時カード有効性を組合せ検証 | 500,513,700D | 締め時クレカ/月締→後から現金変更; 決済時カード有効/無効 | 有効時のみ対象、無効時API未呼出し |
| TC-CC-002 | REQ-CC-002 | 全額決済・部分決済不可 | 513,513B | 今回請求額=10000; 部分額=5000 | API金額10000、部分決済不可 |
| TC-CC-003 | REQ-CC-003 | API成功後DB保存例外 | 513B | 決済API成功、DB保存を例外化 | 再決済禁止、要管理者対応 |
| TC-CC-004 | REQ-CC-004, REQ-CC-010 | APIタイムアウト時の再実行抑止 | 513B,515 | 外部成功の可能性があるタイムアウト | 管理者復旧前の再決済API呼出し0 |
| TC-CC-005 | REQ-CC-005 | 取消条件と全額取消 | 513D | 成功/未決済/エラーあり、金額10000 | 成功・エラーなしのみ10000全額取消 |
| TC-CC-006 | REQ-CC-006 | 決済済み請求の変更・削除・締め直し禁止 | 500,501,502,503 | 決済済み・取消前 | 更新・削除・再締め不可 |
| TC-CC-007 | REQ-CC-007, REQ-CC-013, REQ-CC-031 | カード削除後の過去取引照会・取消 | 513D,515,700D | 決済後カード削除 | 履歴参照・過去取引番号で取消可 |
| TC-CC-008 | REQ-CC-008 | 入金日翌日 | 513B | 金曜、月末、12/31の決済 | 各決済日の翌暦日 |
| TC-CC-009 | REQ-CC-011 | 同時決済競合 | 513B | 同一請求を2端末から同時実行 | API実行は最大1件 |
| TC-CC-010 | REQ-CC-012, REQ-CC-018 | 取消後の締め直しと新番号 | 500,513,513D | 取消後に旧番号再実行、新番号作成 | 旧番号不可、新番号・最新マスタで実行可 |
| TC-CC-011 | REQ-CC-015 | 一括決済中の明確なAPI失敗 | 513B | 10件中5件目のみ明確な失敗 | 5件目失敗記録、6〜10件目続行 |
| TC-CC-012 | REQ-CC-017 | 取消時の入金削除・履歴保持 | 513D | 成功決済を全額取消 | 入金明細なし、成功と取消の両履歴あり |
| TC-CC-013 | REQ-CC-019 | 集計対象の除外 | 818,819 | 成功・取消済み・エラーを混在 | 有効成功分のみ集計 |
| TC-CC-014 | REQ-CC-021 | 取消APIタイムアウト時の禁止 | 513D | 取消結果不明を注入 | 再取消・再決済・請求変更禁止 |
| TC-CC-015 | REQ-CC-022 | TP/FRの独立性 | 509,510,511,512,512D,513 | TP請求とFR請求を用意 | 相互の請求データに誤作用しない |
| TC-CC-016 | REQ-CC-023 | 操作権限 | 513,513D,700D | 許可/不許可ユーザー | 不許可ユーザーは実行不可 |
| TC-CC-017 | REQ-CC-024 | 決済金額境界 | 513,513B | 下限-1、下限、0、-1 | 下限のみ条件充足なら可、それ以外API0回 |
| TC-CC-018 | REQ-CC-025 | 最新料率での手数料 | 818,819 | 決済時3%、表示時4% | 表示時4%を使用 |
| TC-CC-019 | REQ-CC-026 | 決済履歴と操作ログの分離 | 513B,513D,515 | 成功・失敗・取消・照会・管理者復旧 | 前3種は決済履歴、後2種は別操作ログ |
| TC-CC-020 | REQ-CC-027 | 結果不明後も一括決済続行 | 513B | 10件中5件目タイムアウト | 5件目ロック、6〜10件目続行 |
| TC-CC-021 | REQ-CC-028 | 結果不明解除の権限制御 | 513B,513D | 専用管理者/一般担当者 | 管理者のみ解除可 |
| TC-CC-022 | REQ-CC-030 | 結果不明と明確な失敗の表示区別 | 513B,513D | API失敗応答/タイムアウト | 異なる状態表示 |
| TC-CC-023 | REQ-CC-032 | 残存入金明細がある取消後の締め直し | 500,513D | 取消後に入金明細を意図的に残す | 締め直し自体は許可、整合性リスクを報告 |
| TC-CC-024 | REQ-CC-020 | 強制終了後の復旧方式（仕様未定） | 513B | 処理中に強制終了 | BLOCKED_SPEC：復旧方式は合否判定しない |
| TC-CC-025 | REQ-CC-033 | 取消API成功・DB保存失敗の復旧方式（仕様未定） | 513D | 取消API成功後DB例外 | BLOCKED_SPEC：具体的復旧方式は合否判定しない |

## ソース調査後に追記する項目（各テストIDごと）
- 対象クラス・メソッド・ファイルパス・行番号
- 参照/更新DBテーブル・列とトランザクション境界
- 外部APIメソッド、リクエストの一意キー、重複防止仕様
- テスト種別（単体/結合/UI/静的レビュー）
- 実行コマンド、テストコードパス、実行環境
- 実測結果、PASS/FAIL/BLOCKED_ENV/BLOCKED_SPEC/REVIEW_ONLY/NOT_RUN、証跡

## 重点リスク（未検証の不具合候補）
| リスクID | 内容 | 関連テスト |
|---|---|---|
| BUG-CC-001 | 決済通信断後、未決済・エラーなしで再決済可能 | TC-CC-004 |
| BUG-CC-002 | 同時決済の排他不足 | TC-CC-009 |
| BUG-CC-003 | 取消後に同じ請求番号で再決済可能 | TC-CC-010 |
| BUG-CC-004 | 強制終了後、未完了決済が検知されない | TC-CC-024、TC-CC-004 |
| BUG-CC-005 | 取消通信断後、再取消可能 | TC-CC-014 |
| BUG-CC-006 | 残存入金明細による二重計上・不整合 | TC-CC-023 |

`BUG`は現時点では調査IDであり、再現済みバグを意味しない。REQ-CC-032により残存明細があっても締め直し自体は許可されるため、**締め直しが成功したことだけを不具合扱いしない**。

## Codexへの作業順序
1. 対象20フォームと関連サービス、DBアクセス、外部APIを調査する。
2. 要件→受け入れ条件→テストID→クラス/メソッドのトレーサビリティを補完する。
3. 実行可能なテスト基盤と必要な追加パッケージ・ファイルを報告する。
4. モック/隔離DBでテストを実装・実行し、証跡付きで結果を記録する。
5. 仕様不明・環境不足・既存コードの欠陥を区別して報告する。

## 実装への対応（main 8de0c44）

以下のファイルはすべてリポジトリの `EmuLunch2/` 配下。フォーム名と同名の .cs を参照。動的検証の観測点は外部スタブの要求履歴、独立接続から見たコミット済DB、画面再表示、および別プロセスからの操作とする。

| TC-CC | AC-CC | 対象ファイル・メソッド | 未実行の検証で観測する箇所 |
|---|---|---|---|
| 001 | 001 | Form500.btnExec_Click、SeikyuShimeExec.CreateTotalTemp、Form513.btnQuery_Click / btnExec_Click、Form513B.SettleExec、Form700D.btnCardDelete_Click | 請求合計.FR支払フラグ、締区分、得意先マスタ.クレカ無効フラグ。検索後に無効化したケースも作る |
| 002 | 002 | Form513.btnExec_Click、Form513B.SettleExec、WebAPI/FRSettlement.SaleSettlement | スタブの billingAmount と画面からの部分額指定可否 |
| 003 | 003 | Form513B.SettleExec / Save / ConsistencyExec | API成功後に履歴・請求・入金の保存を順に失敗させ、再起動後の再送回数と管理者対応状態を確認 |
| 004 | 004 | Form513B.SettleExec / ConsistencyExec、Form515.InquiryExec、WebAPI/WebAPIBase.CallMainFR | スタブ側では成功、クライアントには切断またはタイムアウト。照会前後の再送回数と状態 |
| 005 | 005 | Form513D.Init / CancelExec、WebAPI/FRSettlement.SaleCancel | 完了・エラー・取引番号と取消要求。取消金額はAPI側の契約で確認 |
| 006 | 006 | Form500.btnExec_Click、Form501.CanDeleteFR / Delete、Form502.CanSave / Save、SeikyuShimeExec.GetExcludeSeikyuCDList / GetDeleteTargetSQL | 請求合計・請求ヘッダ。Form503.GetTarget は印刷抽出、Form506.GetGassanSeikyusyo は合算請求書取得 |
| 007 | 007 | Form700.ShowDetail、Form700D.btnCardDelete_Click / CallInfoDeleteCard、Form513D.GetSettlementHistory / CancelExec、Form515.InquiryExec、Form819.btnQuery_Click | カード削除後のローカル履歴と元取引番号、利用明細 |
| 008 | 008 | Form513B.Save | 入金明細.入金日、処理が深夜を跨ぐ場合は決済日と比較 |
| 009 | 009 | Form513B.SettleExec / Save、Data/SeikyuGokei.GetFRRecord | 2プロセスの読取をAPIスタブで同期待機させ、同一請求への要求数を集計 |
| 010 | 010 | Form513D.Save、Form513.btnExec_Click、Form500.btnExec_Click、SeikyuShimeExec.CreateTotalTemp / CreateTotal、Form508.SaveSeikyuGokei | 旧請求への要求回数、新請求番号と最新マスタによるスナップショット |
| 011 | 011 | Form513B.SettleExec / Save、WebAPI/FRSettlement.SaleSettlement | 5件目の応答ヘッダー失敗と業務結果失敗を分け、履歴と6〜10件目を観測 |
| 012 | 012 | Form513D.Save | FR_T_クレカ決済履歴に元・取消両行、入金明細の対象行削除 |
| 013 | 013 | Form818.btnQuery_Click、Form819.btnQuery_Click | 請求合計の完了フラグ・エラー区分を含む集計フィルター |
| 014 | 014 | Form513D.CancelExec / Save / ConsistencyExec、Form501.CanDeleteFR、SeikyuShimeExec.GetDeleteTargetSQL | 取消送信後に応答を失い、別セッションから再取消・再決済・請求変更を試行 |
| 015 | 015 | Form509.btnExec_Click、PaySeikyuShimeExec.Run、Form510.btnQuery_Click / Delete、Form511.Form511_Load、Form512.btnQuery_Click / ShowDetail、Form512D.GetNyukin、Form513.btnQuery_Click | TP側の電子決済請求データとFR側の請求合計を同時に用意し、各更新と参照を比較 |
| 016 | 016 | Form500.btnExec_Click、Form514.btnExec_Click、Form513.btnExec_Click / ShowDetail、Form513B.Form513B_Load / btnExec_Click、Form513D.Form513D_Load / Init / btnExec_Click、Form700D.btnCardDelete_Click | 許可・不許可で通常遷移と直接操作の両方を試行 |
| 017 | 017 | Form513.btnExec_Click / LockCriteria、Form513B.SettleExec | 現行下限100円。99・100・0・−1円の要求回数 |
| 018 | 018 | Form818.btnQuery_Click / GetFeeRate、WebAPI/FRSettlement.InfoGetFeeRateList、Form819.btnQuery_Click | 画面表示で最新4%と旧3%を識別。料率適用箇所はForm818 |
| 019 | 019 | Form513B.SettleExec / Save、Form513D.CancelExec / Save、Form515.InquiryExec | 決済履歴、操作ログの書込み、誰がいつ照会・解除したか |
| 020 | 020 | Form513B.SettleExec / ShowProgress / ConsistencyExec | 5件目タイムアウトで後続が進み、5件目のロックが再起動後も有効 |
| 021 | 021 | Form513B.ConsistencyExec、Form513D.ConsistencyExec、Form515.InquiryExec | 専用解除経路の実装位置は未確認。一般権限から書き換え不可を確認 |
| 022 | 022 | Form513.btnQuery_Click、Form513B.SettleExec、Form513D.Init / btnExec_Click | 実行中の注記・ダイアログ、再読込後の永続状態 |
| 023 | 023 | Form500.btnExec_Click、SeikyuShimeExec.GetDeleteTargetSQL / CreateTotalTemp、Form513D.Save | 入金明細残存時の締め直し可否と金額・入金紐付けをそれぞれ観測 |
| 024 | — | Form513B.SettleExec / Save | API送信直後・保存直前にプロセス終了。復旧方式はBLOCKED_SPEC |
| 025 | — | Form513D.CancelExec / Save / ConsistencyExec | 取消成功後にDELETEまたはCOMMITを失敗させる。具体的復旧方式はBLOCKED_SPEC |

決済・取消では `GetFRRecord` で請求を読み、APIを呼んでから `Save` でSQLトランザクションを開始する。履歴の採番や請求ヘッダ取得には別のDB呼び出しがある。取消要求は元履歴の `seqNo`、決済要求は `billingNumber` を含む。クライアントから確認できる範囲では、これらの値に対する外部サービスの冪等性保証は確認できない。

## 追加したアダプター単体テスト

実装ファイルは `tests/FRAdapter`。実際の `EmuLunch2/WebAPI/FRSettlement.cs` をコンパイルし、通信基底クラスとデータ型をテスト用に置き換える。実機のシリアライザー、HTTP、フォーム、SQL Serverは対象外。リポジトリルートから `dotnet run --project credit-card-test-docs/tests/FRAdapter` で実行する。追加NuGetパッケージなし。

| ID | 要件 | 対象 | スタブ入力と期待結果 |
|---|---|---|---|
| TC-CC-026 | REQ-CC-002,024 | FRSettlement.SaleSettlement | 10000円・100円の請求で各1回、請求番号と全額を送信し取引番号を保持 |
| TC-CC-027 | REQ-CC-005,013 | FRSettlement.SaleCancel | カード情報を持たない合成請求で元取引番号を送信し取消取引番号を取得 |
| TC-CC-028 | REQ-CC-015,030 | FRSettlement.SaleSettlement | 明示失敗ヘッダーでfalseとエラーメッセージを返す |
| TC-CC-029 | REQ-CC-004,010,027 | FRSettlement.SaleSettlement | 注入したタイムアウトが呼出元に届き、アダプター内の送信試行は1回 |
| TC-CC-030 | REQ-CC-021 | FRSettlement.SaleCancel | 注入したタイムアウトが呼出元に届き、アダプター内の送信試行は1回 |
| TC-CC-031 | REQ-CC-025 | FRSettlement.InfoGetFeeRateList | レスポンスの4%を料率モデルに反映 |

## 追加の障害シナリオ

| ID | 要件 | 対象 | 注入と期待結果 | 判定 |
|---|---|---|---|---|
| TC-CC-032 | REQ-CC-005,021 | Form513D.CancelExec / Save | 同じ成功取引の取消画面を2端末で開き、最初の取消の応答受領前に2台目から操作。結果不明を記録した後は再取消0回。応答受領前のAPI側重複排除は仕様照会 | BLOCKED_ENV |
| TC-CC-033 | REQ-CC-009 | Form513.btnQuery_Click / btnExec_Click、Form513B.SettleExec | 検索後にテストマスタのカードを無効化してから決済。API呼出し0回 | BLOCKED_ENV |

## フォームを起動しない処理境界テスト

`tests/FRWorkflow` は本番の `Form513B.SettleExec`、`Form513B.ConsistencyExec`、`Form513D.CancelExec` の本文をビルド時に直接取り込む。API・DB・画面・履歴はテストダブル。対象メソッド、入力、期待結果、未検証範囲は [TC-CC-034〜042の実行記録](test-results/2026-11-18-run-002.md) を参照。元シナリオとの対応と解除条件は [環境再評価](analysis/test-environment-reassessment.md) に記録。
