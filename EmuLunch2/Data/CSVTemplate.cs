using System.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emu.Bento.Data
{
    /// <summary>
    /// CSVテンプレート
    /// </summary>
    /// <remarks>
    /// テーブルに対して追加・更新・削除はしないので、TableBase2は継承しない。
    /// エラーチェックも無いので、IDataErrorInfoも実装しない。
    /// </remarks>
    public class CSVTemplate
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
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
        private int _テンプレートＣＤ;
        private int _項目順;
        private string _項目名;
        private string _値;
        private string _フォーマット;

        private List<object> _出力用値リスト = new List<object>();

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CSVTemplate()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public CSVTemplate(CSVTemplate src)
        {
            テンプレートＣＤ = src.テンプレートＣＤ;
            項目順 = src.項目順;
            項目名 = src.項目名;
            値 = src.値;
            フォーマット = src.フォーマット;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public CSVTemplate(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            テンプレートＣＤ = (int)dr["テンプレートＣＤ", version];
            項目順 = (int)dr["項目順", version];
            項目名 = dr["項目名", version] as string;
            値 = dr["値", version] as string;
            フォーマット = dr["フォーマット", version] as string;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int テンプレートＣＤ
        {
            get { return _テンプレートＣＤ; }
            set { _テンプレートＣＤ = value; }
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

        public string 値
        {
            get { return _値; }
            set { _値 = value; }
        }

        public string フォーマット
        {
            get { return _フォーマット; }
            set { _フォーマット = value; }
        }

        public List<object> 出力用値リスト
        {
            get { return _出力用値リスト; }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 出力用値リストとフォーマットで編集したした文字列を返します。
        /// </summary>
        /// <returns>CSVテンプレートが表す文字列</returns>
        public override string ToString()
        {
            if (_出力用値リスト.Count == 0)
            {
                return "";
            }

            string ret;
            if (string.IsNullOrWhiteSpace(フォーマット))
            {
                // フォーマット文字列が未指定の場合は、出力用値リストの中身を文字列化したものを返す
                StringBuilder sb = new StringBuilder();
                string comma = "";
                foreach (object obj in _出力用値リスト)
                {
                    sb.AppendFormat("{0}{1}", comma, obj.ToString());   // 元々カンマで区切られていたはずなので、それを再現する
                    comma = ",";
                }
                ret = sb.ToString();
            }
            else
            {
                // フォーマット文字列がある場合は、出力用値リストを引数にしてFormatに渡す
                ret = string.Format(フォーマット.Trim(), _出力用値リスト.ToArray());
            }
            return ret;
        }
        #endregion
    }
}
