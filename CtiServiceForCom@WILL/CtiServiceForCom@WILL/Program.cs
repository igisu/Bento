using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace com.emukikaku.CtiServiceComWILL
{
    static class Program
    {

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            //Mutex名を決める（必ずアプリケーション固有の文字列に変更すること！）
            string mutexName = "CtiServiceComWILL";
            //Mutex名の先頭に「Global\」を付けて、Global Mutexにする
            mutexName = "Global\\" + mutexName;
            //すべてのユーザーにフルコントロールを許可するMutexSecurityを作成する
            System.Security.AccessControl.MutexAccessRule rule =
                new System.Security.AccessControl.MutexAccessRule(
                    new System.Security.Principal.SecurityIdentifier(
                        System.Security.Principal.WellKnownSidType.WorldSid, null),
                        System.Security.AccessControl.MutexRights.FullControl,
                        System.Security.AccessControl.AccessControlType.Allow);
            System.Security.AccessControl.MutexSecurity mutexSecurity =
                new System.Security.AccessControl.MutexSecurity();
            mutexSecurity.AddAccessRule(rule);
            //Mutexオブジェクトを作成する
            bool createdNew;
            System.Threading.Mutex mutex =
                new System.Threading.Mutex(false, mutexName, out createdNew, mutexSecurity);

            //ミューテックスの所有権を要求する
            if (mutex.WaitOne(0, false) == false)
            {
                //すでに起動していると判断して終了(ソフトフォンからのEXE起動メッセージをメインのPGに伝達)
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                System.Net.Sockets.UdpClient udpClientSoftphone = null;
                try
                {
                    udpClientSoftphone = new System.Net.Sockets.UdpClient();
                    string strArgs = "";
                    for (int i = 0; i < args.Length; i++)
                    {
                        strArgs += "＜" + args[i];
                    }
                    byte[] sendBytes = enc.GetBytes(strArgs);

                    IniFile _iniFile = new IniFile();
                    udpClientSoftphone.Send(sendBytes, sendBytes.Length, "localhost", _iniFile.CTISOFTPHONEANALYZEPORT);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                finally
                {
                    try
                    {
                        if (udpClientSoftphone != null)
                        {
                            udpClientSoftphone.Close();
                        }
                    } catch (Exception ex2) { } finally { }
                }
                return;
            }
           
            Application.Run(new frmCtiMain());


        }
    
    }

}
