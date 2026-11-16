using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 出荷一覧
    /// </summary>
    public partial class Form306 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_CD_COLUMN       = 0;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN     = 1;    // 得意先名列インデックス
        private const int SYUKEI_SYOKUJI_KBN_COLUMN = 2;    // 食事区分列インデックス
        private const int SYUKKA_TIME_COLUMN        = 3;    // 積載時刻列インデックス
        private const int SEAL_SYOKUSU_COLUMN       = 4;    // シール食数列インデックス
        private const int CURRENT_SYOKUSU_COLUMN    = 5;    // 現在数量列インデックス
        private const int BAG_COLUMN                = 6;    // 袋列インデックス

        /// <summary>
        /// シール食数と現在数量が異なる場合の背景色
        /// </summary>
        private readonly Color SEAL_NOT_EQUAL_CURRENT_BACK_COLOR = Color.PapayaWhip;

        /// <summary>
        /// 出荷履歴が無い場合の背景色
        /// </summary>
        private readonly Color NO_HISTORY_BACK_COLOR = Color.LightPink;

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 出荷一覧検索条件
        /// </summary>
        private ShippingHistoryCriteria _shippingHistoryCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="shippingHistoryCriteria">出荷一覧検索条件</param>
        /// <returns></returns>
        public Form306(ShippingHistoryCriteria shippingHistoryCriteria)
        {
            try
            {
                InitializeComponent();

                _shippingHistoryCriteria = new ShippingHistoryCriteria(shippingHistoryCriteria);
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
        private void Form306_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                //*****************
                // スプレッドの設定
                //*****************
                // 得意先名列について、同一内容を持つ隣接セルを一つのセルとしてグループ表示する
                fpSpread1_Sheet1.SetColumnMerge(TOKUISAKI_CD_COLUMN, FarPoint.Win.Spread.Model.MergePolicy.Always);
                fpSpread1_Sheet1.SetColumnMerge(TOKUISAKI_NAME_COLUMN, FarPoint.Win.Spread.Model.MergePolicy.Always);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                shippingHistoryCriteriaBindingSource.DataSource = _shippingHistoryCriteria;

                // データ取得
                GetData();
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
        private void Form306_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// Excel出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                saveFileDialog1.FileName = this.Text;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = saveFileDialog1.FileName;
                    FarPoint.Excel.ExcelSaveFlags excelSaveFlags = FarPoint.Excel.ExcelSaveFlags.SaveCustomColumnHeaders;
                    if (fileName.ToLower().EndsWith("xlsx"))
                    {
                        excelSaveFlags |= FarPoint.Excel.ExcelSaveFlags.UseOOXMLFormat;
                    }
                    fpSpread1.SaveExcel(fileName, excelSaveFlags);
                    if (MessageBox.Show(this, "Excel出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException)
            {
                MessageBox.Show(this, fileName + "\nファイルが開かれています。保存できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// データ取得
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
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.出荷番号" +
                                      "\r\n     ,A.配送日" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.得意先名" +
                                      "\r\n     ,A.食事区分" +
                                      "\r\n     ,A.集計印刷名" +
                                      "\r\n     ,B.積載時刻" +
                                      "\r\n     ,A.シール食数" +
                                      "\r\n     ,C.現在数量" +
                                      "\r\n     ,A.袋数表記" +
                                      "\r\n FROM" +
                                      // -- 駆動表兼出荷明細
                                      "\r\n     (SELECT" +
                                      "\r\n         SM.企業ＣＤ" +
                                      "\r\n         ,SM.出荷番号" +
                                      "\r\n         ,SM.配送日" +
                                      "\r\n         ,SM.部署ＣＤ" +
                                      "\r\n         ,SM.得意先ＣＤ" +
                                      "\r\n         ,SM.得意先名" +
                                      "\r\n         ,SM.食事区分" +
                                      "\r\n         ,TSM.集計印刷名" +
                                      "\r\n         ,TSM.印刷順" +
                                      "\r\n         ,SM.集計注文数 AS シール食数" +
                                      "\r\n         ,SM.袋数表記" +
                                      "\r\n     FROM" +
                                      // -- 指定した配送日・コースで、同じ得意先・食事区分の最新のレコードを抽出する
                                      // -- ※短冊を複数回出力することがあるので、最新のレコードを優先する必要がある
                                      "\r\n         (SELECT" +
                                      "\r\n             MAX(出荷番号) AS 出荷番号" +
                                      "\r\n             ,企業ＣＤ" +
                                      "\r\n             ,部署ＣＤ" +
                                      "\r\n             ,得意先ＣＤ" +
                                      "\r\n             ,食事区分" +
                                      "\r\n         FROM" +
                                      "\r\n             SP_T_出荷明細" +
                                      "\r\n         WHERE" +
                                      "\r\n             企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND 配送日 = @配送日" +
                                      "\r\n             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND コースＣＤ = @コースＣＤ" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             企業ＣＤ" +
                                      "\r\n             ,部署ＣＤ" +
                                      "\r\n             ,得意先ＣＤ" +
                                      "\r\n             ,食事区分" +
                                      "\r\n         ) AS M1" +
                                      "\r\n         INNER JOIN SP_T_出荷明細 SM ON SM.企業ＣＤ = M1.企業ＣＤ AND SM.出荷番号 = M1.出荷番号" +
                                      "\r\n         LEFT JOIN 短冊集計設定マスタ AS TSM" +
                                      "\r\n             ON TSM.企業ＣＤ  = SM.企業ＣＤ" +
                                      "\r\n             AND TSM.部署ＣＤ = SM.部署ＣＤ" +
                                      "\r\n             AND TSM.食事区分ＣＤ = SM.食事区分" +
                                      "\r\n     ) AS A" +
                                      // -- 出荷履歴
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             SH.企業ＣＤ" +
                                      "\r\n             ,SH.出荷番号" +
                                      "\r\n             ,MAX(SH.出荷日時) AS 積載時刻" +
                                      "\r\n         FROM" +
                                      "\r\n             SP_T_出荷履歴 AS SH" +
                                      "\r\n         WHERE" +
                                      "\r\n             SH.担当者ＣＤ = @担当者ＣＤ" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             SH.企業ＣＤ" +
                                      "\r\n             ,SH.出荷番号" +
                                      "\r\n         ) AS B" +
                                      "\r\n             ON A.企業ＣＤ = B.企業ＣＤ" +
                                      "\r\n             AND A.出荷番号 = B.出荷番号" +
                                      // -- 注文数
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             H.企業ＣＤ" +
                                      "\r\n             ,H.部署ＣＤ" +
                                      "\r\n             ,H.配送日" +
                                      "\r\n             ,H.得意先ＣＤ" +
                                      "\r\n             ,SM.食事区分" +
                                      "\r\n             ,SUM(M.商品数量) AS 現在数量" +
                                      "\r\n         FROM" +
                                      "\r\n             注文ヘッダ AS H" +
                                      "\r\n             INNER JOIN 注文明細 AS M" +
                                      "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                      "\r\n                 AND H.注文区分   = M.注文区分" +
                                      "\r\n                 AND H.喫食日     = M.喫食日" +
                                      "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                                      "\r\n             INNER JOIN 商品マスタ AS SM" +
                                      "\r\n                 ON SM.企業ＣＤ  = M.企業ＣＤ" +
                                      "\r\n                 AND SM.商品ＣＤ = M.商品ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             H.注文区分 = @注文区分注文" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             H.企業ＣＤ" +
                                      "\r\n             ,H.部署ＣＤ" +
                                      "\r\n             ,H.配送日" +
                                      "\r\n             ,H.得意先ＣＤ" +
                                      "\r\n             ,SM.食事区分" +
                                      "\r\n         ) AS C" +
                                      "\r\n             ON A.企業ＣＤ = C.企業ＣＤ" +
                                      "\r\n             AND A.部署ＣＤ = C.部署ＣＤ" +
                                      "\r\n             AND A.配送日 = C.配送日" +
                                      "\r\n             AND A.得意先ＣＤ = C.得意先ＣＤ" +
                                      "\r\n             AND A.食事区分 = C.食事区分" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.得意先ＣＤ" +
                                      "\r\n     ,A.印刷順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _shippingHistoryCriteria.配送日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _shippingHistoryCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _shippingHistoryCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = _shippingHistoryCriteria.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("出荷一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // 数式を消すため、全行削除する
                    //fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "出荷一覧";

                    log.Info("出荷一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,       "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN,     "得意先名");
                fpSpread1_Sheet1.BindDataColumn(SYUKEI_SYOKUJI_KBN_COLUMN, "集計印刷名");
                fpSpread1_Sheet1.BindDataColumn(SYUKKA_TIME_COLUMN,        "積載時刻");
                fpSpread1_Sheet1.BindDataColumn(SEAL_SYOKUSU_COLUMN,       "シール食数");
                fpSpread1_Sheet1.BindDataColumn(CURRENT_SYOKUSU_COLUMN,    "現在数量");
                fpSpread1_Sheet1.BindDataColumn(BAG_COLUMN,                "袋数表記");

                if (fpSpread1_Sheet1.Rows.Count == 0)
                {
                    return;
                }

                // 罫線を重ねて描画する
                fpSpread1.BorderCollapse = FarPoint.Win.Spread.BorderCollapse.Collapse;

                // シール食数≠現在数量なら現在数量を着色
                // ついでに得意先を区切る罫線を描画
                int tokuisakiCD = (int)fpSpread1_Sheet1.Cells[0, TOKUISAKI_CD_COLUMN].Value;
                FarPoint.Win.LineBorder border = new FarPoint.Win.LineBorder(SystemColors.ControlDark, 2, false, true, false, false);

                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    decimal? seal = fpSpread1_Sheet1.Cells[rowIndex, SEAL_SYOKUSU_COLUMN].Value as decimal?;
                    decimal? current = fpSpread1_Sheet1.Cells[rowIndex, CURRENT_SYOKUSU_COLUMN].Value as decimal?;
                    if (current != null && seal != current)
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, CURRENT_SYOKUSU_COLUMN].ForeColor = Color.Red;
                        fpSpread1_Sheet1.Cells[rowIndex, SYUKEI_SYOKUJI_KBN_COLUMN].BackColor = SEAL_NOT_EQUAL_CURRENT_BACK_COLOR;
                        fpSpread1_Sheet1.Cells[rowIndex, SYUKKA_TIME_COLUMN].BackColor = SEAL_NOT_EQUAL_CURRENT_BACK_COLOR;
                        fpSpread1_Sheet1.Cells[rowIndex, SEAL_SYOKUSU_COLUMN].BackColor = SEAL_NOT_EQUAL_CURRENT_BACK_COLOR;
                        fpSpread1_Sheet1.Cells[rowIndex, CURRENT_SYOKUSU_COLUMN].BackColor = SEAL_NOT_EQUAL_CURRENT_BACK_COLOR;
                        fpSpread1_Sheet1.Cells[rowIndex, BAG_COLUMN].BackColor = SEAL_NOT_EQUAL_CURRENT_BACK_COLOR;
                    }

                    // 出荷履歴が無い場合は背景を着色
                    if (fpSpread1_Sheet1.Cells[rowIndex, SYUKKA_TIME_COLUMN].Value == null)
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, SYUKEI_SYOKUJI_KBN_COLUMN].BackColor = NO_HISTORY_BACK_COLOR;
                        fpSpread1_Sheet1.Cells[rowIndex, SYUKKA_TIME_COLUMN].BackColor = NO_HISTORY_BACK_COLOR;
                        fpSpread1_Sheet1.Cells[rowIndex, SEAL_SYOKUSU_COLUMN].BackColor = NO_HISTORY_BACK_COLOR;
                        fpSpread1_Sheet1.Cells[rowIndex, CURRENT_SYOKUSU_COLUMN].BackColor = NO_HISTORY_BACK_COLOR;
                        fpSpread1_Sheet1.Cells[rowIndex, BAG_COLUMN].BackColor = NO_HISTORY_BACK_COLOR;
                    }

                    // 得意先が変われば罫線を描画
                    int tokuisakiCD2 = (int)fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_CD_COLUMN].Value;
                    if (tokuisakiCD != tokuisakiCD2)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].Border = border;
                    }
                    tokuisakiCD = tokuisakiCD2;
                }
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
