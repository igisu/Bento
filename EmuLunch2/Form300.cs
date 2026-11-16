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
    /// 過不足一覧
    /// </summary>
    public partial class Form300 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SYUKEI_COLUMN         = 0;    // 集計チェックボックス列インデックス
        private const int COURSE_CD_COLUMN      = 1;    // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN    = 2;    // コース名列インデックス
        // これ以降の列は動的に追加される
        private const int SYOHIN_START_COLUMN   = 3;    // 商品(部品)名列開始列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 過不足一覧検索条件
        /// </summary>
        private KafusokuListCriteria _kafusokuListCriteria;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = SYOHIN_START_COLUMN;

        /// <summary>
        /// 列として採用する商品名の一覧（商品ＣＤ、商品名略称）を持つデータテーブル
        /// </summary>
        private DataTable _syohinDt = null;

        /// <summary>
        /// スプレッドのデータソース用のデータテーブル。列の数(=商品)が不定。
        /// </summary>
        private ColumnListDataTable _cldt = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form300()
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
        private void Form300_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

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
                _kafusokuListCriteria = new KafusokuListCriteria();

                kafusokuListBindingSource.DataSource = _kafusokuListCriteria;

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
        private void Form300_Shown(object sender, EventArgs e)
        {
            // コース区分にフォーカス
            cboCourseKbn.Focus();
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form300_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
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
                _kafusokuListCriteria.グループ種別ＣＤ = -1;
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
        /// コース選択ダイアログクローズ（コースＣＤ）
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
                    _kafusokuListCriteria.コースＣＤFrom = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameFrom.Text = course.コース名;
                    _kafusokuListCriteria.コース名From = course.コース名;
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
        /// コース選択ダイアログクローズ（コースＣＤ）
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
                    _kafusokuListCriteria.コースＣＤTo = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameTo.Text = course.コース名;
                    _kafusokuListCriteria.コース名To = course.コース名;
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_kafusokuListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_kafusokuListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _kafusokuListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 配送日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_kafusokuListCriteria.配送日 > warnDate)
                {
                    if (MessageBox.Show(this, "配送日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                //****************************************************************
                // 主要部品リストマスタにある商品名を列として追加し、
                // コースマスタにあるコースを行とする
                // データそのものは注文明細とモバイル_持ち出し入力などから取得する
                //****************************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                DataTable dt = new DataTable("注文明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     0 AS 合計行,");
                    sql.AppendLine("     1 AS 集計対象,");
                    sql.AppendLine("     MAIN.工場フラグ,");
                    sql.AppendLine("     MAIN.コースＣＤ,");
                    sql.AppendLine("     MAIN.コース名,");
                    sql.AppendLine("     MAIN.商品ＣＤ,");
                    sql.AppendLine("     MAIN.商品名略称,");
                    sql.AppendLine("     MCDS.個数 AS 持ち出し数,");
                    sql.AppendLine("     INIT.数量 AS 売上予定,");
                    sql.AppendLine("     TAKE_KOJO.個数 AS 受取数_工場,");
                    sql.AppendLine("     TAKE_IPPAN.個数 AS 受取数_一般,");
                    sql.AppendLine("     GIVE.個数 AS 引渡数,");
                    // -- 持出の個数＋受け取った個数－渡した個数－販売数
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
                    sql.AppendLine("                                         ) T2 ");   //-- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
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
                    sql.AppendLine("             UNION ALL");   // -- SUM()の対象にするので、同一コース、同一得意先、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                    // -- ②－２ 予測と注文の副食
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
                    sql.AppendLine("                                         ) T2 ");   //-- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
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
                    sql.AppendLine("                                         ) T2 ");   //-- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
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
                    sql.AppendLine("                                         ) T2 ");   //-- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
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
                    sql.AppendLine("                                         ) T2 ");   //-- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
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
                    // -- 注文ヘッダ（①：注文ヘッダ.喫食日==売上ヘッダ.売上日の売上
                    // --             ①が存在しない場合は、売上ヘッダ.売上日 = 当日の売上）
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
                    // -- 注文ヘッダ（①：注文ヘッダ.喫食日==売上ヘッダ.売上日の売上
                    // --             ①が存在しない場合は、売上ヘッダ.売上日 = 当日の売上）
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
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     1 = 1");
                    if (_kafusokuListCriteria.コースＣＤFrom != null)
                    {
                        sql.AppendLine("     AND MAIN.コースＣＤ >= @コースＣＤFrom");
                        cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _kafusokuListCriteria.コースＣＤFrom;
                    }
                    if (_kafusokuListCriteria.コースＣＤTo != null)
                    {
                        sql.AppendLine("     AND MAIN.コースＣＤ <= @コースＣＤTo");
                        cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _kafusokuListCriteria.コースＣＤTo;
                    }
                    if (_kafusokuListCriteria.グループ種別ＣＤ != null && _kafusokuListCriteria.グループ種別ＣＤ != -1)
                    {
                        sql.AppendLine("     AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _kafusokuListCriteria.グループ種別ＣＤ;
                    }
                    // -- ⑧合計行
                    sql.AppendLine(" UNION");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     1 AS 合計行,");
                    sql.AppendLine("     0 AS 集計対象,");
                    sql.AppendLine("     NULL AS 工場フラグ,");
                    sql.AppendLine("     NULL AS コースＣＤ,");
                    sql.AppendLine("     '合計' AS コース名,");
                    sql.AppendLine("     -1 AS 商品ＣＤ,");
                    sql.AppendLine("     '' As 商品名略称,");
                    sql.AppendLine("     -1 AS 持ち出し数,");
                    sql.AppendLine("     -1 AS  売上予定,");
                    sql.AppendLine("     -1 AS  受取数_工場,");
                    sql.AppendLine("     -1 AS  受取数_一般,");
                    sql.AppendLine("     -1 AS  引渡数,");
                    sql.AppendLine("     -1 AS  残数,");
                    sql.AppendLine("     -1 AS グループ種別ＣＤ,");
                    sql.AppendLine("     -1 AS グループＳＥＱ");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _kafusokuListCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _kafusokuListCriteria.コース区分;
                    cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _kafusokuListCriteria.配送日;
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分その他", SqlDbType.Int).Value = (int)Consts.BentoKubun.Other;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("過不足一覧リストを検索しました。");
                }

                // 列を指定したデータテーブルを作成
                _cldt = new ColumnListDataTable("過不足一覧");
                // 商品名列を作成
                _cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                _cldt.AddFixedColumn(dt.Columns["集計対象"]);
                _cldt.AddFixedColumn(dt.Columns["工場フラグ"]);
                _cldt.AddFixedColumn(dt.Columns["コースＣＤ"]);
                _cldt.AddFixedColumn(dt.Columns["コース名"]);
                // インポート
                _cldt.Import(dt, "商品ＣＤ", "残数");

                DataSet ds = new DataSet();
                ds.Tables.Add(_cldt);

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "過不足一覧";

                fpSpread1_Sheet1.BindDataColumn(SYUKEI_COLUMN,      "集計対象");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,   "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN, "コース名");

                // 商品名を列名としてバインド
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (string columnName in _cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

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
                foreach (DataRow dr in _cldt.Rows)
                {
                    if (((dr["工場フラグ"] as bool?) ?? false) == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = Color.LightCyan;
                    }
                    rowIndex++;
                }

                // 最終の対象商品数行を固定行にする
                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = Color.LightYellow;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    CalcTotal();
                }

                if (fpSpread1_Sheet1.Rows.Count > 1)  // 合計行のみの場合はフォーカスしない
                {
                    fpSpread1.Focus();
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
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
        /// Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // チェックボックスのクリックと同時に再集計したいので、DatatableのColumnChangedイベントではなく、このイベントで処理する
                if (e.Column == SYUKEI_COLUMN)
                {
                    // 集計処理では集計対象の行を対象に集計しているが、
                    // デフォルトでは別の行・列・セルへ移動しないとスプレッドからデータソースへの値が反映されない。
                    // そのため、別の行・列・セルへの移動を待たずに、カレントレコードの集計対象列の値を強制的に変える
                    DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                    if ((bool)fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, fpSpread1_Sheet1.ActiveColumnIndex].Value)
                    {
                        dr["集計対象"] = 1;
                    }
                    else
                    {
                        dr["集計対象"] = 0;
                    }

                    // 集計処理
                    CalcTotal();
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
        /// Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)bindingSource1.Current).Row;
            dr.EndEdit();
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

        /// <summary>
        /// 表示倍率スクロールバー マウスオーバー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sclScale_MouseEnter(object sender, EventArgs e)
        {
            // いったんマウスで表示倍率を変えると（このスクロールバーをアクティブになるので）
            // マウスホイールやカーソルキーでスクロールバーがスクロールする（表示倍率が変わってしまう）のを防ぐために、
            // アクティブコントロールを保持しておいて、スクロール完了時に復元する。
            // ※Enterイベントだとタイミングが遅すぎる（アクティブコントロールがこのコントロールになっている）
            Control activeControl = this.ActiveControl;
            sclScale.Tag = activeControl;
        }

        /// <summary>
        /// 表示倍率スクロールバー スクロール
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sclScale_Scroll(object sender, ScrollEventArgs e)
        {
            fpSpread1_Sheet1.ZoomFactor = (float)sclScale.Value / 100f;
            lblScale.Text = string.Format("{0}%", sclScale.Value);

            // 保持しておいたコントロールにフォーカスを移す
            Control activeControl = (Control)sclScale.Tag;
            activeControl.Select();
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 列として採用する商品名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetSyohinTable()
        {
            DataTable dt = new DataTable("商品一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.商品ＣＤ," +
                                      "\r\n     S.商品名略称" +
                                      "\r\n FROM" +
                                      "\r\n     主要部品リストマスタ AS A" +
                                      "\r\n     INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _kafusokuListCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("商品一覧を取得しました。");
                }

                return dt;
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
        /// 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
        /// </summary>
        /// <param name="dt"></param>
        private void SetSpreadColumn(DataTable syohinDt, string labelColumnName)
        {
            try
            {
                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN;

                // 今回追加する商品名の列数をセット
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN + syohinDt.Rows.Count;

                // 固定列に続いて、商品名の列の属性をセット
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (DataRow dr in syohinDt.Rows)
                {
                    FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                    numberCellType.DecimalPlaces = 0;
                    numberCellType.EnableSubEditor = false;
                    numberCellType.MaximumValue = 99999D;
                    numberCellType.MinimumValue = -99999D;
                    numberCellType.NegativeRed = true;
                    numberCellType.Static = true;
                    numberCellType.ReadOnly = true;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).Label = dr[labelColumnName].ToString();
                    fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
                    columnIndex++;
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

        /// <summary>
        /// 集計処理
        /// </summary>
        private void CalcTotal()
        {
            try
            {
                DataRow drTotal = _cldt.Rows[_cldt.Rows.Count - 1];
                foreach (DataColumn dc in _cldt.Columns)
                {
                    switch (dc.ColumnName)
                    {
                        case "集計対象":        // 固定列は集計対象外
                        case "工場フラグ":
                        case "コースＣＤ":
                        case "コース名":
                            break;

                        default:
                            // 商品列ごとに、集計対象の行について、合計を算出する
                            // 列名(=商品名略称)に半角括弧があってもいいように角括弧で括る
                            // さらに、角括弧があってもいいようにエスケープする
                            string columnName = dc.ColumnName.Replace(@"\", @"\\");
                            columnName = columnName.Replace(@"]", @"\]");
                            object value = _cldt.Compute(string.Format("SUM([{0}])", columnName), "集計対象 <> 0");
                            drTotal[dc.ColumnName] = value;
                            break;
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
