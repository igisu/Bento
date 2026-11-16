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
    /// 配送予定入力
    /// </summary>
    public partial class Form100 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_CD_COLUMN   = 0;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 1;    // 得意先名列インデックス
        private const int COURSE_CD_COLUMN      = 2;    // コースＣＤ列インデックス
        // これ以降の列は動的に追加される
        private const int SYOHIN_START_COLUMN   = 3;    // 商品名列開始列インデックス

        /// <summary>
        /// 別コースで注文有りの背景色
        /// </summary>
        private readonly Color OTHER_COURSE_BACK_COLOR = Color.Lavender;

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 配送予定入力検索条件
        /// </summary>
        private HaisoYoteiCriteria _haisoYoteiCriteria;

        /// <summary>
        /// 配送予定入力検索条件検索実行時スナップショット
        /// </summary>
        private HaisoYoteiCriteria _haisoYoteiCriteriaSnapShot;

        /// <summary>
        /// 配送予定入力実績読込条件
        /// </summary>
        private HaisoYoteiFromUriage _haisoYoteiFromUriage;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        // Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        private bool _isF2forSpreadEdit = false;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = SYOHIN_START_COLUMN;

        /// <summary>
        /// 列として採用する商品名の一覧（商品ＣＤ、商品名略称）を持つデータテーブル
        /// </summary>
        private DataTable _syohinDt = null;

        /// <summary>
        /// データ本体（注文フラグを含む、cldtにインポートする元ネタ）のデータテーブル
        /// </summary>
        private DataTable _sourceDt = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form100()
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
        private void Form100_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_flatbed_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

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

                //***********************************************************************************
                // Spreadの行のロック(編集不可)を有効にするためにシートのプロテクト(保護)を有効にする
                // ただし、Excel出力するとシートのあらゆる書式や設定が変更できなくなるので、
                // Excel出力の前後でプロテクト状態を切り替える(□)
                //***********************************************************************************
                fpSpread1_Sheet1.Protect = true;

                // スプレッドのコースＣＤ列を非表示にする
                fpSpread1_Sheet1.Columns[COURSE_CD_COLUMN].Visible = false;

                // 動的に追加される列以外を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = SYOHIN_START_COLUMN;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

                // 常に更新
                _IsNewRecord = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _haisoYoteiCriteria = new HaisoYoteiCriteria();
                haisoYoteiBindingSource.DataSource = _haisoYoteiCriteria;

                // 「コースの曜日が注文日と一致しません」ラベルのVisibleを設定
                ToggleUnmatch();

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _haisoYoteiFromUriage = new HaisoYoteiFromUriage();
                haisoYoteiFromUriageBindingSource.DataSource = _haisoYoteiFromUriage;
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
        private void Form100_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F9:                       // 更新ボタン押下
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread1.EditMode == true)
                    {
                        fpSpread1.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    btnSave.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form100_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // このフォーム以外の要因で閉じられようとしている場合は閉じる
                if (e.CloseReason != CloseReason.UserClosing)
                {
                    return;
                }

                // 登録or削除済み？
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                // 初期状態と異なる？
                if (HasChange())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            if (CanSave() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (Save())
                            {
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            }
                            else
                            {
                                e.Cancel = true;
                            }
                            return;

                        case DialogResult.Cancel:
                            e.Cancel = true;
                            return;
                    }
                }
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
        /// 注文日テキスト 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpChumonbi_Validated(object sender, EventArgs e)
        {
            try
            {
                // 「コースの曜日が注文日と一致しません」ラベルのVisibleを設定
                ToggleUnmatch();
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
                    _haisoYoteiCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;

                    // 「コースの曜日が注文日と一致しません」ラベルのVisibleを設定
                    ToggleUnmatch();

                    _haisoYoteiCriteria.部署ＣＤ = course.部署ＣＤ;
                    ReadBinding(cboBusyo);      // コントロールに反映
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
        /// コースＣＤ検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // 「コースの曜日が注文日と一致しません」ラベルのVisibleを設定
                ToggleUnmatch();

                // コースによって決まった部署ＣＤを表示
                cboBusyo.SelectedValue = _haisoYoteiCriteria.部署ＣＤ ?? -1;
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_haisoYoteiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_haisoYoteiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _haisoYoteiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_haisoYoteiCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        _haisoYoteiCriteriaSnapShot = null;
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // Spreadの内部状態が壊れてColumnCountやRowCountを設定するとIndexOutOfRangeExceptionが発生することがあるので、それを回避すべく、DataSourceをリセットする
                bindingSource1.DataSource = null;

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                //***************************************************
                // 主要商品リストマスタにある商品名を列として追加し、
                // コーステーブルにある得意先を行とする
                // データそのものは注文明細から取得する
                //***************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                _sourceDt = new DataTable("注文明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    if (_haisoYoteiCriteria.コースＣＤ != null)
                    {
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n     C.得意先ＣＤ" +
                                          "\r\n     ,T.得意先名" +
                                          "\r\n     ,CASE WHEN A.商品ＣＤ IS NULL THEN C.コースＣＤ ELSE A.コースＣＤ END AS コースＣＤ" +  // -- 駆動表に紐付く予測注文レコードのコースＣＤがNULLの場合はそのまま取得したい
                                          "\r\n     ,A.商品ＣＤ" +
                                          "\r\n     ,A.商品注文数量" +
                                          "\r\n     ,A.商品予測数量" +
                                          "\r\n     ,CASE WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN 1 ELSE 0 END AS 注文フラグ" +
                                          "\r\n     ,CASE WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN A.商品注文数量 ELSE A.商品予測数量 END AS 商品数量" +
                                          "\r\n FROM" +
                                          // -- ①駆動表（指定されたコースにぶら下がる得意先）
                                          // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                          "\r\n     (SELECT" +
                                          "\r\n         CT.企業ＣＤ" +
                                          "\r\n         ,CT.コースＣＤ" +
                                          "\r\n         ,CT.得意先ＣＤ" +
                                          "\r\n         ,CT.コース順" +
                                          "\r\n         ,CT.無効フラグ" +
                                          "\r\n         ,CT.作成担当者ＣＤ" +
                                          "\r\n         ,CT.作成日" +
                                          "\r\n         ,CT.修正担当者ＣＤ" +
                                          "\r\n         ,CT.修正日" +
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
                                          "\r\n                 AND コースＣＤ = @コースＣＤ" +
                                          "\r\n                 AND 実施日 = @注文日" +
                                          "\r\n             ) AS RM" +
                                          "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                                          "\r\n     WHERE" +
                                          "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND CT.コースＣＤ = @コースＣＤ" +
                                          "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                                          "\r\n         AND RM.コースＣＤ IS NULL" +
                                          // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         RT.企業ＣＤ" +
                                          "\r\n         ,RT.コースＣＤ" +
                                          "\r\n         ,RT.得意先ＣＤ" +
                                          "\r\n         ,RT.コース順" +
                                          "\r\n         ,RT.無効フラグ" +
                                          "\r\n         ,RT.作成担当者ＣＤ" +
                                          "\r\n         ,RT.作成日" +
                                          "\r\n         ,RT.修正担当者ＣＤ" +
                                          "\r\n         ,RT.修正日" +
                                          "\r\n     FROM" +
                                          "\r\n         臨時コーステーブル AS RT" +
                                          "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                          "\r\n     WHERE" +
                                          "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND RT.コースＣＤ = @コースＣＤ" +
                                          "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                                          "\r\n         AND RT.実施日 = @注文日" +
                                          "\r\n     ) AS C" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND C.得意先ＣＤ = T.得意先ＣＤ" +
                                          // -- ②駆動表に紐付く予測と注文
                                          // -- コース注文入力とは異なり、コースを無視して(コースの結合はせずに)注文を取得する。ただし、検索条件のコースかどうかを示すためにコースCDは取得する(△)
                                          "\r\n     LEFT JOIN" +
                                          // -- 予測と注文を１行にまとめる
                                          "\r\n        (SELECT" +
                                          "\r\n            B.コースＣＤ" +
                                          "\r\n            ,B.得意先ＣＤ" +
                                          "\r\n            ,B.商品ＣＤ" +
                                          "\r\n            ,SUM(CASE WHEN B.注文区分 = @注文区分注文 THEN B.商品数量 ELSE null END) AS 商品注文数量" +
                                          "\r\n            ,SUM(CASE WHEN B.注文区分 = @注文区分予測 THEN B.商品数量 ELSE null END) AS 商品予測数量" +
                                          "\r\n         FROM" +
                                          // -- ②－１ 予測を取得
                                          "\r\n             (SELECT" +
                                          "\r\n                 H.コースＣＤ" +
                                          "\r\n                 ,H.注文区分" +
                                          "\r\n                 ,M.得意先ＣＤ" +
                                          "\r\n                 ,M.商品ＣＤ" +
                                          "\r\n                 ,M.商品数量" +
                                          "\r\n             FROM" +
                                          "\r\n                 注文ヘッダ AS H" +
                                          "\r\n                 INNER JOIN 注文明細 AS M" +
                                          "\r\n                     ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n                     AND H.注文区分   = M.注文区分" +
                                          "\r\n                     AND H.喫食日     = M.喫食日" +
                                          "\r\n                     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n                     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n             WHERE" +
                                          "\r\n                 H.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n                 AND H.喫食日   = @注文日" +
                                          "\r\n                 AND H.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n                 AND H.注文区分 = @注文区分予測" +
                                          "\r\n                 AND H.コースＣＤ IS NOT NULL" +     // -- コースがある
                                          // -- ②－２ 注文を取得
                                          "\r\n             UNION" +
                                          "\r\n             SELECT" +
                                          "\r\n                 ISNULL(HY.コースＣＤ, @コースＣＤ) AS コースＣＤ" +     // -- コースＣＤは予測のものを優先し、注文から取得する場合は検索条件のコースとする
                                          "\r\n                 ,HC.注文区分" +
                                          "\r\n                 ,M.得意先ＣＤ" +
                                          "\r\n                 ,M.商品ＣＤ" +
                                          "\r\n                 ,M.商品数量" +
                                          "\r\n             FROM" +
                                          "\r\n                 注文ヘッダ AS HC" +
                                          "\r\n                 INNER JOIN 注文明細 AS M" +
                                          "\r\n                     ON  HC.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n                     AND HC.注文区分   = M.注文区分" +
                                          "\r\n                     AND HC.喫食日     = M.喫食日" +
                                          "\r\n                     AND HC.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n                     AND HC.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n                 LEFT JOIN 注文ヘッダ AS HY" +
                                          "\r\n                     ON  HY.企業ＣＤ   = HC.企業ＣＤ" +
                                          "\r\n                     AND HY.喫食日     = HC.喫食日" +
                                          "\r\n                     AND HY.得意先ＣＤ = HC.得意先ＣＤ" +
                                          "\r\n                     AND HY.部署ＣＤ   = HC.部署ＣＤ" +
                                          "\r\n                     AND HY.注文区分   = @注文区分予測" +
                                          "\r\n             WHERE" +
                                          "\r\n                 HC.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n                 AND HC.喫食日   = @注文日" +
                                          "\r\n                 AND HC.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n                 AND HC.注文区分 = @注文区分注文" +
                                          "\r\n             ) AS B" +
                                          "\r\n         GROUP BY" +
                                          "\r\n            B.コースＣＤ" +
                                          "\r\n            ,B.得意先ＣＤ" +
                                          "\r\n            ,B.商品ＣＤ" +
                                          "\r\n         ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ" +
                                          "\r\n ORDER BY" +
                                          "\r\n     C.コース順," +
                                          "\r\n     A.商品ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _haisoYoteiCriteria.注文日;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.コースＣＤ;
                        cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                        cmd.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                    }
                    else
                    {
                        // コースが未入力の場合はコーステーブルは見ない
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n     C.得意先ＣＤ" +
                                          "\r\n     ,T.得意先名" +
                                          "\r\n     ,A.コースＣＤ" +
                                          "\r\n     ,A.商品ＣＤ" +
                                          "\r\n     ,A.商品注文数量" +
                                          "\r\n     ,A.商品予測数量" +
                                          "\r\n     ,CASE WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN 1 ELSE 0 END AS 注文フラグ" +
                                          "\r\n     ,CASE WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN A.商品注文数量 ELSE A.商品予測数量 END AS 商品数量" +
                                          "\r\n FROM" +
                                          // -- ①駆動表（コースがNULLの予測）
                                          "\r\n     (SELECT" +
                                          "\r\n         得意先ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         注文ヘッダ" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND 喫食日   = @注文日" +
                                          "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                          "\r\n         AND コースＣＤ IS NULL" +       // -- コースが無い
                                          "\r\n         AND 注文区分 = @注文区分予測" +
                                          "\r\n     ) AS C" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND C.得意先ＣＤ = T.得意先ＣＤ" +
                                          // -- ②駆動表に紐付く予測と注文
                                          // -- 予測と注文を１行にまとめる
                                          "\r\n     LEFT JOIN" +
                                          "\r\n        (SELECT" +
                                          "\r\n            B.コースＣＤ" +
                                          "\r\n            ,B.得意先ＣＤ" +
                                          "\r\n            ,B.商品ＣＤ" +
                                          "\r\n            ,SUM(CASE WHEN B.注文区分 = @注文区分注文 THEN B.商品数量 ELSE null END) AS 商品注文数量" +
                                          "\r\n            ,SUM(CASE WHEN B.注文区分 = @注文区分予測 THEN B.商品数量 ELSE null END) AS 商品予測数量" +
                                          "\r\n         FROM" +
                                          // -- ②－１ 予測を取得
                                          "\r\n             (SELECT" +
                                          "\r\n                 NULL AS コースＣＤ" +
                                          "\r\n                 ,H.注文区分" +
                                          "\r\n                 ,M.得意先ＣＤ" +
                                          "\r\n                 ,M.商品ＣＤ" +
                                          "\r\n                 ,M.商品数量" +
                                          "\r\n             FROM" +
                                          "\r\n                 注文ヘッダ AS H" +
                                          "\r\n                 INNER JOIN 注文明細 AS M" +
                                          "\r\n                     ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n                     AND H.注文区分   = M.注文区分" +
                                          "\r\n                     AND H.喫食日     = M.喫食日" +
                                          "\r\n                     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n                     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n             WHERE" +
                                          "\r\n                 H.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n                 AND H.喫食日   = @注文日" +
                                          "\r\n                 AND H.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n                 AND H.注文区分 = @注文区分予測" +
                                          "\r\n                 AND H.コースＣＤ IS NULL" +     // -- コースが無い
                                          // -- ②－２ 注文を取得
                                          "\r\n             UNION" +
                                          "\r\n             SELECT" +
                                          "\r\n                 HY.コースＣＤ" +        // -- 注文に紐付く予測がある場合は、その予測のコースＣＤを取得
                                          "\r\n                 ,HC.注文区分" +
                                          "\r\n                 ,M.得意先ＣＤ" +
                                          "\r\n                 ,M.商品ＣＤ" +
                                          "\r\n                 ,M.商品数量" +
                                          "\r\n             FROM" +
                                          "\r\n                 注文ヘッダ AS HC" +
                                          "\r\n                 INNER JOIN 注文明細 AS M" +
                                          "\r\n                     ON  HC.企業ＣＤ   = M.企業ＣＤ" +
                                          "\r\n                     AND HC.注文区分   = M.注文区分" +
                                          "\r\n                     AND HC.喫食日     = M.喫食日" +
                                          "\r\n                     AND HC.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n                     AND HC.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n                 LEFT JOIN 注文ヘッダ AS HY" +
                                          "\r\n                     ON  HY.企業ＣＤ   = HC.企業ＣＤ" +
                                          "\r\n                     AND HY.喫食日     = HC.喫食日" +
                                          "\r\n                     AND HY.得意先ＣＤ = HC.得意先ＣＤ" +
                                          "\r\n                     AND HY.部署ＣＤ   = HC.部署ＣＤ" +
                                          "\r\n                     AND HY.注文区分   = @注文区分予測" +
                                          "\r\n             WHERE" +
                                          "\r\n                 HC.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n                 AND HC.喫食日   = @注文日" +
                                          "\r\n                 AND HC.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n                 AND HC.注文区分 = @注文区分注文" +
                                          "\r\n             ) AS B" +
                                          "\r\n         GROUP BY" +
                                          "\r\n            B.コースＣＤ" +
                                          "\r\n            ,B.得意先ＣＤ" +
                                          "\r\n            ,B.商品ＣＤ" +
                                          "\r\n         ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ" +
                                          "\r\n ORDER BY" +
                                          "\r\n     A.得意先ＣＤ," +
                                          "\r\n     A.商品ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _haisoYoteiCriteria.注文日;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                        cmd.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(_sourceDt);

                    log.Info("配送予定入力リスト（予測・注文）を検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("配送予定入力");
                // 商品名列を作成
                cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(_sourceDt.Columns["得意先ＣＤ"]);
                cldt.AddFixedColumn(_sourceDt.Columns["得意先名"]);
                cldt.AddFixedColumn(_sourceDt.Columns["コースＣＤ"]);
                // インポート
                cldt.Import(_sourceDt, "商品ＣＤ", "商品数量");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);
                cldt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                fpSpread1.SuspendLayout();      // 描画を一時停止

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "配送予定入力";

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", bindingSource1.Count);

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");

                // 商品名を列名としてバインド
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // スプレッドの文字色を以下のように変更
                // 予測・実績：黒
                // 注文：赤
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    // ついでにコースが異なる場合の背景色をセット(△)
                    if (_haisoYoteiCriteria.コースＣＤ != fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].Value as int?)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = OTHER_COURSE_BACK_COLOR;

                        // レコードを取得してエラーを設定
                        DataRow dr = cldt.Rows[rowIndex];
                        dr.SetColumnError("得意先名", "この得意先の注文は別のコースで登録されています。");
                    }

                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();   // 黒に戻す
                    for (columnIndex = SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        // スプレッドの各行の先頭列から得意先ＣＤを取得
                        //                     各列の商品ＣＤは商品名一覧のデータテーブルから取得
                        DataRow[] drs = _sourceDt.Select(string.Format("得意先ＣＤ={0} and 商品ＣＤ={1}",
                                                    fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_CD_COLUMN].Value,
                                                    _syohinDt.Rows[columnIndex - SYOHIN_START_COLUMN]["商品ＣＤ"]));
                        if (drs != null && drs.Length != 0)
                        {
                            // 注文明細の注文フラグを取得
                            if ((int)(drs[0]["注文フラグ"]) == 1)
                            {
                                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = System.Drawing.Color.Red;
                                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Locked = true;        // 入力不可にする(□)
                            }
                        }
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, SYOHIN_START_COLUMN);
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _haisoYoteiCriteriaSnapShot = new HaisoYoteiCriteria(_haisoYoteiCriteria);
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
            finally
            {
                fpSpread1.ResumeLayout(true);       // 描画再開
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 実績読込ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_haisoYoteiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_haisoYoteiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _haisoYoteiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 実績読込条件の日付のエラー状態を更新
                _haisoYoteiFromUriage.売上日 = _haisoYoteiFromUriage.売上日;

                // 実績読込条件の入力にエラーがある？
                if (_haisoYoteiFromUriage.HasErrors)
                {
                    if (string.IsNullOrEmpty(_haisoYoteiFromUriage.Error) == false)
                    {
                        MessageBox.Show(this, _haisoYoteiFromUriage.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // Spreadの内部状態が壊れてColumnCountやRowCountを設定するとIndexOutOfRangeExceptionが発生することがあるので、それを回避すべく、DataSourceをリセットする
                bindingSource1.DataSource = null;

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                //***************************************************
                // 主要商品リストマスタにある商品名を列として追加し、
                // コーステーブルにある得意先を行とする
                // データそのものは売上明細から取得する
                //***************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                _sourceDt = new DataTable("商品明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    if (_haisoYoteiCriteria.コースＣＤ != null)
                    {
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n     C.得意先ＣＤ" +
                                          "\r\n     ,T.得意先名" +
                                          "\r\n     ,NULL AS コースＣＤ" +
                                          "\r\n     ,A.商品ＣＤ" +
                                          "\r\n     ,0 AS 注文フラグ" +
                                          "\r\n     ,A.商品数量" +
                                          "\r\n FROM" +
                                          // -- ①駆動表
                                          // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                          "\r\n     (SELECT" +
                                          "\r\n         CT.企業ＣＤ" +
                                          "\r\n         ,CT.コースＣＤ" +
                                          "\r\n         ,CT.得意先ＣＤ" +
                                          "\r\n         ,CT.コース順" +
                                          "\r\n         ,CT.無効フラグ" +
                                          "\r\n         ,CT.作成担当者ＣＤ" +
                                          "\r\n         ,CT.作成日" +
                                          "\r\n         ,CT.修正担当者ＣＤ" +
                                          "\r\n         ,CT.修正日" +
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
                                          "\r\n                 AND コースＣＤ = @コースＣＤ" +
                                          "\r\n                 AND 実施日 = @注文日" +
                                          "\r\n             ) AS RM" +
                                          "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                                          "\r\n     WHERE" +
                                          "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND CT.コースＣＤ = @コースＣＤ" +
                                          "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                                          "\r\n         AND RM.コースＣＤ IS NULL" +
                                          // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         RT.企業ＣＤ" +
                                          "\r\n         ,RT.コースＣＤ" +
                                          "\r\n         ,RT.得意先ＣＤ" +
                                          "\r\n         ,RT.コース順" +
                                          "\r\n         ,RT.無効フラグ" +
                                          "\r\n         ,RT.作成担当者ＣＤ" +
                                          "\r\n         ,RT.作成日" +
                                          "\r\n         ,RT.修正担当者ＣＤ" +
                                          "\r\n         ,RT.修正日" +
                                          "\r\n     FROM" +
                                          "\r\n         臨時コーステーブル AS RT" +
                                          "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                          "\r\n     WHERE" +
                                          "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND RT.コースＣＤ = @コースＣＤ" +
                                          "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                                          "\r\n         AND RT.実施日 = @注文日" +
                                          "\r\n     ) AS C" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND C.得意先ＣＤ = T.得意先ＣＤ" +
                                          // -- ②駆動表に紐付く売上（売上のコースは見ない）
                                          "\r\n     LEFT JOIN (" +
                                          "\r\n         SELECT" +
                                          "\r\n             D.得意先ＣＤ," +
                                          "\r\n             D.商品ＣＤ," +
                                          "\r\n             SUM(D.商品数量) AS 商品数量" +
                                          "\r\n         FROM" +
                                          "\r\n             売上ヘッダ AS H" +
                                          "\r\n             INNER JOIN 売上明細 AS D" +
                                          "\r\n                 ON  H.企業ＣＤ   = D.企業ＣＤ" +
                                          "\r\n                 AND H.売上日     = D.売上日" +
                                          "\r\n                 AND H.得意先ＣＤ = D.得意先ＣＤ" +
                                          "\r\n                 AND H.部署ＣＤ   = D.部署ＣＤ" +
                                          "\r\n         WHERE" +
                                          "\r\n             H.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n             H.売上日   = @売上日 AND" +
                                          "\r\n             H.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n             ISNULL(D.現金補填フラグ, 0) = 0" +  // -- このフラグが立っているのは厳密に言うと商品の売上ではないので除外する
                                          "\r\n         GROUP BY" +
                                          "\r\n             D.得意先ＣＤ," +
                                          "\r\n             D.商品ＣＤ" +
                                          "\r\n         ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     C.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n     AND ISNULL(C.無効フラグ, 0) = 0" +
                                          "\r\n     AND C.コースＣＤ = @コースＣＤ" +
                                          "\r\n ORDER BY" +
                                          "\r\n     C.コース順," +
                                          "\r\n     A.商品ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _haisoYoteiCriteria.注文日;
                        cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _haisoYoteiFromUriage.売上日;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.コースＣＤ;
                    }
                    else
                    {
                        // コースが未入力の場合はコーステーブルは見ない
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n     A.得意先ＣＤ" +
                                          "\r\n     ,A.得意先名" +
                                          "\r\n     ,NULL AS コースＣＤ" +
                                          "\r\n     ,A.商品ＣＤ" +
                                          "\r\n     ,0 AS 注文フラグ" +
                                          "\r\n     ,A.商品数量" +
                                          "\r\n FROM" +
                                          "\r\n     (SELECT" +
                                          "\r\n         D.得意先ＣＤ," +
                                          "\r\n         H.得意先名," +
                                          "\r\n         D.商品ＣＤ," +
                                          "\r\n         SUM(D.商品数量) AS 商品数量" +
                                          "\r\n     FROM" +
                                          "\r\n         売上ヘッダ AS H " +
                                          "\r\n         INNER JOIN 売上明細 AS D" +
                                          "\r\n             ON  D.企業ＣＤ   = H.企業ＣＤ " +
                                          "\r\n             AND D.売上日     = H.売上日 " +
                                          "\r\n             AND D.得意先ＣＤ = H.得意先ＣＤ " +
                                          "\r\n             AND D.部署ＣＤ   = H.部署ＣＤ " +
                                          "\r\n     WHERE" +
                                          "\r\n         H.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n         H.売上日   = @売上日 AND" +
                                          "\r\n         H.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         H.コースＣＤ IS NULL AND" +
                                          "\r\n         ISNULL(D.現金補填フラグ, 0) = 0" +      // -- このフラグが立っているのは厳密に言うと商品の売上ではないので除外する
                                          "\r\n     GROUP BY" +
                                          "\r\n         D.得意先ＣＤ," +
                                          "\r\n         H.得意先名," +
                                          "\r\n         D.商品ＣＤ" +
                                          "\r\n     ) AS A" +
                                          "\r\n ORDER BY" +
                                          "\r\n     A.得意先ＣＤ," +
                                          "\r\n     A.商品ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _haisoYoteiFromUriage.売上日;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.部署ＣＤ;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(_sourceDt);

                    log.Info("配送予定入力リスト（実績）を検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("配送予定入力");
                // 商品名列を作成
                cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(_sourceDt.Columns["得意先ＣＤ"]);
                cldt.AddFixedColumn(_sourceDt.Columns["得意先名"]);
                cldt.AddFixedColumn(_sourceDt.Columns["コースＣＤ"]);
                // インポート
                cldt.Import(_sourceDt, "商品ＣＤ", "商品数量");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);
                cldt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                fpSpread1.SuspendLayout();      // 描画を一時停止

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "配送予定入力";

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", bindingSource1.Count);

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");

                // 商品名を列名としてバインド
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // スプレッドの文字色を以下のように変更
                // 予測・実績：黒
                // 注文：赤
                // 元々黒なので何もしない。
                // 予測から取得したわけではないので、コースが異なる場合の背景色もセットしない

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, SYOHIN_START_COLUMN);
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _haisoYoteiCriteriaSnapShot = new HaisoYoteiCriteria(_haisoYoteiCriteria);
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
            finally
            {
                fpSpread1.ResumeLayout(true);       // 描画再開
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                _isDtColumnChanging = true;

                // 特に何もしない
                // 将来必要になったときのためにメソッドだけ用意している
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
            finally
            {
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread1.EditMode)
                {
                    fpSpread1.CancelCellEditing();
                }
                else
                {
                    fpSpread1.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= MAX_COLUMN_INDEX)
                    {
                        newColumnIndex = SYOHIN_START_COLUMN;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread1_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread1_Sheet1.ActiveColumnIndex)
                {
                    case TOKUISAKI_CD_COLUMN:   // 得意先ＣＤ
                    case TOKUISAKI_NAME_COLUMN: // 得意先名
                    case COURSE_CD_COLUMN:      // コースＣＤ
                        colIndex = -1;
                        break;

                    default:
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                        break;
                }

                if (colIndex >= 0)
                {
                    // Locked=true なセルについては、その値を消さないようにする(□)
                    if (fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Locked == false)
                    {
                        fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                    }
                }
            }
        }

        /// <summary>
        /// SPREAD編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread1.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread1.EditingControl.KeyDown += new KeyEventHandler(fpSpread1Edit_KeyDown);
            }
        }

        /// <summary>
        /// SPREAD編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread1.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread1, fpSpread1_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread1.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread1Edit_KeyDown);
            }
        }

        /// <summary>
        /// Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1Edit_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合も呼ばれるので、その場合は何もしない
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread1.EditMode)
                    {
                        fpSpread1.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread1.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= MAX_COLUMN_INDEX)
                    {
                        newColumnIndex = SYOHIN_START_COLUMN;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread1_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // セルの文字色を黒(デフォルト)に戻す
            fpSpread1_Sheet1.Cells[e.Row, e.Column].ResetForeColor();

            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)bindingSource1.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 更新ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CanSave() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                if (Save())
                {
                    // 再読み込み
                    Reload();
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
                fpSpread1_Sheet1.Protect = false;       // シートのプロテクトを解除する(□)

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
            catch (BentoException ex)
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
            finally
            {
                fpSpread1_Sheet1.Protect = true;        // シートのプロテクトを有効にする(□)
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
        /// 「コースの曜日が注文日と一致しません」ラベルのVisibleを設定
        /// </summary>
        private void ToggleUnmatch()
        {
            lblUnmatch.Visible = _haisoYoteiCriteria.コース区分注文日不一致フラグ;
        }

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
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.部署ＣＤ;

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
                    numberCellType.MinimumValue = 0D;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).Label = dr[labelColumnName].ToString();
                    fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
                    columnIndex++;
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
        /// 配送予定入力リストが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 配送予定入力リストを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["配送予定入力"];

            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                    case DataRowState.Modified:     // 変更
                    case DataRowState.Deleted:      // 削除
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSave()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索条件の入力にエラーがある？
                if (_haisoYoteiCriteria.HasErrors)
                {
                    MessageBox.Show(this, _haisoYoteiCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (_haisoYoteiCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_haisoYoteiCriteria.Equals(_haisoYoteiCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                return true;
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
        /// DB保存
        /// </summary>
        /// <param name="shain">社員</param>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                bool? ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;
                DateTime kissyokubi = (DateTime)_haisoYoteiCriteria.注文日;
                Syohizei syohizei = Syohizei.GetSyohizei(kissyokubi);   // 消費税

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        //****************************************************************************
                        // ・cldtの行をループしながら以下を実施
                        //   ・得意先に該当する予測レコードを全て取得
                        //   ・得意先に該当する注文明細を全て削除
                        //   ・cldtの行（得意先）の、注文ヘッダを取得
                        //   ・商品一覧データテーブルをループしつつ商品ＣＤを取得
                        //   ・cldtの行（得意先）の、商品ＣＤに該当する列の値を取得（ただし注文フラグがONの場合は予測の値を取得）
                        //   ・注文明細を追加
                        //   ・注文ヘッダが有れば更新、無ければ追加
                        // ・最後に、注文明細がない注文ヘッダを削除
                        //****************************************************************************

                        //***************************************************************************
                        // cldtの行をループしつつ、商品一覧データテーブルをループしつつ商品ＣＤを取得
                        //***************************************************************************
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["配送予定入力"];

                        for (int rowIndex = 0; rowIndex < cldt.Rows.Count; rowIndex++)
                        {
                            int tokuisakiCD = (int)cldt.Rows[rowIndex]["得意先ＣＤ"];   // 行に該当する得意先ＣＤを取得
                            Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki(tokuisakiCD);  // 得意先マスタを取得

                            //*****************************
                            // 注文明細の予測レコードを取得
                            //*****************************
                            DataTable dtYosoku = GetChumonMeisai(tokuisakiCD);

                            //*******************
                            // 注文明細を全て削除
                            //*******************
                            SqlCommand cmdDelM = con.CreateCommand();
                            cmdDelM.CommandText = "\r\n DELETE M" +
                                                  "\r\n FROM 注文明細 AS M" +
                                                  "\r\n INNER JOIN 注文ヘッダ AS H" +
                                                  "\r\n     ON H.企業ＣＤ    = M.企業ＣＤ" +
                                                  "\r\n     AND H.注文区分   = M.注文区分" +
                                                  "\r\n     AND H.喫食日     = M.喫食日" +
                                                  "\r\n     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                                  "\r\n     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                                  "\r\n WHERE M.企業ＣＤ = @企業ＣＤ" +
                                                  "\r\n   AND M.注文区分 = @注文区分予測" +
                                                  "\r\n   AND M.喫食日   = @注文日" +
                                                  "\r\n   AND M.得意先ＣＤ = @得意先ＣＤ" +
                                                  "\r\n   AND M.部署ＣＤ = @部署ＣＤ";

                            cmdDelM.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                            cmdDelM.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                            cmdDelM.Parameters.Add("@注文日", SqlDbType.Date).Value = kissyokubi;
                            cmdDelM.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                            cmdDelM.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.部署ＣＤ;
                            cmdDelM.Transaction = tran;
                            log.Debug(cmdDelM.CommandTextWithParameters());
                            cmdDelM.ExecuteNonQuery();

                            int seq = 0;

                            // 注文ヘッダ更新フラグを初期化
                            bool needChumonHeaderUpdate = false;

                            //*****************
                            // 注文ヘッダを取得
                            //*****************
                            ChumonHeader chumonHeader = ChumonHeader.GetChumonHeader(Consts.ChumonKubun.Yosoku, kissyokubi, tokuisakiCD, (int)_haisoYoteiCriteria.部署ＣＤ);
                            if (chumonHeader == null)
                            {
                                chumonHeader = new ChumonHeader();
                                chumonHeader.注文区分 = (int)Consts.ChumonKubun.Yosoku;
                                chumonHeader.喫食日 = kissyokubi;
                                chumonHeader.得意先ＣＤ = tokuisakiCD;
                                chumonHeader.部署ＣＤ = _haisoYoteiCriteria.部署ＣＤ;
                                chumonHeader.配送日 = kissyokubi;
                                chumonHeader.納品書伝票番号 = null;
                                chumonHeader.備考1 = null;
                                chumonHeader.備考2 = null;
                                chumonHeader.備考3 = null;
                                chumonHeader.備考4 = null;
                                chumonHeader.備考5 = null;

                                if (tokuisaki != null)
                                {
                                    chumonHeader.得意先名 = tokuisaki.得意先名;
                                    chumonHeader.配送先ＣＤ = tokuisaki.受注得意先ＣＤ;
                                    chumonHeader.税区分 = tokuisaki.税区分 ?? (int)Syohizei.ZeiKubun.Uchi;
                                    chumonHeader.丸め処理区分 = tokuisaki.丸め処理区分 ?? (int)Rounding.HasuKubun.Floor;
                                    chumonHeader.備考1 = tokuisaki.備考１;
                                    chumonHeader.備考2 = tokuisaki.備考２;
                                    chumonHeader.備考3 = tokuisaki.備考３;
                                }
                            }
                            chumonHeader.担当者ＣＤ = _haisoYoteiCriteria.担当者ＣＤ;   // コースの配送担当者
                            chumonHeader.コースＣＤ = _haisoYoteiCriteria.コースＣＤ;

                            //***************************************************
                            // 商品一覧データテーブルをループしつつ商品ＣＤを取得
                            //***************************************************
                            foreach (DataRow syohinDr in _syohinDt.Rows)
                            {
                                int syohinCD = (int)syohinDr["商品ＣＤ"];               // 列に該当する商品ＣＤを取得

                                //*****************************************************
                                // cldtの行（得意先）の、商品ＣＤに該当する列の値を取得
                                //*****************************************************
                                int? suryo = (int?)cldt[rowIndex, syohinCD];

                                // 該当する予測レコードを予め取得
                                DataRow[] drsYosoku = dtYosoku.Select(string.Format("喫食日=#{0:yyyy/MM/dd}# and 商品ＣＤ={1}", kissyokubi, syohinCD));

                                // 注文フラグがONの場合は注文の値が表示されているので、それは採用せずに予測の値を再び登録する
                                DataRow[] drs = _sourceDt.Select(string.Format("得意先ＣＤ={0} and 商品ＣＤ={1}", tokuisakiCD, syohinCD));
                                if (drs != null && drs.Length != 0)
                                {
                                    // (予測ではなく)注文の数量を表示している？
                                    if ((int)(drs[0]["注文フラグ"]) == 1)
                                    {
                                        suryo = null;       // 注文の値は採用しない
                                        // 既存の注文明細(予測)があれば、その値を採用する
                                        if (drsYosoku != null && drsYosoku.Length != 0)
                                        {
                                            decimal? yosokuSuryo = drsYosoku[0]["商品数量"] as decimal?;
                                            suryo = (int?)yosokuSuryo;
                                        }
                                    }
                                }

                                if (suryo == null)
                                {
                                    // 既存の注文明細が存在していれば、予測を削除しようとしていることなので、注文ヘッダ（の修正日）を更新する(フラグを立てる)
                                    if (drsYosoku != null && drsYosoku.Length != 0)
                                    {
                                        needChumonHeaderUpdate = true;
                                    }
                                    continue;
                                }

                                // 注文明細を追加または更新することが確定したので、既存の注文明細が存在しないか異なっていれば、注文ヘッダ（の修正日）を更新する(フラグを立てる)
                                if (drsYosoku == null || drsYosoku.Length == 0)
                                {
                                    // 存在しないので追加しようとしている
                                    needChumonHeaderUpdate = true;
                                }
                                else
                                {
                                    // 存在していて、数量が異なる？
                                    if (((decimal)drsYosoku[0]["商品数量"]) != (decimal)suryo)
                                    {
                                        needChumonHeaderUpdate = true;
                                    }
                                }

                                //***************
                                // 注文明細を追加
                                //***************
                                // 日配品の登録を主とするので、製造日＝配送日＝喫食日
                                ChumonMeisai chumonMeisai = new ChumonMeisai();
                                chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Yosoku;
                                chumonMeisai.喫食日 = kissyokubi;
                                chumonMeisai.得意先ＣＤ = tokuisakiCD;
                                chumonMeisai.部署ＣＤ = _haisoYoteiCriteria.部署ＣＤ;
                                chumonMeisai.商品ＣＤ = syohinCD;
                                seq++;
                                chumonMeisai.明細番号 = seq;
                                chumonMeisai.製造日 = kissyokubi;
                                chumonMeisai.商品数量 = suryo;

                                // 商品マスタから
                                Syohin syohin = Syohin.GetSyohin(syohinCD);
                                if (syohin != null)
                                {
                                    chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, syohin.商品単価税込, syohin.商品単価税抜);
                                }

                                // 得意先単価マスタから
                                TokuisakiTanka tokuisakiTanka = TokuisakiTanka.GetTokuisakiTanka(tokuisakiCD, syohinCD);
                                if (tokuisakiTanka != null)
                                {
                                    chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, tokuisakiTanka.商品単価税込, tokuisakiTanka.商品単価税抜);
                                }

                                // 得意先マスタから
                                if (tokuisaki != null)
                                {
                                    chumonMeisai.売掛現金区分 = tokuisaki.売掛現金区分;
                                }

                                // 主要商品リストマスタから
                                SyuyoSyohin syuyoSyohin = SyuyoSyohin.GetSyuyoSyohin((int)_haisoYoteiCriteria.部署ＣＤ, syohinCD);
                                if (syuyoSyohin != null)
                                {
                                    chumonMeisai.商品グループＣＤ = syuyoSyohin.商品グループＣＤ;
                                }

                                // 商品金額や税額を算出
                                chumonMeisai.商品金額合計 = Syohizei.GetAmount(chumonMeisai.商品数量, chumonMeisai.商品単価);
                                chumonMeisai.消費税額 = syohizei.GetTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, syohin.軽減税率フラグ, chumonHeader.丸め処理区分);
                                chumonMeisai.商品金額合計税込 = syohizei.GetAmountIncludeTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, syohin.軽減税率フラグ, chumonHeader.丸め処理区分);
                                chumonMeisai.計算税率 = syohizei.GetTaxRate(syohin.軽減税率フラグ);

                                ret = chumonMeisai.Insert(con, tran);
                                if (ret == false)
                                {
                                    MessageBox.Show(this, chumonMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                historyProcess = History.ProcessType.Create;
                                History.Save(con, tran, this.Text, "注文明細", chumonMeisai.Code, historyProcess);
                            }

                            // 注文明細を追加していれば、注文ヘッダを追加or更新する
                            if (seq != 0)
                            {
                                // ****************************************
                                // 注文ヘッダを更新。新規作成していれば追加
                                // ****************************************
                                if (chumonHeader.作成日 == null)
                                {
                                    historyProcess = History.ProcessType.Create;
                                    ret = chumonHeader.Insert(con, tran);
                                }
                                else
                                {
                                    // 注文ヘッダ更新フラグに従って更新
                                    if (needChumonHeaderUpdate)
                                    {
                                        historyProcess = History.ProcessType.Update;
                                        ret = chumonHeader.Update(con, tran);
                                    }
                                    else
                                    {
                                        continue;       // 全く変化がないので何もしない
                                    }
                                }

                                if (ret == false)
                                {
                                    MessageBox.Show(this, chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                History.Save(con, tran, this.Text, "注文ヘッダ", chumonHeader.Code, historyProcess);
                            }
                        }

                        //*************************************
                        // 注文明細が存在しない注文ヘッダを削除
                        //*************************************
                        SqlCommand cmdDelH = con.CreateCommand();
                        cmdDelH.CommandText = "\r\n DELETE 注文ヘッダ" +
                                              "\r\n FROM" +
                                              "\r\n     注文ヘッダ AS H" +
                                              "\r\n     LEFT JOIN 注文明細 AS M" +
                                              "\r\n     ON H.企業ＣＤ    = M.企業ＣＤ" +
                                              "\r\n     AND H.注文区分   = M.注文区分" +
                                              "\r\n     AND H.喫食日     = M.喫食日" +
                                              "\r\n     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                              "\r\n     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                              "\r\n WHERE" +
                                              "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                                              "\r\n     AND H.注文区分 = @注文区分" +
                                              "\r\n     AND H.喫食日 = @注文日" +
                                              "\r\n     AND H.部署ＣＤ = @部署ＣＤ" +
                                              "\r\n     AND M.企業ＣＤ IS NULL";
                        cmdDelH.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmdDelH.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                        cmdDelH.Parameters.Add("@注文日", SqlDbType.Date).Value = _haisoYoteiCriteria.注文日;
                        cmdDelH.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.部署ＣＤ;
                        cmdDelH.Transaction = tran;
                        log.Debug(cmdDelH.CommandTextWithParameters());
                        cmdDelH.ExecuteNonQuery();

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("配送予定入力を{0}しました。", historyProcess.DisplayName());
                        MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (BentoException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        log.Error(ex);
                        History.Error(ex);
                        throw new BentoException(ex);
                    }
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
        /// 注文明細レコード取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <returns>注文明細レコードを格納したデータテーブル</returns>
        private DataTable GetChumonMeisai(int tokuisakiCD)
        {
            try
            {
                DataTable dt = new DataTable("注文明細予測");

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     M.企業ＣＤ" +
                                      "\r\n     ,M.注文区分" +
                                      "\r\n     ,M.喫食日" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,M.商品ＣＤ" +
                                      "\r\n     ,M.部署ＣＤ" +
                                      "\r\n     ,M.明細番号" +
                                      "\r\n     ,M.製造日" +
                                      "\r\n     ,M.商品グループＣＤ" +
                                      "\r\n     ,M.売掛現金区分" +
                                      "\r\n     ,M.商品単価" +
                                      "\r\n     ,M.商品数量" +
                                      "\r\n     ,M.商品金額合計" +
                                      "\r\n     ,M.消費税額" +
                                      "\r\n     ,M.商品金額合計税込" +
                                      "\r\n     ,M.計算税率" +
                                      "\r\n     ,M.作成担当者ＣＤ" +
                                      "\r\n     ,M.作成日" +
                                      "\r\n     ,M.修正担当者ＣＤ" +
                                      "\r\n     ,M.修正日" +
                                      "\r\n FROM 注文明細 AS M" +
                                      "\r\n INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n     ON H.企業ＣＤ    = M.企業ＣＤ" +
                                      "\r\n     AND H.注文区分   = M.注文区分" +
                                      "\r\n     AND H.喫食日     = M.喫食日" +
                                      "\r\n     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                      "\r\n WHERE M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n   AND M.注文区分 = @注文区分予測" +
                                      "\r\n   AND M.喫食日 = @喫食日" +
                                      "\r\n   AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n   AND M.部署ＣＤ = @部署ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                    cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _haisoYoteiCriteria.注文日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _haisoYoteiCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("注文明細予測を検索しました。");
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

        /// <summary>
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                btnQuery.PerformClick();        // 検索ボタン押下;

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
