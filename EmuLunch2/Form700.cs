using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 得意先マスタ一覧
    /// </summary>
    public partial class Form700 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_CD_COLUMN = 0;   // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 1;   // 得意先名列インデックス
        private const int TOKUISAKI_RYAKU_COLUMN = 2;   // 得意先名略称列インデックス
        private const int TEL_COLUMN = 3;   // 電話番号列インデックス
        private const int BUSYO_COLUMN = 4;   // 部署名列インデックス
        private const int JYOTAI_COLUMN = 5;   // 状態列インデックス
        private const int COURSE_CD_COLUMN = 6;   // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN = 7;   // コース名列インデックス
        private const int BIRTHDAY_1_COLUMN = 8;   // 誕生日1列インデックス
        private const int BIRTHDAY_2_COLUMN = 9;   // 誕生日2列インデックス
        private const int INVALID_COLUMN = 10;  // 無効フラグ列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用得意先マスタ
        /// </summary>
        private TokuisakiCriteria _tokuisakiCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form700()
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
        private void Form700_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnNew.Enabled = this.HasAuthEdit;
                btnDetail.Enabled = this.HasAuthBrowse;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);
                ComboUtil.SetJotaiKubun(cboJotaiKbn);
                ComboUtil.Set(cboSyushimeYobi,
                            new string[] { "",
                                           string.Format("{0}:日曜日", (int)DayOfWeek.Sunday),
                                           string.Format("{0}:月曜日", (int)DayOfWeek.Monday),
                                           string.Format("{0}:火曜日", (int)DayOfWeek.Tuesday),
                                           string.Format("{0}:水曜日", (int)DayOfWeek.Wednesday),
                                           string.Format("{0}:木曜日", (int)DayOfWeek.Thursday),
                                           string.Format("{0}:金曜日", (int)DayOfWeek.Friday),
                                           string.Format("{0}:土曜日", (int)DayOfWeek.Saturday) },
                            new int[] { -1,
                                        (int)DayOfWeek.Sunday,
                                        (int)DayOfWeek.Monday,
                                        (int)DayOfWeek.Tuesday,
                                        (int)DayOfWeek.Wednesday,
                                        (int)DayOfWeek.Thursday,
                                        (int)DayOfWeek.Friday,
                                        (int)DayOfWeek.Saturday});

                // 月のリストを生成して各コンボボックスの選択肢とする
                List<string> displayMonthList = Enumerable.Range(1, 12).Select(x => x.ToString() + "月").ToList();
                displayMonthList.Insert(0, "");
                List<int> valueMonthList = Enumerable.Range(1, 12).ToList();
                valueMonthList.Insert(0, -1);
                ComboUtil.Set(cboBirthdayFromMonth, displayMonthList.ToArray(), valueMonthList.ToArray());
                ComboUtil.Set(cboBirthdayToMonth, displayMonthList.ToArray(), valueMonthList.ToArray());

                // 日のリストを生成して各コンボボックスの選択肢とする
                List<string> displayDayList = Enumerable.Range(1, 31).Select(x => x.ToString() + "日").ToList();
                displayDayList.Insert(0, "");
                List<int> valueDayList = Enumerable.Range(1, 31).ToList();
                valueDayList.Insert(0, -1);
                ComboUtil.Set(cboBirthdayFromDay, displayDayList.ToArray(), valueDayList.ToArray());
                ComboUtil.Set(cboBirthdayToDay, displayDayList.ToArray(), valueDayList.ToArray());

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _tokuisakiCriteria = new TokuisakiCriteria();

                // 部署CDコンボの初期値を設定
                _tokuisakiCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                tokuisakiBindingSource.DataSource = _tokuisakiCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "得意先CD", "得意先名", "得意先名略称", "部署", "状態" },
                    new string[] { "A.得意先ＣＤ {0}", "A.得意先名 {0}, A.得意先ＣＤ", "A.得意先名略称 {0}, A.得意先ＣＤ",
                                   "A.部署ＣＤ {0}, A.得意先ＣＤ", "VJ.表示順 {0}, A.得意先ＣＤ" });

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                // スプレッドの最終列（無効フラグ）を非表示にする
                fpSpread1_Sheet1.Columns[INVALID_COLUMN].Visible = false;
            }
            catch (EmuException ex)
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
        private void Form700_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F6:                       // 新規作成ボタン押下
                        btnNew.PerformClick();
                        break;

                    case Keys.F8:                       // 詳細画面ボタン押下
                        btnDetail.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
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
            catch (EmuException ex)
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
                    _tokuisakiCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;
                }
            }
            catch (EmuException ex)
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
        /// 請求先CDキー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSeikyuCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSeikyuCD.PerformClick();
            }
        }

        /// <summary>
        /// 請求先CD参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeikyuCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900SeikyusakiCD_FormClosed);
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
            catch (EmuException ex)
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
        /// 得意先選択ダイアログクローズ（請求先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900SeikyusakiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtSeikyuCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtSeikyuCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _tokuisakiCriteria.請求先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblSeikyuName.Text = tokuisaki.得意先名;
                }
            }
            catch (EmuException ex)
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
                if (_tokuisakiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_tokuisakiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _tokuisakiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                        sql.AppendLine("SELECT DISTINCT");
                    }
                    else
                    {
                        sql.AppendFormat("SELECT DISTINCT TOP({0})", cboListMax.SelectedValue);
                    }
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.得意先ＣＤ");
                    sql.AppendLine("     ,A.得意先ＣＤ AS 得意先ＣＤorg");
                    sql.AppendLine("     ,A.得意先名");
                    sql.AppendLine("     ,A.得意先名カナ");
                    sql.AppendLine("     ,A.得意先名略称");
                    sql.AppendLine("     ,A.郵便番号");
                    sql.AppendLine("     ,A.住所1");
                    sql.AppendLine("     ,A.住所2");
                    sql.AppendLine("     ,A.電話番号1");
                    sql.AppendLine("     ,A.ＦＡＸ1");
                    sql.AppendLine("     ,A.電話番号2");
                    sql.AppendLine("     ,A.ＦＡＸ2");
                    sql.AppendLine("     ,A.お届け先郵便番号");
                    sql.AppendLine("     ,A.お届け先住所1");
                    sql.AppendLine("     ,A.お届け先住所2");
                    sql.AppendLine("     ,A.お届け先電話番号1");
                    sql.AppendLine("     ,A.お届け先ＦＡＸ1");
                    sql.AppendLine("     ,A.お届け先電話番号2");
                    sql.AppendLine("     ,A.お届け先ＦＡＸ2");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.売掛現金区分");
                    sql.AppendLine("     ,A.電話確認区分");
                    sql.AppendLine("     ,A.締区分");
                    sql.AppendLine("     ,A.月締日");
                    sql.AppendLine("     ,A.週締曜日");
                    sql.AppendLine("     ,A.支払サイト");
                    sql.AppendLine("     ,A.支払日");
                    sql.AppendLine("     ,A.請求先ＣＤ");
                    sql.AppendLine("     ,S.得意先名 AS 請求先名");
                    sql.AppendLine("     ,A.支払方法");
                    sql.AppendLine("     ,A.クレカ顧客番号");
                    sql.AppendLine("     ,A.クレカ番号下4桁");
                    sql.AppendLine("     ,A.クレカ有効期限");
                    sql.AppendLine("     ,A.クレカ無効フラグ");
                    sql.AppendLine("     ,A.クレカ有効性確認日");
                    sql.AppendLine("     ,A.振込手数料フラグ");
                    sql.AppendLine("     ,A.部署合算請求フラグ");
                    sql.AppendLine("     ,A.金融機関ＣＤ");
                    sql.AppendLine("     ,K.銀行名 AS 金融機関名");
                    sql.AppendLine("     ,A.金融機関支店ＣＤ");
                    sql.AppendLine("     ,KB.支店名 AS 金融機関支店名");
                    sql.AppendLine("     ,A.記号番号");
                    sql.AppendLine("     ,A.口座種別");
                    sql.AppendLine("     ,A.口座番号");
                    sql.AppendLine("     ,A.口座名義人");
                    sql.AppendLine("     ,A.チケット枚数");
                    sql.AppendLine("     ,A.サービスチケット枚数");
                    sql.AppendLine("     ,A.営業担当者ＣＤ");
                    sql.AppendLine("     ,A3.担当者名 AS 営業担当者名");
                    sql.AppendLine("     ,A.獲得営業者ＣＤ");
                    sql.AppendLine("     ,A4.担当者名 AS 獲得営業者名");
                    sql.AppendLine("     ,A.登録担当者ＣＤ");
                    sql.AppendLine("     ,A5.担当者名 AS 登録担当者名");
                    sql.AppendLine("     ,A.受注得意先ＣＤ");
                    sql.AppendLine("     ,J.得意先名 AS 受注得意先名");
                    sql.AppendLine("     ,A.受注方法");
                    sql.AppendLine("     ,A.電話確認時間_時");
                    sql.AppendLine("     ,A.電話確認時間_分");
                    sql.AppendLine("     ,A.味噌汁区分");
                    sql.AppendLine("     ,A.ふりかけ区分");
                    sql.AppendLine("     ,A.納品書発行区分");
                    sql.AppendLine("     ,A.空き容器回収区分");
                    sql.AppendLine("     ,A.請求書敬称");
                    sql.AppendLine("     ,A.備考１");
                    sql.AppendLine("     ,A.備考２");
                    sql.AppendLine("     ,A.備考３");
                    sql.AppendLine("     ,A.状態区分");
                    sql.AppendLine("     ,A.承認日");
                    sql.AppendLine("     ,A.承認者ＣＤ");
                    sql.AppendLine("     ,A6.担当者名 AS 承認者名");
                    sql.AppendLine("     ,A.状態理由区分");
                    sql.AppendLine("     ,A.休日設定");
                    sql.AppendLine("     ,A.新規登録日");
                    sql.AppendLine("     ,A.税区分");
                    sql.AppendLine("     ,A.丸め処理区分");
                    sql.AppendLine("     ,A.祝日配送区分");
                    sql.AppendLine("     ,A.誕生日1");
                    sql.AppendLine("     ,A.誕生日2");
                    sql.AppendLine("     ,A.失客日");
                    sql.AppendLine("     ,A.EB銀行支店ＣＤ");
                    sql.AppendLine("     ,A.受注時特記事項");
                    sql.AppendLine("     ,A.請求書郵便マークフラグ");
                    sql.AppendLine("     ,A.集計ＣＤ");
                    sql.AppendLine("     ,A.配送得意先ＣＤ");
                    sql.AppendLine("     ,H.得意先名 AS 配送先名");
                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,VJ.表示順");
                    sql.AppendLine("     ,VJ.状態区分名");
                    sql.AppendLine("     ,C.コースＣＤ");
                    sql.AppendLine("     ,C.コース名");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     得意先マスタ AS A");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ     AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ     AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");

                    // -- コースによる絞り込み用
                    // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                    // -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
                    if (_tokuisakiCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT DISTINCT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CT.コースＣＤ");
                        sql.AppendLine("             ,CM.コース名");
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
                        sql.AppendLine("         AND A.得意先ＣＤ = C.得意先ＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _tokuisakiCriteria.コースＣＤ;
                    }
                    else
                    {
                        // -- 検索条件にコースが指定されていない場合は、ヒットした得意先が持つコースのうち最小値を取得する
                        sql.AppendLine("     LEFT JOIN");
                        sql.AppendLine("         (SELECT");
                        sql.AppendLine("             CT2.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT1.コースＣＤ");
                        sql.AppendLine("             ,CT1.得意先ＣＤ");
                        sql.AppendLine("             ,CT1.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コーステーブル AS CT1");
                        sql.AppendLine("             INNER JOIN");
                        sql.AppendLine("                 (SELECT");
                        sql.AppendLine("                     M.部署ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                     ,MIN(T.コースＣＤ) AS コースＣＤ");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     コースマスタ AS M");
                        sql.AppendLine("                     LEFT JOIN コーステーブル AS T ON");
                        sql.AppendLine("                         T.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND T.コースＣＤ = M.コースＣＤ");
                        sql.AppendLine("                 WHERE");
                        sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                 GROUP BY");
                        sql.AppendLine("                     M.部署ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                 ) AS CT2 ON CT2.得意先ＣＤ = CT1.得意先ＣＤ and CT2.コースＣＤ = CT1.コースＣＤ");
                        sql.AppendLine("             INNER JOIN コースマスタ AS CM");
                        sql.AppendLine("                 ON CM.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT1.コースＣＤ");
                        sql.AppendLine("         WHERE");
                        sql.AppendLine("             CT1.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON A.部署ＣＤ = C.部署ＣＤ");
                        sql.AppendLine("         AND A.得意先ＣＤ = C.得意先ＣＤ");
                    }

                    sql.AppendLine("     LEFT JOIN 部署マスタ       AS B  ON  B.企業ＣＤ = A.企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0  AND A.部署ＣＤ = B.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_状態区分       AS VJ ON  A.状態区分 = VJ.状態区分CD");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ     AS S  ON  S.企業ＣＤ = A.企業ＣＤ AND ISNULL(S.無効フラグ, 0) = 0 AND A.請求先ＣＤ = S.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 金融機関名称     AS K  ON  A.金融機関ＣＤ = K.銀行ＣＤ");
                    sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KB ON  A.金融機関ＣＤ = KB.銀行ＣＤ AND A.金融機関支店ＣＤ = KB.支店ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ     AS A3 ON A3.企業ＣＤ = A.企業ＣＤ AND A.営業担当者ＣＤ = A3.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ     AS A4 ON A4.企業ＣＤ = A.企業ＣＤ AND A.獲得営業者ＣＤ = A4.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ     AS A5 ON A5.企業ＣＤ = A.企業ＣＤ AND A.登録担当者ＣＤ = A5.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ     AS A6 ON A6.企業ＣＤ = A.企業ＣＤ AND A.承認者ＣＤ = A6.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ     AS J  ON  J.企業ＣＤ = A.企業ＣＤ AND A.受注得意先ＣＤ = J.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ     AS H  ON  H.企業ＣＤ = A.企業ＣＤ AND ISNULL(H.無効フラグ, 0) = 0 AND A.配送得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN C_TelToCust      AS TC ON TC.企業ＣＤ = A.企業ＣＤ AND TC.Tel_CustNo = A.得意先ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_tokuisakiCriteria.得意先ＣＤFrom != null)
                    {
                        sql.AppendLine(" and A.得意先ＣＤ >= @得意先ＣＤFrom ");
                        cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _tokuisakiCriteria.得意先ＣＤFrom;
                    }

                    if (_tokuisakiCriteria.得意先ＣＤTo != null)
                    {
                        sql.AppendLine(" and A.得意先ＣＤ <= @得意先ＣＤTo ");
                        cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _tokuisakiCriteria.得意先ＣＤTo;
                    }

                    if (_tokuisakiCriteria.得意先名 != null)
                    {
                        // 名称そのままでの検索と、半角/全角スペースを除いた名称でも検索
                        sql.AppendLine(" and (A.得意先名 like @得意先名 escape '#' OR A.得意先名カナ like @得意先名カナ escape '#' OR A.得意先名略称 like @得意先名略称 escape '#'");
                        sql.AppendLine(" OR replace(replace(A.得意先名, ' ', ''), '　', '') like @得意先名 escape '#'");
                        sql.AppendLine(" OR replace(replace(A.得意先名カナ, ' ', ''), '　', '') like @得意先名カナ escape '#'");
                        sql.AppendLine(" OR replace(replace(A.得意先名略称, ' ', ''), '　', '') like @得意先名略称 escape '#')");
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tokuisakiCriteria.得意先名);
                        cmd.Parameters.Add("@得意先名カナ", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tokuisakiCriteria.得意先名);
                        cmd.Parameters.Add("@得意先名略称", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tokuisakiCriteria.得意先名);
                    }

                    if (_tokuisakiCriteria.電話番号 != null)
                    {
                        // 電話番号はハイフン・スペース・カッコを除いたもので検索
                        sql.AppendLine(" and (replace(replace(replace(replace(A.電話番号1, '-', ''), ' ', ''), '(', ''), ')', '') like @電話番号 escape '#'");
                        sql.AppendLine(" OR replace(replace(replace(replace(A.電話番号2, '-', ''), ' ', ''), '(', ''), ')', '') like @電話番号 escape '#'");
                        sql.AppendLine(" OR replace(replace(replace(replace(TC.Tel_TelNo, '-', ''), ' ', ''), '(', ''), ')', '') like @電話番号 escape '#')");
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tokuisakiCriteria.電話番号);
                    }

                    if (_tokuisakiCriteria.部署ＣＤ != null && _tokuisakiCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tokuisakiCriteria.部署ＣＤ;
                    }

                    if (_tokuisakiCriteria.状態区分 != null && _tokuisakiCriteria.状態区分 != -1)
                    {
                        sql.AppendLine(" and A.状態区分 = @状態区分 ");
                        cmd.Parameters.Add("@状態区分", SqlDbType.Int).Value = _tokuisakiCriteria.状態区分;
                    }

                    if (_tokuisakiCriteria.月締日 != null)
                    {
                        sql.AppendLine(" and A.月締日 = @月締日");
                        cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = _tokuisakiCriteria.月締日;
                    }

                    if (_tokuisakiCriteria.週締曜日 != null && _tokuisakiCriteria.週締曜日 != -1)
                    {
                        sql.AppendLine(" and A.週締曜日 = @週締曜日");
                        cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = _tokuisakiCriteria.週締曜日;
                    }

                    if (_tokuisakiCriteria.請求先ＣＤ != null)
                    {
                        sql.AppendLine(" and ISNULL(A.請求先ＣＤ, A.得意先ＣＤ) = @請求先ＣＤ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _tokuisakiCriteria.請求先ＣＤ;
                    }

                    // 両方とも入力されている場合のみ
                    if (_tokuisakiCriteria.月日From != null && _tokuisakiCriteria.月日To != null)
                    {
                        // 月日From＞月日Toなら、ANDがORになる（12/01～01/31など）
                        string andor = "       AND";
                        if (_tokuisakiCriteria.月日From.CompareTo(_tokuisakiCriteria.月日To) > 0)
                        {
                            andor = "       OR";
                        }
                        sql.AppendLine(" and ((RIGHT('00' + convert(varchar, month(A.誕生日1)) ,2) + RIGHT('00' + convert(varchar, day(A.誕生日1)), 2) >= @月日From");
                        sql.AppendLine(andor);
                        sql.AppendLine("       RIGHT('00' + convert(varchar, month(A.誕生日1)) ,2) + RIGHT('00' + convert(varchar, day(A.誕生日1)), 2) <= @月日To");
                        sql.AppendLine("      )");
                        sql.AppendLine("      OR");
                        sql.AppendLine("      (RIGHT('00' + convert(varchar, month(A.誕生日2)) ,2) + RIGHT('00' + convert(varchar, day(A.誕生日2)), 2) >= @月日From");
                        sql.AppendLine(andor);
                        sql.AppendLine("       RIGHT('00' + convert(varchar, month(A.誕生日2)) ,2) + RIGHT('00' + convert(varchar, day(A.誕生日2)), 2) <= @月日To");
                        sql.AppendLine("      )");
                        sql.AppendLine("     )");
                        cmd.Parameters.Add("@月日From", SqlDbType.VarChar).Value = _tokuisakiCriteria.月日From;
                        cmd.Parameters.Add("@月日To", SqlDbType.VarChar).Value = _tokuisakiCriteria.月日To;
                    }

                    if (_tokuisakiCriteria.集計ＣＤ != null)
                    {
                        sql.AppendLine(" and A.集計ＣＤ = @集計ＣＤ ");
                        cmd.Parameters.Add("@集計ＣＤ", SqlDbType.Int).Value = _tokuisakiCriteria.集計ＣＤ;
                    }

                    // チェックONの場合は全て検索するので条件に加えない（つまりチェックOFFの場合は無効フラグが0のものだけを検索）
                    if (_tokuisakiCriteria.無効フラグ == null || _tokuisakiCriteria.無効フラグ == false)
                    {
                        sql.AppendLine(" and ISNULL(A.無効フラグ, 0) = 0 ");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("得意先マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "得意先マスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("得意先マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_RYAKU_COLUMN, "得意先名略称");
                fpSpread1_Sheet1.BindDataColumn(TEL_COLUMN, "電話番号1");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_COLUMN, "部署名");
                fpSpread1_Sheet1.BindDataColumn(JYOTAI_COLUMN, "状態区分名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN, "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN, "コース名");
                fpSpread1_Sheet1.BindDataColumn(BIRTHDAY_1_COLUMN, "誕生日1");
                fpSpread1_Sheet1.BindDataColumn(BIRTHDAY_2_COLUMN, "誕生日2");
                fpSpread1_Sheet1.BindDataColumn(INVALID_COLUMN, "無効フラグ");

                // 無効フラグの値に応じて行の背景色を変える
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread1_Sheet1.Cells[rowIndex, INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    // 検索→行を選択→検索→同じ行を選択 こうすると、bindingSource.Positionが反映されずに0のまま。
                    // （なので詳細画面を開くと、選択した行とは異なるレコードが表示される）
                    // どうも(暗示的／明示的に関わらず)bindingSource.Position=0の設定が無視されているようなので、いったん最終行をカレント行に設定する
                    fpSpread1_Sheet1.ActiveRowIndex = fpSpread1_Sheet1.Rows.Count - 1;
                    bindingSource1.Position = fpSpread1_Sheet1.Rows.Count - 1;

                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    bindingSource1.Position = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }
            }
            catch (EmuException ex)
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
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件を初期化
                _tokuisakiCriteria = new TokuisakiCriteria();

                // 部署CDコンボの初期値を設定
                _tokuisakiCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                tokuisakiBindingSource.DataSource = _tokuisakiCriteria;

                // 一覧もクリアする
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;
                bindingSource1.DataSource = null;

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", fpSpread1_Sheet1.Rows.Count);
            }
            catch (EmuException ex)
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
        /// 新規作成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowNew();      // 詳細表示（新規レコード）
            }
            catch (EmuException ex)
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
        /// 詳細画面ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount != 0)
                {
                    ShowDetail();       // 詳細表示（既存レコード）
                }
                else
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (EmuException ex)
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
            catch (EmuException ex)
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
            catch (EmuException ex)
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

                ShowDetail();       // 詳細表示
            }
            catch (EmuException ex)
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
                    if (fpSpread1_Sheet1.SelectionCount != 0)
                    {
                        ShowDetail();       // 詳細表示
                    }
                }
            }
            catch (EmuException ex)
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
        /// 詳細表示（新規レコード）
        /// </summary>
        private void ShowNew()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                Tokuisaki tokuisaki = new Tokuisaki();
                Form700D form = new Form700D(tokuisaki);
                if (form.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form700D_FormClosed);
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
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
                Tokuisaki tokuisaki = new Tokuisaki(dr);
                Form700D form = new Form700D(tokuisaki);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form700D_FormClosed);
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
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
        private void Form700D_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Form form = (Form)sender;
                form.Dispose();
                form = null;

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
                    fpSpread1_Sheet1.AddSelection(activeRowIndex, -1, 1, -1);   // 単一選択行を復元
                }
                fpSpread1_Sheet1.ActiveColumnIndex = activeColumnIndex;
            }
            catch (EmuException ex)
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
