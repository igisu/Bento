using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FarPoint.Win.Spread;

namespace Emu.Bento
{
    /// <summary>
    /// Spreadの行移動機能ヘルパー
    /// </summary>
    /// <remarks>
    /// Spreadに以下の機能を持たせる場合は、Formにこのクラスを継承してください。
    /// ・いちばん上へ／上へ／下へ／いちばん下へボタンによる行移動
    /// ・行ヘッダのドラッグによる行移動
    /// 【使い方】
    /// ・Spreadに２つの列を用意してください。非表示にしても構いません。（ユニークキー、変更フラグ）
    /// ・Form_Loadイベントでこのクラスのインスタンスを作ってください。
    /// ・BindingSource.DataSource には DataTableを指定してください。（このDataTableにPrimaryKeyを指定してはダメです）
    /// ・ボタンに応じたメソッドを呼び出します。
    ///   ┌──────┬────────┐
    ///   │ボタン      │呼び出すメソッド│
    ///   ├──────┼────────┤
    ///   │いちばん上へ│MoveTop()       │
    ///   ├──────┼────────┤
    ///   │上へ        │MoveUp()        │
    ///   ├──────┼────────┤
    ///   │下へ        │MoveDown()      │
    ///   ├──────┼────────┤
    ///   │いちばん下へ│MoveBottom()    │
    ///   └──────┴────────┘
    /// 【制限】
    /// これらの機能を実現するために、以下の制限があります。
    /// ・スプレッドの行ヘッダの高さの変更（リサイズ）を禁止しています。（ドラッグしやすいように）
    /// ・スプレッドに RowDragMoveイベントとRowDragMoveCompletedイベントを追加しています。
    /// ・BindingSource.DataSource に指定した DataTable に以下を設定しています。
    ///   ・TableNewRowイベントを追加しています。
    ///   ・以下の列を追加しています。
    ///     ┌─────┬──┬──────┐
    ///     │列名      │型  │列の内容    │
    ///     ├─────┼──┼──────┤
    ///     │_RowKey_  │Guid│ユニークキー│
    ///     ├─────┼──┼──────┤
    ///     │_Updated_ │bool│変更フラグ  │
    ///     └─────┴──┴──────┘
    ///   ・バインドしたデータソース(=DataTable)は順不同になります。
    ///     ┌────────┬───────────────────────┐
    ///     │禁止事項        │代替策                                        │
    ///     ├────────┼───────────────────────┤
    ///     │DataRowを挿入   │fpSpread1_Sheet1.Rows.Add()                   │
    ///     ├────────┼───────────────────────┤
    ///     │DataRowを削除   │fpSpread1_Sheet1.Rows.Remove()                │
    ///     ├────────┼───────────────────────┤
    ///     │直接foreachする │GetRowsInSpreadOrder()が返すコレクションを使う│
    ///     └────────┴───────────────────────┘
    /// </remarks>
    public class SpreadRowMoveHelper
    {

        //**********************************************************************************************************
        // ドラッグで行を移動しても、移動するのは見た目だけであり、バインドしているDataTableの行は移動しない。
        // ドロップ時にDataTableを再構築するのは処理が複雑になる。
        // →DataTableにGuidなどのユニークキーを持ち（Spreadではその列を非表示にしておく）、
        //   登録ボタン押下時などのタイミングでSpreadを先頭行から走査し、キー列でDataTableからDataRowを取得する。
        // →新規行を移動できないよう（AllowRowMoveDataAllowAddNew = false）にしていても、登録後に再読込(※)すると、
        //   その設定が無視されてしまい、新規行を超えてドラッグできてしまう。
        //   (※)bindingSource.DataSource = dt。１度目はうまくいくが、２度目以降はダメ。
        // →新規行を移動できるよう設定（AllowRowMoveDataAllowAddNew = true）しておき、
        //   RowDragMoveイベントで新規行のドラッグ禁止、
        //   RowDragMoveCompletedイベントで新規行より下へのドロップ禁止（新規行の手前にドロップしたことにする）
        //**********************************************************************************************************

        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const string GUID_COLUMN_NAME = "_RowKey_";
        private const string UPDATED_COLUMN_NAME = "_Updated_";
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// ユニークキー列インデックス
        /// </summary>
        private int GUID_COLUMN;

        /// <summary>
        /// 変更フラグ列インデックス
        /// </summary>
        private int UPDATED_COLUMN;

