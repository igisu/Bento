using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 得意先別売上推移
    /// </summary>
    public partial class Form815 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int BUSYO_CD_COLUMN       = 0;    // 部署CD列インデックス
        private const int BUSYO_NAME_COLUMN     = 1;    // 部署名列インデックス
        private const int COURSE_CD_COLUMN      = 2;    // コースCD列インデックス
        private const int COURSE_NAME_COLUMN    = 3;    // コース名列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 4;    // 得意先CD列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 5;    // 得意先名列インデックス
        // これ以降の列は動的に追加される
        private const int MONTH_START_COLUMN = 6;       // 年月列開始列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 得意先別売上推移検索条件
        /// </summary>
        private GetsujiTokuisakiSuiiCriteria _getsujiTokuisakiSuiiCriteria;

        /// <summary>
        /// 得意先別売上推移検索条件検索実行時スナップショット
        /// </summary>
        private GetsujiTokuisakiSuiiCriteria _getsujiTokuisakiSuiiCriteriaSnapShot;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = MONTH_START_COLUMN;

        /// <summary>
        /// 列として採用する年月(yyyyMM)の一覧を持つデータテーブル
        /// </summary>
        private DataTable _ymDt = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form815()
        {
            InitializeComponent();
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
        private void Form815_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.chart_curve_ico;

                lblCount.Text = "";

                // 年月列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = MONTH_START_COLUMN;

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

                // 先頭6列を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = MONTH_START_COLUMN;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _getsujiTokuisakiSuiiCriteria = new GetsujiTokuisakiSuiiCriteria();
                getsujiTokuisakiSuiiCriteriaBindingSource.DataSource = _getsujiTokuisakiSuiiCriteria;

                // ソートキーの選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "部署CD", "コースCD", "得意先CD" },
                    new string[] { " 部署ＣＤ {0}, コースＣＤ, 得意先ＣＤ",
                                   " コースＣＤ {0}, 部署ＣＤ, 得意先ＣＤ",
                                   " 得意先ＣＤ {0}, 部署ＣＤ, コースＣＤ"});
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
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form815_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form815_Shown(object sender, EventArgs e)
        {
            // コースＣＤにフォーカス
            txtCourseCD.Focus();
        }

        /// <summary>
        /// コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCD.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCD_FormClosed);
                formCourseDlg.ShowDialog();
                if (formCourseDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
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
        }

        /// <summary>
        /// コース選択ダイアログクローズ（コースＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form902 form = (Form902)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCD, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    _getsujiTokuisakiSuiiCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtCourseCD);
                    ReadBinding(lblCourseName);
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
        }

        /// <summary>
        /// 得意先ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCD.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCD_FormClosed);
                formTokuisakiDlg.ShowDialog();
                if (formTokuisakiDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
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
        }

        /// <summary>
        /// 得意先選択ダイアログクローズ（得意先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form900 form = (Form900)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtTokuisakiCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    _getsujiTokuisakiSuiiCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTokuisakiCD);
                    ReadBinding(lblTokuisakiName);
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
        }

        /// <summary>
        /// 部署ごとに集計チェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSummaryByBusyo_CheckedChanged(object sender, EventArgs e)
        {
            // 部署コンボとコース（テキスト・ボタン）の活性を切り替える
            cboBusyo.Enabled = chkSummaryByBusyo.Checked;
            txtCourseCD.Enabled = chkSummaryByBusyo.Checked;
            lblCourseName.Enabled = chkSummaryByBusyo.Checked;
            btnCourseCD.Enabled = chkSummaryByBusyo.Checked;
        }

        /// <summary>
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_getsujiTokuisakiSuiiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_getsujiTokuisakiSuiiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _getsujiTokuisakiSuiiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                fpSpread1.SuspendLayout();      // 描画を一時停止

                //*******************************************************
                // 入力された年月～表示月数を遡った年月を列として追加し、
                // 月次売上集計にある得意先を行とする
                // データそのものは月次売上集計から取得する
                //*******************************************************

                // 列として採用する年月の一覧をデータテーブルとして取得する
                _ymDt = GetYMTable();

                // 年月一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_ymDt, "表示用年月");

                // データ本体を取得
                DataTable dt = new DataTable("得意先別売上推移データ");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     0 AS 明細合計区分");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,C.コースＣＤ");
                    sql.AppendLine("     ,C.コース名");
                    sql.AppendLine("     ,A.得意先ＣＤ");
                    sql.AppendLine("     ,T.得意先名");
                    sql.AppendLine("     ,(A.集計年 * 100) + A.集計月 AS 年月");
                    sql.AppendLine("     ,A.金額");
                    sql.AppendLine(" FROM");
                    // -- 年月ごとの金額
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         U.企業ＣＤ");

                    if (_getsujiTokuisakiSuiiCriteria.部署ごとに集計 == true)
                    {
                        sql.AppendLine("         ,U.部署ＣＤ");         // -- 部署ごとに集計する場合はこちら
                    }
                    else
                    {
                        sql.AppendLine("         ,NULL AS 部署ＣＤ");   // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこちら
                    }

                    sql.AppendLine("         ,U.集計年");
                    sql.AppendLine("         ,U.集計月");
                    sql.AppendLine("         ,U.得意先ＣＤ");
                    sql.AppendLine("         ,J.得意先ＣＤ AS 受注得意先ＣＤ");
                    sql.AppendLine("         ,SUM(U.今月売上額) AS 金額");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         月次売上集計 AS U");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = U.企業ＣＤ AND T.得意先ＣＤ = U.得意先ＣＤ");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = T.企業ＣＤ AND J.得意先ＣＤ = ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ)");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         U.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND ((U.集計年 * 100) + U.集計月) >= @集計年月From");
                    sql.AppendLine("         AND ((U.集計年 * 100) + U.集計月) <= @集計年月To");

                    if (_getsujiTokuisakiSuiiCriteria.部署ごとに集計 == true && _getsujiTokuisakiSuiiCriteria.部署ＣＤ != null && _getsujiTokuisakiSuiiCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("         AND U.部署ＣＤ = @部署ＣＤ");      // -- 部署ごとに集計する場合のみ有効
                    }

                    if (_getsujiTokuisakiSuiiCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("         AND U.得意先ＣＤ = @得意先ＣＤ");
                    }

                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         U.企業ＣＤ");

                    if (_getsujiTokuisakiSuiiCriteria.部署ごとに集計 == true)
                    {
                        sql.AppendLine("         ,U.部署ＣＤ");     // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこの行は不要
                    }

                    sql.AppendLine("         ,U.集計年");
                    sql.AppendLine("         ,U.集計月");
                    sql.AppendLine("         ,U.得意先ＣＤ");
                    sql.AppendLine("         ,J.得意先ＣＤ");
                    sql.AppendLine("     UNION");
                    // -- 合計列
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         U.企業ＣＤ");

                    if (_getsujiTokuisakiSuiiCriteria.部署ごとに集計 == true)
                    {
                        sql.AppendLine("         ,U.部署ＣＤ");         // -- 部署ごとに集計する場合はこちら
                    }
                    else
                    {
                        sql.AppendLine("         ,NULL AS 部署ＣＤ");   // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこちら
                    }

                    sql.AppendLine("         ,9999 AS 集計年");
                    sql.AppendLine("         ,99 AS 集計月");
                    sql.AppendLine("         ,U.得意先ＣＤ");
                    sql.AppendLine("         ,J.得意先ＣＤ AS 受注得意先ＣＤ");
                    sql.AppendLine("         ,SUM(U.今月売上額) AS 金額");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         月次売上集計 AS U");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = U.企業ＣＤ AND T.得意先ＣＤ = U.得意先ＣＤ");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = T.企業ＣＤ AND J.得意先ＣＤ = ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ)");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         U.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND ((U.集計年 * 100) + U.集計月) >= @集計年月From");
                    sql.AppendLine("         AND ((U.集計年 * 100) + U.集計月) <= @集計年月To");

                    if (_getsujiTokuisakiSuiiCriteria.部署ごとに集計 == true && _getsujiTokuisakiSuiiCriteria.部署ＣＤ != null && _getsujiTokuisakiSuiiCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("         AND U.部署ＣＤ = @部署ＣＤ");      // -- 部署ごとに集計する場合のみ有効
                    }

                    if (_getsujiTokuisakiSuiiCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("         AND U.得意先ＣＤ = @得意先ＣＤ");
                    }

                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         U.企業ＣＤ");

                    if (_getsujiTokuisakiSuiiCriteria.部署ごとに集計 == true)
                    {
                        sql.AppendLine("         ,U.部署ＣＤ");     // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこの行は不要
                    }

                    sql.AppendLine("         ,U.得意先ＣＤ");
                    sql.AppendLine("         ,J.得意先ＣＤ");
                    sql.AppendLine("     ) AS A");

                    if (_getsujiTokuisakiSuiiCriteria.部署ごとに集計 == true && _getsujiTokuisakiSuiiCriteria.コースＣＤ != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合は追加しない
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT DISTINCT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CM.コースＣＤ");
                        sql.AppendLine("             ,CT.得意先ＣＤ");
                        sql.AppendLine("             ,CT.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コースマスタ AS CM");
                        sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                        sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                        sql.AppendLine("         WHERE");
                        sql.AppendLine("             CT.コースＣＤ = @コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON A.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND A.部署ＣＤ = C.部署ＣＤ");     // 月次売上集計の部署と一致するものに限る
                        sql.AppendLine("         AND A.受注得意先ＣＤ = C.得意先ＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _getsujiTokuisakiSuiiCriteria.コースＣＤ;
                    }
                    else
                    {
                        // -- 検索条件にコースが指定されていない場合は、ヒットした得意先が持つコースのうち最小値を取得する
                        // -- （コース注文一覧／コース売上一覧とは異なり、コースが指定されていない＝コースがnullの注文/売上 ではなく、コースを条件に加えない ということ）
                        // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合は追加する
                        sql.AppendLine("     LEFT JOIN");
                        sql.AppendLine("         (SELECT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT1.コースＣＤ");
                        sql.AppendLine("             ,CT1.得意先ＣＤ");
                        sql.AppendLine("             ,CT1.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コーステーブル AS CT1");
                        sql.AppendLine("             INNER JOIN");
                        sql.AppendLine("                 (SELECT");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                     ,MIN(T.コースＣＤ) AS コースＣＤ");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     コースマスタ AS M");
                        sql.AppendLine("                     LEFT JOIN コーステーブル AS T ON");
                        sql.AppendLine("                         T.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND T.コースＣＤ = M.コースＣＤ");
                        sql.AppendLine("                 GROUP BY");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                 ) AS CT2");
                        sql.AppendLine("                     ON CT2.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                     AND CT2.得意先ＣＤ = CT1.得意先ＣＤ");
                        sql.AppendLine("                     AND CT2.コースＣＤ = CT1.コースＣＤ");
                        sql.AppendLine("             INNER JOIN コースマスタ AS CM");
                        sql.AppendLine("                 ON CM.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT1.コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON A.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND A.受注得意先ＣＤ = C.得意先ＣＤ");
                    }
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B");
                    sql.AppendLine("         ON B.企業ＣＤ = A.企業ＣＤ");
                    sql.AppendLine("         AND B.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                    sql.AppendLine("         ON T.企業ＣＤ = A.企業ＣＤ");
                    sql.AppendLine("         AND T.得意先ＣＤ = A.得意先ＣＤ");
                    sql.AppendLine(" UNION");
                    // -- 合計行
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     1 AS 明細合計区分");
                    sql.AppendLine("     ,NULL AS 部署ＣＤ");
                    sql.AppendLine("     ,'' AS 部署名");
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                    sql.AppendLine("     ,'' AS コース名");
                    sql.AppendLine("     ,NULL AS 得意先ＣＤ");
                    sql.AppendLine("     ,'合計' AS 得意先名");
                    sql.AppendLine("     ,NULL AS 年月");
                    sql.AppendLine("     ,NULL AS 金額");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     明細合計区分,");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@集計年月From", SqlDbType.Int).Value = _getsujiTokuisakiSuiiCriteria.集計年月From;
                    cmd.Parameters.Add("@集計年月To", SqlDbType.Int).Value = _getsujiTokuisakiSuiiCriteria.集計年月To;

                    if (_getsujiTokuisakiSuiiCriteria.部署ごとに集計 == true && _getsujiTokuisakiSuiiCriteria.部署ＣＤ != null && _getsujiTokuisakiSuiiCriteria.部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _getsujiTokuisakiSuiiCriteria.部署ＣＤ;
                    }

                    if (_getsujiTokuisakiSuiiCriteria.得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _getsujiTokuisakiSuiiCriteria.得意先ＣＤ;
                    }

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("得意先別売上推移を検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("得意先別売上推移");
                // 年月列を作成
                cldt.AddColumns(_ymDt, "表示用年月", "年月", typeof(decimal));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["部署ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["部署名"]);
                cldt.AddFixedColumn(dt.Columns["コースＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["コース名"]);
                cldt.AddFixedColumn(dt.Columns["得意先ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["得意先名"]);

                log.Info("得意先別売上推移インポート開始");

                // インポート
                cldt.Import(dt, "年月", "金額");

                log.Info("得意先別売上推移データソースバインド開始");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "得意先別売上推移";

                log.Info("得意先別売上推移列バインド開始");

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", fpSpread1_Sheet1.Rows.Count - 1);

                fpSpread1_Sheet1.BindDataColumn(BUSYO_CD_COLUMN,       "部署ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,     "部署名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");

                // 年月を列名としてバインド
                int columnIndex = MONTH_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;
                log.Info("得意先別売上推移合計行数式設定開始");

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (columnIndex = MONTH_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];
                        FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];
                        sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
                    }
                }

                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    fpSpread1.Focus();
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, MONTH_START_COLUMN);
                    fpSpread1_Sheet1.AddSelection(0, MONTH_START_COLUMN, 1, 1);
                }

                // 検索実行時の検索条件を保存（Excel出力時に使用する）
                _getsujiTokuisakiSuiiCriteriaSnapShot = new GetsujiTokuisakiSuiiCriteria(_getsujiTokuisakiSuiiCriteria);
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
                fpSpread1.ResumeLayout(true);       // 描画再開
                this.MdiParent.Cursor = Cursors.Default;
                log.Info("表示ボタン終了");
            }
        }

        /// <summary>
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件を初期化
                _getsujiTokuisakiSuiiCriteria = new GetsujiTokuisakiSuiiCriteria();
                getsujiTokuisakiSuiiCriteriaBindingSource.DataSource = _getsujiTokuisakiSuiiCriteria;

                // 一覧もクリアする
                // 年月列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = MONTH_START_COLUMN;
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;
                bindingSource1.DataSource = null;

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", fpSpread1_Sheet1.Rows.Count);
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
        /// Excel出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                if (_getsujiTokuisakiSuiiCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string busyoName = "";
                if (_getsujiTokuisakiSuiiCriteriaSnapShot.部署ごとに集計 == true && _getsujiTokuisakiSuiiCriteriaSnapShot.部署ＣＤ != null && _getsujiTokuisakiSuiiCriteriaSnapShot.部署ＣＤ != -1)
                {
                    Busyo busyo = Busyo.GetBusyo((int)_getsujiTokuisakiSuiiCriteriaSnapShot.部署ＣＤ);
                    if (busyo != null)
                    {
                        busyoName = string.Format("({0})", busyo.部署名);
                    }
                }

                saveFileDialog1.FileName = string.Format("{0}_{1}年{2}月{3}",  this.Text,_getsujiTokuisakiSuiiCriteriaSnapShot.集計年, _getsujiTokuisakiSuiiCriteriaSnapShot.集計月, busyoName);
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
        /// 列として採用する年月の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetYMTable()
        {
            DataTable dt = new DataTable("年月一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    // -- 集計年月日が2017/05/01
                    // -- 表示年月が12の場合、以下のように並べる
                    // -- ┌───┬───┬───┬─┬───┬───┬──┐
                    // -- │2016/6│2016/7│2016/8│～│2017/4│2017/5│合計│
                    // -- └───┴───┴───┴─┴───┴───┴──┘

                                      // -- 0～@表示月数-1までを取得する再帰CTE（共通テーブル式）
                    cmd.CommandText = "\r\n With T(I) AS" +
                                      "\r\n (SELECT" +
                                      "\r\n     0 AS I" +
                                      "\r\n UNION ALL" +
                                      "\r\n SELECT" +
                                      "\r\n     I + 1" +
                                      "\r\n FROM" +
                                      "\r\n     T" +
                                      "\r\n WHERE" +
                                      "\r\n     I < @表示月数 - 1" +
                                      "\r\n )" +
                                      // -- ② ①で取得した日付の年月を取得
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 合計区分" +
                                      "\r\n     ,(YEAR(A.日付) * 100) + MONTH(A.日付) AS 年月" +
                                      "\r\n     ,cast(YEAR(A.日付) AS varchar) + '/' + cast(MONTH(A.日付) AS varchar) AS 表示用年月" +
                                      "\r\n FROM" +
                                      // -- ① 0～@表示月数-1までを取得する再帰CTEを用いて、指定した日付以前の日付を取得
                                      "\r\n     (SELECT" +
                                      "\r\n         CONVERT(date, DateAdd(month, I * -1, @集計年月日)) AS 日付" +
                                      "\r\n     FROM" +
                                      "\r\n         T" +
                                      "\r\n     ) AS A" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 合計区分" +
                                      "\r\n     ,999999 AS 年月" +
                                      "\r\n     ,'合計' AS 表示用年月" +
                                      "\r\n ORDER BY" +
                                      "\r\n     合計区分" +
                                      "\r\n     ,年月";

                    cmd.Parameters.Add("@表示月数", SqlDbType.Int).Value = _getsujiTokuisakiSuiiCriteria.表示月数;
                    cmd.Parameters.Add("@集計年月日", SqlDbType.Date).Value = _getsujiTokuisakiSuiiCriteria.集計年月日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("年月一覧を取得しました。");
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
        /// 年月一覧のデータテーブルを基にして、スプレッドに列を追加する
        /// </summary>
        /// <param name="ymDt">年月が格納されたデータテーブル</param>
        /// <param name="labelColumnName">表示用年月が格納された列の名前</param>
        private void SetSpreadColumn(DataTable ymDt, string labelColumnName)
        {
            try
            {
                // 年月列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = MONTH_START_COLUMN;

                // 今回追加する年月の列数をセット
                fpSpread1_Sheet1.ColumnCount = MONTH_START_COLUMN + ymDt.Rows.Count;

                // 固定列に続いて、年月の列の属性をセット
                int columnIndex = MONTH_START_COLUMN;
                for (int i = 0; i < ymDt.Rows.Count; i++)
                {
                    DataRow dr = ymDt.Rows[i];

                    // 金額
                    FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType = new FarPoint.Win.Spread.CellType.CurrencyCellType();
                    currencyCellType.CurrencySymbol = "\\";
                    currencyCellType.DecimalPlaces = 0;
                    currencyCellType.EnableSubEditor = false;
                    currencyCellType.MaximumValue = new decimal(new int[] {
                    999999999,
                    0,
                    0,
                    0});
                    currencyCellType.MinimumValue = new decimal(new int[] {
                    0,
                    0,
                    0,
                    0});
                    currencyCellType.ReadOnly = true;
                    currencyCellType.Separator = ",";
                    currencyCellType.ShowSeparator = true;
                    currencyCellType.Static = true;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = currencyCellType;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).Label = dr[labelColumnName].ToString();
                    fpSpread1_Sheet1.Columns.Get(columnIndex).Width = 100F;

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
        #endregion
    }
}
