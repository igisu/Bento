using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// プリンタ選択ダイアログ
    /// </summary>
    public partial class FormPrinterSelect : BaseForm, IDialog
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// フォームを閉じてよいか
        /// </summary>
        private bool _canClose = true;
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 帳票／プリンタ設定マスタ
        /// </summary>
        public PrintSettings PrintSettings { get; private set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="baseReport">レポートのベース</param>
        public FormPrinterSelect(BaseReport baseReport)
        {
            InitializeComponent();

            Type type = baseReport.GetType();
            string reportID = type.Name;

            // 帳票／プリンタ設定マスタから取得する（が、存在しないはず。基本的にこの画面は、マスタにレコードが存在しない場合に呼ばれるので）
            this.PrintSettings = PrintSettings.GetPrintSettings(reportID);
            if (this.PrintSettings == null)
            {
                this.PrintSettings = new PrintSettings();
                this.PrintSettings.帳票ID = reportID;
                this.PrintSettings.プレビューフラグ = true;
            }
            this.PrintSettings.帳票名 = baseReport.TitleWithSize;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="reportID">帳票ID</param>
        /// <param name="reportTitle">帳票タイトルと用紙サイズ</param>
        public FormPrinterSelect(string reportID, string reportTitleWithSize)
        {
            InitializeComponent();

            // 帳票／プリンタ設定マスタから取得する（が、存在しないはず。基本的にこの画面は、マスタにレコードが存在しない場合に呼ばれるので）
            this.PrintSettings = PrintSettings.GetPrintSettings(reportID);
            if (this.PrintSettings == null)
            {
                this.PrintSettings = new PrintSettings();
                this.PrintSettings.帳票ID = reportID;
                this.PrintSettings.プレビューフラグ = true;
            }
            this.PrintSettings.帳票名 = reportTitleWithSize;
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
        private void FormPrinterSelect_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.magnifier_ico;

                // コンボボックスの選択肢生成
                ComboUtil.SetPrinter(cboPrinter);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                printSettingsBindingSource.DataSource = this.PrintSettings;
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPrinterSelect_Shown(object sender, EventArgs e)
        {
            btnSave.Focus();
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPrinterSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            // OKボタンのチェックに引っかかった？
            if (_canClose == false)
            {
                e.Cancel = true;        // フォームのクローズを今回だけキャンセル
                _canClose = true;       // 終了ボタンなどで閉じれるようにする
            }
        }

        /// <summary>
        /// OKボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // プレビューフラグはバインドしていないので、ここで設定する
                this.PrintSettings.プレビューフラグ = chkPreview.Checked;

                _canClose = true;

                // 設定保存チェックボックスがONの時にプリンタが未選択の場合はエラー
                if (chkDefault.Checked)
                {
                    if (string.IsNullOrEmpty(PrintSettings.プリンタ名))
                    {
                        _canClose = false;
                        MessageBox.Show(this, "設定を保存するにはプリンタを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    Save();
                }

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
        /// 終了ボタンクリック
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
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 帳票／プリンタ設定マスタ保存
                    if (SavePrintSettings(con, tran) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("帳票／プリンタ設定マスタを{0}しました。", historyProcess.DisplayName());

                    return true;
                }
                catch (EmuException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    History.Error(ex);
                    throw new EmuException(ex);
                }
            }
        }

        /// <summary>
        /// 帳票／プリンタ設定マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SavePrintSettings(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (this.PrintSettings.作成担当者ＣＤ == null)
                {
                    UserData userData = UserData.getInstance();
                    this.PrintSettings.担当者ＣＤ = userData.担当者ＣＤ;
                    ret = this.PrintSettings.Insert(con, tran);
                }
                else
                {
                    ret = this.PrintSettings.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, this.PrintSettings.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "帳票／プリンタ設定マスタ", this.PrintSettings.Code, historyProcess);
                return true;
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        #endregion
    }
}
