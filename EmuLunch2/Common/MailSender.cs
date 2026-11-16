using System;
using System.Net.Mail;
using log4net;
using System.Reflection;
using System.Net;

namespace Emu.Bento.Common
{
    /// <summary>
    /// メール送信クラス
    /// </summary>
    public class MailSender
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        ///  メール送信に使用するSMTPサーバー
        /// </summary>
        private string _host;

        /// <summary>
        ///  メール送信に使用するSMTPサーバーのポート番号
        /// </summary>
        private int _port;

        /// <summary>
        /// SMTP認証用のユーザー名
        /// </summary>
        private string _user;

        /// <summary>
        /// SMTP認証用のパスワード
        /// </summary>
        private string _pass;

        /// <summary>
        /// 送信メールメッセージ
        /// </summary>
        private MailMessage _mailMessage;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MailSender()
        {
            _mailMessage = new MailMessage();
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        ///  メール送信に使用するSMTPサーバー
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        ///  メール送信に使用するSMTPサーバーのポート番号
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 送信元アドレスをセットします
        /// </summary>
        /// <param name="mailAddress">送信元アドレス</param>
        public void SetFrom(string mailAddress)
        {
            try
            {
                _mailMessage.From = new MailAddress(mailAddress);
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
        /// 送信元アドレスをセットします
        /// </summary>
        /// <param name="mailAddress">送信元アドレス</param>
        /// <param name="name">宛名</param>
        public void SetFrom(string mailAddress, string name)
        {
            try
            {
                _mailMessage.From = new MailAddress(mailAddress, name);
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
        /// 送信先アドレスを追加します
        /// </summary>
        /// <param name="mailAddress">送信先アドレス</param>
        public void AddTo(string mailAddress)
        {
            try
            {
                _mailMessage.To.Add(new MailAddress(mailAddress));
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
        /// 送信先アドレスを追加します
        /// </summary>
        /// <param name="mailAddress">送信先アドレス</param>
        /// <param name="name">宛名</param>
        public void AddTo(string mailAddress, string name)
        {
            try
            {
                _mailMessage.To.Add(new MailAddress(mailAddress, name));
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
        /// CCのアドレスを追加します
        /// </summary>
        /// <param name="mailAddress">CCのアドレス</param>
        public void AddCc(string mailAddress)
        {
            try
            {
                _mailMessage.CC.Add(new MailAddress(mailAddress));
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
        /// CCのアドレスを追加します
        /// </summary>
        /// <param name="mailAddress">CCのアドレス</param>
        /// <param name="name">宛名</param>
        public void AddCc(string mailAddress, string name)
        {
            try
            {
                _mailMessage.CC.Add(new MailAddress(mailAddress, name));
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
        /// BCCアドレスを追加します
        /// </summary>
        /// <param name="mailAddress">BCCーアドレス</param>
        public void AddBcc(string mailAddress)
        {
            try
            {
                _mailMessage.Bcc.Add(new MailAddress(mailAddress));
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
        /// BCCアドレスを追加します
        /// </summary>
        /// <param name="mailAddress">BCCーアドレス</param>
        /// <param name="name">宛名</param>
        public void AddBcc(string mailAddress, string name)
        {
            try
            {
                _mailMessage.Bcc.Add(new MailAddress(mailAddress, name));
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
        /// SMTP認証用のユーザー名とパスワードを設定します
        /// </summary>
        /// <param name="user">ユーザー名</param>
        /// <param name="pass">パスワード</param>
        public void SetAuth(string user, string pass)
        {
            try
            {
                _user = user;
                _pass = pass;
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
        /// 件名と本文を指定してメールを送信します
        /// </summary>
        /// <param name="subject">件名</param>
        /// <param name="body">本文</param>
        public void Send(string subject, string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(_host, _port))
                {
                    // メールの件名
                    _mailMessage.Subject = subject;

                    // メールの本文
                    _mailMessage.Body = body;

                    // SMTP認証用のユーザー名とパスワードを設定する
                    if (string.IsNullOrEmpty(_user) == false && string.IsNullOrEmpty(_pass) == false)
                    {
                        client.Credentials = new NetworkCredential(_user, _pass);
                    }

                    // 作成したメールを送信する
                    client.Send(_mailMessage);
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
