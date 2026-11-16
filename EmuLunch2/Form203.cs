using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using Emu.Bento.WebAPI;

namespace Emu.Bento
{
    /// <summary>
    /// コース注文一覧
    /// </summary>
    public partial class Form203 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_COLUMN = 0;     // 得意先列インデックス
        private const int TYPE_COLUMN = 1;          // タイプ列インデックス
        // これ以降の列は動的に追加される
        private const int SYOHIN_START_COLUMN = 2;  // 商品名列開始列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// コース注文一覧検索条件
        /// </summary>
        private CourseChumonListCriteria _chumonListCriteria;

        /// <summary>
        /// コース注文一覧検索条件検索実行時スナップショット
        /// </summary>
        private CourseChumonListCriteria _chumonListCriteriaSnapShot;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = SYOHIN_START_COLUMN;

        /// <summary>
        /// 列として採用する商品名の一覧（商品ＣＤ、商品名略称）を持つデータテーブル
        /// </summary>
        private DataTable _syohinDt = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form203()
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
        private void Form203_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                // 権限によりボタンを制御
                btnUriageHanei.Enabled = this.HasAuthEdit;

                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN;

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread1.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 先頭2列を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = 2;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _chumonListCriteria = new CourseChumonListCriteria();
                _chumonListCriteria.喫食日 = DateTime.Today;
                chumonListCriteriaBindingSource.DataSource = _chumonListCriteria;

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
        private void Form203_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F8:                       // 注文入力ボタン押下
                    btnChumon.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// 配送日にするチェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkHaisobi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                lblChumonbi.Text = (chkHaisobi.Checked ? "配送日" : "注文日");
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
        /// コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCD.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902((int)cboBusyo.SelectedValue);
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCD_FormClosed);
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
        private void Form902CourseCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtCourseCD, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCD.Text = course.コースＣＤ.ToString();
                    _chumonListCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;

                    _chumonListCriteria.部署ＣＤ = course.部署ＣＤ;
                    ReadBinding(cboBusyo);      // コントロールに反映
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
        /// コースＣＤ検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // コースによって決まった部署ＣＤを表示
                cboBusyo.SelectedValue = _chumonListCriteria.部署ＣＤ ?? -1;
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
                if (_chumonListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_chumonListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _chumonListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_chumonListCriteria.喫食日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        _chumonListCriteriaSnapShot = null;
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                //***************************************************
                // 主要商品リストマスタにある商品名を列として追加（それ以外の商品は「その他」列にまとめる）し、
                // コーステーブルにある得意先を行とする
                // データそのものは注文明細から取得する
                //***************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_syohinDt, "商品名略称", "注文売上区分", 1);

                // データ本体を取得
                DataTable dt = new DataTable("コース注文一覧データ");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    string sql = "";
                    // -- 商品ＣＤのアンダーバー以降は
                    // -- C:注文 U:売上
                    // -- 0:個数 1:金額
                    if (_chumonListCriteria.コースＣＤ != null)
                    {
                        sql = // --- 注文現金個数
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     C.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.個数 AS 値," +
                              "\r\n     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順," +
                              "\r\n     CO.コース順" +
                              "\r\n FROM" +
                              // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CT.企業ＣＤ" +
                              "\r\n         ,CT.コースＣＤ" +
                              "\r\n         ,CT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コーステーブル AS CT" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @喫食日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RT.企業ＣＤ" +
                              "\r\n         ,RT.コースＣＤ" +
                              "\r\n         ,RT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RT.実施日 = @喫食日" +
                              // -- 注文そのものも対象にする
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         企業ＣＤ" +
                              "\r\n         ,コースＣＤ" +
                              "\r\n         ,得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND {0} = @喫食日" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ," +
                              "\r\n             SUM(M.商品数量) AS 個数" +
                              "\r\n         FROM" +
                              "\r\n             注文明細 AS M" +
                              "\r\n             INNER JOIN 注文ヘッダ AS H" +
                              "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n                 AND H.注文区分   = M.注文区分" +
                              "\r\n                 AND H.喫食日     = M.喫食日" +
                              "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n             LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n                 ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n                 AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n                 AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n                 AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n             H.注文区分 = @注文区分注文 AND" +
                              "\r\n             H.{0} = @喫食日 AND" +
                              "\r\n             H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n             M.売掛現金区分 = @売掛現金区分現金" +
                              "\r\n         GROUP BY" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             A.商品ＣＤ" +
                              "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                              // -- コース順取得用
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             CT.企業ＣＤ" +
                              "\r\n             ,CT.コースＣＤ" +
                              "\r\n             ,CT.得意先ＣＤ" +
                              "\r\n             ,CT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             コーステーブル AS CT" +
                              "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n             LEFT JOIN" +
                              "\r\n                 (SELECT" +
                              "\r\n                     コースＣＤ" +
                              "\r\n                 FROM" +
                              "\r\n                     臨時コースマスタ" +
                              "\r\n                 WHERE" +
                              "\r\n                     企業ＣＤ = @企業ＣＤ" +
                              "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                     AND 実施日 = @喫食日" +
                              "\r\n                 ) AS RM" +
                              "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n             AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n         UNION" +
                              "\r\n         SELECT" +
                              "\r\n             RT.企業ＣＤ" +
                              "\r\n             ,RT.コースＣＤ" +
                              "\r\n             ,RT.得意先ＣＤ" +
                              "\r\n             ,RT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             臨時コーステーブル AS RT" +
                              "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n             AND RT.実施日 = @喫食日" +
                              "\r\n         ) AS CO ON C.コースＣＤ = CO.コースＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     C.コースＣＤ = @コースＣＤ" +
                              "\r\n UNION" +
                              // --- 注文現金金額
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     C.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.金額 AS 値," +
                              "\r\n     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順," +
                              "\r\n     CO.コース順" +
                              "\r\n FROM" +
                              // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CT.企業ＣＤ" +
                              "\r\n         ,CT.コースＣＤ" +
                              "\r\n         ,CT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コーステーブル AS CT" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @喫食日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RT.企業ＣＤ" +
                              "\r\n         ,RT.コースＣＤ" +
                              "\r\n         ,RT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RT.実施日 = @喫食日" +
                              // -- 注文そのものも対象にする
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         企業ＣＤ" +
                              "\r\n         ,コースＣＤ" +
                              "\r\n         ,得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND {0} = @喫食日" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ," +
                              "\r\n             SUM(M.商品金額合計税込) AS 金額" +
                              "\r\n         FROM" +
                              "\r\n             注文明細 AS M" +
                              "\r\n             INNER JOIN 注文ヘッダ AS H" +
                              "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n                 AND H.注文区分   = M.注文区分" +
                              "\r\n                 AND H.喫食日     = M.喫食日" +
                              "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n             LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n                 ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n                 AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n                 AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n                 AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n             H.注文区分 = @注文区分注文 AND" +
                              "\r\n             H.{0} = @喫食日 AND" +
                              "\r\n             H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n             M.売掛現金区分 = @売掛現金区分現金" +
                              "\r\n         GROUP BY" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             A.商品ＣＤ" +
                              "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                              // -- コース順取得用
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             CT.企業ＣＤ" +
                              "\r\n             ,CT.コースＣＤ" +
                              "\r\n             ,CT.得意先ＣＤ" +
                              "\r\n             ,CT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             コーステーブル AS CT" +
                              "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n             LEFT JOIN" +
                              "\r\n                 (SELECT" +
                              "\r\n                     コースＣＤ" +
                              "\r\n                 FROM" +
                              "\r\n                     臨時コースマスタ" +
                              "\r\n                 WHERE" +
                              "\r\n                     企業ＣＤ = @企業ＣＤ" +
                              "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                     AND 実施日 = @喫食日" +
                              "\r\n                 ) AS RM" +
                              "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n             AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n         UNION" +
                              "\r\n         SELECT" +
                              "\r\n             RT.企業ＣＤ" +
                              "\r\n             ,RT.コースＣＤ" +
                              "\r\n             ,RT.得意先ＣＤ" +
                              "\r\n             ,RT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             臨時コーステーブル AS RT" +
                              "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n             AND RT.実施日 = @喫食日" +
                              "\r\n         ) AS CO ON C.コースＣＤ = CO.コースＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     C.コースＣＤ = @コースＣＤ" +
                              "\r\n UNION" +
                              // --- 注文売掛個数
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     C.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.個数 AS 値," +
                              "\r\n     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順," +
                              "\r\n     CO.コース順" +
                              "\r\n FROM" +
                              // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CT.企業ＣＤ" +
                              "\r\n         ,CT.コースＣＤ" +
                              "\r\n         ,CT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コーステーブル AS CT" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @喫食日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RT.企業ＣＤ" +
                              "\r\n         ,RT.コースＣＤ" +
                              "\r\n         ,RT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RT.実施日 = @喫食日" +
                              // -- 注文そのものも対象にする
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         企業ＣＤ" +
                              "\r\n         ,コースＣＤ" +
                              "\r\n         ,得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND {0} = @喫食日" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ," +
                              "\r\n             SUM(M.商品数量) AS 個数" +
                              "\r\n         FROM" +
                              "\r\n             注文明細 AS M" +
                              "\r\n             INNER JOIN 注文ヘッダ AS H" +
                              "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n                 AND H.注文区分   = M.注文区分" +
                              "\r\n                 AND H.喫食日     = M.喫食日" +
                              "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n             LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n                 ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n                 AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n                 AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n                 AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n             H.注文区分 = @注文区分注文 AND" +
                              "\r\n             H.{0} = @喫食日 AND" +
                              "\r\n             H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n             M.売掛現金区分 <> @売掛現金区分現金" +      // -- 売掛、LINE決済
                              "\r\n         GROUP BY" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             A.商品ＣＤ" +
                              "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                              // -- コース順取得用
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             CT.企業ＣＤ" +
                              "\r\n             ,CT.コースＣＤ" +
                              "\r\n             ,CT.得意先ＣＤ" +
                              "\r\n             ,CT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             コーステーブル AS CT" +
                              "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n             LEFT JOIN" +
                              "\r\n                 (SELECT" +
                              "\r\n                     コースＣＤ" +
                              "\r\n                 FROM" +
                              "\r\n                     臨時コースマスタ" +
                              "\r\n                 WHERE" +
                              "\r\n                     企業ＣＤ = @企業ＣＤ" +
                              "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                     AND 実施日 = @喫食日" +
                              "\r\n                 ) AS RM" +
                              "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n             AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n         UNION" +
                              "\r\n         SELECT" +
                              "\r\n             RT.企業ＣＤ" +
                              "\r\n             ,RT.コースＣＤ" +
                              "\r\n             ,RT.得意先ＣＤ" +
                              "\r\n             ,RT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             臨時コーステーブル AS RT" +
                              "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n             AND RT.実施日 = @喫食日" +
                              "\r\n         ) AS CO ON C.コースＣＤ = CO.コースＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     C.コースＣＤ = @コースＣＤ" +
                              "\r\n UNION" +
                              // --- 注文売掛金額
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     C.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.金額 AS 値," +
                              "\r\n     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順," +
                              "\r\n     CO.コース順" +
                              "\r\n FROM" +
                              // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CT.企業ＣＤ" +
                              "\r\n         ,CT.コースＣＤ" +
                              "\r\n         ,CT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コーステーブル AS CT" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @喫食日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RT.企業ＣＤ" +
                              "\r\n         ,RT.コースＣＤ" +
                              "\r\n         ,RT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RT.実施日 = @喫食日" +
                              // -- 注文そのものも対象にする
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         企業ＣＤ" +
                              "\r\n         ,コースＣＤ" +
                              "\r\n         ,得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND {0} = @喫食日" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ," +
                              "\r\n             SUM(M.商品金額合計税込) AS 金額" +
                              "\r\n         FROM" +
                              "\r\n             注文明細 AS M" +
                              "\r\n             INNER JOIN 注文ヘッダ AS H" +
                              "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n                 AND H.注文区分   = M.注文区分" +
                              "\r\n                 AND H.喫食日     = M.喫食日" +
                              "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n             LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n                 ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n                 AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n                 AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n                 AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n             H.注文区分 = @注文区分注文 AND" +
                              "\r\n             H.{0} = @喫食日 AND" +
                              "\r\n             H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n             M.売掛現金区分 <> @売掛現金区分現金" +      // -- 売掛、LINE決済
                              "\r\n         GROUP BY" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             A.商品ＣＤ" +
                              "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                              // -- コース順取得用
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             CT.企業ＣＤ" +
                              "\r\n             ,CT.コースＣＤ" +
                              "\r\n             ,CT.得意先ＣＤ" +
                              "\r\n             ,CT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             コーステーブル AS CT" +
                              "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n             LEFT JOIN" +
                              "\r\n                 (SELECT" +
                              "\r\n                     コースＣＤ" +
                              "\r\n                 FROM" +
                              "\r\n                     臨時コースマスタ" +
                              "\r\n                 WHERE" +
                              "\r\n                     企業ＣＤ = @企業ＣＤ" +
                              "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                     AND 実施日 = @喫食日" +
                              "\r\n                 ) AS RM" +
                              "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n             AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n         UNION" +
                              "\r\n         SELECT" +
                              "\r\n             RT.企業ＣＤ" +
                              "\r\n             ,RT.コースＣＤ" +
                              "\r\n             ,RT.得意先ＣＤ" +
                              "\r\n             ,RT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             臨時コーステーブル AS RT" +
                              "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n             AND RT.実施日 = @喫食日" +
                              "\r\n         ) AS CO ON C.コースＣＤ = CO.コースＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     C.コースＣＤ = @コースＣＤ" +
                              "\r\n UNION" +
                              // --- 売上現金個数
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     C.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.個数 AS 値," +
                              "\r\n     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順," +
                              "\r\n     CO.コース順" +
                              "\r\n FROM" +
                              // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CT.企業ＣＤ" +
                              "\r\n         ,CT.コースＣＤ" +
                              "\r\n         ,CT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コーステーブル AS CT" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @喫食日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RT.企業ＣＤ" +
                              "\r\n         ,RT.コースＣＤ" +
                              "\r\n         ,RT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RT.実施日 = @喫食日" +
                              // -- 注文そのものも対象にする
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         企業ＣＤ" +
                              "\r\n         ,コースＣＤ" +
                              "\r\n         ,得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND {0} = @喫食日" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ," +
                              "\r\n             SUM(M.商品数量) AS 個数" +
                              "\r\n         FROM" +
                              "\r\n             売上明細 AS M" +
                              "\r\n             INNER JOIN 売上ヘッダ AS H" +
                              "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n                 AND H.売上日     = M.売上日" +
                              "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n             LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n                 ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n                 AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n                 AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n                 AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             M.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n             M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND" +      // -- 現金、チケット、バークレー、ボーナスは現金と見なす
                              "\r\n             M.売上日 = @喫食日 AND" +
                              "\r\n             M.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         GROUP BY" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             A.商品ＣＤ" +
                              "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                              // -- コース順取得用
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             CT.企業ＣＤ" +
                              "\r\n             ,CT.コースＣＤ" +
                              "\r\n             ,CT.得意先ＣＤ" +
                              "\r\n             ,CT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             コーステーブル AS CT" +
                              "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n             LEFT JOIN" +
                              "\r\n                 (SELECT" +
                              "\r\n                     コースＣＤ" +
                              "\r\n                 FROM" +
                              "\r\n                     臨時コースマスタ" +
                              "\r\n                 WHERE" +
                              "\r\n                     企業ＣＤ = @企業ＣＤ" +
                              "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                     AND 実施日 = @喫食日" +
                              "\r\n                 ) AS RM" +
                              "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n             AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n         UNION" +
                              "\r\n         SELECT" +
                              "\r\n             RT.企業ＣＤ" +
                              "\r\n             ,RT.コースＣＤ" +
                              "\r\n             ,RT.得意先ＣＤ" +
                              "\r\n             ,RT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             臨時コーステーブル AS RT" +
                              "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n             AND RT.実施日 = @喫食日" +
                              "\r\n         ) AS CO ON C.コースＣＤ = CO.コースＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     C.コースＣＤ = @コースＣＤ" +
                              "\r\n UNION" +
                              // --- 売上現金金額
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     C.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.金額 AS 値," +
                              "\r\n     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順," +
                              "\r\n     CO.コース順" +
                              "\r\n FROM" +
                              // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CT.企業ＣＤ" +
                              "\r\n         ,CT.コースＣＤ" +
                              "\r\n         ,CT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コーステーブル AS CT" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @喫食日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RT.企業ＣＤ" +
                              "\r\n         ,RT.コースＣＤ" +
                              "\r\n         ,RT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RT.実施日 = @喫食日" +
                              // -- 注文そのものも対象にする
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         企業ＣＤ" +
                              "\r\n         ,コースＣＤ" +
                              "\r\n         ,得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND {0} = @喫食日" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ," +
                              "\r\n             SUM(M.商品金額合計税込) AS 金額" +
                              "\r\n         FROM" +
                              "\r\n             売上明細 AS M" +
                              "\r\n             INNER JOIN 売上ヘッダ AS H" +
                              "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n                 AND H.売上日     = M.売上日" +
                              "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n             LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n                 ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n                 AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n                 AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n                 AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             M.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n             M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND" +      // -- 現金、チケット、バークレー、ボーナスは現金と見なす
                              "\r\n             M.売上日 = @喫食日 AND" +
                              "\r\n             M.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         GROUP BY" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             A.商品ＣＤ" +
                              "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                              // -- コース順取得用
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             CT.企業ＣＤ" +
                              "\r\n             ,CT.コースＣＤ" +
                              "\r\n             ,CT.得意先ＣＤ" +
                              "\r\n             ,CT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             コーステーブル AS CT" +
                              "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n             LEFT JOIN" +
                              "\r\n                 (SELECT" +
                              "\r\n                     コースＣＤ" +
                              "\r\n                 FROM" +
                              "\r\n                     臨時コースマスタ" +
                              "\r\n                 WHERE" +
                              "\r\n                     企業ＣＤ = @企業ＣＤ" +
                              "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                     AND 実施日 = @喫食日" +
                              "\r\n                 ) AS RM" +
                              "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n             AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n         UNION" +
                              "\r\n         SELECT" +
                              "\r\n             RT.企業ＣＤ" +
                              "\r\n             ,RT.コースＣＤ" +
                              "\r\n             ,RT.得意先ＣＤ" +
                              "\r\n             ,RT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             臨時コーステーブル AS RT" +
                              "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n             AND RT.実施日 = @喫食日" +
                              "\r\n         ) AS CO ON C.コースＣＤ = CO.コースＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     C.コースＣＤ = @コースＣＤ" +
                              "\r\n UNION" +
                              // --- 売上売掛個数
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     C.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.個数 AS 値," +
                              "\r\n     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順," +
                              "\r\n     CO.コース順" +
                              "\r\n FROM" +
                              // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CT.企業ＣＤ" +
                              "\r\n         ,CT.コースＣＤ" +
                              "\r\n         ,CT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コーステーブル AS CT" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @喫食日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RT.企業ＣＤ" +
                              "\r\n         ,RT.コースＣＤ" +
                              "\r\n         ,RT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RT.実施日 = @喫食日" +
                              // -- 注文そのものも対象にする
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         企業ＣＤ" +
                              "\r\n         ,コースＣＤ" +
                              "\r\n         ,得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND {0} = @喫食日" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ," +
                              "\r\n             SUM(M.商品数量) AS 個数" +
                              "\r\n         FROM" +
                              "\r\n             売上明細 AS M" +
                              "\r\n             INNER JOIN 売上ヘッダ AS H" +
                              "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n                 AND H.売上日     = M.売上日" +
                              "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n             LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n                 ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n                 AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n                 AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n                 AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             M.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n             M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND" +     // -- LINE決済は売掛と見なす
                              "\r\n             M.売上日 = @喫食日 AND" +
                              "\r\n             M.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         GROUP BY" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             A.商品ＣＤ" +
                              "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                              // -- コース順取得用
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             CT.企業ＣＤ" +
                              "\r\n             ,CT.コースＣＤ" +
                              "\r\n             ,CT.得意先ＣＤ" +
                              "\r\n             ,CT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             コーステーブル AS CT" +
                              "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n             LEFT JOIN" +
                              "\r\n                 (SELECT" +
                              "\r\n                     コースＣＤ" +
                              "\r\n                 FROM" +
                              "\r\n                     臨時コースマスタ" +
                              "\r\n                 WHERE" +
                              "\r\n                     企業ＣＤ = @企業ＣＤ" +
                              "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                     AND 実施日 = @喫食日" +
                              "\r\n                 ) AS RM" +
                              "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n             AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n         UNION" +
                              "\r\n         SELECT" +
                              "\r\n             RT.企業ＣＤ" +
                              "\r\n             ,RT.コースＣＤ" +
                              "\r\n             ,RT.得意先ＣＤ" +
                              "\r\n             ,RT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             臨時コーステーブル AS RT" +
                              "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n             AND RT.実施日 = @喫食日" +
                              "\r\n         ) AS CO ON C.コースＣＤ = CO.コースＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     C.コースＣＤ = @コースＣＤ" +
                              "\r\n UNION" +
                              // --- 売上売掛金額
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     C.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, C.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.金額 AS 値," +
                              "\r\n     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順," +
                              "\r\n     CO.コース順" +
                              "\r\n FROM" +
                              // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CT.企業ＣＤ" +
                              "\r\n         ,CT.コースＣＤ" +
                              "\r\n         ,CT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コーステーブル AS CT" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @喫食日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RT.企業ＣＤ" +
                              "\r\n         ,RT.コースＣＤ" +
                              "\r\n         ,RT.得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RT.実施日 = @喫食日" +
                              // -- 注文そのものも対象にする
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         企業ＣＤ" +
                              "\r\n         ,コースＣＤ" +
                              "\r\n         ,得意先ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND {0} = @喫食日" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ," +
                              "\r\n             SUM(M.商品金額合計税込) AS 金額" +
                              "\r\n         FROM" +
                              "\r\n             売上明細 AS M" +
                              "\r\n             INNER JOIN 売上ヘッダ AS H" +
                              "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n                 AND H.売上日     = M.売上日" +
                              "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n             LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n                 ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n                 AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n                 AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n                 AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             M.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n             M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND" +     // -- LINE決済は売掛と見なす
                              "\r\n             M.売上日 = @喫食日 AND" +
                              "\r\n             M.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         GROUP BY" +
                              "\r\n             H.コースＣＤ," +
                              "\r\n             M.得意先ＣＤ," +
                              "\r\n             A.商品ＣＤ" +
                              "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                              // -- コース順取得用
                              "\r\n     LEFT JOIN (" +
                              "\r\n         SELECT" +
                              "\r\n             CT.企業ＣＤ" +
                              "\r\n             ,CT.コースＣＤ" +
                              "\r\n             ,CT.得意先ＣＤ" +
                              "\r\n             ,CT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             コーステーブル AS CT" +
                              "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n             LEFT JOIN" +
                              "\r\n                 (SELECT" +
                              "\r\n                     コースＣＤ" +
                              "\r\n                 FROM" +
                              "\r\n                     臨時コースマスタ" +
                              "\r\n                 WHERE" +
                              "\r\n                     企業ＣＤ = @企業ＣＤ" +
                              "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                     AND 実施日 = @喫食日" +
                              "\r\n                 ) AS RM" +
                              "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                              "\r\n             AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n         UNION" +
                              "\r\n         SELECT" +
                              "\r\n             RT.企業ＣＤ" +
                              "\r\n             ,RT.コースＣＤ" +
                              "\r\n             ,RT.得意先ＣＤ" +
                              "\r\n             ,RT.コース順" +
                              "\r\n         FROM" +
                              "\r\n             臨時コーステーブル AS RT" +
                              "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n         WHERE" +
                              "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n             AND RT.実施日 = @喫食日" +
                              "\r\n         ) AS CO ON C.コースＣＤ = CO.コースＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     C.コースＣＤ = @コースＣＤ" +
                              "\r\n UNION" +
                              // --- 現金合計
                              "\r\n SELECT" +
                              "\r\n     1 AS 明細合計区分," +
                              "\r\n     -1 AS 得意先ＣＤ," +
                              "\r\n     NULL AS 得意先名," +
                              "\r\n     '合計' AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     NULL AS 商品ＣＤ," +
                              "\r\n     NULL AS 値," +
                              "\r\n     0 AS コースに含む順," +
                              "\r\n     0 AS コース順" +
                              "\r\n FROM" +
                              "\r\n     V_売掛現金区分 AS VT" +
                              "\r\n WHERE" +
                              "\r\n     VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n UNION" +
                              // --- 売掛合計
                              "\r\n SELECT" +
                              "\r\n     1 AS 明細合計区分," +
                              "\r\n     -1 AS 得意先ＣＤ," +
                              "\r\n     NULL AS 得意先名," +
                              "\r\n     '合計' AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     NULL AS 商品ＣＤ," +
                              "\r\n     NULL AS 値," +
                              "\r\n     0 AS コースに含む順," +
                              "\r\n     0 AS コース順" +
                              "\r\n FROM" +
                              "\r\n     V_売掛現金区分 AS VT" +
                              "\r\n WHERE" +
                              "\r\n     VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n ORDER BY" +
                              "\r\n     明細合計区分," +
                              "\r\n     コースに含む順," +
                              "\r\n     コース順," +
                              "\r\n     得意先ＣＤ," +
                              "\r\n     売掛現金区分," +
                              "\r\n     商品ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _chumonListCriteria.喫食日;
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _chumonListCriteria.コースＣＤ;
                        cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                        cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                        cmd.Parameters.Add("@売掛現金区分売掛", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Urikake;
                        cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                        cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;
                    }
                    else
                    {
                        // コースが未入力の場合はコーステーブルは見ない
                        sql = // --- 注文現金個数
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     MAIN.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.個数 AS 値" +
                              "\r\n FROM" +
                              // -- 駆動表（注文明細）
                              "\r\n     (SELECT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.注文区分 = @注文区分注文 AND" +
                              "\r\n         H.{0} = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     ) AS MAIN" +
                              "\r\n     LEFT JOIN" +
                              // -- 注文明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
                              "\r\n     (SELECT" +
                              "\r\n         H.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ" +
                              "\r\n         ,SUM(M.商品数量) AS 個数" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.注文区分 = @注文区分注文 AND" +
                              "\r\n         H.{0} = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL AND" +
                              "\r\n         M.売掛現金区分 = @売掛現金区分現金" +
                              "\r\n     GROUP BY" +
                              "\r\n         H.得意先ＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS A" +
                              "\r\n         ON MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                              "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n UNION" +
                              // --- 注文現金金額
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     MAIN.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.金額 AS 値" +
                              "\r\n FROM" +
                              // -- 駆動表（注文明細）
                              "\r\n     (SELECT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.注文区分 = @注文区分注文 AND" +
                              "\r\n         H.{0} = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     ) AS MAIN" +
                              "\r\n     LEFT JOIN" +
                              // -- 注文明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
                              "\r\n     (SELECT" +
                              "\r\n         H.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ" +
                              "\r\n         ,SUM(M.商品金額合計税込) AS 金額" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.注文区分 = @注文区分注文 AND" +
                              "\r\n         H.{0} = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL AND" +
                              "\r\n         M.売掛現金区分 = @売掛現金区分現金" +
                              "\r\n     GROUP BY" +
                              "\r\n         H.得意先ＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS A" +
                              "\r\n         ON MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                              "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n UNION" +
                              // --- 注文売掛個数
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     MAIN.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.個数 AS 値" +
                              "\r\n FROM" +
                              // -- 駆動表（注文明細）
                              "\r\n     (SELECT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.注文区分 = @注文区分注文 AND" +
                              "\r\n         H.{0} = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     ) AS MAIN" +
                              "\r\n     LEFT JOIN" +
                              // -- 注文明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
                              "\r\n     (SELECT" +
                              "\r\n         H.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C0' AS 商品ＣＤ" +
                              "\r\n         ,SUM(M.商品数量) AS 個数" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.注文区分 = @注文区分注文 AND" +
                              "\r\n         H.{0} = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL AND" +
                              "\r\n         M.売掛現金区分 <> @売掛現金区分現金" +      // -- 売掛、LINE決済
                              "\r\n     GROUP BY" +
                              "\r\n         H.得意先ＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS A" +
                              "\r\n         ON MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                              "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n UNION" +
                              // --- 注文売掛金額
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     MAIN.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.金額 AS 値" +
                              "\r\n FROM" +
                              // -- 駆動表（注文明細）
                              "\r\n     (SELECT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.注文区分 = @注文区分注文 AND" +
                              "\r\n         H.{0} = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     ) AS MAIN" +
                              "\r\n     LEFT JOIN" +
                              // -- 注文明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
                              "\r\n     (SELECT" +
                              "\r\n         H.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_C1' AS 商品ＣＤ" +
                              "\r\n         ,SUM(M.商品金額合計税込) AS 金額" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.注文区分 = @注文区分注文 AND" +
                              "\r\n         H.{0} = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL AND" +
                              "\r\n         M.売掛現金区分 <> @売掛現金区分現金" +      // -- 売掛、LINE決済
                              "\r\n     GROUP BY" +
                              "\r\n         H.得意先ＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS A" +
                              "\r\n         ON MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                              "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n UNION" +
                              // --- 売上現金個数
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     MAIN.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.個数 AS 値" +
                              "\r\n FROM" +
                              // -- 駆動表（売上明細）
                              "\r\n     (SELECT DISTINCT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         売上明細 AS M" +
                              "\r\n         INNER JOIN 売上ヘッダ AS H" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.売上日     = M.売上日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.売上日 = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     ) AS MAIN" +
                              "\r\n     LEFT JOIN" +
                              // -- 売上明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
                              "\r\n     (SELECT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ" +
                              "\r\n         ,SUM(M.商品数量) AS 個数" +
                              "\r\n     FROM" +
                              "\r\n         売上明細 AS M" +
                              "\r\n         INNER JOIN 売上ヘッダ AS H" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.売上日     = M.売上日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND" +      // -- 現金、チケット、バークレー、ボーナスは現金と見なす
                              "\r\n         M.売上日 = @喫食日 AND" +
                              "\r\n         M.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     GROUP BY" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS A" +
                              "\r\n         ON MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                              "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n UNION" +
                              // --- 売上現金金額
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     MAIN.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.金額 AS 値" +
                              "\r\n FROM" +
                              // -- 駆動表（売上明細）
                              "\r\n     (SELECT DISTINCT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         売上明細 AS M" +
                              "\r\n         INNER JOIN 売上ヘッダ AS H" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.売上日     = M.売上日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.売上日 = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     ) AS MAIN" +
                              "\r\n     LEFT JOIN" +
                              // -- 売上明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
                              "\r\n     (SELECT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ" +
                              "\r\n         ,SUM(M.商品金額合計税込) AS 金額" +
                              "\r\n     FROM" +
                              "\r\n         売上明細 AS M" +
                              "\r\n         INNER JOIN 売上ヘッダ AS H" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.売上日     = M.売上日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND" +      // -- 現金、チケット、バークレー、ボーナスは現金と見なす
                              "\r\n         M.売上日 = @喫食日 AND" +
                              "\r\n         M.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     GROUP BY" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS A" +
                              "\r\n         ON MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                              "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n UNION" +
                              // --- 売上売掛個数
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     MAIN.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.個数 AS 値" +
                              "\r\n FROM" +
                              // -- 駆動表（売上明細）
                              "\r\n     (SELECT DISTINCT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         売上明細 AS M" +
                              "\r\n         INNER JOIN 売上ヘッダ AS H" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.売上日     = M.売上日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.売上日 = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     ) AS MAIN" +
                              "\r\n     LEFT JOIN" +
                              // -- 売上明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
                              "\r\n     (SELECT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U0' AS 商品ＣＤ" +
                              "\r\n         ,SUM(M.商品数量) AS 個数" +
                              "\r\n     FROM" +
                              "\r\n         売上明細 AS M" +
                              "\r\n         INNER JOIN 売上ヘッダ AS H" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.売上日     = M.売上日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND" +     // -- LINE決済は売掛と見なす
                              "\r\n         M.売上日 = @喫食日 AND" +
                              "\r\n         M.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     GROUP BY" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS A" +
                              "\r\n         ON MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                              "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n UNION" +
                              // --- 売上売掛金額
                              "\r\n SELECT" +
                              "\r\n     0 AS 明細合計区分," +
                              "\r\n     MAIN.得意先ＣＤ," +
                              "\r\n     T.得意先名," +
                              "\r\n     CONVERT(varchar, MAIN.得意先ＣＤ) + ' ' + ISNULL(V.売掛現金区分名, ' ') + CHAR(10) + ISNULL(T.得意先名, '') AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     A.商品ＣＤ," +
                              "\r\n     A.金額 AS 値" +
                              "\r\n FROM" +
                              // -- 駆動表（売上明細）
                              "\r\n     (SELECT DISTINCT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         売上明細 AS M" +
                              "\r\n         INNER JOIN 売上ヘッダ AS H" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.売上日     = M.売上日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         H.売上日 = @喫食日 AND" +
                              "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     ) AS MAIN" +
                              "\r\n     LEFT JOIN" +
                              // -- 売上明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
                              "\r\n     (SELECT" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') + '_U1' AS 商品ＣＤ" +
                              "\r\n         ,SUM(M.商品金額合計税込) AS 金額" +
                              "\r\n     FROM" +
                              "\r\n         売上明細 AS M" +
                              "\r\n         INNER JOIN 売上ヘッダ AS H" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.売上日     = M.売上日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A" +
                              "\r\n             ON A.企業ＣＤ = M.企業ＣＤ" +
                              "\r\n             AND A.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n             AND A.部署ＣＤ = M.部署ＣＤ" +
                              "\r\n             AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                              "\r\n         M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND" +     // -- LINE決済は売掛と見なす
                              "\r\n         M.売上日 = @喫食日 AND" +
                              "\r\n         M.部署ＣＤ = @部署ＣＤ AND" +
                              "\r\n         H.コースＣＤ IS NULL" +
                              "\r\n     GROUP BY" +
                              "\r\n         M.得意先ＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS A" +
                              "\r\n         ON MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                              "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                              "\r\n     LEFT JOIN 得意先マスタ  AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                              "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n UNION" +
                              // --- 現金合計
                              "\r\n SELECT" +
                              "\r\n     1 AS 明細合計区分," +
                              "\r\n     -1 AS 得意先ＣＤ," +
                              "\r\n     NULL AS 得意先名," +
                              "\r\n     '合計' AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     NULL AS 商品ＣＤ," +
                              "\r\n     NULL AS 値" +
                              "\r\n FROM" +
                              "\r\n     V_売掛現金区分 AS VT" +
                              "\r\n WHERE" +
                              "\r\n     VT.売掛現金区分CD = @売掛現金区分現金" +
                              "\r\n UNION" +
                              // --- 売掛合計
                              "\r\n SELECT" +
                              "\r\n     1 AS 明細合計区分," +
                              "\r\n     -1 AS 得意先ＣＤ," +
                              "\r\n     NULL AS 得意先名," +
                              "\r\n     '合計' AS 表示用得意先名," +
                              "\r\n     VT.売掛現金区分CD AS 売掛現金区分," +
                              "\r\n     VT.売掛現金区分名 AS タイプ," +
                              "\r\n     NULL AS 商品ＣＤ," +
                              "\r\n     NULL AS 値" +
                              "\r\n FROM" +
                              "\r\n     V_売掛現金区分 AS VT" +
                              "\r\n WHERE" +
                              "\r\n     VT.売掛現金区分CD = @売掛現金区分売掛" +
                              "\r\n ORDER BY" +
                              "\r\n     明細合計区分," +
                              "\r\n     得意先ＣＤ," +
                              "\r\n     売掛現金区分," +
                              "\r\n     商品ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _chumonListCriteria.喫食日;
                        cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                        cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                        cmd.Parameters.Add("@売掛現金区分売掛", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Urikake;
                        cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                        cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;
                    }
                    cmd.CommandText = string.Format(sql, (chkHaisobi.Checked ? "配送日" : "喫食日"));

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("コース注文一覧を検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("コース注文一覧");
                // 商品名列を作成
                cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["得意先ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["得意先名"]);
                cldt.AddFixedColumn(dt.Columns["表示用得意先名"]);
                cldt.AddFixedColumn(dt.Columns["タイプ"]);
                // インポート
                cldt.Import(dt, "商品ＣＤ", "値");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);

                fpSpread1.SuspendLayout();      // 描画を一時停止

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "コース注文一覧";

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_COLUMN, "表示用得意先名");
                fpSpread1_Sheet1.BindDataColumn(TYPE_COLUMN, "タイプ");

                // 商品名を列名としてバインド
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // 表示用得意先名列を結合
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    // ついでに背景色とCanFocusをリセット
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;

                    if (rowIndex % 2 == 0)
                    {
                        fpSpread1_Sheet1.AddSpanCell(rowIndex, 0, 2, 1);
                    }
                }

                // 最終２行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 2;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 2].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 2].CanFocus = false;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;

                // 合計行に数式を設定する
                for (columnIndex = SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                {
                    int? genkinTotal = null;
                    int? urikakeTotal = null;
                    // 数式の結果が表示されないので自力で計算
                    for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count - 2; rowIndex++)
                    {
                        if (fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value != null)
                        {
                            if (rowIndex % 2 == 0)
                            {
                                // 現金
                                genkinTotal = (genkinTotal ?? 0) + (int)(fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value);  // intで格納されている
                            }
                            else
                            {
                                // 売掛
                                urikakeTotal = (urikakeTotal ?? 0) + (int)(fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value);
                            }
                        }
                    }

                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex].Value = genkinTotal;       // 現金合計
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex].Value = urikakeTotal;      // 売掛合計

                    //FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                    //FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 3, columnIndex];         // -2 までが合計行なので-3
                    //FarPoint.Win.Spread.Cell genkinSumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];      // 現金の合計(奇数行)
                    //FarPoint.Win.Spread.Cell urikakeSumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];     // 売掛の合計(偶数行)
                    //// 奇数行・偶数行の合計のお約束的な数式
                    //genkinSumCell.Formula = string.Format("SUMPRODUCT((MOD(ROW({0}:{1}),2)=1)*({0}:{1}))", topCell.ToString(), bottomCell.ToString());
                    //urikakeSumCell.Formula = string.Format("SUMPRODUCT((MOD(ROW({0}:{1}),2)=0)*({0}:{1}))", topCell.ToString(), bottomCell.ToString());
                }

                if (fpSpread1_Sheet1.Rows.Count > 2)    // ２行は合計行なので
                {
                    fpSpread1.Focus();
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, SYOHIN_START_COLUMN);
                    fpSpread1_Sheet1.AddSelection(0, SYOHIN_START_COLUMN, 1, 1);
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _chumonListCriteriaSnapShot = new CourseChumonListCriteria(_chumonListCriteria);

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
                fpSpread1.ResumeLayout(true);       // 描画再開
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
                    if (fpSpread1_Sheet1.Rows.Count > 0)
                    {
                        if (fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.ActiveRowIndex].BackColor != TOTAL_BACK_COLOR)
                        {
                            ShowDetail();       // 詳細表示
                        }
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
        /// 注文入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChumon_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_chumonListCriteriaSnapShot == null)
                {
                    return;
                }

                if (fpSpread1_Sheet1.Rows.Count <= 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowDetail();       // 詳細表示（既存レコード）
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
        /// コース注文入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseChumon_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_chumonListCriteriaSnapShot == null)
                {
                    return;
                }

                ShowCourseOrder();      // コース注文入力表示
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
        /// 売上反映ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUriageHanei_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_chumonListCriteriaSnapShot == null)
                {
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_chumonListCriteria.HasErrors)
                {
                    MessageBox.Show(this, _chumonListCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_chumonListCriteria.Equals(_chumonListCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (fpSpread1_Sheet1.Rows.Count <= 2)       // 合計行が２行あるので
                {
                    MessageBox.Show(this, "売上に反映できるレコードがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show(this, "表示している全ての注文を売上に反映しますか？\n（売上が存在する場合は上書きします）", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 注文から売上を生成
                CreateUriage();

                btnQuery.PerformClick();        // 再読み込み
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

                    // -- 以下のように並べる
                    // -- ┌─────────────────┬─────────────────┐
                    // -- │               注文               │               売上               │
                    // -- ├─────┬─────┬─────┼─────┬─────┬─────┤
                    // -- │  商品Ａ  │  商品Ｂ  │  その他  │  商品Ａ  │  商品Ｂ  │  その他  │
                    // -- ├──┬──┼──┬──┼──┬──┼──┬──┼──┬──┼──┬──┤
                    // -- │個数│金額│個数│金額│個数│金額│個数│金額│個数│金額│個数│金額│
                    // -- └──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┘
                    // -- 商品ＣＤのアンダーバー以降は
                    // -- C:注文 U:売上
                    // -- 0:個数 1:金額

                                    // --①商品名（注文個数）
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     0 AS 注文売上区分" +
                                      "\r\n     ,0 AS その他区分" +
                                      "\r\n     ,A.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, A.商品ＣＤ) + '_C0' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,'個数' AS 個数金額" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      // -- ②商品名（注文金額）
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 注文売上区分" +
                                      "\r\n     ,0 AS その他区分" +
                                      "\r\n     ,A.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, A.商品ＣＤ) + '_C1' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,'金額' AS 個数金額" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      // -- ③その他（注文個数）
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 注文売上区分" +
                                      "\r\n     ,1 AS その他区分" +
                                      "\r\n     ,-1 AS ソート用商品ＣＤ" +
                                      "\r\n     ,'XXX_C0' AS 商品ＣＤ" +
                                      "\r\n     ,'その他' AS 商品名略称" +
                                      "\r\n     ,'個数' AS 個数金額" +
                                      "\r\n UNION" +
                                      // -- ④その他（注文金額）
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 注文売上区分" +
                                      "\r\n     ,1 AS その他区分" +
                                      "\r\n     ,-1 AS ソート用商品ＣＤ" +
                                      "\r\n     ,'XXX_C1' AS 商品ＣＤ" +
                                      "\r\n     ,'その他' AS 商品名略称" +
                                      "\r\n     ,'金額' AS 個数金額" +
                                      "\r\n UNION" +
                                      // -- ⑤商品名（売上個数）
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 注文売上区分" +
                                      "\r\n     ,0 AS その他区分" +
                                      "\r\n     ,A.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, A.商品ＣＤ) + '_U0' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,'個数' AS 個数金額" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      // -- ⑥商品名（売上金額）
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 注文売上区分" +
                                      "\r\n     ,0 AS その他区分" +
                                      "\r\n     ,A.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, A.商品ＣＤ) + '_U1' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,'金額' AS 個数金額" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      // -- ⑦その他（売上個数）
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 注文売上区分" +
                                      "\r\n     ,1 AS その他区分" +
                                      "\r\n     ,-1 AS ソート用商品ＣＤ" +
                                      "\r\n     ,'XXX_U0' AS 商品ＣＤ" +
                                      "\r\n     ,'その他' AS 商品名略称" +
                                      "\r\n     ,'個数' AS 個数金額" +
                                      "\r\n UNION" +
                                      // ⑧その他（売上金額）
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 注文売上区分" +
                                      "\r\n     ,1 AS その他区分" +
                                      "\r\n     ,-1 AS ソート用商品ＣＤ" +
                                      "\r\n     ,'XXX_U1' AS 商品ＣＤ" +
                                      "\r\n     ,'その他' AS 商品名略称" +
                                      "\r\n     ,'金額' AS 個数金額" +
                                      "\r\n ORDER BY" +
                                      "\r\n     注文売上区分" +
                                      "\r\n     ,その他区分" +
                                      "\r\n     ,ソート用商品ＣＤ" +
                                      "\r\n     ,商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;

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
        /// <param name="syohinDt">商品名が格納されたデータテーブル</param>
        /// <param name="labelColumnName">商品名が格納された列の名前</param>
        /// <param name="colorChangeColumnName">背景色を変える判断をする列の名前</param>
        /// <param name="colorChangeValue">背景色を変える判断をする列の値が格納されている列の名前</param>
        private void SetSpreadColumn(DataTable syohinDt, string labelColumnName, string colorChangeColumnName, int colorChangeValue)
        {
            try
            {
                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN;

                // 今回追加する商品名の列数をセット
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN + syohinDt.Rows.Count;

                // 固定列に続いて、商品名の列の属性をセット
                int columnIndex = SYOHIN_START_COLUMN;
                int prevColorChangeValue = -1;
                int changeStartColumnIndex = -1;
                foreach (DataRow dr in syohinDt.Rows)
                {
                    if (columnIndex % 2 == 0)
                    {
                        // 個数
                        FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                        numberCellType.EnableSubEditor = false;
                        numberCellType.DecimalPlaces = 0;
                        numberCellType.MaximumValue = 99999D;
                        numberCellType.MinimumValue = 0D;
                        numberCellType.ReadOnly = true;
                        numberCellType.Separator = ",";
                        numberCellType.ShowSeparator = true;
                        numberCellType.Static = true;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Label = "個数";
                        fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    }
                    else
                    {
                        // 金額
                        FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType = new FarPoint.Win.Spread.CellType.CurrencyCellType();
                        currencyCellType.CurrencySymbol = "\\";
                        currencyCellType.DecimalPlaces = 0;
                        currencyCellType.EnableSubEditor = false;
                        currencyCellType.MaximumValue = new decimal(new int[] {
                        999999999,
                        0,
                        0,
                        0});
                        currencyCellType.MinimumValue = new decimal(new int[] {
                        0,
                        0,
                        0,
                        0});
                        currencyCellType.ReadOnly = true;
                        currencyCellType.Separator = ",";
                        currencyCellType.ShowSeparator = true;
                        currencyCellType.Static = true;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = currencyCellType;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Label = "金額";
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Width = 80F;

                        // 個数と金額が揃ったので、列ヘッダーを設定
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex - 1).ColumnSpan = 2;
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex - 1).Value = dr[labelColumnName].ToString();
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(2, columnIndex - 1).Value = "個数";
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(2, columnIndex).Value = "金額";
                    }

                    // 注文売上区分が変わった？
                    if (prevColorChangeValue != -1 && (int)dr[colorChangeColumnName] != prevColorChangeValue)
                    {
                        // それまでの列を「注文」とする
                        this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, SYOHIN_START_COLUMN).ColumnSpan = columnIndex - SYOHIN_START_COLUMN;
                        this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, SYOHIN_START_COLUMN).Value = "注文";
                        this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, SYOHIN_START_COLUMN).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                        this.fpSpread1_Sheet1.ColumnHeader.Rows[0].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                        changeStartColumnIndex = columnIndex;
                    }
                    prevColorChangeValue = (int)dr[colorChangeColumnName];

                    // 列ヘッダーの背景色を変える？
                    if ((int)dr[colorChangeColumnName] == colorChangeValue)
                    {
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex).BackColor = Color.FromArgb(192, 192, 0);
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex).BackColor = Color.FromArgb(192, 192, 0);
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(2, columnIndex).BackColor = Color.FromArgb(192, 192, 0);
                    }
                    columnIndex++;
                }

                // 「注文」以降のここまでの列を「売上」とする
                this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, changeStartColumnIndex).ColumnSpan = columnIndex - changeStartColumnIndex;
                this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, changeStartColumnIndex).Value = "売上";
                this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, changeStartColumnIndex).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
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
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_chumonListCriteria.HasErrors)
                {
                    MessageBox.Show(this, _chumonListCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_chumonListCriteria.Equals(_chumonListCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                ChumonMeisaiCriteria chumonCriteria = new ChumonMeisaiCriteria();
                chumonCriteria.stopOverwrite = true;
                chumonCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                chumonCriteria.得意先名 = dr["得意先名"] as string;
                chumonCriteria.部署ＣＤ = _chumonListCriteria.部署ＣＤ;
                chumonCriteria.注文日 = _chumonListCriteria.喫食日;
                chumonCriteria.コースＣＤ = _chumonListCriteria.コースＣＤ;     // デフォルト表示するコース

                // 無効な得意先CDの場合はNOP
                if (chumonCriteria.得意先ＣＤ == null || chumonCriteria.得意先ＣＤ < 0)
                {
                    return;
                }

                Form200 formChumon = new Form200(chumonCriteria);
                if (formChumon.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(formChumon);
                formChumon.ListOpened = true;
                formChumon.Show();
                formChumon.FormClosed += new FormClosedEventHandler(Form200_FormClosed);
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
        /// 注文入力画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form200_FormClosed(object sender, FormClosedEventArgs e)
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

                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                btnQuery.PerformClick();        // 再表示

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

        /// <summary>
        /// コース注文入力表示
        /// </summary>
        private void ShowCourseOrder()
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_chumonListCriteria.HasErrors)
                {
                    MessageBox.Show(this, _chumonListCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // コース注文入力検索条件
                CourseChumonCriteria courseChumonCriteria = new CourseChumonCriteria();
                courseChumonCriteria.注文日 = _chumonListCriteria.喫食日;
                courseChumonCriteria.コースＣＤ = _chumonListCriteria.コースＣＤ;

                Form201 formCourseChumon = new Form201(courseChumonCriteria);
                if (formCourseChumon.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(formCourseChumon);
                formCourseChumon.Show();
                formCourseChumon.FormClosed += new FormClosedEventHandler(Form201_FormClosed);
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
        /// コース注文入力画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form201_FormClosed(object sender, FormClosedEventArgs e)
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

                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                btnQuery.PerformClick();        // 再表示

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

        /// <summary>
        /// 注文から売上を生成する（既存の売上は上書きする）
        /// </summary>
        private void CreateUriage()
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                DateTime kissyokubi = (DateTime)_chumonListCriteria.喫食日;
                List<int> tokuisakiCDList = new List<int>();            // 売上作成済み得意先CDリスト
                LineProxy lineProxy = new LineProxy();                  // 配達完了受入API（呼び出し判定用）
                bool canApiCall = lineProxy.CanCall();
                List<int> apiTargetTokuisakiCDList = new List<int>();   // 配達完了受入API用得意先CDリスト

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    DataSet ds = (DataSet)bindingSource1.DataSource;
                    DataTable dt = ds.Tables["コース注文一覧"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        int tokuisakiCD = (int)dr["得意先ＣＤ"];
                        // 無効な得意先CDの場合はNOP
                        if (tokuisakiCD < 0)
                        {
                            continue;
                        }

                        // 一度処理した得意先はスキップ（１つの得意先で２レコード使っているので）
                        if (tokuisakiCDList.Contains(tokuisakiCD))
                        {
                            continue;
                        }
                        tokuisakiCDList.Add(tokuisakiCD);

                        // 売上ヘッダを作成
                        if (CreateUriageHeader(con, tran, tokuisakiCD, kissyokubi) == false)
                        {
                            return;
                        }

                        // 売上明細を作成
                        if (CreateUriageMeisai(con, tran, tokuisakiCD, kissyokubi) == false)
                        {
                            return;
                        }

                        // 配達完了受入APIを呼び出す必要がある場合は、LINEによる注文があればその得意先を対象にする
                        if (canApiCall)
                        {
                            if (HasOrderFromLine(con, tran, tokuisakiCD, kissyokubi))
                            {
                                apiTargetTokuisakiCDList.Add(tokuisakiCD);
                            }
                        }
                    }
                    // コミット
                    tran.Commit();
                    log.InfoFormat("売上ヘッダ・売上明細を{0}しました。", historyProcess.DisplayName());
                    History.Save(con, tran, this.Text, "注文から売上", string.Format("コースＣＤ=[{0}] {1}=[{2:yyyy/MM/dd}]", _chumonListCriteria.コースＣＤ, ((_chumonListCriteria.配送日フラグ ?? false) ? "配送日" : "喫食日"), _chumonListCriteria.喫食日), historyProcess);
                }

                // LINE決済用中間サーバの配達完了受入APIを非同期で呼ぶ
                if (canApiCall)
                {
                    CallSetArrived(kissyokubi, apiTargetTokuisakiCDList);
                }

                MessageBox.Show(this, "売上に反映しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 売上ヘッダを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <param name="kissyokubi">喫食日</param>
        /// <returns>成功した場合は True</returns>
        private bool CreateUriageHeader(SqlConnection con, SqlTransaction tran, int tokuisakiCD, DateTime kissyokubi)
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                // 売上ヘッダを削除
                SqlCommand cmdDel = con.CreateCommand();
                cmdDel.Transaction = tran;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" DELETE UH");
                sql.AppendLine(" FROM 売上ヘッダ AS UH");
                sql.AppendLine("      INNER JOIN 注文ヘッダ AS CH");
                sql.AppendLine("      ON UH.企業ＣＤ    = CH.企業ＣＤ");
                sql.AppendLine("      AND UH.売上日     = CH.喫食日");
                sql.AppendLine("      AND UH.得意先ＣＤ = CH.得意先ＣＤ");
                sql.AppendLine("      AND UH.部署ＣＤ   = CH.部署ＣＤ");
                sql.AppendLine("  WHERE CH.企業ＣＤ   = @企業ＣＤ");
                sql.AppendLine("    AND CH.注文区分   = @注文区分");
                sql.AppendLine("    AND CH.{0}     = @喫食日");
                sql.AppendLine("    AND CH.得意先ＣＤ = @得意先ＣＤ");
                sql.AppendLine("    AND CH.部署ＣＤ   = @部署ＣＤ");
                if (_chumonListCriteria.コースＣＤ != null)
                {
                    sql.AppendLine("    AND CH.コースＣＤ = @コースＣＤ ");
                    cmdDel.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _chumonListCriteria.コースＣＤ;
                }
                else
                {
                    sql.AppendLine("    AND CH.コースＣＤ IS NULL ");
                }

                cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmdDel.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmdDel.Parameters.Add("@喫食日", SqlDbType.Date).Value = kissyokubi;
                cmdDel.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmdDel.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;

                cmdDel.CommandText = string.Format(sql.ToString(), ((_chumonListCriteria.配送日フラグ ?? false) ? "配送日" : "喫食日"));

                log.Debug(cmdDel.CommandTextWithParameters());
                cmdDel.ExecuteNonQuery();

                // 注文ヘッダから売上ヘッダを作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                sql.Clear();
                sql.AppendLine(" INSERT INTO 売上ヘッダ(");
                sql.AppendLine("     企業ＣＤ");
                sql.AppendLine("     ,売上日");
                sql.AppendLine("     ,得意先ＣＤ");
                sql.AppendLine("     ,部署ＣＤ");
                sql.AppendLine("     ,得意先名");
                sql.AppendLine("     ,コースＣＤ");
                sql.AppendLine("     ,配送担当者ＣＤ");
                sql.AppendLine("     ,税区分");
                sql.AppendLine("     ,丸め処理区分");
                sql.AppendLine("     ,納品書伝票番号");
                sql.AppendLine("     ,作成担当者ＣＤ");
                sql.AppendLine("     ,作成日");
                sql.AppendLine("     ,修正担当者ＣＤ");
                sql.AppendLine("     ,修正日");
                sql.AppendLine(" ) SELECT");
                sql.AppendLine("     企業ＣＤ");
                sql.AppendLine("     ,喫食日");
                sql.AppendLine("     ,得意先ＣＤ");
                sql.AppendLine("     ,部署ＣＤ");
                sql.AppendLine("     ,得意先名");
                sql.AppendLine("     ,コースＣＤ");
                sql.AppendLine("     ,担当者ＣＤ");
                sql.AppendLine("     ,税区分");
                sql.AppendLine("     ,丸め処理区分");
                sql.AppendLine("     ,納品書伝票番号");
                sql.AppendLine("     ,@作成担当者ＣＤ");
                sql.AppendLine("     ,getdate()");
                sql.AppendLine("     ,@修正担当者ＣＤ");
                sql.AppendLine("     ,getdate()");
                sql.AppendLine(" FROM");
                sql.AppendLine("     注文ヘッダ");
                sql.AppendLine(" WHERE ");
                sql.AppendLine("     企業ＣＤ   = @企業ＣＤ");
                sql.AppendLine(" AND 注文区分   = @注文区分");
                sql.AppendLine(" AND {0}     = @喫食日");
                sql.AppendLine(" AND 得意先ＣＤ = @得意先ＣＤ");
                sql.AppendLine(" AND 部署ＣＤ   = @部署ＣＤ");
                if (_chumonListCriteria.コースＣＤ != null)
                {
                    sql.AppendLine(" AND コースＣＤ = @コースＣＤ ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _chumonListCriteria.コースＣＤ;
                }
                else
                {
                    sql.AppendLine(" AND コースＣＤ IS NULL ");
                }

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = kissyokubi;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;
                UserData userData = UserData.getInstance();
                cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                cmd.CommandText = string.Format(sql.ToString(), ((_chumonListCriteria.配送日フラグ ?? false) ? "配送日" : "喫食日"));

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("売上ヘッダが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("売上ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 売上明細を作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <param name="kissyokubi">喫食日</param>
        /// <returns>成功した場合は True</returns>
        private bool CreateUriageMeisai(SqlConnection con, SqlTransaction tran, int tokuisakiCD, DateTime kissyokubi)
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                // 売上明細を削除
                SqlCommand cmdDel = con.CreateCommand();
                cmdDel.Transaction = tran;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" DELETE UM");
                sql.AppendLine(" FROM 売上明細 AS UM");
                sql.AppendLine("      INNER JOIN 注文ヘッダ AS CH");
                sql.AppendLine("      ON UM.企業ＣＤ    = CH.企業ＣＤ");
                sql.AppendLine("      AND UM.売上日     = CH.喫食日");
                sql.AppendLine("      AND UM.得意先ＣＤ = CH.得意先ＣＤ");
                sql.AppendLine("      AND UM.部署ＣＤ   = CH.部署ＣＤ");
                sql.AppendLine("  WHERE CH.企業ＣＤ   = @企業ＣＤ");
                sql.AppendLine("    AND CH.注文区分   = @注文区分");
                sql.AppendLine("    AND CH.{0}     = @喫食日");
                sql.AppendLine("    AND CH.得意先ＣＤ = @得意先ＣＤ");
                sql.AppendLine("    AND CH.部署ＣＤ   = @部署ＣＤ");
                if (_chumonListCriteria.コースＣＤ != null)
                {
                    sql.AppendLine("    AND CH.コースＣＤ = @コースＣＤ ");
                    cmdDel.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _chumonListCriteria.コースＣＤ;
                }
                else
                {
                    sql.AppendLine("    AND CH.コースＣＤ IS NULL ");
                }

                cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmdDel.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmdDel.Parameters.Add("@喫食日", SqlDbType.Date).Value = kissyokubi;
                cmdDel.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmdDel.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;

                cmdDel.CommandText = string.Format(sql.ToString(), ((_chumonListCriteria.配送日フラグ ?? false) ? "配送日" : "喫食日"));

                log.Debug(cmdDel.CommandTextWithParameters());
                cmdDel.ExecuteNonQuery();

                // 注文明細から売上明細を作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                sql.Clear();
                sql.AppendLine(" INSERT INTO 売上明細(");
                sql.AppendLine("     企業ＣＤ");
                sql.AppendLine("     ,売上日");
                sql.AppendLine("     ,得意先ＣＤ");
                sql.AppendLine("     ,部署ＣＤ");
                sql.AppendLine("     ,明細番号");
                sql.AppendLine("     ,商品ＣＤ");
                sql.AppendLine("     ,商品グループＣＤ");
                sql.AppendLine("     ,売上区分");
                sql.AppendLine("     ,商品単価");
                sql.AppendLine("     ,商品数量");
                sql.AppendLine("     ,値引額");
                sql.AppendLine("     ,値引事由区分");
                sql.AppendLine("     ,値引前金額");
                sql.AppendLine("     ,値引後金額");
                sql.AppendLine("     ,消費税額");
                sql.AppendLine("     ,商品金額合計税込");
                sql.AppendLine("     ,計算税率");
                sql.AppendLine("     ,チケット束フラグ");
                sql.AppendLine("     ,現金補填フラグ");
                sql.AppendLine("     ,主食ＣＤ");
                sql.AppendLine("     ,副食ＣＤ");
                sql.AppendLine("     ,食事区分");
                sql.AppendLine("     ,分配元数量");
                sql.AppendLine("     ,作成担当者ＣＤ");
                sql.AppendLine("     ,作成日");
                sql.AppendLine("     ,修正担当者ＣＤ");
                sql.AppendLine("     ,修正日");
                sql.AppendLine(" ) SELECT");
                sql.AppendLine("     A.企業ＣＤ");
                sql.AppendLine("     ,A.喫食日");
                sql.AppendLine("     ,A.得意先ＣＤ");
                sql.AppendLine("     ,A.部署ＣＤ");
                sql.AppendLine("     ,A.明細番号");
                sql.AppendLine("     ,A.商品ＣＤ");
                sql.AppendLine("     ,A.商品グループＣＤ");
                sql.AppendLine("     ,CASE WHEN T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN @売上区分チケット ELSE A.売掛現金区分 END AS 売上区分");
                sql.AppendLine("     ,A.商品単価");
                sql.AppendLine("     ,A.商品数量");
                sql.AppendLine("     ,0");
                sql.AppendLine("     ,NULL");
                sql.AppendLine("     ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0 AND T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN 0 ELSE A.商品金額合計     END AS 値引前金額");
                sql.AppendLine("     ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0 AND T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN 0 ELSE A.商品金額合計     END AS 値引後金額");
                sql.AppendLine("     ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0 AND T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN 0 ELSE A.消費税額         END AS 消費税額");
                sql.AppendLine("     ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0 AND T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN 0 ELSE A.商品金額合計税込 END AS 商品金額合計税込");
                sql.AppendLine("     ,A.計算税率");
                sql.AppendLine("     ,CASE WHEN S.弁当区分 = @弁当区分チケット THEN 1 ELSE 0 END AS チケット束フラグ");
                sql.AppendLine("     ,0");
                sql.AppendLine("     ,S.主食ＣＤ");
                sql.AppendLine("     ,S.副食ＣＤ");
                sql.AppendLine("     ,S.食事区分");
                sql.AppendLine("     ,NULL");
                sql.AppendLine("     ,@作成担当者ＣＤ");
                sql.AppendLine("     ,getdate()");
                sql.AppendLine("     ,@修正担当者ＣＤ");
                sql.AppendLine("     ,getdate()");
                sql.AppendLine(" FROM");
                sql.AppendLine("     注文明細 AS A");
                sql.AppendLine("     INNER JOIN 注文ヘッダ AS H");
                sql.AppendLine("         ON  H.企業ＣＤ   = A.企業ＣＤ");
                sql.AppendLine("         AND H.注文区分   = A.注文区分");
                sql.AppendLine("         AND H.喫食日     = A.喫食日");
                sql.AppendLine("         AND H.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("         AND H.部署ＣＤ   = A.部署ＣＤ");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("     INNER JOIN 管理マスタ AS K ON K.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine(" WHERE ");
                sql.AppendLine("     H.企業ＣＤ   = @企業ＣＤ");
                sql.AppendLine(" AND H.注文区分   = @注文区分");
                sql.AppendLine(" AND H.{0}     = @喫食日");
                sql.AppendLine(" AND H.得意先ＣＤ = @得意先ＣＤ");
                sql.AppendLine(" AND H.部署ＣＤ   = @部署ＣＤ");
                if (_chumonListCriteria.コースＣＤ != null)
                {
                    sql.AppendLine(" AND H.コースＣＤ = @コースＣＤ ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _chumonListCriteria.コースＣＤ;
                }
                else
                {
                    sql.AppendLine(" AND H.コースＣＤ IS NULL ");
                }

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = kissyokubi;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;
                cmd.Parameters.Add("@弁当区分チケット", SqlDbType.Int).Value = (int)Consts.BentoKubun.Ticket;
                cmd.Parameters.Add("@支払方法チケット", SqlDbType.Int).Value = (int)Consts.ShiharaiHohoKubun.Ticket;
                cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;
                UserData userData = UserData.getInstance();
                cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                cmd.CommandText = string.Format(sql.ToString(), ((_chumonListCriteria.配送日フラグ ?? false) ? "配送日" : "喫食日"));

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("売上明細が重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("売上明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// LINEによる注文の有無を返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <param name="kissyokubi">喫食日</param>
        /// <returns>LINEによる注文が有ればTrue</returns>
        private bool HasOrderFromLine(SqlConnection con, SqlTransaction tran, int tokuisakiCD, DateTime kissyokubi)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     COUNT(A.明細番号) AS CNT");
                sql.AppendLine(" FROM");
                sql.AppendLine("     注文明細 AS A");
                sql.AppendLine("     INNER JOIN 注文ヘッダ AS H");
                sql.AppendLine("         ON  H.企業ＣＤ   = A.企業ＣＤ");
                sql.AppendLine("         AND H.注文区分   = A.注文区分");
                sql.AppendLine("         AND H.喫食日     = A.喫食日");
                sql.AppendLine("         AND H.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("         AND H.部署ＣＤ   = A.部署ＣＤ");
                sql.AppendLine(" WHERE");
                sql.AppendLine("     H.企業ＣＤ     = @企業ＣＤ");
                sql.AppendLine(" AND H.注文区分     = @注文区分");
                sql.AppendLine(" AND H.{0}     = @喫食日");
                sql.AppendLine(" AND H.得意先ＣＤ   = @得意先ＣＤ");
                sql.AppendLine(" AND H.部署ＣＤ     = @部署ＣＤ");
                if (_chumonListCriteria.コースＣＤ != null)
                {
                    sql.AppendLine(" AND H.コースＣＤ = @コースＣＤ ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _chumonListCriteria.コースＣＤ;
                }
                else
                {
                    sql.AppendLine(" AND H.コースＣＤ IS NULL ");
                }
                sql.AppendLine(" AND A.売掛現金区分 = @売掛現金区分LINE");

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = kissyokubi;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;
                cmd.Parameters.Add("@売掛現金区分LINE", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Line;

                cmd.CommandText = string.Format(sql.ToString(), ((_chumonListCriteria.配送日フラグ ?? false) ? "配送日" : "喫食日"));

                log.Debug(cmd.CommandTextWithParameters());

                int cnt = 0;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    cnt = (int)reader["CNT"];
                }

                // レコードの有無を返す
                return (cnt > 0);
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
        /// LINE決済用中間サーバの配達完了受入APIを非同期で呼びだす
        /// </summary>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="apiTargetTokuisakiCDList">得意先CDのリスト</param>
        private void CallSetArrived(DateTime kissyokubi, List<int> apiTargetTokuisakiCDList)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // 配達完了受入API
                        LineProxy lineProxy = new LineProxy();
                        lineProxy.SetArrived((int)_chumonListCriteria.部署ＣＤ, kissyokubi, apiTargetTokuisakiCDList);
                    }
                    catch (AggregateException ex)   // 非同期のメソッドで発生した例外をCatchする
                    {
                        log.Error(ex);
                        History.Error(ex);
                        // ここで throw new EmuException(ex) しても上位には伝播されないので、ここでメッセージボックスを表示する
                        MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }).ContinueWith(_ =>
                {
                    // 非同期処理の完了後の処理

                    // 特に何もしない

                }, TaskScheduler.FromCurrentSynchronizationContext());  // UIスレッドで動作させる
        }
        #endregion
    }
}
