using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 売上精算入力
    /// </summary>
    public partial class Form403 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int URIAGE_SYOHIN_NAME_COLUMN = 0;        // 商品名列インデックス
        private const int URIAGE_KOSU_COLUMN        = 1;        // 個数列インデックス
        private const int URIAGE_URIAGE_COLUMN      = 2;        // 売上額列インデックス
        private const int URIAGE_NEBIKI_COLUMN      = 3;        // 値引額列インデックス

        private const int MOCHIDASHI_SYOHIN_NAME_COLUMN = 0;    // 商品名列インデックス
        private const int MOCHIDASHI_KOSU_COLUMN        = 1;    // 持ち出し数列インデックス
        private const int MOCHIDASHI_TAKE_KOJO_COLUMN   = 2;    // 工場追加列インデックス
        private const int MOCHIDASHI_TAKE_IPPAN_COLUMN  = 3;    // もらった列インデックス
        private const int MOCHIDASHI_GIVE_COLUMN        = 4;    // やった列インデックス
        private const int MOCHIDASHI_URIAGE_COLUMN      = 5;    // 売上数列インデックス
        private const int MOCHIDASHI_ZANSU_COLUMN       = 6;    // 残数列インデックス

        private const int TICKET_SALE_TOKUISAKI_CD_COLUMN   = 0;    // 得意先ＣＤ列インデックス
        private const int TICKET_SALE_TOKUISAKI_NAME_COLUMN = 1;    // 得意先名列インデックス
        private const int TICKET_SALE_PAY_COLUMN            = 2;    // 単価列インデックス
        private const int TICKET_SALE_TANKA_COLUMN          = 3;    // 単価列インデックス
        private const int TICKET_SALE_KAZU_COLUMN           = 4;    // 束数or枚数列インデックス
        private const int TICKET_SALE_URIAGE_COLUMN         = 5;    // 売上額列インデックス

        private const int TICKET_USE_TOKUISAKI_CD_COLUMN   = 0;     // 得意先ＣＤ列インデックス
        private const int TICKET_USE_TOKUISAKI_NAME_COLUMN = 1;     // 得意先名列インデックス
        private const int TICKET_USE_TANKA_COLUMN          = 2;     // 単価列インデックス
        private const int TICKET_USE_KAZU_COLUMN           = 3;     // 束数or枚数列インデックス
        private const int TICKET_USE_URIAGE_COLUMN         = 4;     // 売上額列インデックス

        private const int KAISYU_TOKUISAKI_CD_COLUMN   = 0;     // 得意先ＣＤ列インデックス
        private const int KAISYU_TOKUISAKI_NAME_COLUMN = 1;     // 得意先名列インデックス
        private const int KAISYU_KINGAKU_COLUMN        = 2;     // 金額列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 売上精算入力検索条件
        /// </summary>
        private UriageSeisanCriteria _uriageSeisanCriteria;

        /// <summary>
        /// 売上精算入力検索条件検索実行時スナップショット
        /// </summary>
        private UriageSeisanCriteria _uriageSeisanCriteriaSnapShot;

        /// <summary>
        /// 売上精算_ヘッダ
        /// </summary>
        private UriageSeisanHeader _uriageSeisanHeader;

        /// <summary>
        /// 初期状態用売上精算_ヘッダ
        /// </summary>
        private UriageSeisanHeader _origin = null;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="uriageSeisanCriteria">売上精算入力検索条件</param>
        public Form403(UriageSeisanCriteria uriageSeisanCriteria)
        {
            try
            {
                InitializeComponent();

                _uriageSeisanCriteria = new UriageSeisanCriteria(uriageSeisanCriteria);

                _IsNewRecord = true;

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
        private void Form403_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnDelete.Enabled = false;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // フォーカス制御用ダミーテキストをフォームの外に追いやって見えなくする
                // ※フォーカスを受け取りたいので、非表示にはできない
                txtFocusDummy.Left = this.Width;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                uriageSeisanBindingSource.DataSource = _uriageSeisanCriteria;

                // 未検索状態でも入力できるようにしておく
                _uriageSeisanHeader = new UriageSeisanHeader();
                uriageSeisanHeaderBindingSource.DataSource = _uriageSeisanHeader;

                // 検索実行（検索条件にエラーがある場合は無視される）
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
        private void Form403_Shown(object sender, EventArgs e)
        {
            // 精算日にフォーカス
            dtpSeisanbi.Focus();
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form403_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F9:
                        btnSave.PerformClick();        // 登録ボタン押下
                        break;

                    case Keys.F10:
                        btnPrint.PerformClick();        // 印刷ボタン押下
                        break;

                    case Keys.F11:                      // 削除
                        btnDelete.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.Focus();
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form403_FormClosing(object sender, FormClosingEventArgs e)
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

                // 初期状態と異なる？
                if (HasChangeUriageSeisanHeader())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // 売上精算_ヘッダの入力チェック
                            if (CanSaveUriageSeisanHeader() == false)
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
                    _uriageSeisanCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;
                    _uriageSeisanCriteria.コース名 = course.コース名;

                    _uriageSeisanCriteria.部署ＣＤ = course.部署ＣＤ;
                    ReadBinding(cboBusyo);      // コントロールに反映
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
        /// コースＣＤ検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // コースによって決まった部署ＣＤを表示
                // 以下の条件を全て満たすときにフォームを閉じると例外が発生するので、それを回避するためにItems.Countを判断している
                // ・売上精算一覧(=親画面)が閉じられている
                // ・コースＣＤにフォーカスが有る
                if (cboBusyo.Items.Count != 0)
                {
                    cboBusyo.SelectedValue = _uriageSeisanCriteria.部署ＣＤ ?? -1;
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_uriageSeisanCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_uriageSeisanCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _uriageSeisanCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 売上精算_ヘッダを取得
                string msg = GetUriageSeisanHeader();

                // 既存データがある？
                if (_IsNewRecord == false)
                {
                    // 現金合計＝売上合計－値引合計＋回収現金
                    _uriageSeisanHeader.現金合計 = (_uriageSeisanHeader.現金_売上合計 ?? 0m) - (_uriageSeisanHeader.現金_値引合計 ?? 0m) + (_uriageSeisanHeader.現金_回収現金 ?? 0m);

                    // 掛売合計＝売上合計－値引合計
                    // （未集金は売上合計に含まれている）
                    _uriageSeisanHeader.掛売合計 = (_uriageSeisanHeader.掛売_売上合計 ?? 0m) - (_uriageSeisanHeader.掛売_値引合計 ?? 0m);

                    // 電子決済合計＝売上合計－値引合計
                    _uriageSeisanHeader.E合計 = (_uriageSeisanHeader.E_売上合計 ?? 0m) - (_uriageSeisanHeader.E_値引合計 ?? 0m);

                    // 売上精算_売上明細を取得し、現金と売掛別にそれぞれのスプレッドに表示
                    GetUriageSeisanUriageMeisai();

                    // 売上精算_持出明細を取得し、持出数管理スプレッドに表示
                    GetUriageSeisanMochidashiMeisai();
                }
                else
                {
                    // 売上区分を問わず、全ての売上の値引の合計を取得
                    GetTotalNebiki();

                    // 回収現金を取得
                    GetKaisyuGenkin();

                    // 未集金を取得
                    GetMisyukin();

                    // 売上明細を取得し、現金と売掛と電子決済別にそれぞれのスプレッドに表示
                    GetUriageMeisai();

                    // 回収Tapionを取得
                    GetKaisyuTapion();

                    // チケット使用（チケット／ボーナス）を取得
                    // ※バークレーの300と200の枚数は入力
                    GetTicket();

                    // チケット束の売上金額の合計（現金のみ）を取得
                    GetTicketSaleTotalGenkin();

                    // チケット束の売上金額の合計（Tapionのみ）を取得
                    GetTicketSaleTotalTapion();

                    // チケット束の売上金額の合計（売上区分不問）を取得
                    GetTicketSaleTotal();

                    // 売上区分がチケットの売上を集計
                    GetTicketUseUriage();

                    // 得意先数を取得（画面には表示しない）
                    GetTokuisakiCount();

                    // 算出された現金合計を、金種計算用に本日現金合計にセット
                    _uriageSeisanHeader.現金_本日現金合計 = (_uriageSeisanHeader.現金合計 ?? 0m) + (_uriageSeisanHeader.現金_チケット束代 ?? 0m);

                    // 掛売の計算結果も格納
                    _uriageSeisanHeader.掛売_本日掛売合計 = (_uriageSeisanHeader.掛売合計 ?? 0m);

                    // 電子決済の計算結果も格納
                    _uriageSeisanHeader.E_本日E合計 = (_uriageSeisanHeader.E合計 ?? 0m) + (_uriageSeisanHeader.E_回収金額 ?? 0m) + (_uriageSeisanHeader.E_チケット束代 ?? 0m);

                    // 持出数管理スプレッドを表示
                    GetMochidashiMeisai();

                    // 合計を計算
                    _uriageSeisanHeader.CalcTotal();
                }

                // チケット束販売取得
                GetTicketSale();

                // 得意先チケット使用取得
                GetTicketUse();

                // 回収現金の明細を取得
                GetKaisyuGenkinList();

                // 回収Tapionの明細を取得
                GetKaisyuTapionList();

                if (_IsNewRecord)
                {
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                    btnDelete.Enabled = false;
                }
                else
                {
                    lblMode.Text = MODE_CAPTION_EDIT;
                    lblMode.BackColor = BACK_COLOR_EDIT;
                    btnSave.Text = "更新(F9)";
                    btnDelete.Enabled = this.HasAuthEdit;
                }

                // 売上精算_ヘッダを表示
                // 新規作成した売上精算_ヘッダに回収現金と未集金が設定されてから表示する
                uriageSeisanHeaderBindingSource.DataSource = _uriageSeisanHeader;

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _uriageSeisanCriteriaSnapShot = new UriageSeisanCriteria(_uriageSeisanCriteria);

                // 初期状態として保持
                _origin = new UriageSeisanHeader(_uriageSeisanHeader);

                // 入力データ一時保存から復元した？
                if (string.IsNullOrEmpty(msg) == false)
                {
                    MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// テキスト入力（検証）完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_Validated(object sender, EventArgs e)
        {
            try
            {
                // 合計を計算
                _uriageSeisanHeader.CalcTotal();

                // データソースを画面に反映
                uriageSeisanHeaderBindingSource.ResetBindings(false);
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
        /// ダミーテキスト Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFocusDummy_Enter(object sender, EventArgs e)
        {
            try
            {
                // *****************************************************************************************************************************
                // 領収書テキストから次のコントロールに移動しようとする場合、登録ボタンに行きたがるのを、10000円札枚数テキストに導くための仕組み
                // このコントロールにフォーカスが来るのは、領収書テキストでEnterまたはTabキーが押下された場合のみ。
                // 10000円札枚数にてShift＋Tabが押された場合は、タブコントロールにフォーカスが移る。
                // Enterイベントなどで別のコントロールにフォーカスを移すのは、
                // アプリケーションやオペレーティング システムが応答を停止することがあるので、本当はよくない。
                // *****************************************************************************************************************************

                txt10000Mai.Focus();    // 10000円札枚数テキストにフォーカスを移動する
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 担当者ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTantosyaCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTantosyaCD.PerformClick();
            }
        }

        /// <summary>
        /// 担当者ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTantosyaCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904TantosyaCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 担当者選択ダイアログクローズ（担当者ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904TantosyaCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtTantosyaCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtTantosyaCD.Text = tantousya.担当者ＣＤ.ToString();
                    _uriageSeisanHeader.配送担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    lblTantosyaName.Text = tantousya.担当者名;
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
        /// 登録ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 売上精算_ヘッダの入力チェック
                if (CanSaveUriageSeisanHeader() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                if (Save())
                {
                    // 再読み込み
                    Reload();
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
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_uriageSeisanCriteria.HasErrors)
                {
                    MessageBox.Show(this, _uriageSeisanCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_uriageSeisanCriteriaSnapShot != null)
                {
                    if (_uriageSeisanCriteria.Equals(_uriageSeisanCriteriaSnapShot) == false)
                    {
                        MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                // 未検索？
                if (_origin == null)
                {
                    MessageBox.Show(this, "印刷するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 未保存？
                if (_IsNewRecord)
                {
                    MessageBox.Show(this, "未保存のデータは印刷できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 変更あり？
                if (_uriageSeisanHeader.Equals(_origin) == false)
                {
                    MessageBox.Show(this, "変更されています。印刷する前に更新してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 帳票に該当するプリンタを取得。未選択ならダイアログを開く。そのあと印刷処理を実行
                DecidePrinter(Consts.Reports.UriageMeisaihyo, PrintMain);
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
        /// 削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (CanDelete() == false)
                {
                    return;
                }

                if (MessageBox.Show(this, "このデータを削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                if (Delete())
                {
                    // 未検索状態でも入力できるようにしておく
                    _uriageSeisanHeader = new UriageSeisanHeader();
                    uriageSeisanHeaderBindingSource.DataSource = _uriageSeisanHeader;
                    _origin = null;

                    // 各BindingSourceを初期化
                    genkinUriageBindingSource.DataSource = null;    // 現金売上
                    kakeuriUriageBindingSource.DataSource = null;   // 掛売売上
                    mochidashiBindingSource.DataSource = null;      // 持出明細
                    ticketSaleBindingSource.DataSource = null;      // チケット束販売
                    ticketUseBindingSource.DataSource = null;       // 得意先チケット使用

                    _IsNewRecord = true;
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                    btnDelete.Enabled = false;
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
        /// 売上精算_ヘッダ取得
        /// </summary>
        /// <returns>金種などを復元したことを示す文字列</returns>
        private string GetUriageSeisanHeader()
        {
            try
            {
                string msg = null;

                // 売上精算_ヘッダを取得
                _uriageSeisanHeader = UriageSeisanHeader.GetUriageSeisanHeader((DateTime)_uriageSeisanCriteria.精算日, (int)_uriageSeisanCriteria.部署ＣＤ, (int)_uriageSeisanCriteria.コースＣＤ);
                if (_uriageSeisanHeader == null)
                {
                    _IsNewRecord = true;        // 新規レコード

                    // 入力データ一時保存テーブルがあれば、その内容を採用する
                    UriageSeisanTemporary uriageSeisanTemporary = UriageSeisanTemporary.GetUriageSeisanTemporary((DateTime)_uriageSeisanCriteria.精算日, (int)_uriageSeisanCriteria.部署ＣＤ, (int)_uriageSeisanCriteria.コースＣＤ);
                    if (uriageSeisanTemporary != null)
                    {
                        msg = string.Format("{0:yyyy/MM/dd HH:mm} に削除した売上精算から金種枚数を復元しました。", uriageSeisanTemporary.作成日);
                        _uriageSeisanHeader = new UriageSeisanHeader(uriageSeisanTemporary);
                    }
                    else
                    {
                        // 存在しない場合は初期化
                        _uriageSeisanHeader = new UriageSeisanHeader();
                        _uriageSeisanHeader.精算日 = _uriageSeisanCriteria.精算日;
                        _uriageSeisanHeader.部署ＣＤ = _uriageSeisanCriteria.部署ＣＤ;
                        _uriageSeisanHeader.コースＣＤ = _uriageSeisanCriteria.コースＣＤ;
                    }

                    Busyo busyo = Busyo.GetBusyo((int)_uriageSeisanHeader.部署ＣＤ);
                    _uriageSeisanHeader.部署名 = (busyo != null) ? busyo.部署名 : null;
                    _uriageSeisanHeader.コース名 = _uriageSeisanCriteria.コース名;

                    // コースから取得した担当者を初期値として持つ
                    // 臨時コースマスタから優先して取得
                    RinjiCourse rinjiCourse = RinjiCourse.GetRinjiCourse((int)_uriageSeisanCriteria.コースＣＤ, (DateTime)_uriageSeisanCriteria.精算日);
                    if (rinjiCourse != null)
                    {
                        _uriageSeisanHeader.配送担当者ＣＤ = rinjiCourse.担当者ＣＤ;
                    }
                    else
                    {
                        Course course = Course.GetCourse((int)_uriageSeisanCriteria.コースＣＤ);
                        if (course != null)
                        {
                            _uriageSeisanHeader.配送担当者ＣＤ = course.担当者ＣＤ;
                        }
                    }
                }
                else
                {
                    _IsNewRecord = false;
                }

                return msg;
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
        /// 売上精算_売上明細取得
        /// </summary>
        private void GetUriageSeisanUriageMeisai()
        {
            try
            {
                // 現金売上を取得
                GetUriageSeisanUriageMeisaiMain(Consts.UriageKubun.Genkin, "現金売上", genkinUriageBindingSource, fpSpread_Genkin_Sheet1);

                // 掛売売上を取得
                GetUriageSeisanUriageMeisaiMain(Consts.UriageKubun.Urikake, "掛売売上", kakeuriUriageBindingSource, fpSpread_Kakeuri_Sheet1);

                // 電子決済売上を取得
                GetUriageSeisanUriageMeisaiMain(Consts.UriageKubun.Line, "E売上", payUriageBindingSource, fpSpread_Pay_Sheet1);
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
        /// 売上精算_売上明細取得メイン
        /// </summary>
        /// <param name="uriageKubun">売上区分</param>
        /// <param name="tableName">データテーブル名</param>
        /// <param name="bindingSource">バインド先バインディングソース</param>
        /// <param name="fpSpread1_Sheet">表示先シート</param>
        private void GetUriageSeisanUriageMeisaiMain(Consts.UriageKubun uriageKubun, string tableName, BindingSource bindingSource, FarPoint.Win.Spread.SheetView fpSpread1_Sheet)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.精算日" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.売上区分" +
                                      "\r\n     ,A.商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,A.売上数量" +
                                      "\r\n     ,A.売上金額" +
                                      "\r\n     ,A.値引額" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     売上精算_売上明細 AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                                      "\r\n AND A.精算日     = @精算日" +
                                      "\r\n AND A.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n AND A.コースＣＤ = @コースＣＤ" +
                                      "\r\n AND A.売上区分   = @売上区分" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = (int)uriageKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable(tableName);
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource.DataSource = ds;
                    bindingSource.DataMember = tableName;

                    log.Info("売上精算_売上明細を検索しました。");
                }

                fpSpread1_Sheet.BindDataColumn(URIAGE_SYOHIN_NAME_COLUMN, "商品名略称");
                fpSpread1_Sheet.BindDataColumn(URIAGE_KOSU_COLUMN,        "売上数量");
                fpSpread1_Sheet.BindDataColumn(URIAGE_URIAGE_COLUMN,      "売上金額");
                fpSpread1_Sheet.BindDataColumn(URIAGE_NEBIKI_COLUMN,      "値引額");
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
        /// 売上精算_持出明細取得
        /// </summary>
        private void GetUriageSeisanMochidashiMeisai()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.精算日" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,A.持出個数" +
                                      "\r\n     ,A.工場追加数" +
                                      "\r\n     ,A.もらった数" +
                                      "\r\n     ,A.やった数" +
                                      "\r\n     ,A.残数" +
                                      "\r\n     ,A.売上予定実績数" +
                                      "\r\n     ,A.ボーナス消費数" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     売上精算_持出明細 AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                                      "\r\n AND A.精算日     = @精算日" +
                                      "\r\n AND A.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n AND A.コースＣＤ = @コースＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("持出明細");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    mochidashiBindingSource.DataSource = ds;
                    mochidashiBindingSource.DataMember = "持出明細";

                    log.Info("売上精算_持出明細を検索しました。");
                }

                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_SYOHIN_NAME_COLUMN, "商品名略称");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_KOSU_COLUMN,        "持出個数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_TAKE_KOJO_COLUMN,   "工場追加数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_TAKE_IPPAN_COLUMN,  "もらった数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_GIVE_COLUMN,        "やった数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_URIAGE_COLUMN,      "売上予定実績数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_ZANSU_COLUMN,       "残数");
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
        /// 売上明細取得
        /// </summary>
        private void GetUriageMeisai()
        {
            try
            {
                // 現金売上を取得
                GetUriageMeisaiMain(Consts.UriageKubun.Genkin, "現金売上", genkinUriageBindingSource, fpSpread_Genkin_Sheet1);

                // 掛売売上を取得
                GetUriageMeisaiMain(Consts.UriageKubun.Urikake, "掛売売上", kakeuriUriageBindingSource, fpSpread_Kakeuri_Sheet1);

                // 電子決済売上を取得
                GetUriageMeisaiMain(Consts.UriageKubun.Line, "E売上", payUriageBindingSource, fpSpread_Pay_Sheet1);
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
        /// 売上明細取得メイン
        /// </summary>
        /// <param name="uriageKubun">売上区分</param>
        /// <param name="tableName">データテーブル名</param>
        /// <param name="bindingSource">バインド先バインディングソース</param>
        /// <param name="fpSpread1_Sheet">表示先シート</param>
        private void GetUriageMeisaiMain(Consts.UriageKubun uriageKubun, string tableName, BindingSource bindingSource, FarPoint.Win.Spread.SheetView fpSpread1_Sheet)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     M.企業ＣＤ" +
                                      "\r\n     ,M.売上日 AS 精算日" +
                                      "\r\n     ,M.部署ＣＤ" +
                                      "\r\n     ,H.コースＣＤ" +
                                      "\r\n     ,M.売上区分" +
                                      "\r\n     ,M.商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,ISNULL(SUM(M.商品数量), 0)         AS 売上数量" +
                                      "\r\n     ,ISNULL(SUM(M.値引額), 0)           AS 値引額" +
                                      "\r\n     ,ISNULL(SUM(M.値引前金額), 0)       AS 売上金額" +
                                      "\r\n     ,ISNULL(SUM(M.値引後金額), 0)       AS 値引後金額" +
                                      "\r\n     ,ISNULL(SUM(M.消費税額), 0)         AS 消費税額" +
                                      "\r\n     ,ISNULL(SUM(M.商品金額合計税込), 0) AS 商品金額合計税込" +
                                      "\r\n     ,ISNULL(SUM(CASE WHEN S.弁当区分 IN (@弁当区分弁当, @弁当区分副食) THEN M.商品数量 ELSE 0 END), 0) AS 食数" +
                                      "\r\n     ,NULL AS 作成担当者ＣＤ" +
                                      "\r\n     ,NULL AS 作成担当者名" +
                                      "\r\n     ,NULL AS 作成日" +
                                      "\r\n     ,NULL AS 修正担当者ＣＤ" +
                                      "\r\n     ,NULL AS 修正担当者名" +
                                      "\r\n     ,NULL AS 修正日" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n     INNER JOIN 売上明細 AS M " +
                                      "\r\n         ON  M.企業ＣＤ   = H.企業ＣＤ " +
                                      "\r\n         AND M.売上日     = H.売上日 " +
                                      "\r\n         AND M.得意先ＣＤ = H.得意先ＣＤ " +
                                      "\r\n         AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S " +
                                      "\r\n         ON  S.企業ＣＤ = M.企業ＣＤ " +
                                      "\r\n         AND S.商品ＣＤ = M.商品ＣＤ" +
                                      "\r\n     INNER JOIN 得意先マスタ AS T " +
                                      "\r\n         ON  T.企業ＣＤ   = H.企業ＣＤ " +
                                      "\r\n         AND T.得意先ＣＤ = H.得意先ＣＤ " +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n     AND H.売上日     = @精算日" +
                                      "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND M.売上区分   = @売上区分" +
                                      "\r\n     AND ISNULL(M.チケット束フラグ, 0) = 0" +        // --チケット束売りは除く
                                      "\r\n GROUP BY" +
                                      "\r\n     M.企業ＣＤ" +
                                      "\r\n     ,M.売上日" +
                                      "\r\n     ,M.部署ＣＤ" +
                                      "\r\n     ,H.コースＣＤ" +
                                      "\r\n     ,M.売上区分" +
                                      "\r\n     ,M.商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n ORDER BY" +
                                      "\r\n     M.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = (int)uriageKubun;
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable(tableName);
                    adp.Fill(dt);

                    // 売上合計と値引合計と食数合計を取得し、売上精算_ヘッダにセット
                    object totalKingaku = dt.Compute("SUM(商品金額合計税込)", "");
                    decimal? totalKingakuValue = null;
                    if (totalKingaku != null && string.IsNullOrEmpty(totalKingaku.ToString()) == false)
                    {
                        totalKingakuValue = decimal.Parse(totalKingaku.ToString());
                    }
                    object totalNebiki = dt.Compute("SUM(値引額)", "");
                    decimal? totalNebikiValue = null;
                    if (totalNebiki != null && string.IsNullOrEmpty(totalNebiki.ToString()) == false)
                    {
                        totalNebikiValue = decimal.Parse(totalNebiki.ToString());
                    }
                    object totalSyokusu = dt.Compute("SUM(食数)", "");
                    decimal? totalSyokusuValue = null;
                    if (totalSyokusu != null && string.IsNullOrEmpty(totalSyokusu.ToString()) == false)
                    {
                        totalSyokusuValue = decimal.Parse(totalSyokusu.ToString());
                    }

                    //***********************************************************************************************************************************************
                    // 画面に表示する現金合計は売上合計－値引合計＋回収現金で求まるが、
                    // この場合、売上合計は値引前金額の合計でなければならない。（売上合計が値引後金額の合計だと二重に値引額を引くことになる）
                    // さらに求まった現金合計は消費税込みでなければならない。（現金合計＋現金_チケット束代が本日現金入金合計と一致していないといけないので）
                    //
                    // つまり、おおもとの売上合計も消費税込みでなければならないが、前述の通り売上合計は値引前金額の合計なので、消費税込みにはなっているとは限らない。
                    // （外税の場合は値引前金額ではなく値引後金額に税率を掛けたものが商品金額合計税込に格納される）
                    // したがって、(外税込みである)現金合計を、売上合計－値引合計＋回収現金で求めることができない。
                    //
                    // そこで、現金合計を売上合計と値引合計と回収現金から求めるのではなく、売上合計を現金合計と値引合計と回収現金から求めるようにする。
                    // こうすると、商品金額合計税込の合計＋回収現金が現金合計になり、回収現金を引いて値引合計を加えると、売上合計が求まる。
                    // （掛売の場合は売上合計に含まれている。そのため、掛売合計の計算では考慮しない）
                    //***********************************************************************************************************************************************

                    switch (uriageKubun)
                    {
                        case Consts.UriageKubun.Genkin:     // 現金
                            _uriageSeisanHeader.現金合計 = (totalKingakuValue ?? 0m) + (_uriageSeisanHeader.現金_回収現金 ?? 0m);
                            _uriageSeisanHeader.現金_値引合計 = totalNebikiValue;
                            _uriageSeisanHeader.現金_売上合計 = (_uriageSeisanHeader.現金合計 ?? 0m) - (_uriageSeisanHeader.現金_回収現金 ?? 0m) + (_uriageSeisanHeader.現金_値引合計 ?? 0m);
                            _uriageSeisanHeader.現金_食数 = totalSyokusuValue;
                            break;

                        case Consts.UriageKubun.Urikake:    // 売掛（未集金は売上合計に含まれている）
                            _uriageSeisanHeader.掛売合計 = (totalKingakuValue ?? 0m);
                            _uriageSeisanHeader.掛売_値引合計 = totalNebikiValue;
                            _uriageSeisanHeader.掛売_売上合計 = (_uriageSeisanHeader.掛売合計 ?? 0m) + (_uriageSeisanHeader.掛売_値引合計 ?? 0m);
                            _uriageSeisanHeader.掛売_食数 = totalSyokusuValue;
                            break;

                        case Consts.UriageKubun.Line:      // LINE決済
                            _uriageSeisanHeader.E合計 = (totalKingakuValue ?? 0m);
                            _uriageSeisanHeader.E_値引合計 = totalNebikiValue;
                            _uriageSeisanHeader.E_売上合計 = (_uriageSeisanHeader.E合計 ?? 0m) + (_uriageSeisanHeader.E_値引合計 ?? 0m);
                            _uriageSeisanHeader.E_食数 = totalSyokusuValue;
                            break;
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource.DataSource = ds;
                    bindingSource.DataMember = tableName;

                    log.Info("売上明細を検索しました。");
                }

                fpSpread1_Sheet.BindDataColumn(URIAGE_SYOHIN_NAME_COLUMN, "商品名略称");
                fpSpread1_Sheet.BindDataColumn(URIAGE_KOSU_COLUMN,        "売上数量");
                fpSpread1_Sheet.BindDataColumn(URIAGE_URIAGE_COLUMN,      "売上金額");
                fpSpread1_Sheet.BindDataColumn(URIAGE_NEBIKI_COLUMN,      "値引額");
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
        /// 売上区分を問わず、全ての売上の値引の合計を取得
        /// </summary>
        private void GetTotalNebiki()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     ISNULL(SUM(M.値引額), 0) AS 値引額" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n     INNER JOIN 売上明細 AS M" +
                                      "\r\n         ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n         AND M.売上日     = H.売上日" +
                                      "\r\n         AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n         AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n     AND H.売上日     = @精算日" +
                                      "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      //"\r\n     AND (M.売上区分 <> @売上区分現金 OR ISNULL(M.チケット束フラグ, 0) = 0)";    // -- 現金のチケット束売りの値引きを除く（現金ではないか、(現金でも)チケット束売りではないものを抽出）(2019/12/25)
                                      // -- 現金のチケット束売りの値引きとバークレーの値引きを除く（現金ではないか、(現金でも)チケット束売りではないもの かつ バークレーでもないものを抽出）(2020/01/07)
                                      // "\r\n     AND ((M.売上区分 <> @売上区分現金 OR ISNULL(M.チケット束フラグ, 0) = 0) AND M.売上区分 <> @売上区分バークレー)";
                                      // -- 以下の値引きを除く
                                      // -- ・現金のチケット束売りの値引き
                                      // -- ・Tapionのチケット束売りの値引き
                                      // -- ・バークレーの値引き
                                      // -- （現金でもTapionでもないか、(現金またはTapionでも)チケット束売りではないもの かつ バークレーでもないものを抽出）
                                      "\r\n     AND ((M.売上区分 NOT IN (@売上区分現金, @売上区分LINE) OR ISNULL(M.チケット束フラグ, 0) = 0) AND M.売上区分 <> @売上区分バークレー)";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分現金", SqlDbType.Int).Value = (int)Consts.UriageKubun.Genkin;
                    cmd.Parameters.Add("@売上区分バークレー", SqlDbType.Int).Value = (int)Consts.UriageKubun.Barclay;
                    cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        _uriageSeisanHeader.合計_本日総値引 = (decimal)reader["値引額"];
                    }

                    log.Info("値引額の合計を検索しました。");
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
        /// 入金明細から回収現金を取得
        /// </summary>
        private void GetKaisyuGenkin()
        {
            try
            {
                DataTable dt = GetKaisyuMain(Consts.NyukinHoho.Genkin, "回収現金");

                decimal kaisyuKingaku = 0m;
                int kaisyuKingakuKensu = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    // 入金額の合計を算出
                    object value = dr["入金額"];
                    if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                    {
                        kaisyuKingaku += decimal.Parse(value.ToString());
                        kaisyuKingakuKensu++;
                    }
                }
                _uriageSeisanHeader.現金_回収現金 = kaisyuKingaku;
                _uriageSeisanHeader.現金_回収現金件数 = kaisyuKingakuKensu;
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
        /// 入金明細から回収Tapionを取得
        /// </summary>
        private void GetKaisyuTapion()
        {
            try
            {
                DataTable dt = GetKaisyuMain(Consts.NyukinHoho.Tapion, "回収Tapion");

                decimal kaisyuKingaku = 0m;
                int kaisyuKingakuKensu = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    // 入金額の合計を算出
                    object value = dr["入金額"];
                    if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                    {
                        kaisyuKingaku += decimal.Parse(value.ToString());
                        kaisyuKingakuKensu++;
                    }
                }
                _uriageSeisanHeader.E_回収金額 = kaisyuKingaku;
                _uriageSeisanHeader.E_回収金額件数 = kaisyuKingakuKensu;
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
        /// 入金明細から回収金額を取得
        /// </summary>
        /// <param name="nyukinHoho">入金方法</param>
        /// <param name="tableName">データテーブルに付ける名前</param>
        /// <returns>入金明細から取得した回収金額が格納されたデータテーブル</returns>
        private DataTable GetKaisyuMain(Consts.NyukinHoho nyukinHoho, string tableName)
        {
            try
            {
                DataTable dt = new DataTable(tableName);
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // 入金入力区分=1(モバイル)＆入金方法=現金（またはTapion）の入金明細の入金額の合計
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     N.得意先ＣＤ" +
                                      "\r\n     ,T.得意先名" +
                                      "\r\n     ,ISNULL(SUM(N.入金額), 0) AS 入金額" +
                                      "\r\n FROM" +
                                      // -- コースに紐付く得意先の入金を取得する
                                      // -- 分配した場合は、基本的に分配先はコースに属していないので、コースに属している分配元(=受注得意先)を経由して入金を取得する
                                      "\r\n     (SELECT DISTINCT" +
                                      "\r\n         T.企業ＣＤ" +
                                      "\r\n         ,T.得意先ＣＤ" +
                                      "\r\n     FROM" +
                                      // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                      "\r\n         (SELECT" +
                                      "\r\n             CT.企業ＣＤ" +
                                      "\r\n             ,CT.得意先ＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             コーステーブル AS CT" +
                                      "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             LEFT JOIN" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     コースＣＤ" +
                                      "\r\n                 FROM" +
                                      "\r\n                     臨時コースマスタ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                     AND 実施日 = @精算日" +
                                      "\r\n                 ) AS RM" +
                                      "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND CT.コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                                      "\r\n             AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n         UNION" +
                                      "\r\n         SELECT" +
                                      "\r\n             RT.企業ＣＤ" +
                                      "\r\n             ,RT.得意先ＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             臨時コーステーブル AS RT" +
                                      "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n         WHERE" +
                                      "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND RT.コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n             AND RT.実施日 = @精算日" +
                                      "\r\n         ) AS CT" +
                                      "\r\n         INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ) = CT.得意先ＣＤ" +
                                      "\r\n     ) AS JT" +
                                      "\r\n     INNER JOIN 入金明細 AS N ON N.企業ＣＤ = JT.企業ＣＤ AND N.得意先ＣＤ = JT.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = N.企業ＣＤ AND T.得意先ＣＤ = N.得意先ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     N.入金日 = @精算日" +
                                      "\r\n     AND N.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND N.入金入力区分 = @入金入力区分モバイル" +
                                      "\r\n     AND N.入金方法 = @入金方法" +
                                      "\r\n     AND ISNULL(N.入金額, 0) <> 0" +
                                      "\r\n GROUP BY" +
                                      "\r\n     N.得意先ＣＤ" +
                                      "\r\n     ,T.得意先名" +
                                      "\r\n ORDER BY" +
                                      "\r\n     N.得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@入金入力区分モバイル", SqlDbType.Int).Value = (int)Consts.NyukinNyuryokuKubun.Mobile;
                    cmd.Parameters.Add("@入金方法", SqlDbType.Int).Value = (int)nyukinHoho;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.InfoFormat("入金明細({0})を検索しました。", tableName);
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
        /// 売上明細から未集金を取得
        /// </summary>
        private void GetMisyukin()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // 現金の得意先 かつ 掛売の売上明細の金額の合計
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     M.得意先ＣＤ" +
                                      "\r\n     ,ISNULL(SUM(M.商品金額合計税込), 0) AS 商品金額合計税込" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n     INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = H.企業ＣＤ AND T.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n     INNER JOIN 売上明細 AS M ON M.企業ＣＤ = H.企業ＣＤ AND M.売上日 = H.売上日 AND M.得意先ＣＤ = H.得意先ＣＤ AND M.部署ＣＤ = H.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n     AND H.売上日     = @精算日" +
                                      "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND ISNULL(T.売掛現金区分, 0) = @売掛現金区分現金" +
                                      "\r\n     AND M.売上区分   = @売上区分売掛" +
                                      "\r\n GROUP BY" +
                                      "\r\n     M.得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    adp.Fill(dt);

                    decimal misyukin = 0m;
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 商品金額合計税込の合計を算出
                        object value = dr["商品金額合計税込"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            misyukin += decimal.Parse(value.ToString());
                        }
                    }
                    _uriageSeisanHeader.掛売_未集金 = misyukin;

                    log.Info("売上明細(未集金)を検索しました。");
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
        /// 売上明細からチケットとボーナスとバークレーの件数と金額を取得
        /// </summary>
        private void GetTicket()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // ┌─────┬─────┐
                    // │チケット  │枚数と金額│
                    // ├─────┼─────┤
                    // │バークレー│金額のみ  │
                    // ├─────┼─────┤
                    // │ボーナス  │枚数だけ  │
                    // └─────┴─────┘
                    // 値引額については、GetTotalNebiki()で売上区分を問わず全ての売上の値引の合計を取得したあと、
                    // 実質売上(合計_実質総売上)計算時に値引額を減算しているため、ここでは値引前の金額（＝値引額を加算した金額）を求める(2019/12/04)
                    // としていたが、バークレーには値引後の金額を表示することにする。
                    // したがって、GetTotalNebiki()では売上の値引の合計を求めているが、ここからバークレーの値引額を除外する(2020/01/07)
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     ISNULL(SUM(CASE WHEN M.売上区分 = @売上区分チケット   THEN M.商品数量                               ELSE 0 END), 0) AS チケット枚数" +
                                      "\r\n    ,ISNULL(SUM(CASE WHEN M.売上区分 = @売上区分チケット   THEN M.商品金額合計税込 + ISNULL(M.値引額, 0) ELSE 0 END), 0) AS チケット金額" +
                                      "\r\n    ,ISNULL(SUM(CASE WHEN M.売上区分 = @売上区分バークレー THEN M.商品金額合計税込                       ELSE 0 END), 0) AS バークレー金額" +
                                      "\r\n    ,ISNULL(SUM(CASE WHEN M.売上区分 = @売上区分ボーナス   THEN M.商品数量                               ELSE 0 END), 0) AS ボーナス枚数" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n     INNER JOIN 売上明細 AS M" +
                                      "\r\n         ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n         AND M.売上日     = H.売上日" +
                                      "\r\n         AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n         AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n     AND H.売上日     = @精算日" +
                                      "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND M.売上区分 IN(@売上区分チケット, @売上区分バークレー, @売上区分ボーナス)";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;
                    cmd.Parameters.Add("@売上区分バークレー", SqlDbType.Int).Value = (int)Consts.UriageKubun.Barclay;
                    cmd.Parameters.Add("@売上区分ボーナス", SqlDbType.Int).Value = (int)Consts.UriageKubun.Bonus;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        object value = reader["チケット枚数"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            _uriageSeisanHeader.消費チケ枚数_チケット = int.Parse(value.ToString());
                        }

                        value = reader["チケット金額"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            _uriageSeisanHeader.消費チケ金額_チケット = decimal.Parse(value.ToString());
                        }

                        value = reader["バークレー金額"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            _uriageSeisanHeader.合計_バークレー売上 = int.Parse(value.ToString());
                        }

                        value = reader["ボーナス枚数"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            _uriageSeisanHeader.消費チケ枚数_ボーナス = int.Parse(value.ToString());
                        }
                    }

                    log.Info("売上明細(使用チケット)を検索しました。");
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
        /// チケット束の売上金額の合計（現金のみ）を取得
        /// </summary>
        private void GetTicketSaleTotalGenkin()
        {
            try
            {
                decimal value = GetTicketSaleTotalByKubun(Consts.UriageKubun.Genkin);
                _uriageSeisanHeader.現金_チケット束代 = value;
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
        /// チケット束の売上金額の合計（Tapionのみ）を取得
        /// </summary>
        private void GetTicketSaleTotalTapion()
        {
            try
            {
                decimal value = GetTicketSaleTotalByKubun(Consts.UriageKubun.Line);
                _uriageSeisanHeader.E_チケット束代 = value;
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
        /// 売上区分によるチケット束の売上金額の合計を取得
        /// </summary>
        /// <param name="uriageKubun">売上区分</param>
        /// <returns>指定した売上区分の売上明細から取得したチケット束の売上金額の合計</returns>
        private decimal GetTicketSaleTotalByKubun(Consts.UriageKubun uriageKubun)
        {
            try
            {
                decimal kingaku = 0m;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // チケット束フラグが１の売上明細の合計を取得
                    // 値引額については、GetTotalNebiki()で売上区分を問わず全ての売上の値引の合計を取得したあと、
                    // 実質売上(合計_実質総売上)計算時に値引額を減算しているため、ここでは値引前の金額（＝値引額を加算した金額）を求める(2019/12/04)
                    // としていたが、現金_チケット束代には値引後の金額を表示することにする。
                    // したがって、GetTotalNebiki()では売上の値引の合計を求めているが、ここから現金_チケット束代の値引額を除外する(2019/12/25)
                    // E_チケット束代も値引後の金額を表示するので、GetTotalNebiki()からE_チケット束代の値引額を除外する(2024/12/05)
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     ISNULL(SUM(M.商品金額合計税込), 0) AS 商品金額合計税込" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n     INNER JOIN 売上明細 AS M" +
                                      "\r\n         ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n         AND M.売上日     = H.売上日" +
                                      "\r\n         AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n         AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n     AND H.売上日     = @精算日" +
                                      "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND ISNULL(M.チケット束フラグ, 0) <> 0" +
                                      "\r\n     AND M.売上区分 = @売上区分";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = (int)uriageKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        object value = reader["商品金額合計税込"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            //_uriageSeisanHeader.合計_チケット売上 = int.Parse(value.ToString());
                            // _uriageSeisanHeader.現金_チケット束代 = decimal.Parse(value.ToString());
                            kingaku = decimal.Parse(value.ToString());
                        }
                    }

                    log.InfoFormat("チケット束({0})の売上金額の合計を取得しました。", uriageKubun);
                }

                return kingaku;
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
        /// チケット束の売上金額の合計（売上区分不問）を取得
        /// </summary>
        private void GetTicketSaleTotal()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // チケット束フラグが１の売上明細の合計を取得
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     ISNULL(SUM(M.商品数量), 0) AS 商品数量" +
                                      "\r\n     ,ISNULL(SUM(M.商品金額合計税込), 0) AS 商品金額合計税込" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n     INNER JOIN 売上明細 AS M" +
                                      "\r\n         ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n         AND M.売上日     = H.売上日" +
                                      "\r\n         AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n         AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n     AND H.売上日     = @精算日" +
                                      "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND ISNULL(M.チケット束フラグ, 0) <> 0";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        object value = reader["商品数量"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            _uriageSeisanHeader.チケット束_売上枚数 = int.Parse(value.ToString());
                        }
                        value = reader["商品金額合計税込"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            _uriageSeisanHeader.チケット束_売上金額 = decimal.Parse(value.ToString());
                        }
                    }

                    log.Info("チケット束の売上金額の合計を取得しました。");
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
        /// 売上区分がチケットの売上を集計
        /// </summary>
        private void GetTicketUseUriage()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     ISNULL(SUM(M.商品金額合計税込), 0) AS 商品金額合計税込" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n     INNER JOIN 売上明細 AS M" +
                                      "\r\n         ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n         AND M.売上日     = H.売上日" +
                                      "\r\n         AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n         AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n     AND H.売上日     = @精算日" +
                                      "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND M.売上区分   = @売上区分チケット";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        object value = reader["商品金額合計税込"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            _uriageSeisanHeader.合計_チケット売上 = int.Parse(value.ToString());
                        }
                    }

                    log.Info("売上区分がチケットの売上を集計しました。");
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
        /// 販売実績がある得意先の件数を取得
        /// </summary>
        private void GetTokuisakiCount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     COUNT(得意先ＣＤ) AS 得意先数" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n     AND H.売上日     = @精算日" +
                                      "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        object value = reader["得意先数"];
                        if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                        {
                            _uriageSeisanHeader.得意先数 = int.Parse(value.ToString());
                        }
                    }

                    log.Info("販売実績がある得意先の件数を取得しました。");
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
        /// 持出明細取得
        /// </summary>
        private void GetMochidashiMeisai()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     MAIN.企業ＣＤ," +
                                      "\r\n     NULL AS 精算日," +
                                      "\r\n     NULL AS 部署ＣＤ," +
                                      "\r\n     MAIN.コースＣＤ," +
                                      "\r\n     MAIN.商品ＣＤ," +
                                      "\r\n     MAIN.商品名略称," +
                                      "\r\n     MCDS.個数 AS 持出個数," +
                                      "\r\n     TAKE_KOJO.個数 AS 工場追加数," +
                                      "\r\n     TAKE_IPPAN.個数 AS もらった数," +
                                      "\r\n     GIVE.個数 AS やった数," +
                                      // -- 持出の個数＋工場から受け取った個数＋工場以外から受け取った個数－渡した個数－売上数 を残数とする
                                      "\r\n     ISNULL(MCDS.個数, 0) +" +
                                      "\r\n     ISNULL(TAKE_KOJO.個数, 0) +" +
                                      "\r\n     ISNULL(TAKE_IPPAN.個数, 0) -" +
                                      "\r\n     ISNULL(GIVE.個数, 0) -" +
                                      "\r\n     ISNULL(INIT.売上予定実績数, 0)" +
                                      "\r\n     AS 残数," +
                                      "\r\n     INIT.売上予定実績数," +
                                      "\r\n     INIT.ボーナス消費数," +
                                      "\r\n     NULL AS 作成担当者ＣＤ," +
                                      "\r\n     NULL AS 作成担当者名," +
                                      "\r\n     NULL AS 作成日," +
                                      "\r\n     NULL AS 修正担当者ＣＤ," +
                                      "\r\n     NULL AS 修正担当者名," +
                                      "\r\n     NULL AS 修正日" +
                                      "\r\n FROM" +
                                      // -- ①駆動表（コースに該当する商品）
                                      "\r\n     (SELECT" +
                                      "\r\n         CM.企業ＣＤ," +
                                      "\r\n         CM.コースＣＤ," +
                                      "\r\n         CM.コース名," +
                                      "\r\n         P.商品ＣＤ," +
                                      "\r\n         S.商品名略称" +
                                      "\r\n     FROM" +
                                      "\r\n         コースマスタ AS CM" +
                                      "\r\n         INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ" +
                                      "\r\n         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND CM.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n     ) AS MAIN" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ②売上の部品（各商品を分割した部品ごとに合計する）
                                      "\r\n     (SELECT" +
                                      "\r\n         PART.コースＣＤ," +
                                      "\r\n         PART.部品ＣＤ," +
                                      "\r\n         SUM(PART.売上予定実績数) AS 売上予定実績数," +
                                      "\r\n         SUM(PART.ボーナス) AS ボーナス消費数" +
                                      "\r\n     FROM" +
                                      // -- ②－１ 売上の主食
                                      "\r\n         (SELECT" +
                                      "\r\n             CM.コースＣＤ," +
                                      "\r\n             D.主食ＣＤ AS 部品ＣＤ," +
                                      "\r\n             SUM(D.商品数量) AS 売上予定実績数," +
                                      "\r\n             SUM(CASE WHEN D.売上区分  = @売上区分ボーナス THEN D.商品数量 ELSE 0 END) AS ボーナス" +
                                      "\r\n         FROM" +
                                      "\r\n             コースマスタ AS CM" +
                                      "\r\n             INNER JOIN 売上ヘッダ AS H" +
                                      "\r\n                 ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                 AND H.売上日     = @精算日" +
                                      "\r\n                 AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                 AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n             INNER JOIN 売上明細 AS D" +
                                      "\r\n                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                 AND D.売上日     = H.売上日" +
                                      "\r\n                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             AND ISNULL(D.主食ＣＤ, 0) <> 0" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             CM.コースＣＤ," +
                                      "\r\n             D.主食ＣＤ" +
                                      "\r\n         UNION" +
                                      // -- ②－２ 売上の副食
                                      "\r\n         SELECT" +
                                      "\r\n             CM.コースＣＤ," +
                                      "\r\n             D.副食ＣＤ AS 部品ＣＤ," +
                                      "\r\n             SUM(D.商品数量) AS 売上予定実績数," +
                                      "\r\n             SUM(CASE WHEN D.売上区分  = @売上区分ボーナス THEN D.商品数量 ELSE 0 END) AS ボーナス" +
                                      "\r\n         FROM" +
                                      "\r\n             コースマスタ AS CM" +
                                      "\r\n             INNER JOIN 売上ヘッダ AS H" +
                                      "\r\n                 ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                 AND H.売上日     = @精算日" +
                                      "\r\n                 AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                 AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n             INNER JOIN 売上明細 AS D" +
                                      "\r\n                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                 AND D.売上日     = H.売上日" +
                                      "\r\n                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             AND ISNULL(D.副食ＣＤ, 0) <> 0" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             CM.コースＣＤ," +
                                      "\r\n             D.副食ＣＤ" +
                                      "\r\n         UNION" +
                                      // -- ②－３ 売上の主食でも副食でもないもの
                                      "\r\n         SELECT" +
                                      "\r\n             CM.コースＣＤ," +
                                      "\r\n             D.商品ＣＤ AS 部品ＣＤ," +
                                      "\r\n             SUM(D.商品数量) AS 売上予定実績数," +
                                      "\r\n             SUM(CASE WHEN D.売上区分  = @売上区分ボーナス THEN D.商品数量 ELSE 0 END) AS ボーナス" +
                                      "\r\n         FROM" +
                                      "\r\n             コースマスタ AS CM" +
                                      "\r\n             INNER JOIN 売上ヘッダ AS H" +
                                      "\r\n                 ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                 AND H.売上日     = @精算日" +
                                      "\r\n                 AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                 AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n             INNER JOIN 売上明細 AS D" +
                                      "\r\n                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                 AND D.売上日     = H.売上日" +
                                      "\r\n                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             AND ISNULL(D.主食ＣＤ, 0) = 0" +
                                      "\r\n             AND ISNULL(D.副食ＣＤ, 0) = 0" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             CM.コースＣＤ," +
                                      "\r\n             D.商品ＣＤ" +
                                      "\r\n         ) AS PART" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         PART.コースＣＤ," +
                                      "\r\n         PART.部品ＣＤ" +
                                      "\r\n     ) AS INIT ON MAIN.コースＣＤ = INIT.コースＣＤ AND MAIN.商品ＣＤ = INIT.部品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ③持ち出し数
                                      "\r\n     (SELECT" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n         ,SUM(個数) AS 個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_持ち出し入力" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n         持出日 = @精算日" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n     ) AS MCDS ON MAIN.コースＣＤ = MCDS.コースＣＤ AND MAIN.商品ＣＤ = MCDS.商品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ④工場からもらった個数
                                      "\r\n     (SELECT" +
                                      "\r\n         MI.相手コースＣＤ" +
                                      "\r\n         ,MI.商品ＣＤ" +
                                      "\r\n         ,SUM(MI.個数) AS 個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_移動入力 AS MI" +
                                      "\r\n         LEFT JOIN コースマスタ AS CM" +
                                      "\r\n         ON MI.企業ＣＤ = CM.企業ＣＤ" +
                                      "\r\n         AND MI.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         AND ISNULL(CM.工場フラグ, 0) <> 0" +
                                      "\r\n         AND MI.移動日 = @精算日" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         MI.相手コースＣＤ" +
                                      "\r\n         ,MI.商品ＣＤ" +
                                      "\r\n     ) AS TAKE_KOJO ON MAIN.コースＣＤ = TAKE_KOJO.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_KOJO.商品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ⑤工場以外からもらった個数
                                      "\r\n     (SELECT" +
                                      "\r\n         MI.相手コースＣＤ" +
                                      "\r\n         ,MI.商品ＣＤ" +
                                      "\r\n         ,SUM(MI.個数) AS 個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_移動入力 AS MI" +
                                      "\r\n         LEFT JOIN コースマスタ AS CM" +
                                      "\r\n         ON MI.企業ＣＤ = CM.企業ＣＤ" +
                                      "\r\n         AND MI.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         AND ISNULL(CM.工場フラグ, 0) = 0" +
                                      "\r\n         AND MI.移動日 = @精算日" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         MI.相手コースＣＤ" +
                                      "\r\n         ,MI.商品ＣＤ" +
                                      "\r\n     ) AS TAKE_IPPAN ON MAIN.コースＣＤ = TAKE_IPPAN.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_IPPAN.商品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ⑥渡した個数
                                      "\r\n     (SELECT" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n         ,SUM(個数) AS 個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_移動入力" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND 移動日 = @精算日" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n     ) AS GIVE ON MAIN.コースＣＤ = GIVE.コースＣＤ AND MAIN.商品ＣＤ = GIVE.商品ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分ボーナス", SqlDbType.Int).Value = (int)Consts.UriageKubun.Bonus;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("持出明細");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    mochidashiBindingSource.DataSource = ds;
                    mochidashiBindingSource.DataMember = "持出明細";

                    log.Info("持出明細を検索しました。");
                }

                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_SYOHIN_NAME_COLUMN, "商品名略称");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_KOSU_COLUMN,        "持出個数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_TAKE_KOJO_COLUMN,   "工場追加数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_TAKE_IPPAN_COLUMN,  "もらった数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_GIVE_COLUMN,        "やった数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_URIAGE_COLUMN,      "売上予定実績数");
                fpSpread_Mochidashi_Sheet1.BindDataColumn(MOCHIDASHI_ZANSU_COLUMN,       "残数");
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
        /// チケット束販売取得
        /// </summary>
        private void GetTicketSale()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // チケット束フラグが１の売上明細を取得
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     0 AS 合計行" +
                                      "\r\n     ,A.企業ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.得意先名" +
                                      "\r\n     ,CASE WHEN A.電子決済 = 0 THEN '' ELSE '○' END AS 電子決済" +      // -- 電子決済の場合は○を表示する
                                      "\r\n     ,A.商品単価" +
                                      "\r\n     ,A.商品数量" +
                                      "\r\n     ,A.商品金額合計税込" +
                                      "\r\n     ,C.コース順" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         H.企業ＣＤ" +
                                      "\r\n         ,H.コースＣＤ" +
                                      "\r\n         ,H.得意先ＣＤ" +
                                      "\r\n         ,H.得意先名" +
                                      "\r\n         ,CASE WHEN M.売上区分 = @売上区分LINE THEN 1 ELSE 0 END AS 電子決済" +
                                      "\r\n         ,M.商品単価" +
                                      "\r\n         ,SUM(M.商品数量)         AS 商品数量" +
                                      "\r\n         ,SUM(M.商品金額合計税込) AS 商品金額合計税込" +
                                      "\r\n     FROM" +
                                      "\r\n         売上ヘッダ AS H" +
                                      "\r\n         INNER JOIN 売上明細 AS M" +
                                      "\r\n             ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n             AND M.売上日     = H.売上日" +
                                      "\r\n             AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n             AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n         AND H.売上日     = @精算日" +
                                      "\r\n         AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND ISNULL(M.チケット束フラグ, 0) <> 0" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         H.企業ＣＤ" +
                                      "\r\n         ,H.コースＣＤ" +
                                      "\r\n         ,H.得意先ＣＤ" +
                                      "\r\n         ,H.得意先名" +
                                      "\r\n         ,(CASE WHEN M.売上区分 = @売上区分LINE THEN 1 ELSE 0 END)" +
                                      "\r\n         ,M.商品単価" +
                                      "\r\n     ) AS A" +
                                      "\r\n     LEFT JOIN コーステーブル AS C" +
                                      "\r\n         ON  C.企業ＣＤ   = A.企業ＣＤ" +
                                      "\r\n         AND C.コースＣＤ = A.コースＣＤ" +
                                      "\r\n         AND C.得意先ＣＤ = A.得意先ＣＤ" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 合計行" +
                                      "\r\n     ,A.企業ＣＤ" +
                                      "\r\n     ,NULL AS コースＣＤ" +
                                      "\r\n     ,NULL AS 得意先ＣＤ" +
                                      "\r\n     ,'合計' AS 得意先名" +
                                      "\r\n     ,NULL AS 商品単価" +
                                      "\r\n     ,NULL AS 電子決済" +
                                      "\r\n     ,SUM(A.商品数量) AS 商品数量" +
                                      "\r\n     ,SUM(A.商品金額合計税込) AS 商品金額合計税込" +
                                      "\r\n     ,NULL AS コース順" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         H.企業ＣＤ" +
                                      "\r\n         ,H.コースＣＤ" +
                                      "\r\n         ,H.得意先ＣＤ" +
                                      "\r\n         ,H.得意先名" +
                                      "\r\n         ,M.商品単価" +
                                      "\r\n         ,SUM(M.商品数量)         AS 商品数量" +
                                      "\r\n         ,SUM(M.商品金額合計税込) AS 商品金額合計税込" +
                                      "\r\n     FROM" +
                                      "\r\n         売上ヘッダ AS H" +
                                      "\r\n         INNER JOIN 売上明細 AS M" +
                                      "\r\n             ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n             AND M.売上日     = H.売上日" +
                                      "\r\n             AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n             AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n         AND H.売上日     = @精算日" +
                                      "\r\n         AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND ISNULL(M.チケット束フラグ, 0) <> 0" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         H.企業ＣＤ" +
                                      "\r\n         ,H.コースＣＤ" +
                                      "\r\n         ,H.得意先ＣＤ" +
                                      "\r\n         ,H.得意先名" +
                                      "\r\n         ,M.商品単価" +
                                      "\r\n     ) AS A" +
                                      "\r\n GROUP BY" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     合計行" +
                                      "\r\n     ,商品単価" +
                                      "\r\n     ,得意先ＣＤ" +
                                      "\r\n     ,電子決済";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("チケット束販売");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // 数式を消すため、全行削除する
                    fpSpread_TicketSale_Sheet1.Rows.Count = 0;

                    ticketSaleBindingSource.DataSource = ds;
                    ticketSaleBindingSource.DataMember = "チケット束販売";

                    log.Info("チケット束販売を検索しました。");
                }

                fpSpread_TicketSale_Sheet1.BindDataColumn(TICKET_SALE_TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread_TicketSale_Sheet1.BindDataColumn(TICKET_SALE_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_TicketSale_Sheet1.BindDataColumn(TICKET_SALE_PAY_COLUMN,            "電子決済");
                fpSpread_TicketSale_Sheet1.BindDataColumn(TICKET_SALE_TANKA_COLUMN,          "商品単価");
                fpSpread_TicketSale_Sheet1.BindDataColumn(TICKET_SALE_KAZU_COLUMN,           "商品数量");
                fpSpread_TicketSale_Sheet1.BindDataColumn(TICKET_SALE_URIAGE_COLUMN,         "商品金額合計税込");

                // 最終行を固定行にする
                if (fpSpread_TicketSale_Sheet1.Rows.Count > 1)
                {
                    fpSpread_TicketSale_Sheet1.FrozenTrailingRowCount = 1;
                    fpSpread_TicketSale_Sheet1.Rows[fpSpread_TicketSale_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
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
        /// 得意先チケット使用取得取得
        /// </summary>
        private void GetTicketUse()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // 売上区分がチケットの売上明細を取得
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     0 AS 合計行" +
                                      "\r\n     ,A.企業ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.得意先名" +
                                      "\r\n     ,A.商品単価" +
                                      "\r\n     ,A.商品数量" +
                                      "\r\n     ,A.商品金額合計税込" +
                                      "\r\n     ,C.コース順" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         H.企業ＣＤ" +
                                      "\r\n         ,H.コースＣＤ" +
                                      "\r\n         ,H.得意先ＣＤ" +
                                      "\r\n         ,H.得意先名" +
                                      "\r\n         ,M.商品単価" +
                                      "\r\n         ,SUM(M.商品数量)         AS 商品数量" +
                                      "\r\n         ,SUM(M.商品金額合計税込) AS 商品金額合計税込" +
                                      "\r\n     FROM" +
                                      "\r\n         売上ヘッダ AS H" +
                                      "\r\n         INNER JOIN 売上明細 AS M" +
                                      "\r\n             ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n             AND M.売上日     = H.売上日" +
                                      "\r\n             AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n             AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n         AND H.売上日     = @精算日" +
                                      "\r\n         AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND M.売上区分   = @売上区分チケット" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         H.企業ＣＤ" +
                                      "\r\n         ,H.コースＣＤ" +
                                      "\r\n         ,H.得意先ＣＤ" +
                                      "\r\n         ,H.得意先名" +
                                      "\r\n         ,M.商品単価" +
                                      "\r\n     ) AS A" +
                                      "\r\n     LEFT JOIN コーステーブル AS C" +
                                      "\r\n         ON  C.企業ＣＤ   = A.企業ＣＤ" +
                                      "\r\n         AND C.コースＣＤ = A.コースＣＤ" +
                                      "\r\n         AND C.得意先ＣＤ = A.得意先ＣＤ" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 合計行" +
                                      "\r\n     ,A.企業ＣＤ" +
                                      "\r\n     ,NULL AS コースＣＤ" +
                                      "\r\n     ,NULL AS 得意先ＣＤ" +
                                      "\r\n     ,'合計' AS 得意先名" +
                                      "\r\n     ,NULL AS 商品単価" +
                                      "\r\n     ,SUM(A.商品数量) AS 商品数量" +
                                      "\r\n     ,SUM(A.商品金額合計税込) AS 商品金額合計税込" +
                                      "\r\n     ,NULL AS コース順" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         H.企業ＣＤ" +
                                      "\r\n         ,H.コースＣＤ" +
                                      "\r\n         ,H.得意先ＣＤ" +
                                      "\r\n         ,H.得意先名" +
                                      "\r\n         ,M.商品単価" +
                                      "\r\n         ,SUM(M.商品数量)         AS 商品数量" +
                                      "\r\n         ,SUM(M.商品金額合計税込) AS 商品金額合計税込" +
                                      "\r\n     FROM" +
                                      "\r\n         売上ヘッダ AS H" +
                                      "\r\n         INNER JOIN 売上明細 AS M" +
                                      "\r\n             ON  M.企業ＣＤ   = H.企業ＣＤ" +
                                      "\r\n             AND M.売上日     = H.売上日" +
                                      "\r\n             AND M.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n             AND M.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         H.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n         AND H.売上日     = @精算日" +
                                      "\r\n         AND H.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND M.売上区分   = @売上区分チケット" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         H.企業ＣＤ" +
                                      "\r\n         ,H.コースＣＤ" +
                                      "\r\n         ,H.得意先ＣＤ" +
                                      "\r\n         ,H.得意先名" +
                                      "\r\n         ,M.商品単価" +
                                      "\r\n     ) AS A" +
                                      "\r\n GROUP BY" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     合計行" +
                                      "\r\n     ,商品単価" +
                                      "\r\n     ,得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("得意先チケット使用");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // 数式を消すため、全行削除する
                    fpSpread_TicketUse_Sheet1.Rows.Count = 0;

                    ticketUseBindingSource.DataSource = null;
                    ticketUseBindingSource.DataSource = ds;
                    ticketUseBindingSource.DataMember = "得意先チケット使用";

                    log.Info("得意先チケット使用を検索しました。");
                }

                fpSpread_TicketUse_Sheet1.BindDataColumn(TICKET_USE_TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread_TicketUse_Sheet1.BindDataColumn(TICKET_USE_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_TicketUse_Sheet1.BindDataColumn(TICKET_USE_TANKA_COLUMN,          "商品単価");
                fpSpread_TicketUse_Sheet1.BindDataColumn(TICKET_USE_KAZU_COLUMN,           "商品数量");
                fpSpread_TicketUse_Sheet1.BindDataColumn(TICKET_USE_URIAGE_COLUMN,         "商品金額合計税込");

                // 最終行を固定行にする
                if (fpSpread_TicketUse_Sheet1.Rows.Count > 1)
                {
                    fpSpread_TicketUse_Sheet1.FrozenTrailingRowCount = 1;
                    fpSpread_TicketUse_Sheet1.Rows[fpSpread_TicketUse_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
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
        /// 回収現金の明細を取得
        /// </summary>
        private void GetKaisyuGenkinList()
        {
            try
            {
                DataTable dt = GetKaisyuMain(Consts.NyukinHoho.Genkin, "回収現金");

                // 明細があれば合計行を追加
                if (dt.Rows.Count > 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                // 数式を消すため、全行削除する
                fpSpread_KaisyuGenkin_Sheet1.Rows.Count = 0;

                kaisyuGenkinBindingSource.DataSource = ds;
                kaisyuGenkinBindingSource.DataMember = "回収現金";

                fpSpread_KaisyuGenkin_Sheet1.BindDataColumn(KAISYU_TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread_KaisyuGenkin_Sheet1.BindDataColumn(KAISYU_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_KaisyuGenkin_Sheet1.BindDataColumn(KAISYU_KINGAKU_COLUMN,        "入金額");

                // 最終行を固定行にする
                if (fpSpread_KaisyuGenkin_Sheet1.Rows.Count > 1)
                {
                    fpSpread_KaisyuGenkin_Sheet1.FrozenTrailingRowCount = 1;
                    fpSpread_KaisyuGenkin_Sheet1.Rows[fpSpread_KaisyuGenkin_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                    fpSpread_KaisyuGenkin_Sheet1.Cells[fpSpread_KaisyuGenkin_Sheet1.Rows.Count - 1, KAISYU_TOKUISAKI_NAME_COLUMN].Text = "合計";

                    // 合計行に数式を設定する
                    FarPoint.Win.Spread.Cell topCell = fpSpread_KaisyuGenkin_Sheet1.Cells[0, KAISYU_KINGAKU_COLUMN];
                    FarPoint.Win.Spread.Cell bottomCell = fpSpread_KaisyuGenkin_Sheet1.Cells[fpSpread_KaisyuGenkin_Sheet1.Rows.Count - 2, KAISYU_KINGAKU_COLUMN];
                    FarPoint.Win.Spread.Cell sumCell = fpSpread_KaisyuGenkin_Sheet1.Cells[fpSpread_KaisyuGenkin_Sheet1.Rows.Count - 1, KAISYU_KINGAKU_COLUMN];
                    sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
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
        /// 回収Tapionの明細を取得
        /// </summary>
        private void GetKaisyuTapionList()
        {
                DataTable dt = GetKaisyuMain(Consts.NyukinHoho.Tapion, "回収Tapion");

                // 明細があれば合計行を追加
                if (dt.Rows.Count > 0)
                {
                    dt.Rows.Add(dt.NewRow());
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                // 数式を消すため、全行削除する
                fpSpread_KaisyuTapion_Sheet1.Rows.Count = 0;

                kaisyuTapionBindingSource.DataSource = ds;
                kaisyuTapionBindingSource.DataMember = "回収Tapion";

                fpSpread_KaisyuTapion_Sheet1.BindDataColumn(KAISYU_TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread_KaisyuTapion_Sheet1.BindDataColumn(KAISYU_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_KaisyuTapion_Sheet1.BindDataColumn(KAISYU_KINGAKU_COLUMN,        "入金額");

                // 最終行を固定行にする
                if (fpSpread_KaisyuTapion_Sheet1.Rows.Count > 1)
                {
                    fpSpread_KaisyuTapion_Sheet1.FrozenTrailingRowCount = 1;
                    fpSpread_KaisyuTapion_Sheet1.Rows[fpSpread_KaisyuTapion_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                    fpSpread_KaisyuTapion_Sheet1.Cells[fpSpread_KaisyuTapion_Sheet1.Rows.Count - 1, KAISYU_TOKUISAKI_NAME_COLUMN].Text = "合計";

                    // 合計行に数式を設定する
                    FarPoint.Win.Spread.Cell topCell = fpSpread_KaisyuTapion_Sheet1.Cells[0, KAISYU_KINGAKU_COLUMN];
                    FarPoint.Win.Spread.Cell bottomCell = fpSpread_KaisyuTapion_Sheet1.Cells[fpSpread_KaisyuTapion_Sheet1.Rows.Count - 2, KAISYU_KINGAKU_COLUMN];
                    FarPoint.Win.Spread.Cell sumCell = fpSpread_KaisyuTapion_Sheet1.Cells[fpSpread_KaisyuTapion_Sheet1.Rows.Count - 1, KAISYU_KINGAKU_COLUMN];
                    sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
                }
        }

        /// <summary>
        /// 売上精算_ヘッダが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeUriageSeisanHeader()
        {
            // 売上精算_ヘッダを比較
            if (_uriageSeisanHeader == null)
            {
                return false;
            }

            // 未検索？
            if (_origin == null)
            {
                return false;
            }

            // 取得済みの売上精算_ヘッダが新規の場合は変更済みと見なす
            if (_IsNewRecord)
            {
                return true;
            }

            if (_uriageSeisanHeader.Equals(_origin))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 売上精算_ヘッダが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveUriageSeisanHeader()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索条件の入力にエラーがある？
                if (_uriageSeisanCriteria.HasErrors)
                {
                    MessageBox.Show(this, _uriageSeisanCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_uriageSeisanCriteriaSnapShot != null)
                {
                    if (_uriageSeisanCriteria.Equals(_uriageSeisanCriteriaSnapShot) == false)
                    {
                        MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                // 未検索？
                if (_origin == null)
                {
                    MessageBox.Show(this, "登録するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_uriageSeisanHeader.HasErrors)
                {
                    MessageBox.Show(this, _uriageSeisanHeader.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 精算日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                // Q.なぜ表示ボタン押下時ではなく登録ボタン押下時にチェックするのか？
                // A.表示ボタン押下時にチェックしても、どのみち続行（データがあれば表示、無ければ新規入力）するしかないので
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_uriageSeisanCriteria.精算日 > warnDate)
                {
                    if (MessageBox.Show(this, "精算日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
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
            History.ProcessType historyProcess = History.ProcessType.Update;
            string logMessage = "売上精算_ヘッダを{0}しました。";

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 売上精算_入力データ一時保存を削除
                    if (DeleteUriageSeisanTemporary(con, tran) == false)
                    {
                        return false;
                    }

                    // 売上精算_ヘッダ保存
                    if (SaveUriageSeisanHeader(con, tran) == false)
                    {
                        return false;
                    }

                    if (_IsNewRecord)
                    {
                        historyProcess = History.ProcessType.Create;
                        logMessage = "売上精算_ヘッダ・売上精算_売上明細、売上精算_持出明細を{0}しました。";

                        // 売上精算_売上明細保存
                        if (SaveUriageSeisanUriageMeisai(con, tran) == false)
                        {
                            return false;
                        }

                        // 売上精算_持出明細保存
                        if (SaveUriageSeisanMochidashiMeisai(con, tran) == false)
                        {
                            return false;
                        }
                    }

                    // コースにぶら下がる得意先のうち、売掛現金区分=現金のものだけを抽出して締め対象にする
                    SortedSet<string> shimeTokuisaki = GetGenkinTokuisaki(con, tran);
                    if (shimeTokuisaki != null && shimeTokuisaki.Count != 0)
                    {
                        string[] tokuisakiCDs = shimeTokuisaki.ToArray();
                        if (SeikyuShimeExec.Run(con, tran, _uriageSeisanCriteria, tokuisakiCDs) == false)
                        {
                            return false;
                        }
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat(logMessage, historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        /// <summary>
        /// 売上精算_入力データ一時保存削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUriageSeisanTemporary(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                UriageSeisanTemporary uriageSeisanTemporary = new UriageSeisanTemporary(_uriageSeisanHeader);
                bool ret = uriageSeisanTemporary.Delete(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, uriageSeisanTemporary.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "売上精算_入力データ一時保存", uriageSeisanTemporary.Code, historyProcess);
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
        /// 売上精算_ヘッダ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveUriageSeisanHeader(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _uriageSeisanHeader.Insert(con, tran);
                }
                else
                {
                    ret = _uriageSeisanHeader.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _uriageSeisanHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "売上精算_ヘッダ", _uriageSeisanHeader.Code, historyProcess);
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
        /// 売上精算_売上明細保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveUriageSeisanUriageMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // 現金売上を保存
                if (SaveUriageSeisanUriageMeisaiMain(con, tran, "現金売上", genkinUriageBindingSource) == false)
                {
                    return false;
                }

                // 掛売売上を保存
                if (SaveUriageSeisanUriageMeisaiMain(con, tran, "掛売売上", kakeuriUriageBindingSource) == false)
                {
                    return false;
                }

                // 電子決済売上を保存
                if (SaveUriageSeisanUriageMeisaiMain(con, tran, "E売上", payUriageBindingSource) == false)
                {
                    return false;
                }

                // 現金・掛売・電子決済以外を取得して保存する（売上集計表に使うため）
                if (SaveUriageSeisanUriageMeisaiElse(con, tran) == false)
                {
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
        /// 売上精算_売上明細保存処理メイン
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tableName">データテーブル名</param>
        /// <param name="bindingSource">バインド先バインディングソース</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveUriageSeisanUriageMeisaiMain(SqlConnection con, SqlTransaction tran, string tableName, BindingSource bindingSource)
        {
            try
            {
                DataSet ds = (DataSet)bindingSource.DataSource;
                DataTable dt = ds.Tables[tableName];

                History.ProcessType historyProcess = History.ProcessType.Create;
                bool ret = false;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    UriageSeisanUriageMeisai uriageSeisanUriageMeisai = new UriageSeisanUriageMeisai(dr);

                    // 新規追加のみなので、RowStateにかかわらず、常に追加
                    ret = uriageSeisanUriageMeisai.Insert(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, uriageSeisanUriageMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "売上精算_売上明細", uriageSeisanUriageMeisai.Code, historyProcess);
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
        /// 売上精算_売上明細保存処理（現金・掛売・電子決済以外）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveUriageSeisanUriageMeisaiElse(SqlConnection con, SqlTransaction tran)
        {
            try
            {

                History.ProcessType historyProcess = History.ProcessType.Create;
                bool ret = false;

                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     M.企業ＣＤ" +
                                  "\r\n     ,M.売上日 AS 精算日" +
                                  "\r\n     ,M.部署ＣＤ" +
                                  "\r\n     ,H.コースＣＤ" +
                                  "\r\n     ,M.売上区分" +
                                  "\r\n     ,M.商品ＣＤ" +
                                  "\r\n     ,S.商品名略称" +
                                  "\r\n     ,ISNULL(SUM(M.商品数量), 0)         AS 売上数量" +
                                  "\r\n     ,ISNULL(SUM(M.値引額), 0)           AS 値引額" +
                                  "\r\n     ,ISNULL(SUM(M.値引前金額), 0)       AS 売上金額" +
                                  "\r\n     ,ISNULL(SUM(M.値引後金額), 0)       AS 値引後金額" +
                                  "\r\n     ,ISNULL(SUM(M.消費税額), 0)         AS 消費税額" +
                                  "\r\n     ,ISNULL(SUM(M.商品金額合計税込), 0) AS 商品金額合計税込" +
                                  "\r\n     ,NULL AS 作成担当者ＣＤ" +
                                  "\r\n     ,NULL AS 作成担当者名" +
                                  "\r\n     ,NULL AS 作成日" +
                                  "\r\n     ,NULL AS 修正担当者ＣＤ" +
                                  "\r\n     ,NULL AS 修正担当者名" +
                                  "\r\n     ,NULL AS 修正日" +
                                  "\r\n FROM" +
                                  "\r\n     売上ヘッダ AS H" +
                                  "\r\n     INNER JOIN 売上明細 AS M " +
                                  "\r\n         ON  M.企業ＣＤ   = H.企業ＣＤ " +
                                  "\r\n         AND M.売上日     = H.売上日 " +
                                  "\r\n         AND M.得意先ＣＤ = H.得意先ＣＤ " +
                                  "\r\n         AND M.部署ＣＤ   = H.部署ＣＤ" +
                                  "\r\n     LEFT JOIN 商品マスタ AS S " +
                                  "\r\n         ON  S.企業ＣＤ = M.企業ＣＤ " +
                                  "\r\n         AND S.商品ＣＤ = M.商品ＣＤ" +
                                  "\r\n     INNER JOIN 得意先マスタ AS T " +
                                  "\r\n         ON  T.企業ＣＤ   = H.企業ＣＤ " +
                                  "\r\n         AND T.得意先ＣＤ = H.得意先ＣＤ " +
                                  "\r\n WHERE" +
                                  "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                  "\r\n     AND H.売上日     = @精算日" +
                                  "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                  "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                  "\r\n     AND M.売上区分 IN (@売上区分チケット, @売上区分バークレー, @売上区分ボーナス)" +     // チケットとバークレーとボーナス（つまり現金と掛売と電子決済以外）
                                  "\r\n     AND ISNULL(M.チケット束フラグ, 0) = 0" +                    // --チケット束売りは除く
                                  "\r\n GROUP BY" +
                                  "\r\n     M.企業ＣＤ" +
                                  "\r\n     ,M.売上日" +
                                  "\r\n     ,M.部署ＣＤ" +
                                  "\r\n     ,H.コースＣＤ" +
                                  "\r\n     ,M.売上区分" +
                                  "\r\n     ,M.商品ＣＤ" +
                                  "\r\n     ,S.商品名略称" +
                                  "\r\n ORDER BY" +
                                  "\r\n     M.商品ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;
                cmd.Parameters.Add("@売上区分バークレー", SqlDbType.Int).Value = (int)Consts.UriageKubun.Barclay;
                cmd.Parameters.Add("@売上区分ボーナス", SqlDbType.Int).Value = (int)Consts.UriageKubun.Bonus;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adp.Fill(dt);

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    UriageSeisanUriageMeisai uriageSeisanUriageMeisai = new UriageSeisanUriageMeisai(dr);

                    // 新規追加のみなので、RowStateにかかわらず、常に追加
                    ret = uriageSeisanUriageMeisai.Insert(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, uriageSeisanUriageMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "売上精算_売上明細", uriageSeisanUriageMeisai.Code, historyProcess);
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
        /// 売上精算_持出明細保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveUriageSeisanMochidashiMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)mochidashiBindingSource.DataSource;
                DataTable dt = ds.Tables["持出明細"];

                History.ProcessType historyProcess = History.ProcessType.Create;
                bool ret = false;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    UriageSeisanMochidashiMeisai uriageSeisanMochidashiMeisai = new UriageSeisanMochidashiMeisai(dr);

                    // 新規追加のみなので、RowStateにかかわらず、常に追加
                    uriageSeisanMochidashiMeisai.精算日 = _uriageSeisanHeader.精算日;
                    uriageSeisanMochidashiMeisai.部署ＣＤ = _uriageSeisanHeader.部署ＣＤ;
                    ret = uriageSeisanMochidashiMeisai.Insert(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, uriageSeisanMochidashiMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "売上精算_持出明細", uriageSeisanMochidashiMeisai.Code, historyProcess);
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
        /// コースに属しており、売上または入金がある、売掛現金区分=現金の得意先を抽出する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        private  SortedSet<string> GetGenkinTokuisaki(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                SortedSet<string> shimeTokuisaki = new SortedSet<string>();

                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.得意先ＣＤ" +
                                  "\r\n FROM" +
                                  // -- 駆動表① 売上が存在する得意先
                                  "\r\n     (SELECT" +
                                  "\r\n         U1.得意先ＣＤ" +
                                  "\r\n     FROM" +
                                  "\r\n         売上ヘッダ AS U1" +
                                  "\r\n         INNER JOIN 売上明細 AS UM1" +
                                  "\r\n             ON UM1.企業ＣＤ = U1.企業ＣＤ" +
                                  "\r\n             AND UM1.売上日 = U1.売上日" +
                                  "\r\n             AND UM1.得意先ＣＤ = U1.得意先ＣＤ" +
                                  "\r\n             AND UM1.部署ＣＤ = U1.部署ＣＤ" +
                                  "\r\n         LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n             (SELECT" +
                                  "\r\n                 H1.得意先ＣＤ" +
                                  "\r\n                 ,H1.部署ＣＤ" +
                                  "\r\n                 ,MAX(H1.請求日) AS 前回締日" +
                                  "\r\n             FROM" +
                                  "\r\n                 請求ヘッダ AS H1" +
                                  "\r\n                 INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号" +
                                  "\r\n             WHERE" +
                                  "\r\n                 H1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND H1.請求日 < @精算日" +      // -- 今回締日（入力された請求日）
                                  "\r\n             GROUP BY" +
                                  "\r\n                 H1.得意先ＣＤ" +
                                  "\r\n                 ,H1.部署ＣＤ" +
                                  "\r\n             ) AS M11 ON" +
                                  "\r\n                 M11.得意先ＣＤ = U1.得意先ＣＤ" +
                                  "\r\n                 AND M11.部署ＣＤ = U1.部署ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         U1.企業ＣＤ = @企業ＣＤ" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                                  "\r\n         AND U1.売上日 > ISNULL(M11.前回締日, '2000/01/01')" +
                                  "\r\n         AND U1.売上日 <= @精算日" +     // -- 今回締日（入力された請求日）
                                  "\r\n         AND UM1.売上区分 = @売上区分売掛" +     // -- 売掛
                                  "\r\n     UNION" +
                                  // -- 駆動表② 入金が存在する得意先
                                  "\r\n     SELECT" +
                                  "\r\n         N1.得意先ＣＤ" +
                                  "\r\n     FROM" +
                                  "\r\n         入金明細 AS N1" +
                                  "\r\n         LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n             (SELECT" +
                                  "\r\n                 G2.請求先ＣＤ" +
                                  "\r\n                 ,G2.部署ＣＤ" +
                                  "\r\n                 ,MAX(G2.請求日) AS 前回締日" +
                                  "\r\n             FROM" +
                                  "\r\n                 請求ヘッダ AS H2" +
                                  "\r\n                 INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号" +
                                  "\r\n             WHERE" +
                                  "\r\n                 H2.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND H2.請求日 < @精算日" +      // -- 今回締日（入力された請求日）
                                  "\r\n             GROUP BY" +
                                  "\r\n                 G2.請求先ＣＤ" +
                                  "\r\n                 ,G2.部署ＣＤ" +
                                  "\r\n             ) AS M21 ON" +
                                  "\r\n                 M21.請求先ＣＤ = N1.得意先ＣＤ" +
                                  "\r\n                 AND M21.部署ＣＤ = N1.部署ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         N1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n         AND N1.入金日 > ISNULL(M21.前回締日, '2000/01/01')" +
                                  "\r\n         AND N1.入金日 <= @精算日" +     // -- 今回締日（入力された請求日）
                                  "\r\n     ) AS A" +
                                  "\r\n INNER JOIN" +
                                  "\r\n     (SELECT" +
                                  "\r\n         T.得意先ＣＤ" +
                                  "\r\n     FROM" +
                                  // -- コースに紐付く得意先の入金を取得する
                                  // -- 分配した場合は、基本的に分配先はコースに属していないので、コースに属している分配元(=受注得意先)を経由して入金を取得する
                                  "\r\n         (SELECT DISTINCT" +
                                  "\r\n             T.企業ＣＤ" +
                                  "\r\n             ,T.得意先ＣＤ" +
                                  "\r\n         FROM" +
                                  "\r\n             (SELECT" +
                                  "\r\n                 CT.企業ＣＤ" +
                                  "\r\n                 ,CT.得意先ＣＤ" +
                                  "\r\n             FROM" +
                                  "\r\n                 コーステーブル AS CT" +
                                  "\r\n                 INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                  "\r\n                 LEFT JOIN" +
                                  "\r\n                     (SELECT" +
                                  "\r\n                         コースＣＤ" +
                                  "\r\n                     FROM" +
                                  "\r\n                         臨時コースマスタ" +
                                  "\r\n                     WHERE" +
                                  "\r\n                         企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                         AND ISNULL(無効フラグ, 0) = 0" +
                                  "\r\n                         AND コースＣＤ = @コースＣＤ" +
                                  "\r\n                         AND 実施日 = @精算日" +
                                  "\r\n                     ) AS RM" +
                                  "\r\n                     ON RM.コースＣＤ = CT.コースＣＤ" +
                                  "\r\n             WHERE" +
                                  "\r\n                 CT.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND CT.コースＣＤ = @コースＣＤ" +
                                  "\r\n                 AND ISNULL(CT.無効フラグ, 0) = 0" +
                                  "\r\n                 AND RM.コースＣＤ IS NULL" +
                                  // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                  "\r\n             UNION" +
                                  "\r\n             SELECT" +
                                  "\r\n                 RT.企業ＣＤ" +
                                  "\r\n                 ,RT.得意先ＣＤ" +
                                  "\r\n             FROM" +
                                  "\r\n                 臨時コーステーブル AS RT" +
                                  "\r\n                 INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                  "\r\n             WHERE" +
                                  "\r\n                 RT.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND RT.コースＣＤ = @コースＣＤ" +
                                  "\r\n                 AND ISNULL(RT.無効フラグ, 0) = 0" +
                                  "\r\n                 AND RT.実施日 = @精算日" +
                                  "\r\n             ) AS CT" +
                                  "\r\n             INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ) = CT.得意先ＣＤ" +
                                  "\r\n         ) AS JT" +
                                  "\r\n         INNER JOIN 得意先マスタ AS T" +
                                  "\r\n             ON  T.企業ＣＤ = JT.企業ＣＤ" +
                                  "\r\n             AND T.得意先ＣＤ = JT.得意先ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         T.売掛現金区分 = @売掛現金区分現金" +
                                  "\r\n     ) AS B" +
                                  "\r\n         ON A.得意先ＣＤ = B.得意先ＣＤ" +
                                  "\r\n ORDER BY" +
                                  "\r\n     A.得意先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;

                log.Debug(cmd.CommandTextWithParameters());

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        shimeTokuisaki.Add(reader["得意先ＣＤ"].ToString());
                    }
                }

                log.Info("コースにぶら下がる売掛現金区分=現金の得意先を検索しました。");

                return shimeTokuisaki;
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                btnQuery.PerformClick();        // 検索ボタン押下
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
        /// 印刷処理
        /// </summary>
        /// <param name="printerNameA4">A4プリンタ名</param>
        private void PrintMain(string printerNameA4)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                Report403AMain report403AMain = new Report403AMain();

                // チケット精算情報（持出枚数・持帰枚数・売上枚数・売上金額）を含んだ売上精算_ヘッダを取得
                DataTable dtMain = GetUriageSeisanHeaderForPrint();

                // メインレポートのデータソースにセット
                report403AMain.DataSource = dtMain;

                // 売上精算_持出明細のデータテーブルをサブレポートのデータソースにセット
                // ただし、表示する値が無い部品は除外する
                DataSet ds = (DataSet)mochidashiBindingSource.DataSource;
                DataTable dtMochidashi = ds.Tables["持出明細"].Clone();
                DataRow[] drs = ds.Tables["持出明細"].Select("ISNULL(持出個数, 0) <> 0 OR " +
                                                             "ISNULL(工場追加数, 0) <> 0 OR " +
                                                             "ISNULL(もらった数, 0) <> 0 OR " +
                                                             "ISNULL(やった数, 0) <> 0 OR " +
                                                             "ISNULL(ボーナス消費数, 0) <> 0 OR " +
                                                             "ISNULL(売上予定実績数, 0) <> 0", "商品ＣＤ");
                if (drs != null)
                {
                    foreach (DataRow dr in drs)
                    {
                        dtMochidashi.ImportRow(dr);
                    }
                }
                report403AMain.DtMochidashi = dtMochidashi;

                // 売上精算_売上明細（現金）のデータテーブルをサブレポートのデータソースにセット
                ds = (DataSet)genkinUriageBindingSource.DataSource;
                DataTable dtgenkin = ds.Tables["現金売上"].Copy();
                // 現金_回収現金件数を持たせる
                dtgenkin.Columns.Add(dtMain.Columns["現金_回収現金件数"].ColumnName, dtMain.Columns["現金_回収現金件数"].DataType);
                if (dtgenkin.Rows.Count > 0)
                {
                    dtgenkin.Rows[0]["現金_回収現金件数"] = dtMain.Rows[0]["現金_回収現金件数"];
                }
                report403AMain.DtGenkin = dtgenkin;

                // 売上精算_売上明細（掛売）のデータテーブルをサブレポートのデータソースにセット
                ds = (DataSet)kakeuriUriageBindingSource.DataSource;
                DataTable dtkakeuri = ds.Tables["掛売売上"].Copy();
                report403AMain.DtKakeuri = dtkakeuri;

                // 売上精算_売上明細（電子決済）のデータテーブルをサブレポートのデータソースにセット
                ds = (DataSet)payUriageBindingSource.DataSource;
                DataTable dtPay = ds.Tables["E売上"].Copy();
                // E_回収金額件数を持たせる
                dtPay.Columns.Add(dtMain.Columns["E_回収金額件数"].ColumnName, dtMain.Columns["E_回収金額件数"].DataType);
                if (dtPay.Rows.Count > 0)
                {
                    dtPay.Rows[0]["E_回収金額件数"] = dtMain.Rows[0]["E_回収金額件数"];
                }
                report403AMain.DtPay = dtPay;

                report403AMain.Document.Printer.DocumentName = "売上明細表";
                report403AMain.Document.Printer.PrinterName = printerNameA4;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "売上明細表";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report403AMain);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);

                string code = string.Format("部署=[{0}] 精算日=[{1:yyyy/MM/dd}] コース=[{2}]",
                    (int)_uriageSeisanCriteria.部署ＣＤ, (DateTime)_uriageSeisanCriteria.精算日, (int)_uriageSeisanCriteria.コースＣＤ);
                History.Save(this.Text, "売上明細表", code, History.ProcessType.Output);

                log.InfoFormat("売上明細表を印刷しました。部署={0} 精算日={1} コース={2}",
                    (int)_uriageSeisanCriteria.部署ＣＤ, (DateTime)_uriageSeisanCriteria.精算日, (int)_uriageSeisanCriteria.コースＣＤ);
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
        /// チケット精算情報（持出枚数・持帰枚数・売上枚数・売上金額）を含んだ売上精算_ヘッダを取得
        /// </summary>
        /// <returns>チケット精算情報（持出枚数・持帰枚数・売上枚数・売上金額）を含んだ売上精算_ヘッダが格納されたデータテーブル</returns>
        private DataTable GetUriageSeisanHeaderForPrint()
        {
            try
            {
                DataTable dt = new DataTable("売上明細表メイン");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.精算日" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.配送担当者ＣＤ" +
                                      "\r\n     ,A.配送担当者名" +
                                      "\r\n     ,A.部署名" +
                                      "\r\n     ,A.コース名" +
                                      "\r\n     ,ISNULL(M.チケット持出枚数, 0) AS チケット持出枚数" +
                                      "\r\n     ,ISNULL(M.チケット持出枚数, 0) - ISNULL(A.チケット束_売上枚数, 0) AS チケット持帰枚数" +
                                      "\r\n     ,ISNULL(A.チケット束_売上枚数, 0) AS チケット売上枚数" +
                                      "\r\n     ,ISNULL(A.チケット束_売上金額, 0) AS チケット売上金額" +
                                      "\r\n     ,ISNULL(A.現金_売上合計, 0) AS 現金_売上合計" +
                                      "\r\n     ,ISNULL(A.現金_値引合計, 0) AS 現金_値引合計" +
                                      "\r\n     ,ISNULL(A.現金_回収現金件数, 0) AS 現金_回収現金件数" +
                                      "\r\n     ,ISNULL(A.現金_回収現金, 0) AS 現金_回収現金" +
                                      "\r\n     ,ISNULL(A.現金_チケット束代, 0) AS 現金_チケット束代" +
                                      "\r\n     ,ISNULL(A.現金_本日現金合計, 0) AS 現金_本日現金合計" +
                                      "\r\n     ,ISNULL(A.掛売_売上合計, 0) AS 掛売_売上合計" +
                                      "\r\n     ,ISNULL(A.掛売_値引合計, 0) AS 掛売_値引合計" +
                                      "\r\n     ,ISNULL(A.掛売_未集金, 0) AS 掛売_未集金" +
                                      "\r\n     ,ISNULL(A.掛売_未集金外売上, 0) AS 掛売_未集金外売上" +
                                      "\r\n     ,ISNULL(A.掛売_本日掛売合計, 0) AS 掛売_本日掛売合計" +
                                      "\r\n     ,ISNULL(A.E_売上合計, 0) AS E_売上合計" +
                                      "\r\n     ,ISNULL(A.E_値引合計, 0) AS E_値引合計" +
                                      "\r\n     ,ISNULL(A.E_回収金額件数, 0) AS E_回収金額件数" +
                                      "\r\n     ,ISNULL(A.E_回収金額, 0) AS E_回収金額" +
                                      "\r\n     ,ISNULL(A.E_チケット束代, 0) AS E_チケット束代" +
                                      "\r\n     ,ISNULL(A.E_本日E合計, 0) AS E_本日E合計" +
                                      "\r\n     ,ISNULL(A.合計_本日総売上, 0) AS 合計_本日総売上" +
                                      "\r\n     ,ISNULL(A.合計_本日総値引, 0) AS 合計_本日総値引" +
                                      "\r\n     ,ISNULL(A.合計_チケット売上, 0) AS 合計_チケット売上" +
                                      "\r\n     ,ISNULL(A.合計_バークレー売上, 0) AS 合計_バークレー売上" +
                                      "\r\n     ,ISNULL(A.合計_実質総売上, 0) AS 合計_実質総売上" +
                                      "\r\n     ,ISNULL(A.消費チケ枚数_チケット, 0) AS 消費チケ枚数_チケット" +
                                      "\r\n     ,ISNULL(A.消費チケ枚数_ボーナス, 0) AS 消費チケ枚数_ボーナス" +
                                      "\r\n     ,ISNULL(A.消費チケ枚数_ＢＶ300, 0) AS 消費チケ枚数_ＢＶ300" +
                                      "\r\n     ,ISNULL(A.消費チケ枚数_ＢＶ200, 0) AS 消費チケ枚数_ＢＶ200" +
                                      "\r\n     ,ISNULL(A.消費チケ金額_ＢＶ300, 0) AS 消費チケ金額_ＢＶ300" +
                                      "\r\n     ,ISNULL(A.消費チケ金額_ＢＶ200, 0) AS 消費チケ金額_ＢＶ200" +
                                      "\r\n     ,ISNULL(A.金銭内訳_10000円, 0) AS 金銭内訳_10000円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_5000円, 0) AS 金銭内訳_5000円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_2000円, 0) AS 金銭内訳_2000円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_1000円, 0) AS 金銭内訳_1000円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_500円, 0) AS 金銭内訳_500円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_100円, 0) AS 金銭内訳_100円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_50円, 0) AS 金銭内訳_50円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_10円, 0) AS 金銭内訳_10円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_5円, 0) AS 金銭内訳_5円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_1円, 0) AS 金銭内訳_1円" +
                                      "\r\n     ,ISNULL(A.金銭内訳_小切手, 0) AS 金銭内訳_小切手" +
                                      "\r\n     ,ISNULL(A.金銭内訳_領収書, 0) AS 金銭内訳_領収書" +
                                      "\r\n     ,ISNULL(A.金銭内訳_合計, 0) AS 金銭内訳_合計" +
                                      "\r\n     ,ISNULL(A.金銭内訳_過剰金, 0) AS 金銭内訳_過剰金" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     売上精算_ヘッダ AS A" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A2.企業ＣＤ = A.企業ＣＤ AND A.配送担当者ＣＤ = A3.担当者ＣＤ" +
                                      // -- 売上明細表_チケット持出枚数
                                      "\r\n     LEFT JOIN (" +
                                      "\r\n         SELECT" +
                                      "\r\n             A.企業ＣＤ" +
                                      "\r\n             ,A.精算日" +
                                      "\r\n             ,A.部署ＣＤ" +
                                      "\r\n             ,A.コースＣＤ" +
                                      "\r\n             ,SUM(A.持出個数) AS チケット持出枚数" +
                                      "\r\n         FROM" +
                                      "\r\n             売上精算_持出明細 AS A" +
                                      "\r\n             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             A.企業ＣＤ       = @企業ＣＤ" +
                                      "\r\n             AND A.精算日     = @精算日" +
                                      "\r\n             AND A.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n             AND A.コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND S.弁当区分   = @弁当区分チケット" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             A.企業ＣＤ" +
                                      "\r\n             ,A.精算日" +
                                      "\r\n             ,A.部署ＣＤ" +
                                      "\r\n             ,A.コースＣＤ" +
                                      "\r\n     ) AS M" +
                                      "\r\n         ON M.企業ＣＤ    = A.企業ＣＤ" +
                                      "\r\n         AND M.精算日     = A.精算日" +
                                      "\r\n         AND M.部署ＣＤ   = A.部署ＣＤ" +
                                      "\r\n         AND M.コースＣＤ = A.コースＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                                      "\r\n AND A.精算日     = @精算日" +
                                      "\r\n AND A.部署ＣＤ   = @部署ＣＤ" +
                                      "\r\n AND A.コースＣＤ = @コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanCriteria.精算日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanCriteria.コースＣＤ;
                    cmd.Parameters.Add("@弁当区分チケット", SqlDbType.Int).Value = (int)Consts.BentoKubun.Ticket;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

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
        /// 削除可能か？
        /// </summary>
        /// <returns></returns>
        private bool CanDelete()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// DB削除
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Delete()
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 売上精算_入力データ一時保存に保存
                    if (SaveUriageSeisanTemporary(con, tran) == false)
                    {
                        return false;
                    }

                    // 売上精算_ヘッダ削除
                    if (DeleteUriageSeisanHeader(con, tran) == false)
                    {
                        return false;
                    }

                    // 売上精算_売上明細削除
                    if (DeleteUriageSeisanUriageMeisai(con, tran) == false)
                    {
                        return false;
                    }

                    // 売上精算_持出明細削除
                    if (DeleteUriageSeisanMochidashiMeisai(con, tran) == false)
                    {
                        return false;
                    }

                    tran.Commit();
                    log.InfoFormat("売上精算_ヘッダ・売上精算_売上明細、売上精算_持出明細を{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        /// <summary>
        /// 売上精算_入力データ一時保存保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveUriageSeisanTemporary(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 売上精算_入力データ一時保存を生成・保存
                UriageSeisanTemporary uriageSeisanTemporary = new UriageSeisanTemporary(_uriageSeisanHeader);
                bool ret = uriageSeisanTemporary.Insert(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, uriageSeisanTemporary.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "売上精算_入力データ一時保存", uriageSeisanTemporary.Code, historyProcess);
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
        /// 売上精算_ヘッダ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUriageSeisanHeader(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                bool ret = _uriageSeisanHeader.Delete(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, _uriageSeisanHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "売上精算_ヘッダ", _uriageSeisanHeader.Code, historyProcess);
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
        /// 売上精算_売上明細削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUriageSeisanUriageMeisai(SqlConnection con, SqlTransaction tran)
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            try
            {
                // 全削除
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n DELETE FROM 売上精算_売上明細" +
                                  "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                                  "\r\n   AND 精算日     = @精算日" +
                                  "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                                  "\r\n   AND コースＣＤ = @コースＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanHeader.精算日;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanHeader.部署ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanHeader.コースＣＤ;

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                History.Save(con, tran, this.Text, "売上精算_売上明細", _uriageSeisanHeader.Code, historyProcess);

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("売上精算_売上明細が重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上精算_売上明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("売上精算_売上明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 売上精算_持出明細削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUriageSeisanMochidashiMeisai(SqlConnection con, SqlTransaction tran)
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            try
            {
                // 全削除
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n DELETE FROM 売上精算_持出明細" +
                                  "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                                  "\r\n   AND 精算日     = @精算日" +
                                  "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                                  "\r\n   AND コースＣＤ = @コースＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageSeisanHeader.精算日;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanHeader.部署ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_uriageSeisanHeader.コースＣＤ;

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                History.Save(con, tran, this.Text, "売上精算_持出明細", _uriageSeisanHeader.Code, historyProcess);

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("売上精算_持出明細が重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上精算_持出明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("売上精算_持出明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

    }
}
