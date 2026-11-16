using System.Collections.Specialized;

namespace Emu.Bento.Common
{
    /// <summary>
    /// インストールされているプリンタを保持する
    /// </summary>
    public static class Printers
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// プリンタ名コレクション
        /// </summary>
        private static StringCollection _installedPrinterNames = null;

        /// <summary>
        /// プリンタ名選択肢コレクション（先頭に空白を持つ）
        /// </summary>
        private static StringCollection _printerNameOptions = null;

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// プリンタ名コレクション
        /// </summary>
        public static StringCollection InstalledPrinterNames
        {
            get
            {
                if (_installedPrinterNames == null)
                {
                    GetPrinter();
                }
                return _installedPrinterNames;
            }
        }

        /// <summary>
        /// プリンタ名選択肢コレクション（先頭に空白を持つ）
        /// </summary>
        public static StringCollection PrinterNameOptions
        {
            get
            {
                if (_printerNameOptions == null)
                {
                    GetPrinter();
                }
                return _printerNameOptions;
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定したプリンタ(名)がインストールされているかを返します。
        /// </summary>
        /// <param name="printerName">プリンタ名</param>
        /// <returns>指定したプリンタ(名)がインストールされていればtrue</returns>
        public static bool HasPrinter(string printerName)
        {
            // 無効なプリンタ名は、当然存在しない
            if (string.IsNullOrEmpty(printerName))
            {
                return false;
            }

            GetPrinter();
            return _installedPrinterNames.Contains(printerName);
        }

        /// <summary>
        /// プリンタ名取得
        /// </summary>
        private static void GetPrinter()
        {
            if (_installedPrinterNames == null)
            {
                _installedPrinterNames = new StringCollection();
                _printerNameOptions = new StringCollection();
                _printerNameOptions.Add("");
                foreach (string s in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    _installedPrinterNames.Add(s);
                    _printerNameOptions.Add(s);
                }
            }
        }
        #endregion
    }
}
