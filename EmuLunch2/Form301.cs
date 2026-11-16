using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 移動入力
    /// </summary>
    public partial class Form301 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int COURSE_NAME_COLUMN  = 0;      // コース名列インデックス
        private const int SYOHIN_NAME_COLUMN  = 1;      // 商品名列インデックス
        private const int MOCHIDASHI_COLUMN   = 2;      // 持ち出し数列インデックス
        private const int URIAGE_YOTEI_COLUMN = 3;      // 売上予定列インデックス
        private const int TAKE_KOJO_COLUMN    = 4;      // 工場追加列インデックス
        private const int TAKE_IPPAN_COLUMN   = 5;      // もらった列インデックス
        private const int TAKE_DETAIL_COLUMN  = 6;      // もらった詳細列インデックス
        private const int GIVE_COLUMN         = 7;      // やった列インデックス
        private const int GIVE_DETAIL_COLUMN  = 8;      // やった詳細列インデックス
        private const int ZANSU_COLUMN        = 9;      // 残数列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 移動入力検索条件
        /// </summary>
        private IdoNyuryokuCriteria _idoNyuryokuCriteria;

        /// <summary>
        /// 移動入力検索条件検索実行時スナップショット
        /// </summary>
        private IdoNyuryokuCriteria _idoNyuryokuCriteriaSnapShot;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form301()
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
        private void Form301_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnDetail.Enabled = this.HasAuthEdit;

                //*****************
                // スプレッドの設定
                //*****************
                // コース名列について、同一内容を持つ隣接セルを一つのセルとしてグループ表示する
                fpSpread1_Sheet1.SetColumnMerge(COURSE_NAME_COLUMN, FarPoint.Win.Spread.Model.MergePolicy.Always);
                // テキスト位置の自動調整
                fpSpread1.AllowCellContentFloat = true;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "コース", "グループ種別" },
                    new string[] { "合計行, コースＣＤ {0}, 商品ＣＤ", "合計行, グループＳＥＱ {0}, コースＣＤ {0}, 商品ＣＤ"});

                // コンボボックスの選択肢生成
                ComboUtil.SetCourseKubun(cboCourseKbn);
                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _idoNyuryokuCriteria = new IdoNyuryokuCriteria();

                idoNyuryokuBindingSource.DataSource = _idoNyuryokuCriteria;

                // 部署ＣＤの初期値を設定したので、コース別グループ種別コンボボックスの選択肢を設定する
                ComboUtil.SetCourseGroup(cboGroupSyubetsu, (int)cboBusyo.SelectedValue);
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
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
                cboBusyo.SelectedValueChanged +=  new EventHandler(cboBusyo_SelectedValueChanged);   // 部署ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form301_Shown(object sender, EventArgs e)
        {
            // コース区分にフォーカス
            cboCourseKbn.Focus();
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form301_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F6:                       // 一括移動ボタン押下
                    btnIkkatsu.PerformClick();
                    break;

                case Keys.F8:                       // 更新ボタン押下
                    btnDetail.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
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
                // グループ種別コンボの選択肢を、部署にぶら下がるものに再設定する
                if ((int)cboBusyo.SelectedValue < 0)
                {
                    cboGroupSyubetsu.DataSource = null;
                }
                else
                {
                    ComboUtil.SetCourseGroup(cboGroupSyubetsu, (int)cboBusyo.SelectedValue);
                }

                // グループ種別コンボを初期化
                _idoNyuryokuCriteria.グループ種別ＣＤ = -1;
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            int syohinCount = 0;            // 対象商品数
            DataTable dt;

            try
            {
                // 検索条件の入力にエラーがある？
                if (_idoNyuryokuCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_idoNyuryokuCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _idoNyuryokuCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 配送日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_idoNyuryokuCriteria.配送日 > warnDate)
                {
                    if (MessageBox.Show(this, "配送日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        _idoNyuryokuCriteriaSnapShot = null;
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    // 対象商品の数を取得
                    SqlCommand cmdCnt = con.CreateCommand();
                    cmdCnt.CommandText = "\r\n SELECT" +
                                         "\r\n     COUNT(A.商品ＣＤ) AS CNT" +
                                         "\r\n FROM" +
                                         "\r\n     (SELECT DISTINCT" +
                                         "\r\n         P.商品ＣＤ," +
                                         "\r\n         S.商品名略称" +
                                         "\r\n     FROM" +
                                         "\r\n         主要部品リストマスタ AS P" +
                                         "\r\n         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                         "\r\n     WHERE" +
                                         "\r\n         P.企業ＣＤ       = @企業ＣＤ" +
                                         "\r\n         AND P.部署ＣＤ   = @部署ＣＤ" +
                                         "\r\n         AND ISNULL(S.無効フラグ, 0) = 0" +
                                         "\r\n     ) AS A";


                    cmdCnt.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmdCnt.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _idoNyuryokuCriteria.部署ＣＤ;
                    log.Debug(cmdCnt.CommandTextWithParameters());
                    using (SqlDataReader reader = cmdCnt.ExecuteReader())
                    {
                        reader.Read();
                        syohinCount = (int)reader["CNT"];
                    }

                    // 商品が無ければ移動のしようもないので表示しない
                    if (syohinCount <= 0)
                    {
                        bindingSource1.DataSource = null;
                        MessageBox.Show(this, "移動できる商品が無いので表示できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    // ※移動が発生するのは日配の弁当。
                    //   日配の弁当は、コースと部署が関連している（ケータリングは関連しないことがある）。
                    //   したがって、注文の部署はコースの部署と一致するものを取得している（指定された部署を持つ注文ではない）
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     0 AS 合計行,");
                    sql.AppendLine("     MAIN.工場フラグ,");
                    sql.AppendLine("     MAIN.コースＣＤ,");
                    sql.AppendLine("     MAIN.コース名,");
                    sql.AppendLine("     cast(MAIN.コースＣＤ as varchar) + CHAR(10) + MAIN.コース名 AS 表示用コース名,");
                    sql.AppendLine("     MAIN.商品ＣＤ,");
                    sql.AppendLine("     MAIN.商品名略称,");
                    sql.AppendLine("     MCDS.個数 AS 持ち出し数,");
                    sql.AppendLine("     INIT.数量 AS 売上予定,");
                    sql.AppendLine("     TAKE_KOJO.個数 AS 受取数_工場,");
                    sql.AppendLine("     TAKE_IPPAN.個数 AS 受取数_一般,");
                    // -- 商品に対する受け取り元のコースとその個数をカンマ区切りで取得
                    sql.AppendLine("     REPLACE('START'+ISNULL(");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             ',' + CM.コース名 + ' ' + CONVERT(varchar, SUM(MI.個数))");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_移動入力 MI");
                    sql.AppendLine("              LEFT JOIN コースマスタ AS CM");
                    sql.AppendLine("              ON MI.企業ＣＤ = CM.企業ＣＤ");
                    sql.AppendLine("              AND MI.コースＣＤ = CM.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             MI.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND MI.移動日 = @配送日");
                    sql.AppendLine("             AND MI.相手コースＣＤ = MAIN.コースＣＤ");
                    sql.AppendLine("             AND MI.商品ＣＤ = MAIN.商品ＣＤ");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             CM.コース名");
                    sql.AppendLine("         ORDER BY");
                    sql.AppendLine("             CM.コース名");
                    sql.AppendLine("         FOR XML PATH ('')");
                    sql.AppendLine("         ),','),'START,','') AS 受取詳細,");
                    sql.AppendLine("     GIVE.個数 AS 引渡数,");
                    // -- 商品に対する渡し先のコースとその個数をカンマ区切りで取得
                    sql.AppendLine("     REPLACE('START'+ISNULL(");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             ',' + CM.コース名 + ' ' + CONVERT(varchar, SUM(MI.個数))");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_移動入力 MI");
                    sql.AppendLine("              LEFT JOIN コースマスタ AS CM");
                    sql.AppendLine("              ON MI.企業ＣＤ = CM.企業ＣＤ");
                    sql.AppendLine("              AND MI.相手コースＣＤ = CM.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             MI.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND MI.移動日 = @配送日");
                    sql.AppendLine("             AND MI.コースＣＤ = MAIN.コースＣＤ");
                    sql.AppendLine("             AND MI.商品ＣＤ = MAIN.商品ＣＤ");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             CM.コース名");
                    sql.AppendLine("         ORDER BY");
                    sql.AppendLine("             CM.コース名");
                    sql.AppendLine("         FOR XML PATH ('')");
                    sql.AppendLine("         ),','),'START,','') AS 引渡詳細,");
                    // -- 持出の個数＋工場から受け取った個数＋工場以外から受け取った個数－渡した個数－販売数 を残数とする
                    // -- すべてNULLなら残数もNULL
                    sql.AppendLine("     CASE WHEN");
                    sql.AppendLine("         MCDS.個数       IS NULL AND");
                    sql.AppendLine("         TAKE_KOJO.個数  IS NULL AND");
                    sql.AppendLine("         TAKE_IPPAN.個数 IS NULL AND");
                    sql.AppendLine("         GIVE.個数       IS NULL AND");
                    sql.AppendLine("         INIT.数量       IS NULL THEN NULL");
                    sql.AppendLine("     ELSE ISNULL(MCDS.個数, 0) +");
                    sql.AppendLine("          ISNULL(TAKE_KOJO.個数, 0) +");
                    sql.AppendLine("          ISNULL(TAKE_IPPAN.個数, 0) -");
                    sql.AppendLine("          ISNULL(GIVE.個数, 0) -");
                    sql.AppendLine("          ISNULL(INIT.数量, 0)");
                    sql.AppendLine("     END AS 残数,");
                    sql.AppendLine("     CG.グループ種別ＣＤ,");
                    sql.AppendLine("     CG.グループＳＥＱ");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表（コースと商品の全組み合わせ）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         C.工場フラグ,");
                    sql.AppendLine("         C.コースＣＤ,");
                    sql.AppendLine("         C.コース名,");
                    sql.AppendLine("         P.商品ＣＤ,");
                    sql.AppendLine("         S.商品名略称");
                    sql.AppendLine("     FROM");
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             CM.企業ＣＤ");
                    sql.AppendLine("             ,CM.部署ＣＤ");
                    sql.AppendLine("             ,CM.コースＣＤ");
                    sql.AppendLine("             ,CM.コース名");
                    sql.AppendLine("             ,CM.工場フラグ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ AS CM");
                    sql.AppendLine("             LEFT JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     臨時コースマスタ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND コース区分 = @コース区分");
                    sql.AppendLine("                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND 実施日 = @配送日");
                    sql.AppendLine("                 ) AS RM");
                    sql.AppendLine("                 ON RM.コースＣＤ = CM.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND CM.コース区分 = @コース区分");
                    sql.AppendLine("             AND CM.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             RM.企業ＣＤ");
                    sql.AppendLine("             ,RM.部署ＣＤ");
                    sql.AppendLine("             ,RM.コースＣＤ");
                    sql.AppendLine("             ,RM.コース名");
                    sql.AppendLine("             ,RM.工場フラグ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             臨時コーステーブル AS RT");
                    sql.AppendLine("             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             RM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND RM.コース区分 = @コース区分");
                    sql.AppendLine("             AND RM.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RM.実施日 = @配送日");
                    sql.AppendLine("         ) AS C");
                    sql.AppendLine("         INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = C.企業ＣＤ AND P.部署ＣＤ = C.部署ＣＤ");
                    sql.AppendLine("         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         ISNULL(S.無効フラグ, 0) = 0");
                    sql.AppendLine("     ) AS MAIN");
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         PARTSUM.コースＣＤ,");
                    sql.AppendLine("         PARTSUM.部品ＣＤ");
                    sql.AppendLine("         ,SUM(PARTSUM.数量) AS 数量");
                    sql.AppendLine("     FROM");
                    // -- ②予測と注文と売上の部品（各商品を分割した部品ごとに合計する）
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             PART.コースＣＤ,");
                    sql.AppendLine("             PART.得意先ＣＤ,");
                    sql.AppendLine("             PART.部品ＣＤ,");
                    sql.AppendLine("             CASE WHEN ISNULL(SUM(PART.商品売上数量), -1) >= 0 THEN SUM(PART.商品売上数量)");
                    sql.AppendLine("                  ELSE ISNULL(SUM(PART.商品注文予測数量), 0) END");
                    sql.AppendLine("             AS 数量");
                    sql.AppendLine("         FROM");
                    // -- ②－１ 予測と注文の弁当の主食
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 PART1.コースＣＤ,");
                    sql.AppendLine("                 PART1.得意先ＣＤ,");
                    sql.AppendLine("                 PART1.部品ＣＤ,");
                    sql.AppendLine("                 SUM(ISNULL(PART1.商品注文数量, PART1.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                 null AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                         AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                     LEFT JOIN");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                            H.企業ＣＤ,");
                    sql.AppendLine("                            H.売上日,");
                    sql.AppendLine("                            H.得意先ＣＤ,");
                    sql.AppendLine("                            H.部署ＣＤ,");
                    sql.AppendLine("                            H.コースＣＤ,");
                    sql.AppendLine("                            D.商品ＣＤ,");
                    sql.AppendLine("                            D.明細番号");
                    sql.AppendLine("                         FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");       // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                 INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                     T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                     AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                             UNION");
                    sql.AppendLine("                             SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 売上ヘッダ T1");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日   = @配送日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 AND NOT EXISTS (");
                    sql.AppendLine("                                     SELECT 'X'");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         (SELECT");
                    sql.AppendLine("                                             企業ＣＤ,");
                    sql.AppendLine("                                             部署ＣＤ,");
                    sql.AppendLine("                                             コースＣＤ,");
                    sql.AppendLine("                                             得意先ＣＤ,");
                    sql.AppendLine("                                             喫食日 AS 売上日");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             注文ヘッダ");
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                             AND 配送日   = @配送日");
                    sql.AppendLine("                                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                             ) AS H");
                    sql.AppendLine("                             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                         ) AS KHD");
                    sql.AppendLine("                         ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                         AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                         AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                         AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.配送日 = @配送日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                     AND ISNULL(I.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                     AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                     AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.主食ＣＤ");
                    sql.AppendLine("                 ) AS PART1");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART1.コースＣＤ");
                    sql.AppendLine("                 ,PART1.得意先ＣＤ");
                    sql.AppendLine("                 ,PART1.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");       // -- SUM()の対象にするので、同一コース、同一得意先、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                    // -- ②－２ 予測と注文の弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART2.コースＣＤ,");
                    sql.AppendLine("                 PART2.得意先ＣＤ,");
                    sql.AppendLine("                 PART2.部品ＣＤ,");
                    sql.AppendLine("                 SUM(ISNULL(PART2.商品注文数量, PART2.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                 null AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                         AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                     LEFT JOIN");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                            H.企業ＣＤ,");
                    sql.AppendLine("                            H.売上日,");
                    sql.AppendLine("                            H.得意先ＣＤ,");
                    sql.AppendLine("                            H.部署ＣＤ,");
                    sql.AppendLine("                            H.コースＣＤ,");
                    sql.AppendLine("                            D.商品ＣＤ,");
                    sql.AppendLine("                            D.明細番号");
                    sql.AppendLine("                         FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                 INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                     T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                     AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                             UNION");
                    sql.AppendLine("                             SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 売上ヘッダ T1");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日   = @配送日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 AND NOT EXISTS (");
                    sql.AppendLine("                                     SELECT 'X'");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         (SELECT");
                    sql.AppendLine("                                             企業ＣＤ,");
                    sql.AppendLine("                                             部署ＣＤ,");
                    sql.AppendLine("                                             コースＣＤ,");
                    sql.AppendLine("                                             得意先ＣＤ,");
                    sql.AppendLine("                                             喫食日 AS 売上日");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             注文ヘッダ");
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                             AND 配送日   = @配送日");
                    sql.AppendLine("                                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                             ) AS H");
                    sql.AppendLine("                             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                         ) AS KHD");
                    sql.AppendLine("                         ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                         AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                         AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                         AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.配送日 = @配送日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                     AND ISNULL(I.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                     AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                     AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.副食ＣＤ");
                    sql.AppendLine("                 ) AS PART2");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART2.コースＣＤ");
                    sql.AppendLine("                 ,PART2.得意先ＣＤ");
                    sql.AppendLine("                 ,PART2.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－３ 予測と注文の主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART3.コースＣＤ,");
                    sql.AppendLine("                 PART3.得意先ＣＤ,");
                    sql.AppendLine("                 PART3.部品ＣＤ,");
                    sql.AppendLine("                 SUM(ISNULL(PART3.商品注文数量, PART3.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                 null AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                         AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                     LEFT JOIN");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                            H.企業ＣＤ,");
                    sql.AppendLine("                            H.売上日,");
                    sql.AppendLine("                            H.得意先ＣＤ,");
                    sql.AppendLine("                            H.部署ＣＤ,");
                    sql.AppendLine("                            H.コースＣＤ,");
                    sql.AppendLine("                            D.商品ＣＤ,");
                    sql.AppendLine("                            D.明細番号");
                    sql.AppendLine("                         FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                 INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                     T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                     AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                             UNION");
                    sql.AppendLine("                             SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 売上ヘッダ T1");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日   = @配送日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 AND NOT EXISTS (");
                    sql.AppendLine("                                     SELECT 'X'");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         (SELECT");
                    sql.AppendLine("                                             企業ＣＤ,");
                    sql.AppendLine("                                             部署ＣＤ,");
                    sql.AppendLine("                                             コースＣＤ,");
                    sql.AppendLine("                                             得意先ＣＤ,");
                    sql.AppendLine("                                             喫食日 AS 売上日");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             注文ヘッダ");
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                             AND 配送日   = @配送日");
                    sql.AppendLine("                                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                             ) AS H");
                    sql.AppendLine("                             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                         ) AS KHD");
                    sql.AppendLine("                         ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                         AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                         AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                         AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.配送日 = @配送日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分主食");   // -- 弁当区分主食
                    sql.AppendLine("                     AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.主食ＣＤ");
                    sql.AppendLine("                 ) AS PART3");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART3.コースＣＤ");
                    sql.AppendLine("                 ,PART3.得意先ＣＤ");
                    sql.AppendLine("                 ,PART3.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－４ 予測と注文の副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART4.コースＣＤ,");
                    sql.AppendLine("                 PART4.得意先ＣＤ,");
                    sql.AppendLine("                 PART4.部品ＣＤ,");
                    sql.AppendLine("                 SUM(ISNULL(PART4.商品注文数量, PART4.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                 null AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                         AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                     LEFT JOIN");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                            H.企業ＣＤ,");
                    sql.AppendLine("                            H.売上日,");
                    sql.AppendLine("                            H.得意先ＣＤ,");
                    sql.AppendLine("                            H.部署ＣＤ,");
                    sql.AppendLine("                            H.コースＣＤ,");
                    sql.AppendLine("                            D.商品ＣＤ,");
                    sql.AppendLine("                            D.明細番号");
                    sql.AppendLine("                         FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                 INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                     T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                     AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                             UNION");
                    sql.AppendLine("                             SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 売上ヘッダ T1");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日   = @配送日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 AND NOT EXISTS (");
                    sql.AppendLine("                                     SELECT 'X'");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         (SELECT");
                    sql.AppendLine("                                             企業ＣＤ,");
                    sql.AppendLine("                                             部署ＣＤ,");
                    sql.AppendLine("                                             コースＣＤ,");
                    sql.AppendLine("                                             得意先ＣＤ,");
                    sql.AppendLine("                                             喫食日 AS 売上日");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             注文ヘッダ");
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                             AND 配送日   = @配送日");
                    sql.AppendLine("                                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                             ) AS H");
                    sql.AppendLine("                             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                         ) AS KHD");
                    sql.AppendLine("                         ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                         AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                         AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                         AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.配送日 = @配送日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分副食");   // -- 弁当区分副食
                    sql.AppendLine("                     AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.副食ＣＤ");
                    sql.AppendLine("                 ) AS PART4");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART4.コースＣＤ");
                    sql.AppendLine("                 ,PART4.得意先ＣＤ");
                    sql.AppendLine("                 ,PART4.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－５ 予測と注文の主食でも副食でもないもの
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART5.コースＣＤ,");
                    sql.AppendLine("                 PART5.得意先ＣＤ,");
                    sql.AppendLine("                 PART5.部品ＣＤ,");
                    sql.AppendLine("                 SUM(ISNULL(PART5.商品注文数量, PART5.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                 null AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.商品ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                         AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                     LEFT JOIN");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                            H.企業ＣＤ,");
                    sql.AppendLine("                            H.売上日,");
                    sql.AppendLine("                            H.得意先ＣＤ,");
                    sql.AppendLine("                            H.部署ＣＤ,");
                    sql.AppendLine("                            H.コースＣＤ,");
                    sql.AppendLine("                            D.商品ＣＤ,");
                    sql.AppendLine("                            D.明細番号");
                    sql.AppendLine("                         FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                 INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                     T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                     AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                             UNION");
                    sql.AppendLine("                             SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 売上ヘッダ T1");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日   = @配送日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 AND NOT EXISTS (");
                    sql.AppendLine("                                     SELECT 'X'");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         (SELECT");
                    sql.AppendLine("                                             企業ＣＤ,");
                    sql.AppendLine("                                             部署ＣＤ,");
                    sql.AppendLine("                                             コースＣＤ,");
                    sql.AppendLine("                                             得意先ＣＤ,");
                    sql.AppendLine("                                             喫食日 AS 売上日");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             注文ヘッダ");
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                             AND 配送日   = @配送日");
                    sql.AppendLine("                                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                             ) AS H");
                    sql.AppendLine("                             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                         ) AS KHD");
                    sql.AppendLine("                         ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                         AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                         AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                         AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.配送日 = @配送日");
                    sql.AppendLine("                     AND ISNULL(I.主食ＣＤ, 0) = 0");
                    sql.AppendLine("                     AND ISNULL(I.副食ＣＤ, 0) = 0");
                    sql.AppendLine("                     AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.商品ＣＤ");
                    sql.AppendLine("                 ) AS PART5");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART5.コースＣＤ");
                    sql.AppendLine("                 ,PART5.得意先ＣＤ");
                    sql.AppendLine("                 ,PART5.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－６ 予測と注文のその他の主食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART6.コースＣＤ,");
                    sql.AppendLine("                 PART6.得意先ＣＤ,");
                    sql.AppendLine("                 PART6.部品ＣＤ,");
                    sql.AppendLine("                 SUM(ISNULL(PART6.商品注文数量, PART6.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                 null AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                         AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                     LEFT JOIN");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                            H.企業ＣＤ,");
                    sql.AppendLine("                            H.売上日,");
                    sql.AppendLine("                            H.得意先ＣＤ,");
                    sql.AppendLine("                            H.部署ＣＤ,");
                    sql.AppendLine("                            H.コースＣＤ,");
                    sql.AppendLine("                            D.商品ＣＤ,");
                    sql.AppendLine("                            D.明細番号");
                    sql.AppendLine("                         FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                 INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                     T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                     AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                             UNION");
                    sql.AppendLine("                             SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 売上ヘッダ T1");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日   = @配送日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 AND NOT EXISTS (");
                    sql.AppendLine("                                     SELECT 'X'");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         (SELECT");
                    sql.AppendLine("                                             企業ＣＤ,");
                    sql.AppendLine("                                             部署ＣＤ,");
                    sql.AppendLine("                                             コースＣＤ,");
                    sql.AppendLine("                                             得意先ＣＤ,");
                    sql.AppendLine("                                             喫食日 AS 売上日");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             注文ヘッダ");
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                             AND 配送日   = @配送日");
                    sql.AppendLine("                                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                             ) AS H");
                    sql.AppendLine("                             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                         ) AS KHD");
                    sql.AppendLine("                         ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                         AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                         AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                         AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.配送日 = @配送日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                     AND ISNULL(I.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                     AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                     AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.主食ＣＤ");
                    sql.AppendLine("                 ) AS PART6");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART6.コースＣＤ");
                    sql.AppendLine("                 ,PART6.得意先ＣＤ");
                    sql.AppendLine("                 ,PART6.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－７ 予測と注文のその他の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART7.コースＣＤ,");
                    sql.AppendLine("                 PART7.得意先ＣＤ,");
                    sql.AppendLine("                 PART7.部品ＣＤ,");
                    sql.AppendLine("                 SUM(ISNULL(PART7.商品注文数量, PART7.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                 null AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文ヘッダ AS H");
                    sql.AppendLine("                     INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                         AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                     LEFT JOIN");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                            H.企業ＣＤ,");
                    sql.AppendLine("                            H.売上日,");
                    sql.AppendLine("                            H.得意先ＣＤ,");
                    sql.AppendLine("                            H.部署ＣＤ,");
                    sql.AppendLine("                            H.コースＣＤ,");
                    sql.AppendLine("                            D.商品ＣＤ,");
                    sql.AppendLine("                            D.明細番号");
                    sql.AppendLine("                         FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                 INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                     T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                     AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                             UNION");
                    sql.AppendLine("                             SELECT");
                    sql.AppendLine("                                 T1.企業ＣＤ,");
                    sql.AppendLine("                                 T1.売上日,");
                    sql.AppendLine("                                 T1.得意先ＣＤ,");
                    sql.AppendLine("                                 T1.部署ＣＤ,");
                    sql.AppendLine("                                 T1.コースＣＤ");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 売上ヘッダ T1");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日   = @配送日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 AND NOT EXISTS (");
                    sql.AppendLine("                                     SELECT 'X'");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         (SELECT");
                    sql.AppendLine("                                             企業ＣＤ,");
                    sql.AppendLine("                                             部署ＣＤ,");
                    sql.AppendLine("                                             コースＣＤ,");
                    sql.AppendLine("                                             得意先ＣＤ,");
                    sql.AppendLine("                                             喫食日 AS 売上日");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             注文ヘッダ");
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                             AND 配送日   = @配送日");
                    sql.AppendLine("                                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                             ) AS H");
                    sql.AppendLine("                             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                         ) AS KHD");
                    sql.AppendLine("                         ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                         AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                         AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                         AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.配送日 = @配送日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                     AND ISNULL(I.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                     AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                     AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     I.副食ＣＤ");
                    sql.AppendLine("                 ) AS PART7");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART7.コースＣＤ");
                    sql.AppendLine("                 ,PART7.得意先ＣＤ");
                    sql.AppendLine("                 ,PART7.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    sql.AppendLine(" ");
                    // -- ②－８ 売上の弁当の主食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                 null AS 商品注文予測数量,");
                    sql.AppendLine("                 SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         企業ＣＤ,");
                    sql.AppendLine("                         部署ＣＤ,");
                    sql.AppendLine("                         コースＣＤ,");
                    sql.AppendLine("                         得意先ＣＤ,");
                    sql.AppendLine("                         喫食日 AS 売上日");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND 配送日   = @配送日");
                    sql.AppendLine("                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                 UNION");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上ヘッダ T1");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND NOT EXISTS (");
                    sql.AppendLine("                         SELECT 'X'");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 企業ＣＤ,");
                    sql.AppendLine("                                 部署ＣＤ,");
                    sql.AppendLine("                                 コースＣＤ,");
                    sql.AppendLine("                                 得意先ＣＤ,");
                    sql.AppendLine("                                 喫食日 AS 売上日");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 注文ヘッダ");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                             ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                 ) AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");          // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ"); // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－９ 売上の弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                 null AS 商品注文予測数量,");
                    sql.AppendLine("                 SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         企業ＣＤ,");
                    sql.AppendLine("                         部署ＣＤ,");
                    sql.AppendLine("                         コースＣＤ,");
                    sql.AppendLine("                         得意先ＣＤ,");
                    sql.AppendLine("                         喫食日 AS 売上日");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND 配送日   = @配送日");
                    sql.AppendLine("                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                 UNION");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上ヘッダ T1");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND NOT EXISTS (");
                    sql.AppendLine("                         SELECT 'X'");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 企業ＣＤ,");
                    sql.AppendLine("                                 部署ＣＤ,");
                    sql.AppendLine("                                 コースＣＤ,");
                    sql.AppendLine("                                 得意先ＣＤ,");
                    sql.AppendLine("                                 喫食日 AS 売上日");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 注文ヘッダ");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                             ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                 ) AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");          // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ"); // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１０ 売上の主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                 null AS 商品注文予測数量,");
                    sql.AppendLine("                 SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         企業ＣＤ,");
                    sql.AppendLine("                         部署ＣＤ,");
                    sql.AppendLine("                         コースＣＤ,");
                    sql.AppendLine("                         得意先ＣＤ,");
                    sql.AppendLine("                         喫食日 AS 売上日");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND 配送日   = @配送日");
                    sql.AppendLine("                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                 UNION");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上ヘッダ T1");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND NOT EXISTS (");
                    sql.AppendLine("                         SELECT 'X'");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 企業ＣＤ,");
                    sql.AppendLine("                                 部署ＣＤ,");
                    sql.AppendLine("                                 コースＣＤ,");
                    sql.AppendLine("                                 得意先ＣＤ,");
                    sql.AppendLine("                                 喫食日 AS 売上日");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 注文ヘッダ");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                             ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                 ) AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１１ 売上の副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                 null AS 商品注文予測数量,");
                    sql.AppendLine("                 SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         企業ＣＤ,");
                    sql.AppendLine("                         部署ＣＤ,");
                    sql.AppendLine("                         コースＣＤ,");
                    sql.AppendLine("                         得意先ＣＤ,");
                    sql.AppendLine("                         喫食日 AS 売上日");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND 配送日   = @配送日");
                    sql.AppendLine("                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                 UNION");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上ヘッダ T1");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND NOT EXISTS (");
                    sql.AppendLine("                         SELECT 'X'");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 企業ＣＤ,");
                    sql.AppendLine("                                 部署ＣＤ,");
                    sql.AppendLine("                                 コースＣＤ,");
                    sql.AppendLine("                                 得意先ＣＤ,");
                    sql.AppendLine("                                 喫食日 AS 売上日");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 注文ヘッダ");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                             ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                 ) AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１２ 売上の主食でも副食でもないもの
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.商品ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                 null AS 商品注文予測数量,");
                    sql.AppendLine("                 SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         企業ＣＤ,");
                    sql.AppendLine("                         部署ＣＤ,");
                    sql.AppendLine("                         コースＣＤ,");
                    sql.AppendLine("                         得意先ＣＤ,");
                    sql.AppendLine("                         喫食日 AS 売上日");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND 配送日   = @配送日");
                    sql.AppendLine("                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                 UNION");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上ヘッダ T1");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND NOT EXISTS (");
                    sql.AppendLine("                         SELECT 'X'");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 企業ＣＤ,");
                    sql.AppendLine("                                 部署ＣＤ,");
                    sql.AppendLine("                                 コースＣＤ,");
                    sql.AppendLine("                                 得意先ＣＤ,");
                    sql.AppendLine("                                 喫食日 AS 売上日");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 注文ヘッダ");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                             ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                 ) AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 ISNULL(D.主食ＣＤ, 0) = 0");
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) = 0");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.商品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１３ 売上のその他の主食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                 null AS 商品注文予測数量,");
                    sql.AppendLine("                 SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         企業ＣＤ,");
                    sql.AppendLine("                         部署ＣＤ,");
                    sql.AppendLine("                         コースＣＤ,");
                    sql.AppendLine("                         得意先ＣＤ,");
                    sql.AppendLine("                         喫食日 AS 売上日");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND 配送日   = @配送日");
                    sql.AppendLine("                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                 UNION");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上ヘッダ T1");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND NOT EXISTS (");
                    sql.AppendLine("                         SELECT 'X'");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 企業ＣＤ,");
                    sql.AppendLine("                                 部署ＣＤ,");
                    sql.AppendLine("                                 コースＣＤ,");
                    sql.AppendLine("                                 得意先ＣＤ,");
                    sql.AppendLine("                                 喫食日 AS 売上日");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 注文ヘッダ");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                 ) AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");          // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ"); // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１４ 売上のその他の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                 null AS 商品注文予測数量,");
                    sql.AppendLine("                 SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         企業ＣＤ,");
                    sql.AppendLine("                         部署ＣＤ,");
                    sql.AppendLine("                         コースＣＤ,");
                    sql.AppendLine("                         得意先ＣＤ,");
                    sql.AppendLine("                         喫食日 AS 売上日");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND 配送日   = @配送日");
                    sql.AppendLine("                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                 UNION");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     T1.企業ＣＤ,");
                    sql.AppendLine("                     T1.売上日,");
                    sql.AppendLine("                     T1.得意先ＣＤ,");
                    sql.AppendLine("                     T1.部署ＣＤ,");
                    sql.AppendLine("                     T1.コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上ヘッダ T1");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND NOT EXISTS (");
                    sql.AppendLine("                         SELECT 'X'");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                 企業ＣＤ,");
                    sql.AppendLine("                                 部署ＣＤ,");
                    sql.AppendLine("                                 コースＣＤ,");
                    sql.AppendLine("                                 得意先ＣＤ,");
                    sql.AppendLine("                                 喫食日 AS 売上日");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 注文ヘッダ");
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                 ) AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");          // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ"); // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ,");
                    sql.AppendLine("                 H.得意先ＣＤ,");
                    sql.AppendLine("                 D.副食ＣＤ");
                    sql.AppendLine("             ) AS PART");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             PART.コースＣＤ,");
                    sql.AppendLine("             PART.得意先ＣＤ,");
                    sql.AppendLine("             PART.部品ＣＤ");
                    sql.AppendLine("         ) AS PARTSUM");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         PARTSUM.コースＣＤ,");
                    sql.AppendLine("         PARTSUM.部品ＣＤ");
                    sql.AppendLine("     ) AS INIT ON MAIN.コースＣＤ = INIT.コースＣＤ AND MAIN.商品ＣＤ = INIT.部品ＣＤ");
                    sql.AppendLine("     LEFT JOIN");
                    // -- ③持ち出し数
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         コースＣＤ");
                    sql.AppendLine("         ,商品ＣＤ");
                    sql.AppendLine("         ,SUM(個数) AS 個数");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         モバイル_持ち出し入力");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ AND");
                    sql.AppendLine("         持出日 = @配送日");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         コースＣＤ");
                    sql.AppendLine("         ,商品ＣＤ");
                    sql.AppendLine("     ) AS MCDS ON MAIN.コースＣＤ = MCDS.コースＣＤ AND MAIN.商品ＣＤ = MCDS.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN");
                    // -- ④工場からもらった個数
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         MI.相手コースＣＤ");
                    sql.AppendLine("         ,MI.商品ＣＤ");
                    sql.AppendLine("         ,SUM(MI.個数) AS 個数");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         モバイル_移動入力 AS MI");
                    sql.AppendLine("         LEFT JOIN コースマスタ AS CM");
                    sql.AppendLine("         ON MI.企業ＣＤ = CM.企業ＣＤ");
                    sql.AppendLine("         AND MI.コースＣＤ = CM.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         CM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND ISNULL(CM.工場フラグ, 0) <> 0");
                    sql.AppendLine("         AND MI.移動日 = @配送日");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         MI.相手コースＣＤ");
                    sql.AppendLine("         ,MI.商品ＣＤ");
                    sql.AppendLine("     ) AS TAKE_KOJO ON MAIN.コースＣＤ = TAKE_KOJO.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_KOJO.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN");
                    // -- ⑤工場以外からもらった個数
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         MI.相手コースＣＤ");
                    sql.AppendLine("         ,MI.商品ＣＤ");
                    sql.AppendLine("         ,SUM(MI.個数) AS 個数");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         モバイル_移動入力 AS MI");
                    sql.AppendLine("         LEFT JOIN コースマスタ AS CM");
                    sql.AppendLine("         ON MI.企業ＣＤ = CM.企業ＣＤ");
                    sql.AppendLine("         AND MI.コースＣＤ = CM.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         CM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND ISNULL(CM.工場フラグ, 0) = 0");
                    sql.AppendLine("         AND MI.移動日 = @配送日");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         MI.相手コースＣＤ");
                    sql.AppendLine("         ,MI.商品ＣＤ");
                    sql.AppendLine("     ) AS TAKE_IPPAN ON MAIN.コースＣＤ = TAKE_IPPAN.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_IPPAN.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN");
                    // -- ⑥渡した個数
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         コースＣＤ");
                    sql.AppendLine("         ,商品ＣＤ");
                    sql.AppendLine("         ,SUM(個数) AS 個数");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         モバイル_移動入力");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND 移動日 = @配送日");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         コースＣＤ");
                    sql.AppendLine("         ,商品ＣＤ");
                    sql.AppendLine("     ) AS GIVE ON MAIN.コースＣＤ = GIVE.コースＣＤ AND MAIN.商品ＣＤ = GIVE.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN");
                    // -- ⑦コースグループの検索条件＆ソート順用
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         グループ種別ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,グループＳＥＱ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         コース別グループマスタ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     ) AS CG ON CG.コースＣＤ = MAIN.コースＣＤ");

                    if (_idoNyuryokuCriteria.グループ種別ＣＤ != null && _idoNyuryokuCriteria.グループ種別ＣＤ != -1)
                    {
                        sql.AppendLine(" WHERE");
                        sql.AppendLine("     CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                    }

                    sql.AppendLine(" UNION");
                    // -- ⑧合計行
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     1 AS 合計行,");
                    sql.AppendLine("     NULL AS 工場フラグ,");
                    sql.AppendLine("     NULL AS コースＣＤ,");
                    sql.AppendLine("     '合計' AS コース名,");
                    sql.AppendLine("     '合計' AS 表示用コース名,");
                    sql.AppendLine("     TARGET.商品ＣＤ,");
                    sql.AppendLine("     TARGET.商品名略称,");
                    sql.AppendLine("     SUM(A.持ち出し数) AS 持ち出し数,");
                    sql.AppendLine("     SUM(A.売上予定) AS 売上予定,");
                    sql.AppendLine("     SUM(A.受取数_工場) AS 受取数_工場,");
                    sql.AppendLine("     SUM(A.受取数_一般) AS 受取数_一般,");
                    sql.AppendLine("     NULL AS 受取詳細,");
                    sql.AppendLine("     SUM(A.引渡数) AS 引渡数,");
                    sql.AppendLine("     NULL AS 引渡詳細,");
                    sql.AppendLine("     SUM(A.残数) AS 残数,");
                    sql.AppendLine("     NULL AS グループ種別ＣＤ,");
                    sql.AppendLine("     NULL AS グループＳＥＱ");
                    sql.AppendLine(" FROM");
                    // -- 駆動表からは工場は除くが、キーとなる商品ＣＤは工場も含める
                    sql.AppendLine("     (SELECT DISTINCT");
                    sql.AppendLine("         P.商品ＣＤ,");
                    sql.AppendLine("         S.商品名略称");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         主要部品リストマスタ AS P");
                    sql.AppendLine("         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         P.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND P.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND ISNULL(S.無効フラグ, 0) = 0");
                    sql.AppendLine("     ) AS TARGET");
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         MAIN.商品ＣＤ,");
                    sql.AppendLine("         MCDS.個数 AS 持ち出し数,");
                    sql.AppendLine("         INIT.数量 AS 売上予定,");
                    sql.AppendLine("         TAKE_KOJO.個数 AS 受取数_工場,");
                    sql.AppendLine("         TAKE_IPPAN.個数 AS 受取数_一般,");
                    sql.AppendLine("         GIVE.個数 AS 引渡数,");
                    sql.AppendLine("         CASE WHEN");
                    sql.AppendLine("             MCDS.個数       IS NULL AND");
                    sql.AppendLine("             TAKE_KOJO.個数  IS NULL AND");
                    sql.AppendLine("             TAKE_IPPAN.個数 IS NULL AND");
                    sql.AppendLine("             GIVE.個数       IS NULL AND");
                    sql.AppendLine("             INIT.数量       IS NULL THEN NULL");
                    sql.AppendLine("         ELSE ISNULL(MCDS.個数, 0) +");
                    sql.AppendLine("              ISNULL(TAKE_KOJO.個数, 0) +");
                    sql.AppendLine("              ISNULL(TAKE_IPPAN.個数, 0) -");
                    sql.AppendLine("              ISNULL(GIVE.個数, 0) -");
                    sql.AppendLine("              ISNULL(INIT.数量, 0)");
                    sql.AppendLine("         END AS 残数");
                    sql.AppendLine("     FROM");
                    // -- ①駆動表（コースと商品の全組み合わせ）
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             C.工場フラグ,");
                    sql.AppendLine("             C.コースＣＤ,");
                    sql.AppendLine("             C.コース名,");
                    sql.AppendLine("             P.商品ＣＤ");
                    sql.AppendLine("         FROM");
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 CM.企業ＣＤ");
                    sql.AppendLine("                 ,CM.部署ＣＤ");
                    sql.AppendLine("                 ,CM.コースＣＤ");
                    sql.AppendLine("                 ,CM.コース名");
                    sql.AppendLine("                 ,CM.工場フラグ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 コースマスタ AS CM");
                    sql.AppendLine("                 LEFT JOIN");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         臨時コースマスタ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                         AND コース区分 = @コース区分");
                    sql.AppendLine("                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND ISNULL(工場フラグ, 0) = 0");
                    sql.AppendLine("                         AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND 実施日 = @配送日");
                    sql.AppendLine("                     ) AS RM");
                    sql.AppendLine("                     ON RM.コースＣＤ = CM.コースＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 CM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND CM.コース区分 = @コース区分");
                    sql.AppendLine("                 AND CM.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                 AND ISNULL(CM.工場フラグ, 0) = 0");    // -- 合計行は工場は除く
                    sql.AppendLine("                 AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("             UNION");
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 RM.企業ＣＤ");
                    sql.AppendLine("                 ,RM.部署ＣＤ");
                    sql.AppendLine("                 ,RM.コースＣＤ");
                    sql.AppendLine("                 ,RM.コース名");
                    sql.AppendLine("                 ,RM.工場フラグ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 臨時コーステーブル AS RT");
                    sql.AppendLine("                 INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 RM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND RM.コース区分 = @コース区分");
                    sql.AppendLine("                 AND RM.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                 AND ISNULL(RM.工場フラグ, 0) = 0");    // -- 合計行は工場は除く
                    sql.AppendLine("                 AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND RM.実施日 = @配送日");
                    sql.AppendLine("             ) AS C");
                    sql.AppendLine("             INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = C.企業ＣＤ AND P.部署ＣＤ = C.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             ISNULL(S.無効フラグ, 0) = 0");
                    sql.AppendLine("         ) AS MAIN");
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             PARTSUM.コースＣＤ,");
                    sql.AppendLine("             PARTSUM.部品ＣＤ");
                    sql.AppendLine("             ,SUM(PARTSUM.数量) AS 数量");
                    sql.AppendLine("         FROM");
                    // -- ②予測と注文と売上の部品（各商品を分割した部品ごとに合計する）
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 PART.コースＣＤ,");
                    sql.AppendLine("                 PART.得意先ＣＤ,");
                    sql.AppendLine("                 PART.部品ＣＤ,");
                    sql.AppendLine("                 CASE WHEN ISNULL(SUM(PART.商品売上数量), -1) >= 0 THEN SUM(PART.商品売上数量)");
                    sql.AppendLine("                      ELSE ISNULL(SUM(PART.商品注文予測数量), 0) END");
                    sql.AppendLine("                 AS 数量");
                    sql.AppendLine("             FROM");
                    // -- ②－１ 予測と注文の弁当の主食
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     PART1.コースＣＤ,");
                    sql.AppendLine("                     PART1.得意先ＣＤ,");
                    sql.AppendLine("                     PART1.部品ＣＤ,");
                    sql.AppendLine("                     SUM(ISNULL(PART1.商品注文数量, PART1.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                     null AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ AS H");
                    sql.AppendLine("                         INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                             ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                             AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                             AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                         LEFT JOIN");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                H.企業ＣＤ,");
                    sql.AppendLine("                                H.売上日,");
                    sql.AppendLine("                                H.得意先ＣＤ,");
                    sql.AppendLine("                                H.部署ＣＤ,");
                    sql.AppendLine("                                H.コースＣＤ,");
                    sql.AppendLine("                                D.商品ＣＤ,");
                    sql.AppendLine("                                D.明細番号");
                    sql.AppendLine("                             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     (SELECT");
                    sql.AppendLine("                                         企業ＣＤ,");
                    sql.AppendLine("                                         部署ＣＤ,");
                    sql.AppendLine("                                         コースＣＤ,");
                    sql.AppendLine("                                         得意先ＣＤ,");
                    sql.AppendLine("                                         喫食日 AS 売上日");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         注文ヘッダ");
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                         AND 配送日   = @配送日");
                    sql.AppendLine("                                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                                 UNION");
                    sql.AppendLine("                                 SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     売上ヘッダ T1");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     AND NOT EXISTS (");
                    sql.AppendLine("                                         SELECT 'X'");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             (SELECT");
                    sql.AppendLine("                                                 企業ＣＤ,");
                    sql.AppendLine("                                                 部署ＣＤ,");
                    sql.AppendLine("                                                 コースＣＤ,");
                    sql.AppendLine("                                                 得意先ＣＤ,");
                    sql.AppendLine("                                                 喫食日 AS 売上日");
                    sql.AppendLine("                                             FROM");
                    sql.AppendLine("                                                 注文ヘッダ");
                    sql.AppendLine("                                             WHERE");
                    sql.AppendLine("                                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                                 ) AS H");
                    sql.AppendLine("                                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                             ) AS KHD");
                    sql.AppendLine("                             ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                             AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                             AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                             AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                         INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                             ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                             AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                             AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND H.配送日 = @配送日");
                    sql.AppendLine("                         AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                         AND ISNULL(I.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                         AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                         AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                     GROUP BY");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.主食ＣＤ");
                    sql.AppendLine("                     ) AS PART1");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     PART1.コースＣＤ");
                    sql.AppendLine("                     ,PART1.得意先ＣＤ");
                    sql.AppendLine("                     ,PART1.部品ＣＤ");
                    sql.AppendLine("                 UNION ALL");   // -- SUM()の対象にするので、同一コース、同一得意先、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                    // -- ②－２ 予測と注文の副食
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     PART2.コースＣＤ,");
                    sql.AppendLine("                     PART2.得意先ＣＤ,");
                    sql.AppendLine("                     PART2.部品ＣＤ,");
                    sql.AppendLine("                     SUM(ISNULL(PART2.商品注文数量, PART2.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                     null AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ AS H");
                    sql.AppendLine("                         INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                             ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                             AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                             AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                         LEFT JOIN");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                H.企業ＣＤ,");
                    sql.AppendLine("                                H.売上日,");
                    sql.AppendLine("                                H.得意先ＣＤ,");
                    sql.AppendLine("                                H.部署ＣＤ,");
                    sql.AppendLine("                                H.コースＣＤ,");
                    sql.AppendLine("                                D.商品ＣＤ,");
                    sql.AppendLine("                                D.明細番号");
                    sql.AppendLine("                             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     (SELECT");
                    sql.AppendLine("                                         企業ＣＤ,");
                    sql.AppendLine("                                         部署ＣＤ,");
                    sql.AppendLine("                                         コースＣＤ,");
                    sql.AppendLine("                                         得意先ＣＤ,");
                    sql.AppendLine("                                         喫食日 AS 売上日");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         注文ヘッダ");
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                         AND 配送日   = @配送日");
                    sql.AppendLine("                                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                                 UNION");
                    sql.AppendLine("                                 SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     売上ヘッダ T1");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     AND NOT EXISTS (");
                    sql.AppendLine("                                         SELECT 'X'");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             (SELECT");
                    sql.AppendLine("                                                 企業ＣＤ,");
                    sql.AppendLine("                                                 部署ＣＤ,");
                    sql.AppendLine("                                                 コースＣＤ,");
                    sql.AppendLine("                                                 得意先ＣＤ,");
                    sql.AppendLine("                                                 喫食日 AS 売上日");
                    sql.AppendLine("                                             FROM");
                    sql.AppendLine("                                                 注文ヘッダ");
                    sql.AppendLine("                                             WHERE");
                    sql.AppendLine("                                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                                 ) AS H");
                    sql.AppendLine("                                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                             ) AS KHD");
                    sql.AppendLine("                             ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                             AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                             AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                             AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                         INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                             ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                             AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                             AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND H.配送日 = @配送日");
                    sql.AppendLine("                         AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                         AND ISNULL(I.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                         AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                         AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                     GROUP BY");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.副食ＣＤ");
                    sql.AppendLine("                     ) AS PART2");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     PART2.コースＣＤ");
                    sql.AppendLine("                     ,PART2.得意先ＣＤ");
                    sql.AppendLine("                     ,PART2.部品ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－３ 予測と注文の主食のみ
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     PART3.コースＣＤ,");
                    sql.AppendLine("                     PART3.得意先ＣＤ,");
                    sql.AppendLine("                     PART3.部品ＣＤ,");
                    sql.AppendLine("                     SUM(ISNULL(PART3.商品注文数量, PART3.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                     null AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ AS H");
                    sql.AppendLine("                         INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                             ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                             AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                             AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                         LEFT JOIN");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                H.企業ＣＤ,");
                    sql.AppendLine("                                H.売上日,");
                    sql.AppendLine("                                H.得意先ＣＤ,");
                    sql.AppendLine("                                H.部署ＣＤ,");
                    sql.AppendLine("                                H.コースＣＤ,");
                    sql.AppendLine("                                D.商品ＣＤ,");
                    sql.AppendLine("                                D.明細番号");
                    sql.AppendLine("                             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     (SELECT");
                    sql.AppendLine("                                         企業ＣＤ,");
                    sql.AppendLine("                                         部署ＣＤ,");
                    sql.AppendLine("                                         コースＣＤ,");
                    sql.AppendLine("                                         得意先ＣＤ,");
                    sql.AppendLine("                                         喫食日 AS 売上日");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         注文ヘッダ");
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                         AND 配送日   = @配送日");
                    sql.AppendLine("                                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                                 UNION");
                    sql.AppendLine("                                 SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     売上ヘッダ T1");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     AND NOT EXISTS (");
                    sql.AppendLine("                                         SELECT 'X'");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             (SELECT");
                    sql.AppendLine("                                                 企業ＣＤ,");
                    sql.AppendLine("                                                 部署ＣＤ,");
                    sql.AppendLine("                                                 コースＣＤ,");
                    sql.AppendLine("                                                 得意先ＣＤ,");
                    sql.AppendLine("                                                 喫食日 AS 売上日");
                    sql.AppendLine("                                             FROM");
                    sql.AppendLine("                                                 注文ヘッダ");
                    sql.AppendLine("                                             WHERE");
                    sql.AppendLine("                                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                                 ) AS H");
                    sql.AppendLine("                                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                             ) AS KHD");
                    sql.AppendLine("                             ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                             AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                             AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                             AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                         INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                             ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                             AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                             AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND H.配送日 = @配送日");
                    sql.AppendLine("                         AND ISNULL(I.弁当区分, 0) = @弁当区分主食");   // -- 弁当区分主食
                    sql.AppendLine("                         AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                     GROUP BY");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.主食ＣＤ");
                    sql.AppendLine("                     ) AS PART3");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     PART3.コースＣＤ");
                    sql.AppendLine("                     ,PART3.得意先ＣＤ");
                    sql.AppendLine("                     ,PART3.部品ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－４ 予測と注文の副食のみ
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     PART4.コースＣＤ,");
                    sql.AppendLine("                     PART4.得意先ＣＤ,");
                    sql.AppendLine("                     PART4.部品ＣＤ,");
                    sql.AppendLine("                     SUM(ISNULL(PART4.商品注文数量, PART4.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                     null AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ AS H");
                    sql.AppendLine("                         INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                             ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                             AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                             AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                         LEFT JOIN");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                H.企業ＣＤ,");
                    sql.AppendLine("                                H.売上日,");
                    sql.AppendLine("                                H.得意先ＣＤ,");
                    sql.AppendLine("                                H.部署ＣＤ,");
                    sql.AppendLine("                                H.コースＣＤ,");
                    sql.AppendLine("                                D.商品ＣＤ,");
                    sql.AppendLine("                                D.明細番号");
                    sql.AppendLine("                             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     (SELECT");
                    sql.AppendLine("                                         企業ＣＤ,");
                    sql.AppendLine("                                         部署ＣＤ,");
                    sql.AppendLine("                                         コースＣＤ,");
                    sql.AppendLine("                                         得意先ＣＤ,");
                    sql.AppendLine("                                         喫食日 AS 売上日");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         注文ヘッダ");
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                         AND 配送日   = @配送日");
                    sql.AppendLine("                                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                                 UNION");
                    sql.AppendLine("                                 SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     売上ヘッダ T1");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     AND NOT EXISTS (");
                    sql.AppendLine("                                         SELECT 'X'");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             (SELECT");
                    sql.AppendLine("                                                 企業ＣＤ,");
                    sql.AppendLine("                                                 部署ＣＤ,");
                    sql.AppendLine("                                                 コースＣＤ,");
                    sql.AppendLine("                                                 得意先ＣＤ,");
                    sql.AppendLine("                                                 喫食日 AS 売上日");
                    sql.AppendLine("                                             FROM");
                    sql.AppendLine("                                                 注文ヘッダ");
                    sql.AppendLine("                                             WHERE");
                    sql.AppendLine("                                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                                 ) AS H");
                    sql.AppendLine("                                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                             ) AS KHD");
                    sql.AppendLine("                             ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                             AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                             AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                             AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                         INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                             ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                             AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                             AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND H.配送日 = @配送日");
                    sql.AppendLine("                         AND ISNULL(I.弁当区分, 0) = @弁当区分副食");   // -- 弁当区分副食
                    sql.AppendLine("                         AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                     GROUP BY");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.副食ＣＤ");
                    sql.AppendLine("                     ) AS PART4");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     PART4.コースＣＤ");
                    sql.AppendLine("                     ,PART4.得意先ＣＤ");
                    sql.AppendLine("                     ,PART4.部品ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－５ 予測と注文の主食でも副食でもないもの
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     PART5.コースＣＤ,");
                    sql.AppendLine("                     PART5.得意先ＣＤ,");
                    sql.AppendLine("                     PART5.部品ＣＤ,");
                    sql.AppendLine("                     SUM(ISNULL(PART5.商品注文数量, PART5.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                     null AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.商品ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ AS H");
                    sql.AppendLine("                         INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                             ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                             AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                             AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                         LEFT JOIN");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                H.企業ＣＤ,");
                    sql.AppendLine("                                H.売上日,");
                    sql.AppendLine("                                H.得意先ＣＤ,");
                    sql.AppendLine("                                H.部署ＣＤ,");
                    sql.AppendLine("                                H.コースＣＤ,");
                    sql.AppendLine("                                D.商品ＣＤ,");
                    sql.AppendLine("                                D.明細番号");
                    sql.AppendLine("                             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     (SELECT");
                    sql.AppendLine("                                         企業ＣＤ,");
                    sql.AppendLine("                                         部署ＣＤ,");
                    sql.AppendLine("                                         コースＣＤ,");
                    sql.AppendLine("                                         得意先ＣＤ,");
                    sql.AppendLine("                                         喫食日 AS 売上日");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         注文ヘッダ");
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                         AND 配送日   = @配送日");
                    sql.AppendLine("                                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                                 UNION");
                    sql.AppendLine("                                 SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     売上ヘッダ T1");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     AND NOT EXISTS (");
                    sql.AppendLine("                                         SELECT 'X'");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             (SELECT");
                    sql.AppendLine("                                                 企業ＣＤ,");
                    sql.AppendLine("                                                 部署ＣＤ,");
                    sql.AppendLine("                                                 コースＣＤ,");
                    sql.AppendLine("                                                 得意先ＣＤ,");
                    sql.AppendLine("                                                 喫食日 AS 売上日");
                    sql.AppendLine("                                             FROM");
                    sql.AppendLine("                                                 注文ヘッダ");
                    sql.AppendLine("                                             WHERE");
                    sql.AppendLine("                                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                                 ) AS H");
                    sql.AppendLine("                                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                             ) AS KHD");
                    sql.AppendLine("                             ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                             AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                             AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                             AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                         INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                             ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                             AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                             AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND H.配送日 = @配送日");
                    sql.AppendLine("                         AND ISNULL(I.主食ＣＤ, 0) = 0");
                    sql.AppendLine("                         AND ISNULL(I.副食ＣＤ, 0) = 0");
                    sql.AppendLine("                         AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                     GROUP BY");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.商品ＣＤ");
                    sql.AppendLine("                     ) AS PART5");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     PART5.コースＣＤ");
                    sql.AppendLine("                     ,PART5.得意先ＣＤ");
                    sql.AppendLine("                     ,PART5.部品ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－６ 予測と注文のその他の主食
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     PART6.コースＣＤ,");
                    sql.AppendLine("                     PART6.得意先ＣＤ,");
                    sql.AppendLine("                     PART6.部品ＣＤ,");
                    sql.AppendLine("                     SUM(ISNULL(PART6.商品注文数量, PART6.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                     null AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ AS H");
                    sql.AppendLine("                         INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                             ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                             AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                             AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ   = H.部署ＣＤ");
                    //  -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    //  -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                         LEFT JOIN");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                H.企業ＣＤ,");
                    sql.AppendLine("                                H.売上日,");
                    sql.AppendLine("                                H.得意先ＣＤ,");
                    sql.AppendLine("                                H.部署ＣＤ,");
                    sql.AppendLine("                                H.コースＣＤ,");
                    sql.AppendLine("                                D.商品ＣＤ,");
                    sql.AppendLine("                                D.明細番号");
                    sql.AppendLine("                             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     (SELECT");
                    sql.AppendLine("                                         企業ＣＤ,");
                    sql.AppendLine("                                         部署ＣＤ,");
                    sql.AppendLine("                                         コースＣＤ,");
                    sql.AppendLine("                                         得意先ＣＤ,");
                    sql.AppendLine("                                         喫食日 AS 売上日");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         注文ヘッダ");
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                         AND 配送日   = @配送日");
                    sql.AppendLine("                                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                                 UNION");
                    sql.AppendLine("                                 SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     売上ヘッダ T1");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     AND NOT EXISTS (");
                    sql.AppendLine("                                         SELECT 'X'");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             (SELECT");
                    sql.AppendLine("                                                 企業ＣＤ,");
                    sql.AppendLine("                                                 部署ＣＤ,");
                    sql.AppendLine("                                                 コースＣＤ,");
                    sql.AppendLine("                                                 得意先ＣＤ,");
                    sql.AppendLine("                                                 喫食日 AS 売上日");
                    sql.AppendLine("                                             FROM");
                    sql.AppendLine("                                                 注文ヘッダ");
                    sql.AppendLine("                                             WHERE");
                    sql.AppendLine("                                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                                 ) AS H");
                    sql.AppendLine("                                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                             ) AS KHD");
                    sql.AppendLine("                             ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                             AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                             AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                             AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                         INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                             ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                             AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                             AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND H.配送日 = @配送日");
                    sql.AppendLine("                         AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                         AND ISNULL(I.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                         AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                         AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                     GROUP BY");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.主食ＣＤ");
                    sql.AppendLine("                     ) AS PART6");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     PART6.コースＣＤ");
                    sql.AppendLine("                     ,PART6.得意先ＣＤ");
                    sql.AppendLine("                     ,PART6.部品ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－７ 予測と注文のその他の副食
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     PART7.コースＣＤ,");
                    sql.AppendLine("                     PART7.得意先ＣＤ,");
                    sql.AppendLine("                     PART7.部品ＣＤ,");
                    sql.AppendLine("                     SUM(ISNULL(PART7.商品注文数量, PART7.商品予測数量)) AS 商品注文予測数量,");
                    sql.AppendLine("                     null AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量,");
                    sql.AppendLine("                         SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         注文ヘッダ AS H");
                    sql.AppendLine("                         INNER JOIN 注文明細 AS D");
                    sql.AppendLine("                             ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                             AND D.注文区分   = H.注文区分");
                    sql.AppendLine("                             AND D.喫食日     = H.喫食日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ   = H.部署ＣＤ");
                    // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                    // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                    sql.AppendLine("                         LEFT JOIN");
                    sql.AppendLine("                             (SELECT");
                    sql.AppendLine("                                H.企業ＣＤ,");
                    sql.AppendLine("                                H.売上日,");
                    sql.AppendLine("                                H.得意先ＣＤ,");
                    sql.AppendLine("                                H.部署ＣＤ,");
                    sql.AppendLine("                                H.コースＣＤ,");
                    sql.AppendLine("                                D.商品ＣＤ,");
                    sql.AppendLine("                                D.明細番号");
                    sql.AppendLine("                             FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     (SELECT");
                    sql.AppendLine("                                         企業ＣＤ,");
                    sql.AppendLine("                                         部署ＣＤ,");
                    sql.AppendLine("                                         コースＣＤ,");
                    sql.AppendLine("                                         得意先ＣＤ,");
                    sql.AppendLine("                                         喫食日 AS 売上日");
                    sql.AppendLine("                                     FROM");
                    sql.AppendLine("                                         注文ヘッダ");
                    sql.AppendLine("                                     WHERE");
                    sql.AppendLine("                                         企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                         AND 配送日   = @配送日");
                    sql.AppendLine("                                         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                     INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                                         T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                         AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                         AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                         AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                                 UNION");
                    sql.AppendLine("                                 SELECT");
                    sql.AppendLine("                                     T1.企業ＣＤ,");
                    sql.AppendLine("                                     T1.売上日,");
                    sql.AppendLine("                                     T1.得意先ＣＤ,");
                    sql.AppendLine("                                     T1.部署ＣＤ,");
                    sql.AppendLine("                                     T1.コースＣＤ");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     売上ヘッダ T1");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND T1.売上日   = @配送日");
                    sql.AppendLine("                                     AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                     AND NOT EXISTS (");
                    sql.AppendLine("                                         SELECT 'X'");
                    sql.AppendLine("                                         FROM");
                    sql.AppendLine("                                             (SELECT");
                    sql.AppendLine("                                                 企業ＣＤ,");
                    sql.AppendLine("                                                 部署ＣＤ,");
                    sql.AppendLine("                                                 コースＣＤ,");
                    sql.AppendLine("                                                 得意先ＣＤ,");
                    sql.AppendLine("                                                 喫食日 AS 売上日");
                    sql.AppendLine("                                             FROM");
                    sql.AppendLine("                                                 注文ヘッダ");
                    sql.AppendLine("                                             WHERE");
                    sql.AppendLine("                                                 企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                                 AND 配送日   = @配送日");
                    sql.AppendLine("                                                 AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                             ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                                         WHERE");
                    sql.AppendLine("                                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                             AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                                 ) AS H");
                    sql.AppendLine("                                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                             ) AS KHD");
                    sql.AppendLine("                             ON H.企業ＣＤ = KHD.企業ＣＤ");
                    sql.AppendLine("                             AND H.コースＣＤ = KHD.コースＣＤ");
                    sql.AppendLine("                             AND D.喫食日 = KHD.売上日");
                    sql.AppendLine("                             AND D.得意先ＣＤ = KHD.得意先ＣＤ");
                    sql.AppendLine("                             AND D.部署ＣＤ = KHD.部署ＣＤ");
                    sql.AppendLine("                             AND D.商品ＣＤ = KHD.商品ＣＤ");
                    sql.AppendLine("                         INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                             ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                             AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                             AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND H.配送日 = @配送日");
                    sql.AppendLine("                         AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                         AND ISNULL(I.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                         AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                         AND KHD.明細番号 IS NULL");
                    sql.AppendLine("                     GROUP BY");
                    sql.AppendLine("                         H.コースＣＤ,");
                    sql.AppendLine("                         H.得意先ＣＤ,");
                    sql.AppendLine("                         I.副食ＣＤ");
                    sql.AppendLine("                     ) AS PART7");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     PART7.コースＣＤ");
                    sql.AppendLine("                     ,PART7.得意先ＣＤ");
                    sql.AppendLine("                     ,PART7.部品ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－８ 売上の弁当の主食
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     null AS 商品注文予測数量,");
                    sql.AppendLine("                     SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                             企業ＣＤ,");
                    sql.AppendLine("                             部署ＣＤ,");
                    sql.AppendLine("                             コースＣＤ,");
                    sql.AppendLine("                             得意先ＣＤ,");
                    sql.AppendLine("                             喫食日 AS 売上日");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             注文ヘッダ");
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                             AND 配送日   = @配送日");
                    sql.AppendLine("                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                     UNION");
                    sql.AppendLine("                     SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         売上ヘッダ T1");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND T1.売上日   = @配送日");
                    sql.AppendLine("                         AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND NOT EXISTS (");
                    sql.AppendLine("                             SELECT 'X'");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                 AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                     ) AS H");
                    sql.AppendLine("                     INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.売上日     = H.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                     AND ISNULL(D.主食ＣＤ, 0) <> 0");          // -- 主食がある
                    sql.AppendLine("                     AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ"); // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.主食ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－９ 売上の弁当の副食
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     null AS 商品注文予測数量,");
                    sql.AppendLine("                     SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                             企業ＣＤ,");
                    sql.AppendLine("                             部署ＣＤ,");
                    sql.AppendLine("                             コースＣＤ,");
                    sql.AppendLine("                             得意先ＣＤ,");
                    sql.AppendLine("                             喫食日 AS 売上日");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             注文ヘッダ");
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                             AND 配送日   = @配送日");
                    sql.AppendLine("                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                     UNION");
                    sql.AppendLine("                     SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         売上ヘッダ T1");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND T1.売上日   = @配送日");
                    sql.AppendLine("                         AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND NOT EXISTS (");
                    sql.AppendLine("                             SELECT 'X'");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                 AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                     ) AS H");
                    sql.AppendLine("                     INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.売上日     = H.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                     AND ISNULL(D.副食ＣＤ, 0) <> 0");          // -- 副食がある
                    sql.AppendLine("                     AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ"); // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.副食ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－１０ 売上の主食のみ
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     null AS 商品注文予測数量,");
                    sql.AppendLine("                     SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                             企業ＣＤ,");
                    sql.AppendLine("                             部署ＣＤ,");
                    sql.AppendLine("                             コースＣＤ,");
                    sql.AppendLine("                             得意先ＣＤ,");
                    sql.AppendLine("                             喫食日 AS 売上日");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             注文ヘッダ");
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                             AND 配送日   = @配送日");
                    sql.AppendLine("                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                     UNION");
                    sql.AppendLine("                     SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         売上ヘッダ T1");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND T1.売上日   = @配送日");
                    sql.AppendLine("                         AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND NOT EXISTS (");
                    sql.AppendLine("                             SELECT 'X'");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                 AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                     ) AS H");
                    sql.AppendLine("                     INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.売上日     = H.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.主食ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－１１ 売上の副食のみ
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     null AS 商品注文予測数量,");
                    sql.AppendLine("                     SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                             企業ＣＤ,");
                    sql.AppendLine("                             部署ＣＤ,");
                    sql.AppendLine("                             コースＣＤ,");
                    sql.AppendLine("                             得意先ＣＤ,");
                    sql.AppendLine("                             喫食日 AS 売上日");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             注文ヘッダ");
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                             AND 配送日   = @配送日");
                    sql.AppendLine("                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                     UNION");
                    sql.AppendLine("                     SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         売上ヘッダ T1");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND T1.売上日   = @配送日");
                    sql.AppendLine("                         AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND NOT EXISTS (");
                    sql.AppendLine("                             SELECT 'X'");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                 AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                     ) AS H");
                    sql.AppendLine("                     INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.売上日     = H.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.副食ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－１２ 売上の主食でも副食でもないもの
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.商品ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     null AS 商品注文予測数量,");
                    sql.AppendLine("                     SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                             企業ＣＤ,");
                    sql.AppendLine("                             部署ＣＤ,");
                    sql.AppendLine("                             コースＣＤ,");
                    sql.AppendLine("                             得意先ＣＤ,");
                    sql.AppendLine("                             喫食日 AS 売上日");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             注文ヘッダ");
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                             AND 配送日   = @配送日");
                    sql.AppendLine("                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                     UNION");
                    sql.AppendLine("                     SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         売上ヘッダ T1");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND T1.売上日   = @配送日");
                    sql.AppendLine("                         AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND NOT EXISTS (");
                    sql.AppendLine("                             SELECT 'X'");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2");    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                 AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                     ) AS H");
                    sql.AppendLine("                     INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.売上日     = H.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     ISNULL(D.主食ＣＤ, 0) = 0");
                    sql.AppendLine("                     AND ISNULL(D.副食ＣＤ, 0) = 0");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.商品ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－１３ 売上のその他の主食
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.主食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     null AS 商品注文予測数量,");
                    sql.AppendLine("                     SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                             企業ＣＤ,");
                    sql.AppendLine("                             部署ＣＤ,");
                    sql.AppendLine("                             コースＣＤ,");
                    sql.AppendLine("                             得意先ＣＤ,");
                    sql.AppendLine("                             喫食日 AS 売上日");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             注文ヘッダ");
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                             AND 配送日   = @配送日");
                    sql.AppendLine("                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                     UNION");
                    sql.AppendLine("                     SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         売上ヘッダ T1");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND T1.売上日   = @配送日");
                    sql.AppendLine("                         AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND NOT EXISTS (");
                    sql.AppendLine("                             SELECT 'X'");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                 AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                     ) AS H");
                    sql.AppendLine("                     INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.売上日     = H.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                     AND ISNULL(D.主食ＣＤ, 0) <> 0");          // -- 主食がある
                    sql.AppendLine("                     AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ"); // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.主食ＣＤ");
                    sql.AppendLine("                 UNION ALL");
                    // -- ②－１４ 売上のその他の副食
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.副食ＣＤ AS 部品ＣＤ,");
                    sql.AppendLine("                     null AS 商品注文予測数量,");
                    sql.AppendLine("                     SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("                 FROM");
                    // -- 喫食日売上ヘッダ
                    // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                    // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         (SELECT");
                    sql.AppendLine("                             企業ＣＤ,");
                    sql.AppendLine("                             部署ＣＤ,");
                    sql.AppendLine("                             コースＣＤ,");
                    sql.AppendLine("                             得意先ＣＤ,");
                    sql.AppendLine("                             喫食日 AS 売上日");
                    sql.AppendLine("                         FROM");
                    sql.AppendLine("                             注文ヘッダ");
                    sql.AppendLine("                         WHERE");
                    sql.AppendLine("                             企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                             AND 配送日   = @配送日");
                    sql.AppendLine("                             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         ) T2 ");   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                         INNER JOIN 売上ヘッダ T1 ON");
                    sql.AppendLine("                             T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                             AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                             AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                             AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                             AND T1.コースＣＤ = T2.コースＣＤ");
                    sql.AppendLine("                     UNION");
                    sql.AppendLine("                     SELECT");
                    sql.AppendLine("                         T1.企業ＣＤ,");
                    sql.AppendLine("                         T1.売上日,");
                    sql.AppendLine("                         T1.得意先ＣＤ,");
                    sql.AppendLine("                         T1.部署ＣＤ,");
                    sql.AppendLine("                         T1.コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         売上ヘッダ T1");
                    sql.AppendLine("                     WHERE");
                    sql.AppendLine("                         T1.企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                         AND T1.売上日   = @配送日");
                    sql.AppendLine("                         AND T1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                         AND NOT EXISTS (");
                    sql.AppendLine("                             SELECT 'X'");
                    sql.AppendLine("                             FROM");
                    sql.AppendLine("                                 (SELECT");
                    sql.AppendLine("                                     企業ＣＤ,");
                    sql.AppendLine("                                     部署ＣＤ,");
                    sql.AppendLine("                                     コースＣＤ,");
                    sql.AppendLine("                                     得意先ＣＤ,");
                    sql.AppendLine("                                     喫食日 AS 売上日");
                    sql.AppendLine("                                 FROM");
                    sql.AppendLine("                                     注文ヘッダ");
                    sql.AppendLine("                                 WHERE");
                    sql.AppendLine("                                     企業ＣＤ     = @企業ＣＤ");
                    sql.AppendLine("                                     AND 配送日   = @配送日");
                    sql.AppendLine("                                     AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                                 ) T2 ");       // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                    sql.AppendLine("                             WHERE");
                    sql.AppendLine("                                 T1.企業ＣＤ       = T2.企業ＣＤ");
                    sql.AppendLine("                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ");
                    sql.AppendLine("                                 AND T1.売上日     = T2.売上日");
                    sql.AppendLine("                                 AND T1.部署ＣＤ   = T2.部署ＣＤ");
                    sql.AppendLine("                                 AND T1.コースＣＤ = T2.コースＣＤ)");
                    sql.AppendLine("                     ) AS H");
                    sql.AppendLine("                     INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                         AND D.売上日     = H.売上日");
                    sql.AppendLine("                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                     AND ISNULL(D.副食ＣＤ, 0) <> 0");          // -- 副食がある
                    sql.AppendLine("                     AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ"); // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ,");
                    sql.AppendLine("                     H.得意先ＣＤ,");
                    sql.AppendLine("                     D.副食ＣＤ");
                    sql.AppendLine("                 ) AS PART");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART.コースＣＤ,");
                    sql.AppendLine("                 PART.得意先ＣＤ,");
                    sql.AppendLine("                 PART.部品ＣＤ");
                    sql.AppendLine("             ) AS PARTSUM");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             PARTSUM.コースＣＤ,");
                    sql.AppendLine("             PARTSUM.部品ＣＤ");
                    sql.AppendLine("         ) AS INIT ON MAIN.コースＣＤ = INIT.コースＣＤ AND MAIN.商品ＣＤ = INIT.部品ＣＤ");
                    sql.AppendLine("         LEFT JOIN");
                    // -- ③持ち出し数
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("             ,商品ＣＤ");
                    sql.AppendLine("             ,SUM(個数) AS 個数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_持ち出し入力");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ AND");
                    sql.AppendLine("             持出日 = @配送日");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("             ,商品ＣＤ");
                    sql.AppendLine("         ) AS MCDS ON MAIN.コースＣＤ = MCDS.コースＣＤ AND MAIN.商品ＣＤ = MCDS.商品ＣＤ");
                    sql.AppendLine("         LEFT JOIN");
                    // -- ④工場からもらった個数
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             MI.相手コースＣＤ");
                    sql.AppendLine("             ,MI.商品ＣＤ");
                    sql.AppendLine("             ,SUM(MI.個数) AS 個数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_移動入力 AS MI");
                    sql.AppendLine("             LEFT JOIN コースマスタ AS CM");
                    sql.AppendLine("             ON MI.企業ＣＤ = CM.企業ＣＤ");
                    sql.AppendLine("             AND MI.コースＣＤ = CM.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND ISNULL(CM.工場フラグ, 0) <> 0");
                    sql.AppendLine("             AND MI.移動日 = @配送日");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             MI.相手コースＣＤ");
                    sql.AppendLine("             ,MI.商品ＣＤ");
                    sql.AppendLine("         ) AS TAKE_KOJO ON MAIN.コースＣＤ = TAKE_KOJO.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_KOJO.商品ＣＤ");
                    sql.AppendLine("         LEFT JOIN");
                    // -- ⑤工場以外からもらった個数
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             MI.相手コースＣＤ");
                    sql.AppendLine("             ,MI.商品ＣＤ");
                    sql.AppendLine("             ,SUM(MI.個数) AS 個数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_移動入力 AS MI");
                    sql.AppendLine("             LEFT JOIN コースマスタ AS CM");
                    sql.AppendLine("             ON MI.企業ＣＤ = CM.企業ＣＤ");
                    sql.AppendLine("             AND MI.コースＣＤ = CM.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND ISNULL(CM.工場フラグ, 0) = 0");
                    sql.AppendLine("             AND MI.移動日 = @配送日");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             MI.相手コースＣＤ");
                    sql.AppendLine("             ,MI.商品ＣＤ");
                    sql.AppendLine("         ) AS TAKE_IPPAN ON MAIN.コースＣＤ = TAKE_IPPAN.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_IPPAN.商品ＣＤ");
                    sql.AppendLine("         LEFT JOIN");
                    // -- ⑥渡した個数
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("             ,商品ＣＤ");
                    sql.AppendLine("             ,SUM(個数) AS 個数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_移動入力");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 移動日 = @配送日");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("             ,商品ＣＤ");
                    sql.AppendLine("         ) AS GIVE ON MAIN.コースＣＤ = GIVE.コースＣＤ AND MAIN.商品ＣＤ = GIVE.商品ＣＤ");
                    sql.AppendLine("         LEFT JOIN");
                    // -- ⑦コースグループの検索条件＆ソート順用
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             グループ種別ＣＤ");
                    sql.AppendLine("             ,コースＣＤ");
                    sql.AppendLine("             ,グループＳＥＱ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コース別グループマスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         ) AS CG ON CG.コースＣＤ = MAIN.コースＣＤ");

                    if (_idoNyuryokuCriteria.グループ種別ＣＤ != null && _idoNyuryokuCriteria.グループ種別ＣＤ != -1)
                    {
                        sql.AppendLine("     WHERE");
                        sql.AppendLine("         CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                    }

                    sql.AppendLine("     ) AS A ON A.商品ＣＤ = TARGET.商品ＣＤ");
                    sql.AppendLine(" GROUP BY");
                    sql.AppendLine("     TARGET.商品ＣＤ,");
                    sql.AppendLine("     TARGET.商品名略称");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _idoNyuryokuCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _idoNyuryokuCriteria.コース区分;
                    cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _idoNyuryokuCriteria.配送日;
                    if (_idoNyuryokuCriteria.グループ種別ＣＤ != null && _idoNyuryokuCriteria.グループ種別ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _idoNyuryokuCriteria.グループ種別ＣＤ;
                    }
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分その他", SqlDbType.Int).Value = (int)Consts.BentoKubun.Other;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("移動入力");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "移動入力";

                    log.Info("移動入力リストを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,  "表示用コース名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN,  "商品名略称");
                fpSpread1_Sheet1.BindDataColumn(MOCHIDASHI_COLUMN,   "持ち出し数");
                fpSpread1_Sheet1.BindDataColumn(URIAGE_YOTEI_COLUMN, "売上予定");
                fpSpread1_Sheet1.BindDataColumn(TAKE_KOJO_COLUMN,    "受取数_工場");
                fpSpread1_Sheet1.BindDataColumn(TAKE_IPPAN_COLUMN,   "受取数_一般");
                fpSpread1_Sheet1.BindDataColumn(TAKE_DETAIL_COLUMN,  "受取詳細");
                fpSpread1_Sheet1.BindDataColumn(GIVE_COLUMN,         "引渡数");
                fpSpread1_Sheet1.BindDataColumn(GIVE_DETAIL_COLUMN,  "引渡詳細");
                fpSpread1_Sheet1.BindDataColumn(ZANSU_COLUMN,        "残数");

                // 罫線と背景色をリセット
                int rowIndex;
                for (rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBorder();
                }

                // 工場フラグがONのコースの背景色を変える
                rowIndex = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (((dr["工場フラグ"] as bool?) ?? false) == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = Color.LightCyan;
                    }
                    rowIndex++;
                }

                // 最終の対象商品数行を固定行にする
                //fpSpread1_Sheet1.FrozenTrailingRowCount = syohinCount;
                for (rowIndex = 0; rowIndex < syohinCount; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - rowIndex - 1].BackColor = TOTAL_BACK_COLOR;
                    fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - rowIndex - 1].CanFocus = false;
                }

                // 罫線を重ねて描画する
                fpSpread1.BorderCollapse = FarPoint.Win.Spread.BorderCollapse.Collapse;
                // コースを区切る罫線を描画（コースが持つ商品の行数ごとに罫線を引く）
                FarPoint.Win.LineBorder border = new FarPoint.Win.LineBorder(SystemColors.ControlDark, 2, false, true, false, false);
                for (rowIndex = syohinCount; rowIndex < fpSpread1_Sheet1.RowCount - 1; rowIndex += syohinCount)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].Border = border;
                }

                if (fpSpread1_Sheet1.Rows.Count > syohinCount)  // 合計行のみの場合はフォーカスしない
                {
                    fpSpread1.Focus();
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _idoNyuryokuCriteriaSnapShot = new IdoNyuryokuCriteria(_idoNyuryokuCriteria);
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
        /// Spreadセルダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                // 列ヘッダー or 列フッター or 行ヘッダー？
                if (e.ColumnHeader || e.ColumnFooter || e.RowHeader)
                {
                    return;
                }

                // 合計行？
                if (fpSpread1_Sheet1.Rows[e.Row].BackColor == TOTAL_BACK_COLOR)
                {
                    return;
                }

                ShowDetail();       // 詳細表示
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
        /// Spreadキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    if (fpSpread1_Sheet1.SelectionCount != 0)
                    {
                        ShowDetail();       // 詳細表示
                    }
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
        /// Spreadセル移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            // 合計行には移動しない
            if (fpSpread1_Sheet1.Rows[e.NewRow].BackColor == TOTAL_BACK_COLOR)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 一括移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIkkatsu_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount != 0)
                {
                    if (fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.ActiveRowIndex].BackColor != TOTAL_BACK_COLOR)
                    {
                        ShowIkkatsu();       // 一括移動入力画面表示
                    }
                    else
                    {
                        MessageBox.Show(this, "合計行の一括移動は表示できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 詳細画面ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount != 0)
                {
                    if (fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.ActiveRowIndex].BackColor != TOTAL_BACK_COLOR)
                    {
                        ShowDetail();       // 詳細表示（既存レコード）
                    }
                    else
                    {
                        MessageBox.Show(this, "合計行の詳細は表示できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 一括移動入力画面表示
        /// </summary>
        private void ShowIkkatsu()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteria = IdoNyuryokuDetailCriteria.GetIdoNyuryokuDetailCriteria((int)dr["コースＣＤ"], (DateTime)_idoNyuryokuCriteria.配送日);
                idoNyuryokuDetailCriteria.配送日 = _idoNyuryokuCriteria.配送日;
                Form301B form = new Form301B(idoNyuryokuDetailCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                // モーダルにせず、複数開けるようにする
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form301D_FormClosed);     // 詳細画面クローズと兼用する
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteria = IdoNyuryokuDetailCriteria.GetIdoNyuryokuDetailCriteria((int)dr["コースＣＤ"], (DateTime)_idoNyuryokuCriteria.配送日);
                idoNyuryokuDetailCriteria.配送日 = _idoNyuryokuCriteria.配送日;
                Form301D form = new Form301D(idoNyuryokuDetailCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                // 2016/08/26 REP S モーダルにせず、複数開けるようにする
                //SubFormOpenInit(form);
                form.MdiParent = this.MdiParent;
                // 2016/08/22 REP E
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form301D_FormClosed);
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 詳細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form301D_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Form form = (Form)sender;
                form.Dispose();
                form = null;

                // 一覧画面を閉じた後に詳細画面で閉じられた場合、一覧画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();

                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                btnQuery.PerformClick();        // 再検索

                // 先頭行のインデックスを復元（そこにスクロール）
                fpSpread1.SetViewportTopRow(0, rowIndex);

                // アクティブセルも可能な限り復元する
                if (activeRowIndex >= fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount)
                {
                    activeRowIndex = fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount - 1;
                }
                if (0 <= activeRowIndex)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = activeRowIndex;
                    bindingSource1.Position = activeRowIndex;       // Currentプロパティがずれる場合があるのを回避
                    fpSpread1_Sheet1.AddSelection(activeRowIndex, -1, 1, -1);   // 単一選択行を復元
                }
                fpSpread1_Sheet1.ActiveColumnIndex = activeColumnIndex;
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
    }
}