        private FpSpread _spread;
        private BindingSource _bindingSource;
        private SheetView _sheet;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="spread">スプレッド</param>
        /// <param name="bindingSource">スプレッドにバインドするBindingSource</param>
        /// <param name="uniqueKeyColumnIndex">ユニークキー列インデックス</param>
        /// <param name="updatedFlgColumnIndex">変更フラグ列インデックス</param>
        public SpreadRowMoveHelper(FpSpread spread, BindingSource bindingSource, int uniqueKeyColumnIndex, int updatedFlgColumnIndex)
        {
            _spread = spread;
            _bindingSource = bindingSource;
            _sheet = _spread.Sheets[0];

            GUID_COLUMN = uniqueKeyColumnIndex;
            UPDATED_COLUMN = updatedFlgColumnIndex;

            _spread.AllowRowMove = true;                    // 行のドラッグ移動を許可
            _spread.AllowRowMoveDataAllowAddNew = true;     // 新規行は移動可能とした上で、コードで移動不可とする
            _spread.ShowDragBandOnMoving = true;            // 行列のドラッグ移動時にドラッグバンドを表示する
            _sheet.Rows.Default.Resizable = false;          // 行のリサイズを禁止（ドラッグしやすくする）

            // イベントを登録
            _bindingSource.DataSourceChanged += _bindingSource_DataSourceChanged;   // DataSource プロパティ値が変更された
            _spread.RowDragMove += _spread_RowDragMove;                             // スプレッド 行ドラッグ
            _spread.RowDragMoveCompleted += _spread_RowDragMoveCompleted;           // スプレッド 行ドラッグ完了
        }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// DataSource プロパティ値が変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _bindingSource_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable dt = _bindingSource.DataSource as DataTable;
            if (dt == null)
            {
                return;
            }

            // ユニークキー(GUID)列と変更フラグ列を追加
            dt.Columns.Add(GUID_COLUMN_NAME, typeof(Guid));
            dt.Columns.Add(UPDATED_COLUMN_NAME, typeof(bool));
            foreach (DataRow dr in dt.Rows)
            {
                dr[GUID_COLUMN_NAME] = Guid.NewGuid();
                dr[UPDATED_COLUMN_NAME] = false;
            }
            dt.AcceptChanges();     // 変更をコミット

            dt.TableNewRow += dt_TableNewRow;       // テーブル行追加イベント

            _sheet.BindDataColumn(GUID_COLUMN,    GUID_COLUMN_NAME);
            _sheet.BindDataColumn(UPDATED_COLUMN, UPDATED_COLUMN_NAME);
        }

