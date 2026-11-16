using System;
using System.Data.SqlClient;

namespace Emu.Bento.Common
{
    /// <summary>
    /// ログ出力済みを示す例外
    /// </summary>
    class EmuException : Exception
    {
        public EmuException() : base() { }

        public EmuException(Exception ex) : base("例外が発生しました。", ex) { }

        public EmuException(string s) : base(s) { }

        public override string ToString()
        {
            string s = base.ToString();
            if (InnerException is SqlException)
            {
                SqlException e = (SqlException)InnerException;
                switch (e.Number)
                {
                    case 53:        // SQL Server への接続を確立しているときにネットワーク関連またはインスタンス固有のエラーが発生しました。サーバーが見つからないかアクセスできません。インスタンス名が正しいこと、および SQL Server がリモート接続を許可するように構成されていることを確認してください。 (provider: Named Pipes Provider, error: 40 - SQL Server への接続を開けませんでした) ---> System.ComponentModel.Win32Exception (0x80004005): ネットワーク パスが見つかりません。
                    case 64:        // サーバーとの接続を正常に確立しましたが、ログイン前のハンドシェイク中にエラーが発生しました。 (provider: TCP Provider, error: 0 - 指定されたネットワーク名は利用できません。) ---> System.ComponentModel.Win32Exception (0x80004005): 指定されたネットワーク名は利用できません。
                        s = "データベースに接続できませんでした。";
                        break;
                }
            }
            return s;
        }
    }
}
