using System;
using System.Collections.Generic;
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
    /// 未回収金一覧
    /// </summary>
    public partial class Form603 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SEIKYUBI_COLUMN       = 0;    // 売上日列インデックス
        private const int BUSYO_NAME_COLUMN     = 1;    // 部署列インデックス
        private const int COURSE_CD_COLUMN      = 2;    // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN    = 3;    // コース名列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 4;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 5;    // 得意先名列インデックス
        private const int SHIHARAI_HOHO_COLUMN  = 6;    // 支払区分列インデックス
        private const int SHIMEKUBUN_COLUMN     = 7;    // 締区分列インデックス
        private const int ZENKAI_SEIKYU_COLUMN  = 8;    // 前回請求額列インデックス
        private const int KONKAI_SEIKYU_COLUMN  = 9;    // 今回請求額列インデックス
        private const int NYUKIN_COLUMN         = 10;   // 入金額列インデックス
        private const int URIKAKEZAN_COLUMN     = 11;   // 売掛残高列インデックス
        private const int NYUKIN_CHK_COLUMN     = 12;   // 一括入金チェックボックス列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 未回収金一覧検索条件
        /// </summary>
        private MikaisyuListCriteria _mikaisyuListCriteria;

        /// <summary>
        /// 未回収金一覧検索条件スナップショット
        /// </summary>
        private MikaisyuListCriteria _mikaisyuListCriteriaSnapShot = null;

        private Form507 _formSeikyuDataEBReCreate = null;       // 請求EBデータ再作成
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form603()
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
        private void Form603_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.coins_ico;

                lblCount.Text = "";

                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.SetShiharaiHoho(cboShiharaiHoho);
                //ComboUtil.SetShimeKubun(cboShime);
                ComboUtil.Set<int>(cboShime,
                    new string[] { "", Consts.ShimeKubun.Daily.OptionDisplay(), Consts.ShimeKubun.Weekly.OptionDisplay(), Consts.ShimeKubun.Monthly.OptionDisplay(), Consts.ShimeKubun.Tsuke.OptionDisplay() },
                    new int[] { -1, (int)Consts.ShimeKubun.Daily, (int)Consts.ShimeKubun.Weekly, (int)Consts.ShimeKubun.Monthly, (int)Consts.ShimeKubun.Tsuke });

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

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _mikaisyuListCriteria = new MikaisyuListCriteria();
                mikaisyuListBindingSource.DataSource = _mikaisyuListCriteria;

                // 振替用EB口座の選択肢を、部署にぶら下がるものに再設定する
                ComboUtil.SetBusyoEB(cboEBKoza, (int)cboBusyo.SelectedValue);

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "請求日", "コースCD", "得意先CD" },
                    new string[] { "D.請求日 {0}, D.請求先ＣＤ",
                                   "D.コースＣＤ {0}, D.請求先ＣＤ, D.請求日",
                                   "D.請求先ＣＤ {0}, D.請求日"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                rdoDesc.Checked = true;
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
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
                cboBusyo.SelectedValueChanged +=  new EventHandler(cboBusyo_SelectedValueChanged);      // 部署ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form603_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F8:
                        btnDetail.PerformClick();        // 入金入力ボタン押下
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// 部署コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBusyo_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 振替用EB口座の選択肢を、部署にぶら下がるものに再設定する
                ComboUtil.SetBusyoEB(cboEBKoza, (int)cboBusyo.SelectedValue);
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
                    txtCourseCD.Text = course.コースＣＤ.ToString();
                    _mikaisyuListCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;
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
                    txtTokuisakiCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _mikaisyuListCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;
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
        /// 支払方法コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShiharaiHoho_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 支払方法が振替の場合は、振替用EB口座を入力可能
                cboEBKoza.Enabled = ((int?)cboShiharaiHoho.SelectedValue == (int)Consts.ShiharaiHohoKubun.Furikae);
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
        /// 締区分コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShime_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 締区分が月締めの場合は、締日を入力可能
                txtShimebi.Enabled = ((int?)cboShime.SelectedValue == (int)Consts.ShimeKubun.Monthly);
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
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_mikaisyuListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_mikaisyuListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _mikaisyuListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    if ((int)cboListMax.SelectedValue < 0)
                    {
                        sql.AppendLine("SELECT");
                    }
                    else
                    {
                        sql.AppendFormat("SELECT TOP({0})", cboListMax.SelectedValue);
                    }
                    sql.AppendLine("     D.請求番号");
                    sql.AppendLine("     ,D.請求日");
                    sql.AppendLine("     ,D.締区分");
                    sql.AppendLine("     ,D.締め区分名");
                    sql.AppendLine("     ,D.部署ＣＤ");
                    sql.AppendLine("     ,D.部署名");
                    sql.AppendLine("     ,D.コースＣＤ");
                    sql.AppendLine("     ,D.コース名");
                    sql.AppendLine("     ,D.請求先ＣＤ");
                    sql.AppendLine("     ,D.請求先名");
                    sql.AppendLine("     ,D.支払方法");
                    sql.AppendLine("     ,D.支払方法区分名");
                    sql.AppendLine("     ,D.前回請求額");
                    sql.AppendLine("     ,D.今回請求額");
                    sql.AppendLine("     ,D.当月入金額");
                    sql.AppendLine("     ,(D.今回請求額 - D.当月入金額) AS 売掛残高");
                    sql.AppendLine("     ,0 AS 一括入金対象");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         A.請求番号");
                    sql.AppendLine("         ,A.請求日");
                    sql.AppendLine("         ,A.締区分");
                    sql.AppendLine("         ,CASE A.締区分");
                    // V_締め区分に「未収金」が無いので結合できない
                    sql.AppendFormat("             WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Daily,   Consts.ShimeKubun.Daily.DisplayName()).AppendLine();
                    sql.AppendFormat("             WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Weekly,  Consts.ShimeKubun.Weekly.DisplayName()).AppendLine();
                    sql.AppendFormat("             WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Monthly, Consts.ShimeKubun.Monthly.DisplayName()).AppendLine();
                    sql.AppendFormat("             WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Tsuke,   Consts.ShimeKubun.Tsuke.DisplayName()).AppendLine();
                    sql.AppendLine("             ELSE ''");
                    sql.AppendLine("         END AS 締め区分名");
                    sql.AppendLine("         ,A.部署ＣＤ");
                    sql.AppendLine("         ,B.部署名");
                    sql.AppendLine("         ,C.コースＣＤ");
                    sql.AppendLine("         ,C.コース名");
                    sql.AppendLine("         ,A.請求先ＣＤ");
                    sql.AppendLine("         ,A.請求先名");
                    sql.AppendLine("         ,T.支払方法");
                    sql.AppendLine("         ,VH.支払方法区分名");
                    sql.AppendLine("         ,A.前回請求額");
                    sql.AppendLine("         ,A.今回請求額");
                    // -- 入金明細から入金額の合計を取得する相関サブクエリ（スカラサブクエリ）
                    sql.AppendLine("         ,(SELECT");
                    sql.AppendLine("             ISNULL(SUM(N.入金額), 0) AS 入金額");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             入金明細 AS N");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             N.企業ＣＤ = A.企業ＣＤ");
                    sql.AppendLine("             AND N.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("             AND N.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("             AND N.入金日 > A.請求日");
                    sql.AppendLine("             AND N.入金日 <= @請求年月日");
                    sql.AppendLine("         ) AS 当月入金額");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         請求合計 AS A");
                    sql.AppendLine("         INNER JOIN");
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 G.企業ＣＤ");
                    sql.AppendLine("                 ,G.部署ＣＤ");
                    sql.AppendLine("                 ,G.請求先ＣＤ");
                    sql.AppendLine("                 ,MAX(G.請求日) AS 請求日");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 請求合計 AS G");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 G.企業ＣＤ = @企業ＣＤ");

                    if (_mikaisyuListCriteria.部署ＣＤ != null && _mikaisyuListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("                 AND G.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _mikaisyuListCriteria.部署ＣＤ;
                    }

                    if (_mikaisyuListCriteria.得意先ＣＤ != null && _mikaisyuListCriteria.得意先ＣＤ != -1)
                    {
                        sql.AppendLine("                 AND G.請求先ＣＤ = @請求先ＣＤ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _mikaisyuListCriteria.得意先ＣＤ;
                    }

                    sql.AppendLine("                 AND G.請求日 <= @請求年月日");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 G.企業ＣＤ");
                    sql.AppendLine("                 ,G.部署ＣＤ");
                    sql.AppendLine("                 ,G.請求先ＣＤ");
                    sql.AppendLine("             ) AS SG");
                    sql.AppendLine("             ON A.企業ＣＤ = SG.企業ＣＤ");
                    sql.AppendLine("             AND A.部署ＣＤ = SG.部署ＣＤ");
                    sql.AppendLine("             AND A.請求先ＣＤ = SG.請求先ＣＤ");
                    sql.AppendLine("             AND A.請求日 = SG.請求日");
                    sql.AppendLine("         LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("         LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("         LEFT JOIN V_支払方法区分 AS VH ON VH.支払方法区分CD = T.支払方法");
                    sql.AppendLine("         LEFT JOIN V_締め区分 AS VS ON VS.締め区分CD = A.締区分");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");

                    if (_mikaisyuListCriteria.コースＣＤ != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT DISTINCT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT.コースＣＤ");
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
                        sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND A.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                        sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _mikaisyuListCriteria.コースＣＤ;
                    }
                    else
                    {
                        // -- 検索条件にコースが指定されていない場合は、ヒットした得意先が持つコースのうち最小値を取得する
                        sql.AppendLine("     LEFT JOIN");
                        sql.AppendLine("         (SELECT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT1.コースＣＤ");
                        sql.AppendLine("             ,CT1.得意先ＣＤ");
                        sql.AppendLine("             ,CT1.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コーステーブル AS CT1");
                        sql.AppendLine("             INNER JOIN");
                        sql.AppendLine("                 (SELECT");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                     ,MIN(T.コースＣＤ) AS コースＣＤ");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     コースマスタ AS M");
                        sql.AppendLine("                     LEFT JOIN コーステーブル AS T ON");
                        sql.AppendLine("                         T.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND T.コースＣＤ = M.コースＣＤ");
                        sql.AppendLine("                 GROUP BY");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                 ) AS CT2");
                        sql.AppendLine("                     ON CT2.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                     AND CT2.得意先ＣＤ = CT1.得意先ＣＤ");
                        sql.AppendLine("                     AND CT2.コースＣＤ = CT1.コースＣＤ");
                        sql.AppendLine("             INNER JOIN コースマスタ AS CM");
                        sql.AppendLine("                 ON CM.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT1.コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("             ON J.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("             AND A.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                        sql.AppendLine("             AND J.得意先ＣＤ = C.得意先ＣＤ");
                    }
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         1 = 1");

                    if (_mikaisyuListCriteria.締区分 != null && _mikaisyuListCriteria.締区分 != -1)
                    {
                        sql.AppendLine("         AND A.締区分 = @締区分");
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = _mikaisyuListCriteria.締区分;
                    }

                    if (_mikaisyuListCriteria.支払方法 != null && _mikaisyuListCriteria.支払方法 != -1)
                    {
                        sql.AppendLine("         AND T.支払方法 = @支払方法");
                        cmd.Parameters.Add("@支払方法", SqlDbType.Int).Value = _mikaisyuListCriteria.支払方法;

                        // 支払方法が振替であれば、EB銀行支店ＣＤを条件に追加する
                        if (_mikaisyuListCriteria.支払方法 == (int)Consts.ShiharaiHohoKubun.Furikae)
                        {
                            if (string.IsNullOrEmpty(_mikaisyuListCriteria.EB銀行支店ＣＤ) == false)
                            {
                                sql.AppendLine("         AND T.EB銀行支店ＣＤ = @EB銀行支店ＣＤ");
                                cmd.Parameters.Add("@EB銀行支店ＣＤ", SqlDbType.VarChar).Value = _mikaisyuListCriteria.EB銀行支店ＣＤ;
                            }
                        }
                    }

                    // 締区分が月締であれば、月締日を条件に追加する
                    if (_mikaisyuListCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                    {
                        if (_mikaisyuListCriteria.月締日 != null)
                        {
                            sql.AppendLine("         AND T.月締日 = @月締日");
                            cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = _mikaisyuListCriteria.月締日;
                        }
                    }
                    sql.AppendLine("     ) AS D");

                    // 売掛残が0円でも表示する場合は、売掛残を条件に追加しない（0円の売掛残を表示しない場合は条件に追加する）
                    if (_mikaisyuListCriteria.売掛残ゼロ == null || _mikaisyuListCriteria.売掛残ゼロ == false)
                    {
                        sql.AppendLine(" WHERE (D.今回請求額 - D.当月入金額) <> 0");
                    }

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求年月日", SqlDbType.Date).Value = _mikaisyuListCriteria.請求年月日;

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("未回収金一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    // 合計行を追加する
                    dt.Rows.Add(dt.NewRow());

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "未回収金一覧";

                    log.Info("未回収金一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SEIKYUBI_COLUMN,       "請求日");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,     "部署名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(SHIHARAI_HOHO_COLUMN,  "支払方法区分名");
                fpSpread1_Sheet1.BindDataColumn(SHIMEKUBUN_COLUMN,     "締め区分名");
                fpSpread1_Sheet1.BindDataColumn(ZENKAI_SEIKYU_COLUMN,  "前回請求額");
                fpSpread1_Sheet1.BindDataColumn(KONKAI_SEIKYU_COLUMN,  "今回請求額");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_COLUMN,         "当月入金額");
                fpSpread1_Sheet1.BindDataColumn(URIKAKEZAN_COLUMN,     "売掛残高");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_CHK_COLUMN,     "一括入金対象");

                // 背景色をリセット
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;
                }

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, SHIMEKUBUN_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = ZENKAI_SEIKYU_COLUMN; columnIndex <= URIKAKEZAN_COLUMN; columnIndex++)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];
                        FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];
                        sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
                    }
                }

                // 検索実行時の検索条件を保存
                _mikaisyuListCriteriaSnapShot = new MikaisyuListCriteria(_mikaisyuListCriteria);

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.SetActiveCell(0, SEIKYUBI_COLUMN);
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
        /// 全選択ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds == null)
                {
                    return;
                }
                DataTable dt = ds.Tables["未回収金一覧"];
                // 合計行は除く
                for (int i = 0 ; i < dt.Rows.Count - 1 ; i++)
                {
                    DataRow dr = dt.Rows[i];
                    dr["一括入金対象"] = 1;
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
        /// 全解除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds == null)
                {
                    return;
                }
                DataTable dt = ds.Tables["未回収金一覧"];
                // 合計行は除く
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    DataRow dr = dt.Rows[i];
                    dr["一括入金対象"] = 0;
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
        /// Spreadセルダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                // 列ヘッダー or 列フッター or 行ヘッダー？
                if (e.ColumnHeader || e.ColumnFooter || e.RowHeader)
                {
                    return;
                }

                // チェックボックス列？
                if (e.Column == NYUKIN_CHK_COLUMN)
                {
                    return;
                }

                // 合計行？
                if (fpSpread1_Sheet1.Rows[e.Row].BackColor == TOTAL_BACK_COLOR)
                {
                    return;
                }

                ShowDetail();       // 詳細表示
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
        /// Spreadキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    if (fpSpread1_Sheet1.Rows.Count > 0)
                    {
                        if (fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.ActiveRowIndex].BackColor != TOTAL_BACK_COLOR)
                        {
                            // チェックボックス列は詳細表示しない
                            if (fpSpread1_Sheet1.ActiveColumnIndex != NYUKIN_CHK_COLUMN)
                            {
                                ShowDetail();       // 詳細表示
                            }
                        }
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
        }

        /// <summary>
        /// Spreadセル移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            // 最終行(=合計行)には移動しない
            if (e.NewRow >= fpSpread1_Sheet1.Rows.Count - 1)
            {
                e.Cancel = true;
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
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件を初期化
                _mikaisyuListCriteria = new MikaisyuListCriteria();
                mikaisyuListBindingSource.DataSource = _mikaisyuListCriteria;

                // スナップショットもクリア
                _mikaisyuListCriteriaSnapShot = null;

                // 一覧もクリアする
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
        /// 請求EBデータ再出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeikyuEB_Click(object sender, EventArgs e)
        {
            try
            {
                ShowSeikyuDataEBReCreate();     // 請求EBデータ再出力画面表示
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
        /// 入金入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.Rows.Count == 0)
                {
                    return;
                }

                if (fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.ActiveRowIndex].BackColor == TOTAL_BACK_COLOR)
                {
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
        /// 一括入金ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatch_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.Rows.Count == 0)
                {
                    return;
                }

                if (_mikaisyuListCriteriaSnapShot == null)
                {
                    return;
                }

                // 表示年月(=請求年月)が過去の場合は警告を表示
                if (_mikaisyuListCriteria.Equals(_mikaisyuListCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                DateTime today = DateTime.Today;
                DateTime thisMonth = new DateTime(today.Year, today.Month, 1);
                DateTime targetMonth = new DateTime((int)_mikaisyuListCriteria.請求年, (int)_mikaisyuListCriteria.請求月, 1);
                if (targetMonth < thisMonth)
                {
                    if (MessageBox.Show(this, "過去の未回収金を一括入金しようとしています。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                ShowBatch();       // 一括入金入力画面表示
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
        /// 請求EBデータ再出力画面表示
        /// </summary>
        private void ShowSeikyuDataEBReCreate()
        {
            try
            {
                if (_formSeikyuDataEBReCreate == null || _formSeikyuDataEBReCreate.IsDisposed)
                {
                    this.MdiParent.Cursor = Cursors.WaitCursor;
                    _formSeikyuDataEBReCreate = null;
                    _formSeikyuDataEBReCreate = new Form507(_mikaisyuListCriteria);
                    // 別のインスタンスが開いていればそちらをアクティブにする
                    if (_formSeikyuDataEBReCreate.ActivateAnotherMe())
                    {
                        _formSeikyuDataEBReCreate = null;
                        return;
                    }
                    if (_formSeikyuDataEBReCreate.HasAuthBrowse == false)
                    {
                        MessageBox.Show(this, "画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                // モーダルにはしない。複数開くようにもしない。閉じても再検索しない。
                _formSeikyuDataEBReCreate.MdiParent = this.MdiParent;
                _formSeikyuDataEBReCreate.Show();
                _formSeikyuDataEBReCreate.Activate();
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
        /// 入金入力表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                NyukinTarget nyukinTarget = new NyukinTarget();
                nyukinTarget.得意先ＣＤ = (int)dr["請求先ＣＤ"];
                Form601 form = new Form601(nyukinTarget);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form601_FormClosed);
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
        private void Form601_FormClosed(object sender, FormClosedEventArgs e)
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

                btnQuery.PerformClick();        // 再検索

                fpSpread1.Focus();

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
                    bindingSource1.Position = activeRowIndex;       // Currentプロパティがずれる場合があるのを回避
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
        /// 一括入金入力画面表示
        /// </summary>
        private void ShowBatch()
        {
            try
            {
                // 一括入金対象の入金明細
                List<Nyukin> nyukinList = new List<Nyukin>();

                // 未選択？
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["未回収金一覧"];
                DataRow[] drs = dt.Select("一括入金対象 = 1");

                foreach (DataRow dr in drs)
                {
                    Nyukin nyukin = new Nyukin();
                    nyukin.得意先ＣＤ = (int)dr["請求先ＣＤ"];
                    nyukin.得意先名 = dr["請求先名"].ToString();
                    nyukin.入金入力区分 = (int)Consts.NyukinNyuryokuKubun.PC;
                    nyukin.入金額 = (decimal)dr["売掛残高"];
                    nyukin.対応請求番号 = (decimal)dr["請求番号"];
                    nyukin.備考 = "一括入金";
                    // 入金番号、入金日、部署ＣＤ、入金方法は一括入金入力画面で設定する
                    // 依頼人ＣＤ、振込依頼人名、入金摘要区分は設定しない
                    nyukinList.Add(nyukin);
                }

                if (nyukinList.Count == 0)
                {
                    MessageBox.Show(this, "一括入金対象が未選択です。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;
                Form603N form = new Form603N(nyukinList);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form603N_FormClosed);
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
        /// 一括入金入力画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form603N_FormClosed(object sender, FormClosedEventArgs e)
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

                btnQuery.PerformClick();        // 再検索

                fpSpread1.Focus();

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
                    bindingSource1.Position = activeRowIndex;       // Currentプロパティがずれる場合があるのを回避
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
        #endregion
    }
}
