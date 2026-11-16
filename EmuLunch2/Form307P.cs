using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 大袋ラベル印刷確認ダイアログ
    /// </summary>
    public partial class Form307P : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 大袋ラベル印刷条件
        /// </summary>
        private BigBagLabelPrintCriteria _bigBagLabelPrintCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bigBagLabelPrintCriteria">大袋ラベル印刷条件</param>
        public Form307P(BigBagLabelPrintCriteria bigBagLabelPrintCriteria)
        {
            InitializeComponent();

            _bigBagLabelPrintCriteria = bigBagLabelPrintCriteria;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 大袋ラベル印刷種別
        /// </summary>
        public Consts.BigBagPrintKind BigBagPrintKind
        {
            get
            {
                return _bigBagLabelPrintCriteria.印刷種別ＣＤ;
            }
        }

        /// <summary>
        /// 印刷したかどうか
        /// </summary>
        public bool IsPrinted { get; set; }
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
        private void Form307P_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                IsPrinted = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                bigBagLabelPrintCriteriaBindingSource.DataSource = _bigBagLabelPrintCriteria;

                // 大袋ラベル印刷履歴があれば「印刷済です」を表示
                ShowPrinted();
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
        private void Form307P_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:                      // 処理開始ボタン押下
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                    btnPrint.PerformClick();
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

        /// <summary>
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintBigBag();      // 大袋ラベル印刷
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
        /// 終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuit_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
        /// 印刷済みかどうかを表示
        /// </summary>
        private void ShowPrinted()
        {
            try
            {
                int printCnt = 0;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     COUNT(印刷実行番号) AS CNT" +
                                      "\r\n FROM" +
                                      "\r\n     大袋ラベル印刷履歴" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     AND 大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤ" +
                                      "\r\n     AND 喫食日 = @注文日";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _bigBagLabelPrintCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@大袋ラベル印刷種別ＣＤ", SqlDbType.Int).Value = (int)_bigBagLabelPrintCriteria.印刷種別ＣＤ;
                    cmd.Parameters.Add("@大袋ラベル食事区分ＣＤ", SqlDbType.Int).Value = _bigBagLabelPrintCriteria.食事区分ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _bigBagLabelPrintCriteria.注文日;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        printCnt = (int)reader["CNT"];
                    }
                }

                lblPrinted.Visible = (printCnt != 0);       // レコード数が0じゃなければ表示
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
        /// 大袋ラベル印刷処理
        /// </summary>
        private void PrintBigBag()
        {
            try
            {
                Report307A report = new Report307A();       // 大袋ラベル
                PrintSettings printSettings = this.GetPrintSettings(report);
                if (printSettings == null)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                //******************************************************************************************
                // セットと単品とで、食数を取得するSQLが異なる。
                // 単品の場合は食数をそのままラベルに出力すればいいが、
                // セットの場合は得意先ごとに10食ごととその余りの1枚のラベルを出力する必要がある。
                // （例えば24食だと10食が2枚と4食が1枚）
                // 画面にて指定された印刷順（ラベルに記載された食数の昇順/降順）にソートする必要がある。
                // 単品はSQLの時点でソートすればいいが、セットはそうはいかない。
                // なのでAcrtiveReportに渡す直前にソートする必要があり、ソースしやすい形式である必要がある。
                //******************************************************************************************

                List<BigBagLabel> bigBagLabelList = null;
                if (_bigBagLabelPrintCriteria.印刷種別ＣＤ == Consts.BigBagPrintKind.Set)
                {
                    bigBagLabelList = GetBigBagLabelSet();      // 大袋ラベル印刷データ(セット)を取得
                }
                else
                {
                    bigBagLabelList = GetBigBagLabelUnit();     // 大袋ラベル印刷データ(単品)を取得
                }

                if (bigBagLabelList.Count == 0)
                {
                    MessageBox.Show(this, "印刷するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 印刷順でソート
                if (rdoAsc.Checked)
                {
                    bigBagLabelList.Sort(BigBagLabel.CompareAscending);
                }
                else
                {
                    bigBagLabelList.Sort(BigBagLabel.CompareDescending);
                }

                // 大袋ラベル印刷履歴を出力
                if(SaveBigBagLabelHistory(bigBagLabelList.Count) == false)
                {
                    return;
                }

                History.Save(this.Text, "大袋ラベル印刷", string.Format("{0:yyyy/MM/dd} {1} {2} {3}",
                            _bigBagLabelPrintCriteria.注文日, _bigBagLabelPrintCriteria.部署名, _bigBagLabelPrintCriteria.食事区分名, _bigBagLabelPrintCriteria.印刷種別名), History.ProcessType.Output);

                report.DataSource = bigBagLabelList;

                // 帳票／プリンタ設定マスタにしたがって印刷またはプレビュー
                PrintReport(report, printSettings);

                log.InfoFormat("大袋ラベル({0})を印刷しました。件数={1}", _bigBagLabelPrintCriteria.印刷種別名, bigBagLabelList.Count);

                IsPrinted = true;

                // 「印刷済です」を表示
                ShowPrinted();
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
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 大袋ラベル印刷データ(セット)を取得
        /// </summary>
        /// <returns>大袋ラベル出力用データのリスト</returns>
        private List<BigBagLabel> GetBigBagLabelSet()
        {
            try
            {
                DataTable dt = null;
                List<BigBagLabel> bigBagLabelList = new List<BigBagLabel>();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     VS.大袋ラベル食事区分名" +
                                      "\r\n     ,H.喫食日" +
                                      "\r\n     ,H.得意先ＣＤ" +
                                      "\r\n     ,SUM(M.商品数量) AS 商品数量" +
                                      "\r\n FROM" +
                                      "\r\n     注文ヘッダ AS H" +
                                      "\r\n     INNER JOIN 大袋ラベル印刷設定 AS OS" +
                                      "\r\n         ON OS.企業ＣＤ = H.企業ＣＤ" +
                                      "\r\n         AND OS.部署ＣＤ = H.部署ＣＤ" +
                                      "\r\n     INNER JOIN 注文明細 AS M" +
                                      "\r\n         ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                      "\r\n         AND H.注文区分   = M.注文区分" +
                                      "\r\n         AND H.喫食日     = M.喫食日" +
                                      "\r\n         AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n         AND H.部署ＣＤ   = M.部署ＣＤ" +
                                      "\r\n         AND M.商品ＣＤ   = OS.商品ＣＤ" +
                                      "\r\n     LEFT JOIN V_大袋ラベル食事区分 AS VS ON OS.大袋ラベル食事区分ＣＤ = VS.大袋ラベル食事区分ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(M.商品数量, 0) <> 0" +       // -- 0注文はシールを印刷しない
                                      "\r\n     AND H.喫食日 = @注文日" +
                                      "\r\n     AND H.注文区分 = @注文区分注文" +
                                      "\r\n     AND OS.大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     AND OS.大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤ" +
                                      "\r\n GROUP BY" +
                                      "\r\n     VS.大袋ラベル食事区分名" +
                                      "\r\n     ,H.喫食日" +
                                      "\r\n     ,H.得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _bigBagLabelPrintCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _bigBagLabelPrintCriteria.注文日;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@大袋ラベル印刷種別ＣＤ", SqlDbType.Int).Value = (int)_bigBagLabelPrintCriteria.印刷種別ＣＤ;
                    cmd.Parameters.Add("@大袋ラベル食事区分ＣＤ", SqlDbType.Int).Value = (int)_bigBagLabelPrintCriteria.食事区分ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("大袋ラベル印刷(セット)");
                    adp.Fill(dt);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    BigBagLabel bigBagLabel = new BigBagLabel(dr);

                    // セットのラベルデータを生成
                    bigBagLabelList.AddRange(BigBagLabel.GetSetLabels(bigBagLabel));
                }

                return bigBagLabelList;
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
        /// 大袋ラベル印刷データ(単品)を取得
        /// </summary>
        /// <returns>大袋ラベル出力用データのリスト</returns>
        private List<BigBagLabel> GetBigBagLabelUnit()
        {
            try
            {
                DataTable dt = null;
                List<BigBagLabel> bigBagLabelList = new List<BigBagLabel>();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     VS.大袋ラベル食事区分名" +
                                      "\r\n     ,H.喫食日" +
                                      "\r\n     ,M.商品数量" +
                                      "\r\n FROM" +
                                      "\r\n     注文ヘッダ AS H" +
                                      "\r\n     INNER JOIN 大袋ラベル印刷設定 AS OS" +
                                      "\r\n         ON OS.企業ＣＤ = H.企業ＣＤ" +
                                      "\r\n         AND OS.部署ＣＤ = H.部署ＣＤ" +
                                      "\r\n     INNER JOIN 注文明細 AS M" +
                                      "\r\n         ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                      "\r\n         AND H.注文区分   = M.注文区分" +
                                      "\r\n         AND H.喫食日     = M.喫食日" +
                                      "\r\n         AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n         AND H.部署ＣＤ   = M.部署ＣＤ" +
                                      "\r\n         AND M.商品ＣＤ   = OS.商品ＣＤ" +
                                      "\r\n     LEFT JOIN V_大袋ラベル食事区分 AS VS ON OS.大袋ラベル食事区分ＣＤ = VS.大袋ラベル食事区分ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(M.商品数量, 0) <> 0" +       // -- 0注文はシールを印刷しない
                                      "\r\n     AND H.喫食日 = @注文日" +
                                      "\r\n     AND H.注文区分 = @注文区分注文" +
                                      "\r\n     AND OS.大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     AND OS.大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _bigBagLabelPrintCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _bigBagLabelPrintCriteria.注文日;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@大袋ラベル印刷種別ＣＤ", SqlDbType.Int).Value = (int)_bigBagLabelPrintCriteria.印刷種別ＣＤ;
                    cmd.Parameters.Add("@大袋ラベル食事区分ＣＤ", SqlDbType.Int).Value = (int)_bigBagLabelPrintCriteria.食事区分ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("大袋ラベル印刷(単品)");
                    adp.Fill(dt);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    BigBagLabel bigBagLabel = new BigBagLabel(dr);

                    // 単品のラベルデータを生成（そのまま追加する）
                    bigBagLabelList.Add(bigBagLabel);
                }

                return bigBagLabelList;
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
        /// 大袋ラベル印刷履歴を出力
        /// </summary>
        /// <param name="count">印刷枚数</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBigBagLabelHistory(int count)
        {
            try
            {
                BigBagLabelHistory bigBagLabelHistory = new BigBagLabelHistory();
                bigBagLabelHistory.部署ＣＤ = _bigBagLabelPrintCriteria.部署ＣＤ;
                bigBagLabelHistory.大袋ラベル印刷種別ＣＤ = (int)_bigBagLabelPrintCriteria.印刷種別ＣＤ;
                bigBagLabelHistory.大袋ラベル食事区分ＣＤ = _bigBagLabelPrintCriteria.食事区分ＣＤ;
                bigBagLabelHistory.喫食日 = _bigBagLabelPrintCriteria.注文日;
                // 印刷実行番号はInsert実行時に取得している
                bigBagLabelHistory.印刷枚数 = count;

                bool ret = bigBagLabelHistory.Insert();

                if (ret == false)
                {
                    MessageBox.Show(this, bigBagLabelHistory.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 操作履歴に保存
                History.Save(this.Text, "大袋ラベル印刷履歴", bigBagLabelHistory.Code, History.ProcessType.Create);

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
