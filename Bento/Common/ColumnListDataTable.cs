using System;
using System.Collections.Generic;
using System.Data;

namespace Emu.Bento.Common
{
    /// <summary>
    /// 含める列を指定できるデータテーブル。列数が不定で、行を列として表現したいときでの使用を想定。
    /// </summary>
    /// <example>
    /// ①固定列を指定する（AddFixedColumn）
    /// ┌────┐
    /// │固定列X │
    /// ├────┤
    /// │固定列Y │
    /// ├────┤
    /// │固定列Z │
    /// └────┘
    ///
    /// ②不定列を指定する（AddColumns）
    /// ┌───┬───┐
    /// │商品名│商品CD│
    /// ├───┼───┤
    /// │商品A │1     │
    /// ├───┼───┤
    /// │商品B │2     │
    /// ├───┼───┤
    /// │商品C │3     │
    /// ├───┼───┤
    /// │商品D │4     │
    /// └───┴───┘
    ///
    /// ③個数列を値列に指定して、以下のようなテーブルをインポートする（Import）
    /// ┌────┬────┬────┬───┬──┐
    /// │固定列X │固定列Y │固定列Z │商品CD│個数│
    /// ├────┼────┼────┼───┼──┤
    /// │fooX    │fooY    │fooZ    │1     │  30│
    /// ├────┼────┼────┼───┼──┤
    /// │fooX    │fooY    │fooZ    │2     │ 505│
    /// ├────┼────┼────┼───┼──┤
    /// │fooX    │fooY    │fooZ    │3     │  43│
    /// ├────┼────┼────┼───┼──┤
    /// │barX    │barY    │barZ    │1     │   9│
    /// ├────┼────┼────┼───┼──┤
    /// │barX    │barY    │barZ    │3     │  78│
    /// ├────┼────┼────┼───┼──┤
    /// │hogeX   │hogeY   │hogeZ   │2     │ 162│
    /// └────┴────┴────┴───┴──┘
    ///
    /// ④結果は以下の通り
    /// ┌────┬────┬────┬───┬───┬───┬───┐
    /// │固定列X │固定列Y │固定列Z │商品A │商品B │商品C │商品D │
    /// ├────┼────┼────┼───┼───┼───┼───┤
    /// │fooX    │fooY    │fooZ    │   30 │  505 │   43 │      │
    /// ├────┼────┼────┼───┼───┼───┼───┤
    /// │barX    │barY    │barZ    │    9 │      │   78 │      │
    /// ├────┼────┼────┼───┼───┼───┼───┤
    /// │hogeX   │hogeY   │hogeZ   │      │  162 │      │      │
    /// └────┴────┴────┴───┴───┴───┴───┘
    ///  ※列の順序に拘らなければ、AddFixedColumnとAddColumnsの呼び出しはどちらが先でもよい
    /// </example>
    public class ColumnListDataTable : DataTable
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 列が持つコードをキーとして、列インデックスを値に持つマップ
        /// </summary>
        private Dictionary<object, int> _codeIndexMap = new Dictionary<object, int>();

        /// <summary>
        /// 固定列名のセット
        /// </summary>
        private List<string> _fixedColumnList = new List<string>();
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 不定列名のリスト
        /// </summary>
        public List<string> ColumnNames { get; private set; }

        /// <summary>
        /// インデックスで指定した行の、コードに該当する列の値を取得します。
        /// </summary>
        /// <param name="rowIndex">0から始まる行インデックス</param>
        /// <param name="code">列を特定するコード</param>
        /// <returns>指定した行の、コードに該当する列の値</returns>
        /// <exception cref="IndexOutOfRangeException">指定した行 または列がありません。</exception>
        /// <exception cref="KeyNotFoundException">コードに該当する列が見つかりません。</exception>
        public object this[int rowIndex, object code]
        {
            get
            {
                DataRow dr = this.Rows[rowIndex];
                int columnIndex = _codeIndexMap[code];
                object ret = dr[columnIndex];
                if (ret == DBNull.Value)
                {
                    ret = null;
                }
                return ret;
            }
        }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ColumnListDataTable() : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        public ColumnListDataTable(string tableName) : base(tableName)
        {
        }

        #endregion

        /// <summary>
        /// このテーブルに含める不定列を追加する。
        /// </summary>
        /// <param name="dtColumns">列として含めるデータを持つデータテーブル。このデータテーブルの行方向の値を列として保持する</param>
        /// <param name="nameColumnName">列の名前が格納されているdtColumnsの列名</param>
        /// <param name="codeColumnName">列を特定するためのコードが格納されているdtColumnsの列名</param>
        /// <param name="valueColumnType">追加する列のタイプ</param>
        public void AddColumns(DataTable dtColumns, string nameColumnName, string codeColumnName, Type valueColumnType)
        {
            if (ColumnNames == null)
            {
                ColumnNames = new List<string>();
            }

            foreach (DataRow dr in dtColumns.Rows)
            {
                // コードの列インデックスを保存
                object code = dr[codeColumnName];
                _codeIndexMap[code] = this.Columns.Count;

                string name = (string)dr[nameColumnName];
                // 既に同名の列が存在する場合はコードを付加する
                if (this.Columns.Contains(name))
                {
                    name = string.Format("{0}({1})", name, code);
                }
                this.Columns.Add(name, valueColumnType);     // 列を追加
                ColumnNames.Add(name);
            }
        }

        /// <summary>
        /// このテーブルに含める固定列を追加する。
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="columnType">列のタイプ</param>
        public void AddFixedColumn(string columnName, Type columnType)
        {
            this.Columns.Add(columnName, columnType);     // 列を追加
            _fixedColumnList.Add(columnName);
        }

