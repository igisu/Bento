using System;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 非顧客電話番号削除
    /// </summary>
    public partial class Form716 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 非顧客電話番号マスタ
        /// </summary>
        private HiKokyakuTelNo _hiKokyakuTelNo;

        /// <summary>
        /// 非顧客電話番号マスタ（検索条件用）
        /// </summary>
        private HiKokyakuTelNo _hiKokyakuTelNoCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form716()
        {
            try
            {
                InitializeComponent();
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
        private void Form716_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                // 権限によりボタンを制御
                btnDelete.Enabled = this.HasAuthEdit;

                lblNotFound.Visible = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _hiKokyakuTelNoCriteria = new HiKokyakuTelNo();
                hiKokyakuTelNoCriteriaBindingSource.DataSource = _hiKokyakuTelNoCriteria;

                _hiKokyakuTelNo = new HiKokyakuTelNo();
                hiKokyakuTelNoBindingSource.DataSource = _hiKokyakuTelNo;
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
        private void Form716_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
                {
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                    switch (e.KeyCode)
                    {
                        case Keys.F5:               // 検索
                            btnQuery.PerformClick();
                            break;

                        case Keys.F11:              // 削除
                            btnDelete.PerformClick();
                            break;

                        case Keys.F12:              // 終了
                            btnQuit.PerformClick();
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
                // 電話番号が未入力の場合は何もしない
                lblNotFound.Visible = false;
                if (_hiKokyakuTelNoCriteria.電話番号 == null)
                {
                    return;
                }

                // 非顧客電話番号マスタを検索
                _hiKokyakuTelNo = HiKokyakuTelNo.GetHiKokyakuTelNo(_hiKokyakuTelNoCriteria.電話番号);
                if (_hiKokyakuTelNo == null)
                {
                    //  見つからない場合は空のインスタンスを生成して表示する
                    _hiKokyakuTelNo = new HiKokyakuTelNo();
                    // 「見つかりませんでした」ラベルを表示
                    lblNotFound.Visible = true;
                }
                hiKokyakuTelNoBindingSource.DataSource = _hiKokyakuTelNo;
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
        /// 削除ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索結果が無い？
                if (_hiKokyakuTelNo == null || _hiKokyakuTelNo.電話番号 == null)
                {
                    return;
                }

                if (MessageBox.Show(this, "このデータを削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                if (_hiKokyakuTelNo.Delete() == false)
                {
                    MessageBox.Show(this, _hiKokyakuTelNo.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                History.Save(this.Text, "非顧客電話番号マスタ", _hiKokyakuTelNo.Code, History.ProcessType.Delete);

                // 画面を初期化
                _hiKokyakuTelNo = new HiKokyakuTelNo();
                hiKokyakuTelNoBindingSource.DataSource = _hiKokyakuTelNo;
                lblNotFound.Visible = false;
                txtTel.Focus();
                txtTel.SelectAll();
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

        #endregion
    }
}
