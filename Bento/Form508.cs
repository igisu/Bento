using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 請求レコード追加
    /// </summary>
    public partial class Form508 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 編集用請求追加
        /// </summary>
        private SeikyuAdd _seikyuAdd;

        /// <summary>
        /// 編集用請求追加の直前の値
        /// </summary>
        private SeikyuAdd _prevSeikyuAdd;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form508()
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
        private void Form508_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.SetShimeKubun(cboShime);
                SetTaxRateCombo();                      // 計算税率のコンボボックスの選択肢を作成

                _seikyuAdd = new SeikyuAdd();
                _prevSeikyuAdd = new SeikyuAdd(_seikyuAdd);    // 直前値保持用を生成

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                seikyuAddBindingSource.DataSource = _seikyuAdd;

                // メタ情報は請求合計から取得するが、それを取得するのは追加後
                seikyuGokeiBindingSource.DataSource = new SeikyuGokei();
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
        private void Form508_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.F3:
                        btnClear.PerformClick();        // クリアボタン押下
                        break;

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
        private void Form508_FormClosing(object sender, FormClosingEventArgs e)
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

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCDFrom_FormClosed);
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤFrom）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCDFrom_FormClosed(object sender, FormClosedEventArgs e)
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
                    _seikyuAdd.請求先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
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
        /// 前回請求額・入金額検証終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtZenkaiSeikyuNyukinAmount_Validated(object sender, EventArgs e)
        {
            // 明示的に変更された場合のみ計算する
            if ((_seikyuAdd.前回請求額 ?? 0) != (_prevSeikyuAdd.前回請求額 ?? 0) ||
                (_seikyuAdd.入金額 ?? 0) != (_prevSeikyuAdd.入金額 ?? 0))
            {
                // 繰越額＝前月請求額－入金額
                txtCarryover.Text = string.Format("{0:N0}", (_seikyuAdd.前回請求額 ?? 0) - (_seikyuAdd.入金額 ?? 0));
                _seikyuAdd.繰越額 = (_seikyuAdd.前回請求額 ?? 0) - (_seikyuAdd.入金額 ?? 0);

                // 当月請求額＝繰越額＋当月売上額＋調整額＋消費税
                txtSeikyuZandaka.Text = string.Format("{0:N0}", (_seikyuAdd.繰越額 ?? 0) + (_seikyuAdd.当月売上額 ?? 0) + (_seikyuAdd.調整額 ?? 0) + (_seikyuAdd.消費税額 ?? 0));
                _seikyuAdd.当月請求額 = (_seikyuAdd.繰越額 ?? 0) + (_seikyuAdd.当月売上額 ?? 0) + (_seikyuAdd.調整額 ?? 0) + (_seikyuAdd.消費税額 ?? 0);

                _prevSeikyuAdd = new SeikyuAdd(_seikyuAdd);        // 直前の値を保持しなおす
            }
        }

        /// <summary>
        /// 繰越額・当月売上額・調整額・消費税検証終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCarryoverMonthUriageAdjustTax_Validated(object sender, EventArgs e)
        {
            // 明示的に変更された場合のみ計算する
            if ((_seikyuAdd.繰越額 ?? 0) != (_prevSeikyuAdd.繰越額 ?? 0) ||
                (_seikyuAdd.当月売上額 ?? 0) != (_prevSeikyuAdd.当月売上額 ?? 0) ||
                (_seikyuAdd.調整額 ?? 0) != (_prevSeikyuAdd.調整額 ?? 0) ||
                (_seikyuAdd.消費税額 ?? 0) != (_prevSeikyuAdd.消費税額 ?? 0))
            {
                // 当月請求額＝繰越額＋当月売上額＋調整額＋消費税
                txtSeikyuZandaka.Text = string.Format("{0:N0}", (_seikyuAdd.繰越額 ?? 0) + (_seikyuAdd.当月売上額 ?? 0) + (_seikyuAdd.調整額 ?? 0) + (_seikyuAdd.消費税額 ?? 0));
                _seikyuAdd.当月請求額 = (_seikyuAdd.繰越額 ?? 0) + (_seikyuAdd.当月売上額 ?? 0) + (_seikyuAdd.調整額 ?? 0) + (_seikyuAdd.消費税額 ?? 0);

                _prevSeikyuAdd = new SeikyuAdd(_seikyuAdd);        // 直前の値を保持しなおす
            }
        }

        /// <summary>
        /// クリアボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                _seikyuAdd = new SeikyuAdd();
                _prevSeikyuAdd = new SeikyuAdd(_seikyuAdd);    // 直前値保持用を生成
                seikyuAddBindingSource.DataSource = _seikyuAdd;

                // メタ情報は請求合計から取得するが、それを取得するのは追加後
                seikyuGokeiBindingSource.DataSource = new SeikyuGokei();

                btnSave.Enabled = this.HasAuthEdit;
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

                decimal seikyuNo;
                if (Save(out seikyuNo))
                {
                    // 再読み込み
                    Reload(seikyuNo);
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
        /// 消費税率コンボ作成
        /// </summary>
        private void SetTaxRateCombo()
        {
            try
            {
                DataTable dt;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     -1 AS 消費税率" +
                                      "\r\n     ,'' AS 表示用消費税率" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 消費税率" +
                                      "\r\n     ,'0.00' AS 表示用消費税率" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     消費税率" +
                                      "\r\n     ,cast(消費税率 as varchar) as 表示用消費税率" +
                                      "\r\n FROM" +
                                      "\r\n     消費税率マスタ" +
                                      "\r\n WHERE" +
                                      "\r\n     施行日 = (SELECT MAX(施行日) FROM 消費税率マスタ)" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     軽減税率 AS 消費税率" +
                                      "\r\n     ,cast(軽減税率 as varchar) as 表示用消費税率" +
                                      "\r\n FROM" +
                                      "\r\n     消費税率マスタ" +
                                      "\r\n WHERE" +
                                      "\r\n     施行日 = (SELECT MAX(施行日) FROM 消費税率マスタ)" +
                                      "\r\n     AND 軽減税率 IS NOT NULL" +
                                      "\r\n ORDER BY" +
                                      "\r\n     消費税率";

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adp.Fill(dt);
                }

                cboTaxRate.DisplayMember = "表示用消費税率";
                cboTaxRate.ValueMember = "消費税率";
                cboTaxRate.DataSource = dt.Copy();
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

                if (_seikyuAdd.HasErrors)
                {
                    MessageBox.Show(this, _seikyuAdd.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 同じ請求日、部署ＣＤ、請求先ＣＤが請求合計に存在しないこと。
                if (IsUnique() == false)
                {
                    MessageBox.Show(this, "追加しようとしている請求データは既に存在しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 請求先がいないこと（子でないこと）。
                Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_seikyuAdd.請求先ＣＤ);
                if (tokuisaki.請求先ＣＤ != null && tokuisaki.得意先ＣＤ != tokuisaki.請求先ＣＤ)   // 請求先が未設定 または 得意先と請求先が同じなら追加可能
                {
                    MessageBox.Show(this, "この請求先には請求データを追加できません。（請求親ではありません）", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 最新の請求日のレコードしか作ってはダメ。（入力された請求日より新しいレコードが有ってはダメ）
                if (IsNewest() == false)
                {
                    MessageBox.Show(this, "追加しようとしている請求データより新しい請求データが存在します。追加できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 同じ請求日、部署ＣＤ、請求先ＣＤが請求合計に存在しないこと
        /// </summary>
        /// <returns>同じ請求日、部署ＣＤ、請求先ＣＤが請求合計に存在しない場合は true</returns>
        private bool IsUnique()
        {
            bool ret = false;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     COUNT(請求番号) AS CNT" +
                                      "\r\n FROM" +
                                      "\r\n     請求合計" +
                                      "\r\n WHERE" +
                                      "\r\n     請求日 = @請求日" +
                                      "\r\n     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND 請求先ＣＤ = @請求先ＣＤ";

                    cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = _seikyuAdd.請求日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuAdd.部署ＣＤ;
                    cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _seikyuAdd.請求先ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if ((int)reader["CNT"] == 0)
                        {
                            ret = true;
                        }
                    }
                }

                return ret;
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
        /// 入力された請求日より新しいレコードが存在しないこと
        /// </summary>
        /// <returns>入力された請求日より新しいレコードが存在しない場合は true</returns>
        private bool IsNewest()
        {
            bool ret = false;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     COUNT(請求番号) AS CNT" +
                                      "\r\n FROM" +
                                      "\r\n     請求合計" +
                                      "\r\n WHERE" +
                                      "\r\n     請求日 > @請求日" +
                                      "\r\n     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND 請求先ＣＤ = @請求先ＣＤ";

                    cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = _seikyuAdd.請求日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuAdd.部署ＣＤ;
                    cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _seikyuAdd.請求先ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if ((int)reader["CNT"] == 0)
                        {
                            ret = true;
                        }
                    }
                }

                return ret;
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
        /// <param name="seikyuNo">出力した請求番号</param>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save(out decimal seikyuNo)
        {
            try
            {
                bool ret = false;

                // 回収予定日算出用の得意先マスタを取得
                Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_seikyuAdd.請求先ＣＤ);

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    // 現在の請求番号を取得
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = "SELECT ISNULL(MAX(請求番号), 0) AS 請求番号 FROM 請求合計 WHERE 企業ＣＤ = @企業ＣＤ";
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        seikyuNo = (decimal)reader["請求番号"];
                    }

                    seikyuNo++;

                    // 請求合計を追加
                    ret = SaveSeikyuGokei(con, tran, seikyuNo, tokuisaki);
                    if (ret == false)
                    {
                        return false;
                    }

                    // 請求ヘッダを追加
                    ret = SaveSeikyuHeader(con, tran, seikyuNo);
                    if (ret == false)
                    {
                        return false;
                    }

                    tran.Commit();
                }

                MessageBox.Show(this, string.Format("{0}しました。", History.ProcessType.Create.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 請求合計追加
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="tokuisaki">得意先マスタ</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveSeikyuGokei(SqlConnection con, SqlTransaction tran, decimal seikyuNo, Tokuisaki tokuisaki)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                SeikyuGokei seikyuGokei = new SeikyuGokei();
                seikyuGokei.請求番号 = seikyuNo;
                seikyuGokei.請求日 = _seikyuAdd.請求日;
                seikyuGokei.締区分 = _seikyuAdd.締区分;
                seikyuGokei.部署ＣＤ = _seikyuAdd.部署ＣＤ;
                seikyuGokei.請求先ＣＤ = _seikyuAdd.請求先ＣＤ;
                seikyuGokei.請求先名 = _seikyuAdd.請求先名;
                seikyuGokei.前回請求額 = _seikyuAdd.前回請求額 ?? 0m;
                seikyuGokei.入金額 = _seikyuAdd.入金額 ?? 0m;
                seikyuGokei.繰越額 = _seikyuAdd.繰越額 ?? 0m;
                seikyuGokei.買上金額 = _seikyuAdd.当月売上額 ?? 0m;
                seikyuGokei.調整額 = _seikyuAdd.調整額 ?? 0m;
                seikyuGokei.消費税 = _seikyuAdd.消費税額 ?? 0m;
                seikyuGokei.今回請求額 = _seikyuAdd.当月請求額 ?? 0m;
                seikyuGokei.回収予定日 = SeikyuShimeExec.GetKaisyubi((int)seikyuGokei.締区分, (DateTime)seikyuGokei.請求日, tokuisaki.支払サイト ?? 0, tokuisaki.支払日);
                seikyuGokei.印刷者ＣＤ = null;
                seikyuGokei.印刷日時 = null;
                seikyuGokei.EB作成済フラグ = false;

                ret = seikyuGokei.Insert(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, seikyuGokei.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(this.Text, "請求合計", seikyuGokei.Code, historyProcess);
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
        /// 請求ヘッダー追加
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveSeikyuHeader(SqlConnection con, SqlTransaction tran, decimal seikyuNo)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                SeikyuHeader seikyuHeader = new SeikyuHeader();
                seikyuHeader.請求番号 = seikyuNo;
                seikyuHeader.得意先ＣＤ = _seikyuAdd.請求先ＣＤ;
                seikyuHeader.部署ＣＤ = _seikyuAdd.部署ＣＤ;
                seikyuHeader.計算税率 = _seikyuAdd.計算税率;
                seikyuHeader.得意先名 = _seikyuAdd.請求先名;
                seikyuHeader.請求日 = _seikyuAdd.請求日;
                seikyuHeader.請求先ＣＤ = _seikyuAdd.請求先ＣＤ;
                seikyuHeader.請求先名 = _seikyuAdd.請求先名;
                seikyuHeader.売上金額 = _seikyuAdd.当月売上額 ?? 0m;
                seikyuHeader.消費税 = _seikyuAdd.消費税額 ?? 0m;
                seikyuHeader.売上金額税込 = (_seikyuAdd.当月売上額 ?? 0m) + (_seikyuAdd.消費税額 ?? 0m);

                ret = seikyuHeader.Insert(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, seikyuHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(this.Text, "請求ヘッダ", seikyuHeader.Code, historyProcess);
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
        /// <param name="seikyuNo">請求番号</param>
        private void Reload(decimal seikyuNo)
        {
            try
            {
                _prevSeikyuAdd = new SeikyuAdd(_seikyuAdd);    // 直前値保持用を生成

                // メタ情報を請求合計から取得
                SeikyuGokei seikyuGokei = SeikyuGokei.GetSeikyuGokei(seikyuNo);
                if (seikyuGokei != null)
                {
                    seikyuGokeiBindingSource.DataSource = seikyuGokei;
                }

                // クリアボタン押下まで追加不可とする
                btnSave.Enabled = false;
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
