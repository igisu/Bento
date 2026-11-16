using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 請求鑑編集
    /// </summary>
    public partial class Form502 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 編集対象請求合計
        /// </summary>
        private SeikyuGokei _seikyuGokei;

        /// <summary>
        /// 編集用請求鑑
        /// </summary>
        private SeikyuKagami _seikyuKagami;

        /// <summary>
        /// 編集用請求鑑の直前の値
        /// </summary>
        private SeikyuKagami _prevSeikyuKagami;

        /// <summary>
        /// 初期状態用請求鑑
        /// </summary>
        private SeikyuKagami _origin;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="seikyuGokei">編集対象請求合計</param>
        public Form502(SeikyuGokei seikyuGokei)
        {
            try
            {
                InitializeComponent();

                _seikyuGokei = seikyuGokei;
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
        private void Form502_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // 鑑データを取得
                GetKagami();
                _prevSeikyuKagami = new SeikyuKagami(_seikyuKagami);    // 直前値保持用を生成
                _origin = new SeikyuKagami(_seikyuKagami);              // 初期状態用を生成

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                seikyuKagamiBindingSource.DataSource = _seikyuKagami;

                // グループボックスの見出し（yyyy年M月d日請求分）
                if (_seikyuKagami.前々回請求日 != null)
                {
                    grpZenZen.Text = grpZenZen.Text + ((DateTime)_seikyuKagami.前々回請求日).ToString("（yyyy年M月d日請求分）");
                }
                if (_seikyuKagami.前回請求日 != null)
                {
                    grpZen.Text = grpZen.Text + ((DateTime)_seikyuKagami.前回請求日).ToString("（yyyy年M月d日請求分）");
                }
                if (_seikyuKagami.請求日 != null)
                {
                    grpKon.Text = grpKon.Text + ((DateTime)_seikyuKagami.請求日).ToString("（yyyy年M月d日請求分）");
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
        private void Form502_KeyDown(object sender, KeyEventArgs e)
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
        private void Form502_FormClosing(object sender, FormClosingEventArgs e)
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

                if (_seikyuKagami.Equals(_origin) == false)
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
        /// 前回請求額・入金額検証終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtZenkaiSeikyuNyukinAmount_Validated(object sender, EventArgs e)
        {
            // 明示的に変更された場合のみ計算する
            if ((_seikyuKagami.前回請求額 ?? 0) != (_prevSeikyuKagami.前回請求額 ?? 0) ||
                (_seikyuKagami.入金額 ?? 0) != (_prevSeikyuKagami.入金額 ?? 0))
            {
                // 繰越額＝前月請求額－入金額
                txtCarryover.Text = string.Format("{0:N0}", (_seikyuKagami.前回請求額 ?? 0) - (_seikyuKagami.入金額 ?? 0));
                _seikyuKagami.繰越額 = (_seikyuKagami.前回請求額 ?? 0) - (_seikyuKagami.入金額 ?? 0);

                // 当月請求額＝繰越額＋当月売上額＋調整額＋消費税
                txtSeikyuZandaka.Text = string.Format("{0:N0}", (_seikyuKagami.繰越額 ?? 0) + (_seikyuKagami.買上金額 ?? 0) + (_seikyuKagami.調整額 ?? 0) + (_seikyuKagami.消費税 ?? 0));
                _seikyuKagami.今回請求額 = (_seikyuKagami.繰越額 ?? 0) + (_seikyuKagami.買上金額 ?? 0) + (_seikyuKagami.調整額 ?? 0) + (_seikyuKagami.消費税 ?? 0);

                _prevSeikyuKagami = new SeikyuKagami(_seikyuKagami);        // 直前の値を保持しなおす
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
            if ((_seikyuKagami.繰越額 ?? 0) != (_prevSeikyuKagami.繰越額 ?? 0) ||
                (_seikyuKagami.買上金額 ?? 0) != (_prevSeikyuKagami.買上金額 ?? 0) ||
                (_seikyuKagami.調整額 ?? 0) != (_prevSeikyuKagami.調整額 ?? 0) ||
                (_seikyuKagami.消費税 ?? 0) != (_prevSeikyuKagami.消費税 ?? 0))
            {
                // 当月請求額＝繰越額＋当月売上額＋調整額＋消費税
                txtSeikyuZandaka.Text = string.Format("{0:N0}", (_seikyuKagami.繰越額 ?? 0) + (_seikyuKagami.買上金額 ?? 0) + (_seikyuKagami.調整額 ?? 0) + (_seikyuKagami.消費税 ?? 0));
                _seikyuKagami.今回請求額 = (_seikyuKagami.繰越額 ?? 0) + (_seikyuKagami.買上金額 ?? 0) + (_seikyuKagami.調整額 ?? 0)  + (_seikyuKagami.消費税 ?? 0);

                _prevSeikyuKagami = new SeikyuKagami(_seikyuKagami);        // 直前の値を保持しなおす
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
        /// 請求合計から鑑データを取得
        /// </summary>
        private void GetKagami()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     K.企業ＣＤ" +
                                      "\r\n     ,K.請求番号" +
                                      "\r\n     ,K.請求日" +
                                      "\r\n     ,K.締区分" +
                                      "\r\n     ,K.部署ＣＤ" +
                                      "\r\n     ,K.請求先ＣＤ" +
                                      "\r\n     ,K.請求先名" +
                                      "\r\n     ,K.前回請求額" +
                                      "\r\n     ,K.入金額" +
                                      "\r\n     ,K.繰越額" +
                                      "\r\n     ,K.買上金額" +
                                      "\r\n     ,K.調整額" +
                                      "\r\n     ,K.消費税" +
                                      "\r\n     ,K.今回請求額" +
                                      "\r\n     ,K.回収予定日" +
                                      "\r\n     ,K.印刷者ＣＤ" +
                                      "\r\n     ,K.印刷日時" +
                                      "\r\n     ,K.EB作成済フラグ" +
                                      "\r\n     ,K.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,K.作成日" +
                                      "\r\n     ,K.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,K.修正日" +
                                      "\r\n     ,Z1.請求日 AS 前回請求日" +
                                      "\r\n     ,Z1.前回請求額 AS 前回前回請求額" +
                                      "\r\n     ,Z1.入金額 AS 前回入金額" +
                                      "\r\n     ,Z1.繰越額 AS 前回繰越額" +
                                      "\r\n     ,Z1.買上金額 AS 前回買上金額" +
                                      "\r\n     ,Z1.調整額 AS 前回調整額" +
                                      "\r\n     ,Z1.消費税 AS 前回消費税" +
                                      "\r\n     ,Z1.今回請求額 AS 前回今回請求額" +
                                      "\r\n     ,Z2.請求日 AS 前々回請求日" +
                                      "\r\n     ,Z2.前回請求額 AS 前々回前回請求額" +
                                      "\r\n     ,Z2.入金額 AS 前々回入金額" +
                                      "\r\n     ,Z2.繰越額 AS 前々回繰越額" +
                                      "\r\n     ,Z2.買上金額 AS 前々回買上金額" +
                                      "\r\n     ,Z2.調整額 AS 前々回調整額" +
                                      "\r\n     ,Z2.消費税 AS 前々回消費税" +
                                      "\r\n     ,Z2.今回請求額 AS 前々回今回請求額" +
                                      "\r\n FROM" +
                        // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                                      "\r\n     (SELECT" +
                                      "\r\n         T1.企業ＣＤ" +
                                      "\r\n         ,T1.請求番号 AS 今回請求番号" +
                                      "\r\n         ,T1.請求先ＣＤ" +
                                      "\r\n         ,MAX(P1.請求日) AS 前回請求日" +
                                      "\r\n         ,MAX(P2.請求日) AS 前々回請求日" +
                                      "\r\n     FROM" +
                                      "\r\n         請求合計 AS T1" +
                                      "\r\n         LEFT JOIN 請求合計 AS P1" +
                                      "\r\n             ON T1.企業ＣＤ = P1.企業ＣＤ" +
                                      "\r\n             AND T1.請求先ＣＤ = P1.請求先ＣＤ" +
                                      "\r\n             AND P1.請求日 < T1.請求日" +
                                      "\r\n         LEFT JOIN 請求合計 AS P2" +
                                      "\r\n             ON P2.企業ＣＤ = P1.企業ＣＤ" +
                                      "\r\n             AND P2.請求先ＣＤ = P1.請求先ＣＤ" +
                                      "\r\n             AND P2.請求日 < P1.請求日" +
                                      "\r\n     WHERE" +
                                      "\r\n         T1.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND T1.請求番号 = @請求番号" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         T1.企業ＣＤ" +
                                      "\r\n         ,T1.請求番号" +
                                      "\r\n         ,T1.請求先ＣＤ" +
                                      "\r\n     ) AS T" +
                        // -- ②今回請求の詳細
                                      "\r\n     INNER JOIN 請求合計 K ON" +
                                      "\r\n         K.企業ＣＤ = T.企業ＣＤ" +
                                      "\r\n         AND K.請求番号 = T.今回請求番号" +
                        // -- ③前回請求の詳細
                                      "\r\n     LEFT JOIN 請求合計 Z1 ON" +
                                      "\r\n         Z1.企業ＣＤ = T.企業ＣＤ" +
                                      "\r\n         AND Z1.請求先ＣＤ = T.請求先ＣＤ" +
                                      "\r\n         AND Z1.請求日 = T.前回請求日" +
                        // -- ③前々回請求の詳細
                                      "\r\n     LEFT JOIN 請求合計 Z2 ON" +
                                      "\r\n         Z2.企業ＣＤ = T.企業ＣＤ" +
                                      "\r\n         AND Z2.請求先ＣＤ = T.請求先ＣＤ" +
                                      "\r\n         AND Z2.請求日 = T.前々回請求日" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = K.企業ＣＤ AND K.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = K.企業ＣＤ AND K.修正担当者ＣＤ = A2.担当者ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = _seikyuGokei.請求番号;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("請求合計");
                    adp.Fill(dt);

                    // データテーブルの先頭行から請求鑑を生成
                    _seikyuKagami = new SeikyuKagami(dt.Rows[0]);
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

                if (_seikyuKagami.HasErrors)
                {
                    MessageBox.Show(this, _seikyuKagami.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

                ret = _seikyuKagami.Update();

                if (ret == false)
                {
                    MessageBox.Show(this, _seikyuKagami.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(this.Text, "請求合計", _seikyuKagami.Code, historyProcess);
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
                // 鑑データを取得
                GetKagami();
                _prevSeikyuKagami = new SeikyuKagami(_seikyuKagami);    // 直前値保持用を生成
                _origin = new SeikyuKagami(_seikyuKagami);              // 初期状態用を生成

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                seikyuKagamiBindingSource.DataSource = _seikyuKagami;
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
