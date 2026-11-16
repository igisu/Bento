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
    /// 請求EBデータ出力
    /// </summary>
    public partial class Form504 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 請求EBデータ作成
        /// </summary>
        private SeikyuDataEB _seikyuDataEB;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form504()
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
        private void Form504_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除
                lstBusyoEB.SelectedIndexChanged -= lstBusyoEB_SelectedIndexChanged; // 出力先EB口座リストイベントをいったん削除

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _seikyuDataEB = new SeikyuDataEB();
                seikyuDataEBbindingSource.DataSource = _seikyuDataEB;

                // 出力先EB口座の選択肢を、部署にぶら下がるものに再設定する
                SetBusyoEB(lstBusyoEB, (int)cboBusyo.SelectedValue);
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
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
                cboBusyo.SelectedValueChanged +=  new EventHandler(cboBusyo_SelectedValueChanged);      // 部署ＣＤコンボイベントを追加
                lstBusyoEB.SelectedIndexChanged += new EventHandler(lstBusyoEB_SelectedIndexChanged);   // 出力先EB口座リストイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form504_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F10:                      // 処理開始ボタン押下
                        btnOutput.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// 部署コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBusyo_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 出力先EB口座の選択肢を、部署にぶら下がるものに再設定する
                SetBusyoEB(lstBusyoEB, (int)cboBusyo.SelectedValue);
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
        /// 出力先EB口座リスト変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstBusyoEB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // ***********************************************************************************************
                // SelectedValueChangedだと、選択したインデックスと異なる行が bindingSource.Current になっている。
                // SelectedIndexChangedだと、そうはならない
                // ***********************************************************************************************

                // 選択されている出力先EB口座の引落日をデフォルト表示する
                ShowHikiotoshibi();
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
        /// 得意先ＣＤFromキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCDFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCDFrom.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤFrom参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCDFrom_Click(object sender, EventArgs e)
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
                    errorProvider1.SetError(txtTokuisakiCDFrom, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtTokuisakiCDFrom.Text = tokuisaki.得意先ＣＤ.ToString();
                    _seikyuDataEB.請求先ＣＤFrom = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiNameFrom.Text = tokuisaki.得意先名;
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
        /// 得意先ＣＤToキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCDTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCDTo.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤTo参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCDTo_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCDTo_FormClosed);
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤTo）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCDTo_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtTokuisakiCDTo, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtTokuisakiCDTo.Text = tokuisaki.得意先ＣＤ.ToString();
                    _seikyuDataEB.請求先ＣＤTo = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiNameTo.Text = tokuisaki.得意先名;
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
        /// 出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutput_Click(object sender, EventArgs e)
        {
            try
            {
                // 口座引落EBデータが出力可能？
                if (CanOutput() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                OutputEB();       // EBデータ出力
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
        /// 出力先EB口座リスト設定
        /// </summary>
        /// <param name="lst">出力先EB口座リスト</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        private void SetBusyoEB(ListBox lst, int? busyoCD)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.銀行ＣＤ" +
                                      "\r\n     ,A.支店ＣＤ" +
                                      "\r\n     ,A.銀行ＣＤ + A.支店ＣＤ AS 銀行支店ＣＤ" +
                                      "\r\n     ,A.引落日" +
                                      "\r\n     ,A.EBセット名" +
                                      "\r\n FROM" +
                                      "\r\n     部署EB設定マスタ AS A" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.銀行ＣＤ" +
                                      "\r\n     ,A.支店ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD ?? -1;  // null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("部署EB設定マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    lst.BeginUpdate();      // 描画を一旦停止

                    busyoEBbindingSource.DataSource = ds;
                    busyoEBbindingSource.DataMember = "部署EB設定マスタ";

                    lst.DisplayMember = "EBセット名";
                    lst.ValueMember = "銀行支店ＣＤ";

                    // 選択されている出力先EB口座の引落日をデフォルト表示する
                    ShowHikiotoshibi();

                    log.Info("部署EB設定マスタを検索しました。");
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
            finally
            {
                lst.EndUpdate();        // 描画再開
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 選択されている出力先EB口座の引落日をデフォルト表示する
        /// </summary>
        private void ShowHikiotoshibi()
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime hikiotoshibiDate = DateTime.Today;
                if (busyoEBbindingSource.DataSource != null && busyoEBbindingSource.Current != null)
                {
                    DataRow dr = ((DataRowView)busyoEBbindingSource.Current).Row;
                    int? hikiotoshibi = dr["引落日"] as int?;
                    if (hikiotoshibi != null)
                    {
                        if (hikiotoshibi == 99)
                        {
                            hikiotoshibiDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                        }
                        else
                        {
                            hikiotoshibiDate = new DateTime(today.Year, today.Month, (int)hikiotoshibi);
                        }
                    }
                }

                // 土日なら翌月曜にする
                if (hikiotoshibiDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    hikiotoshibiDate = hikiotoshibiDate.AddDays(2);
                }
                if (hikiotoshibiDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    hikiotoshibiDate = hikiotoshibiDate.AddDays(1);
                }

                // 求めた引落日を表示
                if (_seikyuDataEB != null)
                {
                    _seikyuDataEB.引落日 = hikiotoshibiDate;
                    ReadBinding(dtpHikiotoshibi);       // コントロールに反映
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
        /// 口座引落EBデータが出力可能か？
        /// </summary>
        /// <returns></returns>
        private bool CanOutput()
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seikyuDataEB.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuDataEB.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuDataEB.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return false;
                }

                // 年月が（システム日付＋未来月数警告閾値）を超えていれば警告メッセージを表示
                DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime warnDate = currentMonth.AddMonths(SystemMaster.GetWarningFutureMonths());
                DateTime targetMonth = new DateTime((int)_seikyuDataEB.対象年, (int)_seikyuDataEB.対象月, 1);
                if (targetMonth > warnDate)
                {
                    if (MessageBox.Show(this, "年月として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return false;
                    }
                }

                // 引落日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_seikyuDataEB.引落日 > warnDate)
                {
                    if (MessageBox.Show(this, "引落日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return false;
                    }
                }

                // EB先銀行支店ＣＤは出力先EB口座リストボックスにバインドしていない（※）ので、ここで取得する
                // ※部署を選択し直すとリストの先頭が選択状態になり、部署を確定すると以前に記憶していたEB先銀行支店ＣＤが選択状態になる(選択位置が変化する)ので
                _seikyuDataEB.EB先銀行支店ＣＤ = (string)lstBusyoEB.SelectedValue;

                // EB先銀行支店ＣＤが未選択？
                if (string.IsNullOrEmpty(_seikyuDataEB.EB先銀行支店ＣＤ))
                {
                    MessageBox.Show(this, "出力先ＥＢ口座を選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// EBデータ出力
        /// </summary>
        private void OutputEB()
        {
            try
            {
                // 請求データ取得
                DataTable dtSeikyu = GetSeikyuData();
                if (dtSeikyu.Rows.Count == 0)
                {
                    log.Warn("EBデータ無し");
                    MessageBox.Show(this, "出力する請求EBデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 請求EBデータファイルを生成
                SeikyuEBWriter seikyuEBWriter = new SeikyuEBWriter((int)_seikyuDataEB.部署ＣＤ, _seikyuDataEB.銀行ＣＤ, _seikyuDataEB.支店ＣＤ, (DateTime)_seikyuDataEB.引落日);
                if (seikyuEBWriter.Generate(dtSeikyu) == false)
                {
                    MessageBox.Show(this, seikyuEBWriter.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // ファイル名を決定
                string fileName = string.Format("EBデータ_{0}{1:00}月請求分({2}_{3})", lstBusyoEB.Text, _seikyuDataEB.対象月, seikyuEBWriter.件数, seikyuEBWriter.金額);
                saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                saveFileDialog1.FileName = fileName;
                if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                fileName = saveFileDialog1.FileName;    // ファイルパスとして取得

                // 請求EBデータファイルを出力
                seikyuEBWriter.Output(fileName);

                // 請求合計のEB出力済みフラグを更新
                if (seikyuEBWriter.UpdateSeikyuTotal(this.Text) == false)
                {
                    MessageBox.Show(this, seikyuEBWriter.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 操作履歴に保存
                History.Save(this.Text, "請求EBデータ", null, History.ProcessType.Output);

                MessageBox.Show(this, "請求EBデータ出力が完了しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.None);
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
        /// 請求データ取得
        /// </summary>
        private DataTable GetSeikyuData()
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
                    // SeikyuGokei格納用 ここから
                    sql.AppendLine("     G.企業ＣＤ");
                    sql.AppendLine("     ,G.請求番号");
                    sql.AppendLine("     ,G.請求日");
                    sql.AppendLine("     ,G.締区分");
                    sql.AppendLine("     ,G.部署ＣＤ");
                    sql.AppendLine("     ,G.請求先ＣＤ");
                    sql.AppendLine("     ,G.請求先名");
                    sql.AppendLine("     ,G.前回請求額");
                    sql.AppendLine("     ,G.入金額");
                    sql.AppendLine("     ,G.繰越額");
                    sql.AppendLine("     ,G.買上金額");
                    sql.AppendLine("     ,G.調整額");
                    sql.AppendLine("     ,G.消費税");
                    sql.AppendLine("     ,G.今回請求額");
                    sql.AppendLine("     ,G.回収予定日");
                    sql.AppendLine("     ,G.印刷者ＣＤ");
                    sql.AppendLine("     ,G.印刷日時");
                    sql.AppendLine("     ,G.EB作成済フラグ");
                    sql.AppendLine("     ,G.作成担当者ＣＤ");
                    sql.AppendLine("     ,G.作成日");
                    sql.AppendLine("     ,G.修正担当者ＣＤ");
                    sql.AppendLine("     ,G.修正日");
                    // SeikyuGokei格納用 ここまで
                    sql.AppendLine("     ,G.今回請求額 AS 請求金額");
                    sql.AppendLine("     ,G.請求先ＣＤ AS 得意先ＣＤ");
                    sql.AppendLine("     ,T.得意先名");
                    sql.AppendLine("     ,T.金融機関ＣＤ AS 銀行ＣＤ");
                    sql.AppendLine("     ,K.銀行名カナ");
                    sql.AppendLine("     ,T.金融機関支店ＣＤ AS 支店ＣＤ");
                    sql.AppendLine("     ,KB.支店名カナ");
                    sql.AppendLine("     ,T.口座種別");
                    sql.AppendLine("     ,T.口座番号");
                    sql.AppendLine("     ,T.口座名義人");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     請求合計 AS G");
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS T");
                    sql.AppendLine("         ON G.企業ＣＤ = T.企業ＣＤ");
                    sql.AppendLine("         AND G.請求先ＣＤ = T.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 金融機関名称 AS K ON K.銀行ＣＤ = T.金融機関ＣＤ");
                    sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KB ON KB.銀行ＣＤ = T.金融機関ＣＤ AND KB.支店ＣＤ = T.金融機関支店ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     G.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND T.締区分 = @締区分月締");
                    sql.AppendLine("     AND T.月締日 = @月締日");
                    sql.AppendLine("     AND T.支払方法 = @支払方法振替");
                    sql.AppendLine("     AND T.EB銀行支店ＣＤ = @銀行支店ＣＤ");
                    sql.AppendLine("     AND G.締区分 = @締区分月締");

                    if (_seikyuDataEB.全部署出力フラグ != true)
                    {
                        sql.AppendLine("     AND G.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuDataEB.部署ＣＤ;
                    }

                    if (_seikyuDataEB.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("     AND G.請求先ＣＤ >= @請求先ＣＤFrom");
                        cmd.Parameters.Add("@請求先ＣＤFrom", SqlDbType.Int).Value = _seikyuDataEB.請求先ＣＤFrom;
                    }
                    if (_seikyuDataEB.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("     AND G.請求先ＣＤ <= @請求先ＣＤTo");
                        cmd.Parameters.Add("@請求先ＣＤTo", SqlDbType.Int).Value = _seikyuDataEB.請求先ＣＤTo;
                    }

                    sql.AppendLine("     AND G.請求日 >= @請求年月日From");
                    sql.AppendLine("     AND G.請求日 <= @請求年月日To");

                    // 作成済みのものも対象にする場合は、EB作成済フラグを条件に加えない（通常は作成済みのものは対象外）
                    if (_seikyuDataEB.印刷済みフラグ == null || _seikyuDataEB.印刷済みフラグ == false)
                    {
                        sql.AppendLine("     AND G.EB作成済フラグ = 0");
                    }
                    sql.AppendLine("     AND G.今回請求額 > 0");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     G.請求先ＣＤ,");
                    sql.AppendLine("     G.今回請求額");

                    cmd.CommandText = sql.ToString();

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@締区分月締", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Monthly;
                    cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = _seikyuDataEB.月締日;
                    cmd.Parameters.Add("@支払方法振替", SqlDbType.Int).Value = (int)Consts.ShiharaiHohoKubun.Furikae;
                    cmd.Parameters.Add("@銀行支店ＣＤ", SqlDbType.VarChar).Value = _seikyuDataEB.EB先銀行支店ＣＤ;

                    // 年月
                    DateTime d = new DateTime((int)_seikyuDataEB.対象年, (int)_seikyuDataEB.対象月, 1);
                    cmd.Parameters.Add("@請求年月日From", SqlDbType.Date).Value = d;
                    // 末日を求める
                    d = d.AddMonths(1);
                    d = d.AddDays(-1);
                    cmd.Parameters.Add("@請求年月日To", SqlDbType.Date).Value = d;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("EB作成対象請求データ");
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
        /// 請求データをチェックします。
        /// </summary>
        /// <param name="dtSeikyu">請求データ</param>
        /// <returns>チェックOKの場合はtrue。それ以外はfalse</returns>
        private bool CheckSeikyuData(DataTable dtSeikyu)
        {
            try
            {
                string errorMessage = "";
                foreach (DataRow dr in dtSeikyu.Rows)
                {
                    // 以下の項目をチェックする
                    // ・金融機関ＣＤ AS 銀行ＣＤ
                    // ・銀行名カナ
                    // ・金融機関支店ＣＤ AS 支店ＣＤ
                    // ・支店名カナ
                    // ・口座種別
                    // ・口座番号
                    // ・口座名義人
                    string tokuisakiName = dr["得意先名"] as string;
                    string bankCD = dr["銀行ＣＤ"] as string;
                    string bankName = dr["銀行名カナ"] as string;
                    string branchCD = dr["支店ＣＤ"] as string;
                    string branchName = dr["支店名カナ"] as string;
                    int? kozaSyubetsu = dr["口座種別"] as int?;
                    string kozaNo = dr["口座番号"] as string;
                    string kozaName = dr["口座名義人"] as string;

                    if (string.IsNullOrEmpty(bankCD))
                    {
                        errorMessage = string.Format("得意先「{0}」の金融機関ＣＤが未入力です。処理を中止します。", tokuisakiName);
                        log.Warn(errorMessage);
                        MessageBox.Show(this, errorMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if (string.IsNullOrEmpty(bankName))
                    {
                        errorMessage = string.Format("得意先「{0}」の金融機関ＣＤ【{1}】は存在しません。処理を中止します。", tokuisakiName, bankCD);
                        log.Warn(errorMessage);
                        MessageBox.Show(this, errorMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if (string.IsNullOrEmpty(branchCD))
                    {
                        errorMessage = string.Format("得意先「{0}」の金融機関支店ＣＤが未入力です。処理を中止します。", tokuisakiName);
                        log.Warn(errorMessage);
                        MessageBox.Show(this, errorMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if (string.IsNullOrEmpty(branchName))
                    {
                        errorMessage = string.Format("得意先「{0}」の金融機関支店ＣＤ【{1}】は存在しません。処理を中止します。", tokuisakiName, branchCD);
                        log.Warn(errorMessage);
                        MessageBox.Show(this, errorMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if (kozaSyubetsu == null)
                    {
                        errorMessage = string.Format("得意先「{0}」の口座種別が未入力です。処理を中止します。", tokuisakiName);
                        log.Warn(errorMessage);
                        MessageBox.Show(this, errorMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    if (string.IsNullOrEmpty(kozaNo))
                    {
                        errorMessage = string.Format("得意先「{0}」の口座番号が未入力です。処理を中止します。", tokuisakiName);
                        log.Warn(errorMessage);
                        MessageBox.Show(this, errorMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    // 口座名義人が半角かどうかチェック
                    Tokuisaki tokuisaki = new Tokuisaki();
                    tokuisaki.口座名義人 = kozaName;
                    if (string.IsNullOrEmpty(tokuisaki["口座名義人"]) == false)
                    {
                        errorMessage = string.Format("得意先「{0}」の口座名義人が半角ではありません。処理を中止します。", tokuisakiName);
                        log.Warn(errorMessage);
                        MessageBox.Show(this, errorMessage, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 請求合計のEB出力済みフラグを更新
        /// </summary>
        /// <param name="seikyuBangoList">請求番号リスト</param>
        private bool UpdateSeikyuTotal(List<decimal> seikyuBangoList)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        foreach (decimal seikyuNo in seikyuBangoList)
                        {
                            SqlCommand cmd = con.CreateCommand();
                            cmd.Transaction = tran;
                            cmd.CommandText = "UPDATE 請求合計 SET EB作成済フラグ = 1 WHERE 企業ＣＤ = @企業ＣＤ AND 請求番号 = @請求番号";
                            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                            cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = seikyuNo;
                            log.Debug(cmd.CommandTextWithParameters());
                            cmd.ExecuteNonQuery();

                            // 操作履歴に保存
                            History.Save(con, tran, this.Text, "請求合計", seikyuNo.ToString(), History.ProcessType.Update);
                        }
                        tran.Commit();
                    }
                    catch (SqlException ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            MessageBox.Show(this, "請求合計テーブルが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            History.Error(ex);
                            MessageBox.Show(this, string.Format("請求合計テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Update.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        History.Error(ex);
                        MessageBox.Show(this, string.Format("請求合計の{0}に失敗しました。\n{1}", History.ProcessType.Update.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        #endregion
    }
}