        /// <summary>
        /// このテーブルに含める固定列を追加する。
        /// </summary>
        /// <param name="dataColumn">追加する列を表すDataColumn（このコピーが追加される）</param>
        public void AddFixedColumn(DataColumn dataColumn)
        {
            this.Columns.Add(dataColumn.ColumnName, dataColumn.DataType);
            _fixedColumnList.Add(dataColumn.ColumnName);
        }

        /// <summary>
        /// インポート。
        /// </summary>
        /// <param name="dataTable">インポート対象のデータテーブル</param>
        /// <param name="codeColumnName">インポート対象のデータテーブルのうち、不定列を特定するためのコードの列の名前</param>
        /// <param name="valueColumnName">キーと見なした固定列によって特定された行と、不定列とが交わるセルに設定する値が格納された列の名前</param>
        public void Import(DataTable dataTable, string codeColumnName, string valueColumnName)
        {
            // dataTableの複数行が、このテーブルの1行になる。
            // つまり、dataTableの1行は、このテーブルの新規行になることもあれば、既存の行に含まれることもある。
            // したがって、固定列をキーとみなして、このテーブルの行を特定する

            DataView dv = new DataView(this);           // this.Select()を早くするためにインデックスを生成（この時点ではレコードが無いのでコストはほぼゼロ）

            foreach (DataRow dr in dataTable.Rows)
            {
                // ループの中で、srcList←targetList←空リスト にしているので、ここではtargetListに元データを格納している
                List<DataRow> targetList = new List<DataRow>(this.Select());    // 条件に該当するデータを格納するリスト
                List<DataRow> srcList = new List<DataRow>();                    // 検索対象のデータリスト

                foreach (string fixedColumnName in _fixedColumnList)
                {
                    // レコードが無いのに探しても意味がない
                    if (targetList == null || targetList.Count == 0)
                    {
                        break;
                    }

                    // 列名に該当する列インデックスを取得（これをマップで管理しても、あまり速くならなかった）
                    int dataTableColumnIndex = dataTable.Columns.IndexOf(fixedColumnName);
                    int thisColumnIndex = this.Columns.IndexOf(fixedColumnName);

                    srcList = targetList;               // 条件に該当したデータを新たな検索対象のデータとして…
                    targetList = new List<DataRow>();   // 条件に該当するデータリストを初期化

                    // Null?
                    if (dr.IsNull(dataTableColumnIndex))
                    {
                        foreach (DataRow row in srcList)
                        {
                            if (row.IsNull(thisColumnIndex))
                            {
                                targetList.Add(row);
                            }
                        }
                    }
                    else
                    {
                        // int?
                        if (this.Columns[thisColumnIndex].DataType == typeof(int))
                        {
                            foreach (DataRow row in srcList)
                            {
                                if ((row[thisColumnIndex] as int?) == (dr[dataTableColumnIndex] as int?))
                                {
                                    targetList.Add(row);
                                }
                            }
                        }
                        // 日時？
                        else if (this.Columns[thisColumnIndex].DataType == typeof(DateTime))
                        {
                            foreach (DataRow row in srcList)
                            {
                                if ((row[thisColumnIndex] as DateTime?) == (dr[dataTableColumnIndex] as DateTime?))
                                {
                                    targetList.Add(row);
                                }
                            }
                        }
                        // decimal?
                        else if (this.Columns[thisColumnIndex].DataType == typeof(decimal))
                        {
                            foreach (DataRow row in srcList)
                            {
                                if ((row[thisColumnIndex] as decimal?) == (dr[dataTableColumnIndex] as decimal?))
                                {
                                    targetList.Add(row);
                                }
                            }
                        }
                        // bool?
                        else if (this.Columns[thisColumnIndex].DataType == typeof(bool))
                        {
                            foreach (DataRow row in srcList)
                            {
                                if ((row[thisColumnIndex] as bool?) == (dr[dataTableColumnIndex] as bool?))
                                {
                                    targetList.Add(row);
                                }
                            }
                        }
                        // 上記以外
                        else
                        {
                            foreach (DataRow row in srcList)
                            {
                                if (row[thisColumnIndex].ToString() == dr[dataTableColumnIndex].ToString())
                                {
                                    targetList.Add(row);
                                }
                            }
                        }
                    }
                }

                // 既存の行を取得
                DataRow targetRow = null;
                if (targetList != null && targetList.Count > 0)
                {
                    targetRow = targetList[0];
                    targetRow.BeginEdit();
                }
                else
                {
                    // 無い場合は新規行
                    targetRow = this.NewRow();
                    targetRow.BeginEdit();
                    this.Rows.Add(targetRow);
                    foreach (string fixedColumnName in _fixedColumnList)
                    {
                        targetRow[fixedColumnName] = dr[fixedColumnName];
                    }
                }

                // コードと値をそれぞれ指定された列から取得
                object code = dr[codeColumnName];
                object value = dr[valueColumnName];

                // コードに該当する列を特定して値を格納
                if (_codeIndexMap.ContainsKey(code))
                {
                    int columnIndex = _codeIndexMap[code];
                    targetRow[columnIndex] = value;
                }
                else
                {
                    // 不定列に存在しないコードは無視する
                }
                targetRow.EndEdit();
            }

            // 変更を適用する
            this.AcceptChanges();
        }

        /// <summary>
        /// コードに該当する列のインデックスを取得します。
        /// </summary>
        /// <param name="code">列を特定するコード</param>
        /// <returns>コードに該当する列のインデックス。見つからない場合は-1</returns>
        public int CodeToColumnIndex(object code)
        {
            int columnIndex = -1;
            if (_codeIndexMap.ContainsKey(code))
            {
                columnIndex = _codeIndexMap[code];
            }
            return columnIndex;
        }
    }
}
