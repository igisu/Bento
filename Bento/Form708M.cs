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
    /// コース移動
    /// </summary>
    public partial class Form708M : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 移動元コースマスタ
        /// </summary>
        private Course _course;

        /// <summary>
        /// 移動先コースマスタ表示条件
        /// </summary>
        private CourseMoveCriteria _courseMoveCriteria;

        /// <summary>
        /// 移動先コースマスタ表示条件スナップショット
        /// </summary>
        private CourseMoveCriteria _courseMoveCriteriaSnapShot = null;

        /// <summary>
        /// マウスの押された位置（ドラッグ処理用）
        /// </summary>
        private Point _mouseDownPoint = Point.Empty;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="course"></param>
        public Form708M(Course course)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _course = new Course(course);
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
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form708M_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                courseBindingSource.DataSource = _course;

                // 移動元コーステーブルを取得
                GetCourseTable(lstLeft, (int)_course.コースＣＤ);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _courseMoveCriteria = new CourseMoveCriteria();
                courseMoveBindingSource.DataSource = _courseMoveCriteria;

                // リセットボタンは表示ボタン押下まで使用不可
                btnReset.Enabled = false;
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form708M_Shown(object sender, EventArgs e)
        {
            // 移動先コースにフォーカス
            txtCourseCDRight.Focus();
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form708M_KeyDown(object sender, KeyEventArgs e)
        {
                switch (e.KeyCode)
                {
                    case Keys.F9:                       // 更新ボタン押下
                        WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                        btnSave.PerformClick();
                        break;

                    // 終了はフォームのCancelButtonなのでEscキーとしていたが、
                    // (ボタンのクリックではなく)キー押下でフォームを閉じたときに親フォームがアクティブにならないという問題がある。
                    // これを回避するには当該キー押下時に当該ボタンにフォーカスすればよいのだが、
                    // CancelButtonだとイベントが発生せずEscキー押下を検知できず、ボタンにフォーカスする契機が無い。
                    case Keys.Escape:
                        btnQuit.Focus();
                        btnQuit.PerformClick();
                        break;
                }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form708M_FormClosing(object sender, FormClosingEventArgs e)
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
                if (HasChange(courseTableLeftBindingSource) || HasChange(courseTableRightBindingSource))
                {
                    DialogResult r = MessageBox.Show(this, "変更されています。更新しますか？", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // 入力チェック
                            if (CanSave() == false)
                            {
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
        /// コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCDRight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCDRight.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCDRight_Click(object sender, EventArgs e)
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
                    errorProvider1.SetError(txtCourseCDRight, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCDRight.Text = course.コースＣＤ.ToString();
                    _courseMoveCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    // コントロールに反映
                    ReadBinding(lblCourseNameRight);
                    ReadBinding(lblCourseKbnRight);
                    ReadBinding(lblBushoRight);
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_courseMoveCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_courseMoveCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _courseMoveCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 移動元と同じ？
                if (_course.コースＣＤ == _courseMoveCriteria.コースＣＤ)
                {
                    MessageBox.Show(this, "移動元と同じコースは指定できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 初期状態と異なる？
                if (HasChange(courseTableRightBindingSource))
                {
                    if (MessageBox.Show(this, "移動先の変更内容を破棄して再読み込みします。\n\n※移動元も再読み込みする場合はリセットボタンを使用してください。", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 移動先コーステーブルを取得
                GetCourseTable(lstRight, (int)_courseMoveCriteria.コースＣＤ);

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _courseMoveCriteriaSnapShot = new CourseMoveCriteria(_courseMoveCriteria);

                // リセットボタン使用可能
                btnReset.Enabled = true;
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
        /// リストボックス マウスボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                // マウスの左ボタンだけが押されている時のみドラッグできるようにする
                if (e.Button == MouseButtons.Left)
                {
                    // ドラッグの準備
                    ListBox lbx = (ListBox)sender;
                    // マウスの押された位置を記憶
                    if (lbx.IndexFromPoint(e.X, e.Y) >= 0)
                    {
                        _mouseDownPoint = new Point(e.X, e.Y);
                    }
                }
                else
                {
                    _mouseDownPoint = Point.Empty;
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
        /// リストボックス マウス移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_mouseDownPoint != Point.Empty)
                {
                    // ドラッグとしないマウスの移動範囲を取得する
                    Rectangle moveRect = new Rectangle(
                        _mouseDownPoint.X - SystemInformation.DragSize.Width / 2,
                        _mouseDownPoint.Y - SystemInformation.DragSize.Height / 2,
                        SystemInformation.DragSize.Width,
                        SystemInformation.DragSize.Height);

                    // ドラッグとする移動範囲を超えたか調べる
                    if (moveRect.Contains(e.X, e.Y) == false)
                    {
                        // ドラッグの準備
                        ListBox lbx = (ListBox)sender;
                        // ドラッグするアイテムのインデックスを取得する
                        int itemIndex = lbx.IndexFromPoint(_mouseDownPoint);
                        if (itemIndex < 0)
                        {
                            return;
                        }

                        // 移動先が検索されていない？
                        DataSet ds = (DataSet)courseTableRightBindingSource.DataSource;
                        if (ds == null)
                        {
                            return;
                        }

                        // ドラッグするアイテムの内容を取得する
                        DataRow drCurrent = ((DataRowView)lbx.Items[itemIndex]).Row;

                        // ドラッグ＆ドロップ処理を開始する
                        DragDropEffects dde = lbx.DoDragDrop(drCurrent, DragDropEffects.Move | DragDropEffects.Scroll);
                        // ドロップ効果がMoveの時はもとのアイテムを削除する（ドロップ後にこのコードを通る）
                        if (dde == DragDropEffects.Move)
                        {
                            drCurrent.Delete();
                        }
                        _mouseDownPoint = Point.Empty;
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
        /// リストボックス マウスボタン離す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                _mouseDownPoint = Point.Empty;
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
        /// リストボックス ドラッグ問い合わせ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            try
            {
                // マウスの右ボタンが押されていればドラッグをキャンセル
                // "2"はマウスの右ボタンを表す
                if ((e.KeyState & 2) == 2)
                {
                    e.Action = DragAction.Cancel;
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
        /// リストボックス ドラッグ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                // ドラッグされているデータがDataRow型か調べる
                // DataRow型でなければ受け入れない
                // ※ここの処理でマウスカーソルが変わるので省略できない
                if (e.Data.GetDataPresent(typeof(DataRow)))
                {
                    // ドラッグを開始したリストボックスに対するドロップはできないようにする
                    DataRow drCurrent = (DataRow)e.Data.GetData(typeof(DataRow));
                    CourseTable courseTableCurrent = new CourseTable(drCurrent);
                    ListBox lbx = (ListBox)sender;
                    if (lbx == lstLeft && courseTableCurrent.コースＣＤ == _course.コースＣＤ)
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                    if (lbx == lstRight && courseTableCurrent.コースＣＤ == _courseMoveCriteria.コースＣＤ)
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                    e.Effect = DragDropEffects.Move;
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
        /// リストボックス ドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // ドラッグされているデータがDataRow型か調べる
                if (e.Data.GetDataPresent(typeof(DataRow)))
                //if (e.Data.GetDataPresent(typeof(string)))
                {
                    ListBox lstTarget = (ListBox)sender;
                    // ドロップされたデータ(DataRow型)を取得して、CourseTable型として取得
                    DataRow drCurrent = (DataRow)e.Data.GetData(typeof(DataRow));
                    CourseTable courseTableCurrent = new CourseTable(drCurrent);

                    // コースＣＤを、ドロップされた方に合わせる
                    if (lstTarget == lstLeft)
                    {
                        courseTableCurrent.コースＣＤ = _course.コースＣＤ;
                    }
                    else
                    {
                        courseTableCurrent.コースＣＤ = _courseMoveCriteria.コースＣＤ;
                    }

                    // リストボックスのデータソースであるDataTableを取得
                    BindingSource bindingSource = (BindingSource)lstTarget.DataSource;
                    DataSet ds = (DataSet)bindingSource.DataSource;
                    if (ds == null)
                    {
                        return;
                    }

                    // ドロップ元のデータをドロップ先にセット
                    DataTable dt = ds.Tables["コーステーブル"];
                    DataRow drNew = dt.NewRow();
                    courseTableCurrent.SetDataRow(drNew);

                    //********************************************************************************************************************
                    // 得意先を左右に移動することにより、リストボックスからは削除されるが、
                    // そのデータソースであるDataTableにはレコードは残ったまま（RowStateがDeletedとして残っている）
                    // したがって、リストスボックスが指すインデックスに追加するのではなく、DataTableの適切な位置に追加しないといけない。
                    // リストボックスが指すインデックスにあるレコードが、DataTableのどの位置にあるのかを求めて、そこに追加する必要がある。
                    //********************************************************************************************************************
                    // ドロップされた位置に該当するインデックスを取得
                    int itemIndex = lstTarget.IndexFromPoint(lstTarget.PointToClient(new Point(e.X, e.Y)));
                    if (itemIndex < 0)
                    {
                        // 範囲外なので末尾に追加する
                        dt.Rows.Add(drNew);
                    }
                    else
                    {
                        DataRow drTarget = ((DataRowView)lstTarget.Items[itemIndex]).Row;   // インデックスに該当するレコードを取得
                        int targetIndex = dt.Rows.IndexOf(drTarget);                        // DataTableへの追加先インデックスを取得
                        dt.Rows.InsertAt(drNew, targetIndex);
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
        /// 移動元いちばん上へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTopLeft_Click(object sender, EventArgs e)
        {
            try
            {
                // いちばん上に移動
                MoveTop(courseTableLeftBindingSource);
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
        /// 移動元上へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpLeft_Click(object sender, EventArgs e)
        {
            try
            {
                // 上移動
                MoveUp(courseTableLeftBindingSource);
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
        /// 移動元下へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownLeft_Click(object sender, EventArgs e)
        {
            try
            {
                // 下移動
                MoveDown(courseTableLeftBindingSource);
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
        /// 移動元いちばん下へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBottomLeft_Click(object sender, EventArgs e)
        {
            try
            {
                // いちばん下に移動
                MoveBottom(courseTableLeftBindingSource);
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
        /// 移動先いちばん上へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTopRight_Click(object sender, EventArgs e)
        {
            try
            {
                // いちばん上に移動
                MoveTop(courseTableRightBindingSource);
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
        /// 移動先上へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpRight_Click(object sender, EventArgs e)
        {
            try
            {
                // 上移動
                MoveUp(courseTableRightBindingSource);
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
        /// 移動先下へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownRight_Click(object sender, EventArgs e)
        {
            try
            {
                // 下移動
                MoveDown(courseTableRightBindingSource);
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
        /// 移動先いちばん下へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBottomRight_Click(object sender, EventArgs e)
        {
            try
            {
                // いちばん下に移動
                MoveBottom(courseTableRightBindingSource);
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
        /// 右移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e)
        {
            try
            {
                // 左から右に移動
                Transfer(courseTableLeftBindingSource, courseTableRightBindingSource);
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
        /// 全右移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRightAll_Click(object sender, EventArgs e)
        {
            try
            {
                // 左から右に移動
                TransferAll(courseTableLeftBindingSource, courseTableRightBindingSource);
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
        /// 左移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeft_Click(object sender, EventArgs e)
        {
            try
            {
                // 右から左に移動
                Transfer(courseTableRightBindingSource, courseTableLeftBindingSource);
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
        /// 全左移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeftAll_Click(object sender, EventArgs e)
        {
            try
            {
                // 右から左に移動
                TransferAll(courseTableRightBindingSource, courseTableLeftBindingSource);
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
        /// リセットボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_courseMoveCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_courseMoveCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _courseMoveCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 移動元と同じ？
                if (_course.コースＣＤ == _courseMoveCriteria.コースＣＤ)
                {
                    MessageBox.Show(this, "移動元と同じコースは指定できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show(this, "変更内容を破棄して再読み込みします。", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                // 再読み込み
                Reload();

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _courseMoveCriteriaSnapShot = new CourseMoveCriteria(_courseMoveCriteria);
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
        /// コーステーブル取得
        /// </summary>
        /// <param name="listBox">リストボックス</param>
        /// <param name="courseCD">コースＣＤ</param>
        private void GetCourseTable(ListBox listBox, int courseCD)
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

                    BindingSource bindingSource = (BindingSource)listBox.DataSource;

                    bindingSource.DataSource = ds;
                    bindingSource.DataMember = "コーステーブル";

                    listBox.DisplayMember = "得意先名略称";
                    listBox.ValueMember = "得意先ＣＤ";

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
        /// いちばん上に移動
        /// </summary>
        /// <param name="bindingSource">バインディングソース</param>
        private void MoveTop(BindingSource bindingSource)
        {
            try
            {
                // 先頭行以外の場合
                if (bindingSource.Position > 0)
                {
                    this.MdiParent.Cursor = Cursors.WaitCursor;
                    lstLeft.BeginUpdate();
                    lstRight.BeginUpdate();

                    while (bindingSource.Position > 0)
                    {
                        // 現在行を取得
                        DataRow drCurrent = ((DataRowView)bindingSource.Current).Row;
                        CourseTable courseTableCurrent = new CourseTable(drCurrent);

                        // 有効な前の行を取得
                        bindingSource.MovePrevious();
                        DataRow drTarget = ((DataRowView)bindingSource.Current).Row;
                        CourseTable courseTableTarget = new CourseTable(drTarget);

                        // 現在行と前の行を交換
                        courseTableCurrent.SetDataRow(drTarget);
                        courseTableTarget.SetDataRow(drCurrent);
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
                this.MdiParent.Cursor = Cursors.Default;
                lstLeft.EndUpdate();
                lstRight.EndUpdate();
            }
        }

        /// <summary>
        /// 上移動
        /// </summary>
        /// <param name="bindingSource">バインディングソース</param>
        private void MoveUp(BindingSource bindingSource)
        {
            try
            {
                // 先頭行以外の場合
                if (bindingSource.Position > 0)
                {
                    // 現在行を取得
                    DataRow drCurrent = ((DataRowView)bindingSource.Current).Row;
                    CourseTable courseTableCurrent = new CourseTable(drCurrent);

                    // 有効な前の行を取得
                    bindingSource.MovePrevious();
                    DataRow drTarget = ((DataRowView)bindingSource.Current).Row;
                    CourseTable courseTableTarget = new CourseTable(drTarget);

                    // 現在行と前の行を交換
                    courseTableCurrent.SetDataRow(drTarget);
                    courseTableTarget.SetDataRow(drCurrent);
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
        /// 下移動
        /// </summary>
        /// <param name="bindingSource">バインディングソース</param>
        private void MoveDown(BindingSource bindingSource)
        {
            try
            {
                // 最終行以外場合
                if (bindingSource.Position < bindingSource.Count - 1)
                {
                    // 現在行を取得
                    DataRow drCurrent = ((DataRowView)bindingSource.Current).Row;
                    CourseTable courseTableCurrent = new CourseTable(drCurrent);

                    // 有効な次の行を取得
                    bindingSource.MoveNext();
                    DataRow drTarget = ((DataRowView)bindingSource.Current).Row;
                    CourseTable courseTableTarget = new CourseTable(drTarget);

                    // 現在行と次の行を交換
                    courseTableCurrent.SetDataRow(drTarget);
                    courseTableTarget.SetDataRow(drCurrent);
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
        /// いちばん下に移動
        /// </summary>
        /// <param name="bindingSource">バインディングソース</param>
        private void MoveBottom(BindingSource bindingSource)
        {
            try
            {
                // 最終行以外場合
                if (bindingSource.Position < bindingSource.Count - 1)
                {
                    this.MdiParent.Cursor = Cursors.WaitCursor;
                    lstLeft.BeginUpdate();
                    lstRight.BeginUpdate();

                    while (bindingSource.Position < bindingSource.Count - 1)
                    {
                        // 現在行を取得
                        DataRow drCurrent = ((DataRowView)bindingSource.Current).Row;
                        CourseTable courseTableCurrent = new CourseTable(drCurrent);

                        // 有効な次の行を取得
                        bindingSource.MoveNext();
                        DataRow drTarget = ((DataRowView)bindingSource.Current).Row;
                        CourseTable courseTableTarget = new CourseTable(drTarget);

                        // 現在行と次の行を交換
                        courseTableCurrent.SetDataRow(drTarget);
                        courseTableTarget.SetDataRow(drCurrent);
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
                this.MdiParent.Cursor = Cursors.Default;
                lstLeft.EndUpdate();
                lstRight.EndUpdate();
            }
        }

        /// <summary>
        /// 左右の移動
        /// </summary>
        /// <param name="bindingSourceFrom">移動元BindingSource</param>
        /// <param name="bindingSourceTo">移動先BindingSource</param>
        private void Transfer(BindingSource bindingSourceFrom, BindingSource bindingSourceTo)
        {
            try
            {
                // 移動元未選択？
                if (bindingSourceFrom.Position < 0)
                {
                    return;
                }

                // 移動元行を取得
                DataRow drCurrent = ((DataRowView)bindingSourceFrom.Current).Row;
                CourseTable courseTableCurrent = new CourseTable(drCurrent);

                // 移動先の新しい行を生成
                DataSet ds = (DataSet)bindingSourceTo.DataSource;
                if (ds == null)
                {
                    return;
                }
                DataTable dt = ds.Tables["コーステーブル"];
                DataRow drTarget = dt.NewRow();

                // 移動元を移動先にセット
                courseTableCurrent.SetDataRow(drTarget);

                drCurrent.Delete();     // 移動元を削除
                dt.Rows.Add(drTarget);  // 移動先を追加
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
        /// 左右の全移動
        /// </summary>
        /// <param name="bindingSourceFrom">移動元BindingSource</param>
        /// <param name="bindingSourceTo">移動先BindingSource</param>
        private void TransferAll(BindingSource bindingSourceFrom, BindingSource bindingSourceTo)
        {
            try
            {
                // 移動元に行が無い？
                if (bindingSourceFrom.Position < 0)
                {
                    return;
                }

                // 移動先に行が無い？
                DataSet ds = (DataSet)bindingSourceTo.DataSource;
                if (ds == null)
                {
                    return;
                }

                // 移動元の行が無くなるまで
                while (bindingSourceFrom.Count > 0)
                {
                    bindingSourceFrom.Position = 0;                 // 先頭行を選択
                    Transfer(bindingSourceFrom, bindingSourceTo);   // 移動
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
        /// コーステーブルが変更されているかを返します。
        /// </summary>
        /// <param name="bindingSource">バインディングソース</param>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange(BindingSource bindingSource)
        {
            // コーステーブルを比較
            DataSet ds = (DataSet)bindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["コーステーブル"];

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

                // 検索されていない？
                if (_courseMoveCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "移動先が表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_courseMoveCriteria.Equals(_courseMoveCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 移動元と移動先とで、コース区分と部署のいずれかが一致していない？
                if (_course.コース区分 != _courseMoveCriteria.コース区分 ||
                    _course.部署ＣＤ != _courseMoveCriteria.部署ＣＤ)
                {
                    if (MessageBox.Show(this, "コース区分 または 部署が異なるコースです。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel)
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
        /// <returns>ダイアログを閉じる場合はtrue</returns>
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
                    History.Save(con, tran, string.Format("コース移動（移動元[{0}] 移動先[{1}]）を開始しました。", _course.コースＣＤ, _courseMoveCriteria.コースＣＤ));

                    // 移動元コーステーブル保存
                    if (SaveCourseTable(con, tran, courseTableLeftBindingSource, (int)_course.コースＣＤ) == false)
                    {
                        return false;
                    }

                    // 移動先コーステーブル保存
                    if (SaveCourseTable(con, tran, courseTableRightBindingSource, (int)_courseMoveCriteria.コースＣＤ) == false)
                    {
                        return false;
                    }

                    // 移動元コースにぶら下がる得意先に対して、コースＣＤを注文に反映する
                    if (UpdateChumonCourseOriginal(con, tran, courseTableLeftBindingSource, courseTableRightBindingSource, (int)_course.コースＣＤ, (int)_courseMoveCriteria.コースＣＤ) == false)
                    {
                        return false;
                    }

                    // 移動先コースにぶら下がる得意先に対して、コースＣＤを注文に反映する
                    if (UpdateChumonCourseOriginal(con, tran, courseTableRightBindingSource, courseTableLeftBindingSource, (int)_courseMoveCriteria.コースＣＤ, (int)_course.コースＣＤ) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("コーステーブルを{0}しました。", historyProcess.DisplayName());
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
        /// コーステーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="bindingSource">バインディングソース</param>
        /// <param name="courseCD">コースCD</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveCourseTable(SqlConnection con, SqlTransaction tran, BindingSource bindingSource, int courseCD)
        {
            try
            {
                DataSet ds = (DataSet)bindingSource.DataSource;
                DataTable dt = ds.Tables["コーステーブル"];

                History.ProcessType historyProcess = History.ProcessType.Create;
                int seq = 1;
                bool ret = false;

                // 全削除→全追加
                SqlCommand cmdDel = con.CreateCommand();
                cmdDel.CommandText = "DELETE FROM コーステーブル WHERE 企業ＣＤ = @企業ＣＤ AND コースＣＤ = @コースＣＤ";
                cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmdDel.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                cmdDel.Transaction = tran;
                log.Debug(cmdDel.CommandTextWithParameters());
                cmdDel.ExecuteNonQuery();

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    CourseTable courseTable = new CourseTable(dr);

                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    courseTable.コースＣＤ = courseCD;
                    courseTable.コース順 = seq;
                    seq++;
                    ret = courseTable.Insert(con, tran);
                    historyProcess = History.ProcessType.Create;

                    if (ret == false)
                    {
                        MessageBox.Show(this, courseTable.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "コーステーブル", courseTable.Code, historyProcess);
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
        /// コースにぶら下がる得意先に対して、コースＣＤを注文に反映する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="bindingSource">バインディングソース</param>
        /// <param name="bindingSourceAnother">もう片方のバインディングソース</param>
        /// <param name="masterCourseCD">主コースCD（移動元 または 移動先）</param>
        /// <param name="slaveCourseCD">従コースCD（masterCourseCDが移動元の場合は移動先、masterCourseCDが移動先の場合は移動元）</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateChumonCourseOriginal(SqlConnection con, SqlTransaction tran, BindingSource bindingSource, BindingSource bindingSourceAnother, int masterCourseCD, int slaveCourseCD)
        {
            History.ProcessType historyProcess = History.ProcessType.Update;
            bool ret = true;

            try
            {
                // コース臨時組み替えのような一度のSQLでは実現できない。
                // ※コーステーブルから削除された得意先については注文が取得できないので。
                // そこで、データソースのレコードの内容に応じて、以下のように処理を振り分ける。
                // ただし、得意先・配送日に該当する臨時コースがあれば、そちらを優先する。
                // ┌───────────┬────────────────────────────────────────────┐
                // │行の状態              │処理内容                                                                                │
                // ├───────────┼────────────────────────────────────────────┤
                // │変更無し              │注文への反映不要                                                                        │
                // ├───────────┼────────────────────────────────────────────┤
                // │追加（移動元→先）    │移動先を処理している場合、注文のコースＣＤが移動元と同じであれば移動先のコースＣＤで更新│
                // ├───────────┼────────────────────────────────────────────┤
                // │追加（移動元→先→元）│移動先を処理している場合、注文のコースＣＤが移動先と同じであれば何もしない              │
                // ├───────────┼────────────────────────────────────────────┤
                // │削除                  │追加とセットになっているので気にしなくてよい                                            │
                // └───────────┴────────────────────────────────────────────┘
                // しかし、移動（追加）したものを上下に動かしても、行の状態も一緒に動くわけではない。
                // （いちばん下に追加されて、それを上下に動かしても、RowStateがAddedの行は常にいちばん下）
                // なので、削除されたものを除く全ての行について、追加されたものとみなして処理を行う。
                // ※むしろ削除された行だけを対象にすれば、他方に追加された行を対象にしていることになるはずだが、
                //   削除された行に対して処理を行うことに抵抗を感じるし、バグを誘発しかねないので却下。

                // ※コースマスタ詳細と異なり、注文のコースＣＤが異なる場合でも更新していることに注意
                // ┌────────┬───────────────────────────────────────────┐
                // │コースマスタ詳細│追加された得意先が別のコースに属していることになる。そのコースＣＤを勝手に変えてはダメ│
                // ├────────┼───────────────────────────────────────────┤
                // │コース移動      │指定されたコースを互いに交換している                                                  │
                // └────────┴───────────────────────────────────────────┘

                // 当該得意先の未来日の注文ヘッダを取得するSQL
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.注文区分" +
                             "\r\n     ,A.喫食日" +
                             "\r\n     ,A.得意先ＣＤ" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,A.配送日" +
                             "\r\n     ,A.得意先名" +
                             "\r\n     ,A.担当者ＣＤ" +
                             "\r\n     ,A3.担当者名 AS 担当者名" +
                             "\r\n     ,A.コースＣＤ" +
                             "\r\n     ,A.配送先ＣＤ" +
                             "\r\n     ,A.税区分" +
                             "\r\n     ,A.丸め処理区分" +
                             "\r\n     ,A.備考1" +
                             "\r\n     ,A.備考2" +
                             "\r\n     ,A.備考3" +
                             "\r\n     ,A.備考4" +
                             "\r\n     ,A.備考5" +
                             "\r\n     ,A.納品書伝票番号" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     注文ヘッダ AS A" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A2.企業ＣＤ = @企業ＣＤ AND A.担当者ＣＤ = A3.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                             "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                             "\r\n AND A.配送日     > cast(getdate() as date)";     // 未来の日付

                // もう片方のデータテーブルを取得
                DataSet anotherDs = (DataSet)bindingSourceAnother.DataSource;
                DataTable anotherDt = anotherDs.Tables["コーステーブル"];

                // 行数分ループ
                DataSet ds = (DataSet)bindingSource.DataSource;
                DataTable dt = ds.Tables["コーステーブル"];
                foreach (DataRow dr in dt.Rows)
                {
                    CourseTable courseTable = new CourseTable(dr);

                    // 削除以外の行について
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        // もう片方のリスト（のデータテーブル）にも同じ得意先が存在する場合は、移動はしていないということなので、何もしない
                        DataRow[] drs = anotherDt.Select(string.Format("得意先ＣＤ={0}", courseTable.得意先ＣＤ));
                        if (drs != null && drs.Length != 0)
                        {
                            continue;
                        }

                        // 当該得意先の未来日の注文ヘッダを取得する
                        SqlCommand cmd = con.CreateCommand();
                        cmd.Transaction = tran;
                        cmd.CommandText = sql;
                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = courseTable.得意先ＣＤ;

                        log.Debug(cmd.CommandTextWithParameters());

                        SqlDataAdapter adp = new SqlDataAdapter(cmd);

                        DataTable dtChumonHeader = new DataTable("注文ヘッダ");
                        adp.Fill(dtChumonHeader);

                        foreach (DataRow drChumonHeader in dtChumonHeader.Rows)
                        {
                            ChumonHeader chumonHeader = new ChumonHeader(drChumonHeader);
                            // （移動先を処理している場合）注文のコースＣＤが移動元と同じであれば移動先のコースＣＤで更新
                            if (chumonHeader.コースＣＤ == slaveCourseCD)
                            {
                                // ただし臨時コースが存在すれば、それが適用されているはずなので更新しない
                                ret = IsExistRinjiCourse(con, tran, chumonHeader);
                                if (ret == false)
                                {
                                    chumonHeader.コースＣＤ = masterCourseCD;
                                    ret = chumonHeader.UpdateCourse(con, tran);
                                    if (ret == false)
                                    {
                                        MessageBox.Show(this, chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return false;
                                    }
                                    History.Save(con, tran, string.Format("{0}で未来の注文ヘッダのコース（{1}）を{2}から{3}へ移動しました。", this.Text, chumonHeader.Code, slaveCourseCD, masterCourseCD));
                                }
                            }
                        }
                    }
                }

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
        /// 注文の得意先・配送日に該当する臨時コースが存在するか（存在すれば適用されているはずなので、つまり臨時コースが適用されているか）を返す
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="chumonHeader">更新対象の注文ヘッダ</param>
        /// <returns>臨時コースが存在する場合は True</returns>
        private bool IsExistRinjiCourse(SqlConnection con, SqlTransaction tran, ChumonHeader chumonHeader)
        {
            bool ret = false;

            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     count(企業ＣＤ) AS CNT" +
                                  "\r\n FROM" +
                                  "\r\n     臨時コーステーブル" +
                                  "\r\n WHERE" +
                                  "\r\n     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND 実施日 = @実施日" +
                                  "\r\n     AND 得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n     AND ISNULL(無効フラグ, 0) = 0";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = chumonHeader.配送日;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = chumonHeader.得意先ＣＤ;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    if ((int)reader["CNT"] != 0)
                    {
                        ret = true;
                    }
                }
                return ret;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
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
                // 移動元コーステーブルを取得
                GetCourseTable(lstLeft, (int)_course.コースＣＤ);

                // 移動先コーステーブルを取得
                GetCourseTable(lstRight, (int)_courseMoveCriteria.コースＣＤ);
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
