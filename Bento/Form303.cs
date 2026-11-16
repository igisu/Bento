using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 容器回収表印刷
    /// </summary>
    public partial class Form303 : BaseForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int PRINT_GET_SYOHINS = 16;       // 容器回収表に出力するために取得する商品の個数（その他を含めない）
        private const int PRINT_SYOHIN_COLUMNS = PRINT_GET_SYOHINS + 1; // 容器回収表に出力する商品の最大列数（その他を含める）
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 納品書発行
        /// </summary>
        private YokiKaisyuHyo _yokiKaisyuHyo;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form303()
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
        private void Form303_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.SetCourseKubun(cboCourseKbn);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _yokiKaisyuHyo = new YokiKaisyuHyo();
                yokiKaisyuHyoBindingSource.DataSource = _yokiKaisyuHyo;
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
        private void Form303_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F10:                      // 印刷ボタン押下
                        btnPrint.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// コースＣＤFromキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCDFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCDFrom.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤFrom参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCDFrom_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCDFrom_FormClosed);
                formCourseDlg.ShowDialog();
                if (formCourseDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// コース選択ダイアログクローズ（コースＣＤFrom）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCDFrom_FormClosed(object sender, FormClosedEventArgs e)
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
                Form902 form = (Form902)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCDFrom, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCDFrom.Text = course.コースＣＤ.ToString();
                    _yokiKaisyuHyo.コースＣＤFrom = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameFrom.Text = course.コース名;
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
        /// コースＣＤToキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCDTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCDTo.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤTo参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCDTo_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCDTo_FormClosed);
                formCourseDlg.ShowDialog();
                if (formCourseDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// コース選択ダイアログクローズ（コースＣＤTo）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCDTo_FormClosed(object sender, FormClosedEventArgs e)
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
                Form902 form = (Form902)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCDTo, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCDTo.Text = course.コースＣＤ.ToString();
                    _yokiKaisyuHyo.コースＣＤTo = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameTo.Text = course.コース名;
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
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_yokiKaisyuHyo.HasErrors)
                {
                    if (string.IsNullOrEmpty(_yokiKaisyuHyo.Error) == false)
                    {
                        MessageBox.Show(this, _yokiKaisyuHyo.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 売上日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_yokiKaisyuHyo.売上日 > warnDate)
                {
                    if (MessageBox.Show(this, "売上日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                if (_yokiKaisyuHyo.コースＣＤFrom == null && _yokiKaisyuHyo.コースＣＤTo == null)
                {
                    if (MessageBox.Show(this, "コースＣＤが未入力です。印刷に時間が掛かるおそれがあります。続行しますか？",
                        this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }

                // 帳票に該当するプリンタを取得。未選択ならダイアログを開く。そのあと印刷処理を実行
                DecidePrinter(Consts.Reports.YokiKaisyuhyo, PrintMain);
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
        /// 印刷処理
        /// </summary>
        /// <param name="printerNameA4">A4プリンタ名</param>
        private void PrintMain(string printerNameA4)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                DataTable syohinDt = GetSyohinTableForPrint();

                // データ本体を取得
                DataTable dt = new DataTable("売上明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" WITH 対象コース得意先 AS (");
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     CT.企業ＣＤ");
                    sql.AppendLine("     ,CT.コースＣＤ");
                    sql.AppendLine("     ,CM.コース名");
                    sql.AppendLine("     ,CM.担当者ＣＤ");
                    sql.AppendLine("     ,CT.得意先ＣＤ");
                    sql.AppendLine("     ,CT.コース順");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     コーステーブル AS CT");
                    sql.AppendLine("     INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             臨時コースマスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");

                    if (_yokiKaisyuHyo.コースＣＤFrom != null)
                    {
                        sql.AppendLine("             AND コースＣＤ >= @コースＣＤFrom");
                    }
                    if (_yokiKaisyuHyo.コースＣＤTo != null)
                    {
                        sql.AppendLine("             AND コースＣＤ <= @コースＣＤTo");
                    }

                    sql.AppendLine("             AND 実施日 = @売上日");
                    sql.AppendLine("         ) AS RM");
                    sql.AppendLine("         ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     CT.企業ＣＤ = @企業ＣＤ");

                    if (_yokiKaisyuHyo.コースＣＤFrom != null)
                    {
                        sql.AppendLine("     AND CT.コースＣＤ >= @コースＣＤFrom");
                    }
                    if (_yokiKaisyuHyo.コースＣＤTo != null)
                    {
                        sql.AppendLine("     AND CT.コースＣＤ <= @コースＣＤTo");
                    }

                    sql.AppendLine("     AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("     AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine(" UNION");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     RT.企業ＣＤ");
                    sql.AppendLine("     ,RT.コースＣＤ");
                    sql.AppendLine("     ,RM.コース名");
                    sql.AppendLine("     ,RM.担当者ＣＤ");
                    sql.AppendLine("     ,RT.得意先ＣＤ");
                    sql.AppendLine("     ,RT.コース順");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     臨時コーステーブル AS RT");
                    sql.AppendLine("     INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     RT.企業ＣＤ = @企業ＣＤ");

                    if (_yokiKaisyuHyo.コースＣＤFrom != null)
                    {
                        sql.AppendLine("     AND RT.コースＣＤ >= @コースＣＤFrom");
                    }
                    if (_yokiKaisyuHyo.コースＣＤTo != null)
                    {
                        sql.AppendLine("     AND RT.コースＣＤ <= @コースＣＤTo");
                    }

                    sql.AppendLine("     AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("     AND RT.実施日 = @売上日");
                    sql.AppendLine(" )");

                    // -- 駆動表
                    sql.AppendLine(" ,駆動表 AS (");
                    sql.AppendLine(" SELECT DISTINCT");
                    sql.AppendLine("     D.企業ＣＤ");
                    sql.AppendLine("     ,D.コースＣＤ");
                    sql.AppendLine("     ,D.コース名");
                    sql.AppendLine("     ,D.コース得意先 AS コース得意先ＣＤ");
                    sql.AppendLine("     ,D.得意先ＣＤ");
                    sql.AppendLine("     ,D.担当者ＣＤ");
                    sql.AppendLine("     ,D.コース順");
                    sql.AppendLine(" FROM");
                    // -- ①－１（指定されたコースにぶら下がる得意先(集約先)）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         TH.企業ＣＤ");
                    sql.AppendLine("         ,TH.コースＣＤ");
                    sql.AppendLine("         ,TH.コース名");
                    sql.AppendLine("         ,TH.得意先ＣＤ AS コース得意先");
                    sql.AppendLine("         ,TH.配送得意先ＣＤ");
                    sql.AppendLine("         ,SY.集約先ＣＤ");
                    sql.AppendLine("         ,ISNULL(SY.集約先ＣＤ, TH.得意先ＣＤ) AS 得意先ＣＤ");     // -- 他SQLに提供する得意先ＣＤ
                    sql.AppendLine("         ,TH.担当者ＣＤ");
                    sql.AppendLine("         ,TH.コース順 AS 得意先コース順");
                    sql.AppendLine("         ,SY.コース順 AS 集約先コース順");
                    sql.AppendLine("         ,ISNULL(SY.コース順, TH.コース順) AS コース順");       // -- 他SQLに提供するコース順
                    sql.AppendLine("     FROM");
                    // -- ①－１－１（指定されたコースにぶら下がる得意先と配送先）
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             W.企業ＣＤ");
                    sql.AppendLine("             ,W.得意先ＣＤ");
                    sql.AppendLine("             ,T.配送得意先ＣＤ");
                    sql.AppendLine("             ,W.コースＣＤ");
                    sql.AppendLine("             ,W.コース名");
                    sql.AppendLine("             ,W.担当者ＣＤ");
                    sql.AppendLine("             ,W.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             対象コース得意先 AS W");
                    // -- 得意先と配送先を取得したいのでLEFT JOINしている
                    sql.AppendLine("             LEFT JOIN 得意先マスタ AS T");
                    sql.AppendLine("                 ON T.企業ＣＤ = W.企業ＣＤ");
                    sql.AppendLine("                 AND T.得意先ＣＤ = W.得意先ＣＤ");
                    sql.AppendLine("         ) AS TH");
                    // -- ①－１－２（指定されたコースにぶら下がる得意先のうち、注文の集約先になりうる得意先を抽出）
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("             (SELECT DISTINCT");
                    sql.AppendLine("                 W.企業ＣＤ");
                    sql.AppendLine("                 ,W.得意先ＣＤ");
                    sql.AppendLine("                 ,T.配送得意先ＣＤ AS 集約先ＣＤ");
                    sql.AppendLine("                 ,W.コースＣＤ");
                    sql.AppendLine("                 ,W.担当者ＣＤ");
                    sql.AppendLine("                 ,W.コース順");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 対象コース得意先 AS W");
                    // -- 集約先になりうる得意先のみを抽出したいのでINNER JOINしている
                    // -- ホントは 「ISNULL(配送得意先ＣＤ, 得意先ＣＤ) AS 配送得意先ＣＤ」 として取得したかったが、インデックスが効かなくなるので
                    // -- T.配送得意先ＣＤ には NULL を格納せず、代わりに得意先ＣＤを格納している
                    sql.AppendLine("                 INNER JOIN 得意先マスタ AS T");
                    sql.AppendLine("                     ON T.企業ＣＤ = W.企業ＣＤ");
                    sql.AppendLine("                     AND T.配送得意先ＣＤ = W.得意先ＣＤ");
                    sql.AppendLine("             ) AS SY");
                    sql.AppendLine("             ON ISNULL(TH.配送得意先ＣＤ, TH.得意先ＣＤ) = SY.集約先ＣＤ");     // -- 集約先と結合するのは配送先を優先する（集約先≒配送先なので）
                    sql.AppendLine("     ) AS D");
                    // -- 以下の理由により、指定されたコースを持つ売上の得意先は取得しない。
                    // -- ・分配処理を行うと、分配先の売上ヘッダのコースは（分配先が元々コースに属していなくても）分配元のコースになっている
                    // -- ・容器回収表に出力するのは分配元のみにしたい
                    // -- ・分配先の数量は分配元として計上するので、分配先は容器回収表に出力されなくてよい
                    // -- ・売上時に指定したコースではなく、回収コースで出力することがある
                    // -- したがってコーステーブル（臨時コーステーブル）に属している得意先を出力対象とするが、分配先がコースに属していれば、それが出力されるのは回避できない（回避しない）
                    sql.AppendLine(" )");

                    // -- 容器回収表
                    // -- 明細
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     B.部署名,");
                    sql.AppendLine("     B.会社名称,");
                    sql.AppendLine("     B.電話番号,");
                    sql.AppendLine("     B.ＦＡＸ,");
                    sql.AppendLine("     '{0:yyyy/MM/dd(ddd)}' AS  売上日,");
                    sql.AppendLine("     C.コースＣＤ,");
                    sql.AppendLine("     C.コース名,");
                    sql.AppendLine("     D.担当者名,");
                    sql.AppendLine("     C.コース順,");
                    sql.AppendLine("     T.得意先ＣＤ,");
                    sql.AppendLine("     T.得意先名,");
                    sql.AppendLine("     T.電話番号1 AS 得意先電話番号,");
                    sql.AppendLine("     A.商品ＣＤ,");
                    sql.AppendLine("     A.商品数量,");
                    sql.AppendLine("     C.コースＣＤ AS ソート用コースＣＤ,");
                    sql.AppendLine("     C.コース順 AS ソート用コース順,");
                    sql.AppendLine("     T.得意先ＣＤ AS ソート用得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    sql.AppendLine("     駆動表 AS C");
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = C.企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ");
                    // -- ②駆動表に紐付く売上
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             C.得意先ＣＤ");
                    sql.AppendLine("             ,ISNULL(CONVERT(varchar, S.商品ＣＤ), 'XXX') AS 商品ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品数量) AS 商品数量");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS M");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("             INNER JOIN (");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     企業ＣＤ");
                    sql.AppendLine("                     ,得意先ＣＤ");
                    sql.AppendLine("                     ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     得意先マスタ");
                    sql.AppendLine("                 ) AS T");
                    sql.AppendLine("                 ON T.企業ＣＤ  = M.企業ＣＤ");
                    sql.AppendLine("                 AND T.得意先ＣＤ = M.得意先ＣＤ");
                    // -- 注文元の数量は配送先として計上する
                    sql.AppendLine("             INNER JOIN 駆動表 AS C");
                    sql.AppendLine("                 ON C.企業ＣＤ = T.企業ＣＤ");
                    sql.AppendLine("                 AND C.コース得意先ＣＤ = T.受注得意先ＣＤ");
                    sql.AppendLine("             INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日     = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN");
                    // -- 出力できる商品の最大列数は16（その他(XXX)を入れて17）
                    sql.AppendLine("                 (SELECT TOP({1})");
                    sql.AppendLine("                     SS.企業ＣＤ");
                    sql.AppendLine("                     ,SS.商品ＣＤ");
                    sql.AppendLine("                     ,SS.部署ＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     主要商品リストマスタ AS SS");
                    sql.AppendLine("                     LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = SS.企業ＣＤ AND SM.商品ＣＤ = SS.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     SS.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND SS.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND ISNULL(SS.無効フラグ, 0) = 0");
                    sql.AppendLine("                 ORDER BY");
                    sql.AppendLine("                     CASE WHEN SS.商品グループＣＤ IS NULL THEN 1 ELSE 0 END");
                    sql.AppendLine("                     ,SS.商品グループＣＤ");
                    sql.AppendLine("                     ,SM.弁当区分");
                    sql.AppendLine("                     ,SS.商品ＣＤ");
                    sql.AppendLine("                 ) AS S");
                    sql.AppendLine("                 ON S.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND S.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 AND S.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.売上日 = @売上日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             C.得意先ＣＤ");
                    sql.AppendLine("             ,S.商品ＣＤ");
                    sql.AppendLine("         ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS D ON D.企業ＣＤ = C.企業ＣＤ AND D.担当者ＣＤ = C.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = C.企業ＣＤ AND B.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine(" WHERE CM.コース区分 = @コース区分");
                    sql.AppendLine(" UNION");
                    // -- コースごとの合計
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     B.部署名,");
                    sql.AppendLine("     B.会社名称,");
                    sql.AppendLine("     B.電話番号,");
                    sql.AppendLine("     B.ＦＡＸ,");
                    sql.AppendLine("     '2022/01/28(金)' AS 売上日,");
                    sql.AppendLine("     C.コースＣＤ,");
                    sql.AppendLine("     C.コース名,");
                    sql.AppendLine("     D.担当者名,");
                    sql.AppendLine("     NULL AS コース順,");
                    sql.AppendLine("     NULL AS 得意先ＣＤ,");
                    sql.AppendLine("     '【合計】' AS 得意先名,");
                    sql.AppendLine("     NULL AS 得意先電話番号,");
                    sql.AppendLine("     A.商品ＣＤ,");
                    sql.AppendLine("     SUM(A.商品数量) AS 商品数量,");
                    sql.AppendLine("     C.コースＣＤ AS ソート用コースＣＤ,");
                    sql.AppendLine("     999999999 AS ソート用コース順,");
                    sql.AppendLine("     999999999 AS ソート用得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    sql.AppendLine("     駆動表 AS C");
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = C.企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ");
                    // -- ②駆動表に紐付く売上
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             T.受注得意先ＣＤ AS 得意先ＣＤ");
                    sql.AppendLine("             ,ISNULL(CONVERT(varchar, S.商品ＣＤ), 'XXX') AS 商品ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品数量) AS 商品数量");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS M");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("             INNER JOIN (");
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     企業ＣＤ");
                    sql.AppendLine("                     ,得意先ＣＤ");
                    sql.AppendLine("                     ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     得意先マスタ");
                    sql.AppendLine("                 ) AS T");
                    sql.AppendLine("                 ON T.企業ＣＤ  = M.企業ＣＤ");
                    sql.AppendLine("                 AND T.得意先ＣＤ = M.得意先ＣＤ");
                    // -- 注文元の数量を配送先として計上すると、配送先として集計数が注文元のレコード数だけ出現してしまうので、ここではその必要は無い
                    sql.AppendLine("             INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日     = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN");
                    // -- 出力できる商品の最大列数は16（その他(XXX)を入れて17）
                    sql.AppendLine("                 (SELECT TOP({1})");
                    sql.AppendLine("                     SS.企業ＣＤ");
                    sql.AppendLine("                     ,SS.商品ＣＤ");
                    sql.AppendLine("                     ,SS.部署ＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     主要商品リストマスタ AS SS");
                    sql.AppendLine("                     LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = SS.企業ＣＤ AND SM.商品ＣＤ = SS.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     SS.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND SS.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND ISNULL(SS.無効フラグ, 0) = 0");
                    sql.AppendLine("                 ORDER BY");
                    sql.AppendLine("                     CASE WHEN SS.商品グループＣＤ IS NULL THEN 1 ELSE 0 END");
                    sql.AppendLine("                     ,SS.商品グループＣＤ");
                    sql.AppendLine("                     ,SM.弁当区分");
                    sql.AppendLine("                     ,SS.商品ＣＤ");
                    sql.AppendLine("                 ) AS S");
                    sql.AppendLine("                 ON S.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND S.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 AND S.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.売上日 = @売上日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             T.受注得意先ＣＤ");
                    sql.AppendLine("             ,S.商品ＣＤ");
                    sql.AppendLine("         ) AS A ON A.得意先ＣＤ = C.コース得意先ＣＤ        -- コースにぶら下がる得意先");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS D ON D.企業ＣＤ = C.企業ＣＤ AND D.担当者ＣＤ = C.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = C.企業ＣＤ AND B.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine(" WHERE CM.コース区分 = @コース区分");
                    sql.AppendLine(" GROUP BY");
                    sql.AppendLine("     B.部署名,");
                    sql.AppendLine("     B.会社名称,");
                    sql.AppendLine("     B.電話番号,");
                    sql.AppendLine("     B.ＦＡＸ,");
                    sql.AppendLine("     C.コースＣＤ,");
                    sql.AppendLine("     C.コース名,");
                    sql.AppendLine("     D.担当者名,");
                    sql.AppendLine("     A.商品ＣＤ");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     ソート用コースＣＤ,");
                    sql.AppendLine("     ソート用コース順,");
                    sql.AppendLine("     ソート用得意先ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _yokiKaisyuHyo.売上日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _yokiKaisyuHyo.部署ＣＤ;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _yokiKaisyuHyo.コース区分;

                    if (_yokiKaisyuHyo.コースＣＤFrom != null)
                    {
                        cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _yokiKaisyuHyo.コースＣＤFrom;
                    }

                    if (_yokiKaisyuHyo.コースＣＤTo != null)
                    {
                        cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _yokiKaisyuHyo.コースＣＤTo;
                    }

                    cmd.CommandText = string.Format(sql.ToString(), _yokiKaisyuHyo.売上日, PRINT_GET_SYOHINS);

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("容器回収表");
                // 商品名列を作成（最大17列）（不定列ではなく、普通の列として追加する）
                int i;
                for (i = 1 ; i <= PRINT_SYOHIN_COLUMNS ; i++)
                {
                    cldt.Columns.Add(string.Format("商品名{0:00}", i), typeof(string));
                }

                // 固定列を作成
                foreach (DataColumn column in dt.Columns)
                {
                    // 不定列として追加する商品ＣＤと商品数量は除く
                    if (column.ColumnName == "商品ＣＤ" ||
                        column.ColumnName == "商品数量")
                    {
                        continue;
                    }
                    cldt.AddFixedColumn(column);
                }

                // 不定列（を持つデータテーブル）を作成
                DataTable dtSuryo = new DataTable();
                dtSuryo.Columns.Add("数量列名", typeof(string));
                dtSuryo.Columns.Add("商品ＣＤ", typeof(string));
                // 取得した商品名の一覧からレコードを作成
                for (i = 1 ; i <= syohinDt.Rows.Count ; i++)
                {
                    DataRow wkDr = dtSuryo.NewRow();
                    wkDr["数量列名"] = string.Format("数量{0:00}", i);
                    wkDr["商品ＣＤ"] = syohinDt.Rows[i-1]["商品ＣＤ"];
                    dtSuryo.Rows.Add(wkDr);
                }
                // PRINT_SYOHIN_COLUMNSに満たない分を追加する
                for (; i <= PRINT_SYOHIN_COLUMNS ; i++)
                {
                    DataRow wkDr = dtSuryo.NewRow();
                    wkDr["数量列名"] = string.Format("数量{0:00}", i);
                    wkDr["商品ＣＤ"] = string.Format("blank{0:00}", i);
                    dtSuryo.Rows.Add(wkDr);
                }
                cldt.AddColumns(dtSuryo, "数量列名", "商品ＣＤ", typeof(decimal));

                // インポート
                cldt.Import(dt, "商品ＣＤ", "商品数量");

                // 全ての行に対して…
                foreach (DataRow dr in cldt.Rows)
                {
                    // 商品名を埋める
                    for (i = 1 ; i <= syohinDt.Rows.Count ; i++)
                    {
                        string columnName = string.Format("商品名{0:00}", i);
                        string syohinName = (string)syohinDt.Rows[i-1]["商品名略称"];
                        dr[columnName] = syohinName;
                    }
                    for (; i <= PRINT_SYOHIN_COLUMNS ; i++)
                    {
                        string columnName = string.Format("商品名{0:00}", i);
                        dr[columnName] = "";
                    }
                }

                Report303A report303A = new Report303A();
                report303A.DataSource = cldt;
                report303A.Document.Printer.DocumentName = string.Format("容器回収表({0:yyyy-MM-dd})", _yokiKaisyuHyo.売上日);
                report303A.Document.Printer.PrinterName = printerNameA4;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "容器回収表";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report303A);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);

                string code = string.Format("売上日=[{0:yyyy/MM/dd}] コースＣＤFrom=[{1}] コースＣＤTo=[{2}]", _yokiKaisyuHyo.売上日, _yokiKaisyuHyo.コースＣＤFrom, _yokiKaisyuHyo.コースＣＤTo);

                History.Save(this.Text, "容器回収表", code, History.ProcessType.Output);

                log.InfoFormat("容器回収表を印刷しました。{0}", code);
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
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// プレビュー画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formPreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                FormPreview form = (FormPreview)sender;
                form.Dispose();
                form = null;
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
        /// 容器回収表の列として採用する商品名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetSyohinTableForPrint()
        {
            DataTable dt = new DataTable("商品一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    // -- 以下のように並べる
                    // -- ┌───┬───┬───┐
                    // -- │商品Ａ│商品Ｂ│その他│
                    // -- └───┴───┴───┘
                    string sql = "\r\n SELECT" +
                                 "\r\n     0 AS その他区分" +
                                 "\r\n     ,B.商品グループＣＤ" +
                                 "\r\n     ,B.商品ＣＤ AS ソート用商品ＣＤ" +
                                 "\r\n     ,CONVERT(varchar, B.商品ＣＤ) AS 商品ＣＤ" +
                                 "\r\n     ,B.商品名略称" +
                                 "\r\n FROM" +
                                 // -- 出力できる商品の最大列数は16（その他を入れて17）" +
                                 "\r\n     (SELECT TOP({0})" +
                                 "\r\n         A.商品グループＣＤ" +
                                 "\r\n         ,A.商品ＣＤ" +
                                 "\r\n         ,S.商品名略称" +
                                 "\r\n     FROM" +
                                 "\r\n         主要商品リストマスタ AS A" +
                                 "\r\n         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                 "\r\n     WHERE" +
                                 "\r\n         A.企業ＣＤ = @企業ＣＤ" +
                                 "\r\n         AND A.部署ＣＤ = @部署ＣＤ" +
                                 "\r\n         AND ISNULL(A.無効フラグ, 0) = 0" +
                                 "\r\n     ORDER BY" +
                                 "\r\n         CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END" +
                                 "\r\n         ,A.商品グループＣＤ" +
                                 "\r\n         ,S.弁当区分" +
                                 "\r\n         ,A.商品ＣＤ" +
                                 "\r\n     ) AS B" +
                                 "\r\n UNION" +
                                 "\r\n SELECT" +
                                 "\r\n     1 AS その他区分" +
                                 "\r\n     ,-1 AS 商品グループＣＤ" +
                                 "\r\n     ,-1 AS ソート用商品ＣＤ" +
                                 "\r\n     ,'XXX' AS 商品ＣＤ" +
                                 "\r\n     ,'他' AS 商品名略称" +
                                 "\r\n ORDER BY" +
                                 "\r\n     その他区分" +
                                 "\r\n     ,商品グループＣＤ" +
                                 "\r\n     ,ソート用商品ＣＤ" +
                                 "\r\n     ,商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _yokiKaisyuHyo.部署ＣＤ;

                    cmd.CommandText = string.Format(sql, PRINT_GET_SYOHINS);

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("商品一覧を取得しました。");
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
        #endregion
    }
}
