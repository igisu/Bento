using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 一括入金入力
    /// </summary>
    public partial class Form603N : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 一括入金入力条件
        /// </summary>
        private IkkatsuNyukin _ikkatsuNyukin;

        /// <summary>
        /// 一括入金対象の入金明細
        /// </summary>
        private List<Nyukin> _nyukinList;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nyukinTarget"></param>
        public Form603N(List<Nyukin> nyukinList)
        {
            InitializeComponent();
            _nyukinList = nyukinList;
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
        private void Form603N_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.coins_ico;

                // 権限によりボタンを制御
                btnExec.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.SetNyukinHoho(cboNyukinHoho);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _ikkatsuNyukin = new IkkatsuNyukin();
                ikkatsuNyukinBindingSource.DataSource = _ikkatsuNyukin;
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form603N_Shown(object sender, EventArgs e)
        {
            // 入金日にフォーカス
            dtpNyukinbi.Focus();
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form603N_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// 入金確定ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_ikkatsuNyukin.HasErrors)
                {
                    if (string.IsNullOrEmpty(_ikkatsuNyukin.Error) == false)
                    {
                        MessageBox.Show(this, _ikkatsuNyukin.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 入金日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_ikkatsuNyukin.入金日 > warnDate)
                {
                    if (MessageBox.Show(this, "入金日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // 確認メッセージを表示
                if (MessageBox.Show(this, "一括入金処理を行います。よろしいですか？",
                    this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (Exec())
                    {
                        MessageBox.Show(this, "一括入金処理を実行しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
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
        /// 一括入金処理
        /// </summary>
        /// <returns>成功した場合は True</returns>
        private bool Exec()
        {
            decimal newNyukinNo = -1;       // 追加用入金番号

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    this.MdiParent.Cursor = Cursors.WaitCursor;
                    Application.DoEvents();

                    foreach (Nyukin nyukin in _nyukinList)
                    {
                        // 0以下の入金は無視する
                        if (nyukin.入金額 <= 0m)
                        {
                            continue;
                        }

                        // 入金番号、入金日、部署ＣＤ、入金方法を設定する
                        // 得意先ＣＤ、得意先名、入金入力区分、入金額、対応請求番号、備考は呼び出し元画面にて設定済み
                        // 依頼人ＣＤ、振込依頼人名、入金摘要区分は設定しない
                        SqlCommand cmdMax = con.CreateCommand();
                        cmdMax.Transaction = tran;
                        cmdMax.CommandText = "SELECT ISNULL(MAX(入金番号), 0) + 1 as 入金番号 FROM 入金明細 WHERE 企業ＣＤ = @企業ＣＤ";
                        cmdMax.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        using (SqlDataReader reader = cmdMax.ExecuteReader())
                        {
                            reader.Read();
                            newNyukinNo = (decimal)reader["入金番号"];
                        }
                        nyukin.入金番号 = newNyukinNo;
                        nyukin.入金日 = _ikkatsuNyukin.入金日;
                        nyukin.部署ＣＤ = _ikkatsuNyukin.部署ＣＤ;
                        nyukin.入金方法 = _ikkatsuNyukin.入金方法;

                        bool ret = nyukin.Insert(con, tran);
                        if (ret == false)
                        {
                            MessageBox.Show(this, nyukin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        History.Save(con, tran, this.Text, "入金明細", nyukin.Code, History.ProcessType.Create);
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("入金明細テーブルを{0}しました。", History.ProcessType.Create.DisplayName());

                    return true;
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
                finally
                {
                    this.MdiParent.Cursor = Cursors.Default;
                }
            }
        }
        #endregion
    }
}
