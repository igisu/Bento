using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 売掛分配入力
    /// </summary>
    public partial class Form406 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int INIT_VALUE_ROW = 0;       // 分配元の初期値を持つ行
        private const int FORMULA_ROW = 1;          // 分配元数式を持つ行
        private const int PARENT_ROW = 2;           // 分配元を表す行
        private const int CHILDREN_ROW = 3;         // 分配先開始行
        private const int DATASOURCE_CHILDREN_ROW = (CHILDREN_ROW - 1); // データソースの分配先開始行

        private const int TOKUISAKI_CD_COLUMN = 0;      // 得意先CD列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 1;    // 得意先名列インデックス
        // これ以降の列は動的に追加される
        private const int SYOHIN_START_COLUMN = 2;  // 商品名列開始列インデックス

        // これより下の定数は(■)の数と連動する
        private const int DATASOURCE_LINE_KBN_COLUMN = 0;               // データソースの行区分列インデックス
        private const int DATASOURCE_TOKUISAKI_CD_COLUMN = 1;           // データソースの得意先CD列インデックス
        private const int DATASOURCE_TOKUISAKI_CD_WITH_KBN_COLUMN = 2;  // データソースの売上区分付き得意先CD列インデックス
        private const int DATASOURCE_TOKUISAKI_NAME_COLUMN = 3;         // データソースの列得意先名インデックス
        private const int DATASOURCE_SYOHIN_START_COLUMN = 4;           // データソースの列商品名列開始列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 売掛分配検索条件
        /// </summary>
        private UrikakeBunpaiCriteria _urikakeBunpaiCriteria;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = SYOHIN_START_COLUMN;

        /// <summary>
        /// （売上日時点の）消費税
        /// </summary>
        private Syohizei _syohizei;

        /// <summary>
        /// 管理マスタ（チケット回収時売上フラグ）
        /// </summary>
        private Kanri _kanri;

        /// <summary>
        /// 列として採用する商品名の一覧（商品ＣＤ、商品名略称）を持つデータテーブル
        /// </summary>
        private DataTable _syohinDt = null;

        /// <summary>
        /// 更新対象の分配元・分配先の売上明細を持つデータテーブル
        /// </summary>
        private DataTable _uriageMeisaiDt = null;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// スプレッドのコンボボックス（F2,Enter制御用）
        /// </summary>
        private FarPoint.Win.FpCombo _fpCombo;

        /// <summary>
        /// スプレッドのコンボボックスでEnterキーが押下されたか
        /// </summary>
        private bool _isEnterforFpCombo = false;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="urikakeBunpaiCriteria"></param>
        public Form406(UrikakeBunpaiCriteria urikakeBunpaiCriteria)
        {
            try
            {
                InitializeComponent();

                _urikakeBunpaiCriteria = urikakeBunpaiCriteria;
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form406_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.calculator_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN;

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread1.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 先頭2列を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = 2;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                urikakeBunpaiCriteriaBindingSource.DataSource = _urikakeBunpaiCriteria;

                // 消費税を取得
                _syohizei = Syohizei.GetSyohizei((DateTime)_urikakeBunpaiCriteria.売上日);

                // 管理マスタ取得
                _kanri = Kanri.GetKanri();

                // 売掛分配を取得
                btnQuery.PerformClick();
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form406_Shown(object sender, EventArgs e)
        {
            // レコードを表示していればフォーカスする
            if (fpSpread1_Sheet1.Rows.Count > 0)
            {
                fpSpread1.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form406_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F8:
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread1.EditMode == true)
                    {
                        fpSpread1.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    else
                    {
                        WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                    }
                    btnMeisai.PerformClick();       // 明細入力ボタン押下
                    break;

                case Keys.F9:
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread1.EditMode == true)
                    {
                        fpSpread1.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    else
                    {
                        WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                    }
                    btnSave.PerformClick();         // 更新ボタン押下
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form406_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // このフォーム以外の要因で閉じられようとしている場合は閉じる
                if (e.CloseReason != CloseReason.UserClosing)
                {
                    return;
                }

                // 登録or削除済み？
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                // フォームが閉じられようとするとき、スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                if (fpSpread1.EditMode == true)
                {
                    fpSpread1.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                else
                {
                    WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                }

                // 初期状態と異なる？
                if (HasChange())
                {
                    DialogResult r = MessageBox.Show("変更されています。更新しますか？", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            if (CanSave() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (Save())
                            {
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            }
                            else
                            {
                                e.Cancel = true;
                            }
                            return;

                        case DialogResult.Cancel:
                            e.Cancel = true;
                            return;
                    }
                }
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 再表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                //*************************************************************************************************************************
                // 分配元の売上明細に同一売上区分、同一商品が複数存在する場合はエラーとする。
                // この場合、商品単価が異なっているのが考えられる。分配先にはどちらの単価を適用すればよい？
                // 分配入力画面に単価の列を追加すればよい？→単価が同じ場合（商品も売上区分も同じなので）分配元の売上明細を特定するのが困難
                // →以上の理由により（そもそもレアケースというものあるが）
                //   分配元に同一売上区分（売掛）、同一商品が複数存在する場合は、メッセージを表示して分配入力できないようにする。
                //*************************************************************************************************************************
                if (HasDuplicate())
                {
                    MessageBox.Show(this, "分配元に同一商品・同一支払区分の売上明細が複数存在します。分配できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //*********************************************************
                // 分配元の売上詳細にある商品名を列として追加
                // 分配元の得意先を受注得意先として持つ得意先を行とする
                // データそのものは売上明細から取得する
                //*********************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_syohinDt, "商品名略称");

                // 更新対象の分配元・分配先の売上明細を持つデータテーブルを取得する
                _uriageMeisaiDt = GetUriageMeisai();

                // データ本体を取得
                DataTable dt = new DataTable("売掛分配入力データ");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // -- 商品ＣＤのアンダーバー以降は
                    // -- 0:個数 1:金額
                    cmd.CommandText = // -- 分配元_商品数量（初期値）
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 行区分" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, M.得意先ＣＤ) AS 売上区分付き得意先ＣＤ" +
                                      "\r\n     ,'分配元初期値' AS 得意先名" +
                                      "\r\n     ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n     ,M.売上区分" +
                                      "\r\n     ,ISNULL(M.商品数量, 0) AS 値" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS M" +
                                      "\r\n WHERE" +
                                      "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M.売上日   = @売上日" +
                                      "\r\n     AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND M.売上区分 = @売上区分売掛" +
                                      "\r\n UNION" +
                                      // -- 分配元_分配元数量（初期値）
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 行区分" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, M.得意先ＣＤ) AS 売上区分付き得意先ＣＤ" +
                                      "\r\n     ,'分配元初期値' AS 得意先名" +
                                      "\r\n     ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n     ,M.売上区分" +
                                      "\r\n     ,ISNULL(M.分配元数量, 0) AS 値" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS M" +
                                      "\r\n WHERE" +
                                      "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M.売上日   = @売上日" +
                                      "\r\n     AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND M.売上区分 = @売上区分売掛" +
                                      "\r\n UNION" +
                                      // -- 分配元数式行
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 行区分" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, M.得意先ＣＤ) AS 売上区分付き得意先ＣＤ" +
                                      "\r\n     ,'分配元数式' AS 得意先名" +
                                      "\r\n     ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n     ,M.売上区分" +
                                      "\r\n     ,NULL AS 値" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS M" +
                                      "\r\n WHERE" +
                                      "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M.売上日   = @売上日" +
                                      "\r\n     AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND M.売上区分 = @売上区分売掛" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 行区分" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, M.得意先ＣＤ) AS 売上区分付き得意先ＣＤ" +
                                      "\r\n     ,'分配元数式' AS 得意先名" +
                                      "\r\n     ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n     ,M.売上区分" +
                                      "\r\n     ,NULL AS 値" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS M" +
                                      "\r\n WHERE" +
                                      "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M.売上日   = @売上日" +
                                      "\r\n     AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND M.売上区分 = @売上区分売掛" +
                                      "\r\n UNION" +
                                      // -- 分配先個数
                                      "\r\n SELECT" +
                                      "\r\n     3 AS 行区分" +
                                      "\r\n     ,T.得意先ＣＤ" +
                                      "\r\n     ,A.売上区分付き得意先ＣＤ" +
                                      "\r\n     ,T.得意先名" +
                                      "\r\n     ,A.商品ＣＤ" +
                                      "\r\n     ,A.売上区分" +
                                      "\r\n     ,A.個数 AS 値" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         得意先ＣＤ" +
                                      "\r\n         ,得意先名" +
                                      "\r\n     FROM" +
                                      "\r\n         得意先マスタ" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND 受注得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     ) AS T" +
                                      "\r\n     LEFT JOIN (" +
                                      "\r\n         SELECT" +
                                      "\r\n             M.得意先ＣＤ" +
                                      "\r\n             ,CONVERT(varchar, M.得意先ＣＤ) + '_' + CONVERT(varchar, M.売上区分)  AS 売上区分付き得意先ＣＤ" +
                                      "\r\n             ,M.売上区分" +
                                      "\r\n             ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n             ,SUM(M.商品数量) AS 個数" +
                                      "\r\n         FROM" +
                                      "\r\n             売上明細 AS M" +
                                      "\r\n         WHERE" +
                                      "\r\n             M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND M.売上日   = @売上日" +
                                      "\r\n             AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             M.得意先ＣＤ" +
                                      "\r\n             ,M.売上区分" +
                                      "\r\n             ,M.商品ＣＤ" +
                                      "\r\n         ) AS A ON A.得意先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n UNION" +
                                      // -- 分配先売上区分
                                      "\r\n SELECT" +
                                      "\r\n     3 AS 行区分" +
                                      "\r\n     ,T.得意先ＣＤ" +
                                      "\r\n     ,A.売上区分付き得意先ＣＤ" +
                                      "\r\n     ,T.得意先名" +
                                      "\r\n     ,A.商品ＣＤ" +
                                      "\r\n     ,A.売上区分" +
                                      "\r\n     ,A.売上区分 AS 値" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         得意先ＣＤ" +
                                      "\r\n         ,得意先名" +
                                      "\r\n     FROM" +
                                      "\r\n         得意先マスタ" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND 受注得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     ) AS T" +
                                      "\r\n     LEFT JOIN (" +
                                      "\r\n         SELECT DISTINCT" +
                                      "\r\n             M.得意先ＣＤ" +
                                      "\r\n             ,CONVERT(varchar, M.得意先ＣＤ) + '_' + CONVERT(varchar, M.売上区分)  AS 売上区分付き得意先ＣＤ" +
                                      "\r\n             ,M.売上区分" +
                                      "\r\n             ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             売上明細 AS M" +
                                      "\r\n         WHERE" +
                                      "\r\n             M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND M.売上日   = @売上日" +
                                      "\r\n             AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n         ) AS A ON A.得意先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     行区分" +
                                      "\r\n     ,得意先ＣＤ" +
                                      "\r\n     ,売上区分付き得意先ＣＤ" +
                                      "\r\n     ,商品ＣＤ" +
                                      "\r\n     ,売上区分";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _urikakeBunpaiCriteria.売上日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("売掛分配入力を検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("売掛分配入力");
                // 固定列を作成(■)
                cldt.AddFixedColumn(dt.Columns["行区分"]);
                cldt.AddFixedColumn(dt.Columns["得意先ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["売上区分付き得意先ＣＤ"]);      // 同一商品でも売上区分が異なっていれば別のレコードとして表示したいので
                cldt.AddFixedColumn(dt.Columns["得意先名"]);
                // 商品名列を作成
                cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // インポート
                cldt.Import(dt, "商品ＣＤ", "値");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);
                cldt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                fpSpread1.SuspendLayout();      // 描画を一時停止

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "売掛分配入力";

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");

                // 商品名を列名としてバインド
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // レコードが無い場合はこれ以上何もしない
                if (fpSpread1_Sheet1.Rows.Count <= 0)
                {
                    return;
                }

                // ┌─┬───────┬───────────────────────┐
                // │△│              │                    商品Ａ                    │
                // ├─┼───────┼───────┬───────────────┤
                // │△│              │     個数     │           売上区分           │
                // ├─┼───────┼───────┼───────────────┤
                // │★│分配元の初期値│数量【A】     │分配元数量【B】               │
                // ├─┼───────┼───────┼───────────────┤
                // │★│分配元数式    │=(A+B)-D【C】 │=sum(商品Ａの分配先個数)【D】 │
                // ├─┼───────┼───────┴───────────────┤
                // │◇│（分配元）    │=C(D)【E】                                    │
                // ├─┼───────┼───────┬───────────────┤
                // │  │分配先得意先Ｘ│2             │売掛                          │
                // ├─┼───────┼───────┼───────────────┤
                // │  │分配先得意先Ｙ│1             │売掛                          │
                // ├─┼───────┼───────┼───────────────┤
                // │  │分配先得意先Ｚ│1             │売掛                          │
                // └─┴───────┴───────┴───────────────┘
                // △はヘッダ行
                // ★は非表示の行
                // ◇は動的に追加する行
                // ★と◇は固定行

                //*******************************************************************************************
                // 分配元を表示する行を非連結行として追加（セルに表示する型が異なるのでこうしないといけない）
                //*******************************************************************************************
                fpSpread1_Sheet1.AddUnboundRows(PARENT_ROW, 1);
                fpSpread1_Sheet1.Cells[PARENT_ROW, TOKUISAKI_NAME_COLUMN].Text = "（分配元）";

                // 先頭の３行について、以下の設定をする
                // ・商品ごとの列を結合する
                // ・数式を設定する
                // ・固定行にする
                // ・非表示にする
                fpSpread1_Sheet1.FrozenRowCount = PARENT_ROW + 1;
                fpSpread1_Sheet1.Rows[INIT_VALUE_ROW].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[INIT_VALUE_ROW].CanFocus = false;
                fpSpread1_Sheet1.Rows[FORMULA_ROW].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[FORMULA_ROW].CanFocus = false;
                fpSpread1_Sheet1.Rows[PARENT_ROW].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[PARENT_ROW].CanFocus = false;

                int columnMax = fpSpread1_Sheet1.Columns.Count - 1;
                for (columnIndex = SYOHIN_START_COLUMN; columnIndex < columnMax; )
                {
                    // 先頭３行について、セルタイプを解除
                    //FarPoint.Win.Spread.CellType.GeneralCellType generalCellType = new FarPoint.Win.Spread.CellType.GeneralCellType();
                    fpSpread1_Sheet1.Cells[INIT_VALUE_ROW, columnIndex].CellType = new FarPoint.Win.Spread.CellType.GeneralCellType();
                    fpSpread1_Sheet1.Cells[INIT_VALUE_ROW, columnIndex + 1].CellType = new FarPoint.Win.Spread.CellType.GeneralCellType();
                    fpSpread1_Sheet1.Cells[FORMULA_ROW, columnIndex].CellType = new FarPoint.Win.Spread.CellType.GeneralCellType();
                    fpSpread1_Sheet1.Cells[FORMULA_ROW, columnIndex + 1].CellType = new FarPoint.Win.Spread.CellType.GeneralCellType();
                    fpSpread1_Sheet1.Cells[PARENT_ROW, columnIndex].CellType = new FarPoint.Win.Spread.CellType.GeneralCellType();
                    fpSpread1_Sheet1.Cells[PARENT_ROW, columnIndex + 1].CellType = new FarPoint.Win.Spread.CellType.GeneralCellType();
                    fpSpread1_Sheet1.Cells[PARENT_ROW, columnIndex].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;

                    // 数式行に数式を設定
                    FarPoint.Win.Spread.Cell initKosuCell = fpSpread1_Sheet1.Cells[INIT_VALUE_ROW, columnIndex];        // 数量の初期値セル【A】
                    FarPoint.Win.Spread.Cell initBunpaiCell = fpSpread1_Sheet1.Cells[INIT_VALUE_ROW, columnIndex + 1];  // 分配元数量の初期値セル【B】
                    FarPoint.Win.Spread.Cell formulaKosuCell = fpSpread1_Sheet1.Cells[FORMULA_ROW, columnIndex];        // 数量の数式セル【C】
                    FarPoint.Win.Spread.Cell formulaBunpaiCell = fpSpread1_Sheet1.Cells[FORMULA_ROW, columnIndex + 1];  // 分配元数量の数式セル【D】
                    FarPoint.Win.Spread.Cell parentKosuCell = fpSpread1_Sheet1.Cells[PARENT_ROW, columnIndex];          // 数量(分配元数量)【E】

                    _isDtColumnChanging = true;         // データソース列値変更イベントを抑止する

                    // 【C】A+B-D
                    formulaKosuCell.Formula = string.Format("{0}+{1}-{2}", initKosuCell.ToString(), initBunpaiCell.ToString(), formulaBunpaiCell.ToString());

                    // 分配先のレコードがある？
                    if (fpSpread1_Sheet1.Rows.Count > CHILDREN_ROW)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[CHILDREN_ROW, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];

                        // 【D】sum(商品Ａの分配先個数)
                        formulaBunpaiCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());

                        // 【E】C(D)
                        parentKosuCell.Formula = string.Format("{0} & \"(\" & {1} & \")\"", formulaKosuCell.ToString(), formulaBunpaiCell.ToString());
                    }

                    // PARENT_ROWのセルを結合する
                    fpSpread1_Sheet1.AddSpanCell(PARENT_ROW, columnIndex, 1, 2);

                    columnIndex += 2;
                }

                // 分配元の初期値行（1行目）と分配元数式行（2行目）を非表示にする
                fpSpread1_Sheet1.Rows[INIT_VALUE_ROW].Visible = false;
                fpSpread1_Sheet1.Rows[FORMULA_ROW].Visible = false;

                if (fpSpread1_Sheet1.Rows.Count > CHILDREN_ROW)    // ３行までは合計行なので
                {
                    fpSpread1.Focus();

                    // カレント行(bindingSource1.PositionとfpSpread1_Sheet1.ActiveRowIndex)がずれることがあるので、
                    // それを明示的に指定する
                    fpSpread1_Sheet1.ActiveRowIndex = fpSpread1_Sheet1.Rows.Count - 1;
                    bindingSource1.Position = fpSpread1_Sheet1.Rows.Count - 1;

                    // 非表示の行があることに注意
                    fpSpread1_Sheet1.ActiveRowIndex = DATASOURCE_CHILDREN_ROW;
                    bindingSource1.Position = CHILDREN_ROW;
                    fpSpread1_Sheet1.SetActiveCell(CHILDREN_ROW, SYOHIN_START_COLUMN);
                }
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isDtColumnChanging = false;
                fpSpread1.ResumeLayout(true);       // 描画再開
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                //fpSpread1.SuspendLayout();      // 商品ＣＤを入力しても単価が描画されないことがあるのでそれを回避する

                _isDtColumnChanging = true;

                // 個数の列？
                if (IsKosuColumnInDataSource(e.Column.Ordinal))
                {
                    switch (e.Row["行区分"] as int?)
                    {
                        case FORMULA_ROW:       // 分配元数式を持つ行
                            SetErrorKosuOver(); // 分配先数量の合計が、分配元の初期値（数量＋分配元数量）を超える場合はエラー表示する
                            break;

                        case CHILDREN_ROW:      // 分配先
                            int kosuColumnIndex = e.Column.Ordinal;     // 個数列インデックス
                            int kbnColumnIndex = kosuColumnIndex + 1;   // 売上区分列インデックス

                            // 個数が削除された？
                            if (e.Row.IsNull(kosuColumnIndex))
                            {
                                // 売上区分もクリアする
                                e.Row[kbnColumnIndex] = DBNull.Value;
                            }
                            else
                            {
                                // 個数が入力された場合、売上区分が未定であればデフォルト値を設定
                                if (((e.Row[kbnColumnIndex] as int?) ?? -1) < 0)
                                {
                                    e.Row[kbnColumnIndex] = (int)Consts.UriageKubun.Urikake;
                                }
                            }
                            break;
                    }
                }
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isDtColumnChanging = false;
                //fpSpread1.ResumeLayout(true);       // 商品ＣＤを入力しても単価が描画されないことがあるのでそれを回避する
            }
        }

        /// <summary>
        /// Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread1.EditMode)
                {
                    fpSpread1.CancelCellEditing();
                }
                else
                {
                    fpSpread1.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= MAX_COLUMN_INDEX)
                    {
                        newColumnIndex = SYOHIN_START_COLUMN;
                        // ただし最終行は合計行なので移動しない
                        if (newRowIndex < fpSpread1_Sheet1.Rows.Count - 1)
                        {
                            newRowIndex++;
                        }
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread1_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                // 個数の列であれば削除可能
                if (IsKosuColumnInSpread(fpSpread1_Sheet1.ActiveColumnIndex))
                {
                    colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                }

                if (colIndex >= 0)
                {
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread1.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread1.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread1Edit_KeyDown);
            }

            if (fpSpread1.EditingControl is FarPoint.Win.FpCombo)
            {
                // コンボボックスの場合は、Enterキーでのドロップダウンを制御する
                _fpCombo = (FarPoint.Win.FpCombo)fpSpread1.EditingControl;
                _fpCombo.KeyDown += new KeyEventHandler(FpCombo_KeyDown);
                _fpCombo.DropDown += new FarPoint.Win.DropDownEventHandler(FpCombo_DropDown);
            }
        }

        /// <summary>
        /// Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread1.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread1, fpSpread1_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread1.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread1Edit_KeyDown);
            }

            if (fpSpread1.EditingControl is FarPoint.Win.FpCombo)
            {
                // ドロップダウンについても各イベントのハンドルを解除します
                _fpCombo.KeyDown -= new KeyEventHandler(FpCombo_KeyDown);
                _fpCombo.DropDown -= new FarPoint.Win.DropDownEventHandler(FpCombo_DropDown);
            }
        }

        /// <summary>
        /// Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1Edit_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合も呼ばれるので、その場合は何もしない
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread1.EditMode)
                    {
                        fpSpread1.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread1.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= MAX_COLUMN_INDEX)
                    {
                        newColumnIndex = SYOHIN_START_COLUMN;
                        // ただし最終行は合計行なので移動しない
                        if (newRowIndex < fpSpread1_Sheet1.Rows.Count - 1)
                        {
                            newRowIndex++;
                        }
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread1_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// Spreadコンボボックスキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FpCombo_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合に呼ばれた場合は、リストを表示する
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                    _fpCombo.DroppedDown = true;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread1.EditMode)
                    {
                        fpSpread1.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread1.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            //Enterキーが押された場合にフラグを立てる
            _isEnterforFpCombo = (e.KeyCode == Keys.Enter);
        }

        /// <summary>
        /// Spreadコンボボックスドロップダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FpCombo_DropDown(object sender, FarPoint.Win.DropDownEventArgs e)
        {
            //Enterキーを契機とする場合は、DropDown処理をキャンセルする
            if (_isEnterforFpCombo)
            {
                _isEnterforFpCombo = false;
                e.Cancel = true;
                // 次のセルへ移動
                SendKeys.Send("{Tab}");
            }
        }

        /// <summary>
        /// Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)bindingSource1.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 明細入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMeisai_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.Rows.Count <= 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowDetail();       // 詳細表示（既存レコード）
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 更新ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 画面の入力チェック
                if (CanSave() == false)
                {
                    return;
                }

                // 売上明細に変更はないか？
                if (CanSaveUriageMeisai() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;
                if (Save())
                {
                    // 売掛分配を取得
                    btnQuery.PerformClick();
                }
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Excel出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                saveFileDialog1.FileName = this.Text;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = saveFileDialog1.FileName;
                    FarPoint.Excel.ExcelSaveFlags excelSaveFlags = FarPoint.Excel.ExcelSaveFlags.SaveCustomColumnHeaders;
                    if (fileName.ToLower().EndsWith("xlsx"))
                    {
                        excelSaveFlags |= FarPoint.Excel.ExcelSaveFlags.UseOOXMLFormat;
                    }
                    fpSpread1.SaveExcel(fileName, excelSaveFlags);
                    if (MessageBox.Show(this, "Excel出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException)
            {
                MessageBox.Show(this, fileName + "\nファイルが開かれています。保存できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 分配元の売上明細に同一売上区分、同一商品が複数存在するかを返す。
        /// </summary>
        /// <returns>分配元の売上明細に同一売上区分、同一商品が複数存在する場合は true</returns>
        private bool HasDuplicate()
        {
            bool ret = false;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     count(*) AS CNT" +      // -- 1以上なら同一売上区分、同一商品が複数存在
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         商品ＣＤ," +
                                      "\r\n         count(商品ＣＤ) AS 商品数" +
                                      "\r\n     FROM" +
                                      "\r\n         売上明細" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND 売上日 = @売上日" +
                                      "\r\n         AND 得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n         AND 売上区分 = @売上区分売掛" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         商品ＣＤ" +
                                      "\r\n     HAVING" +
                                      "\r\n         count(商品ＣＤ) > 1" +
                                      "\r\n     ) AS A";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _urikakeBunpaiCriteria.売上日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ret = (((reader["CNT"] as int?) ?? 0) >= 1);
                        }
                    }
                }

                return ret;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 列として採用する商品名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetSyohinTable()
        {
            DataTable dt = new DataTable("商品一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    // -- 以下のように並べる
                    // -- ┌───────┬───────┬───────┐
                    // -- │    商品Ａ    │    商品Ｂ    │    商品Ｃ    │
                    // -- ├──┬────┼──┬────┼──┬────┤
                    // -- │個数│売上区分│個数│売上区分│個数│売上区分│
                    // -- └──┴────┴──┴────┴──┴────┘
                    // -- 商品ＣＤのアンダーバー以降は
                    // -- 0:個数 1:売上区分

                                    // --①個数
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     M.明細番号" +
                                      "\r\n     ,CONVERT(VARCHAR, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,'個数' AS 個数区分" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS M" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M.売上日 = @売上日" +
                                      "\r\n     AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND M.売上区分 = @売上区分売掛" +
                                      "\r\n UNION" +
                                      // -- ②売上区分
                                      "\r\n SELECT" +
                                      "\r\n     M.明細番号" +
                                      "\r\n     ,CONVERT(VARCHAR, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,'売上区分' AS 個数区分" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS M" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M.売上日 = @売上日" +
                                      "\r\n     AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND M.売上区分 = @売上区分売掛" +
                                      "\r\n ORDER BY" +
                                      "\r\n     明細番号" +
                                      "\r\n     ,商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _urikakeBunpaiCriteria.売上日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("商品一覧を取得しました。");
                }

                return dt;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
        /// </summary>
        /// <param name="syohinDt">商品名が格納されたデータテーブル</param>
        /// <param name="labelColumnName">商品名が格納された列の名前</param>
        private void SetSpreadColumn(DataTable syohinDt, string labelColumnName)
        {
            try
            {
                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN;

                // 今回追加する商品名の列数をセット
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN + syohinDt.Rows.Count;

                // 固定列に続いて、商品名の列の属性をセット
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (DataRow dr in syohinDt.Rows)
                {
                    // スプレッドの個数列？
                    if (IsKosuColumnInSpread(columnIndex))
                    {
                        // 個数
                        FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                        numberCellType.EnableSubEditor = false;
                        numberCellType.DecimalPlaces = 0;
                        numberCellType.MaximumValue = 99999D;
                        numberCellType.MinimumValue = 0D;
                        numberCellType.Separator = ",";
                        numberCellType.ShowSeparator = true;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Label = "個数";
                        fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
                    }
                    else
                    {
                        // 売上区分
                        FarPoint.Win.Spread.CellType.ComboBoxCellType comboBoxCellType = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                        ComboUtil.SetUriageKubunWithoutLine(comboBoxCellType);      // 売上区分が「5:LINE決済」未満のみに限定
                        comboBoxCellType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
                        comboBoxCellType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
                        comboBoxCellType.ButtonAlign = FarPoint.Win.ButtonAlign.Right;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = comboBoxCellType;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Label = "支払区分";
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Width = 100F;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);

                        // 個数と金額が揃ったので、列ヘッダーを設定
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex - 1).ColumnSpan = 2;
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex - 1).Value = dr[labelColumnName].ToString();
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex - 1).Value = "個数";
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex).Value = "支払区分";
                    }
                    columnIndex++;
                }
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 更新対象の分配元・分配先の売上明細を持つデータテーブルを取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetUriageMeisai()
        {
            DataTable dt = new DataTable("売上明細");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     0 AS 分配元先区分" +
                                      "\r\n     ,M.企業ＣＤ" +
                                      "\r\n     ,M.売上日" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,M.部署ＣＤ" +
                                      "\r\n     ,M.明細番号" +
                                      "\r\n     ,M.商品ＣＤ" +
                                      "\r\n     ,M.商品グループＣＤ" +
                                      "\r\n     ,M.売上区分" +
                                      "\r\n     ,M.商品単価" +
                                      "\r\n     ,M.商品数量" +
                                      "\r\n     ,M.値引額" +
                                      "\r\n     ,M.値引事由区分" +
                                      "\r\n     ,M.値引前金額" +
                                      "\r\n     ,M.値引後金額" +
                                      "\r\n     ,M.消費税額" +
                                      "\r\n     ,M.商品金額合計税込" +
                                      "\r\n     ,M.計算税率" +
                                      "\r\n     ,M.チケット束フラグ" +
                                      "\r\n     ,M.現金補填フラグ" +
                                      "\r\n     ,M.主食ＣＤ" +
                                      "\r\n     ,M.副食ＣＤ" +
                                      "\r\n     ,M.食事区分" +
                                      "\r\n     ,M.分配元数量" +
                                      "\r\n     ,M.作成担当者ＣＤ" +
                                      "\r\n     ,M.作成日" +
                                      "\r\n     ,M.修正担当者ＣＤ" +
                                      "\r\n     ,M.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS M" +
                                      "\r\n WHERE" +
                                      "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M.売上日   = @売上日" +
                                      "\r\n     AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 分配元先区分" +
                                      "\r\n     ,M.企業ＣＤ" +
                                      "\r\n     ,M.売上日" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,M.部署ＣＤ" +
                                      "\r\n     ,M.明細番号" +
                                      "\r\n     ,M.商品ＣＤ" +
                                      "\r\n     ,M.商品グループＣＤ" +
                                      "\r\n     ,M.売上区分" +
                                      "\r\n     ,M.商品単価" +
                                      "\r\n     ,M.商品数量" +
                                      "\r\n     ,M.値引額" +
                                      "\r\n     ,M.値引事由区分" +
                                      "\r\n     ,M.値引前金額" +
                                      "\r\n     ,M.値引後金額" +
                                      "\r\n     ,M.消費税額" +
                                      "\r\n     ,M.商品金額合計税込" +
                                      "\r\n     ,M.計算税率" +
                                      "\r\n     ,M.チケット束フラグ" +
                                      "\r\n     ,M.現金補填フラグ" +
                                      "\r\n     ,M.主食ＣＤ" +
                                      "\r\n     ,M.副食ＣＤ" +
                                      "\r\n     ,M.食事区分" +
                                      "\r\n     ,M.分配元数量" +
                                      "\r\n     ,M.作成担当者ＣＤ" +
                                      "\r\n     ,M.作成日" +
                                      "\r\n     ,M.修正担当者ＣＤ" +
                                      "\r\n     ,M.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS M" +
                                      "\r\n     INNER JOIN 得意先マスタ AS T" +
                                      "\r\n         ON T.企業ＣＤ  = M.企業ＣＤ" +
                                      "\r\n         AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND T.受注得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M.売上日   = @売上日" +
                                      "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     分配元先区分" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,M.明細番号";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _urikakeBunpaiCriteria.売上日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("売上明細を取得しました。");
                }

                return dt;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                UriageMeisaiCriteria uriageMeisaiCriteria = new UriageMeisaiCriteria();
                uriageMeisaiCriteria.stopOverwrite = true;
                uriageMeisaiCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                uriageMeisaiCriteria.得意先名 = dr["得意先名"] as string;
                uriageMeisaiCriteria.部署ＣＤ = _urikakeBunpaiCriteria.部署ＣＤ;
                uriageMeisaiCriteria.売上日 = _urikakeBunpaiCriteria.売上日;
                Form401 form = new Form401(uriageMeisaiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form401_FormClosed);
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 詳細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form401_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();

                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                // 先頭行のインデックスを復元（そこにスクロール）
                fpSpread1.SetViewportTopRow(0, rowIndex);

                // アクティブセルも可能な限り復元する
                if (activeRowIndex >= fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount)
                {
                    activeRowIndex = fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount - 1;
                }
                if (0 <= activeRowIndex)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = activeRowIndex;
                    bindingSource1.Position = activeRowIndex - 1;       // Currentプロパティがずれる場合があるのを回避（さらに動的に追加した行があるのでそれも考慮）
                }
                fpSpread1_Sheet1.ActiveColumnIndex = activeColumnIndex;
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 一括注文入力リストが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 売掛分配入力リストを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["売掛分配入力"];

            // 分配先開始行以降の変更をチェック
            for (int rowIndex = DATASOURCE_CHILDREN_ROW; rowIndex < dt.Rows.Count; rowIndex++)
            {
                DataRow dr = dt.Rows[rowIndex];
                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                    case DataRowState.Modified:     // 変更
                    case DataRowState.Deleted:      // 削除
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSave()
        {
            bool hasError = false;

            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (fpSpread1_Sheet1.Rows.Count <= CHILDREN_ROW)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 分配先数量の合計が、分配元の初期値（数量＋分配元数量）を超えていればエラー
                // （数量の数式セル【C】がマイナス値であればエラー）
                for (int columnIndex = SYOHIN_START_COLUMN; columnIndex < fpSpread1_Sheet1.Columns.Count - 1; )
                {
                    if (HasErrorKosuOver(columnIndex))
                    {
                        hasError = true;
                    }
                    columnIndex += 2;
                }

                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                DataTable dt = ds.Tables["売掛分配入力"];
                // 分配先開始行以降について
                for (int rowIndex = DATASOURCE_CHILDREN_ROW; rowIndex < dt.Rows.Count; rowIndex++)
                {
                    DataRow dr = dt.Rows[rowIndex];
                    // 数量が入力されていて売上区分が未入力の場合はエラー
                    for (int columnIndex = DATASOURCE_SYOHIN_START_COLUMN; columnIndex < dt.Columns.Count; columnIndex++)
                    {
                        // データソースの個数列？
                        if (IsKosuColumnInDataSource(columnIndex))
                        {
                            if (dr.IsNull(columnIndex) == false &&
                                ((dr[columnIndex + 1] as int?) ?? -1) < 0)
                            {
                                dr.SetColumnError(columnIndex + 1, "支払区分が未入力です。");
                                hasError = true;
                            }
                        }
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                return true;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 売上明細が登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveUriageMeisai()
        {
            try
            {
                // 表示直後の更新対象の売上明細が無い？
                if (_uriageMeisaiDt == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 最新の更新対象の売上明細を取得
                DataTable uriageMeisaiDt = GetUriageMeisai();
                if (uriageMeisaiDt == null)
                {
                    MessageBox.Show(this, "更新対象のデータが見つかりません。再表示してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 表示直後と最新の【更新対象の売上明細】に違いがあればエラー
                // 件数が異なる？
                if (_uriageMeisaiDt.Rows.Count != uriageMeisaiDt.Rows.Count)
                {
                    MessageBox.Show(this, "別の端末で更新済みです。再表示してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                for (int rowIndex = 0; rowIndex < uriageMeisaiDt.Rows.Count; rowIndex++)
                {
                    UriageMeisai uriageMeisaiA = new UriageMeisai(_uriageMeisaiDt.Rows[rowIndex]);
                    UriageMeisai uriageMeisaiB = new UriageMeisai(uriageMeisaiDt.Rows[rowIndex]);
                    // 内容が異なる？
                    if (uriageMeisaiA.Equals(uriageMeisaiB) == false)
                    {
                        MessageBox.Show(this, "別の端末で更新済みです。再表示してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                return true;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                //***********************************************************************************************************************************
                // 分配先について、同一得意先、同一商品、異なる売上区分だったのが、同じ売上区分に変更された場合、数量を合算して登録する必要がある。
                // したがって、予め得意先、商品、売上区分をキーにして数量を保持するマップを作成しておく
                // ・分配先の売上明細（分配元と同じ商品）を全て削除する
                // ・マップの内容で売上明細を追加する
                // 分配元については分配元数式（【C】と【D】）の値で、商品数量と分配元数量をそれぞれ更新する
                //***********************************************************************************************************************************
                // 得意先、商品、売上区分をキーとして、数量を値として保持するマップ
                Dictionary<BunpaisakiKey, int> bunpaisakiKeyMap = new Dictionary<BunpaisakiKey, int>();

                // キーの順序を保持するリスト
                List<BunpaisakiKey> bunpaisakiKeyList = new List<BunpaisakiKey>();

                DataSet ds = (DataSet)bindingSource1.DataSource;
                ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["売掛分配入力"];
                for (int rowIndex = DATASOURCE_CHILDREN_ROW; rowIndex < cldt.Rows.Count; rowIndex++)
                {
                    int tokuisakiCD = (int)cldt.Rows[rowIndex]["得意先ＣＤ"];

                    int? kosu = null;
                    int? uriageKubun = null;
                    foreach (DataRow syohinDr in _syohinDt.Rows)
                    {
                        string syohinCDCode = (string)syohinDr["商品ＣＤ"];     // 列に該当する商品ＣＤを取得
                        //  商品ＣＤのアンダーバー以降は 0:個数 1:売上区分
                        if (syohinCDCode.EndsWith("_0"))
                        {
                            kosu = (int?)cldt[rowIndex, syohinCDCode];          // 個数
                            uriageKubun = null;
                        }
                        else
                        {
                            uriageKubun = (int?)cldt[rowIndex, syohinCDCode];   // 売上区分
                            // 個数も売上区分も入力済み？
                            if ((kosu ?? 0) > 0 && uriageKubun != null)
                            {
                                // 商品ＣＤのアンダーバー以降を除去
                                syohinCDCode = syohinCDCode.Substring(0, syohinCDCode.LastIndexOf("_"));
                                int syohinCD = int.Parse(syohinCDCode);

                                // 分配先キーを生成
                                BunpaisakiKey bunpaisakiKey = new BunpaisakiKey(tokuisakiCD, syohinCD, (int)uriageKubun);
                                if (bunpaisakiKeyMap.ContainsKey(bunpaisakiKey) == false)
                                {
                                    // マップに無ければ追加
                                    bunpaisakiKeyMap.Add(bunpaisakiKey, 0);
                                    bunpaisakiKeyList.Add(bunpaisakiKey);
                                }
                                bunpaisakiKeyMap[bunpaisakiKey] += (int)kosu;   // 得意先、商品、売上区分ごとの個数を加算
                            }
                        }
                    }
                }


                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    // 分配先の売上明細（分配元と同じ商品）を全て削除する
                    if (DeleteBunpaisaki(con, tran) == false)
                    {
                        return false;
                    }

                    // 分配元を更新
                    // ・更新対象の分配元・分配先の売上明細を持つデータテーブルから、分配元の商品ごとのレコードを取得
                    // ・商品ごとに、分配元数式行の個数と分配元個数で更新する
                    if (SaveBunpaimoto(con, tran) == false)
                    {
                        return false;
                    }

                    // マップの内容で売上明細を追加する
                    if (SaveBunpaisaki(con, tran, bunpaisakiKeyMap, bunpaisakiKeyList) == false)
                    {
                        return false;
                    }

                    // 売上明細が存在しない売上ヘッダを削除
                    if (DeleteUriageHeader(con, tran) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    History.ProcessType historyProcess = History.ProcessType.Update;
                    log.InfoFormat("売上明細を{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return true;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 分配先の売上明細（分配元と同じ商品）を全て削除する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteBunpaisaki(SqlConnection con, SqlTransaction tran)
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n DELETE M" +
                                  "\r\n FROM" +
                                  "\r\n     売上明細 AS M" +
                                  "\r\n     INNER JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             商品ＣＤ" +
                                  "\r\n         FROM" +
                                  "\r\n             売上明細" +
                                  "\r\n         WHERE" +
                                  "\r\n             企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND 売上日   = @売上日" +
                                  "\r\n             AND 得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n             AND 部署ＣＤ = @部署ＣＤ" +
                                  "\r\n         ) AS S" +
                                  "\r\n         ON S.商品ＣＤ = M.商品ＣＤ" +
                                  "\r\n     INNER JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             得意先ＣＤ" +
                                  "\r\n         FROM" +
                                  "\r\n             得意先マスタ" +
                                  "\r\n         WHERE" +
                                  "\r\n             企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND 受注得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n         ) AS T" +
                                  "\r\n         ON T.得意先ＣＤ = M.得意先ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND M.売上日   = @売上日" +
                                  "\r\n     AND M.部署ＣＤ = @部署ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _urikakeBunpaiCriteria.売上日;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.得意先ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.部署ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("売上明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 分配元の売上明細を更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBunpaimoto(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // 分配元の売上ヘッダを取得（無ければ生成する）
                UriageHeader uriageHeader = GenerateUriageHeader(con, tran, (int)_urikakeBunpaiCriteria.得意先ＣＤ, (DateTime)_urikakeBunpaiCriteria.売上日, (int)_urikakeBunpaiCriteria.部署ＣＤ);
                if (uriageHeader == null)
                {
                    return false;
                }

                DataSet ds = (DataSet)bindingSource1.DataSource;
                ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["売掛分配入力"];

                //*****************************************************************************************
                // 分配元については分配元数式（【C】と【D】）の値で、商品数量と分配元数量をそれぞれ更新する
                //*****************************************************************************************

                int kosu = 0;               // 個数
                int bunpaimotoKosu = 0;     // 分配元個数
                // すべての商品
                foreach (DataRow syohinDr in _syohinDt.Rows)
                {
                    string syohinCDCode = (string)syohinDr["商品ＣＤ"];     // 列に該当する商品ＣＤを取得
                    //  商品ＣＤのアンダーバー以降は 0:個数 1:売上区分
                    if (syohinCDCode.EndsWith("_0"))
                    {
                        kosu = (int)cldt[FORMULA_ROW, syohinCDCode];    // 個数
                    }
                    else
                    {
                        bunpaimotoKosu = (int)cldt[FORMULA_ROW, syohinCDCode];    // 分配元個数

                        // 商品ＣＤのアンダーバー以降を除去
                        syohinCDCode = syohinCDCode.Substring(0, syohinCDCode.LastIndexOf("_"));
                        int syohinCD = int.Parse(syohinCDCode);

                        // 更新対象の分配元・分配先の売上明細を持つデータテーブルから、商品ＣＤに該当する分配元のレコードを取得
                        UriageMeisai uriageMeisai = GetBunpaimotoUriageMeisai(syohinCD);
                        if (uriageMeisai != null)
                        {
                            // 商品ごとに、分配元数式行の個数と分配元個数で更新する
                            uriageMeisai.商品数量 = kosu;
                            uriageMeisai.分配元数量 = bunpaimotoKosu;

                            CalcRow(uriageMeisai, uriageHeader);        // 金額再計算
                            if (uriageMeisai.Update(con, tran) == false)
                            {
                                MessageBox.Show(this, uriageMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                        }
                        else
                        {
                            log.WarnFormat("！！！分配元の売上明細（得意先ＣＤ={0} 商品ＣＤ={1}）が見つかりません！！！", _urikakeBunpaiCriteria.得意先ＣＤ, syohinCD);
                        }
                    }
                }

                return true;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 売上ヘッダ取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="uriagebi">売上日</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        private UriageHeader GenerateUriageHeader(SqlConnection con, SqlTransaction tran, int tokuisakiCD, DateTime uriagebi, int busyoCD)
        {
            try
            {
                // 売上ヘッダを取得
                UriageHeader uriageHeader = UriageHeader.GetUriageHeader(con, tran, tokuisakiCD, uriagebi, busyoCD);
                if (uriageHeader == null)
                {
                    // 存在しない場合は生成して登録
                    uriageHeader = new UriageHeader();
                    uriageHeader.売上日 = uriagebi;
                    uriageHeader.得意先ＣＤ = tokuisakiCD;
                    uriageHeader.部署ＣＤ = busyoCD;
                    uriageHeader.得意先名 = null;

                    // コースＣＤ・配送担当者ＣＤを設定
                    uriageHeader.コースＣＤ = null;
                    uriageHeader.配送担当者ＣＤ = null;
                    SetUriageCourse(uriageHeader);

                    // 得意先名・税区分・丸め処理区分を得意先マスタから取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki(tokuisakiCD);
                    if (tokuisaki == null)
                    {
                        uriageHeader.税区分 = (int)Syohizei.ZeiKubun.Uchi;          // 初期値は1:内税
                        uriageHeader.丸め処理区分 = (int)Rounding.HasuKubun.Floor;  // 初期値は2:切り捨て
                    }
                    else
                    {
                        uriageHeader.得意先名 = tokuisaki.得意先名;
                        uriageHeader.税区分 = tokuisaki.税区分 ?? (int)Syohizei.ZeiKubun.Uchi;
                        uriageHeader.丸め処理区分 = tokuisaki.丸め処理区分 ?? (int)Rounding.HasuKubun.Floor;
                    }

                    // 納品書伝票番号を注文ヘッダから取得
                    uriageHeader.納品書伝票番号 = null;
                    ChumonHeader chumonHeader = ChumonHeader.GetChumonHeader(Consts.ChumonKubun.Chuumon, (DateTime)uriageHeader.売上日, (int)uriageHeader.得意先ＣＤ, (int)uriageHeader.部署ＣＤ);
                    if (chumonHeader != null)
                    {
                        uriageHeader.納品書伝票番号 = chumonHeader.納品書伝票番号;
                    }

                    // 売上ヘッダを登録
                    bool ret = uriageHeader.Insert(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, uriageHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }

                    History.Save(con, tran, this.Text, "売上ヘッダ", uriageHeader.Code, History.ProcessType.Create);
                }

                return uriageHeader;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// マップの内容で（分配先の）売上明細を追加する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="bunpaisakiKeyMap">分配先キーと個数を持つマップ</param>
        /// <param name="bunpaisakiKeyList">キーの順序を保持するリスト</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBunpaisaki(SqlConnection con, SqlTransaction tran, Dictionary<BunpaisakiKey, int> bunpaisakiKeyMap, List<BunpaisakiKey> bunpaisakiKeyList)
        {
            try
            {
                // リストループしてキーを取得
                foreach (BunpaisakiKey bunpaisakiKey in bunpaisakiKeyList)
                {
                    // キーに該当する個数マップから取得
                    int kosu = bunpaisakiKeyMap[bunpaisakiKey];

                    // 分配元の商品単価（と商品グループＣＤ）取得すべく、GetBunpaimotoUriageMeisaiを呼び出す
                    UriageMeisai uriageMeisaiBumpaimoto = GetBunpaimotoUriageMeisai((int)bunpaisakiKey.商品ＣＤ);
                    if (uriageMeisaiBumpaimoto == null)
                    {
                        continue;
                    }

                    // 売上ヘッダを取得（無ければ生成する）
                    //*******************************************************************************************************************************
                    // このメソッドによって、分配先の売上ヘッダに分配元と同じコースが設定される。
                    // 分配先のコースが分配元と同じになることはもともと想定していたわけではなかったが、分配元と分配先が同じコースになることによって、
                    // ・コースによる締め処理の対象になる
                    // ・コースによる請求書発行の対象になる
                    // ・コース売上一覧において、分配先の数量と金額が分配元として計上できるようになる
                    // 以上のことが実現できるので、追認することになった。
                    //*******************************************************************************************************************************
                    UriageHeader uriageHeader = GenerateUriageHeader(con, tran, (int)bunpaisakiKey.得意先ＣＤ, (DateTime)_urikakeBunpaiCriteria.売上日, (int)_urikakeBunpaiCriteria.部署ＣＤ);
                    if (uriageHeader == null)
                    {
                        return false;
                    }

                    // キーから売上明細を生成
                    UriageMeisai uriageMeisai = new UriageMeisai();
                    uriageMeisai.売上日 = _urikakeBunpaiCriteria.売上日;
                    uriageMeisai.得意先ＣＤ = bunpaisakiKey.得意先ＣＤ;
                    uriageMeisai.部署ＣＤ = _urikakeBunpaiCriteria.部署ＣＤ;
                    uriageMeisai.明細番号 = UriageMeisai.GetNextMeisaiNo(con, tran, (int)uriageMeisai.得意先ＣＤ, (DateTime)uriageMeisai.売上日, (int)uriageMeisai.部署ＣＤ);
                    uriageMeisai.商品ＣＤ = bunpaisakiKey.商品ＣＤ;
                    uriageMeisai.商品グループＣＤ = uriageMeisaiBumpaimoto.商品グループＣＤ;
                    uriageMeisai.売上区分 = bunpaisakiKey.売上区分;
                    uriageMeisai.商品単価 = uriageMeisaiBumpaimoto.商品単価;
                    uriageMeisai.商品数量 = kosu;
                    uriageMeisai.値引事由区分 = null;
                    uriageMeisai.チケット束フラグ= false;
                    uriageMeisai.現金補填フラグ = false;
                    uriageMeisai.分配元数量 = null;

                    // 商品マスタから取得
                    Syohin syohin = Syohin.GetSyohin((int)uriageMeisai.商品ＣＤ);
                    uriageMeisai.主食ＣＤ = syohin.主食ＣＤ;
                    uriageMeisai.副食ＣＤ = syohin.副食ＣＤ;
                    uriageMeisai.食事区分 = syohin.食事区分;

                    // チケット？
                    if (syohin.弁当区分 == (int)Consts.BentoKubun.Ticket)
                    {
                        uriageMeisai.チケット束フラグ = true;
                    }

                    uriageMeisai.計算税率 = _syohizei.GetTaxRate(syohin.軽減税率フラグ);
                    // Form401(売上入力)のCalcRowと同じ
                    CalcRow(uriageMeisai, uriageHeader);

                    // 売上明細を登録
                    bool ret = uriageMeisai.Insert(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, uriageMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "売上明細", uriageMeisai.Code, History.ProcessType.Create);
                }
                return true;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 商品ＣＤに該当する分配元の売上明細レコードを、売上明細クラスとして返す
        /// </summary>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <returns>売上明細クラスのインスタンス</returns>
        private UriageMeisai GetBunpaimotoUriageMeisai(int syohinCD)
        {
            UriageMeisai uriageMeisai = null;
            try
            {
                // 更新対象の分配元・分配先の売上明細を持つデータテーブルから、商品ＣＤに該当する分配元のレコードを取得
                DataRow[] drs = _uriageMeisaiDt.Select(string.Format("得意先ＣＤ={0} and 商品ＣＤ={1} and 売上区分={2}",
                                                        _urikakeBunpaiCriteria.得意先ＣＤ, syohinCD, (int)Consts.UriageKubun.Urikake));
                if (drs != null && drs.Length > 0)
                {
                    uriageMeisai = new UriageMeisai(drs[0]);
                }
                return uriageMeisai;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 売上入力コース情報を設定
        /// </summary>
        /// <param name="uriageHeader">売上ヘッダ</param>
        private void SetUriageCourse(UriageHeader uriageHeader)
        {
            try
            {
                // 臨時コーステーブル／マスタから取得
                string sql = "\r\n SELECT" +
                             "\r\n     CT.コースＣＤ" +
                             "\r\n     ,CM.コース名" +
                             "\r\n     ,CM.部署ＣＤ" +
                             "\r\n     ,CM.担当者ＣＤ" +
                             "\r\n     ,T.担当者名" +
                             "\r\n FROM" +
                             "\r\n     臨時コーステーブル AS CT" +
                             "\r\n     INNER JOIN 臨時コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND CT.実施日 = CM.実施日 AND ISNULL(CM.無効フラグ, 0) = 0" +
                             "\r\n     LEFT JOIN 担当者マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND T.担当者ＣＤ = CM.担当者ＣＤ AND ISNULL(T.無効フラグ, 0) = 0" +
                             "\r\n WHERE" +
                             "\r\n     CT.企業ＣＤ = @企業ＣＤ" +
                             "\r\n     AND CT.実施日 = @実施日" +
                             "\r\n     AND CT.得意先ＣＤ = @得意先ＣＤ" +
                             "\r\n     AND ISNULL(CT.無効フラグ, 0) = 0" +
                             "\r\n     AND CM.コース区分 = @コース区分" +
                             "\r\n ORDER BY" +
                             "\r\n     CT.コースＣＤ";

                DataTable dt = GetUriageCourseMain(sql);
                if (dt == null || dt.Rows.Count == 0)
                {
                    // 存在しない場合はコーステーブル／マスタから取得
                    sql = "\r\n SELECT" +
                          "\r\n     CT.コースＣＤ" +
                          "\r\n     ,CM.コース名" +
                          "\r\n     ,CM.部署ＣＤ" +
                          "\r\n     ,CM.担当者ＣＤ" +
                          "\r\n     ,T.担当者名" +
                          "\r\n FROM" +
                          "\r\n     コーステーブル AS CT" +
                          "\r\n     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                          "\r\n     LEFT JOIN 担当者マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND T.担当者ＣＤ = CM.担当者ＣＤ AND ISNULL(T.無効フラグ, 0) = 0" +
                          "\r\n WHERE" +
                          "\r\n     CT.企業ＣＤ = @企業ＣＤ" +
                          "\r\n     AND CT.得意先ＣＤ = @得意先ＣＤ" +
                          "\r\n     AND ISNULL(CT.無効フラグ, 0) = 0" +
                          "\r\n     AND CM.コース区分 = @コース区分" +
                          "\r\n ORDER BY" +
                          "\r\n     CT.コースＣＤ";
                    dt = GetUriageCourseMain(sql);
                }

                // 部署が一致するコースを取得
                foreach (DataRow dr in dt.Rows)
                {
                    if (_urikakeBunpaiCriteria.部署ＣＤ == (int)dr["部署ＣＤ"])
                    {
                        uriageHeader.コースＣＤ = dr["コースＣＤ"] as int?;
                        uriageHeader.配送担当者ＣＤ = dr["担当者ＣＤ"] as int?;
                        break;
                    }
                }
                log.Info("売上入力コース情報を検索しました。");
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 売上入力コース情報取得主処理
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns>コース情報のデータテーブル</returns>
        private DataTable GetUriageCourseMain(string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = _urikakeBunpaiCriteria.売上日;

                    // 売上日(の曜日)からコース区分を求める
                    int courseKubun = Course.GetCourseKubun((DateTime)_urikakeBunpaiCriteria.売上日);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("売上入力コース情報");
                    adp.Fill(dt);
                    return dt;
                }
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 金額を計算
        /// </summary>
        /// <param name="uriageMeisai">計算対象の売上明細</param>
        /// <param name="uriageHeader">税区分・丸め処理区分取得用の売上ヘッダ</param>
        private void CalcRow(UriageMeisai uriageMeisai, UriageHeader uriageHeader)
        {
            try
            {
                decimal? suryo = uriageMeisai.商品数量;
                decimal? tanka = uriageMeisai.商品単価;
                decimal? nebiki = uriageMeisai.値引額;
                int? uriageKubun  = uriageMeisai.売上区分;

                // 管理マスタのチケット回収時売上フラグがOFF かつ 売上区分がチケットorボーナスの場合は、金額を0にする
                // ※回収時売上フラグがONの場合は、売上区分がチケットorボーナスの場合でも金額を算出する
                if (_kanri.チケット回収時売上フラグ != true &&
                    (uriageKubun == (int)Consts.UriageKubun.Ticket ||
                    uriageKubun == (int)Consts.UriageKubun.Bonus))
                {
                    uriageMeisai.値引前金額 = 0m;
                    uriageMeisai.値引後金額 = 0m;
                    uriageMeisai.消費税額 = 0m;
                    uriageMeisai.商品金額合計税込 = 0m;
                }
                else
                {
                    // 値引前金額・値引後金額・消費税額・商品金額合計税込を算出
                    decimal kingaku = Syohizei.GetAmount(suryo, tanka);
                    uriageMeisai.値引前金額 = kingaku;
                    decimal amount = kingaku - (nebiki ?? 0m);
                    uriageMeisai.値引後金額 = amount;
                    uriageMeisai.消費税額 = Syohizei.GetTax(uriageHeader.税区分, amount, (decimal)uriageMeisai.計算税率, uriageHeader.丸め処理区分);
                    uriageMeisai.商品金額合計税込 = Syohizei.GetAmountIncludeTax(uriageHeader.税区分, amount, (decimal)uriageMeisai.計算税率, uriageHeader.丸め処理区分);
                }
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 売上明細が存在しない売上ヘッダを削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUriageHeader(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n DELETE H" +
                                  "\r\n FROM" +
                                  "\r\n     売上ヘッダ AS H" +
                                  // -- 分配先になる得意先
                                  "\r\n     INNER JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             得意先ＣＤ" +
                                  "\r\n         FROM" +
                                  "\r\n             得意先マスタ" +
                                  "\r\n         WHERE" +
                                  "\r\n             企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND 受注得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n         ) AS T" +
                                  "\r\n         ON T.得意先ＣＤ = H.得意先ＣＤ" +
                                  "\r\n     LEFT JOIN 売上明細 AS M" +
                                  "\r\n         ON H.企業ＣＤ    = M.企業ＣＤ" +
                                  "\r\n         AND H.売上日     = M.売上日" +
                                  "\r\n         AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                  "\r\n         AND H.部署ＣＤ   = M.部署ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND H.売上日   = @売上日" +
                                  "\r\n     AND H.部署ＣＤ = @部署ＣＤ" +
                                  "\r\n     AND M.企業ＣＤ IS NULL";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _urikakeBunpaiCriteria.売上日;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.得意先ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _urikakeBunpaiCriteria.部署ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 分配先数量の合計が、分配元の初期値（数量＋分配元数量）を超える場合はエラー表示する
        /// </summary>
        private void SetErrorKosuOver()
        {
            // 全ての個数列において、分配先数量の合計が分配元の初期値（数量＋分配元数量）を超えているかをチェックする
            for (int columnIndex = SYOHIN_START_COLUMN; columnIndex < fpSpread1_Sheet1.Columns.Count - 1; )
            {
                HasErrorKosuOver(columnIndex);
                columnIndex += 2;
            }
        }

        /// <summary>
        /// 分配先数量の合計が、分配元の初期値（数量＋分配元数量）を超えているかどうかを返す
        /// （数量の数式セル【C】がマイナス値であればエラー）
        /// </summary>
        /// <param name="spreadKosuColumnIndex">個数列インデックス</param>
        /// <returns>数量の数式セル【C】がマイナス値であれば true</returns>
        private bool HasErrorKosuOver(int spreadKosuColumnIndex)
        {
            try
            {
                // Spreadの個数列でなければエラーチェックしない
                if (IsKosuColumnInSpread(spreadKosuColumnIndex) == false)
                {
                    return false;
                }

                bool ret = false;
                // PARENT_ROWは非連結行なので（つまり該当するDataRowが存在しないので）エラー（ノート）をSpreadに直接表示する
                fpSpread1_Sheet1.SetNote(PARENT_ROW, spreadKosuColumnIndex, null);
                int value = (int)fpSpread1_Sheet1.GetValue(FORMULA_ROW, spreadKosuColumnIndex);
                if (value < 0)
                {
                    fpSpread1_Sheet1.SetNote(PARENT_ROW, spreadKosuColumnIndex, "分配した数量が元の個数を超えています。");
                    ret = true;
                }

                return ret;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// データソースの個数列かどうかを返す（■と連動することに注意）
        /// </summary>
        /// <param name="columnIndex">列インデックス</param>
        /// <returns>データソースの個数列の場合は true</returns>
        private bool IsKosuColumnInDataSource(int columnIndex)
        {
            bool ret = false;
            if (columnIndex >= DATASOURCE_SYOHIN_START_COLUMN)
            {
                if (columnIndex % 2 == 0)
                {
                    ret = true;
                }
            }
            return ret;
        }

        /// <summary>
        /// Spreadの個数列かどうかを返す
        /// </summary>
        /// <param name="columnIndex">列インデックス</param>
        /// <returns>Spreadの個数列の場合は true</returns>
        private bool IsKosuColumnInSpread(int columnIndex)
        {
            bool ret = false;
            if (columnIndex >= SYOHIN_START_COLUMN)
            {
                if (columnIndex % 2 == 0)
                {
                    ret = true;
                }
            }
            return ret;
        }

        #endregion
    }
}
