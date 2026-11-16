using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace Emu.Bento.Common
{
    /// <summary>
    /// Excel操作クラス
    /// </summary>
    class ExcelManager : IDisposable
    {
        //**************************************
        //* 列挙値
        //**************************************
        #region 列挙値

        /// <summary>
        /// リリース対象
        /// </summary>
        private enum EnumReleaseMode
        {
            Sheet,
            Sheets,
            Book,
            Books,
            App
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        // Excel操作用オブジェクト
        private Application _xlApp = null;
        private Workbooks _xlBooks = null;
        private Workbook _xlBook = null;
        private Sheets _xlSheets = null;
        private Worksheet _xlSheet = null;

        private bool _isDispose = false;
        #endregion


        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ExcelManager()
        {
            _xlApp = new Application();
            _xlApp.DisplayAlerts = false;
            //_xlApp.Visible = false;
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド

        public void Open(string path)
        {
            _xlBooks = _xlApp.Workbooks;
            _xlBook = _xlBooks.Open(path);

            // シートを選択する
            _xlSheets = _xlBook.Worksheets;
            _xlSheet = _xlSheets[1] as Worksheet; // 1シート目を操作対象に設定する
        }

        /// <summary>
        /// セル範囲に値を設定
        /// </summary>
        /// <param name="table">セル範囲に設定する二次元配列</param>
        /// <param name="targetRowPosition">セル範囲の開始行</param>
        /// <param name="targetColumnPosition">セル範囲の開始列</param>
        public void SetTable(object[,] table, int targetRowPosition, int targetColumnPosition)
        {
            int colCnt = 0;    //列数カウンタ
            int rowCnt = 0;    //行数カウンタ
            Range xlCellsFrom = null;   //セル始点（中継用）
            Range xlRangeFrom = null;   //セル始点
            Range xlCellsTo = null;     //セル終点（中継用）
            Range xlRangeTo = null;     //セル終点
            Range xlTarget = null;      //配列設定対象レンジ

            // 配列の要素数取得
            rowCnt = table.GetLength(0) - 1;
            colCnt = table.GetLength(1) - 1;

            // 始点セル取得
            xlCellsFrom = _xlSheet.Cells;
            try
            {
                // 始点セル範囲取得
                xlRangeFrom = xlCellsFrom[targetRowPosition, targetColumnPosition] as Range;
                try
                {
                    // 終点セル取得
                    xlCellsTo = _xlSheet.Cells;
                    try
                    {
                        // 終点セル範囲取得
                        xlRangeTo = xlCellsTo[targetRowPosition + rowCnt, targetColumnPosition + colCnt] as Range;
                        try
                        {
                            // 貼り付け対象レンジ
                            xlTarget = _xlSheet.Range[xlRangeFrom, xlRangeTo];
                            try
                            {
                                // 値設定
                                xlTarget.Value = table;
                            }
                            finally
                            {
                                if (xlCellsTo != null)
                                {
                                    Marshal.ReleaseComObject(xlTarget);
                                }
                            }
                        }
                        finally
                        {
                            if (xlCellsTo != null)
                            {
                                Marshal.ReleaseComObject(xlRangeTo);
                            }
                        }
                    }
                    finally
                    {
                        if (xlCellsTo != null)
                        {
                            Marshal.ReleaseComObject(xlCellsTo);
                        }
                    }
                }
                finally
                {
                    if (xlRangeFrom != null)
                    {
                        Marshal.ReleaseComObject(xlRangeFrom);
                    }
                }
            }
            finally
            {
                if (xlCellsFrom != null)
                {
                    Marshal.ReleaseComObject(xlCellsFrom);
                }
            }
        }

        /// <summary>
        /// シートに入力済みの全てのセルの内容を格納した二次元配列を返す
        /// </summary>
        /// <returns>シートの内容を格納した二次元配列</returns>
        public object[,] GetValue()
        {
            Range xlCellsFrom = null;   // セル始点（中継用）
            Range xlRangeFrom = null;   // セル始点
            Range xlCellsTo = null;     // セル終点（中継用）
            Range xlRangeTo = null;     // セル終点
            Range xlTarget = null;      // 配列設定対象レンジ

            object[,] table = null;     // 二次元配列

            // 始点セル取得
            xlCellsFrom = _xlSheet.Cells;
            try
            {
                // 始点セル範囲取得
                xlRangeFrom = xlCellsFrom[1, 1] as Range;
                try
                {
                    // 終点セル取得
                    xlCellsTo = _xlSheet.Cells;
                    try
                    {
                        // 終点セル範囲取得
                        xlRangeTo = xlCellsTo.SpecialCells(XlCellType.xlCellTypeLastCell) as Range;
                        try
                        {
                            // 取得対象レンジ
                            xlTarget = _xlSheet.Range[xlRangeFrom, xlRangeTo];
                            try
                            {
                                // 値取得
                                table = (object[,])xlTarget.Value;
                            }
                            finally
                            {
                                if (xlTarget != null)
                                {
                                    Marshal.ReleaseComObject(xlTarget);
                                }
                            }
                        }
                        finally
                        {
                            if (xlRangeTo != null)
                            {
                                Marshal.ReleaseComObject(xlRangeTo);
                            }
                        }
                    }
                    finally
                    {
                        if (xlCellsTo != null)
                        {
                            Marshal.ReleaseComObject(xlCellsTo);
                        }
                    }
                }
                finally
                {
                    if (xlRangeFrom != null)
                    {
                        Marshal.ReleaseComObject(xlRangeFrom);
                    }
                }
            }
            finally
            {
                if (xlCellsFrom != null)
                {
                    Marshal.ReleaseComObject(xlCellsFrom);
                }
            }

            return table;
        }

        /// <summary>
        /// アクティブなシートを印刷
        /// </summary>
        /// <param name="printerName">プリンタ名</param>
        public void Print(string printerName)
        {
            _xlSheet.PrintOutEx(ActivePrinter : printerName);
        }

        /// <summary>
        /// Excel終了
        /// </summary>
        public void Quit()
        {
            _xlBook.Saved = true;   // 保存したことにする
        }

        /// <summary>
        /// Excelリソース解放
        /// </summary>
        /// <param name="ReleaseMode">リリース対象Enum</param>
        private void ReleaseExcelComObject(EnumReleaseMode ReleaseMode)
        {
            try
            {
                // xlSheet解放
                if (_xlSheet != null)
                {
                    Marshal.ReleaseComObject(_xlSheet);
                    _xlSheet = null;
                }
                if (ReleaseMode == EnumReleaseMode.Sheet)
                    return;

                // xlSheets解放
                if (_xlSheets != null)
                {
                    Marshal.ReleaseComObject(_xlSheets);
                    _xlSheets = null;
                }
                if (ReleaseMode == EnumReleaseMode.Sheets)
                    return;

                // xlBook解放
                if (_xlBook != null)
                {
                    try
                    {
                        _xlBook.Close();
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(_xlBook);
                        _xlBook = null;
                    }
                }
                if (ReleaseMode == EnumReleaseMode.Book)
                    return;

                // xlBooks解放
                if (_xlBooks != null)
                {
                    Marshal.ReleaseComObject(_xlBooks);
                    _xlBooks = null;
                }
                if (ReleaseMode == EnumReleaseMode.Books)
                    return;

                // xlApp解放
                if (_xlApp != null)
                {
                    try
                    {
                        // アラートを戻して終了
                        _xlApp.DisplayAlerts = true;
                        _xlApp.Quit();
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(_xlApp);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 解放
        /// </summary>
        public void Dispose()
        {
            // ExcelManagerが使われなくなったときに破棄する資源
            ReleaseExcelComObject(EnumReleaseMode.App);

            // dispose判定フラグON
            _isDispose = true;

            // Dispose()が明示的に呼ばれたときは、GCからFinalize()呼び出しをさせない。
            GC.SuppressFinalize(this);
        }
         #endregion

        //**************************************
        //* デストラクタ
        //**************************************
        #region デストラクタ
        ~ExcelManager()
        {
            // 呼出し側がDispose()してなかったらここでDispose
            if (_isDispose == false)
            {
                Dispose();
            }
        }
        #endregion
    }
}
