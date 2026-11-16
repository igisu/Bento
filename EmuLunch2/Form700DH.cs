using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 得意先履歴一覧
    /// </summary>
    public partial class Form700DH : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int JYOTAI_COLUMN      = 0;    // 状態列インデックス
        private const int SYONINBI_COLUMN    = 1;    // 承認日列インデックス
        private const int SYONIN_NAME_COLUMN = 2;    // 承認者列インデックス
        private const int JOTAI_RIYU_COLUMN  = 3;    // 状態理由列インデックス
        private const int SIKKYAKUBI_COLUMN  = 4;    // 失客日列インデックス
        private const int EIGYO_NAME_COLUMN  = 5;    // 営業担当者列インデックス
        private const int SYORIBI_COLUMN     = 6;    // 処理日列インデックス
        private const int TOROKU_NAME_COLUMN = 7;    // 登録担当者列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 得意先ＣＤ
        /// </summary>
        private int _tokuisakiCD;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokuisakiCD">得意先CD</param>
        public Form700DH(int tokuisakiCD)
        {
            InitializeComponent();

            _tokuisakiCD = tokuisakiCD;
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
        private void Form700DH_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                // 得意先履歴テーブルを取得
                GetTokuisakiRireki();
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
        /// 得意先履歴テーブル取得
        /// </summary>
        private void GetTokuisakiRireki()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.状態区分" +
                                      "\r\n     ,VJ.状態区分名" +
                                      "\r\n     ,A.失客理由" +
                                      "\r\n     ,VR.状態理由区分名" +
                                      "\r\n     ,A.失客日" +
                                      "\r\n     ,A.承認日" +
                                      "\r\n     ,A.承認者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 承認者" +
                                      "\r\n     ,A.登録日" +
                                      "\r\n     ,A.営業担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 営業担当者" +
                                      "\r\n     ,A.変更者ＣＤ" +
                                      "\r\n     ,A3.担当者名 AS 変更者" +
                                      "\r\n FROM" +
                                      "\r\n     得意先履歴テーブル AS A" +
                                      "\r\n     LEFT JOIN V_状態区分     AS VJ ON A.状態区分 = VJ.状態区分CD" +
                                      "\r\n     LEFT JOIN V_状態理由区分 AS VR ON A.失客理由 = VR.状態理由区分CD" +
                                      "\r\n     LEFT JOIN 担当者マスタ   AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.承認者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ   AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.営業担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ   AS A3 ON A3.企業ＣＤ = @企業ＣＤ AND A.変更者ＣＤ = A3.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.得意先別履歴番号";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tokuisakiCD;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("得意先履歴テーブル");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "得意先履歴テーブル";

                    log.Info("得意先履歴テーブルを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(JYOTAI_COLUMN,      "状態区分名");
                fpSpread1_Sheet1.BindDataColumn(SYONINBI_COLUMN,    "承認日");
                fpSpread1_Sheet1.BindDataColumn(SYONIN_NAME_COLUMN, "承認者");
                fpSpread1_Sheet1.BindDataColumn(JOTAI_RIYU_COLUMN,  "状態理由区分名");
                fpSpread1_Sheet1.BindDataColumn(SIKKYAKUBI_COLUMN,  "失客日");
                fpSpread1_Sheet1.BindDataColumn(EIGYO_NAME_COLUMN,  "営業担当者");
                fpSpread1_Sheet1.BindDataColumn(SYORIBI_COLUMN,     "登録日");
                fpSpread1_Sheet1.BindDataColumn(TOROKU_NAME_COLUMN, "変更者");
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
