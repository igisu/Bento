using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// ログイン画面
    /// </summary>
    public partial class FormLogin : BaseForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// ライセンスチェックをスキップするモードに入るための隠しコマンド（保守用）
        /// </summary>
        private static readonly Keys[] HIDDEN_COMMAND = { Keys.Up, Keys.Up,
                                                          Keys.Down, Keys.Down,
                                                          Keys.Left, Keys.Right,
                                                          Keys.Left, Keys.Right,
                                                          Keys.Home, Keys.End };
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 入力されたコマンド
        /// </summary>
        private List<Keys> _commandList = new List<Keys>(HIDDEN_COMMAND.Length + 1);

        /// <summary>
        /// ライセンスチェックをスキップするかどうか
        /// </summary>
        private bool _isSkipLicenseCheck;

        /// <summary>
        /// タイトルバーのテキストの初期値
        /// </summary>
        private string _thisTitle;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public FormLogin()
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
        private void FormLogin_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.icon;

                _thisTitle = this.Text;

                lblVersion.Text = string.Format("Version:{0}", Application.ProductVersion);

                // 前回ログインIDを表示
                string loginID = Properties.Settings.Default.LoginID;
                if (string.IsNullOrEmpty(loginID) == false)
                {
                    txtLoginID.Text = loginID;
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLogin_Shown(object sender, EventArgs e)
        {
            // ログインIDを復元した（＝前回ログイン済み）なら、パスワードにフォーカスする
            if (string.IsNullOrEmpty(txtLoginID.Text) == false)
            {
                txtPassword.Focus();
            }
        }

        /// <summary>
        /// パスワード クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_Click(object sender, EventArgs e)
        {
            // 隠しコマンド関連をクリア
            _isSkipLicenseCheck = false;
            this.Text = _thisTitle;
            _commandList.Clear();
        }

        /// <summary>
        /// パスワード キー解放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            // 既にライセンスチェックをスキップするモードに入っている場合はNOP
            if (_isSkipLicenseCheck)
            {
                return;
            }

            _commandList.Add(e.KeyCode);
            // 隠しコマンドの長さを超えないように、古いものを削除する
            if (_commandList.Count >= HIDDEN_COMMAND.Length)
            {
                _commandList.RemoveRange(0, _commandList.Count - (HIDDEN_COMMAND.Length));
                // 入力されたキーが隠しコマンドと同じ？
                if (_commandList.SequenceEqual(HIDDEN_COMMAND))
                {
                    _isSkipLicenseCheck = true;
                    this.Text += "●";
                }
            }
        }

        /// <summary>
        /// ログインボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // ログイン処理
                if (Login(txtLoginID.Text, txtPassword.Text))
                {
                    // ログインIDを保存
                    Properties.Settings.Default.LoginID = txtLoginID.Text;

                    if (_isSkipLicenseCheck == false)
                    {
                        // ライセンス的にログイン可能？
                        string ret = LoginCountControler.CanLogin();
                        if (string.IsNullOrEmpty(ret) == false)
                        {
                            MessageBox.Show(this, ret, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                    txtLoginID.Focus();
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
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// キャンセルボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
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
        /// ログイン処理
        /// </summary>
        /// <param name="loginID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool Login(string loginID, string password)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     担当者ＣＤ");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     担当者マスタ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("  企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("  AND ISNULL(無効フラグ, 0) = 0");
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = ConnectionInformation.CompanyCD;

                    if (string.IsNullOrEmpty(loginID))
                    {
                        sql.AppendLine(" AND ユーザーＩＤ IS NULL ");
                    }
                    else
                    {
                        sql.AppendLine(" AND ユーザーＩＤ = @ユーザーＩＤ");
                        cmd.Parameters.Add("@ユーザーＩＤ", SqlDbType.VarChar).Value = loginID;
                    }

                    if (string.IsNullOrEmpty(password))
                    {
                        sql.AppendLine(" AND ユーザーＰＷ IS NULL");
                    }
                    else
                    {
                        sql.AppendLine(" AND ユーザーＰＷ = @ユーザーＰＷ");
                        cmd.Parameters.Add("@ユーザーＰＷ", SqlDbType.VarChar).Value = password;
                    }

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TableBase._企業ＣＤ = ConnectionInformation.CompanyCD;

                            // ユーザーID、パスワードに該当する担当者マスタを取得し、それをユーザーデータにする
                            int tantousyaCD = (int)reader["担当者ＣＤ"];
                            Tantousya tantousya = Tantousya.GetTantousya(tantousyaCD);
                            UserData.setInstance(tantousya);
                            UserData userData = UserData.getInstance();
                            return true;
                        }
                        else
                        {
                            MessageBox.Show(this, "ユーザーID または パスワードが違います。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }
            }
            catch (EmuException)
            {
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
    }
}
