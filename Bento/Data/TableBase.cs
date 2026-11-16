using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// テーブル基本クラス
    /// </summary>
    public abstract class TableBase
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public static int? _企業ＣＤ;

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public int? 企業ＣＤ
        {
            get
            {
                return _企業ＣＤ;
            }
            set
            {
                // 一度取得した企業CDをNullで上書きしないようにする
                // （Spreadの新規行がこれに該当する）
                if (value != null)
                {
                    _企業ＣＤ = value;
                }
            }
        }

        /// <summary>
        /// 操作履歴出力用キー文字列
        /// </summary>
        public abstract string Code
        {
            get;
        }

        /// <summary>
        /// DB更新時の(エラー)メッセージ
        /// </summary>
        public string Message { get; protected set; }
        #endregion

        //**************************************
        //* 抽象メソッド
        //**************************************
        #region 抽象メソッド
        /// <summary>
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public abstract bool Insert(SqlConnection con, SqlTransaction tran);

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public abstract bool Update(SqlConnection con, SqlTransaction tran);

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public abstract bool Delete(SqlConnection con, SqlTransaction tran);

        /// <summary>
        /// DB更新主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="historyProcess">処理タイプ</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        protected abstract bool Save(SqlConnection con, SqlTransaction tran, string sql, History.ProcessType historyProcess);
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="src"></param>
        protected void Copy(TableBase src)
        {
            企業ＣＤ = src.企業ＣＤ;
        }

        /// <summary>
        /// データ行から生成
        /// </summary>
        /// <param name="dr"></param>
        protected void FromDataRow(DataRow dr)
        {
            FromDataRow(dr, DataRowVersion.Default);
        }

        /// <summary>
        /// データ行から生成
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="version"></param>
        protected void FromDataRow(DataRow dr, DataRowVersion version)
        {
            if (dr.Table.Columns.Contains("企業ＣＤ"))
            {
                企業ＣＤ = dr["企業ＣＤ", version] as int?;
            }
        }

        /// <summary>
        /// データ行へ出力
        /// </summary>
        /// <param name="dr"></param>
        protected void ToDataRow(DataRow dr)
        {
            dr.SetField<int?>("企業ＣＤ", _企業ＣＤ);
        }

        #endregion
    }
}
