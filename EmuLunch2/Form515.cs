using Emu.Bento.Common;
using Emu.Bento.Data;
using Emu.Bento.WebAPI;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emu.Bento
{
    /// <summary>
    /// 中間サーバー問合せ結果
    /// </summary>
    public partial class Form515 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// 請求合計
        /// </summary>
        private SeikyuGokei _seikyuGokei;

        /// <summary>
        /// 処理中かどうか
        /// </summary>
        private bool _isProceccing = false;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form515(SeikyuGokei seikyuGokei)
        {
            InitializeComponent();
            _seikyuGokei = seikyuGokei;
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
        private async void Form515_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                this.MdiParent.Cursor = Cursors.WaitCursor;

                lblMessage.Text = "サーバー問合せ中...";

                // API通信
                await InquiryExec();
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
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form515_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_isProceccing)
                {
                    MessageBox.Show(this, "処理中のため、画面を閉じることはできません。\n処理が完了するまでお待ちください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
                }

                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
        private void Form515_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // 終了はフォームのCancelButtonなのでEscキーとしていたが、
                // (ボタンのクリックではなく)キー押下でフォームを閉じたときに親フォームがアクティブにならないという問題がある。
                // これを回避するには当該キー押下時に当該ボタンにフォーカスすればよいのだが、
                // CancelButtonだとイベントが発生せずEscキー押下を検知できず、ボタンにフォーカスする契機が無い。
                case Keys.Escape:
                    btnQuit.Focus();
                    btnQuit.PerformClick();
                    break;
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
                if (_isProceccing)
                {
                    MessageBox.Show(this, "処理中のため、画面を閉じることはできません。\n処理が完了するまでお待ちください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
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
        /// 問い合わせ処理
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EmuException"></exception>
        private async Task InquiryExec()
        {
            try
            {
                bool result = false;
                string message = null;
                FRSettlementHistory frHistory = new FRSettlementHistory();

                btnQuit.Enabled = false;
                _isProceccing = true;

                await Task.Run(() =>
                {
                    FRSettlement fRSettlement = new FRSettlement();
                    result = fRSettlement.InfoGetSettlement(_seikyuGokei, out frHistory, out message);
                });

                if (result)
                {
                    lblMessage.Text = "中間サーバーへの問い合わせに成功しました。";
                }
                else
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "中間サーバーへの問い合わせに失敗しました。\n" + message;
                }

                if (frHistory.請求番号 != null)
                {
                    frHistory.請求番号 = Math.Floor(frHistory.請求番号.Value);  // 小数点除去
                }

                bindingSource1.DataSource = frHistory;

                if (frHistory.処理結果 == (int)Consts.FRProcessResult.Fail)
                {
                    lblResult.ForeColor = Color.Red;
                }
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
            finally
            {
                btnQuit.Enabled = true;
                _isProceccing = false;
            }
        }

        #endregion
    }
}
