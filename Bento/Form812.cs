using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 月次売掛残高編集
    /// </summary>
    public partial class Form812 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 編集対象月次売掛残高
        /// </summary>
        private GetsujiUrikakeZan _getsujiUrikakeZan;

        /// <summary>
        /// 編集用月次売掛残高
        /// </summary>
        private GetsujiUrikakeZanEdit _getsujiUrikakeZanEdit;

        /// <summary>
        /// 編集用月次売掛残高の直前の値
        /// </summary>
        private GetsujiUrikakeZanEdit _prevGetsujiUrikakeZanEdit;

        /// <summary>
        /// 初期状態用月次売掛残高
        /// </summary>
        private GetsujiUrikakeZanEdit _origin;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="getsujiUrikakeZan">編集対象月次売掛残高</param>
        public Form812(GetsujiUrikakeZan getsujiUrikakeZan)
        {
            try
            {
                InitializeComponent();

                _getsujiUrikakeZan = getsujiUrikakeZan;
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
        private void Form812_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.chart_curve_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // 編集用月次売掛残高データを取得
                GetData();
                _prevGetsujiUrikakeZanEdit = new GetsujiUrikakeZanEdit(_getsujiUrikakeZanEdit);    // 直前値保持用を生成
                _origin = new GetsujiUrikakeZanEdit(_getsujiUrikakeZanEdit);              // 初期状態用を生成

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                getsujiUrikakeZanEditBindingSource.DataSource = _getsujiUrikakeZanEdit;

                // グループボックスの見出し（yyyy年M月集計分）
                if (_getsujiUrikakeZanEdit.前々回集計年 != null)
                {
                    grpZenZen.Text = grpZenZen.Text + string.Format("（{0}年{1}月集計分）", _getsujiUrikakeZanEdit.前々回集計年, _getsujiUrikakeZanEdit.前々回集計月);
                }
                if (_getsujiUrikakeZanEdit.前回集計年 != null)
                {
                    grpZen.Text = grpZen.Text + string.Format("（{0}年{1}月集計分）", _getsujiUrikakeZanEdit.前回集計年, _getsujiUrikakeZanEdit.前回集計月);
                }
                if (_getsujiUrikakeZanEdit.集計年 != null)
                {
                    grpKon.Text = grpKon.Text + string.Format("（{0}年{1}月集計分）", _getsujiUrikakeZanEdit.集計年, _getsujiUrikakeZanEdit.集計月);
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
        private void Form812_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.F9:               // 更新
                        WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                        btnSave.PerformClick();
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
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form812_FormClosing(object sender, FormClosingEventArgs e)
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

                if (_getsujiUrikakeZanEdit.Equals(_origin) == false)
                {
                    DialogResult r = MessageBox.Show(this, "変更されています。更新しますか？", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
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
        /// 前回売掛残高・入金額・当月売上額検証終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtZenkaiUrikakeNyukinUriage_Validated(object sender, EventArgs e)
        {
             // 明示的に変更された場合のみ計算する
            if ((_getsujiUrikakeZanEdit.先月売掛残高 ?? 0) != (_prevGetsujiUrikakeZanEdit.先月売掛残高 ?? 0) ||
                (_getsujiUrikakeZanEdit.今月入金額 ?? 0) != (_prevGetsujiUrikakeZanEdit.今月入金額 ?? 0) ||
                (_getsujiUrikakeZanEdit.今月売上額 ?? 0) != (_prevGetsujiUrikakeZanEdit.今月売上額 ?? 0))
            {
                // 今月売掛残高＝先月売掛残高－今月入金額＋今月売上額
                _getsujiUrikakeZanEdit.今月売掛残高 = (_getsujiUrikakeZanEdit.先月売掛残高 ?? 0) - (_getsujiUrikakeZanEdit.今月入金額 ?? 0) + (_getsujiUrikakeZanEdit.今月売上額 ?? 0);
                ReadBinding(txtUrikakeZan);
                _prevGetsujiUrikakeZanEdit = new GetsujiUrikakeZanEdit(_getsujiUrikakeZanEdit);        // 直前の値を保持しなおす
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
        /// <summary>
        /// 月次売掛残高から編集用データを取得
        /// </summary>
        private void GetData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     Z0.企業ＣＤ" +
                                      "\r\n     ,Z0.集計年" +
                                      "\r\n     ,Z0.集計月" +
                                      "\r\n     ,Z0.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,Z0.請求先ＣＤ" +
                                      "\r\n     ,Z0.請求先名" +
                                      "\r\n     ,Z0.先月売掛残高" +
                                      "\r\n     ,Z0.今月入金額" +
                                      "\r\n     ,Z0.今月売上額" +
                                      "\r\n     ,Z0.今月売掛残高" +
                                      "\r\n     ,Z0.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,Z0.作成日" +
                                      "\r\n     ,Z0.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,Z0.修正日" +
                                      "\r\n     ,Z1.集計年 AS 前回集計年" +
                                      "\r\n     ,Z1.集計月 AS 前回集計月" +
                                      "\r\n     ,Z1.先月売掛残高 AS 前回先月売掛残高" +
                                      "\r\n     ,Z1.今月入金額 AS 前回今月入金額" +
                                      "\r\n     ,Z1.今月売上額 AS 前回今月売上額" +
                                      "\r\n     ,Z1.今月売掛残高 AS 前回今月売掛残高" +
                                      "\r\n     ,Z2.集計年 AS 前々回集計年" +
                                      "\r\n     ,Z2.集計月 AS 前々回集計月" +
                                      "\r\n     ,Z2.先月売掛残高 AS 前々回先月売掛残高" +
                                      "\r\n     ,Z2.今月入金額 AS 前々回今月入金額" +
                                      "\r\n     ,Z2.今月売上額 AS 前々回今月売上額" +
                                      "\r\n     ,Z2.今月売掛残高 AS 前々回今月売掛残高" +
                                      "\r\n FROM" +
                                      "\r\n     月次売掛残高集計 AS Z0" +
                                      "\r\n     LEFT JOIN 月次売掛残高集計 AS Z1" +
                                      "\r\n         ON Z0.企業ＣＤ = Z1.企業ＣＤ" +
                                      "\r\n         AND Z0.部署ＣＤ = Z1.部署ＣＤ" +
                                      "\r\n         AND Z0.請求先ＣＤ = Z1.請求先ＣＤ" +
                                      "\r\n         AND Z1.集計年 = @前回集計年" +
                                      "\r\n         AND Z1.集計月 = @前回集計月" +
                                      "\r\n     LEFT JOIN 月次売掛残高集計 AS Z2" +
                                      "\r\n         ON Z0.企業ＣＤ = Z2.企業ＣＤ" +
                                      "\r\n         AND Z0.部署ＣＤ = Z2.部署ＣＤ" +
                                      "\r\n         AND Z0.請求先ＣＤ = Z2.請求先ＣＤ" +
                                      "\r\n         AND Z2.集計年 = @前々回集計年" +
                                      "\r\n         AND Z2.集計月 = @前々回集計月" +
                                      "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = Z0.企業ＣＤ AND B.部署ＣＤ = Z0.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = Z0.企業ＣＤ AND Z0.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = Z0.企業ＣＤ AND Z0.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     Z0.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND Z0.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND Z0.請求先ＣＤ = @請求先ＣＤ" +
                                      "\r\n     AND Z0.集計年 = @集計年" +
                                      "\r\n     AND Z0.集計月 = @集計月";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _getsujiUrikakeZan.部署ＣＤ;
                    cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _getsujiUrikakeZan.請求先ＣＤ;
                    cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = _getsujiUrikakeZan.集計年;
                    cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = _getsujiUrikakeZan.集計月;

                    // 集計年・月に対する前月を算出
                    DateTime lastMonth = new DateTime((int)_getsujiUrikakeZan.集計年, (int)_getsujiUrikakeZan.集計月, 1);
                    lastMonth = lastMonth.AddMonths(-1);
                    cmd.Parameters.Add("@前回集計年", SqlDbType.Int).Value = lastMonth.Year;
                    cmd.Parameters.Add("@前回集計月", SqlDbType.Int).Value = lastMonth.Month;

                    // さらに前月（集計年・月に対する前々月）を算出
                    lastMonth = lastMonth.AddMonths(-1);
                    cmd.Parameters.Add("@前々回集計年", SqlDbType.Int).Value = lastMonth.Year;
                    cmd.Parameters.Add("@前々回集計月", SqlDbType.Int).Value = lastMonth.Month;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("月次売掛残高");
                    adp.Fill(dt);

                    // データテーブルの先頭行から月次売掛残高を生成
                    _getsujiUrikakeZanEdit = new GetsujiUrikakeZanEdit(dt.Rows[0]);
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

                if (_getsujiUrikakeZanEdit.HasErrors)
                {
                    MessageBox.Show(this, _getsujiUrikakeZanEdit.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                ret = _getsujiUrikakeZanEdit.Update();

                if (ret == false)
                {
                    MessageBox.Show(this, _getsujiUrikakeZanEdit.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(this.Text, "月次売掛残高", _getsujiUrikakeZanEdit.Code, historyProcess);
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

        /// <summary>
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                // 編集用月次売掛残高データを取得
                GetData();
                _prevGetsujiUrikakeZanEdit = new GetsujiUrikakeZanEdit(_getsujiUrikakeZanEdit);    // 直前値保持用を生成
                _origin = new GetsujiUrikakeZanEdit(_getsujiUrikakeZanEdit);              // 初期状態用を生成

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                getsujiUrikakeZanEditBindingSource.DataSource = _getsujiUrikakeZanEdit;
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
