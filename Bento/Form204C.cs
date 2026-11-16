using System;
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
    /// 受注発信一覧コース指定
    /// </summary>
    public partial class Form204C : BaseForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int BUSYO_INDEX       = 0;   // 部署名列インデックス
        private const int COURSE_CD_INDEX   = 1;   // コースＣＤ列インデックス
        private const int COURSE_NAME_INDEX = 2;   // コース名列インデックス
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 担当者架電コースマスタのコース選択検索条件
        /// </summary>
        private CallCourseCriteria _callCourseCriteria;

        /// <summary>
        /// 右側リストビューのに対する変更の有無
        /// </summary>
        private bool _hasChange = false;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="course"></param>
        public Form204C()
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
        private void Form204C_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetCourseKubun(cboCourseKbn);
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _callCourseCriteria = new CallCourseCriteria();
                callCourseBindingSource.DataSource = _callCourseCriteria;

                // 担当者架電コースマスタを取得
                GetTantoKadenCourse();

                // 選択元のコースを取得
                btnQuery.PerformClick();
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
        private void Form204C_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F9:                       // 更新ボタン押下
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
        private void Form204C_FormClosing(object sender, FormClosingEventArgs e)
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_callCourseCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_callCourseCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _callCourseCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     B.部署名");
                    sql.AppendLine("     ,A.コースＣＤ");
                    sql.AppendLine("     ,A.コース名");
                    sql.AppendLine("     ,NULL AS 担当者ＣＤ");
                    sql.AppendLine("     ,NULL AS 作成担当者ＣＤ");
                    sql.AppendLine("     ,NULL AS 作成担当者名");
                    sql.AppendLine("     ,NULL AS 作成日");
                    sql.AppendLine("     ,NULL AS 修正担当者ＣＤ");
                    sql.AppendLine("     ,NULL AS 修正担当者名");
                    sql.AppendLine("     ,NULL AS 修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     コースマスタ AS A");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND ISNULL(A.無効フラグ, 0) = 0");
                    if (_callCourseCriteria.部署ＣＤ != null && _callCourseCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("     AND A.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _callCourseCriteria.部署ＣＤ;
                    }
                    if (_callCourseCriteria.コース区分 != null && _callCourseCriteria.コース区分 != -1)
                    {
                        sql.AppendLine("     AND A.コース区分 = @コース区分");
                        cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _callCourseCriteria.コース区分;
                    }
                    sql.AppendLine(" ORDER BY A.コースＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("コースマスタ");
                    adp.Fill(dt);

                    // 取得したデータテーブルの内容をリストビューに表示
                    ShowListView(lvwLeft, dt);

                    log.Info("コースマスタを検索しました。");
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
        /// リストビュー ドラッグ開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwLeft_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                // 複数選択されていてもe.Itemに渡されるのは１つなので、選択されている全てのアイテムをドラッグ対象にする
                lvwLeft.DoDragDrop((ListView.SelectedListViewItemCollection)lvwLeft.SelectedItems, DragDropEffects.Copy | DragDropEffects.Scroll);
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

                    // ドロップ先のアイテムを取得
                    Point p = this.lvwRight.PointToClient(new Point(e.X, e.Y));
                    ListViewItem item = this.lvwRight.GetItemAt(p.X, p.Y);
                    int destIndex = this.lvwRight.Items.IndexOf(item);

                    // アイテムが存在しない場所を選択すると、IndexOf() が　-1 を返すので、
                    // その場合はリストの最後に追加するようにします。
                    if (destIndex == -1)
                    {
                        destIndex = this.lvwRight.Items.Count;
                    }

                    // ドロップされたリストビューアイテムコレクションの要素を一つずつその場所に挿入する
                    foreach (ListViewItem targetItem in srcItems)
                    {
                        // ドラッグ先に同じものがあればスキップ
                        bool found = false;
                        foreach (ListViewItem destItem in lvwRight.Items)
                        {
                            if (targetItem.SubItems[COURSE_CD_INDEX].Text == destItem.SubItems[COURSE_CD_INDEX].Text)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            continue;
                        }

                        ListViewItem newItem = this.lvwRight.Items.Insert(destIndex, (ListViewItem)targetItem.Clone());
                        destIndex++;
                        newItem.Selected = true;
                        // Move の場合には、ソースのアイテムを削除してあげる必要があります。
                        // Shift Key (MOVE)
                        if ((e.KeyState & 0x4) > 0)
                        {
                            this.lvwLeft.Items.Remove(targetItem);
                        }
                        _hasChange = true;
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
                    // 未選択の場合はNOP
                    if (lvwRight.SelectedItems == null || lvwRight.SelectedItems.Count == 0)
                    {
                        return;
                    }
                    int index = lvwRight.SelectedIndices[0];    // 選択されている最初のインデックスを取得

                    // 選択されている項目を全て削除
                    foreach (ListViewItem item in lvwRight.SelectedItems)
                    {
                        lvwRight.Items.Remove(item);
                        _hasChange = true;
                    }

                    // 次に選択状態にするインデックスを決定
                    if (lvwRight.Items.Count <= index)
                    {
                        index = lvwRight.Items.Count - 1;
                    }
                    if (index >= 0)
                    {
                        lvwRight.Items[index].Selected = true;
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
        /// 右移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e)
        {
            try
            {
                // 左から右に移動
                CopyLeftToRight();
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
                CopyAllLeftToRight();
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
        /// 更新ボタン
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
        /// 担当者架電コースマスタに該当するコースマスタ取得
        /// </summary>
        private void GetTantoKadenCourse()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     B.部署名" +
                                      "\r\n     ,K.担当者ＣＤ" +
                                      "\r\n     ,K.コースＣＤ" +
                                      "\r\n     ,C.コース名" +
                                      "\r\n     ,K.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,K.作成日" +
                                      "\r\n     ,K.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,K.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     担当者架電コースマスタ AS K" +
                                      "\r\n     LEFT JOIN コースマスタ AS C ON C.企業ＣＤ = K.企業ＣＤ AND C.コースＣＤ = K.コースＣＤ" +
                                      "\r\n     LEFT JOIN 部署マスタ   AS B ON B.企業ＣＤ = C.企業ＣＤ AND B.部署ＣＤ = C.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = K.企業ＣＤ AND K.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = K.企業ＣＤ AND K.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     K.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND K.担当者ＣＤ = @担当者ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     K.コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("担当者架電コースマスタ");
                    adp.Fill(dt);

                    // 取得したデータテーブルの内容をリストビューに表示
                    ShowListView(lvwRight, dt);

                    log.Info("担当者架電コースマスタを検索しました。");

                    _hasChange = false;
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
        /// <param name="dt">データテーブル</param>
        private void ShowListView(ListView lvw, DataTable dt)
        {
            try
            {
                // リストビューの表示を一時停止
                lvw.BeginUpdate();

                lvw.Items.Clear();

                foreach (DataRow dr in dt.Rows)
                {
                    // データテーブルの行を、担当者架電コースマスタとして取り出す
                    TantoKadenCourse tantoKadenCourse = new TantoKadenCourse(dr);

                    // リストビューのアイテムに追加
                    ListViewItem item = new ListViewItem(tantoKadenCourse.部署名);
                    item.SubItems.Add((tantoKadenCourse.コースＣＤ == null ? "" : ((int)(tantoKadenCourse.コースＣＤ)).ToString()));    // COURSE_CD_INDEX
                    item.SubItems.Add(tantoKadenCourse.コース名);       // COURSE_NAME_INDEX
                    lvw.Items.Add(item);
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
                lvw.EndUpdate();
            }
        }

        /// <summary>
        /// 選択したコースのコピー
        /// </summary>
        private void CopyLeftToRight()
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

                // コピー元未選択？
                if (lvwLeft.SelectedItems == null || lvwLeft.SelectedItems.Count == 0)
                {
                    return;
                }

                // コピー先にコピー
                int destIndex = this.lvwRight.Items.Count;  // 常に末尾に追加
                foreach (ListViewItem srcItem in lvwLeft.SelectedItems)
                {
                    // コピー元のコースがコピー先にあればスキップ
                    bool found = false;
                    foreach (ListViewItem destItem in lvwRight.Items)
                    {
                        if (srcItem.SubItems[COURSE_CD_INDEX].Text == destItem.SubItems[COURSE_CD_INDEX].Text)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        continue;
                    }

                    ListViewItem newItem = this.lvwRight.Items.Insert(destIndex, (ListViewItem)srcItem.Clone());
                    destIndex++;
                    newItem.Selected = true;
                    _hasChange = true;
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
        /// 全コースのコピー
        /// </summary>
        private void CopyAllLeftToRight()
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

                // コピー元に行が無い？
                if (lvwLeft.Items == null || lvwLeft.Items.Count == 0)
                {
                    return;
                }

                // コピー先にコピー
                int destIndex = this.lvwRight.Items.Count;  // 常に末尾に追加
                foreach (ListViewItem srcItem in lvwLeft.Items)
                {
                    // コピー元のコースがコピー先にあればスキップ
                    bool found = false;
                    foreach (ListViewItem destItem in lvwRight.Items)
                    {
                        if (srcItem.SubItems[COURSE_CD_INDEX].Text == destItem.SubItems[COURSE_CD_INDEX].Text)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        continue;
                    }

                    ListViewItem newItem = this.lvwRight.Items.Insert(destIndex, (ListViewItem)srcItem.Clone());
                    destIndex++;
                    newItem.Selected = true;
                    _hasChange = true;
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
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            return _hasChange;
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
                    UserData userData = UserData.getInstance();

                    // 全削除→全追加
                    SqlCommand cmdDel = con.CreateCommand();
                    cmdDel.CommandText = "DELETE FROM 担当者架電コースマスタ WHERE 企業ＣＤ = @企業ＣＤ AND 担当者ＣＤ = @担当者ＣＤ";
                    cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmdDel.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmdDel.Transaction = tran;
                    log.Debug(cmdDel.CommandTextWithParameters());
                    cmdDel.ExecuteNonQuery();

                    // 行数分ループ
                    foreach (ListViewItem item in lvwRight.Items)
                    {
                        TantoKadenCourse tantoKadenCourse = new TantoKadenCourse();

                        tantoKadenCourse.担当者ＣＤ = userData.担当者ＣＤ;
                        tantoKadenCourse.コースＣＤ = int.Parse(item.SubItems[COURSE_CD_INDEX].Text);

                        bool ret = tantoKadenCourse.Insert(con, tran);
                        historyProcess = History.ProcessType.Create;

                        if (ret == false)
                        {
                            MessageBox.Show(this, tantoKadenCourse.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        History.Save(con, tran, this.Text, "担当者架電コースマスタ", tantoKadenCourse.Code, historyProcess);
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("担当者架電コースマスタを{0}しました。", historyProcess.DisplayName());
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                // 担当者架電コースマスタを取得
                GetTantoKadenCourse();
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
