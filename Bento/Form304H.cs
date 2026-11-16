using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// シール発注
    /// </summary>
    public partial class Form304H : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用短冊設定マスタ
        /// </summary>
        private TanzakuSettei _tanzakuSettei;

        /// <summary>
        /// 短冊シール発注
        /// </summary>
        private TanzakuSealOrder _tanzakuSealOrder;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// 短冊発注メール設定リスト
        /// </summary>
        private List<TanzakuOrderMail> _tanzakuOrderMailList = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="tanzakuOrderMailList">短冊発注メール設定リスト</param>
        public Form304H(int busyoCD, List<TanzakuOrderMail> tanzakuOrderMailList)
        {
            try
            {
                InitializeComponent();

                // 部署に該当する短冊設定マスタを取得
                _tanzakuSettei = TanzakuSettei.GetTanzakuSettei(busyoCD);
                if (_tanzakuSettei == null)
                {
                    _IsNewRecord = true;
                    _tanzakuSettei = new TanzakuSettei();
                    _tanzakuSettei.部署ＣＤ = busyoCD;
                }
                else
                {
                    _IsNewRecord = false;
                }

                if (string.IsNullOrEmpty(_tanzakuSettei.シール送付先))
                {
                    // 部署マスタを取得し、シール送付先を生成
                    Busyo busyo = Busyo.GetBusyo(busyoCD);
                    if (busyo != null)
                    {
                        string sofu = "{0}" +               // 会社名称
                                      "\r\n〒{1}" +         // 郵便番号
                                      "\r\n{2}" +           // 住所
                                      "\r\n電話：{3}";      // 電話番号
                        sofu = string.Format(sofu, busyo.会社名称, busyo.郵便番号, busyo.住所, busyo.電話番号);
                        _tanzakuSettei.シール送付先 = sofu;
                    }
                }

                _tanzakuOrderMailList = tanzakuOrderMailList;   // 短冊発注メール設定リストを受け取る
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
        private void Form304H_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // タイトルバーに部署名を表示
                this.Text = string.Format("シール発注【{0}】", _tanzakuSettei.部署名);

                _tanzakuSealOrder = new TanzakuSealOrder();
                // シール送付先、メールアドレスの入力チェックは（短冊設定マスタではなく）短冊シール発注で行なう
                _tanzakuSealOrder.シール送付先 = _tanzakuSettei.シール送付先;
                _tanzakuSealOrder.メールアドレス = _tanzakuSettei.メールアドレス;
                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                tanzakuSealOrderBindingSource.DataSource = _tanzakuSealOrder;

                // 念のため、短冊発注メール設定のリストが空であれば、発注ボタンを非活性にする
                if (_tanzakuOrderMailList == null || _tanzakuOrderMailList.Count == 0)
                {
                    btnSave.Enabled = false;
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
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form304H_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
                {
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                    switch (e.KeyCode)
                    {
                        case Keys.F9:               // 登録or更新
                            btnSave.PerformClick();
                            break;

                        case Keys.F12:              // 終了
                            btnQuit.PerformClick();
                            break;
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
        /// 発注ボタンクリック
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

                if (Save())
                {
                    // メール送信
                    SendMail();

                    MessageBox.Show(this, "シール発注メールを送信しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 画面を閉じるが、その前に再読込して「変更があります」通知を出さないようにする
                    Reload();
                    this.Close();
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
        /// 終了ボタンクリック
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

                if (_tanzakuSealOrder.HasErrors)
                {
                    MessageBox.Show(this, _tanzakuSealOrder.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 入力チェック後の内容を短冊設定マスタに格納
                _tanzakuSettei.シール送付先 = _tanzakuSealOrder.シール送付先;
                _tanzakuSettei.メールアドレス = _tanzakuSealOrder.メールアドレス;

                if (_IsNewRecord)
                {
                    ret = _tanzakuSettei.Insert();
                }
                else
                {
                    ret = _tanzakuSettei.Update();
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _tanzakuSettei.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(this.Text, "短冊設定マスタ", _tanzakuSettei.Code, historyProcess);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// メール送信
        /// </summary>
        private void SendMail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                string body = "{0}\n" +
                              "\n" +
                              "いつもお世話になっております。\n" +
                              "シールの発注をお願いします。\n" +
                              "\n" +
                              "■発注数\n" +
                              "{1}　×　{2}\n" +
                              "{3}\n" +
                              "\n" +
                              "■納入先\n" +
                              "{4}\n" +
                              "{5}\n" +
                              "\n" +
                              "納入先に注文確認を行い、発送をお願い致します。\n" +
                              "\n" +
                              "納入先へのメール連絡には下記アドレスをCCに設定してください。\n" +
                              "{6}\n" +
                              "\n" +
                              "以上、よろしくお願い致します。\n" +
                              "\n" +
                              "株式会社エム企画\n" +
                              "http://emu-jp.net/\n" +
                              "\n" +
                              "〒810-0062\n" +
                              "福岡市中央区荒戸1-1-6　福岡大濠ビル3F\n" +
                              "tel. 092-735-7898  fax. 092-735-7899\n" +
                              "e-mail.  {6}";

                string host = SystemMaster.Get("SMTPサーバー");
                int port = int.Parse(SystemMaster.Get("SMTPポート"));
                string orderer = SystemMaster.Get("短冊シール発注用送信者名");
                string orderMailAccount = SystemMaster.Get("短冊シール発注用メールアドレス");
                string pass = SystemMaster.Get("短冊シール発注用認証パスワード");
                string note = SystemMaster.Get("短冊シール発注用補足");

                MailSender mailSender = new MailSender();
                mailSender.Host = host;
                mailSender.Port = port;
                mailSender.SetFrom(orderMailAccount, orderer);

                StringBuilder atena = new StringBuilder();
                string delimiter = "";

                foreach (TanzakuOrderMail tanzakuOrderMail in _tanzakuOrderMailList)
                {
                    if (tanzakuOrderMail.CCフラグ == true)
                    {
                        // CC
                        mailSender.AddCc(tanzakuOrderMail.メールアドレス, tanzakuOrderMail.宛名);
                    }
                    else
                    {
                        // To
                        mailSender.AddTo(tanzakuOrderMail.メールアドレス, tanzakuOrderMail.宛名);
                        atena.AppendFormat("{0}{1}様", delimiter, tanzakuOrderMail.宛名);
                        delimiter = " ";
                    }
                }

                mailSender.SetAuth(orderMailAccount, pass);

                body = string.Format(body, atena.ToString(),                        // ○○様 ××様
                                           _tanzakuSealOrder.注文単位枚数編集,      // 15,000枚
                                           _tanzakuSealOrder.注文数,                // 1
                                           note,                                    // プリンタ型番：EV426用シール
                                           _tanzakuSealOrder.シール送付先,          // 株式会社△△～
                                           _tanzakuSealOrder.メールアドレス,        // aaa.example.com
                                           orderMailAccount);                       // seal.order@emu-jp.net

                mailSender.Send("シール発注依頼", body);

                log.Info("短冊シール発注メールを送信しました。");

                History.Save(string.Format("{0}で短冊シールを発注しました。({1}×{2})", _tanzakuSettei.部署名, _tanzakuSealOrder.注文単位枚数編集, _tanzakuSealOrder.注文数));
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                // 短冊設定マスタを取得
                _tanzakuSettei = TanzakuSettei.GetTanzakuSettei(_tanzakuSettei.部署ＣＤ);

                // シール送付先、メールアドレスの入力チェックは（短冊設定マスタではなく）短冊シール発注で行なう
                _tanzakuSealOrder.シール送付先 = _tanzakuSettei.シール送付先;
                _tanzakuSealOrder.メールアドレス = _tanzakuSettei.メールアドレス;
                tanzakuSealOrderBindingSource.ResetBindings(false);

                _IsNewRecord = false;
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
