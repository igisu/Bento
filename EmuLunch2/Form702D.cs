using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 商品マスタメンテ詳細
    /// </summary>
    public partial class Form702D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int BUSYO_CD_COLUMN   = 0;        // 部署ＣＤ列インデックス
        private const int BUSYO_NAME_COLUMN = 1;        // 部署名列インデックス
        private const int SYOHIN_GROUP_CD_COLUMN   = 2; // 商品グループＣＤ列インデックス
        private const int SYOHIN_GROUP_NAME_COLUMN = 3; // 商品グループ名列インデックス

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用商品マスタ
        /// </summary>
        private Syohin _syohin;

        /// <summary>
        /// 初期状態用商品マスタ
        /// </summary>
        private Syohin _origin;

        /// <summary>
        /// （今日時点の）消費税
        /// </summary>
        private Syohizei _syohizei;

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
        /// <param name="shain"></param>
        public Form702D(Syohin shain)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _syohin = new Syohin(shain);

                // 部署グループ、弁当区分、副食CD、主食CD、食事区分（つまりコンボボックス）については、Null を -1（先頭の選択肢） にしておく
                _syohin.部署グループ = _syohin.部署グループ ?? -1;
                _syohin.弁当区分 = _syohin.弁当区分 ?? -1;
                _syohin.副食ＣＤ = _syohin.副食ＣＤ ?? -1;
                _syohin.主食ＣＤ = _syohin.主食ＣＤ ?? -1;
                _syohin.食事区分 = _syohin.食事区分 ?? -1;

                _IsNewRecord = false;
                if (_syohin.商品ＣＤ == null)
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
        private void Form702D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnDelete.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyoGroup(cboBusyoGroup);
                ComboUtil.SetBentoKubun(cboBentoKbn);
                ComboUtil.SetSubDishCD(cboSubDishCD);
                ComboUtil.SetMainDishCD(cboMainDishCD);
                ComboUtil.SetSyokujiKubun(cboSyokujiKbn);

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
                    // 既存の場合は商品CD変更不可
                    txtSyohinCD.Enabled = false;
                }

                // 初期状態として保持
                _origin = new Syohin(_syohin);

                // 消費税を取得
                _syohizei = Syohizei.GetSyohizei(DateTime.Today);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                syohinBindingSource.DataSource = _syohin;

                // この商品が属するグループを表示
                GetGroupList(_syohin.商品ＣＤ);
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
        private void Form702D_KeyDown(object sender, KeyEventArgs e)
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
        private void Form702D_FormClosing(object sender, FormClosingEventArgs e)
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

                if (_syohin.Equals(_origin) == false)
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
        /// 商品単価検証後イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTankaKomi_Validated(object sender, EventArgs e)
        {
            try
            {
                CalcTaxExcludedPrice();     // 税抜き単価を算出
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
        /// 軽減税率チェックボックスクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkKeigen_Click(object sender, EventArgs e)
        {
            try
            {
                CalcTaxExcludedPrice();     // 税抜き単価を算出
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
                // 削除可能かチェック
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
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
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
        /// 税抜き単価を算出
        /// </summary>
        private void CalcTaxExcludedPrice()
        {
            try
            {
                if (_syohin.商品単価税込 == null)
                {
                    return;
                }

                // 税抜き単価を算出
                bool isKeigen = false;
                if (chkKeigen.Checked)
                {
                    isKeigen = true;
                }

                decimal taxRate = _syohizei.GetTaxRate(isKeigen);
                decimal price = (decimal)_syohin.商品単価税込;
                price = price / (1m + taxRate);
                price = Rounding.Round(price, Rounding.HasuKubun.Ceiling);  // 消費税額を切り捨てるのがデフォルトなので、単価は切り上げて算出
                _syohin.商品単価税抜 = price;
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
        /// この商品が属するグループを主要商品リストマスタと商品グループマスタから取得
        /// </summary>
        /// <param name="syohinCD"></param>
        private void GetGroupList(int? syohinCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     L.部署ＣＤ," +
                                      "\r\n     B.部署名," +
                                      "\r\n     G.商品グループＣＤ," +
                                      "\r\n     G.商品グループ名" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS L" +
                                      "\r\n     LEFT JOIN 部署マスタ         AS B ON B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND L.部署ＣＤ = B.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 商品グループマスタ AS G ON G.企業ＣＤ = @企業ＣＤ AND ISNULL(G.無効フラグ, 0) = 0 AND L.商品グループＣＤ = G.商品グループＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     L.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND ISNULL(L.無効フラグ, 0) = 0" +
                                      "\r\n     AND L.商品ＣＤ = @商品ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     L.部署ＣＤ," +
                                      "\r\n     G.商品グループＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syohinCD ?? -1;      // null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("商品グループ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "商品グループ";

                    log.Info("商品グループを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(BUSYO_CD_COLUMN, "部署ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN, "部署名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_GROUP_CD_COLUMN, "商品グループＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_GROUP_NAME_COLUMN, "商品グループ名");
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

                if (_syohin.HasErrors)
                {
                    MessageBox.Show(this, _syohin.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    ret = _syohin.Insert();
                }
                else
                {
                    ret = _syohin.Update();
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _syohin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(this.Text, "商品マスタ", _syohin.Code, historyProcess);

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
                _syohin = Syohin.GetSyohinReload((int)_syohin.商品ＣＤ);

                // 部署グループ、弁当区分、副食CD、主食CD、食事区分（つまりコンボボックス）については、Null を -1（先頭の選択肢） にしておく
                _syohin.部署グループ = _syohin.部署グループ ?? -1;
                _syohin.弁当区分 = _syohin.弁当区分 ?? -1;
                _syohin.副食ＣＤ = _syohin.副食ＣＤ ?? -1;
                _syohin.主食ＣＤ = _syohin.主食ＣＤ ?? -1;
                _syohin.食事区分 = _syohin.食事区分 ?? -1;

                _origin = new Syohin(_syohin);

                _IsNewRecord = false;

                // 副食CDと主食CDは商品マスタに依存しているので毎回作り直す
                ComboUtil.SetSubDishCD(cboSubDishCD);
                ComboUtil.SetMainDishCD(cboMainDishCD);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                syohinBindingSource.DataSource = _syohin;

                lblMode.Text = MODE_CAPTION_EDIT;
                lblMode.BackColor = BACK_COLOR_EDIT;
                btnSave.Text = "更新(F9)";
                // 既存の場合は商品CD変更不可
                txtSyohinCD.Enabled = false;

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

                // 当該商品が主要商品リストマスタに存在している？
                bool isSyohinExist = IsExistSyohin("主要商品リストマスタ", (int)_syohin.商品ＣＤ);

                // 当該商品が主要部品リストマスタに存在している？
                bool isBuhinExist = IsExistSyohin("主要部品リストマスタ", (int)_syohin.商品ＣＤ);

                if (isSyohinExist || isBuhinExist)
                {
                    string msg = string.Format("この商品は{0}{1}{2}にも存在しています。同時に削除します。",
                        (isSyohinExist ? "主要商品リストマスタ" : ""),
                        (isSyohinExist && isBuhinExist ? "と" : ""),
                        (isBuhinExist ? "主要部品リストマスタ" : ""));

                    if (MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return false;
                    }
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
        /// 指定したテーブルに指定した商品が存在するかどうかを返す
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="syohinCD">商品CD</param>
        /// <returns>商品が存在する場合は true</returns>
        private bool IsExistSyohin(string tableName, int syohinCD)
        {
            bool ret = false;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    string sql = "\r\n SELECT COUNT(商品ＣＤ) AS CNT" +
                                 "\r\n FROM {0}" +
                                 "\r\n WHERE 商品ＣＤ = @商品ＣＤ";

                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(sql, tableName);
                    cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syohinCD;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if ((int)reader["CNT"] != 0)
                        {
                            ret = true;
                        }
                    }
                }

                return ret;
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
                    // 商品マスタ削除
                    if (DeleteSyohin(con, tran) == false)
                    {
                        return false;
                    }

                    // 主要商品リストマスタ削除
                    if (DeleteSyohin(con, tran, "主要商品リストマスタ") == false)
                    {
                        return false;
                    }

                    // 主要部品リストマスタ削除
                    if (DeleteSyohin(con, tran, "主要部品リストマスタ") == false)
                    {
                        return false;
                    }

                    // 得意先単価マスタ削除
                    if (DeleteSyohin(con, tran, "得意先単価マスタ") == false)
                    {
                        return false;
                    }

                    // 得意先単価適用マスタ削除
                    if (DeleteSyohin(con, tran, "得意先単価適用マスタ") == false)
                    {
                        return false;
                    }

                    tran.Commit();
                    log.InfoFormat("商品マスタと連動するテーブルを{0}しました。", historyProcess.DisplayName());
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
        }

        /// <summary>
        /// 商品マスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteSyohin(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                if (_syohin.Delete() == false)
                {
                    MessageBox.Show(this, _syohin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                History.Save(con, tran, this.Text, "商品マスタ", _syohin.Code, History.ProcessType.Delete);

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
        /// 指定したテーブルから商品を削除します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteSyohin(SqlConnection con, SqlTransaction tran, string tableName)
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                string sql = "\r\n DELETE {0}" +
                             "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                             "\r\n   AND 商品ＣＤ = @商品ＣＤ";
                cmd.CommandText = string.Format(sql, tableName);

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = (int)_syohin.商品ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                log.InfoFormat("{0}を{1}しました。", tableName, historyProcess.DisplayName());
                History.Save(con, tran, this.Text, tableName, _syohin.Code, History.ProcessType.Delete);
                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show(string.Format("{0}が重複しています。", tableName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("{0}の{1}に失敗しました。\n{2}", tableName, historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("{0}の{1}に失敗しました。\n{2}", tableName, historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
    }
}
