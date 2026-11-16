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
    /// コース一覧
    /// </summary>
    public partial class Form700DC : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int COURSE_CD_COLUMN      = 0;    // コースＣＤ列インデックス
        private const int COURSE_KBN_COLUMN     = 1;    // コース区分列インデックス
        private const int COURSE_NAME_COLUMN    = 2;    // コース名列インデックス
        private const int MASTER_INVALID_COLUMN = 3;    // マスタ無効列インデックス
        private const int TABLE_INVALID_COLUMN  = 4;    // テーブル無効列インデックス

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
        public Form700DC(int tokuisakiCD)
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
        private void Form700DC_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                // コース一覧を取得
                GetCourseList();
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
        /// コース一覧取得
        /// </summary>
        private void GetCourseList()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     M.コースＣＤ" +
                                      "\r\n     ,M.コース区分" +
                                      "\r\n     ,VC.コース区分名" +
                                      "\r\n     ,M.コース名" +
                                      "\r\n     ,M.無効フラグ AS マスタ無効" +
                                      "\r\n     ,T.無効フラグ AS テーブル無効" +
                                      "\r\n FROM" +
                                      "\r\n     コーステーブル AS T " +
                                      "\r\n     LEFT JOIN コースマスタ AS M ON M.企業ＣＤ = @企業ＣＤ AND T.コースＣＤ = M.コースＣＤ" +
                                      "\r\n     LEFT JOIN V_コース区分 AS VC ON M.コース区分 = VC.コース区分CD" +
                                      "\r\n WHERE" +
                                      "\r\n     T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND T.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     M.コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tokuisakiCD;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("コーステーブル");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "コーステーブル";

                    log.Info("コーステーブルを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN, "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_KBN_COLUMN, "コース区分名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN, "コース名");
                fpSpread1_Sheet1.BindDataColumn(MASTER_INVALID_COLUMN, "マスタ無効");
                fpSpread1_Sheet1.BindDataColumn(TABLE_INVALID_COLUMN, "テーブル無効");

                // 無効フラグの値に応じて行の背景色を変える
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread1_Sheet1.Cells[rowIndex, MASTER_INVALID_COLUMN].Value as bool? == true ||
                        fpSpread1_Sheet1.Cells[rowIndex, TABLE_INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
                    }
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
