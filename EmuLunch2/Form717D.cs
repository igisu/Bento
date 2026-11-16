using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 仕訳マスタメンテ詳細
    /// </summary>
    public partial class Form717D : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用仕訳マスタ
        /// </summary>
        private Shiwake _shiwake;

        /// <summary>
        /// 初期状態用仕訳マスタ
        /// </summary>
        private Shiwake _origin;


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
        /// <param name="shiwake"></param>
        public Form717D(Shiwake shiwake)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _shiwake = new Shiwake(shiwake);

                // 部署ＣＤ、売上区分、入金方法（つまりコンボボックス）については、Null を -1（先頭の選択肢） にしておく
                _shiwake.部署ＣＤ = _shiwake.部署ＣＤ ?? -1;
                _shiwake.売上区分 = _shiwake.売上区分 ?? -1;
                _shiwake.入金方法 = _shiwake.入金方法 ?? -1;

                _IsNewRecord = false;
                if (_shiwake.仕訳ＣＤ == null)
                {
                    _IsNewRecord = true;
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
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form717D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnDelete.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);
                ComboUtil.SetSyukeiTaisyoKubun(cboSyukeiKbn);
                ComboUtil.SetUriageKubun(cboUriageKbn);
                ComboUtil.SetNyukinHoho(cboNyukinHoho);
                SetHojoCombo();

                // 新規の場合は削除不可
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
                    // 既存の場合は部署CDと仕訳CDは変更不可
                    cboBusyo.Enabled = false;
                    txtShiwakeCD.Enabled = false;
                }

                // 初期状態として保持
                _origin = new Shiwake(_shiwake);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                shiwakeBindingSource.DataSource = _shiwake;
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
        private void Form717D_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.F9:               // 登録or更新
                        WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                        btnSave.PerformClick();
                        break;

                    case Keys.F11:              // 削除
                        btnDelete.Focus();
                        btnDelete.PerformClick();
                        break;

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
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form717D_FormClosing(object sender, FormClosingEventArgs e)
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

                if (_shiwake.Equals(_origin) == false)
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
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
        /// 集計対象区分コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboSyukeiKbn_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 集計対象区分が0:売上の場合は、売上区分が必須（チケット束フラグも必須だが、boolなのでnullはfalseとみなす）
                // 集計対象区分が1:入金の場合は、入金方法が必須
                bool isNyukin = ((int?)cboSyukeiKbn.SelectedValue == (int)Consts.SyukeiTaisyoKubun.Nyukin);  // 入金かどうか
                cboUriageKbn.Enabled = !isNyukin;   // 入金の場合と逆
                chkTicket.Enabled = !isNyukin;
                cboNyukinHoho.Enabled = isNyukin;
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
        /// 登録ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CanSave() == false)
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
        /// 削除ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(this, "このデータを削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                if (_shiwake.Delete() == false)
                {
                    MessageBox.Show(this, _shiwake.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                History.Save(this.Text, "仕訳マスタ", _shiwake.Code, History.ProcessType.Delete);

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
        /// 補助コンボ作成
        /// </summary>
        private void SetHojoCombo()
        {
            try
            {
                // コンボボックスの選択肢を設定
                cboKariHojo.DataSource = Shiwake.HOJO_OPTION.ToArray();
                cboKashiHojo.DataSource = Shiwake.HOJO_OPTION.ToArray();
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

        /// <summary>
        /// 登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSave()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_shiwake.HasErrors)
                {
                    MessageBox.Show(this, _shiwake.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

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

        /// <summary>
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _shiwake.Insert();
                }
                else
                {
                    ret = _shiwake.Update();
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _shiwake.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(this.Text, "仕訳マスタ", _shiwake.Code, historyProcess);

                MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        /// <summary>
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                _shiwake = Shiwake.GetShiwake((int)_shiwake.部署ＣＤ, (int)_shiwake.仕訳ＣＤ);

                // 部署ＣＤ、売上区分、入金方法（つまりコンボボックス）については、Null を -1（先頭の選択肢） にしておく
                _shiwake.部署ＣＤ = _shiwake.部署ＣＤ ?? -1;
                _shiwake.売上区分 = _shiwake.売上区分 ?? -1;
                _shiwake.入金方法 = _shiwake.入金方法 ?? -1;

                _origin = new Shiwake(_shiwake);

                _IsNewRecord = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                shiwakeBindingSource.DataSource = _shiwake;

                lblMode.Text = MODE_CAPTION_EDIT;
                lblMode.BackColor = BACK_COLOR_EDIT;
                btnSave.Text = "更新(F9)";
                // 既存の場合は部署CDと仕訳CDは変更不可
                cboBusyo.Enabled = false;
                txtShiwakeCD.Enabled = false;

                btnDelete.Enabled = this.HasAuthEdit;
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
