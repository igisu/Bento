using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// TP決済詳細
    /// </summary>
    public partial class Form512D : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// TP決済情報
        /// </summary>
        private TPSettlementInfo _tpSettlementInfo;

        /// <summary>
        /// TP売上履歴
        /// </summary>
        private TPSaleHistory _tpSaleHistory;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ

        public Form512D(TPSettlementInfo tpSettlementInfo)
        {
            try
            {
                InitializeComponent();

                _tpSettlementInfo = tpSettlementInfo;

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
        private void Form512D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                // パラメータ取得
                string tpNo = _tpSettlementInfo.TP決済番号;
                DateTime kessaibi = (DateTime)_tpSettlementInfo.決済日;

                // TP決済情報をバインド
                _tpSettlementInfo = TPSettlementInfo.GetTPSettlementInfo(tpNo, kessaibi);
                TPSettlementInfoBindingSource.DataSource = _tpSettlementInfo;

                // TP売上履歴をバインド
                _tpSaleHistory = TPSaleHistory.GetTPSaleHistory(tpNo, kessaibi);
                TPSaleHistoryBindingSource.DataSource = _tpSaleHistory;

                if (_tpSettlementInfo.売上明細番号 > 0)
                {
                    // 売上明細をバインド
                    uriageBindingSource.DataSource = GetUriage();
                }
                else
                {
                    // 入金明細をバインド
                    uriageBindingSource.DataSource = GetNyukin();
                }                

                // ラベルカラーを設定
                bool isCredit = (_tpSettlementInfo.電子マネーブランドID == null || _tpSettlementInfo.電子マネーブランドID < 1);
                toggleLabelColor(lblCredit, CREDIT_COLOR, isCredit);
                toggleLabelColor(lblEMoney, EMONEY_COLOR, !isCredit);
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
        private void Form512D_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
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
        /// 売上情報(売上ヘッダと売上明細)を取得する
        /// </summary>
        /// <returns></returns>
        private TPUriageInfo GetUriage()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.明細番号" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,H.得意先名" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,A.売上日 AS 決済日" +
                                      "\r\n     ,A.商品ＣＤ" +
                                      "\r\n     ,S.商品名" +
                                      "\r\n     ,ST.支店ＣＤ" +
                                      "\r\n     ,ST.支店名" +
                                      "\r\n     ,H.コースＣＤ" +
                                      "\r\n     ,C.コース名" +
                                      "\r\n     ,H.配送担当者ＣＤ" +
                                      "\r\n     ,T.担当者名 AS 配送担当者名" +
                                      "\r\n     ,'売上' AS 決済対象" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS A" +
                                      "\r\n     LEFT JOIN 売上ヘッダ   AS H  ON H.企業ＣＤ = A.企業ＣＤ AND H.売上日 = A.売上日 AND H.得意先ＣＤ = A.得意先ＣＤ AND H.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 部署マスタ   AS B  ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 支店マスタ   AS ST ON ST.企業ＣＤ = A.企業ＣＤ AND ST.支店ＣＤ = B.支店ＣＤ" +
                                      "\r\n     LEFT JOIN 商品マスタ   AS S  ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                                      "\r\n     LEFT JOIN コースマスタ AS C  ON C.企業ＣＤ = A.企業ＣＤ AND C.コースＣＤ = H.コースＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS T  ON T.企業ＣＤ = A.企業ＣＤ AND T.担当者ＣＤ = H.配送担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.売上日 = @売上日" +
                                      "\r\n     AND A.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND A.明細番号 = @明細番号";
                    
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _tpSettlementInfo.決済日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tpSettlementInfo.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tpSettlementInfo.部署ＣＤ;
                    cmd.Parameters.Add("@明細番号", SqlDbType.Int).Value = _tpSettlementInfo.売上明細番号;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("売上明細");
                    adp.Fill(dt);

                    TPUriageInfo tpUriageInfo = new TPUriageInfo();

                    if (dt.Rows.Count > 0)
                    {
                        tpUriageInfo = new TPUriageInfo(dt.Rows[0]);
                    }

                    log.Info("売上明細を検索しました。");

                    return tpUriageInfo;
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

        private TPUriageInfo GetNyukin()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.入金番号" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.得意先名" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,A.入金日 AS 決済日" +
                                      "\r\n     ,ST.支店ＣＤ" +
                                      "\r\n     ,ST.支店名" +
                                      "\r\n     ,'入金' AS 決済対象" +
                                      "\r\n FROM" +
                                      "\r\n     入金明細 AS A" +
                                      "\r\n     LEFT JOIN 部署マスタ   AS B  ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 支店マスタ   AS ST ON ST.企業ＣＤ = A.企業ＣＤ AND ST.支店ＣＤ = B.支店ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.入金番号 = @入金番号";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@入金番号", SqlDbType.Decimal).Value = _tpSettlementInfo.入金番号;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("入金明細");
                    adp.Fill(dt);

                    TPUriageInfo tpUriageInfo = new TPUriageInfo();

                    if (dt.Rows.Count > 0)
                    {
                        tpUriageInfo = new TPUriageInfo(dt.Rows[0]);
                    }

                    log.Info("入金明細を検索しました。");

                    return tpUriageInfo;
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
        /// クレジット・電子マネーのラベルの前景色・背景色を切り替える
        /// </summary>
        /// <param name="lbl">対象のラベルコントロール</param>
        /// <param name="backColor">ラベル固有の背景色</param>
        /// <param name="enabled">見せる場合はtrue</param>
        private void toggleLabelColor(Label lbl, Color backColor, bool enabled)
        {
            lbl.ForeColor = GetAlphaBlendColor(this.BackColor, Color.Black, enabled ? ENABLE_ALPHA : DISABLE_ALPHA);
            lbl.BackColor = GetAlphaBlendColor(this.BackColor, backColor, enabled ? ENABLE_ALPHA : DISABLE_ALPHA);
        }

        #endregion
    }
}
