using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;

namespace Emu.Bento
{
    public partial class BaseSpreadForm : BaseForm
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BaseSpreadForm()
        {
            InitializeComponent();
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 簡易的に（スラッシュが省略されているなど）入力された日付文字列を正しい日付として解釈し、テキストに設定し直します
        /// </summary>
        /// <param name="spread">対象スプレッド</param>
        /// <param name="sheet">対象シート</param>
        protected void ParseDateText(FpSpread spread, SheetView sheet)
        {
            //***********************************************************************************************************
            // 日付を入力できるのはGeneralCellTypeのみ(※)なので、それ以外のCellTypeは除外する
            // (※)DateTimeCellTypeも日付が入力可能だが、不正な日付の入力ができない仕組みになっているのでここでは除外する
            //***********************************************************************************************************

            // GeneralCellTypeから派生したセル型の基本コントロール？
            if (spread.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
            }
            else
            {
                return;
            }

            if (sheet.ActiveCell == null)
            {
                return;
            }

            // セルまたは列がGeneralCellType？
            GeneralCellType cellType = sheet.ActiveCell.CellType as GeneralCellType;
            if (cellType == null)
            {
                cellType = sheet.ActiveColumn.CellType as GeneralCellType;
            }
            if (cellType == null)
            {
                return;
            }

            // 日付入力？
            if (cellType.IsDateFormat == false)
            {
                return;
            }

            try
            {
                string s = spread.EditingControl.Text;
                DateTime today = DateTime.Today;
                DateTime? valueDate = null;

                if (Regex.IsMatch(s, "^[0-9]{1,2}$"))   // 日付のみ
                {
                    valueDate = new DateTime(today.Year, today.Month, int.Parse(s));
                }
                if (Regex.IsMatch(s, "^[0-9]{4}$"))     // 月日（3文字は 123 が 12/3 か 1/23 か区別できない）
                {
                    valueDate = new DateTime(today.Year, int.Parse(s.Substring(0, 2)), int.Parse(s.Substring(2, 2)));
                }
                if (Regex.IsMatch(s, "^[0-9]{6}$"))     // 年月日(yymmdd)
                {
                    // ２桁年の扱いはDateTimeに任せる
                    valueDate = DateTime.Parse(s.Substring(0, 2) + "/" + s.Substring(2, 2) + "/" + s.Substring(4, 2));
                }
                if (Regex.IsMatch(s, "^[0-9]{8}$"))     // 年月日(yyyymmdd)
                {
                    valueDate = DateTime.Parse(s.Substring(0, 4) + "/" + s.Substring(4, 2) + "/" + s.Substring(6, 2));
                }

                // 上記フォーマットに該当しない場合は、そのままで日付に解釈できる文字列と見なす
                if (valueDate != null)
                {
                    spread.EditingControl.Text = ((DateTime)valueDate).ToString(cellType.FormatString);
                    sheet.ActiveCell.Text = spread.EditingControl.Text;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 非編集状態からの日本語入力動作が不正になるのを回避する。
        /// 日本語を入力するSpreadのPreviewKeyDownイベントで、このメソッドを呼び出してください。
        /// </summary>
        /// <param name="spread">対象スプレッド</param>
        /// <param name="columnIndex">対象列インデックス</param>
        /// <param name="e">イベントデータ</param>
        /// <remarks>
        /// https://developer-tools.zendesk.com/hc/ja/articles/360004041515-Windows-10%E3%81%ABMay-2020-Update%E3%82%92%E9%81%A9%E7%94%A8%E3%81%99%E3%82%8B%E3%81%A8%E9%9D%9E%E7%B7%A8%E9%9B%86%E7%8A%B6%E6%85%8B%E3%81%8B%E3%82%89%E3%81%AE%E6%97%A5%E6%9C%AC%E8%AA%9E%E5%85%A5%E5%8A%9B%E5%8B%95%E4%BD%9C%E3%81%8C%E4%B8%8D%E6%AD%A3%E3%81%AB%E3%81%AA%E3%82%8B
        /// </remarks>
        protected void AvoidEnterJapaneseFirstKeyConfirm(FpSpread spread, int columnIndex, PreviewKeyDownEventArgs e)
        {
            // 非編集状態？
            if (spread.EditMode == false)
            {
                // 対象の列？
                if (spread.ActiveSheet.ActiveColumnIndex == columnIndex)
                {
                    // 日本語を入力しようとしているのなら、明示的にセル編集を開始する
                    // https://developer-tools.zendesk.com/hc/ja/articles/360003897816-Windows-10%E3%81%ABFall-Creators-Update%E3%82%92%E9%81%A9%E7%94%A8%E3%81%99%E3%82%8B%E3%81%A8%E6%96%87%E5%AD%97%E5%85%A5%E5%8A%9B%E5%8B%95%E4%BD%9C%E3%81%8C%E4%B8%8D%E6%AD%A3%E3%81%AB%E3%81%AA%E3%82%8B
                    if (e.KeyCode == Keys.ProcessKey)
                    {
                        spread.StartCellEditing(e, false);
                    }
                }
            }
        }

        /// <summary>
        /// 指定したシートの指定行に対する、データソースの行インデックスを返します。
        /// </summary>
        /// <param name="sheet">データソースを持つシート</param>
        /// <param name="rowIndex">行インデックス</param>
        /// <returns>データソースの行インデックス</returns>
        protected int GetDataRowFromViewRow(SheetView sheet, int rowIndex)
        {
            // シートの行インデックスからデータモデル内の行を取得
            int modelRowIndex = sheet.GetModelRowFromViewRow(rowIndex);

            // データモデルを取得し、データソース行インデックスを取得
            FarPoint.Win.Spread.Model.DefaultSheetDataModel dataModel = (FarPoint.Win.Spread.Model.DefaultSheetDataModel)sheet.Models.Data;
            int dataRowIndex = dataModel.GetDataRowFromModelRow(modelRowIndex);

            return dataRowIndex;
        }

        /// <summary>
        /// Static または Readonly の列のセルをロックします。
        /// </summary>
        /// <param name="sheet">対象シート</param>
        /// <remarks>
        /// ClearUnlockedCells()では、ロックされたセルはクリアの対象外とします。
        /// そのため、選択範囲をまとめてクリアするときは、クリア対象外の列をStaticまたはReadonlyにしておいて
        /// このメソッドを予め呼び出しておいてください。
        /// </remarks>
        protected void LockCellsStaticOrReadonly(SheetView sheet)
        {
            for (int rowIndex = 0; rowIndex < sheet.Rows.Count; rowIndex++)
            {
                for (int columnIndex = 0 ; columnIndex < sheet.Columns.Count ; columnIndex++)
                {
                    EditBaseCellType cellType = sheet.Columns[columnIndex].CellType as EditBaseCellType;
                    if (cellType != null)
                    {
                        if (cellType.Static || cellType.ReadOnly)
                        {
                            sheet.Cells[rowIndex, columnIndex].Locked = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 選択範囲のロックされていないセルをまとめてクリアする
        /// </summary>
        /// <param name="sheet">対象シート</param>
        protected void ClearUnlockedCells(SheetView sheet)
        {
            // 選択範囲をまとめてクリアする
            // GetSelections()を使用して選択情報を取得する
            // ※ISheetSelectionModelインタフェースを利用して選択情報を取得すると、列の選択と先頭セルのみの選択とが区別できないが、
            //   いったんこれを利用して取得しておくと、単一セルでもGetSelections()で選択範囲が取得できる。
            //   （ISheetSelectionModelを使用せずにGetSelections()を使うと、単一セルの場合に要素ゼロのセル配列が返ってくる）
            int anchorRow = sheet.Models.Selection.AnchorRow;
            FarPoint.Win.Spread.Model.CellRange[] cells = sheet.GetSelections();
            if (cells != null && cells.Length > 0)
            {
                FarPoint.Win.Spread.Model.CellRange cell = cells[0];
                int row = cell.Row;
                int rowCount = cell.RowCount;
                int column = cell.Column;
                int columnCount = cell.ColumnCount;

                // ClearDataはマイナス値も受け入れるが、セルの表示が更新されない（＝消えずに残ったように見える）ことがある（別のセルをクリックすると再表示される）
                // 明示的に指定するとちゃんと消える
                if (row < 0) row = 0;
                if (rowCount < 0) rowCount = sheet.Rows.Count;
                if (columnCount < 0) columnCount = sheet.Columns.Count;
                if (column < 0) column = 0;

                // _dataModel.ClearDataは、セルのプロパティに関わらず、その値を消してしまう
                // Locked=true なセルについては、その値を消さないようにする
                //_dataModel.ClearData(row, column, rowCount, columnCount);
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (sheet.Cells[row + i, column + j].Locked == false)
                        {
                            sheet.Cells[row + i, column + j].Value = null;       // // dt_ColumnChangedが発生
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 下方向にコピー
        /// 選択範囲の先頭行のセルの内容をその下の行のセルにコピーします
        /// </summary>
        /// <param name="sheet">対象シート</param>
        protected void FillDown(SheetView sheet)
        {
            FillDown(sheet, null);
        }

        /// <summary>
        /// 下方向にコピー
        /// 選択範囲の先頭行のセルの内容をその下の行のセルにコピーします
        /// </summary>
        /// <param name="sheet">対象シート</param>
        /// <param name="callBack">引数が行インデックス(int)、戻り値がvoidのコールバック用のメソッド。</param>
        protected void FillDown(SheetView sheet, Action<int> callBack)
        {
            // GetSelections()を使用して選択情報を取得する
            // ※ISheetSelectionModelインタフェースを利用して選択情報を取得すると、列の選択と先頭セルのみの選択とが区別できないが、
            //   いったんこれを利用して取得しておくと、単一セルでもGetSelections()で選択範囲が取得できる。
            //   （ISheetSelectionModelを使用せずにGetSelections()を使うと、単一セルの場合に要素ゼロのセル配列が返ってくる）
            int anchorRow = sheet.Models.Selection.AnchorRow;
            FarPoint.Win.Spread.Model.CellRange[] cells = sheet.GetSelections();
            if (cells != null && cells.Length > 0)
            {
                FarPoint.Win.Spread.Model.CellRange cell = cells[0];
                int row = cell.Row;
                int rowCount = cell.RowCount;
                int column = cell.Column;
                int columnCount = cell.ColumnCount;

                // マイナス値を補正
                if (row < 0) row = 0;
                if (rowCount < 0) rowCount = sheet.Rows.Count;
                if (columnCount < 0) columnCount = sheet.Columns.Count;
                if (column < 0) column = 0;

                // 列からループを開始していることに注意
                for (int j = 0; j < columnCount; j++)
                {
                    object value = sheet.Cells[row, column + j].Value;      // 先頭行の値を取得

                    // 先頭行の次の行から
                    for (int i = 1; i < rowCount; i++)
                    {
                        if (sheet.Cells[row + i, column + j].Locked == false)
                        {
                            sheet.Cells[row + i, column + j].Value = value;     // dt_ColumnChangedが発生

                            // コールバック呼び出し
                            if (callBack != null)
                            {
                                callBack(row + i);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
