using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Emu.Bento.Common;

namespace Emu.Bento
{
    static class Program
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数

        /// <summary>
        /// 再起動フラグ
        /// </summary>
        private static bool _Restarting = false;

        #endregion

        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// アプリケーションを再起動する
        /// </summary>
        static void RestartApplication()
        {
            //再起動フラグをTrueにして、アプリケーションを終了させる
            _Restarting = true;
            Application.Exit();
        }
        #endregion

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Mutex名を決める（必ずアプリケーション固有の文字列に変更すること！）
            string mutexName = "EmuLunch@EMU_Planning";

            // Mutexオブジェクトを作成する
            Mutex mutex = new Mutex(false, mutexName);

            bool hasHandle = false;

            try
            {
                //*********************************
                // 再起動に対応した二重起動チェック
                //*********************************
                try
                {
                    // ミューテックスの所有権を要求する
                    hasHandle = mutex.WaitOne(0, false);
                }
                catch (AbandonedMutexException)
                {
                    // 別のアプリケーションがミューテックスを解放しないで終了した時
                    hasHandle = true;
                }

                // ミューテックスを得られたか調べる
                if (hasHandle == false)
                {
                    // 得られなかった場合は、すでに起動していると判断して終了
                    MessageBox.Show("多重起動はできません。", Consts.APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //*******************************************
                // はじめからMainメソッドにあったコードを実行
                //*******************************************
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                DialogResult r;

                // プログラム更新処理
                UpdateManager updateManager = UpdateManager.GetInstance();
                // バックアップファイルを削除する
                if (updateManager.DeleteOldFiles())
                {
                    // ダウンロード済みのアップデートファイルがある？
                    if (updateManager.HasUpdate())
                    {
                        // 更新中ウィンドウを表示
                        FormUpdate form = new FormUpdate();
                        r = form.ShowDialog();
                        form.Dispose();
                        switch (r)
                        {
                            case DialogResult.OK:
                                // 自分自身を再起動する
                                RestartApplication();
                                return;

                            case DialogResult.Cancel:
                                break;

                            default:
                                return;
                        }
                    }
                }

                // 保存した設定の読み込みを試みて、user.configが破損していないかチェックする
                try
                {
                    // 前回ログインIDを取得
                    string loginID = Properties.Settings.Default.LoginID;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("設定ファイルが破損しています。再作成します。\n（前回ログインIDがクリアされ、ウィンドウの位置とサイズが初期化されます）", Consts.APP_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    string filePath = ((ConfigurationErrorsException)ex.InnerException).Filename;
                    File.Delete(filePath);
                    // このまま続行すると破損したままのuser.configを読み込んだままなので、再起動する必要がある。
                    RestartApplication();           // 自分自身を再起動する。
                    return;                         // returnを忘れずに
                }

                // ログインダイアログを表示
                FormLogin f = new FormLogin();
                r = f.ShowDialog();
                f.Dispose();
                if (r == DialogResult.OK)
                {
                    History.Login();    // ログイン履歴保存

                    // ログインに成功していればメインフォームを表示
                    Application.Run(new MDIParent1());

                    History.Logout();   // ログアウト履歴保存

                    LoginCountControler.Logout();   // システム接続端末テーブルから自レコードを削除
                }
            }
            finally
            {
                if (hasHandle)
                {
                    //ミューテックスを解放する
                    mutex.ReleaseMutex();
                }
                mutex.Close();

                // 再起動フラグがtrueの場合は終了前に起動する
                if (_Restarting)
                {
                    Process.Start(Application.ExecutablePath);
                }
            }
        }
    }
}
