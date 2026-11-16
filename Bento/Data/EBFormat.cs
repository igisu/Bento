using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// EBレコードフォーマット
    /// </summary>
    /// <remarks>
    /// テーブルに対して追加・更新・削除はしないので、TableBase2は継承しない。
    /// エラーチェックも無いので、IDataErrorInfoも実装しない。
    /// </remarks>
    public class EBFormat
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// データ区分
        /// </summary>
        public enum DataKubun
        {
            Header = 1,     // ヘッダー
            Data = 2,       // データ
            Trailer = 8,    // トレーラー
            End = 9         // エンド
        }

        /// <summary>
        /// 値の取得元
        /// </summary>
        public enum ValueType
        {
            D,              // データベースから取得したカレントレコード
            V               // マップ
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private string _銀行ＣＤ;
        private int _データ区分;
        private int _項目順;
        private string _項目名;
        private int _文字数;
        private string _値;
        private bool _左寄せフラグ;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EBFormat()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public EBFormat(EBFormat src)
        {
            銀行ＣＤ = src.銀行ＣＤ;
            データ区分 = src.データ区分;
            項目順 = src.項目順;
            項目名 = src.項目名;
            文字数 = src.文字数;
            値 = src.値;
            左寄せフラグ = src.左寄せフラグ;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public EBFormat(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            銀行ＣＤ = dr["銀行ＣＤ", version] as string;
            データ区分 = (int)dr["データ区分", version];
            項目順 = (int)dr["項目順", version];
            項目名 = dr["項目名", version] as string;
            文字数 = (int)dr["文字数", version];
            値 = dr["値", version] as string;
            左寄せフラグ = (bool)dr["左寄せフラグ", version];
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string 銀行ＣＤ
        {
            get { return _銀行ＣＤ; }
            set { _銀行ＣＤ = value; }
        }

        public int データ区分
        {
            get { return _データ区分; }
            set { _データ区分 = value; }
        }

        public int 項目順
        {
            get { return _項目順; }
            set { _項目順 = value; }
        }

        public string 項目名
        {
            get { return _項目名; }
            set { _項目名 = value; }
        }

        public int 文字数
        {
            get { return _文字数; }
            set { _文字数 = value; }
        }

        public string 値
        {
            get { return _値; }
            set { _値 = value; }
        }

        public bool 左寄せフラグ
        {
            get { return _左寄せフラグ; }
            set { _左寄せフラグ = value; }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド

        /// <summary>
        /// 文字数、値、左寄せフラグでフォーマットした文字列を返します。
        /// </summary>
        /// <returns>EBレコードフォーマットが表す文字列</returns>
        public override string ToString()
        {
            string value = _値;
            if (value == null)
            {
                value = "";
            }

            // 値を文字数に収める
            if (value.Length > _文字数)
            {
                value = value.Substring(0, _文字数);
            }

            if (_左寄せフラグ == true)
            {
                value = value.PadRight(_文字数);       // 右に空白を埋める
            }
            else
            {
                value = value.PadLeft(_文字数, '0');   // 左に'0'を埋める
            }

            return value;
        }
        #endregion
    }
}
