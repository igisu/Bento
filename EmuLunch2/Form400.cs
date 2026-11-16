using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// コース売上一覧
    /// </summary>
    public partial class Form400 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int ROWNO_COLUMN = 0;         // 行No列インデックス
        private const int TOKUISAKI_COLUMN = 1;     // 得意先列インデックス
        private const int TYPE_COLUMN = 2;          // タイプ列インデックス
        // これ以降の列は動的に追加される
        private const int SYOHIN_START_COLUMN = 3;  // 商品名列開始列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// コース売上一覧検索条件
        /// </summary>
        private CourseUriageListCriteria _uriageCriteria;

        /// <summary>
        /// コース売上一覧検索条件検索実行時スナップショット
        /// </summary>
        private CourseUriageListCriteria _uriageCriteriaSnapShot;

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
        public Form400()
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
        private void Form400_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.calculator_ico;

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

                // 先頭3列を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = 3;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _uriageCriteria = new CourseUriageListCriteria();
                _uriageCriteria.売上日 = DateTime.Today;
                uriageCriteriaBindingSource.DataSource = _uriageCriteria;

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
        private void Form400_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F8:                       // 明細入力ボタン押下
                    btnMeisai.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
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
                Form902 formCourseDlg = new Form902();
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
                    _uriageCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;

                    _uriageCriteria.部署ＣＤ = course.部署ＣＤ;
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
                cboBusyo.SelectedValue = _uriageCriteria.部署ＣＤ ?? -1;
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
                if (_uriageCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_uriageCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _uriageCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 売上日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_uriageCriteria.売上日 > warnDate)
                {
                    if (MessageBox.Show(this, "売上日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        _uriageCriteriaSnapShot = null;
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                //*********************************************************
                // 主要商品リストマスタと売上詳細にある商品名を列として追加
                // コーステーブルにある得意先を行とする
                // データそのものは売上明細から取得する
                //*********************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                DataTable dt = new DataTable("コース売上一覧データ");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // -- 商品ＣＤのアンダーバー以降は
                    // -- 0:個数 1:金額
                    if (_uriageCriteria.コースＣＤ != null)
                    {
                        cmd.CommandText = // --- 売上現金個数
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
                                          // -- ①駆動表
                                          // -- ①－１（指定されたコースにぶら下がる得意先）
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
                                          "\r\n                 AND 実施日 = @売上日" +
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
                                          "\r\n         AND RT.実施日 = @売上日" +
                                          // -- ①－２（指定されたコースを持つ注文の得意先）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,コースＣＤ" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         注文ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 喫食日 = @売上日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          // -- ①－３（指定されたコースを持つ売上の得意先）"
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,コースＣＤ" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         売上ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 売上日 = @売上日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          "\r\n     ) AS C" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                                          "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                                          "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                                          "\r\n     LEFT JOIN (" +
                                          "\r\n         SELECT" +
                                          "\r\n             H.コースＣＤ," +
                                          "\r\n             T.受注得意先ＣＤ AS 得意先ＣＤ," +
                                          "\r\n             CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ," +
                                          "\r\n             SUM(M.商品数量) AS 個数" +
                                          "\r\n         FROM" +
                                          "\r\n             売上明細 AS M" +
                                          // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                          // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                                          "\r\n             INNER JOIN (" +
                                          "\r\n                 SELECT" +
                                          "\r\n                     企業ＣＤ" +
                                          "\r\n                     ,得意先ＣＤ" +
                                          "\r\n                     ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ" +
                                          "\r\n                 FROM" +
                                          "\r\n                     得意先マスタ" +
                                          "\r\n                 ) AS T" +
                                          "\r\n                 ON T.企業ＣＤ  = M.企業ＣＤ" +
                                          "\r\n                 AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n             INNER JOIN 売上ヘッダ AS H" +
                                          "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n                 AND H.売上日     = M.売上日" +
                                          "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             M.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n             M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND" +      // -- 現金、チケット、バークレー、ボーナスは現金と見なす
                                          "\r\n             M.売上日   = @売上日 AND" +
                                          "\r\n             M.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n         GROUP BY" +
                                          "\r\n             H.コースＣＤ," +
                                          "\r\n             T.受注得意先ＣＤ," +
                                          "\r\n             M.商品ＣＤ" +
                                          "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                                          // -- ②コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
                                          "\r\n     LEFT JOIN" +
                                          "\r\n         (SELECT" +
                                          "\r\n             CT.得意先ＣＤ" +
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
                                          "\r\n                     AND コースＣＤ = @コースＣＤ" +
                                          "\r\n                     AND 実施日 = @売上日" +
                                          "\r\n                 ) AS RM" +
                                          "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n             AND CT.コースＣＤ = @コースＣＤ" +
                                          "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                                          "\r\n             AND RM.コースＣＤ IS NULL" +
                                          // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                          "\r\n         UNION" +
                                          "\r\n         SELECT" +
                                          "\r\n             RT.得意先ＣＤ" +
                                          "\r\n             ,RT.コース順" +
                                          "\r\n         FROM" +
                                          "\r\n             臨時コーステーブル AS RT" +
                                          "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                          "\r\n         WHERE" +
                                          "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n             AND RT.コースＣＤ = @コースＣＤ" +
                                          "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                                          "\r\n             AND RT.実施日 = @売上日" +
                                          "\r\n         ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     C.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n     AND C.コースＣＤ = @コースＣＤ" +
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
                                          // -- ①駆動表
                                          // -- ①－１（指定されたコースにぶら下がる得意先）
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
                                          "\r\n                 AND 実施日 = @売上日" +
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
                                          "\r\n         AND RT.実施日 = @売上日" +
                                          // -- ①－２（指定されたコースを持つ注文の得意先）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,コースＣＤ" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         注文ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 喫食日 = @売上日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          // -- ①－３（指定されたコースを持つ売上の得意先）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,コースＣＤ" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         売上ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 売上日 = @売上日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          "\r\n     ) AS C" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                                          "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                                          "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分現金" +
                                          "\r\n     LEFT JOIN (" +
                                          "\r\n         SELECT" +
                                          "\r\n             H.コースＣＤ," +
                                          "\r\n             T.受注得意先ＣＤ AS 得意先ＣＤ," +
                                          "\r\n             CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ," +
                                          "\r\n             SUM(M.商品金額合計税込) AS 金額" +
                                          "\r\n         FROM" +
                                          "\r\n             売上明細 AS M" +
                                          // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                          // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                                          "\r\n             INNER JOIN (" +
                                          "\r\n                 SELECT" +
                                          "\r\n                     企業ＣＤ" +
                                          "\r\n                     ,得意先ＣＤ" +
                                          "\r\n                     ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ" +
                                          "\r\n                 FROM" +
                                          "\r\n                     得意先マスタ" +
                                          "\r\n                 ) AS T" +
                                          "\r\n                 ON T.企業ＣＤ  = M.企業ＣＤ" +
                                          "\r\n                 AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n             INNER JOIN 売上ヘッダ AS H" +
                                          "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n                 AND H.売上日     = M.売上日" +
                                          "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             M.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n             M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND" +      // -- 現金、チケット、バークレー、ボーナスは現金と見なす
                                          "\r\n             M.売上日   = @売上日 AND" +
                                          "\r\n             M.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n         GROUP BY" +
                                          "\r\n             H.コースＣＤ," +
                                          "\r\n             T.受注得意先ＣＤ," +
                                          "\r\n             M.商品ＣＤ" +
                                          "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                                          // -- ②コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
                                          "\r\n     LEFT JOIN" +
                                          "\r\n         (SELECT" +
                                          "\r\n             CT.得意先ＣＤ" +
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
                                          "\r\n                     AND コースＣＤ = @コースＣＤ" +
                                          "\r\n                     AND 実施日 = @売上日" +
                                          "\r\n                 ) AS RM" +
                                          "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n             AND CT.コースＣＤ = @コースＣＤ" +
                                          "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                                          "\r\n             AND RM.コースＣＤ IS NULL" +
                                          // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                          "\r\n         UNION" +
                                          "\r\n         SELECT" +
                                          "\r\n             RT.得意先ＣＤ" +
                                          "\r\n             ,RT.コース順" +
                                          "\r\n         FROM" +
                                          "\r\n             臨時コーステーブル AS RT" +
                                          "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                          "\r\n         WHERE" +
                                          "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n             AND RT.コースＣＤ = @コースＣＤ" +
                                          "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                                          "\r\n             AND RT.実施日 = @売上日" +
                                          "\r\n         ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     C.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n     AND C.コースＣＤ = @コースＣＤ" +
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
                                          // -- ①駆動表
                                          // -- ①－１（指定されたコースにぶら下がる得意先）
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
                                          "\r\n                 AND 実施日 = @売上日" +
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
                                          "\r\n         AND RT.実施日 = @売上日" +
                                          // -- ①－２（指定されたコースを持つ注文の得意先）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,コースＣＤ" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         注文ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 喫食日 = @売上日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          // -- ①－３（指定されたコースを持つ売上の得意先）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,コースＣＤ" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         売上ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 売上日 = @売上日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          "\r\n     ) AS C" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                                          "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                                          "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                                          "\r\n     LEFT JOIN (" +
                                          "\r\n         SELECT" +
                                          "\r\n             H.コースＣＤ," +
                                          "\r\n             T.受注得意先ＣＤ AS 得意先ＣＤ," +
                                          "\r\n             CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ," +
                                          "\r\n             SUM(M.商品数量) AS 個数" +
                                          "\r\n         FROM" +
                                          "\r\n             売上明細 AS M" +
                                          // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                          // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                                          "\r\n             INNER JOIN (" +
                                          "\r\n                 SELECT" +
                                          "\r\n                     企業ＣＤ" +
                                          "\r\n                     ,得意先ＣＤ" +
                                          "\r\n                     ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ" +
                                          "\r\n                 FROM" +
                                          "\r\n                     得意先マスタ" +
                                          "\r\n                 ) AS T" +
                                          "\r\n                 ON T.企業ＣＤ  = M.企業ＣＤ" +
                                          "\r\n                 AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n             INNER JOIN 売上ヘッダ AS H" +
                                          "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n                 AND H.売上日     = M.売上日" +
                                          "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             M.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n             M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND" +     // -- LINE決済は売掛と見なす
                                          "\r\n             M.売上日   = @売上日 AND" +
                                          "\r\n             M.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n         GROUP BY" +
                                          "\r\n             H.コースＣＤ," +
                                          "\r\n             T.受注得意先ＣＤ," +
                                          "\r\n             M.商品ＣＤ" +
                                          "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                                          // -- ②コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
                                          "\r\n     LEFT JOIN" +
                                          "\r\n         (SELECT" +
                                          "\r\n             CT.得意先ＣＤ" +
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
                                          "\r\n                     AND コースＣＤ = @コースＣＤ" +
                                          "\r\n                     AND 実施日 = @売上日" +
                                          "\r\n                 ) AS RM" +
                                          "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n             AND CT.コースＣＤ = @コースＣＤ" +
                                          "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                                          "\r\n             AND RM.コースＣＤ IS NULL" +
                                          // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                          "\r\n         UNION" +
                                          "\r\n         SELECT" +
                                          "\r\n             RT.得意先ＣＤ" +
                                          "\r\n             ,RT.コース順" +
                                          "\r\n         FROM" +
                                          "\r\n             臨時コーステーブル AS RT" +
                                          "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                          "\r\n         WHERE" +
                                          "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n             AND RT.コースＣＤ = @コースＣＤ" +
                                          "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                                          "\r\n             AND RT.実施日 = @売上日" +
                                          "\r\n         ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     C.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n     AND C.コースＣＤ = @コースＣＤ" +
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
                                          // -- ①駆動表
                                          // -- ①－１（指定されたコースにぶら下がる得意先）
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
                                          "\r\n                 AND 実施日 = @売上日" +
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
                                          "\r\n         AND RT.実施日 = @売上日" +
                                          // -- ①－２（指定されたコースを持つ注文の得意先）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,コースＣＤ" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         注文ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 喫食日 = @売上日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          // -- ①－３（指定されたコースを持つ売上の得意先）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,コースＣＤ" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         売上ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 売上日 = @売上日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          "\r\n     ) AS C" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                                          "\r\n     LEFT JOIN V_売掛現金区分 AS V ON V.売掛現金区分CD = T.売掛現金区分" +
                                          "\r\n     LEFT JOIN V_売掛現金区分 AS VT ON VT.売掛現金区分CD = @売掛現金区分売掛" +
                                          "\r\n     LEFT JOIN (" +
                                          "\r\n         SELECT" +
                                          "\r\n             H.コースＣＤ," +
                                          "\r\n             T.受注得意先ＣＤ AS 得意先ＣＤ," +
                                          "\r\n             CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ," +
                                          "\r\n             SUM(M.商品金額合計税込) AS 金額" +
                                          "\r\n         FROM" +
                                          "\r\n             売上明細 AS M" +
                                          // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                          // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                                          "\r\n             INNER JOIN (" +
                                          "\r\n                 SELECT" +
                                          "\r\n                     企業ＣＤ" +
                                          "\r\n                     ,得意先ＣＤ" +
                                          "\r\n                     ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ" +
                                          "\r\n                 FROM" +
                                          "\r\n                     得意先マスタ" +
                                          "\r\n                 ) AS T" +
                                          "\r\n                 ON T.企業ＣＤ  = M.企業ＣＤ" +
                                          "\r\n                 AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n             INNER JOIN 売上ヘッダ AS H" +
                                          "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n                 AND H.売上日     = M.売上日" +
                                          "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             M.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n             M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND" +     // -- LINE決済は売掛と見なす
                                          "\r\n             M.売上日   = @売上日 AND" +
                                          "\r\n             M.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n         GROUP BY" +
                                          "\r\n             H.コースＣＤ," +
                                          "\r\n             T.受注得意先ＣＤ," +
                                          "\r\n             M.商品ＣＤ" +
                                          "\r\n         ) AS A ON A.コースＣＤ = C.コースＣＤ AND A.得意先ＣＤ = C.得意先ＣＤ" +
                                          // -- ②コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
                                          "\r\n     LEFT JOIN" +
                                          "\r\n         (SELECT" +
                                          "\r\n             CT.得意先ＣＤ" +
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
                                          "\r\n                     AND コースＣＤ = @コースＣＤ" +
                                          "\r\n                     AND 実施日 = @売上日" +
                                          "\r\n                 ) AS RM" +
                                          "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n             AND CT.コースＣＤ = @コースＣＤ" +
                                          "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                                          "\r\n             AND RM.コースＣＤ IS NULL" +
                                          // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                          "\r\n         UNION" +
                                          "\r\n         SELECT" +
                                          "\r\n             RT.得意先ＣＤ" +
                                          "\r\n             ,RT.コース順" +
                                          "\r\n         FROM" +
                                          "\r\n             臨時コーステーブル AS RT" +
                                          "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                          "\r\n         WHERE" +
                                          "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n             AND RT.コースＣＤ = @コースＣＤ" +
                                          "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                                          "\r\n             AND RT.実施日 = @売上日" +
                                          "\r\n         ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     C.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n     AND C.コースＣＤ = @コースＣＤ" +
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
                                          "\r\n     -1 AS コースに含む順," +
                                          "\r\n     -1 AS コース順" +
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
                                          "\r\n     -1 AS コースに含む順," +
                                          "\r\n     -1 AS コース順" +
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
                        cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _uriageCriteria.売上日;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _uriageCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _uriageCriteria.コースＣＤ;
                        cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                        cmd.Parameters.Add("@売掛現金区分売掛", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Urikake;
                        cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                        cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;
                    }
                    else
                    {
                        // コースが未入力の場合はコーステーブルは見ない
                        cmd.CommandText = // --- 売上現金個数
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
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,M.得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
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
                                          "\r\n         H.売上日 = @売上日 AND" +
                                          "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL" +
                                          "\r\n     ) AS MAIN" +
                                          "\r\n     LEFT JOIN" +
                                          // -- 売上明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
                                          "\r\n     (SELECT" +
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,T.受注得意先ＣＤ AS 得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                          "\r\n         ,SUM(M.商品数量) AS 個数" +
                                          "\r\n     FROM" +
                                          "\r\n         売上明細 AS M" +
                                          // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                          // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                                          "\r\n         INNER JOIN (" +
                                          "\r\n             SELECT" +
                                          "\r\n                 企業ＣＤ" +
                                          "\r\n                 ,得意先ＣＤ" +
                                          "\r\n                 ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ" +
                                          "\r\n             FROM" +
                                          "\r\n                 得意先マスタ" +
                                          "\r\n             ) AS T" +
                                          "\r\n             ON T.企業ＣＤ  = M.企業ＣＤ" +
                                          "\r\n             AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n         INNER JOIN 売上ヘッダ AS H" +
                                          "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n             AND H.売上日     = M.売上日" +
                                          "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n     WHERE" +
                                          "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n         M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND" +      // -- 現金、チケット、バークレー、ボーナスは現金と見なす
                                          "\r\n         M.売上日   = @売上日 AND" +
                                          "\r\n         M.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL" +
                                          "\r\n     GROUP BY" +
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,T.受注得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,M.商品ＣＤ" +
                                          "\r\n     ) AS A" +
                                          "\r\n         ON MAIN.企業ＣＤ    = A.企業ＣＤ" +
                                          "\r\n         AND MAIN.売上日     = A.売上日" +
                                          "\r\n         AND MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                                          "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                                          "\r\n         AND MAIN.部署ＣＤ   = A.部署ＣＤ" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = MAIN.企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
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
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,M.得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
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
                                          "\r\n         H.売上日 = @売上日 AND" +
                                          "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL" +
                                          "\r\n     ) AS MAIN" +
                                          "\r\n     LEFT JOIN" +
                                          // -- 売上明細（現金）（レコードがヒットしなくても得意先名とタイプは表示される）
                                          "\r\n     (SELECT" +
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,T.受注得意先ＣＤ AS 得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                          "\r\n         ,SUM(M.商品金額合計税込) AS 金額" +
                                          "\r\n     FROM" +
                                          "\r\n         売上明細 AS M" +
                                          // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                          // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                                          "\r\n         INNER JOIN (" +
                                          "\r\n             SELECT" +
                                          "\r\n                 企業ＣＤ" +
                                          "\r\n                 ,得意先ＣＤ" +
                                          "\r\n                 ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ" +
                                          "\r\n             FROM" +
                                          "\r\n                 得意先マスタ" +
                                          "\r\n             ) AS T" +
                                          "\r\n             ON T.企業ＣＤ  = M.企業ＣＤ" +
                                          "\r\n             AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n         INNER JOIN 売上ヘッダ AS H" +
                                          "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n             AND H.売上日     = M.売上日" +
                                          "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n     WHERE" +
                                          "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n         M.売上区分 NOT IN (@売上区分売掛, @売上区分LINE) AND" +      // -- 現金、チケット、バークレー、ボーナスは現金と見なす
                                          "\r\n         M.売上日   = @売上日 AND" +
                                          "\r\n         M.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL" +
                                          "\r\n     GROUP BY" +
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,T.受注得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,M.商品ＣＤ" +
                                          "\r\n     ) AS A" +
                                          "\r\n         ON MAIN.企業ＣＤ    = A.企業ＣＤ" +
                                          "\r\n         AND MAIN.売上日     = A.売上日" +
                                          "\r\n         AND MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                                          "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                                          "\r\n         AND MAIN.部署ＣＤ   = A.部署ＣＤ" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = MAIN.企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
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
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,M.得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
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
                                          "\r\n         H.売上日 = @売上日 AND" +
                                          "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL" +
                                          "\r\n     ) AS MAIN" +
                                          "\r\n     LEFT JOIN" +
                                          // -- 売上明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
                                          "\r\n     (SELECT" +
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,T.受注得意先ＣＤ AS 得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                          "\r\n         ,SUM(M.商品数量) AS 個数" +
                                          "\r\n     FROM" +
                                          "\r\n         売上明細 AS M" +
                                          // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                          // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                                          "\r\n         INNER JOIN (" +
                                          "\r\n             SELECT" +
                                          "\r\n                 企業ＣＤ" +
                                          "\r\n                 ,得意先ＣＤ" +
                                          "\r\n                 ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ" +
                                          "\r\n             FROM" +
                                          "\r\n                 得意先マスタ" +
                                          "\r\n             ) AS T" +
                                          "\r\n             ON T.企業ＣＤ  = M.企業ＣＤ" +
                                          "\r\n             AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n         INNER JOIN 売上ヘッダ AS H" +
                                          "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n             AND H.売上日     = M.売上日" +
                                          "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n     WHERE" +
                                          "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n         M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND" +     // -- LINE決済は売掛と見なす
                                          "\r\n         M.売上日   = @売上日 AND" +
                                          "\r\n         M.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL" +
                                          "\r\n     GROUP BY" +
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,T.受注得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,M.商品ＣＤ" +
                                          "\r\n     ) AS A" +
                                          "\r\n         ON MAIN.企業ＣＤ    = A.企業ＣＤ" +
                                          "\r\n         AND MAIN.売上日     = A.売上日" +
                                          "\r\n         AND MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                                          "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                                          "\r\n         AND MAIN.部署ＣＤ   = A.部署ＣＤ" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = MAIN.企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
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
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,M.得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
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
                                          "\r\n         H.売上日 = @売上日 AND" +
                                          "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL" +
                                          "\r\n     ) AS MAIN" +
                                          "\r\n     LEFT JOIN" +
                                          // -- 売上明細（売掛）（レコードがヒットしなくても得意先名とタイプは表示される）
                                          "\r\n     (SELECT" +
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,T.受注得意先ＣＤ AS 得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                          "\r\n         ,SUM(M.商品金額合計税込) AS 金額" +
                                          "\r\n     FROM" +
                                          "\r\n         売上明細 AS M" +
                                          // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                          // -- （分配することによって分配先のコースは分配元と同じになっているので、分配元と分配先が一緒に抽出される）
                                          "\r\n         INNER JOIN (" +
                                          "\r\n             SELECT" +
                                          "\r\n                 企業ＣＤ" +
                                          "\r\n                 ,得意先ＣＤ" +
                                          "\r\n                 ,ISNULL(受注得意先ＣＤ, 得意先ＣＤ) AS 受注得意先ＣＤ" +
                                          "\r\n             FROM" +
                                          "\r\n                 得意先マスタ" +
                                          "\r\n             ) AS T" +
                                          "\r\n             ON T.企業ＣＤ  = M.企業ＣＤ" +
                                          "\r\n             AND T.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n         INNER JOIN 売上ヘッダ AS H" +
                                          "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n             AND H.売上日     = M.売上日" +
                                          "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n     WHERE" +
                                          "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n         M.売上区分 IN (@売上区分売掛, @売上区分LINE) AND" +     // -- LINE決済は売掛と見なす
                                          "\r\n         M.売上日   = @売上日 AND" +
                                          "\r\n         M.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL" +
                                          "\r\n     GROUP BY" +
                                          "\r\n         M.企業ＣＤ" +
                                          "\r\n         ,M.売上日" +
                                          "\r\n         ,T.受注得意先ＣＤ" +
                                          "\r\n         ,M.部署ＣＤ" +
                                          "\r\n         ,M.商品ＣＤ" +
                                          "\r\n     ) AS A" +
                                          "\r\n         ON MAIN.企業ＣＤ    = A.企業ＣＤ" +
                                          "\r\n         AND MAIN.売上日     = A.売上日" +
                                          "\r\n         AND MAIN.得意先ＣＤ = A.得意先ＣＤ" +
                                          "\r\n         AND MAIN.商品ＣＤ   = A.商品ＣＤ" +
                                          "\r\n         AND MAIN.部署ＣＤ   = A.部署ＣＤ" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = MAIN.企業ＣＤ AND T.得意先ＣＤ = MAIN.得意先ＣＤ" +
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
                        cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _uriageCriteria.売上日;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _uriageCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                        cmd.Parameters.Add("@売掛現金区分売掛", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Urikake;
                        cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                        cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    cmd.CommandTimeout = 60;           // あじやでタイムアウトが発生しているので延ばす

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("コース売上一覧を検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("コース売上一覧");
                // 商品名列を作成
                cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["得意先ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["得意先名"]);
                cldt.AddFixedColumn(dt.Columns["表示用得意先名"]);
                cldt.AddFixedColumn(dt.Columns["タイプ"]);
                // インポート
                cldt.Import(dt, "商品ＣＤ", "値");

                //cldtに順を追加
                cldt.AddFixedColumn(dt.Columns.Add("順"));
                foreach(DataRow dr in cldt.Rows)
                {
                    dr["順"] = cldt.Rows.IndexOf(dr) / 2 + 1;
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);

                fpSpread1.SuspendLayout();      // 描画を一時停止

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "コース売上一覧";

                fpSpread1_Sheet1.BindDataColumn(ROWNO_COLUMN, "順");
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
                        fpSpread1_Sheet1.AddSpanCell(rowIndex, 1, 2, 1);
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
                    for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count- 2; rowIndex++)
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
                }

                // 最終２列を固定列にする
                fpSpread1_Sheet1.FrozenTrailingColumnCount = 2;
                fpSpread1_Sheet1.Columns[fpSpread1_Sheet1.Columns.Count - 2].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Columns[fpSpread1_Sheet1.Columns.Count - 1].BackColor = TOTAL_BACK_COLOR;

                // 合計列に数式を設定する
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    int? kingakuTotal = null;
                    // 数式の結果が表示されないので自力で計算（最終列は除く）
                    for (columnIndex = SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX - 2; columnIndex++)
                    {
                        // 個数列は除く
                        if ((columnIndex - SYOHIN_START_COLUMN) % 2 == 0)
                        {
                            continue;
                        }

                        if (fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value != null)
                        {
                            kingakuTotal = (kingakuTotal ?? 0) + (int)(fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value);
                        }
                    }
                    fpSpread1_Sheet1.Cells[rowIndex, MAX_COLUMN_INDEX].Value = kingakuTotal;
                }

                if (fpSpread1_Sheet1.Rows.Count > 2)    // ２行は合計行なので
                {
                    fpSpread1.Focus();
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, SYOHIN_START_COLUMN);
                    fpSpread1_Sheet1.AddSelection(0, SYOHIN_START_COLUMN, 1, 1);
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _uriageCriteriaSnapShot = new CourseUriageListCriteria(_uriageCriteria);
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
        /// 分配入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBunpai_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_uriageCriteriaSnapShot == null)
                {
                    return;
                }

                if (fpSpread1_Sheet1.Rows.Count <= 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowBunpai();       // 分配入力表示（既存レコード）
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
        /// 明細入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMeisai_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_uriageCriteriaSnapShot == null)
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
                    // -- ┌─────┬─────┬─────┬─────┐
                    // -- │  商品Ａ  │  商品Ｂ  │  商品Ｃ  │   合計   │
                    // -- ├──┬──┼──┬──┼──┬──┼──┬──┤
                    // -- │個数│金額│個数│金額│個数│金額│個数│金額│
                    // -- └──┴──┴──┴──┴──┴──┴──┴──┘
                    // -- 商品ＣＤのアンダーバー以降は
                    // -- 0:個数 1:金額

                                    // --①個数
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     0 AS 合計区分" +
                                      "\r\n     ,T.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(VARCHAR, T.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n     ,ISNULL(S.商品名略称, CONVERT(VARCHAR, T.商品ＣＤ)) AS 商品名略称" +
                                      "\r\n     ,'個数' AS 個数金額" +
                                      "\r\n FROM" +
                                      // -- 主要商品リストマスタと売上明細にある商品
                                      "\r\n     (SELECT" +
                                      "\r\n         A.企業ＣＤ," +
                                      "\r\n         A.商品ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         主要商品リストマスタ AS A" +
                                      "\r\n     WHERE" +
                                      "\r\n         A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n         AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n     UNION" +
                                      "\r\n     SELECT" +
                                      "\r\n         M.企業ＣＤ," +
                                      "\r\n         M.商品ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         売上明細 AS M" +
                                      "\r\n     WHERE" +
                                      "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n         M.売上日   = @売上日 AND" +
                                      "\r\n         M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     ) AS T" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = T.企業ＣＤ AND S.商品ＣＤ = T.商品ＣＤ" +
                                      "\r\n UNION" +
                                      // -- ②金額
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 合計区分" +
                                      "\r\n     ,T.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(VARCHAR, T.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n     ,ISNULL(S.商品名略称, CONVERT(VARCHAR, T.商品ＣＤ)) AS 商品名略称" +
                                      "\r\n     ,'金額' AS 個数金額" +
                                      "\r\n FROM" +
                                      // -- 主要商品リストマスタと売上明細にある商品
                                      "\r\n     (SELECT" +
                                      "\r\n         A.企業ＣＤ," +
                                      "\r\n         A.商品ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         主要商品リストマスタ AS A" +
                                      "\r\n     WHERE" +
                                      "\r\n         A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n         AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n     UNION" +
                                      "\r\n     SELECT" +
                                      "\r\n         M.企業ＣＤ," +
                                      "\r\n         M.商品ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         売上明細 AS M" +
                                      "\r\n     WHERE" +
                                      "\r\n         M.企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n         M.売上日   = @売上日 AND" +
                                      "\r\n         M.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     ) AS T" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = T.企業ＣＤ AND S.商品ＣＤ = T.商品ＣＤ" +
                                      "\r\n UNION" +
                                      // -- ③合計個数
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 合計区分" +
                                      "\r\n     ,-1 AS ソート用商品ＣＤ" +
                                      "\r\n     , 'XXX_0' AS 商品ＣＤ" +
                                      "\r\n     ,'合計' AS 商品名略称" +
                                      "\r\n     ,'個数' AS 個数金額" +
                                      // -- ④合計金額
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     1 AS 合計区分" +
                                      "\r\n     ,-1 AS ソート用商品ＣＤ" +
                                      "\r\n     , 'XXX_1' AS 商品ＣＤ" +
                                      "\r\n     ,'合計' AS 商品名略称" +
                                      "\r\n     ,'金額' AS 個数金額" +
                                      "\r\n ORDER BY" +
                                      "\r\n     合計区分" +
                                      "\r\n     ,ソート用商品ＣＤ" +
                                      "\r\n     ,商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _uriageCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _uriageCriteria.売上日;

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
                    if ((columnIndex - SYOHIN_START_COLUMN) % 2 == 0)
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
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex - 1).ColumnSpan = 2;
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex - 1).Value = dr[labelColumnName].ToString();
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex - 1).Value = "個数";
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex).Value = "金額";
                    }
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
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_uriageCriteria.HasErrors)
                {
                    MessageBox.Show(this, _uriageCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                UriageMeisaiCriteria uriageMeisaiCriteria = new UriageMeisaiCriteria();
                uriageMeisaiCriteria.stopOverwrite = true;
                uriageMeisaiCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                uriageMeisaiCriteria.得意先名 = dr["得意先名"] as string;
                uriageMeisaiCriteria.部署ＣＤ = _uriageCriteria.部署ＣＤ;
                uriageMeisaiCriteria.売上日 = _uriageCriteria.売上日;
                uriageMeisaiCriteria.コースＣＤ = _uriageCriteria.コースＣＤ;       // デフォルト表示するコース
                Form401 form = new Form401(uriageMeisaiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form401_FormClosed);
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
        private void Form401_FormClosed(object sender, FormClosedEventArgs e)
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
        /// 売掛分配入力表示（既存レコード）
        /// </summary>
        private void ShowBunpai()
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_uriageCriteria.HasErrors)
                {
                    MessageBox.Show(this, _uriageCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                UrikakeBunpaiCriteria urikakeBunpaiCriteria = new UrikakeBunpaiCriteria();
                urikakeBunpaiCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                urikakeBunpaiCriteria.部署ＣＤ = _uriageCriteria.部署ＣＤ;
                urikakeBunpaiCriteria.売上日 = _uriageCriteria.売上日;
                Form406 form = new Form406(urikakeBunpaiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form406_FormClosed);
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
        /// 売掛分配入力画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form406_FormClosed(object sender, FormClosedEventArgs e)
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