        /// <summary>
        /// テーブル行追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row[GUID_COLUMN_NAME] = Guid.NewGuid();       // ユニークキーを追加
        }

        /// <summary>
        /// スプレッド 行ドラッグ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _spread_RowDragMove(object sender, DragMoveEventArgs e)
        {
            // 新規行（AllowAddNew が true の場合、最後の行が新規行）
            if (_sheet.DataAllowAddNew == false)
            {
                return;
            }

            // 新規行のドラッグ禁止
            int newRowIndex = _sheet.RowCount - 1;
            if (e.Index + e.Count - 1 >= newRowIndex)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// スプレッド 行ドラッグ完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _spread_RowDragMoveCompleted(object sender, DragMoveCompletedEventArgs e)
        {
            // 新規行（AllowAddNew が true の場合、最後の行が新規行）
            if (_sheet.DataAllowAddNew == false)
            {
                return;
            }

            int targetRowIndex = e.ToIndex;

            // 新規行より下へのドロップ禁止
            int newRowIndex = _sheet.RowCount - 1;
            if (newRowIndex <= targetRowIndex)
            {
                _sheet.MoveRow(targetRowIndex, newRowIndex - 1, e.Count, false);  // 新規行の１つ上にドロップしたことにする
                _sheet.ActiveRowIndex = newRowIndex - 1;                          // カレントセルを移動
                _sheet.AddSelection(_sheet.ActiveRowIndex, _sheet.ActiveColumnIndex, 1, _sheet.ColumnCount);    // カレント行を選択
                targetRowIndex = newRowIndex - 1;
            }

            _sheet.Cells[targetRowIndex, UPDATED_COLUMN].Value = true;     // 変更したことにする
            // RowStateを強制的に変更する
            DataRow dr = ((DataRowView)_bindingSource.Current).Row;
            dr.EndEdit();
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// カレント行を先頭行に移動
        /// </summary>
        public void MoveTop()
        {
            // 先頭行でない場合
            int currentRowIndex = _sheet.ActiveRowIndex;
            if (currentRowIndex > 0)
            {
                _sheet.MoveRow(currentRowIndex, 0, false);        // 先頭行に移動
                _sheet.Cells[0, UPDATED_COLUMN].Value = true;     // 変更したことにする
                _sheet.ActiveRowIndex = 0;                        // カレントセルを移動

                // RowStateを強制的に変更する
                DataRow dr = ((DataRowView)_bindingSource.Current).Row;
                dr.EndEdit();
            }
        }

        /// <summary>
        /// カレント行を１つ上に移動
        /// </summary>
        public void MoveUp()
        {
            // 先頭行でない場合
            int currentRowIndex = _sheet.ActiveRowIndex;
            if (currentRowIndex > 0)
            {
                int targetRowIndex = currentRowIndex - 1;
                _sheet.MoveRow(currentRowIndex, targetRowIndex, false);       // １つ上の行に移動
                _sheet.Cells[targetRowIndex, UPDATED_COLUMN].Value = true;    // 変更したことにする
                _sheet.ActiveRowIndex = targetRowIndex;                       // カレントセルを移動

                // RowStateを強制的に変更する
                DataRow dr = ((DataRowView)_bindingSource.Current).Row;
                dr.EndEdit();
            }
        }

        /// <summary>
        /// カレント行を１つ下に移動
        /// </summary>
        public void MoveDown()
        {
            int lastRowIndex = _sheet.Rows.Count - 1;
            // 最終行は新規行？
            if (_sheet.DataAllowAddNew)
            {
                lastRowIndex--;
            }

            // 最終行でない場合
            int currentRowIndex = _sheet.ActiveRowIndex;
            if (currentRowIndex < lastRowIndex)
            {
                int targetRowIndex = currentRowIndex + 1;
                _sheet.MoveRow(currentRowIndex, targetRowIndex, false);       // １つ下の行に移動
                _sheet.Cells[targetRowIndex, UPDATED_COLUMN].Value = true;    // 変更したことにする
                _sheet.ActiveRowIndex = targetRowIndex;                       // カレントセルを移動

                // RowStateを強制的に変更する
                DataRow dr = ((DataRowView)_bindingSource.Current).Row;
                dr.EndEdit();
            }
        }

        /// <summary>
        /// カレント行を最終行に移動
        /// </summary>
        public void MoveBottom()
        {
            int lastRowIndex = _sheet.Rows.Count - 1;
            // 最終行は新規行？
            if (_sheet.DataAllowAddNew)
            {
                lastRowIndex--;
            }

            // 最終行でない場合
            int currentRowIndex = _sheet.ActiveRowIndex;
            if (currentRowIndex < lastRowIndex)
            {
                int targetRowIndex = _sheet.Rows.Count - 2;
                _sheet.MoveRow(currentRowIndex, targetRowIndex, false);       // 最終行に移動
                _sheet.Cells[targetRowIndex, UPDATED_COLUMN].Value = true;    // 変更したことにする
                _sheet.ActiveRowIndex = targetRowIndex;                       // カレントセルを移動

                // RowStateを強制的に変更する
                DataRow dr = ((DataRowView)_bindingSource.Current).Row;
                dr.EndEdit();
            }
        }

        /// <summary>
        /// Spreadの順でDataRowのリストを返します
        /// </summary>
        /// <returns>Spreadの順に沿ったDataRowのリスト</returns>
        public List<DataRow> GetRowsInSpreadOrder()
        {
            List<DataRow> drList = new List<DataRow>();

            DataTable dt = _bindingSource.DataSource as DataTable;
            if (dt == null)
            {
                return drList;
            }

            // まず削除済みのレコードを追加
            foreach (DataRow deleted in dt.Rows.Cast<DataRow>().Where(r => r.RowState == DataRowState.Deleted))
            {
                drList.Add(deleted);
            }

            // 削除されていないレコードを表示順で追加
            for (int rowIndex = 0; rowIndex < _sheet.Rows.Count; rowIndex++)
            {
                var key = _sheet.Cells[rowIndex, GUID_COLUMN].Value;
                if (key == null) continue;
                var row = dt.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted && r[GUID_COLUMN_NAME] == key).FirstOrDefault();
                if (row != null)
                {
                    drList.Add(row);
                }
            }

            return drList;
        }
        #endregion
    }
}
