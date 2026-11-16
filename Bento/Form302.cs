using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// コース臨時組み替え
    /// </summary>
    public partial class Form302 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int COURSE_CD_COLUMN   = 0;   // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN = 1;   // コース名列インデックス
        private const int RINJI_FLG_COLUMN   = 2;   // 臨時コース有無フラグ列インデックス

        private const int COURSE_JUN_INDEX     = 0; // コース順列インデックス
        private const int TOKUISAKI_NAME_INDEX = 1; // 得意先名列インデックス
        private const int TOKUISAKI_CD_INDEX   = 2; // 得意先ＣＤ列インデックス

        #endregion

        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体

        /// <summary>
        /// 組み替え対象コースが持つコントロール群
        /// </summary>
        private class TargetCourse
        {
            /// <summary>
            /// 対象コースＣＤ
            /// </summary>
            public int? コースＣＤ { get; set; }

            /// <summary>
            /// コースマスタのバインディングソース
            /// </summary>
            public BindingSource CourseMasterBindingSource { get; set; }

            /// <summary>
            /// コーステーブルのバインディングソース
            /// </summary>
            public BindingSource CourseTableBindingSource { get; set; }

            /// <summary>
            /// 臨時コースマスタのバインディングソース
            /// </summary>
            public BindingSource RinjiCourseMasterBindingSource { get; set; }

            /// <summary>
            /// 臨時コーステーブルのバインディングソース
            /// </summary>
            public BindingSource RinjiCourseTableBindingSource { get; set; }

            /// <summary>
            /// 初期表示用臨時コースマスタ
            /// </summary>
            public RinjiCourse RinjiCourseOrigin { get; set; }

            /// <summary>
            /// 組み替え対象コース指定ボタン
            /// </summary>
            public CheckBox ChkTarget { get; set; }

            /// <summary>
            /// コーステーブル表示用リストビュー
            /// </summary>
            public ListView LvwLeft { get; set; }

            /// <summary>
            /// 臨時コーステーブル表示用リストビュー
            /// </summary>
            public ListView LvwRight { get; set; }

            /// <summary>
            /// 臨時コースマスタの担当者ＣＤテキストボックス
            /// </summary>
            public TextBox TxtRinjiTantosyaCD { get; set; }

            /// <summary>
            /// 臨時コースマスタの担当者ＣＤ参照ボタン
            /// </summary>
            public Button BtnTantosyaCD { get; set; }

            /// <summary>
            /// 臨時コースマスタの担当者名ラベル
            /// </summary>
            public Label LblRinjiTantosyaName { get; set; }

            /// <summary>
            /// 「変更あり」ラベル
            /// </summary>
            public Label LblChanged { get; set; }

            /// <summary>
            /// 臨時コース削除ボタン
            /// </summary>
            public Button BtnCourseDelete { get; set; }

            /// <summary>
            /// エラープロバイダー
            /// </summary>
            public ErrorProvider ErrorProvider { get; set; }

            /// <summary>
            /// コーステーブル件数ラベル
            /// </summary>
            public Label LblCountLeft { get; set; }

            /// <summary>
            /// 臨時コーステーブル件数ラベル
            /// </summary>
            public Label LblCountRight { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="courseMasterBindingSource">コースマスタのバインディングソース</param>
            /// <param name="courseTableBindingSource">コーステーブルのバインディングソース</param>
            /// <param name="rinjiCourseMasterBindingSource">臨時コースマスタのバインディングソース</param>
            /// <param name="rinjiCourseTableBindingSource">臨時コーステーブルのバインディングソース</param>
            /// <param name="chkTarget">組み替え対象コース指定ボタン</param>
            /// <param name="lvwLeft">コーステーブル表示用リストビュー</param>
            /// <param name="lvwRight">臨時コーステーブル表示用リストビュー</param>
            /// <param name="txtRinjiTantosyaCD">臨時コースマスタの担当者ＣＤテキストボックス</param>
            /// <param name="btnTantosyaCD">臨時コースマスタの担当者ＣＤ参照ボタン</param>
            /// <param name="lblRinjiTantosyaName">臨時コースマスタの担当者名ラベル</param>
            /// <param name="lblChanged">「変更あり」ラベル</param>
            /// <param name="btnCourseDelete">臨時コース削除ボタン</param>
            /// <param name="errorProvider">エラープロバイダー</param>
            /// <param name="lblCountLeft">コーステーブル件数ラベル</param>
            /// <param name="lblCountRight">臨時コーステーブル件数ラベル</param>
            public TargetCourse(BindingSource courseMasterBindingSource, BindingSource courseTableBindingSource,
                                BindingSource rinjiCourseMasterBindingSource, BindingSource rinjiCourseTableBindingSource,
                                CheckBox chkTarget, ListView lvwLeft, ListView lvwRight, TextBox txtRinjiTantosyaCD,
                                Button btnTantosyaCD, Label lblRinjiTantosyaName, Label lblChanged, Button btnCourseDelete, ErrorProvider errorProvider,
                                Label lblCountLeft, Label lblCountRight)
            {
                CourseMasterBindingSource = courseMasterBindingSource;
                CourseTableBindingSource = courseTableBindingSource;
                RinjiCourseMasterBindingSource = rinjiCourseMasterBindingSource;
                RinjiCourseTableBindingSource = rinjiCourseTableBindingSource;
                ChkTarget = chkTarget;
                LvwLeft = lvwLeft;
                LvwRight = lvwRight;
                TxtRinjiTantosyaCD = txtRinjiTantosyaCD;
                BtnTantosyaCD = btnTantosyaCD;
                LblRinjiTantosyaName = lblRinjiTantosyaName;
                LblChanged = lblChanged;
                BtnCourseDelete = btnCourseDelete;
                ErrorProvider = errorProvider;
                LblCountLeft = lblCountLeft;
                LblCountRight = lblCountRight;
            }

            /// <summary>
            /// 初期化
            /// </summary>
            public void Clear()
            {
                コースＣＤ = null;
                CourseMasterBindingSource.DataSource = new Course();
                CourseMasterBindingSource.ResetBindings(false);
                CourseTableBindingSource.DataSource = null;
                CourseTableBindingSource.ResetBindings(false);
                RinjiCourseMasterBindingSource.DataSource = new RinjiCourse();
                RinjiCourseMasterBindingSource.ResetBindings(false);
                RinjiCourseTableBindingSource.DataSource = null;
                RinjiCourseTableBindingSource.ResetBindings(false);

                LvwLeft.Items.Clear();
                LvwRight.Items.Clear();

                LblCountLeft.Text = "";
                LblCountRight.Text = "";

                LvwRight.Enabled = false;
                TxtRinjiTantosyaCD.Enabled = false;
                BtnTantosyaCD.Enabled = false;
                LblRinjiTantosyaName.Enabled = false;

                RinjiCourseOrigin = new RinjiCourse();

                LblChanged.Visible = false;
                BtnCourseDelete.Enabled = false;
            }
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// コース臨時組み替え検索条件
        /// </summary>
        private CourseRinjiCriteria _courseRinjiCriteria;

        /// <summary>
        /// コース臨時組み替え検索条件スナップショット
        /// </summary>
        private CourseRinjiCriteria _courseRinjiCriteriaSnapShot = null;

        /// <summary>
        /// 組み替え対象コース１
        /// </summary>
        private TargetCourse _targetCourse1 = null;

        /// <summary>
        /// 組み替え対象コース２
        /// </summary>
        private TargetCourse _targetCourse2 = null;

        /// <summary>
        /// 組み替え対象コース３
        /// </summary>
        private TargetCourse _targetCourse3 = null;

        /// <summary>
        /// 組み替え対象コース４
        /// </summary>
        private TargetCourse _targetCourse4 = null;

        /// <summary>
        /// ダイアログ表示イベント用の組み替え対象コース
        /// </summary>
        private TargetCourse _targetCourseForDialogEvent = null;

        /// <summary>
        /// コースの数字（１～４）（コントロール名の末尾）をキーとして、TargetCourseを値に持つマップ
        /// </summary>
        private Dictionary<string, TargetCourse> _targetCourseMap = new Dictionary<string, TargetCourse>();

        /// <summary>
        /// 組み替え対象コース指定ボタンイベント実行中フラグ
        /// </summary>
        private bool _isCheckChanging = false;

        /// <summary>
        /// マウスの押された位置（ドラッグ処理用）
        /// </summary>
        private Point _mouseDownPoint = Point.Empty;

        /// <summary>
        /// ドラッグ元の組み替え対象コース
        /// </summary>
        private TargetCourse _targetCourseDragSource = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form302()
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
        private void Form302_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // デザイン時に残している文字列をクリア
                lblTantosyaCD1.Text = "";
                lblTantosyaName1.Text = "";
                lblTantosyaCD2.Text = "";
                lblTantosyaName2.Text = "";
                lblTantosyaCD3.Text = "";
                lblTantosyaName3.Text = "";
                lblTantosyaCD4.Text = "";
                lblTantosyaName4.Text = "";

                // 組み替え対象コースを初期化
                _targetCourse1 = new TargetCourse(courseMasterBindingSource1, courseTableBindingSource1, rinjiCourseMasterBindingSource1, rinjiCourseTableBindingSource1,
                                                  chkTarget1, lvwLeft1, lvwRight1, txtRinjiTantosyaCD1, btnTantosyaCD1, lblRinjiTantosyaName1, lblChanged1, btnCourseDelete1, errorProvider1,
                                                  lblCountLeft1, lblCountRight1);
                _targetCourse2 = new TargetCourse(courseMasterBindingSource2, courseTableBindingSource2, rinjiCourseMasterBindingSource2, rinjiCourseTableBindingSource2,
                                                  chkTarget2, lvwLeft2, lvwRight2, txtRinjiTantosyaCD2, btnTantosyaCD2, lblRinjiTantosyaName2, lblChanged2, btnCourseDelete2, errorProvider2,
                                                  lblCountLeft2, lblCountRight2);
                _targetCourse3 = new TargetCourse(courseMasterBindingSource3, courseTableBindingSource3, rinjiCourseMasterBindingSource3, rinjiCourseTableBindingSource3,
                                                  chkTarget3, lvwLeft3, lvwRight3, txtRinjiTantosyaCD3, btnTantosyaCD3, lblRinjiTantosyaName3, lblChanged3, btnCourseDelete3, errorProvider3,
                                                  lblCountLeft3, lblCountRight3);
                _targetCourse4 = new TargetCourse(courseMasterBindingSource4, courseTableBindingSource4, rinjiCourseMasterBindingSource4, rinjiCourseTableBindingSource4,
                                                  chkTarget4, lvwLeft4, lvwRight4, txtRinjiTantosyaCD4, btnTantosyaCD4, lblRinjiTantosyaName4, lblChanged4, btnCourseDelete4, errorProvider4,
                                                  lblCountLeft4, lblCountRight4);

                // 初期化
                _targetCourse1.Clear();
                _targetCourse2.Clear();
                _targetCourse3.Clear();
                _targetCourse4.Clear();

                _targetCourseMap["1"] = _targetCourse1;
                _targetCourseMap["2"] = _targetCourse2;
                _targetCourseMap["3"] = _targetCourse3;
                _targetCourseMap["4"] = _targetCourse4;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);
                ComboUtil.SetCourseKubun(cboCourseKbn);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _courseRinjiCriteria = new CourseRinjiCriteria();
                courseRinjiBindingSource.DataSource = _courseRinjiCriteria;

                // スプレッドの最終列（臨時コース有無フラグ）を非表示にする
                fpSpread1_Sheet1.Columns[RINJI_FLG_COLUMN].Visible = false;
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
        private void Form302_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F9:                       // 登録ボタン押下
                        btnSave.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form302_FormClosing(object sender, FormClosingEventArgs e)
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
                    DialogResult r = MessageBox.Show(this, "変更されています。更新しますか？", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // 入力チェック
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
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_courseRinjiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_courseRinjiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _courseRinjiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 配送日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_courseRinjiCriteria.配送日 > warnDate)
                {
                    if (MessageBox.Show(this, "配送日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        _courseRinjiCriteriaSnapShot = null;

                        // 全ての対象のコースマスタ・コーステーブル・臨時コースマスタ・臨時コーステーブルをクリア
                        _isCheckChanging = true;
                        chkTarget1.Checked = false;
                        chkTarget2.Checked = false;
                        chkTarget3.Checked = false;
                        chkTarget4.Checked = false;

                        _targetCourse1.Clear();
                        _targetCourse2.Clear();
                        _targetCourse3.Clear();
                        _targetCourse4.Clear();

                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                // 配送日から求めたコース区分と選択したコース区分が異なる場合は警告メッセージを表示
                int courseKubun = Course.GetCourseKubun((DateTime)_courseRinjiCriteria.配送日);
                if (courseKubun != _courseRinjiCriteria.コース区分)
                {
                    string msg = "まちがったコース区分が選択されているおそれがあります。\r\n" +
                                 "\r\n" +
                                 "配送日として入力された【{0:yyyy/MM/dd}】は【{1}】ですが、\r\n" +
                                 "選択したコース区分は【{2}】です。\r\n" +
                                 "\r\n" +
                                 "続行しますか？";
                    Consts.CourseKubun haisobiCourseKubun = (Consts.CourseKubun)courseKubun;
                    Consts.CourseKubun selectedCourseKubun = (Consts.CourseKubun)_courseRinjiCriteria.コース区分;
                    msg = string.Format(msg, _courseRinjiCriteria.配送日, haisobiCourseKubun.DisplayName(), selectedCourseKubun.DisplayName());
                    if (MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        _courseRinjiCriteriaSnapShot = null;

                        // 全ての対象のコースマスタ・コーステーブル・臨時コースマスタ・臨時コーステーブルをクリア
                        _isCheckChanging = true;
                        chkTarget1.Checked = false;
                        chkTarget2.Checked = false;
                        chkTarget3.Checked = false;
                        chkTarget4.Checked = false;

                        _targetCourse1.Clear();
                        _targetCourse2.Clear();
                        _targetCourse3.Clear();
                        _targetCourse4.Clear();

                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                // 初期状態と異なる？
                if (HasChange())
                {
                    if (MessageBox.Show(this, "組み替えた内容を破棄して再読み込みします。", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 全ての対象のコースマスタ・コーステーブル・臨時コースマスタ・臨時コーステーブルをクリア
                _isCheckChanging = true;
                chkTarget1.Checked = false;
                chkTarget2.Checked = false;
                chkTarget3.Checked = false;
                chkTarget4.Checked = false;

                _targetCourse1.Clear();
                _targetCourse2.Clear();
                _targetCourse3.Clear();
                _targetCourse4.Clear();

                // コースマスタ取得
                GetCourseMaster();

                // 背景色をリセット
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    // 臨時コースがある場合は背景色を変える
                    if (fpSpread1_Sheet1.Cells[rowIndex, RINJI_FLG_COLUMN].Value as int? == 1)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = Color.PaleGreen;
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _courseRinjiCriteriaSnapShot = new CourseRinjiCriteria(_courseRinjiCriteria);
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
                _isCheckChanging = false;
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 組み替え対象コース指定ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTarget_CheckedChanged(object sender, EventArgs e)
        {
            // イベントの再帰呼び出しを回避する(☆)
            if (_isCheckChanging)
            {
                return;
            }

            try
            {
                _isCheckChanging = true;

                CheckBox chk = (CheckBox)sender;

                // 指定された対象コース（１～４）を決定
                TargetCourse targetCourse = GetTargetCourse(chk.Name);
                if (targetCourse == null)
                {
                    return;
                }

                // チェックON？
                if (chk.Checked)
                {
                    // 未検索であればNOP
                    if (bindingSource1.DataSource == null)
                    {
                        chk.Checked = false;        // (☆)
                        return;
                    }
                    if (fpSpread1_Sheet1.ActiveRowIndex < 0)
                    {
                        chk.Checked = false;        // (☆)
                        return;
                    }

                    // 他の対象コースに指定済みであればキャンセル
                    if (fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.ActiveRowIndex].BackColor == SystemColors.ControlLight)
                    {
                        MessageBox.Show(this, "他の組み込み対象コースに指定済みです。", this.Text, MessageBoxButtons.OK);
                        chk.Checked = false;        // (☆)
                        return;
                    }

                    // スプレッドの背景色を変更
                    fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.ActiveRowIndex].BackColor = SystemColors.ControlLight;

                    //*****************************************************************************
                    // コースマスタ・コーステーブル・臨時コースマスタ・臨時コーステーブルを読み込む
                    //*****************************************************************************
                    targetCourse.コースＣＤ = (int)fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, COURSE_CD_COLUMN].Value;

                    // コースマスタ取得
                    Course course = Course.GetCourse((int)targetCourse.コースＣＤ);
                    targetCourse.CourseMasterBindingSource.DataSource = course;

                    // コーステーブル取得
                    GetCourseTable(targetCourse);

                    targetCourse.LvwRight.Enabled = true;
                    targetCourse.TxtRinjiTantosyaCD.Enabled = true;
                    targetCourse.BtnTantosyaCD.Enabled = true;
                    targetCourse.LblRinjiTantosyaName.Enabled = true;

                    // 臨時コースマスタ取得
                    RinjiCourse rinjiCourse = RinjiCourse.GetRinjiCourse((int)targetCourse.コースＣＤ, (DateTime)_courseRinjiCriteria.配送日);
                    if (rinjiCourse == null)
                    {
                        rinjiCourse = new RinjiCourse(course);  // コースマスタからコピー
                        rinjiCourse.実施日 = _courseRinjiCriteria.配送日;
                        rinjiCourse.作成日 = null;
                        rinjiCourse.作成担当者ＣＤ = null;
                        rinjiCourse.作成担当者名 = null;
                        rinjiCourse.修正日 = null;
                        rinjiCourse.修正担当者ＣＤ = null;
                        rinjiCourse.修正担当者名 = null;
                        targetCourse.BtnCourseDelete.Enabled = false;
                    }
                    else
                    {
                        targetCourse.BtnCourseDelete.Enabled = true;
                    }
                    targetCourse.RinjiCourseMasterBindingSource.DataSource = rinjiCourse;
                    targetCourse.RinjiCourseOrigin = new RinjiCourse(rinjiCourse);          // 初期状態として保持

                    // 臨時コーステーブル取得
                    GetRinjiCourseTable(targetCourse, (DateTime)_courseRinjiCriteria.配送日);

                    targetCourse.LblChanged.Visible = false;
                }
                else
                {
                    // 変更されている？
                    if (HasChangeCourse(targetCourse) || HasChangeRinjiCourseTable(targetCourse.RinjiCourseTableBindingSource))
                    {
                        if (MessageBox.Show(this, "変更されています。解除しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            chk.Checked = true;     // (☆)
                            return;
                        }
                    }

                    // 対象のコースの背景色をクリア
                    for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                    {
                        if ((int)fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].Value == (int)targetCourse.コースＣＤ)
                        {
                            // 臨時コースがある場合は背景色を変える
                            if (fpSpread1_Sheet1.Cells[rowIndex, RINJI_FLG_COLUMN].Value as int? == 1)
                            {
                                fpSpread1_Sheet1.Rows[rowIndex].BackColor = Color.PaleGreen;
                            }
                            else
                            {
                                fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                            }
                            break;
                        }
                    }

                    // 対象のコースマスタ・コーステーブル・臨時コースマスタ・臨時コーステーブルを全てクリア
                    targetCourse.Clear();
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
                _isCheckChanging = false;
            }
        }

        /// <summary>
        /// 右コピーボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e)
        {
            try
            {
                // クリックされたボタンから対象のコースを決定
                Button btn = (Button)sender;
                TargetCourse targetCourse = GetTargetCourse(btn.Name);
                if (targetCourse == null)
                {
                    return;
                }

                // 左から右にコピー
                CopyLeftToRight(targetCourse);
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
        /// 全右コピーボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRightAll_Click(object sender, EventArgs e)
        {
            try
            {
                // クリックされたボタンから対象のコースを決定
                Button btn = (Button)sender;
                TargetCourse targetCourse = GetTargetCourse(btn.Name);
                if (targetCourse == null)
                {
                    return;
                }

                // 左から右にコピー
                CopyAllLeftToRight(targetCourse);
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
        /// リストビュー ドラッグ開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwLeft_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                ListView lvw = (ListView)sender;

                // ドラッグ元のコースを取得
                _targetCourseDragSource = GetTargetCourse(lvw.Name);

                // 複数選択されていてもe.Itemに渡されるのは１つなので、選択されている全てのアイテムをドラッグ対象にする
                lvw.DoDragDrop((ListView.SelectedListViewItemCollection)lvw.SelectedItems, DragDropEffects.Copy | DragDropEffects.Scroll);
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
        /// リストビュー 境界内にドラッグ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwRight_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                // ドラッグされているデータがリストビューアイテムコレクションであれば受け入れる
                // ※ここの処理でマウスカーソルが変わるので省略できない
                if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
                {
                    // 常にコピー
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
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
        /// リストビュー ドラッグ中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwRight_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                //// ドラッグされているデータがリストビューアイテムコレクションであれば、ドロップ先アイテムを選択状態にする（Explorerの動きに合わせる）
                //if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
                //{
                //    Point p = this.lvwRight.PointToClient(new Point(e.X, e.Y));
                //    ListViewItem item = this.lvwRight.GetItemAt(p.X, p.Y);
                //    if (item != null)
                //    {
                //        item.Selected = true;
                //    }
                //}
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
        /// リストビュー ドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwRight_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // ドラッグされているデータがリストビューアイテムコレクション？
                if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
                {
                    ListView.SelectedListViewItemCollection srcItems = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));

                    ListView lvwRight = (ListView)sender;

                    // 対象のコースを決定
                    TargetCourse targetCourse = GetTargetCourse(lvwRight.Name);
                    if (targetCourse == null)
                    {
                        return;
                    }
                    Label lblCount = targetCourse.LblCountRight;

                    // リストボックスのデータソースであるDataTableを取得
                    // 臨時コーステーブル（ドロップ先）を取得
                    DataSet ds = (DataSet)targetCourse.RinjiCourseTableBindingSource.DataSource;
                    if (ds == null)
                    {
                        return;
                    }
                    DataTable dtRinji = ds.Tables["臨時コーステーブル"];

                    // コーステーブル（ドラッグ元）を取得（ドラッグ元のTargetCourseであることに注意）
                    ds = (DataSet)_targetCourseDragSource.CourseTableBindingSource.DataSource;
                    DataTable dtOrigin = ds.Tables["コーステーブル"];

                    // ドロップ先のアイテムを取得
                    Point p = lvwRight.PointToClient(new Point(e.X, e.Y));
                    ListViewItem item = lvwRight.GetItemAt(p.X, p.Y);
                    int destIndex = lvwRight.Items.IndexOf(item);
                    int addRowIndex = destIndex;   // 臨時テーブルレコード追加先インデックス

                    // アイテムが存在しない場所を選択すると、IndexOf() が　-1 を返すので、
                    // その場合はリストの最後に追加するようにします。
                    if (destIndex < 0)
                    {
                        destIndex = lvwRight.Items.Count;
                        addRowIndex = -1;
                    }
                    else
                    {
                        //****************************************************************************************************************
                        // 臨時コースから得意先を削除することにより、リストビューからは削除されるが、
                        // そのデータソースであるDataTableにはレコードは残ったまま（RowStateがDeletedとして残っている）
                        // したがって、リストビュー指すインデックスに追加するのではなく、DataTableの適切な位置に追加しないといけない。
                        // リストビュー指すインデックスにあるレコードが、DataTableのどの位置にあるのかを求めて、そこに追加する必要がある。
                        //****************************************************************************************************************
                        // ドロップされた位置に該当するレコードを取得
                        DataRow[] drRinjis = dtRinji.Select(string.Format("得意先ＣＤ={0}", item.SubItems[TOKUISAKI_CD_INDEX].Text));
                        if (drRinjis == null || drRinjis.Length <= 0)
                        {
                            addRowIndex = -1;
                        }
                        else
                        {
                            addRowIndex = dtRinji.Rows.IndexOf(drRinjis[0]);   // DataTableへの追加先インデックスを取得
                        }
                    }

                    // ドロップされたリストビューアイテムコレクションの要素を一つずつその場所に挿入する
                    foreach (ListViewItem targetItem in srcItems)
                    {
                        // **************************************************************************************
                        // ドラッグ元のデータ（コーステーブル）を取得して、ドロップ先（臨時コーステーブル）に追加
                        // **************************************************************************************
                        // ドラッグ元のデータ（コーステーブルのレコード）を取得
                        DataRow[] drOrigins = dtOrigin.Select(string.Format("得意先ＣＤ={0}", targetItem.SubItems[TOKUISAKI_CD_INDEX].Text));
                        if (drOrigins == null || drOrigins.Length <= 0)
                        {
                            continue;
                        }
                        CourseTable courseTableCurrent = new CourseTable(drOrigins[0]);

                        // CourseTable を RinjiCourseTableに変換
                        RinjiCourseTable rinjiCourseTableCurrent = new RinjiCourseTable(courseTableCurrent);
                        rinjiCourseTableCurrent.実施日 = _courseRinjiCriteria.配送日;
                        rinjiCourseTableCurrent.コースＣＤ = targetCourse.コースＣＤ;     // コースＣＤを、ドロップされたコースに合わせる

                        // ドラッグ元の得意先がドロップ先にあればスキップ
                        DataRow[] drRinjis = dtRinji.Select(string.Format("得意先ＣＤ={0}", targetItem.SubItems[TOKUISAKI_CD_INDEX].Text));
                        if (drRinjis != null && drRinjis.Length > 0)
                        {
                            continue;
                        }

                        // 臨時コーステーブルのレコードを生成
                        DataRow drRinjiNew = dtRinji.NewRow();
                        rinjiCourseTableCurrent.SetDataRow(drRinjiNew);

                        // 臨時コーステーブル（のデータテーブル）に追加
                        if (addRowIndex < 0)
                        {
                            // 範囲外なので末尾に追加する
                            dtRinji.Rows.Add(drRinjiNew);
                            addRowIndex = dtRinji.Rows.Count;   // 次も末尾に追加する
                        }
                        else
                        {
                            dtRinji.Rows.InsertAt(drRinjiNew, addRowIndex);
                        }
                        addRowIndex++;

                        // ******************************************************************************
                        // ドラッグ元のデータ（ListViewItem）を取得して、ドロップ先（リストビュー）に追加
                        // ******************************************************************************
                        ListViewItem newItem = lvwRight.Items.Insert(destIndex, (ListViewItem)targetItem.Clone());
                        destIndex++;
                        newItem.Selected = true;

                        targetCourse.LblChanged.Visible = true;
                    }

                    // 件数を表示
                    lblCount.Text = string.Format("{0}件", lvwRight.Items.Count);
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
        /// リストビュー キーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwRight_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Deleteキー
                if (e.KeyCode == Keys.Delete)
                {
                    // 対象のコースを決定
                    ListView lvwRight = (ListView)sender;
                    TargetCourse targetCourse = GetTargetCourse(lvwRight.Name);
                    if (targetCourse == null)
                    {
                        return;
                    }

                    // 削除
                    if (Remove(targetCourse))
                    {
                        targetCourse.LblChanged.Visible = true;
                    }
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
        /// 担当者ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTantosyaCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                // 対象のテキストボックスからボタンを決定する
                TextBox txt = (TextBox)sender;
                TargetCourse targetCourse = GetTargetCourse(txt.Name);
                if (targetCourse != null)
                {
                    targetCourse.BtnTantosyaCD.PerformClick();
                }
            }
        }

        /// <summary>
        /// 担当者ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTantosyaCD_Click(object sender, EventArgs e)
        {
            try
            {
                // クリックされたボタンから対象のコースを決定
                Button btn = (Button)sender;
                _targetCourseForDialogEvent = GetTargetCourse(btn.Name);

                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904TantosyaCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 担当者選択ダイアログクローズ（担当者ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904TantosyaCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    _targetCourseForDialogEvent.ErrorProvider.SetError(_targetCourseForDialogEvent.TxtRinjiTantosyaCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    _targetCourseForDialogEvent.TxtRinjiTantosyaCD.Text = tantousya.担当者ＣＤ.ToString();
                    RinjiCourse rinjiCourse = (RinjiCourse)_targetCourseForDialogEvent.RinjiCourseMasterBindingSource.DataSource;
                    rinjiCourse.担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    _targetCourseForDialogEvent.LblRinjiTantosyaName.Text = tantousya.担当者名;

                    // リストボックスも含めて変更の有無をきちんと判断する
                    if (HasChangeCourse(_targetCourseForDialogEvent) || HasChangeRinjiCourseTable(_targetCourseForDialogEvent.RinjiCourseTableBindingSource))
                    {
                        _targetCourseForDialogEvent.LblChanged.Visible = true;
                    }
                    else
                    {
                        _targetCourseForDialogEvent.LblChanged.Visible = false;
                    }
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
        /// 担当者ＣＤ検証後イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRinjiTantosyaCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // 対象のコースを決定
                TextBox txt = (TextBox)sender;
                TargetCourse targetCourse = GetTargetCourse(txt.Name);

                // リストボックスも含めて変更の有無をきちんと判断する
                if (HasChangeCourse(targetCourse) || HasChangeRinjiCourseTable(targetCourse.RinjiCourseTableBindingSource))
                {
                    targetCourse.LblChanged.Visible = true;
                }
                else
                {
                    targetCourse.LblChanged.Visible = false;
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
        /// 上移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                // クリックされたボタンから対象のコースを決定
                Button btn = (Button)sender;
                TargetCourse targetCourse = GetTargetCourse(btn.Name);
                if (targetCourse == null)
                {
                    return;
                }

                // 上移動
                if (MoveUp(targetCourse))
                {
                    targetCourse.LblChanged.Visible = true;
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
        /// 下移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                // クリックされたボタンから対象のコースを決定
                Button btn = (Button)sender;
                TargetCourse targetCourse = GetTargetCourse(btn.Name);
                if (targetCourse == null)
                {
                    return;
                }

                // 下移動
                if (MoveDown(targetCourse))
                {
                    targetCourse.LblChanged.Visible = true;
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
        /// 行削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // クリックされたボタンから対象のコースを決定
                Button btn = (Button)sender;
                TargetCourse targetCourse = GetTargetCourse(btn.Name);
                if (targetCourse == null)
                {
                    return;
                }

                // 削除
                if (Remove(targetCourse))
                {
                    targetCourse.LblChanged.Visible = true;
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
        /// 臨時コース削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // クリックされたボタンから対象のコースを決定
                Button btn = (Button)sender;
                TargetCourse targetCourse = GetTargetCourse(btn.Name);
                if (targetCourse == null)
                {
                    return;
                }

                Course course = (Course)targetCourse.CourseMasterBindingSource.DataSource;
                if (MessageBox.Show(this, string.Format("コース「{0}」の臨時コースを削除します。", course.コース名), this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                // コース削除
                if (Delete(targetCourse))
                {
                    // コースマスタ取得
                    GetCourseMaster();

                    // 組み替え対象コース指定ボタン(チェックボックス)をOFFにする
                    _isCheckChanging = true;
                    targetCourse.ChkTarget.Checked = false;

                    // 対象のコースの背景色をクリア
                    for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                    {
                        if ((int)fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].Value == (int)targetCourse.コースＣＤ)
                        {
                            // 臨時コースがある場合は背景色を変える
                            if (fpSpread1_Sheet1.Cells[rowIndex, RINJI_FLG_COLUMN].Value as int? == 1)
                            {
                                fpSpread1_Sheet1.Rows[rowIndex].BackColor = Color.PaleGreen;
                            }
                            else
                            {
                                fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                            }
                            break;
                        }
                    }

                    // 対象のコースマスタ・コーステーブル・臨時コースマスタ・臨時コーステーブルを全てクリア
                    targetCourse.Clear();
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
                _isCheckChanging = false;
            }
        }

        /// <summary>
        /// 登録ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 入力チェック
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
        /// コースマスタ取得
        /// </summary>
        private void GetCourseMaster()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.コース区分" +
                                      "\r\n     ,VC.コース区分名" +
                                      "\r\n     ,VC.表示順" +
                                      "\r\n     ,A.コース名" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,A.担当者ＣＤ" +
                                      "\r\n     ,A3.担当者名" +
                                      "\r\n     ,A.工場フラグ" +
                                      "\r\n     ,A.無効フラグ" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n     ,CASE WHEN R.コースＣＤ IS NULL THEN 0 ELSE 1 END AS 臨時コース有無フラグ" +
                                      "\r\n FROM" +
                                      "\r\n     コースマスタ AS A" +
                                      "\r\n     LEFT JOIN 臨時コースマスタ AS R ON R.企業ＣＤ = A.企業ＣＤ AND R.コースＣＤ = A.コースＣＤ AND R.実施日 = @実施日" +
                                      "\r\n     LEFT JOIN V_コース区分 AS VC ON A.コース区分 = VC.コース区分CD" +
                                      "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.部署ＣＤ = B.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A3.企業ＣＤ = @企業ＣＤ AND A.担当者ＣＤ = A3.担当者ＣＤ" +
                                      "\r\n WHERE A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n   AND A.コース区分 = @コース区分" +
                                      "\r\n   AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n   AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n ORDER BY A.コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _courseRinjiCriteria.コース区分;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseRinjiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = (DateTime)_courseRinjiCriteria.配送日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("コースマスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "コースマスタ";

                    log.Info("コースマスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN, "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN, "コース名");
                fpSpread1_Sheet1.BindDataColumn(RINJI_FLG_COLUMN, "臨時コース有無フラグ");
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
        /// コーステーブル取得
        /// </summary>
        /// <param name="targetCourse">組み替え対象コースが持つコントロール群</param>
        private void GetCourseTable(TargetCourse targetCourse)
        {
            try
            {
                ListView listView = targetCourse.LvwLeft;
                int courseCD = (int)targetCourse.コースＣＤ;
                Label lblCount = targetCourse.LblCountLeft;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                                      "\r\n     ,T.得意先名略称" +
                                      "\r\n     ,A.コース順" +
                                      "\r\n     ,A.無効フラグ" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     コーステーブル AS A" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND A.得意先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND A.コースＣＤ = @コースＣＤ" +
                                      "\r\n AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.コース順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("コーステーブル");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    BindingSource bindingSource = targetCourse.CourseTableBindingSource;

                    bindingSource.DataSource = ds;
                    bindingSource.DataMember = "コーステーブル";

                    // 取得したデータテーブルの内容をリストビューに表示
                    ShowListView(listView, lblCount, dt);

                    log.Info("コーステーブルを検索しました。");
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
        /// 臨時コーステーブル取得
        /// </summary>
        /// <param name="targetCourse">組み替え対象コースが持つコントロール群</param>
        /// <param name="date">実施日</param>
        private void GetRinjiCourseTable(TargetCourse targetCourse, DateTime date)
        {
            try
            {
                ListView listView = targetCourse.LvwRight;
                int courseCD = (int)targetCourse.コースＣＤ;
                Label lblCount = targetCourse.LblCountRight;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.実施日" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                                      "\r\n     ,T.得意先名略称" +
                                      "\r\n     ,A.コース順" +
                                      "\r\n     ,A.無効フラグ" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     臨時コーステーブル AS A" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND A.得意先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND A.コースＣＤ = @コースＣＤ" +
                                      "\r\n AND A.実施日 = @実施日" +
                                      "\r\n AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.コース順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                    cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = date;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("臨時コーステーブル");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    BindingSource bindingSource = targetCourse.RinjiCourseTableBindingSource;

                    bindingSource.DataSource = ds;
                    bindingSource.DataMember = "臨時コーステーブル";

                    // 取得したデータテーブルの内容をリストビューに表示
                    ShowListView(listView, lblCount, dt);

                    log.Info("臨時コーステーブルを検索しました。");
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
        /// データテーブルの内容をリストビューに表示する
        /// </summary>
        /// <param name="lvw">リストビュー</param>
        /// <param name="lblCount">件数ラベル</param>
        /// <param name="dt">データテーブル</param>
        private void ShowListView(ListView lvw, Label lblCount, DataTable dt)
        {
            try
            {
                // リストビューの表示を一時停止
                lvw.BeginUpdate();

                lvw.Items.Clear();

                foreach (DataRow dr in dt.Rows)
                {
                    // データテーブルの行を、コーステーブルとして取り出す
                    // （臨時コーステーブルとほとんど同じであるため）
                    CourseTable courseTable = new CourseTable(dr);

                    // リストビューのアイテムに追加
                    ListViewItem item = new ListViewItem(((int)(courseTable.コース順)).ToString());
                    item.ToolTipText = courseTable.得意先名略称;
                    item.SubItems.Add(courseTable.得意先名略称);                        // TOKUISAKI_NAME_INDEX
                    item.SubItems.Add((((int)(courseTable.得意先ＣＤ)).ToString()));    // TOKUISAKI_CD_INDEX
                    lvw.Items.Add(item);
                }

                // 件数を表示
                lblCount.Text = string.Format("{0}件", lvw.Items.Count);
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
                lvw.EndUpdate();
            }
        }

        /// <summary>
        /// コントロール名に応じたTargetCourse（１～４）を返します。
        /// </summary>
        /// <param name="name">末尾が数字（１～４）のコントロール名</param>
        /// <returns>組み替え対象コース１～４のTargetCourse</returns>
        private TargetCourse GetTargetCourse(string name)
        {
            try
            {
                TargetCourse targetCourse = null;

                string index = name.Substring(name.Length - 1);     // 末尾の1文字を取得

                // キーに該当するものがあれば返す
                if (_targetCourseMap.ContainsKey(index))
                {
                    targetCourse = _targetCourseMap[index];
                }
                return targetCourse;
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
        /// コーステーブルから臨時コーステーブルへのコピー
        /// </summary>
        /// <param name="targetCourse">組み替え対象コース</param>
        /// <returns>コピーした場合はtrue</returns>
        private void CopyLeftToRight(TargetCourse targetCourse)
        {
            try
            {
                // **********************************************************************
                // リストビューのItemsとSelectedItemsとではコレクションの型が異なるので、
                // CopyAllLeftToRight()と無理に共通化はしない
                // ┌───────┬───────────────┐
                // │プロパティ    │型                            │
                // ├───────┼───────────────┤
                // │Items         │ListViewItemCollection        │
                // ├───────┼───────────────┤
                // │SelectedItems │SelectedListViewItemCollection│
                // └───────┴───────────────┘
                // **********************************************************************

                BindingSource bindingSourceCourse = targetCourse.CourseTableBindingSource;
                BindingSource bindingSourceRinji = targetCourse.RinjiCourseTableBindingSource;
                ListView lvwLeft = targetCourse.LvwLeft;
                ListView lvwRight = targetCourse.LvwRight;
                Label lblCount = targetCourse.LblCountRight;

                // コピー元未選択？
                if (lvwLeft.SelectedItems == null || lvwLeft.SelectedItems.Count == 0)
                {
                    return;
                }

                // コピー先が未検索であればNOP
                DataSet ds = (DataSet)bindingSourceRinji.DataSource;
                if (ds == null)
                {
                    return;
                }
                DataTable dtRinji = ds.Tables["臨時コーステーブル"];

                ds = (DataSet)targetCourse.CourseTableBindingSource.DataSource;
                DataTable dtOrigin = ds.Tables["コーステーブル"];

                // コピー先にコピー
                int destIndex = lvwRight.Items.Count;  // リストビューには常に末尾に追加
                foreach (ListViewItem srcItem in lvwLeft.SelectedItems)
                {
                    // コピー元のデータ（コーステーブルのレコード）を取得
                    DataRow[] drOrigins = dtOrigin.Select(string.Format("得意先ＣＤ={0}", srcItem.SubItems[TOKUISAKI_CD_INDEX].Text));
                    if (drOrigins == null || drOrigins.Length <= 0)
                    {
                        continue;
                    }
                    CourseTable courseTableCurrent = new CourseTable(drOrigins[0]);

                    // コピー元の得意先がコピー先にあればスキップ
                    DataRow[] drRinjis = dtRinji.Select(string.Format("得意先ＣＤ={0}", srcItem.SubItems[TOKUISAKI_CD_INDEX].Text));
                    if (drRinjis != null && drRinjis.Length > 0)
                    {
                        continue;
                    }

                    // コピー元（コーステーブル）の内容をコピー先（臨時コーステーブル）にコピー
                    RinjiCourseTable rinjiCourseTable = new RinjiCourseTable(courseTableCurrent);
                    rinjiCourseTable.実施日 = _courseRinjiCriteria.配送日;

                    // 臨時コーステーブルのレコードを生成して追加
                    DataRow drRinjiNew = dtRinji.NewRow();
                    rinjiCourseTable.SetDataRow(drRinjiNew);
                    dtRinji.Rows.Add(drRinjiNew);

                    // リストビューに追加
                    ListViewItem newItem = lvwRight.Items.Insert(destIndex, (ListViewItem)srcItem.Clone());
                    destIndex++;
                    newItem.Selected = true;

                    targetCourse.LblChanged.Visible = true;
                }

                // 件数を表示
                lblCount.Text = string.Format("{0}件", lvwRight.Items.Count);
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
        /// コーステーブルから臨時コーステーブルへの全コピー
        /// </summary>
        /// <param name="targetCourse">組み替え対象コース</param>
        private void CopyAllLeftToRight(TargetCourse targetCourse)
        {
            try
            {
                // **********************************************************************
                // リストビューのItemsとSelectedItemsとではコレクションの型が異なるので、
                // CopyLeftToRight()と無理に共通化はしない
                // ┌───────┬───────────────┐
                // │プロパティ    │型                            │
                // ├───────┼───────────────┤
                // │Items         │ListViewItemCollection        │
                // ├───────┼───────────────┤
                // │SelectedItems │SelectedListViewItemCollection│
                // └───────┴───────────────┘
                // **********************************************************************

                BindingSource bindingSourceCourse = targetCourse.CourseTableBindingSource;
                BindingSource bindingSourceRinji = targetCourse.RinjiCourseTableBindingSource;
                ListView lvwLeft = targetCourse.LvwLeft;
                ListView lvwRight = targetCourse.LvwRight;
                Label lblCount = targetCourse.LblCountRight;

                // コピー元に行が無い？
                if (bindingSourceCourse.Count <= 0)
                {
                    return;
                }

                // コピー先が未検索であればNOP
                DataSet ds = (DataSet)bindingSourceRinji.DataSource;
                if (ds == null)
                {
                    return;
                }
                DataTable dtRinji = ds.Tables["臨時コーステーブル"];

                ds = (DataSet)targetCourse.CourseTableBindingSource.DataSource;
                DataTable dtOrigin = ds.Tables["コーステーブル"];

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // コピー元の全ての行に対して
                int destIndex = lvwRight.Items.Count;  // リストビューには常に末尾に追加
                foreach (ListViewItem srcItem in lvwLeft.Items)
                {
                    // コピー元のデータ（コーステーブルのレコード）を取得
                    DataRow[] drOrigins = dtOrigin.Select(string.Format("得意先ＣＤ={0}", srcItem.SubItems[TOKUISAKI_CD_INDEX].Text));
                    if (drOrigins == null || drOrigins.Length <= 0)
                    {
                        continue;
                    }
                    CourseTable courseTableCurrent = new CourseTable(drOrigins[0]);

                    // コピー元の得意先がコピー先にあればスキップ
                    DataRow[] drRinjis = dtRinji.Select(string.Format("得意先ＣＤ={0}", srcItem.SubItems[TOKUISAKI_CD_INDEX].Text));
                    if (drRinjis != null && drRinjis.Length > 0)
                    {
                        continue;
                    }

                    // コピー元（コーステーブル）の内容をコピー先（臨時コーステーブル）にコピー
                    RinjiCourseTable rinjiCourseTable = new RinjiCourseTable(courseTableCurrent);
                    rinjiCourseTable.実施日 = _courseRinjiCriteria.配送日;

                    // 臨時コーステーブルのレコードを生成して追加
                    DataRow drRinjiNew = dtRinji.NewRow();
                    rinjiCourseTable.SetDataRow(drRinjiNew);
                    dtRinji.Rows.Add(drRinjiNew);

                    // リストビューに追加
                    ListViewItem newItem = lvwRight.Items.Insert(destIndex, (ListViewItem)srcItem.Clone());
                    destIndex++;
                    newItem.Selected = true;

                    targetCourse.LblChanged.Visible = true;
                }

                // 件数を表示
                lblCount.Text = string.Format("{0}件", lvwRight.Items.Count);
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
        /// 上移動
        /// </summary>
        /// <param name="targetCourse">組み替え対象コースが持つコントロール群</param>
        /// <returns>移動できた場合はtrue</returns>
        private bool MoveUp(TargetCourse targetCourse)
        {
            try
            {
                ListView lvwRight = targetCourse.LvwRight;
                // 未選択 または 複数選択の場合はNOP
                if (lvwRight.SelectedItems == null || lvwRight.SelectedItems.Count != 1)
                {
                    return false;
                }

                // 先頭行の場合はNOP
                int currentIndex = lvwRight.SelectedIndices[0];
                if (currentIndex <= 0)
                {
                    return false;
                }

                //***************************
                // リストビュー内の順序を変更
                //***************************
                ListViewItem currentItem = lvwRight.Items[currentIndex];    // 現在行のアイテムを取得
                lvwRight.Items.RemoveAt(currentIndex);                      // 削除しないと追加できない
                lvwRight.Items.Insert(currentIndex - 1, currentItem);       // 前の行に追加
                lvwRight.EnsureVisible(currentIndex - 1);                   // 移動したアイテムを確実に表示する

                //*******************************
                // データソース内のレコードを移動
                //*******************************
                // 臨時コーステーブルのバインディングソースを取得
                BindingSource bindingSource = targetCourse.RinjiCourseTableBindingSource;

                // 現在行を取得
                DataSet ds = (DataSet)targetCourse.RinjiCourseTableBindingSource.DataSource;
                DataTable dtRinji = ds.Tables["臨時コーステーブル"];
                DataRow[] drRinjis = dtRinji.Select(string.Format("得意先ＣＤ={0}", currentItem.SubItems[TOKUISAKI_CD_INDEX].Text));
                DataRow drCurrent = drRinjis[0];
                int currentRowIndex = dtRinji.Rows.IndexOf(drCurrent);
                RinjiCourseTable rinjiCourseTableCurrent = new RinjiCourseTable(drCurrent);
                // 現在行のひとつ前から前方に向かって削除されていない行(=移動先として有効な行)を探す
                for (int rowIndex = currentRowIndex - 1; rowIndex >= 0; rowIndex--)
                {
                    DataRow drTarget = dtRinji.Rows[rowIndex];
                    if (drTarget.RowState != DataRowState.Deleted)
                    {
                        RinjiCourseTable rinjiCourseTableTarget = new RinjiCourseTable(drTarget);

                        // 現在行と移動先行を交換
                        rinjiCourseTableCurrent.SetDataRow(drTarget);
                        rinjiCourseTableTarget.SetDataRow(drCurrent);
                        return true;
                    }
                }
                return false;
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
        /// 下移動
        /// </summary>
        /// <param name="targetCourse">組み替え対象コースが持つコントロール群</param>
        /// <returns>移動できた場合はtrue</returns>
        private bool MoveDown(TargetCourse targetCourse)
        {
            try
            {
                ListView lvwRight = targetCourse.LvwRight;
                // 未選択 または 複数選択の場合はNOP
                if (lvwRight.SelectedItems == null || lvwRight.SelectedItems.Count != 1)
                {
                    return false;
                }

                // 最終行の場合はNOP
                int currentIndex = lvwRight.SelectedIndices[0];
                if (currentIndex >= lvwRight.Items.Count - 1)
                {
                    return false;
                }

                //***************************
                // リストビュー内の順序を変更
                //***************************
                ListViewItem currentItem = lvwRight.Items[currentIndex];    // 現在行のアイテムを取得
                lvwRight.Items.RemoveAt(currentIndex);                      // 削除しないと追加できない
                lvwRight.Items.Insert(currentIndex + 1, currentItem);       // 次の行に追加
                lvwRight.EnsureVisible(currentIndex + 1);                   // 移動したアイテムを確実に表示する

                //*******************************
                // データソース内のレコードを移動
                //*******************************
                // 臨時コーステーブルのバインディングソースを取得
                BindingSource bindingSource = targetCourse.RinjiCourseTableBindingSource;

                // 現在行を取得
                DataSet ds = (DataSet)targetCourse.RinjiCourseTableBindingSource.DataSource;
                DataTable dtRinji = ds.Tables["臨時コーステーブル"];
                DataRow[] drRinjis = dtRinji.Select(string.Format("得意先ＣＤ={0}", currentItem.SubItems[TOKUISAKI_CD_INDEX].Text));
                DataRow drCurrent = drRinjis[0];
                int currentRowIndex = dtRinji.Rows.IndexOf(drCurrent);
                RinjiCourseTable rinjiCourseTableCurrent = new RinjiCourseTable(drCurrent);
                // 現在行のひとつ後から後方に向かって削除されていない行(=移動先として有効な行)を探す
                for (int rowIndex = currentRowIndex + 1; rowIndex < dtRinji.Rows.Count; rowIndex++)
                {
                    DataRow drTarget = dtRinji.Rows[rowIndex];
                    if (drTarget.RowState != DataRowState.Deleted)
                    {
                        RinjiCourseTable rinjiCourseTableTarget = new RinjiCourseTable(drTarget);

                        // 現在行と移動先行を交換
                        rinjiCourseTableCurrent.SetDataRow(drTarget);
                        rinjiCourseTableTarget.SetDataRow(drCurrent);
                        return true;
                    }
                }
                return false;
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
        /// 削除
        /// </summary>
        /// <param name="targetCourse">組み替え対象コースが持つコントロール群</param>
        /// <returns>削除できた場合はtrue</returns>
        private bool Remove(TargetCourse targetCourse)
        {
            try
            {
                // リストボックスのデータソースであるDataTableを取得
                DataSet ds = (DataSet)targetCourse.RinjiCourseTableBindingSource.DataSource;
                if (ds == null)
                {
                    return false;
                }
                DataTable dtRinji = ds.Tables["臨時コーステーブル"];
                ListView lvwRight = targetCourse.LvwRight;
                Label lblCount = targetCourse.LblCountRight;

                // 未選択の場合はNOP
                if (lvwRight.SelectedItems == null || lvwRight.SelectedItems.Count == 0)
                {
                    return false;
                }
                int index = lvwRight.SelectedIndices[0];    // 選択されている最初のインデックスを取得

                // 選択されている項目を全て削除
                foreach (ListViewItem item in lvwRight.SelectedItems)
                {
                    // まず、データテーブルのレコードから削除
                    DataRow[] drRinjis = dtRinji.Select(string.Format("得意先ＣＤ={0}", item.SubItems[TOKUISAKI_CD_INDEX].Text));
                    if (drRinjis != null && drRinjis.Length > 0)
                    {
                        drRinjis[0].Delete();
                    }

                    // リストビューから削除
                    lvwRight.Items.Remove(item);
                }

                // 件数を表示
                lblCount.Text = string.Format("{0}件", lvwRight.Items.Count);

                // 次に選択状態にするインデックスを決定
                if (lvwRight.Items.Count <= index)
                {
                    index = lvwRight.Items.Count - 1;
                }
                if (index >= 0)
                {
                    lvwRight.Items[index].Selected = true;
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
        /// 全ての臨時コースマスタと臨時コーステーブルが変更されているかを返します。
        /// </summary>
        /// <param name="targetCourse">組み替え対象コース</param>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            if (HasChangeCourse(_targetCourse1) || HasChangeRinjiCourseTable(_targetCourse1.RinjiCourseTableBindingSource) ||
                HasChangeCourse(_targetCourse2) || HasChangeRinjiCourseTable(_targetCourse2.RinjiCourseTableBindingSource) ||
                HasChangeCourse(_targetCourse3) || HasChangeRinjiCourseTable(_targetCourse3.RinjiCourseTableBindingSource) ||
                HasChangeCourse(_targetCourse4) || HasChangeRinjiCourseTable(_targetCourse4.RinjiCourseTableBindingSource))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 臨時コースマスタが変更されているかを返します。
        /// </summary>
        /// <param name="targetCourse">組み替え対象コース</param>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeCourse(TargetCourse targetCourse)
        {
            RinjiCourse rinjiCourse = (RinjiCourse)targetCourse.RinjiCourseMasterBindingSource.DataSource;
            if (rinjiCourse.Equals(targetCourse.RinjiCourseOrigin))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 臨時コーステーブルが変更されているかを返します。
        /// </summary>
        /// <param name="bindingSource">バインディングソース</param>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeRinjiCourseTable(BindingSource bindingSource)
        {
            // コーステーブルを比較
            DataSet ds = (DataSet)bindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["臨時コーステーブル"];

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

                // 検索実行時の条件が変わっている？
                if (_courseRinjiCriteriaSnapShot != null)
                {
                    if (_courseRinjiCriteria.Equals(_courseRinjiCriteriaSnapShot) == false)
                    {
                        MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }

                // 登録するものがない？（臨時コーステーブルのレコードの有無は問わない）
                if (chkTarget1.Checked == false && chkTarget2.Checked == false && chkTarget2.Checked == false && chkTarget4.Checked == false)
                {
                    MessageBox.Show(this, "登録するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                int leftTotal = 0;
                int rightTotal = 0;

                // 臨時コースマスタのエラーチェック
                foreach (TargetCourse targetCourse in _targetCourseMap.Values)
                {
                    if (targetCourse.コースＣＤ != null)
                    {
                        RinjiCourse rinjiCourse = (RinjiCourse)targetCourse.RinjiCourseMasterBindingSource.DataSource;
                        if (rinjiCourse.HasErrors)
                        {
                            MessageBox.Show(this, rinjiCourse.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }

                        leftTotal += targetCourse.LvwLeft.Items.Count;
                        rightTotal += targetCourse.LvwRight.Items.Count;
                    }
                }

                // コーステーブルの件数の合計と、臨時コーステーブルの件数の合計が一致していない場合は、得意先の設定漏れが考えられるので注意を促す
                if (leftTotal != rightTotal)
                {
                    if (MessageBox.Show(this, "得意先の件数が一致しません。臨時コースへの得意先設定漏れのおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return false;
                    }
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
        /// <returns>成功した場合は True</returns>
        private bool Save()
        {
            History.ProcessType historyProcess = History.ProcessType.Update;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 組み替え対象コース
                    foreach (TargetCourse targetCourse in _targetCourseMap.Values)
                    {
                        if (targetCourse.コースＣＤ == null)
                        {
                            continue;
                        }

                        // 臨時コースマスタ保存
                        if (SaveRinjiCourse(con, tran, targetCourse) == false)
                        {
                            return false;
                        }

                        // 臨時コーステーブル保存
                        if (SaveRinjiCourseTable(con, tran, targetCourse) == false)
                        {
                            return false;
                        }
                    }

                    // 臨時コースを注文に反映する
                    // いったん元に戻す
                    if (UpdateChumonCourseOriginal(con, tran, -1) == false)
                    {
                        return false;
                    }

                    // 臨時コースを注文に反映
                    if (UpdateChumonCourseRinji(con, tran) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("臨時コースマスタと臨時コーステーブルを{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        /// <summary>
        /// 臨時コースマスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="targetCourse">組み替え対象コース</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveRinjiCourse(SqlConnection con, SqlTransaction tran, TargetCourse targetCourse)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 臨時コースマスタ
                RinjiCourse rinjiCourse = (RinjiCourse)targetCourse.RinjiCourseMasterBindingSource.DataSource;

                if (rinjiCourse.作成日 == null)
                {
                    ret = rinjiCourse.Insert(con, tran);
                    historyProcess = History.ProcessType.Create;
                }
                else
                {
                    ret = rinjiCourse.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, rinjiCourse.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "臨時コースマスタ", rinjiCourse.Code, historyProcess);
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
        /// 臨時コーステーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="targetCourse">組み替え対象コース</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveRinjiCourseTable(SqlConnection con, SqlTransaction tran, TargetCourse targetCourse)
        {
            try
            {
                DataSet ds = (DataSet)targetCourse.RinjiCourseTableBindingSource.DataSource;
                DataTable dt = ds.Tables["臨時コーステーブル"];

                History.ProcessType historyProcess = History.ProcessType.Create;
                int seq = 1;
                bool ret = false;

                // 全削除→全追加
                SqlCommand cmdDel = con.CreateCommand();
                cmdDel.CommandText = "DELETE FROM 臨時コーステーブル WHERE 企業ＣＤ = @企業ＣＤ AND コースＣＤ = @コースＣＤ AND 実施日 = @実施日";
                cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmdDel.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)targetCourse.コースＣＤ;
                cmdDel.Parameters.Add("@実施日", SqlDbType.Date).Value = (DateTime)_courseRinjiCriteria.配送日;
                cmdDel.Transaction = tran;
                log.Debug(cmdDel.CommandTextWithParameters());
                cmdDel.ExecuteNonQuery();

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    RinjiCourseTable rinjiCourseTable = new RinjiCourseTable(dr);

                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    rinjiCourseTable.コースＣＤ = targetCourse.コースＣＤ;
                    rinjiCourseTable.実施日 = _courseRinjiCriteria.配送日;
                    rinjiCourseTable.コース順 = seq;
                    seq++;
                    ret = rinjiCourseTable.Insert(con, tran);
                    historyProcess = History.ProcessType.Create;

                    if (ret == false)
                    {
                        MessageBox.Show(this, rinjiCourseTable.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "臨時コーステーブル", rinjiCourseTable.Code, historyProcess);
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
        /// 臨時コース削除
        /// </summary>
        /// <param name="targetCourse">削除対象コース</param>
        /// <returns>成功した場合は True</returns>
        private bool Delete(TargetCourse targetCourse)
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            if (targetCourse.コースＣＤ == null)
            {
                return true;
            }

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 臨時コースマスタ削除
                    if (DeleteRinjiCourse(con, tran, targetCourse) == false)
                    {
                        return false;
                    }

                    // 臨時コーステーブル削除
                    if (DeleteRinjiCourseTable(con, tran, targetCourse) == false)
                    {
                        return false;
                    }

                    // 臨時コースを削除したことを注文に反映する
                    // 元のコースに戻す
                    if (UpdateChumonCourseOriginal(con, tran, (int)targetCourse.コースＣＤ) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("臨時コースマスタと臨時コーステーブルを{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        /// <summary>
        /// 臨時コースマスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="targetCourse">組み替え対象コース</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteRinjiCourse(SqlConnection con, SqlTransaction tran, TargetCourse targetCourse)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Delete;

                // 臨時コースマスタ
                RinjiCourse rinjiCourse = (RinjiCourse)targetCourse.RinjiCourseMasterBindingSource.DataSource;

                // 既存の臨時コースマスタがある場合のみ削除（新規追加したばかりの場合は削除処理不要）
                if (rinjiCourse.作成日 == null)
                {
                    return true;
                }

                ret = rinjiCourse.Delete(con, tran);
                if (ret == false)
                {
                    MessageBox.Show(this, rinjiCourse.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "臨時コースマスタ", rinjiCourse.Code, historyProcess);
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
        /// 臨時コーステーブル削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="targetCourse">組み替え対象コース</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteRinjiCourseTable(SqlConnection con, SqlTransaction tran, TargetCourse targetCourse)
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            try
            {
                DataSet ds = (DataSet)targetCourse.RinjiCourseTableBindingSource.DataSource;
                DataTable dt = ds.Tables["臨時コーステーブル"];

                // 全削除
                SqlCommand cmdDel = con.CreateCommand();
                cmdDel.CommandText = "DELETE FROM 臨時コーステーブル WHERE 企業ＣＤ = @企業ＣＤ AND コースＣＤ = @コースＣＤ AND 実施日 = @実施日";
                cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmdDel.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)targetCourse.コースＣＤ;
                cmdDel.Parameters.Add("@実施日", SqlDbType.Date).Value = (DateTime)_courseRinjiCriteria.配送日;
                cmdDel.Transaction = tran;
                log.Debug(cmdDel.CommandTextWithParameters());
                cmdDel.ExecuteNonQuery();

                string code = string.Format("コースＣＤ=[{0}] 実施日=[{1}]", (int)targetCourse.コースＣＤ, (DateTime)_courseRinjiCriteria.配送日);
                History.Save(con, tran, this.Text, "臨時コーステーブル", code, historyProcess);
                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("臨時コーステーブルが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("臨時コーステーブルの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("臨時コーステーブルの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                // コースマスタ取得
                GetCourseMaster();

                // 組み替え対象コース
                foreach (TargetCourse targetCourse in _targetCourseMap.Values)
                {
                    if (targetCourse.コースＣＤ == null)
                    {
                        continue;
                    }

                    // 臨時コースマスタ取得
                    RinjiCourse rinjiCourse = RinjiCourse.GetRinjiCourse((int)targetCourse.コースＣＤ, (DateTime)_courseRinjiCriteria.配送日);
                    if (rinjiCourse != null)
                    {
                        targetCourse.RinjiCourseMasterBindingSource.DataSource = rinjiCourse;
                        targetCourse.RinjiCourseOrigin = new RinjiCourse(rinjiCourse);          // 初期状態として保持

                        // 臨時コーステーブル取得
                        GetRinjiCourseTable(targetCourse, (DateTime)_courseRinjiCriteria.配送日);
                        targetCourse.LblChanged.Visible = false;
                        targetCourse.BtnCourseDelete.Enabled = true;
                    }
                    else
                    {
                        // **********************************************************************************************************
                        // 臨時コーステーブルが削除されて臨時コースマスタも削除された場合は、コースマスタとコーステーブルの表示も消す
                        // **********************************************************************************************************

                        // 組み替え対象コース指定ボタン(チェックボックス)をOFFにする
                        _isCheckChanging = true;
                        targetCourse.ChkTarget.Checked = false;

                        // 対象のコースの背景色をクリア
                        for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                        {
                            if ((int)fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].Value == (int)targetCourse.コースＣＤ)
                            {
                                // 臨時コースがある場合は背景色を変える
                                if (fpSpread1_Sheet1.Cells[rowIndex, RINJI_FLG_COLUMN].Value as int? == 1)
                                {
                                    fpSpread1_Sheet1.Rows[rowIndex].BackColor = Color.PaleGreen;
                                }
                                else
                                {
                                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                                }
                                break;
                            }
                        }

                        // 対象のコースマスタ・コーステーブル・臨時コースマスタ・臨時コーステーブルを全てクリア
                        targetCourse.Clear();
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
            finally
            {
                _isCheckChanging = false;
            }
        }

        /// <summary>
        /// 臨時コースの削除結果を注文に反映する（元のコースに戻す）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="courseCD">削除した臨時コースＣＤ（指定しない場合はマイナス値）</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateChumonCourseOriginal(SqlConnection con, SqlTransaction tran, int courseCD)
        {
            History.ProcessType historyProcess = History.ProcessType.Update;

            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" UPDATE 注文ヘッダ");
                sql.AppendLine(" SET コースＣＤ = CM.コースＣＤ");
                sql.AppendLine(" FROM");
                sql.AppendLine("     注文ヘッダ AS CH");
                sql.AppendLine("     INNER JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             CT.企業ＣＤ,");
                sql.AppendLine("             CM.部署ＣＤ,");
                sql.AppendLine("             CM.コース区分,");
                sql.AppendLine("             CT.得意先ＣＤ,");
                sql.AppendLine("             MIN(CT.コースＣＤ) AS コースＣＤ");
                sql.AppendLine("         FROM");
                sql.AppendLine("             コーステーブル AS CT");
                sql.AppendLine("             INNER JOIN コースマスタ AS CM");
                sql.AppendLine("                 ON CM.企業ＣＤ = CT.企業ＣＤ");
                sql.AppendLine("                 AND CM.コースＣＤ = CT.コースＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             CT.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND ISNULL(CT.無効フラグ, 0) = 0");
                sql.AppendLine("             AND CM.部署ＣＤ = @部署ＣＤ");
                sql.AppendLine("             AND CM.コース区分 = @コース区分");
                sql.AppendLine("             AND ISNULL(CM.無効フラグ, 0) = 0");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             CT.企業ＣＤ,");
                sql.AppendLine("             CM.部署ＣＤ,");
                sql.AppendLine("             CM.コース区分,");
                sql.AppendLine("             CT.得意先ＣＤ");
                sql.AppendLine("         ) AS CM");
                sql.AppendLine("         ON CM.企業ＣＤ = CH.企業ＣＤ");
                sql.AppendLine("         AND CM.得意先ＣＤ = CH.得意先ＣＤ");
                // -- ┌────┬───┬───────┐
                // -- │注文区分│部署  │コース        │
                // -- ├────┼───┼───────┤
                // -- │予測    │ランチ│ランチコース1 │
                // -- ├────┼───┼───────┤
                // -- │注文    │ランチ│夕食コース1   │
                // -- └────┴───┴───────┘
                // -- 上記のような注文があるときに夕食のコースを組み替えた場合、
                // -- 夕食のコースのみが変わるべきである（ランチのコースは変わってはダメ）
                // -- したがって（注文の部署ではなく）注文が持つコースの部署が検索条件と一致するレコードを更新対象にする
                sql.AppendLine("     INNER JOIN コースマスタ AS CM2");
                sql.AppendLine("         ON CM2.企業ＣＤ = CH.企業ＣＤ");
                sql.AppendLine("         AND CM2.コースＣＤ = CH.コースＣＤ");
                sql.AppendLine(" WHERE");
                sql.AppendLine("     CH.配送日 = @配送日");
                sql.AppendLine("     AND CM2.部署ＣＤ = @部署ＣＤ");
                if (courseCD >= 0)
                {
                    sql.AppendLine("     AND CH.コースＣＤ = @コースＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                }
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseRinjiCriteria.部署ＣＤ;
                cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _courseRinjiCriteria.コース区分;
                cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = (DateTime)_courseRinjiCriteria.配送日;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                string code = string.Format("コースＣＤ=[{0}] 実施日=[{1}]", courseCD, (DateTime)_courseRinjiCriteria.配送日);
                History.Save(con, tran, this.Text, "注文ヘッダのコース", code, historyProcess);
                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("注文ヘッダが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 臨時コースの内容を注文に反映する（臨時コースを反映する）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateChumonCourseRinji(SqlConnection con, SqlTransaction tran)
        {
            History.ProcessType historyProcess = History.ProcessType.Update;

            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" UPDATE 注文ヘッダ");
                sql.AppendLine(" SET コースＣＤ = RM.コースＣＤ");
                sql.AppendLine(" FROM");
                sql.AppendLine("     注文ヘッダ AS CH");
                sql.AppendLine("     INNER JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             RT.企業ＣＤ,");
                sql.AppendLine("             RM.部署ＣＤ,");
                sql.AppendLine("             RM.コース区分,");
                sql.AppendLine("             RT.得意先ＣＤ,");
                sql.AppendLine("             RM.実施日,");
                sql.AppendLine("             MIN(RT.コースＣＤ) AS コースＣＤ");
                sql.AppendLine("         FROM");
                sql.AppendLine("             臨時コーステーブル AS RT");
                sql.AppendLine("             INNER JOIN 臨時コースマスタ AS RM");
                sql.AppendLine("                 ON RM.企業ＣＤ = RT.企業ＣＤ");
                sql.AppendLine("                 AND RM.コースＣＤ = RT.コースＣＤ");
                sql.AppendLine("                 AND RM.実施日 = RT.実施日");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             RT.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND ISNULL(RT.無効フラグ, 0) = 0");
                sql.AppendLine("             AND RM.実施日 = @配送日");
                sql.AppendLine("             AND RM.部署ＣＤ = @部署ＣＤ");
                sql.AppendLine("             AND RM.コース区分 = @コース区分");
                sql.AppendLine("             AND ISNULL(RM.無効フラグ, 0) = 0");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             RT.企業ＣＤ,");
                sql.AppendLine("             RM.部署ＣＤ,");
                sql.AppendLine("             RM.コース区分,");
                sql.AppendLine("             RT.得意先ＣＤ,");
                sql.AppendLine("             RM.実施日");
                sql.AppendLine("         ) AS RM");
                sql.AppendLine("         ON RM.企業ＣＤ = CH.企業ＣＤ");
                sql.AppendLine("         AND RM.得意先ＣＤ = CH.得意先ＣＤ");
                sql.AppendLine("         AND RM.実施日 = CH.配送日");
                // -- ┌────┬───┬───────┐
                // -- │注文区分│部署  │コース        │
                // -- ├────┼───┼───────┤
                // -- │予測    │ランチ│ランチコース1 │
                // -- ├────┼───┼───────┤
                // -- │注文    │ランチ│夕食コース1   │
                // -- └────┴───┴───────┘
                // -- 上記のような注文があるときに夕食のコースを組み替えた場合、
                // -- 夕食のコースのみが変わるべきである（ランチのコースは変わってはダメ）
                // -- したがって（注文の部署ではなく）注文が持つコースの部署が検索条件と一致するレコードを更新対象にする
                sql.AppendLine("     INNER JOIN 臨時コースマスタ AS RM2");
                sql.AppendLine("         ON RM2.企業ＣＤ = CH.企業ＣＤ");
                sql.AppendLine("         AND RM2.コースＣＤ = CH.コースＣＤ");
                sql.AppendLine("         AND RM2.実施日 = CH.配送日");
                sql.AppendLine(" WHERE");
                sql.AppendLine("     RM2.部署ＣＤ = @部署ＣＤ");

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseRinjiCriteria.部署ＣＤ;
                cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _courseRinjiCriteria.コース区分;
                cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = (DateTime)_courseRinjiCriteria.配送日;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                string code = string.Format("実施日=[{0}]", (DateTime)_courseRinjiCriteria.配送日);
                History.Save(con, tran, this.Text, "注文ヘッダのコース", code, historyProcess);
                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("注文ヘッダが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
    }
}
