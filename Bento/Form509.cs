using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 電子決済請求締処理
    /// </summary>
    public partial class Form509 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 電子決済請求締め条件
        /// </summary>
        private PaySeikyuShimeCriteria _paySeikyuShimeCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form509()
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
        private void Form509_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                // コンボボックスの選択肢生成
                ComboUtil.SetUriageKubunPaySeikyuShime(cboUriageKbn);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _paySeikyuShimeCriteria = new PaySeikyuShimeCriteria();
                paySeikyuShimeCriteriaBindingSource.DataSource = _paySeikyuShimeCriteria;

                // 請求日は特締めの場合に入力可能
                dtpSeikyubi.Enabled = chkTokushimeMonth.Checked;
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
        private void Form509_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F9:                       // 処理開始ボタン押下
                        btnExec.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// 売上区分コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboUriageKbn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //***********************************************************************************************************************************************
                // cboUriageKbn.SelectedValueのデータソース更新モードをOnValidationではなくOnPropertyChangedにしている。
                // このことにより、SelectedValueChangedではなくSelectedIndexChangedにおいて、_paySeikyuShimeCriteria.売上区分ＣＤでバインド後の値が格納されている
                //***********************************************************************************************************************************************

                // 請求締め条件が未生成の場合（フォームロード中）はNOP
                if (_paySeikyuShimeCriteria == null)
                {
                    return;
                }

                // 請求日を算出
                GenerateSeikyubi();
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
        /// 特締チェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTokushime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // 週締・月締を問わず、チェックONの場合は請求日を入力可能にする
                CheckBox chkTokushime = (CheckBox)sender;
                dtpSeikyubi.Enabled = chkTokushime.Checked;

                // 特締めがOFFになった場合は請求日を算出
                if (chkTokushime.Checked == false)
                {
                    GenerateSeikyubi();
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
        /// 月締年月と締日 検証後イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTsukishime_Validated(object sender, EventArgs e)
        {
            try
            {
                // 請求日を算出
                GenerateSeikyubi();
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
        /// 処理開始ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_paySeikyuShimeCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_paySeikyuShimeCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _paySeikyuShimeCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 確認メッセージを表示
                if (MessageBox.Show(this, "電子決済請求締め処理を行います。よろしいですか？",
                    this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.MdiParent.Cursor = Cursors.WaitCursor;
                    if (PaySeikyuShimeExec.Run(_paySeikyuShimeCriteria))
                    {
                        MessageBox.Show(this, "締め処理を実行しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                this.MdiParent.Cursor = Cursors.Default;
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
        /// 締区分と特締めフラグに応じた請求日を算出
        /// </summary>
        private void GenerateSeikyubi()
        {
            try
            {
                DateTime? seikyubi = null;

                if (chkTokushimeMonth.Checked)      // 特締チェックがONの場合は、入力済みの請求日を優先する
                {
                    return;
                }

                // 入力されている年月と締日から求める
                if (_paySeikyuShimeCriteria["月締年"] == null &&
                    _paySeikyuShimeCriteria["月締月"] == null &&
                    _paySeikyuShimeCriteria["月締日"] == null)
                {
                    if (_paySeikyuShimeCriteria.月締日 == 99)
                    {
                        seikyubi = new DateTime((int)_paySeikyuShimeCriteria.月締年, (int)_paySeikyuShimeCriteria.月締月, DateTime.DaysInMonth((int)_paySeikyuShimeCriteria.月締年, (int)_paySeikyuShimeCriteria.月締月));
                    }
                    else
                    {
                        seikyubi = new DateTime((int)_paySeikyuShimeCriteria.月締年, (int)_paySeikyuShimeCriteria.月締月, (int)_paySeikyuShimeCriteria.月締日);
                    }
                }

                // 求めた請求日を表示
                _paySeikyuShimeCriteria.請求日 = seikyubi;
                ReadBinding(dtpSeikyubi);       // コントロールに反映
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
