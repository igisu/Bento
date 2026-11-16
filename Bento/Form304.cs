using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 短冊印刷
    /// </summary>
    public partial class Form304 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        // 集計設定
        private const int SYUKEI_PRINT_ORDER_COLUMN = 0;    // 印刷順列インデックス
        private const int SYUKEI_PRINT_NAME_COLUMN  = 1;    // 集計印刷名列インデックス
        private const int SYUKEI_SYOKUJI_KBN_COLUMN = 2;    // 食事区分列インデックス
        private const int SYUKEI_BAG_COLUMN         = 3;    // 袋列インデックス

        // コース名変換設定
        private const int COURSE_CD_COLUMN         = 0;     // コースCD列インデックス
        private const int COURSE_NAME_COLUMN       = 1;     // コース名列インデックス
        private const int COURSE_PRINT_NAME_COLUMN = 2;     // 印刷名列インデックス
        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// データ作成＆印刷用のソート条件
        /// </summary>
        private static Dictionary<string, string> _sortMapCreateAndPrint = new Dictionary<string, string>(){
            {"0", "MAIN.印刷順 {0}, CO.コースＣＤ, CO.コース順, MAIN.得意先ＣＤ"},
            {"1", "CO.コースＣＤ {0}, CO.コース順, MAIN.印刷順, MAIN.得意先ＣＤ"}
        };

        /// <summary>
        /// 再印刷用のソート条件
        /// </summary>
        private static Dictionary<string, string> _sortMapReprint = new Dictionary<string, string>(){
            {"0", "TSM.印刷順 {0}, A.コースＣＤ, A.コース順, A.得意先ＣＤ"},
            {"1", "A.コースＣＤ {0}, A.コース順, TSM.印刷順, A.得意先ＣＤ"}
        };
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 短冊印刷条件
        /// </summary>
        private TanzakuPrintCriteria _tanzakuPrintCriteria;

        /// <summary>
        /// 短冊設定マスタ（袋パターン表示用）
        /// </summary>
        private TanzakuSettei _tanzakuSettei;

        /// <summary>
        /// 短冊発注メール設定リスト
        /// </summary>
        private List<TanzakuOrderMail> _tanzakuOrderMailList = null;

        /// <summary>
        /// １個口当たりの数量
        /// </summary>
        private int _quantityPerBox;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form304()
        {
            try
            {
                InitializeComponent();

                // １個口当たりの数量を取得
                _quantityPerBox = SystemMaster.GetQuantityPerBox();
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
        private void Form304_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                // 権限によりボタンを制御
                btnDetail.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);
                // 集計区分コンボの選択肢は、部署コンボ選択時に生成する

                // ソートキーの選択肢生成
                // cboSortKey.SelectedValue でソート条件を取得できるようにしたかったが、
                // 「データ作成＆印刷」と「再印刷」とではソート条件が異なる（条件と言うよりエイリアスが異なる）ので、
                // cboSortKey.SelectedValue ではキーを取得できるだけにしておき、実際のソート条件は _sortMapCreateAndPrint または _sortMapReprint から取得する
                ComboUtil.Set(cboSortKey,
                    new string[] { "集計設定印刷順→コースCD→コース順",
                                   "コースCD→コース順→集計設定印刷順" },
                    new string[] { "0",
                                   "1"});

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _tanzakuPrintCriteria = new TanzakuPrintCriteria();
                tanzakuPrintCriteriaBindingSource.DataSource = _tanzakuPrintCriteria;

                GetCourse();                // コース名変換設定を取得

                // 短冊発注メール設定のリストを取得
                // レコードが無ければ、シール発注ボタンを非活性にする
                _tanzakuOrderMailList = TanzakuOrderMail.GetTanzakuOrderMailList();
                if (_tanzakuOrderMailList == null || _tanzakuOrderMailList.Count == 0)
                {
                    btnSealOrder.Enabled = false;
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
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form304_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F8:                       // 設定編集ボタン押下
                    btnDetail.PerformClick();
                    break;

                case Keys.F10:                      // 再印刷ボタン押下
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                    btnRePrint.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// 注文日ラジオボタン変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoChumonbi_CheckedChanged(object sender, EventArgs e)
        {
            // 注文日、配送日の活性状態を日付指定のラジオボタンに合わせる
            dtpChumonbi.Enabled = rdoChumonbi.Checked;
            dtpHaisobi.Enabled = rdoHaisobi.Checked;

            if (rdoChumonbi.Checked)
            {
                _tanzakuPrintCriteria.注文日 = _tanzakuPrintCriteria.配送日;
                _tanzakuPrintCriteria.配送日 = null;
            }
            else
            {
                _tanzakuPrintCriteria.配送日 = _tanzakuPrintCriteria.注文日;
                _tanzakuPrintCriteria.注文日 = null;
            }
            ReadBinding(dtpChumonbi);
            ReadBinding(dtpHaisobi);
        }

        /// <summary>
        /// 部署コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBusyo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //******************************************************************************************************************************************
                // cboBusyo.SelectedValueのデータソース更新モードをOnValidationではなくOnPropertyChangedにしている。
                // このことにより、SelectedValueChangedではなくSelectedIndexChangedにおいて、_tanzakuPrintCriteria.部署ＣＤでバインド後の値が格納されている
                //******************************************************************************************************************************************
                btnDetail.Enabled = false;

                if (_tanzakuPrintCriteria == null)
                {
                    return;
                }

                // [部署名]設定を表示
                if (string.IsNullOrEmpty(cboBusyo.Text))
                {
                    lblBusyoSettei.Text = "";
                }
                else
                {
                    string name = cboBusyo.Text.Substring(cboBusyo.Text.IndexOf(":") + 1);
                    lblBusyoSettei.Text = string.Format("{0}用設定", name);
                    btnDetail.Enabled = true;       // 部署が選択されていれば、その部署の設定編集を可能にする
                }

                // 短冊設定マスタの袋パターンテキストを表示
                _tanzakuSettei = TanzakuSettei.GetTanzakuSettei(_tanzakuPrintCriteria.部署ＣＤ);
                if (_tanzakuSettei == null)
                {
                    _tanzakuSettei = new TanzakuSettei();
                }
                tanzakuSetteiBindingSource.DataSource = _tanzakuSettei;

                SetSyukeiCombo();           // 集計区分コンボの選択肢を作成
                GetTanzakuSyukeiSettei();   // 部署別の集計設定を取得
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
                    _tanzakuPrintCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    // コントロールに反映
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
                    _tanzakuPrintCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    // コントロールに反映
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
        /// データ作成＆印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_tanzakuPrintCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_tanzakuPrintCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _tanzakuPrintCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                DateTime targetDate = _tanzakuPrintCriteria.注文日 != null ? (DateTime)_tanzakuPrintCriteria.注文日 : (DateTime)_tanzakuPrintCriteria.配送日;
                string targetMsg = _tanzakuPrintCriteria.注文日 != null ? "注文日" : "配送日";
                if (targetDate > warnDate)
                {
                    if (MessageBox.Show(this, string.Format("{0}として適切ではない日付が入力されているおそれがあります。続行しますか？", targetMsg), this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // 印刷対象が既に出荷明細に存在すれば警告する
                // ※変更分のみデータ作成する場合は、既存のデータがあってもよい
                if (_tanzakuPrintCriteria.変更分のみ印字フラグ != true)
                {
                    if (HasSyukkaMeisai())
                    {
                        MessageBox.Show(this, "この条件で作成済みのデータがあります。再印刷ボタンを使用してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                // 帳票に該当するプリンタを取得。未選択ならダイアログを開く。そのあと印刷処理を実行
                DecidePrinter(Consts.Reports.Tanzaku, PrintMain);
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
        /// 再印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRePrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_tanzakuPrintCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_tanzakuPrintCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _tanzakuPrintCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                DateTime targetDate = _tanzakuPrintCriteria.注文日 != null ? (DateTime)_tanzakuPrintCriteria.注文日 : (DateTime)_tanzakuPrintCriteria.配送日;
                string targetMsg = _tanzakuPrintCriteria.注文日 != null ? "注文日" : "配送日";
                if (targetDate > warnDate)
                {
                    if (MessageBox.Show(this, string.Format("{0}として適切ではない日付が入力されているおそれがあります。続行しますか？", targetMsg), this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // 帳票に該当するプリンタを取得。未選択ならダイアログを開く。そのあと印刷処理を実行
                DecidePrinter(Consts.Reports.Tanzaku, RePrint);
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
        /// 設定編集ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (_tanzakuPrintCriteria.部署ＣＤ != null && _tanzakuPrintCriteria.部署ＣＤ >= 0)
                {
                    ShowDetail();       // 編集画面表示
                }
                else
                {
                    MessageBox.Show(this, "部署を選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// シール発注ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSealOrder_Click(object sender, EventArgs e)
        {
            try
            {
                ShowOrder();        // シール発注画面表示
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
        /// 集計区分コンボ作成
        /// </summary>
        private void SetSyukeiCombo()
        {
            try
            {
                DataTable dt = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     -1 AS 印刷順" +
                                      "\r\n     ,-1 AS 食事区分CD" +
                                      "\r\n     ,'' AS 集計印刷名 " +
                                      "\r\n UNION " +
                                      "\r\n SELECT" +
                                      "\r\n     印刷順" +
                                      "\r\n    ,食事区分CD" +
                                      "\r\n    ,REPLACE(STR(印刷順, (SELECT LEN(MAX(印刷順)) " +
                                      "\r\n                          FROM 短冊集計設定マスタ " +
                                      "\r\n                          WHERE 企業ＣＤ = @企業ＣＤ AND 部署ＣＤ = @部署ＣＤ)), ' ', '0') + ':' + 集計印刷名 AS 集計印刷名" +
                                      "\r\n FROM" +
                                      "\r\n     短冊集計設定マスタ" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     印刷順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.部署ＣＤ ?? -1;    // 未選択の場合は0件を表示

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adp.Fill(dt);
                }

                cboSyukeiFrom.DisplayMember = "集計印刷名";
                cboSyukeiFrom.ValueMember = "印刷順";           // 集計区分コンボだが、値は印刷順
                cboSyukeiFrom.DataSource = dt;

                cboSyukeiTo.DisplayMember = "集計印刷名";
                cboSyukeiTo.ValueMember = "印刷順";             // 集計区分コンボだが、値は印刷順
                cboSyukeiTo.DataSource = dt.Copy();

                // 短冊設定マスタにレコードが無い場合は、集計区分コンボと印刷実行ボタンを非活性にする
                if (dt.Rows.Count <= 1)     // ブランクの選択肢
                {
                    cboSyukeiFrom.Enabled = false;
                    cboSyukeiTo.Enabled = false;
                    btnCreateAndPrint.Enabled = false;
                }
                else
                {
                    cboSyukeiFrom.Enabled = true;
                    cboSyukeiTo.Enabled = true;
                    btnCreateAndPrint.Enabled = true;
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
        /// 部署別の集計設定を取得
        /// </summary>
        private void GetTanzakuSyukeiSettei()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     TS.企業ＣＤ" +
                                      "\r\n     ,TS.部署ＣＤ" +
                                      "\r\n     ,TS.集計区分ＣＤ" +
                                      "\r\n     ,TS.集計印刷名" +
                                      "\r\n     ,TS.食事区分ＣＤ" +
                                      "\r\n     ,VS.食事区分名" +
                                      "\r\n     ,TS.袋計算フラグ" +
                                      "\r\n     ,TS.印刷順" +
                                      "\r\n     ,TS.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,TS.作成日" +
                                      "\r\n     ,TS.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,TS.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     短冊集計設定マスタ AS TS" +
                                      "\r\n     LEFT JOIN V_食事区分   AS VS ON VS.食事区分CD = TS.食事区分CD" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = TS.企業ＣＤ AND TS.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = TS.企業ＣＤ AND TS.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     TS.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND TS.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     TS.印刷順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.部署ＣＤ ?? -1;    // 未選択の場合は0件を表示

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("短冊集計設定マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    tanzakuSyukeiBindingSource.DataSource = ds;
                    tanzakuSyukeiBindingSource.DataMember = "短冊集計設定マスタ";

                    log.Info("短冊集計設定マスタを検索しました。");
                }

                fpSpread_TanzakuSyukei_Sheet1.BindDataColumn(SYUKEI_PRINT_ORDER_COLUMN, "印刷順");
                fpSpread_TanzakuSyukei_Sheet1.BindDataColumn(SYUKEI_PRINT_NAME_COLUMN,  "集計印刷名");
                fpSpread_TanzakuSyukei_Sheet1.BindDataColumn(SYUKEI_SYOKUJI_KBN_COLUMN, "食事区分名");
                fpSpread_TanzakuSyukei_Sheet1.BindDataColumn(SYUKEI_BAG_COLUMN,         "袋計算フラグ");
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
        /// コース名変換設定取得
        /// </summary>
        private void GetCourse()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     TC.企業ＣＤ" +
                                      "\r\n     ,TC.コースＣＤ" +
                                      "\r\n     ,TC.コースＣＤ AS コースＣＤorg" +
                                      "\r\n     ,CM.コース名" +
                                      "\r\n     ,TC.短冊印刷コース名" +
                                      "\r\n     ,TC.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,TC.作成日" +
                                      "\r\n     ,TC.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,TC.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     短冊コース名マスタ AS TC" +
                                      "\r\n     INNER JOIN コースマスタ AS CM ON TC.企業ＣＤ = CM.企業ＣＤ AND TC.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ  AS A1 ON A1.企業ＣＤ = TC.企業ＣＤ AND TC.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ  AS A2 ON A2.企業ＣＤ = TC.企業ＣＤ AND TC.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     TC.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     TC.コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("短冊コース名マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    tanzakuCourseBindingSource.DataSource = ds;
                    tanzakuCourseBindingSource.DataMember = "短冊コース名マスタ";

                    log.Info("短冊コース名マスタを検索しました。");
                }

                fpSpread_TanzakuCourse_Sheet1.BindDataColumn(COURSE_CD_COLUMN,         "コースＣＤ");
                fpSpread_TanzakuCourse_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,       "コース名");
                fpSpread_TanzakuCourse_Sheet1.BindDataColumn(COURSE_PRINT_NAME_COLUMN, "短冊印刷コース名");
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
        /// 出荷明細の有無を返します
        /// </summary>
        /// <returns>印刷対象データと一致する出荷明細が１件でも存在する場合はtrue。それ以外はfalse</returns>
        private bool HasSyukkaMeisai()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     COUNT(A.出荷番号) AS CNT");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     SP_T_出荷明細 AS A");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND A.部署ＣＤ = @部署ＣＤ");

                    // 注文日と配送日はどちらかのみ指定
                    if (_tanzakuPrintCriteria.注文日 != null)
                    {
                        sql.AppendLine("     AND A.喫食日 = @注文日");
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _tanzakuPrintCriteria.注文日;
                    }

                    if (_tanzakuPrintCriteria.配送日 != null)
                    {
                        sql.AppendLine("     AND A.配送日 = @配送日");
                        cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _tanzakuPrintCriteria.配送日;
                    }

                    if (_tanzakuPrintCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine("     AND A.コースＣＤ = @コースＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.コースＣＤ;
                    }

                    if (_tanzakuPrintCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("     AND A.得意先ＣＤ = @得意先ＣＤ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.得意先ＣＤ;
                    }

                    // 印刷順のFrom/Toのいずれかが指定されている？
                    if ((_tanzakuPrintCriteria.印刷順From != null && _tanzakuPrintCriteria.印刷順From >= 0) ||
                        (_tanzakuPrintCriteria.印刷順To != null && _tanzakuPrintCriteria.印刷順To >= 0))
                    {
                        //  -- 印刷順に該当する食事区分CDを検索条件にする
                        sql.AppendLine("     AND A.食事区分 IN (SELECT");
                        sql.AppendLine("                            食事区分ＣＤ");
                        sql.AppendLine("                        FROM");
                        sql.AppendLine("                            短冊集計設定マスタ");
                        sql.AppendLine("                        WHERE");
                        sql.AppendLine("                            企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                            AND 部署ＣＤ = @部署ＣＤ");

                        if (_tanzakuPrintCriteria.印刷順From != null && _tanzakuPrintCriteria.印刷順From >= 0)
                        {
                            sql.AppendLine("                            AND 印刷順 >= @印刷順From");
                            cmd.Parameters.Add("@印刷順From", SqlDbType.Int).Value = _tanzakuPrintCriteria.印刷順From;
                        }

                        if (_tanzakuPrintCriteria.印刷順To != null && _tanzakuPrintCriteria.印刷順To >= 0)
                        {
                            sql.AppendLine("                            AND 印刷順 <= @印刷順To");
                            cmd.Parameters.Add("@印刷順To", SqlDbType.Int).Value = _tanzakuPrintCriteria.印刷順To;
                        }
                        sql.AppendLine("                       )");
                    }

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.部署ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    int cnt = 0;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        cnt = (int)reader["CNT"];
                    }

                    // レコードの有無を返す
                    return (cnt > 0);
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
        /// データ作成＆印刷処理
        /// </summary>
        /// <param name="printerName">プリンタ名</param>
        private void PrintMain(string printerName)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 印刷対象に該当するSP_T_出荷明細を取得
                DataTable dtSyukkaMeisai = GetSyukkaMeisai();

                // 印刷対象を取得
                DataTable dt = GetPrintData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "印刷するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // SP_T_出荷明細に存在する（＝印刷済みのレコード）を除いた（＝印刷対象の）レコードを保存するデータテーブル
                DataTable dtPrintData = dt.Clone();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    foreach (DataRow dr in dt.Rows)
                    {
                        // 変更があった短冊のみ印刷？
                        if (_tanzakuPrintCriteria.変更分のみ印字フラグ == true)
                        {
                            // 印刷対象とSP_T_出荷明細を比較し、全く同じレコードが出荷明細に存在する場合はスキップする
                            DataRow[] drs = dtSyukkaMeisai.Select(string.Format("配送日=#{0:yyyy/MM/dd}# " +
                                                                                "and 部署ＣＤ={1} " +
                                                                                "and コースＣＤ={2} "+
                                                                                "and 得意先ＣＤ={3} " +
                                                                                "and 喫食日=#{4:yyyy/MM/dd}# "+
                                                                                "and 食事区分={5} " +
                                                                                "and 集計注文数='{6}' " +
                                                                                "and 袋数表記='{7}' " +
                                                                                "and グループ表記比較用='{8}'",
                                                                                (DateTime)dr["配送日"],
                                                                                (int)dr["部署ＣＤ"],
                                                                                ((dr["コースＣＤ"] as int?) ?? -1),
                                                                                (int)dr["得意先ＣＤ"],
                                                                                (DateTime)dr["喫食日"],
                                                                                (int)dr["食事区分"],
                                                                                (decimal)dr["数量"],
                                                                                dr["袋数"].ToString(),
                                                                                dr["比較保存用食事区分別食数"].ToString()));
                            if (drs != null && drs.Length != 0)
                            {
                                continue;
                            }
                        }

                        // 出荷明細を追加・最新の出荷番号のレコード（＝追加したばかりのレコード）を取得
                        SyukkaMeisai syukkaMeisai = new SyukkaMeisai();
                        syukkaMeisai.配送日 = (DateTime)dr["配送日"];
                        syukkaMeisai.部署ＣＤ = (int)dr["部署ＣＤ"];
                        syukkaMeisai.コースＣＤ = dr["コースＣＤ"] as int?;
                        syukkaMeisai.コース名 = dr["コース名"].ToString();
                        syukkaMeisai.コース順 = dr["コース順"] as int?;
                        syukkaMeisai.得意先ＣＤ = (int)dr["得意先ＣＤ"];
                        syukkaMeisai.得意先名 = dr["得意先名"].ToString();
                        syukkaMeisai.喫食日 = (DateTime)dr["喫食日"];
                        syukkaMeisai.食事区分 = (int)dr["食事区分"];
                        syukkaMeisai.集計注文数 = (decimal)dr["数量"];
                        syukkaMeisai.袋数表記 = dr["袋数"].ToString();
                        syukkaMeisai.グループ表記 = dr["食事区分別食数"].ToString();
                        syukkaMeisai.グループ表記比較用 = dr["比較保存用食事区分別食数"].ToString();
                        syukkaMeisai.個口数 = GetBoxes(syukkaMeisai.集計注文数);
                        if (syukkaMeisai.Insert(con, tran) == false)
                        {
                            MessageBox.Show(this, syukkaMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        syukkaMeisai = SyukkaMeisai.GetCurrentSyukkaMeisai(con, tran);
                        if (syukkaMeisai == null)
                        {
                            log.Error("最新の出荷明細の取得に失敗。");
                            tran.Rollback();
                            MessageBox.Show(this, "最新の出荷明細が取得できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        for (int i = 1 ; i <= syukkaMeisai.個口数 ; i++)
                        {
                            // 追加用のレコードを生成
                            DataRow newDr = dtPrintData.NewRow();
                            newDr.ItemArray = dr.ItemArray;

                            // 出荷番号を印刷対象に持たせる
                            newDr["出荷番号"] = string.Format("{0}-{1}", syukkaMeisai.出荷番号, i);

                            // 印刷対象テーブルに追加
                            dtPrintData.Rows.Add(newDr);
                        }
                    }
                    tran.Commit();
                }

                if (dtPrintData.Rows.Count == 0)
                {
                    MessageBox.Show(this, "印刷するデータがありません。すべて印刷済みです。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Report304A report304A = new Report304A();
                report304A.DataSource = dtPrintData;
                report304A.Document.Printer.DocumentName = "短冊";
                report304A.Document.Printer.PrinterName = printerName;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "短冊";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report304A);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);

                log.InfoFormat("短冊を印刷しました。件数={0}", dt.Rows.Count);
                History.Save(this.Text, "短冊", "", History.ProcessType.Output);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 出荷明細を取得
        /// </summary>
        /// <returns>出荷明細テーブル</returns>
        private DataTable GetSyukkaMeisai()
        {
            try
            {
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.出荷番号");
                    sql.AppendLine("     ,A.配送日");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,ISNULL(A.コースＣＤ, -1) AS コースＣＤ");     // -- 比較用に変換する
                    sql.AppendLine("     ,A.コース名");
                    sql.AppendLine("     ,A.コース順");
                    sql.AppendLine("     ,A.得意先ＣＤ");
                    sql.AppendLine("     ,A.得意先名");
                    sql.AppendLine("     ,A.喫食日");
                    sql.AppendLine("     ,A.食事区分");
                    sql.AppendLine("     ,A.集計注文数");
                    sql.AppendLine("     ,A.袋数表記");
                    sql.AppendLine("     ,A.グループ表記");
                    sql.AppendLine("     ,A.グループ表記比較用");
                    sql.AppendLine("     ,A.個口数");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     SP_T_出荷明細 AS A");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND A.部署ＣＤ = @部署ＣＤ");

                    // 注文日と配送日はどちらかのみ指定
                    if (_tanzakuPrintCriteria.注文日 != null)
                    {
                        sql.AppendLine("     AND A.喫食日 = @注文日");
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _tanzakuPrintCriteria.注文日;
                    }

                    if (_tanzakuPrintCriteria.配送日 != null)
                    {
                        sql.AppendLine("     AND A.配送日 = @配送日");
                        cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _tanzakuPrintCriteria.配送日;
                    }

                    if (_tanzakuPrintCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine("     AND A.コースＣＤ = @コースＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.コースＣＤ;
                    }

                    if (_tanzakuPrintCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("     AND A.得意先ＣＤ = @得意先ＣＤ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.得意先ＣＤ;
                    }

                    // 印刷順のFrom/Toのいずれかが指定されている？
                    if ((_tanzakuPrintCriteria.印刷順From != null && _tanzakuPrintCriteria.印刷順From >= 0) ||
                        (_tanzakuPrintCriteria.印刷順To != null && _tanzakuPrintCriteria.印刷順To >= 0))
                    {
                        // 印刷順に該当する食事区分CDを検索条件にする
                        sql.AppendLine("     AND A.食事区分 IN (SELECT");
                        sql.AppendLine("                            食事区分ＣＤ");
                        sql.AppendLine("                        FROM");
                        sql.AppendLine("                            短冊集計設定マスタ");
                        sql.AppendLine("                        WHERE");
                        sql.AppendLine("                            企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                            AND 部署ＣＤ = @部署ＣＤ");

                        if (_tanzakuPrintCriteria.印刷順From != null && _tanzakuPrintCriteria.印刷順From >= 0)
                        {
                            sql.AppendLine("                            AND 印刷順 >= @印刷順From");
                            cmd.Parameters.Add("@印刷順From", SqlDbType.Int).Value = _tanzakuPrintCriteria.印刷順From;
                        }

                        if (_tanzakuPrintCriteria.印刷順To != null && _tanzakuPrintCriteria.印刷順To >= 0)
                        {
                            sql.AppendLine("                            AND 印刷順 <= @印刷順To");
                            cmd.Parameters.Add("@印刷順To", SqlDbType.Int).Value = _tanzakuPrintCriteria.印刷順To;
                        }
                        sql.AppendLine("                       )");
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendLine("     A.出荷番号");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.部署ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("SP_T_出荷明細");
                    adp.Fill(dt);
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
        /// 印刷対象データを取得
        /// </summary>
        /// <returns>印刷対象データ</returns>
        private DataTable GetPrintData()
        {
            try
            {
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     MAIN.喫食日");
                    sql.AppendLine("     ,MAIN.部署ＣＤ");
                    sql.AppendLine("     ,MAIN.配送日");
                    sql.AppendLine("     ,MAIN.得意先ＣＤ");
                    sql.AppendLine("     ,ISNULL(T.得意先名略称, MAIN.得意先名) AS 得意先名");
                    // -- 喫食日、得意先、集計区分（＝食事区分）ごとの食数をスペースで区切って取得
                    sql.AppendLine("     ,REPLACE('START'+ISNULL(");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             '    ' + TSM.集計印刷名 + ' ' + CAST(ISNULL(SUM(A.商品数量), 0) AS VARCHAR)");
                    sql.AppendLine("         FROM");
                    // -- 袋計算フラグが立っている集計区分(=食事区分)は全て取得したいので、短冊集計設定マスタを駆動表にして、注文を外部結合する
                    sql.AppendLine("             短冊集計設定マスタ AS TSM");
                    sql.AppendLine("             LEFT JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.企業ＣＤ");
                    sql.AppendLine("                     ,H.部署ＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,SM.食事区分");
                    sql.AppendLine("                     ,M.商品数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                         ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                         AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS SM");
                    sql.AppendLine("                         ON SM.企業ＣＤ  = M.企業ＣＤ");
                    sql.AppendLine("                         AND SM.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = MAIN.企業ＣＤ");
                    sql.AppendLine("                     AND H.注文区分 = MAIN.注文区分");
                    sql.AppendLine("                     AND H.喫食日 = MAIN.喫食日");
                    sql.AppendLine("                     AND H.得意先ＣＤ = MAIN.得意先ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = MAIN.部署ＣＤ");
                    sql.AppendLine("                 ) AS A");
                    sql.AppendLine("                 ON TSM.企業ＣＤ  = A.企業ＣＤ");
                    sql.AppendLine("                 AND TSM.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("                 AND TSM.食事区分ＣＤ = A.食事区分");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             TSM.企業ＣＤ = MAIN.企業ＣＤ");
                    sql.AppendLine("             AND TSM.部署ＣＤ = MAIN.部署ＣＤ");
                    sql.AppendLine("             AND ISNULL(TSM.袋計算フラグ, 0) <> 0");

                    // ゼロ注文を印字しない場合は、注文数が0以外のものを抽出する
                    if (_tanzakuPrintCriteria.ゼロ注文非印字フラグ == true)
                    {
                        sql.AppendLine("             AND ISNULL(A.商品数量, 0) <> 0");
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             A.得意先ＣＤ");
                    sql.AppendLine("             ,TSM.集計区分ＣＤ");
                    sql.AppendLine("             ,TSM.集計印刷名");
                    sql.AppendLine("             ,TSM.印刷順");
                    sql.AppendLine("         ORDER BY");
                    sql.AppendLine("             TSM.印刷順");
                    sql.AppendLine("         FOR XML PATH ('')");
                    sql.AppendLine("         ),'    '),'START    ','') AS 食事区分別食数");
                    // -- 出荷明細との比較用に再度、喫食日、得意先、集計区分（＝食事区分）ごとの食数をスペースで区切って取得
                    sql.AppendLine("     ,REPLACE('START'+ISNULL(");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             '    ' + TSM.集計印刷名 + ' ' + CAST(ISNULL(SUM(A.商品数量), 0) AS VARCHAR)");
                    sql.AppendLine("         FROM");
                    // -- 袋計算フラグが立っている集計区分(=食事区分)は全て取得したいので、短冊集計設定マスタを駆動表にして、注文を外部結合する
                    sql.AppendLine("             短冊集計設定マスタ AS TSM");
                    sql.AppendLine("             LEFT JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.企業ＣＤ");
                    sql.AppendLine("                     ,H.部署ＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,SM.食事区分");
                    sql.AppendLine("                     ,M.商品数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                         ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                         AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS SM");
                    sql.AppendLine("                         ON SM.企業ＣＤ  = M.企業ＣＤ");
                    sql.AppendLine("                         AND SM.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = MAIN.企業ＣＤ");
                    sql.AppendLine("                     AND H.注文区分 = MAIN.注文区分");
                    sql.AppendLine("                     AND H.喫食日 = MAIN.喫食日");
                    sql.AppendLine("                     AND H.得意先ＣＤ = MAIN.得意先ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = MAIN.部署ＣＤ");
                    sql.AppendLine("                 ) AS A");
                    sql.AppendLine("                 ON TSM.企業ＣＤ  = A.企業ＣＤ");
                    sql.AppendLine("                 AND TSM.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("                 AND TSM.食事区分ＣＤ = A.食事区分");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             TSM.企業ＣＤ = MAIN.企業ＣＤ");
                    sql.AppendLine("             AND TSM.部署ＣＤ = MAIN.部署ＣＤ");
                    sql.AppendLine("             AND ISNULL(TSM.袋計算フラグ, 0) <> 0");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             A.得意先ＣＤ");
                    sql.AppendLine("             ,TSM.集計区分ＣＤ");
                    sql.AppendLine("             ,TSM.集計印刷名");
                    sql.AppendLine("             ,TSM.印刷順");
                    sql.AppendLine("         ORDER BY");
                    sql.AppendLine("             TSM.印刷順");
                    sql.AppendLine("         FOR XML PATH ('')");
                    sql.AppendLine("         ),'    '),'START    ','') AS 比較保存用食事区分別食数");
                    sql.AppendLine("     ,MAIN.コースＣＤ");
                    sql.AppendLine("     ,ISNULL(TC.短冊印刷コース名, CM.コース名) AS コース名");
                    sql.AppendLine("     ,CO.コース順");
                    sql.AppendLine("     ,MAIN.集計印刷名");
                    sql.AppendLine("     ,MAIN.数量");
                    sql.AppendLine("     ,MAIN.袋計算フラグ");
                    sql.AppendLine("     ,MAIN.食事区分");
                    sql.AppendLine("     ,MAIN.集計印刷名 + '  ' + cast(MAIN.数量 as varchar) + ' 食' as 食数");
                    sql.AppendLine("     ,cast(NULL AS varchar) AS 出荷番号");
                    sql.AppendLine(" FROM");
                    // -- 駆動表（兼、集計区分(=食事区分)ごとの食数）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         H.企業ＣＤ");
                    sql.AppendLine("         ,H.喫食日");
                    sql.AppendLine("         ,H.部署ＣＤ");
                    sql.AppendLine("         ,H.配送日");
                    sql.AppendLine("         ,M.注文区分");
                    sql.AppendLine("         ,H.コースＣＤ");
                    sql.AppendLine("         ,H.得意先ＣＤ");
                    sql.AppendLine("         ,H.得意先名");
                    sql.AppendLine("         ,SM.食事区分");
                    sql.AppendLine("         ,TSM.集計区分ＣＤ");
                    sql.AppendLine("         ,TSM.集計印刷名");
                    sql.AppendLine("         ,TSM.印刷順");
                    sql.AppendLine("         ,TSM.袋計算フラグ");
                    sql.AppendLine("         ,SUM(M.商品数量) as 数量");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         注文ヘッダ AS H");
                    sql.AppendLine("         INNER JOIN 注文明細 AS M");
                    sql.AppendLine("             ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("             AND H.注文区分   = M.注文区分");
                    sql.AppendLine("             AND H.喫食日     = M.喫食日");
                    sql.AppendLine("             AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("             AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         INNER JOIN 商品マスタ AS SM");
                    sql.AppendLine("             ON SM.企業ＣＤ  = M.企業ＣＤ");
                    sql.AppendLine("             AND SM.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("         INNER JOIN 短冊集計設定マスタ AS TSM");
                    sql.AppendLine("             ON TSM.企業ＣＤ  = SM.企業ＣＤ");
                    sql.AppendLine("             AND TSM.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND TSM.食事区分ＣＤ = SM.食事区分");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND ISNULL(M.商品数量, 0) <> 0");      // -- 0注文はシールを印刷しない

                    // -- 注文日と配送日はどちらかのみ指定
                    if (_tanzakuPrintCriteria.注文日 != null)
                    {
                        sql.AppendLine("         AND H.喫食日 = @注文日");
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _tanzakuPrintCriteria.注文日;
                    }

                    if (_tanzakuPrintCriteria.配送日 != null)
                    {
                        sql.AppendLine("         AND H.配送日 = @配送日");
                        cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _tanzakuPrintCriteria.配送日;
                    }

                    sql.AppendLine("         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND H.注文区分 = @注文区分注文");
                    sql.AppendLine("         AND H.コースＣＤ IS NOT NULL");        // -- コースに属していない得意先の短冊は出力しなくてよい

                    if (_tanzakuPrintCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine("         AND H.コースＣＤ = @コースＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.コースＣＤ;
                    }

                    if (_tanzakuPrintCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("         AND H.得意先ＣＤ = @得意先ＣＤ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.得意先ＣＤ;
                    }

                    // 印刷順のFrom/Toのいずれかが指定されている？
                    if ((_tanzakuPrintCriteria.印刷順From != null && _tanzakuPrintCriteria.印刷順From >= 0) ||
                        (_tanzakuPrintCriteria.印刷順To != null && _tanzakuPrintCriteria.印刷順To >= 0))
                    {
                        // -- 印刷順に該当する食事区分CDを検索条件にする
                        sql.AppendLine("         AND SM.食事区分 IN (SELECT");
                        sql.AppendLine("                                 食事区分ＣＤ");
                        sql.AppendLine("                             FROM");
                        sql.AppendLine("                                 短冊集計設定マスタ");
                        sql.AppendLine("                             WHERE");
                        sql.AppendLine("                                 企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                                 AND 部署ＣＤ = @部署ＣＤ");

                        if (_tanzakuPrintCriteria.印刷順From != null && _tanzakuPrintCriteria.印刷順From >= 0)
                        {
                            sql.AppendLine("                                 AND 印刷順 >= @印刷順From");
                            cmd.Parameters.Add("@印刷順From", SqlDbType.Int).Value = _tanzakuPrintCriteria.印刷順From;
                        }

                        if (_tanzakuPrintCriteria.印刷順To != null && _tanzakuPrintCriteria.印刷順To >= 0)
                        {
                            sql.AppendLine("                                 AND 印刷順 <= @印刷順To");
                            cmd.Parameters.Add("@印刷順To", SqlDbType.Int).Value = _tanzakuPrintCriteria.印刷順To;
                        }
                        sql.AppendLine("                            )");
                    }

                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         H.企業ＣＤ");
                    sql.AppendLine("         ,H.喫食日");
                    sql.AppendLine("         ,H.部署ＣＤ");
                    sql.AppendLine("         ,H.配送日");
                    sql.AppendLine("         ,M.注文区分");
                    sql.AppendLine("         ,H.コースＣＤ");
                    sql.AppendLine("         ,H.得意先ＣＤ");
                    sql.AppendLine("         ,H.得意先名");
                    sql.AppendLine("         ,SM.食事区分");
                    sql.AppendLine("         ,TSM.集計区分ＣＤ");
                    sql.AppendLine("         ,TSM.集計印刷名");
                    sql.AppendLine("         ,TSM.印刷順");
                    sql.AppendLine("         ,TSM.袋計算フラグ");
                    sql.AppendLine("     ) AS MAIN");
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = MAIN.企業ＣＤ AND CM.コースＣＤ = MAIN.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 短冊コース名マスタ AS TC ON TC.企業ＣＤ = MAIN.企業ＣＤ AND TC.コースＣＤ = MAIN.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = MAIN.企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ");
                    // -- コース順を取得
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             CT.得意先ＣＤ");
                    sql.AppendLine("             ,CT.コースＣＤ");
                    sql.AppendLine("             ,CT.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コーステーブル AS CT");
                    sql.AppendLine("             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("             LEFT JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     臨時コースマスタ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND 実施日 = @実施日");
                    sql.AppendLine("                 ) AS RM");
                    sql.AppendLine("                 ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             RT.得意先ＣＤ");
                    sql.AppendLine("             ,RT.コースＣＤ");
                    sql.AppendLine("             ,RT.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             臨時コーステーブル AS RT");
                    sql.AppendLine("             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RT.実施日 = @実施日");
                    sql.AppendLine("         ) AS CO ON MAIN.得意先ＣＤ = CO.得意先ＣＤ AND MAIN.コースＣＤ = CO.コースＣＤ");
                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat(_sortMapCreateAndPrint[(string)cboSortKey.SelectedValue], rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = _tanzakuPrintCriteria.注文日 != null ? _tanzakuPrintCriteria.注文日 : _tanzakuPrintCriteria.配送日;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("短冊");
                    adp.Fill(dt);
                }

                // 袋数の列を追加
                dt.Columns.Add("袋数", typeof(string));

                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    if ((dr["袋計算フラグ"] as bool?) == true)
                    {
                        // 割るべき数として数量を取得
                        decimal wk_target = (decimal)dr["数量"];
                        int target = (int)wk_target;

                        string bagCnt = CalcBag(target);
                        dr["袋数"] = bagCnt;            // 求めた文字列を袋数に格納
                    }
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
        /// 再印刷処理
        /// </summary>
        /// <param name="printerName">プリンタ名</param>
        private void RePrint(string printerName)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 印刷対象を取得
                DataTable dt = GetRePrintData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "印刷するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // [個口数]分のレコードを追加したデータテーブル
                DataTable dtPrintData = dt.Clone();

                foreach (DataRow dr in dt.Rows)
                {
                    decimal syukkaNo = (decimal)dr["明細出荷番号"];
                    int count = (int)dr["個口数"];
                    for (int i = 1 ; i <= count ; i++)
                    {
                        // 追加用のレコードを生成
                        DataRow newDr = dtPrintData.NewRow();
                        newDr.ItemArray = dr.ItemArray;

                        // 出荷番号を印刷対象に持たせる
                        newDr["出荷番号"] = string.Format("{0}-{1}", syukkaNo, i);

                        // 印刷対象テーブルに追加
                        dtPrintData.Rows.Add(newDr);
                    }
                }

                Report304A report304A = new Report304A();
                report304A.DataSource = dtPrintData;
                report304A.Document.Printer.DocumentName = "短冊";
                report304A.Document.Printer.PrinterName = printerName;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "短冊";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report304A);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);

                log.InfoFormat("短冊を再印刷しました。件数={0}", dt.Rows.Count);
                History.Save(this.Text, "短冊(再印刷)", "", History.ProcessType.Output);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 再印刷対象データを取得
        /// </summary>
        /// <returns>印刷対象データ</returns>
        private DataTable GetRePrintData()
        {
            try
            {
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     A.喫食日");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.配送日");
                    sql.AppendLine("     ,A.得意先ＣＤ");
                    sql.AppendLine("     ,A.得意先名");
                    sql.AppendLine("     ,A.グループ表記 AS 食事区分別食数");
                    sql.AppendLine("     ,A.コースＣＤ");
                    sql.AppendLine("     ,A.コース名");
                    sql.AppendLine("     ,A.コース順");
                    sql.AppendLine("     ,TSM.集計印刷名");
                    sql.AppendLine("     ,A.集計注文数 AS 数量");
                    sql.AppendLine("     ,TSM.袋計算フラグ");
                    sql.AppendLine("     ,A.食事区分");
                    sql.AppendLine("     ,TSM.集計印刷名 + '  ' + cast(A.集計注文数 as varchar) + ' 食' as 食数");
                    sql.AppendLine("     ,A.出荷番号 AS 明細出荷番号");
                    sql.AppendLine("     ,A.個口数");
                    sql.AppendLine("     ,cast(NULL AS varchar) AS 出荷番号");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     SP_T_出荷明細 AS A");
                    // -- 出荷明細を再作成した場合は同じデータでも新しい出荷番号が採番されるので、最新の出荷番号を取得する必要がある
                    sql.AppendLine("     INNER JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             企業ＣＤ");
                    sql.AppendLine("             ,配送日");
                    sql.AppendLine("             ,部署ＣＤ");
                    sql.AppendLine("             ,コースＣＤ");
                    sql.AppendLine("             ,得意先ＣＤ");
                    sql.AppendLine("             ,喫食日");
                    sql.AppendLine("             ,食事区分");
                    sql.AppendLine("             ,MAX(出荷番号) AS 出荷番号");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             SP_T_出荷明細");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");

                    // -- 注文日と配送日はどちらかのみ指定
                    if (_tanzakuPrintCriteria.注文日 != null)
                    {
                        sql.AppendLine("             AND 喫食日 = @注文日");
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _tanzakuPrintCriteria.注文日;
                    }

                    if (_tanzakuPrintCriteria.配送日 != null)
                    {
                        sql.AppendLine("             AND 配送日 = @配送日");
                        cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _tanzakuPrintCriteria.配送日;
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             企業ＣＤ");
                    sql.AppendLine("             ,配送日");
                    sql.AppendLine("             ,部署ＣＤ");
                    sql.AppendLine("             ,コースＣＤ");
                    sql.AppendLine("             ,得意先ＣＤ");
                    sql.AppendLine("             ,喫食日");
                    sql.AppendLine("             ,食事区分");
                    sql.AppendLine("         ) AS A2");
                    sql.AppendLine("         ON A.企業ＣＤ = A2.企業ＣＤ");
                    sql.AppendLine("         AND A.出荷番号 = A2.出荷番号");
                    sql.AppendLine("     INNER JOIN 短冊集計設定マスタ AS TSM");
                    sql.AppendLine("         ON TSM.企業ＣＤ  = A.企業ＣＤ");
                    sql.AppendLine("         AND TSM.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("         AND TSM.食事区分ＣＤ = A.食事区分");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     1 = 1");

                    if (_tanzakuPrintCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine("     AND A.コースＣＤ = @コースＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.コースＣＤ;
                    }

                    if (_tanzakuPrintCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("     AND A.得意先ＣＤ = @得意先ＣＤ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.得意先ＣＤ;
                    }

                    // 印刷順のFrom/Toのいずれかが指定されている？
                    if ((_tanzakuPrintCriteria.印刷順From != null && _tanzakuPrintCriteria.印刷順From >= 0) ||
                        (_tanzakuPrintCriteria.印刷順To != null && _tanzakuPrintCriteria.印刷順To >= 0))
                    {
                        // -- 印刷順に該当する食事区分CDを検索条件にする
                        sql.AppendLine("     AND A.食事区分 IN (SELECT");
                        sql.AppendLine("                             食事区分ＣＤ");
                        sql.AppendLine("                         FROM");
                        sql.AppendLine("                             短冊集計設定マスタ");
                        sql.AppendLine("                         WHERE");
                        sql.AppendLine("                             企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                             AND 部署ＣＤ = @部署ＣＤ");

                        if (_tanzakuPrintCriteria.印刷順From != null && _tanzakuPrintCriteria.印刷順From >= 0)
                        {
                            sql.AppendLine("                                 AND 印刷順 >= @印刷順From");
                            cmd.Parameters.Add("@印刷順From", SqlDbType.Int).Value = _tanzakuPrintCriteria.印刷順From;
                        }

                        if (_tanzakuPrintCriteria.印刷順To != null && _tanzakuPrintCriteria.印刷順To >= 0)
                        {
                            sql.AppendLine("                                 AND 印刷順 <= @印刷順To");
                            cmd.Parameters.Add("@印刷順To", SqlDbType.Int).Value = _tanzakuPrintCriteria.印刷順To;
                        }
                        sql.AppendLine("                            )");
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat(_sortMapReprint[(string)cboSortKey.SelectedValue], rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tanzakuPrintCriteria.部署ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("短冊");
                    adp.Fill(dt);
                }

                // 袋数の列を追加
                dt.Columns.Add("袋数", typeof(string));

                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    if ((dr["袋計算フラグ"] as bool?) == true)
                    {
                        // 割るべき数として数量を取得
                        decimal wk_target = (decimal)dr["数量"];
                        int target = (int)wk_target;

                        string bagCnt = CalcBag(target);
                        dr["袋数"] = bagCnt;            // 求めた文字列を袋数に格納
                    }
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
        /// プレビュー画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formPreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                FormPreview form = (FormPreview)sender;
                form.Dispose();
                form = null;
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
        /// 袋数を算出
        /// </summary>
        /// <param name="suryo">数量</param>
        /// <returns>袋数の文字列</returns>
        private string CalcBag(int suryo)
        {
            try
            {
                StringBuilder result = new StringBuilder();
                string space = "";

                List<int> patternList = _tanzakuSettei.袋パターンリスト;    // 降順でソート済み

                // 袋パターンのうち、最小の袋サイズを取得
                int min = patternList[patternList.Count - 1];

                // 大きい袋から計算
                foreach (int i in patternList)
                {
                    int ans = suryo / i;   // 数量を袋サイズで割った商
                    int amari = suryo % i; // 数量を袋サイズで割った余り

                    // 数量が袋サイズ以上の場合のみ
                    if (ans > 0)
                    {
                        // 余りが0か、余りが最小パターンよりも大きい場合
                        if (amari == 0 || amari >= min)
                        {
                            result.Insert(0, string.Format("{0}人×{1}{2}", i, ans, space));
                            space = "  ";
                            suryo = suryo % i;
                        }
                        else if (ans - 1 > 0 && min != i)
                        {
                            // 余りが最小パターンを下回る場合は、自身の割る数を1減らして、次のパターンで計算を行う
                            result.Insert(0, string.Format("{0}人×{1}{2}", i, (ans - 1), space));
                            space = "  ";
                            suryo = (suryo % i) + i;
                        }
                    }
                }

                // 最終的に余りが出る場合は、最小パターンで切り上げる
                if (suryo > 0)
                {
                    int zan = (int)Math.Ceiling((decimal)suryo / (decimal)min);
                    result.Insert(0, string.Format("{0}人×{1}{2}", min, zan, space));
                }

                return result.ToString();
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
        /// 個口数を求める
        /// </summary>
        /// <param name="suryo">数量</param>
        /// <returns>数量が収まる個口数</returns>
        private int GetBoxes(decimal? suryo)
        {
            try
            {
                int boxes = 1;

                if ((suryo ?? 0m) <= 0m)
                {
                    return boxes;
                }

                int quantity = (int)suryo;

                // 取得した１個口当たりの数量が不正？
                if (_quantityPerBox <= 0)
                {
                    return boxes;
                }

                // 個口数を算出
                boxes = ((quantity - 1) / _quantityPerBox) + 1;

                return boxes;
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
                Form304E form = new Form304E((int)_tanzakuPrintCriteria.部署ＣＤ);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form304x_FormClosed);
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
        /// シール発注画面表示
        /// </summary>
        private void ShowOrder()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                Form304H form = new Form304H((int)_tanzakuPrintCriteria.部署ＣＤ, _tanzakuOrderMailList);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form304x_FormClosed);
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
        private void Form304x_FormClosed(object sender, FormClosedEventArgs e)
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

                Reload();       // 再読み込み
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            // 短冊設定マスタの袋パターンテキストを表示
            _tanzakuSettei = TanzakuSettei.GetTanzakuSettei(_tanzakuPrintCriteria.部署ＣＤ);
            if (_tanzakuSettei == null)
            {
                _tanzakuSettei = new TanzakuSettei();
            }
            tanzakuSetteiBindingSource.DataSource = _tanzakuSettei;

            SetSyukeiCombo();           // 集計区分コンボの選択肢を作成
            GetTanzakuSyukeiSettei();   // 部署別の集計設定を取得
            GetCourse();                // コース名変換設定を取得
        }

        #endregion
    }
}
