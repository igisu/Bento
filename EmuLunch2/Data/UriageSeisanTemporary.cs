using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上精算_入力データ一時保存
    /// </summary>
    public class UriageSeisanTemporary : TableBase2
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するレコードを返します
        /// </summary>
        /// <param name="seisanbi">精算日</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <returns></returns>
        public  static UriageSeisanTemporary GetUriageSeisanTemporary(DateTime seisanbi, int busyoCD, int courseCD)
        {
            UriageSeisanTemporary uriageSeisanTemporary = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.精算日" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,A.コースＣＤ" +
                             "\r\n     ,A.配送担当者ＣＤ" +
                             "\r\n     ,A.配送担当者名" +
                             "\r\n     ,A.金銭内訳_10000円" +
                             "\r\n     ,A.金銭内訳_5000円" +
                             "\r\n     ,A.金銭内訳_2000円" +
                             "\r\n     ,A.金銭内訳_1000円" +
                             "\r\n     ,A.金銭内訳_500円" +
                             "\r\n     ,A.金銭内訳_100円" +
                             "\r\n     ,A.金銭内訳_50円" +
                             "\r\n     ,A.金銭内訳_10円" +
                             "\r\n     ,A.金銭内訳_5円" +
                             "\r\n     ,A.金銭内訳_1円" +
                             "\r\n     ,A.金銭内訳_小切手" +
                             "\r\n     ,A.金銭内訳_領収書" +
                             "\r\n     ,A.金銭内訳_合計" +
                             "\r\n     ,A.金銭内訳_過剰金" +
                             "\r\n     ,A.消費チケ枚数_ＢＶ300" +
                             "\r\n     ,A.消費チケ枚数_ＢＶ200" +
                             "\r\n     ,A.消費チケ金額_ＢＶ300" +
                             "\r\n     ,A.消費チケ金額_ＢＶ200" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     売上精算_入力データ一時保存 AS A" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A2.企業ＣＤ = @企業ＣＤ AND A.配送担当者ＣＤ = A3.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                             "\r\n AND A.精算日     = @精算日" +
                             "\r\n AND A.部署ＣＤ   = @部署ＣＤ" +
                             "\r\n AND A.コースＣＤ = @コースＣＤ";

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = seisanbi;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("売上精算入力データ一時保存");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    uriageSeisanTemporary = new UriageSeisanTemporary(dt.Rows[0]);
                }
            }

            return uriageSeisanTemporary;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private DateTime? _精算日;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private int? _配送担当者ＣＤ;
        private string _配送担当者名;
        private int? _金銭内訳_10000円;
        private int? _金銭内訳_5000円;
        private int? _金銭内訳_2000円;
        private int? _金銭内訳_1000円;
        private int? _金銭内訳_500円;
        private int? _金銭内訳_100円;
        private int? _金銭内訳_50円;
        private int? _金銭内訳_10円;
        private int? _金銭内訳_5円;
        private int? _金銭内訳_1円;
        private decimal? _金銭内訳_小切手;
        private decimal? _金銭内訳_領収書;
        private decimal? _金銭内訳_合計;
        private decimal? _金銭内訳_過剰金;
        private int? _消費チケ枚数_ＢＶ300;
        private int? _消費チケ枚数_ＢＶ200;
        private decimal? _消費チケ金額_ＢＶ300;
        private decimal? _消費チケ金額_ＢＶ200;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageSeisanTemporary()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageSeisanTemporary(UriageSeisanTemporary src)
        {
            精算日 = src.精算日;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;
            配送担当者ＣＤ = src.配送担当者ＣＤ;
            配送担当者名 = src.配送担当者名;
            金銭内訳_10000円 = src.金銭内訳_10000円;
            金銭内訳_5000円 = src.金銭内訳_5000円;
            金銭内訳_2000円 = src.金銭内訳_2000円;
            金銭内訳_1000円 = src.金銭内訳_1000円;
            金銭内訳_500円 = src.金銭内訳_500円;
            金銭内訳_100円 = src.金銭内訳_100円;
            金銭内訳_50円 = src.金銭内訳_50円;
            金銭内訳_10円 = src.金銭内訳_10円;
            金銭内訳_5円 = src.金銭内訳_5円;
            金銭内訳_1円 = src.金銭内訳_1円;
            金銭内訳_小切手 = src.金銭内訳_小切手;
            金銭内訳_領収書 = src.金銭内訳_領収書;
            金銭内訳_合計 = src.金銭内訳_合計;
            金銭内訳_過剰金 = src.金銭内訳_過剰金;
            消費チケ枚数_ＢＶ300 = src.消費チケ枚数_ＢＶ300;
            消費チケ枚数_ＢＶ200 = src.消費チケ枚数_ＢＶ200;
            消費チケ金額_ＢＶ300 = src.消費チケ金額_ＢＶ300;
            消費チケ金額_ＢＶ200 = src.消費チケ金額_ＢＶ200;

            base.Copy(src);
        }

        /// <summary>
        /// 売上精算ヘッダからのコピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageSeisanTemporary(UriageSeisanHeader src)
        {
            精算日 = src.精算日;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;
            配送担当者ＣＤ = src.配送担当者ＣＤ;
            配送担当者名 = src.配送担当者名;
            金銭内訳_10000円 = src.金銭内訳_10000円;
            金銭内訳_5000円 = src.金銭内訳_5000円;
            金銭内訳_2000円 = src.金銭内訳_2000円;
            金銭内訳_1000円 = src.金銭内訳_1000円;
            金銭内訳_500円 = src.金銭内訳_500円;
            金銭内訳_100円 = src.金銭内訳_100円;
            金銭内訳_50円 = src.金銭内訳_50円;
            金銭内訳_10円 = src.金銭内訳_10円;
            金銭内訳_5円 = src.金銭内訳_5円;
            金銭内訳_1円 = src.金銭内訳_1円;
            金銭内訳_小切手 = src.金銭内訳_小切手;
            金銭内訳_領収書 = src.金銭内訳_領収書;
            金銭内訳_合計 = src.金銭内訳_合計;
            金銭内訳_過剰金 = src.金銭内訳_過剰金;
            消費チケ枚数_ＢＶ300 = src.消費チケ枚数_ＢＶ300;
            消費チケ枚数_ＢＶ200 = src.消費チケ枚数_ＢＶ200;
            消費チケ金額_ＢＶ300 = src.消費チケ金額_ＢＶ300;
            消費チケ金額_ＢＶ200 = src.消費チケ金額_ＢＶ200;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public UriageSeisanTemporary(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            精算日 = dr["精算日", version] as DateTime?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            コースＣＤ = dr["コースＣＤ", version] as int?;
            配送担当者ＣＤ = dr["配送担当者ＣＤ", version] as int?;
            配送担当者名 = dr["配送担当者名", version] as string;
            金銭内訳_10000円 = dr["金銭内訳_10000円", version] as int?;
            金銭内訳_5000円 = dr["金銭内訳_5000円", version] as int?;
            金銭内訳_2000円 = dr["金銭内訳_2000円", version] as int?;
            金銭内訳_1000円 = dr["金銭内訳_1000円", version] as int?;
            金銭内訳_500円 = dr["金銭内訳_500円", version] as int?;
            金銭内訳_100円 = dr["金銭内訳_100円", version] as int?;
            金銭内訳_50円 = dr["金銭内訳_50円", version] as int?;
            金銭内訳_10円 = dr["金銭内訳_10円", version] as int?;
            金銭内訳_5円 = dr["金銭内訳_5円", version] as int?;
            金銭内訳_1円 = dr["金銭内訳_1円", version] as int?;
            金銭内訳_小切手 = dr["金銭内訳_小切手", version] as decimal?;
            金銭内訳_領収書 = dr["金銭内訳_領収書", version] as decimal?;
            金銭内訳_合計 = dr["金銭内訳_合計", version] as decimal?;
            金銭内訳_過剰金 = dr["金銭内訳_過剰金", version] as decimal?;
            消費チケ枚数_ＢＶ300 = dr["消費チケ枚数_ＢＶ300", version] as int?;
            消費チケ枚数_ＢＶ200 = dr["消費チケ枚数_ＢＶ200", version] as int?;
            消費チケ金額_ＢＶ300 = dr["消費チケ金額_ＢＶ300", version] as decimal?;
            消費チケ金額_ＢＶ200 = dr["消費チケ金額_ＢＶ200", version] as decimal?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public DateTime? 精算日
        {
            get { return _精算日; }
            set { _精算日 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set { _コースＣＤ = value; }
        }

        public int? 配送担当者ＣＤ
        {
            get { return _配送担当者ＣＤ; }
            set { _配送担当者ＣＤ = value; }
        }

        public string 配送担当者名
        {
            get { return _配送担当者名; }
            set { _配送担当者名 = value; }
        }

        public int? 金銭内訳_10000円
        {
            get { return _金銭内訳_10000円; }
            set { _金銭内訳_10000円 = value; }
        }

        public int? 金銭内訳_5000円
        {
            get { return _金銭内訳_5000円; }
            set { _金銭内訳_5000円 = value; }
        }

        public int? 金銭内訳_2000円
        {
            get { return _金銭内訳_2000円; }
            set { _金銭内訳_2000円 = value; }
        }

        public int? 金銭内訳_1000円
        {
            get { return _金銭内訳_1000円; }
            set { _金銭内訳_1000円 = value; }
        }

        public int? 金銭内訳_500円
        {
            get { return _金銭内訳_500円; }
            set { _金銭内訳_500円 = value; }
        }

        public int? 金銭内訳_100円
        {
            get { return _金銭内訳_100円; }
            set { _金銭内訳_100円 = value; }
        }

        public int? 金銭内訳_50円
        {
            get { return _金銭内訳_50円; }
            set { _金銭内訳_50円 = value; }
        }

        public int? 金銭内訳_10円
        {
            get { return _金銭内訳_10円; }
            set { _金銭内訳_10円 = value; }
        }

        public int? 金銭内訳_5円
        {
            get { return _金銭内訳_5円; }
            set { _金銭内訳_5円 = value; }
        }

        public int? 金銭内訳_1円
        {
            get { return _金銭内訳_1円; }
            set { _金銭内訳_1円 = value; }
        }

        public decimal? 金銭内訳_小切手
        {
            get { return _金銭内訳_小切手; }
            set { _金銭内訳_小切手 = value; }
        }

        public decimal? 金銭内訳_領収書
        {
            get { return _金銭内訳_領収書; }
            set { _金銭内訳_領収書 = value; }
        }

        public decimal? 金銭内訳_合計
        {
            get { return _金銭内訳_合計; }
            set { _金銭内訳_合計 = value; }
        }

        public decimal? 金銭内訳_過剰金
        {
            get { return _金銭内訳_過剰金; }
            set { _金銭内訳_過剰金 = value; }
        }

        public int? 消費チケ枚数_ＢＶ300
        {
            get { return _消費チケ枚数_ＢＶ300; }
            set { _消費チケ枚数_ＢＶ300 = value; }
        }

        public int? 消費チケ枚数_ＢＶ200
        {
            get { return _消費チケ枚数_ＢＶ200; }
            set { _消費チケ枚数_ＢＶ200 = value; }
        }

        public decimal? 消費チケ金額_ＢＶ300
        {
            get { return _消費チケ金額_ＢＶ300; }
            set { _消費チケ金額_ＢＶ300 = value; }
        }

        public decimal? 消費チケ金額_ＢＶ200
        {
            get { return _消費チケ金額_ＢＶ200; }
            set { _消費チケ金額_ＢＶ200 = value; }
        }

        public override string Code
        {
            get { return string.Format("精算日=[{0}] 部署ＣＤ=[{1}] コースＣＤ=[{2}]", 精算日, 部署ＣＤ, コースＣＤ); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get{ return null;}
        }

        public string this[string columnName]
        {
            get
            {
                return (_errors.ContainsKey(columnName) ? _errors[columnName] : null);
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(UriageSeisanTemporary target)
        {
            if (精算日 == target.精算日 &&
                部署ＣＤ == target.部署ＣＤ &&
                コースＣＤ == target.コースＣＤ &&
                配送担当者ＣＤ == target.配送担当者ＣＤ &&
                配送担当者名 == target.配送担当者名 &&
                金銭内訳_10000円 == target.金銭内訳_10000円 &&
                金銭内訳_5000円 == target.金銭内訳_5000円 &&
                金銭内訳_2000円 == target.金銭内訳_2000円 &&
                金銭内訳_1000円 == target.金銭内訳_1000円 &&
                金銭内訳_500円 == target.金銭内訳_500円 &&
                金銭内訳_100円 == target.金銭内訳_100円 &&
                金銭内訳_50円 == target.金銭内訳_50円 &&
                金銭内訳_10円 == target.金銭内訳_10円 &&
                金銭内訳_5円 == target.金銭内訳_5円 &&
                金銭内訳_1円 == target.金銭内訳_1円 &&
                金銭内訳_小切手 == target.金銭内訳_小切手 &&
                金銭内訳_領収書 == target.金銭内訳_領収書 &&
                金銭内訳_合計 == target.金銭内訳_合計 &&
                金銭内訳_過剰金 == target.金銭内訳_過剰金 &&
                消費チケ枚数_ＢＶ300 == target.消費チケ枚数_ＢＶ300 &&
                消費チケ枚数_ＢＶ200 == target.消費チケ枚数_ＢＶ200 &&
                消費チケ金額_ＢＶ300 == target.消費チケ金額_ＢＶ300 &&
                消費チケ金額_ＢＶ200 == target.消費チケ金額_ＢＶ200)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Insert(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n INSERT INTO 売上精算_入力データ一時保存(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,精算日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,配送担当者ＣＤ" +
                         "\r\n     ,配送担当者名" +
                         "\r\n     ,金銭内訳_10000円" +
                         "\r\n     ,金銭内訳_5000円" +
                         "\r\n     ,金銭内訳_2000円" +
                         "\r\n     ,金銭内訳_1000円" +
                         "\r\n     ,金銭内訳_500円" +
                         "\r\n     ,金銭内訳_100円" +
                         "\r\n     ,金銭内訳_50円" +
                         "\r\n     ,金銭内訳_10円" +
                         "\r\n     ,金銭内訳_5円" +
                         "\r\n     ,金銭内訳_1円" +
                         "\r\n     ,金銭内訳_小切手" +
                         "\r\n     ,金銭内訳_領収書" +
                         "\r\n     ,金銭内訳_合計" +
                         "\r\n     ,金銭内訳_過剰金" +
                         "\r\n     ,消費チケ枚数_ＢＶ300" +
                         "\r\n     ,消費チケ枚数_ＢＶ200" +
                         "\r\n     ,消費チケ金額_ＢＶ300" +
                         "\r\n     ,消費チケ金額_ＢＶ200" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@精算日" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@配送担当者ＣＤ" +
                         "\r\n     ,@配送担当者名" +
                         "\r\n     ,@金銭内訳_10000円" +
                         "\r\n     ,@金銭内訳_5000円" +
                         "\r\n     ,@金銭内訳_2000円" +
                         "\r\n     ,@金銭内訳_1000円" +
                         "\r\n     ,@金銭内訳_500円" +
                         "\r\n     ,@金銭内訳_100円" +
                         "\r\n     ,@金銭内訳_50円" +
                         "\r\n     ,@金銭内訳_10円" +
                         "\r\n     ,@金銭内訳_5円" +
                         "\r\n     ,@金銭内訳_1円" +
                         "\r\n     ,@金銭内訳_小切手" +
                         "\r\n     ,@金銭内訳_領収書" +
                         "\r\n     ,@金銭内訳_合計" +
                         "\r\n     ,@金銭内訳_過剰金" +
                         "\r\n     ,@消費チケ枚数_ＢＶ300" +
                         "\r\n     ,@消費チケ枚数_ＢＶ200" +
                         "\r\n     ,@消費チケ金額_ＢＶ300" +
                         "\r\n     ,@消費チケ金額_ＢＶ200" +
                         "\r\n     ,@作成担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n     ,@修正担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n )";

            return Save(con, tran, sql, History.ProcessType.Create);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Update(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n UPDATE 売上精算_入力データ一時保存" +
                         "\r\n SET 配送担当者ＣＤ       = @配送担当者ＣＤ" +
                         "\r\n    ,配送担当者名         = @配送担当者名" +
                         "\r\n    ,金銭内訳_10000円     = @金銭内訳_10000円" +
                         "\r\n    ,金銭内訳_5000円      = @金銭内訳_5000円" +
                         "\r\n    ,金銭内訳_2000円      = @金銭内訳_2000円" +
                         "\r\n    ,金銭内訳_1000円      = @金銭内訳_1000円" +
                         "\r\n    ,金銭内訳_500円       = @金銭内訳_500円" +
                         "\r\n    ,金銭内訳_100円       = @金銭内訳_100円" +
                         "\r\n    ,金銭内訳_50円        = @金銭内訳_50円" +
                         "\r\n    ,金銭内訳_10円        = @金銭内訳_10円" +
                         "\r\n    ,金銭内訳_5円         = @金銭内訳_5円" +
                         "\r\n    ,金銭内訳_1円         = @金銭内訳_1円" +
                         "\r\n    ,金銭内訳_小切手      = @金銭内訳_小切手" +
                         "\r\n    ,金銭内訳_領収書      = @金銭内訳_領収書" +
                         "\r\n    ,金銭内訳_合計        = @金銭内訳_合計" +
                         "\r\n    ,金銭内訳_過剰金      = @金銭内訳_過剰金" +
                         "\r\n    ,消費チケ枚数_ＢＶ300 = @消費チケ枚数_ＢＶ300" +
                         "\r\n    ,消費チケ枚数_ＢＶ200 = @消費チケ枚数_ＢＶ200" +
                         "\r\n    ,消費チケ金額_ＢＶ300 = @消費チケ金額_ＢＶ300" +
                         "\r\n    ,消費チケ金額_ＢＶ200 = @消費チケ金額_ＢＶ200" +
                         "\r\n    ,修正担当者ＣＤ       = @修正担当者ＣＤ" +
                         "\r\n    ,修正日               = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 精算日     = @精算日" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ" +
                         "\r\n   AND 修正日     = @修正日";

            return Save(con, tran, sql, History.ProcessType.Update);
        }

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Delete(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n DELETE FROM 売上精算_入力データ一時保存" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 精算日     = @精算日" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ";

            return Save(con, tran, sql, History.ProcessType.Delete, true);  // 失敗しても構わない
        }

        /// <summary>
        /// DB更新主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="historyProcess">処理タイプ</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        protected override bool Save(SqlConnection con, SqlTransaction tran, string sql, History.ProcessType historyProcess)
        {
            return Save(con, tran, sql, historyProcess, false);
        }

        /// <summary>
        /// DB更新主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="historyProcess">処理タイプ</param>
        /// <param name="ignoreFailure">失敗を無視する場合はtrue</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        protected bool Save(SqlConnection con, SqlTransaction tran, string sql, History.ProcessType historyProcess, bool ignoreFailure)
        {
            try
            {
                try
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (精算日 != null)
                    {
                        cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = 精算日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コースＣＤ != null)
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (配送担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@配送担当者ＣＤ", SqlDbType.Int).Value = 配送担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (配送担当者名 != null)
                    {
                        cmd.Parameters.Add("@配送担当者名", SqlDbType.VarChar).Value = 配送担当者名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送担当者名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (金銭内訳_10000円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_10000円", SqlDbType.Int).Value = 金銭内訳_10000円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_10000円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_5000円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_5000円", SqlDbType.Int).Value = 金銭内訳_5000円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_5000円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_2000円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_2000円", SqlDbType.Int).Value = 金銭内訳_2000円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_2000円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_1000円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_1000円", SqlDbType.Int).Value = 金銭内訳_1000円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_1000円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_500円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_500円", SqlDbType.Int).Value = 金銭内訳_500円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_500円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_100円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_100円", SqlDbType.Int).Value = 金銭内訳_100円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_100円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_50円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_50円", SqlDbType.Int).Value = 金銭内訳_50円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_50円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_10円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_10円", SqlDbType.Int).Value = 金銭内訳_10円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_10円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_5円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_5円", SqlDbType.Int).Value = 金銭内訳_5円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_5円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_1円 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_1円", SqlDbType.Int).Value = 金銭内訳_1円;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_1円", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (金銭内訳_小切手 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_小切手", SqlDbType.Decimal).Value = 金銭内訳_小切手;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_小切手", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (金銭内訳_領収書 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_領収書", SqlDbType.Decimal).Value = 金銭内訳_領収書;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_領収書", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (金銭内訳_合計 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_合計", SqlDbType.Decimal).Value = 金銭内訳_合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (金銭内訳_過剰金 != null)
                    {
                        cmd.Parameters.Add("@金銭内訳_過剰金", SqlDbType.Decimal).Value = 金銭内訳_過剰金;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金銭内訳_過剰金", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (消費チケ枚数_ＢＶ300 != null)
                    {
                        cmd.Parameters.Add("@消費チケ枚数_ＢＶ300", SqlDbType.Int).Value = 消費チケ枚数_ＢＶ300;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費チケ枚数_ＢＶ300", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (消費チケ枚数_ＢＶ200 != null)
                    {
                        cmd.Parameters.Add("@消費チケ枚数_ＢＶ200", SqlDbType.Int).Value = 消費チケ枚数_ＢＶ200;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費チケ枚数_ＢＶ200", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (消費チケ金額_ＢＶ300 != null)
                    {
                        cmd.Parameters.Add("@消費チケ金額_ＢＶ300", SqlDbType.Decimal).Value = 消費チケ金額_ＢＶ300;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費チケ金額_ＢＶ300", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (消費チケ金額_ＢＶ200 != null)
                    {
                        cmd.Parameters.Add("@消費チケ金額_ＢＶ200", SqlDbType.Decimal).Value = 消費チケ金額_ＢＶ200;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費チケ金額_ＢＶ200", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    // 更新の場合のみ編集日時を設定（新規の場合は設定しない）
                    if (修正日 != null)
                    {
                        cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = 修正日;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        // 失敗しても無視する場合はそのまま終了
                        if (ignoreFailure)
                        {
                            return true;
                        }

                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("売上精算_入力データ一時保存を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "精算日、部署ＣＤ、コースＣＤが重複しています。";
                    }
                    else
                    {
                        History.Error(ex);
                        Message = string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                    }
                    return false;

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    History.Error(ex);
                    Message = string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                    return false;
                }
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        #endregion
    }
}
