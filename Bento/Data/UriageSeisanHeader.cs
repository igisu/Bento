using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上精算_ヘッダ
    /// </summary>
    public class UriageSeisanHeader : TableBase2, IDataErrorInfo
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
        public  static UriageSeisanHeader GetUriageSeisanHeader(DateTime seisanbi, int busyoCD, int courseCD)
        {
            UriageSeisanHeader uriageSeisanHeader = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.精算日" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,A.コースＣＤ" +
                             "\r\n     ,A.配送担当者ＣＤ" +
                             "\r\n     ,A.配送担当者名" +
                             "\r\n     ,A.部署名" +
                             "\r\n     ,A.コース名" +
                             "\r\n     ,A.得意先数" +
                             "\r\n     ,A.現金_売上合計" +
                             "\r\n     ,A.現金_値引合計" +
                             "\r\n     ,A.現金_回収現金件数" +
                             "\r\n     ,A.現金_回収現金" +
                             "\r\n     ,A.現金_チケット束代" +
                             "\r\n     ,A.現金_本日現金合計" +
                             "\r\n     ,A.現金_食数" +
                             "\r\n     ,A.掛売_売上合計" +
                             "\r\n     ,A.掛売_値引合計" +
                             "\r\n     ,A.掛売_未集金" +
                             "\r\n     ,A.掛売_未集金外売上" +
                             "\r\n     ,A.掛売_本日掛売合計" +
                             "\r\n     ,A.掛売_食数" +
                             "\r\n     ,A.E_売上合計" +
                             "\r\n     ,A.E_値引合計" +
                             "\r\n     ,A.E_回収金額件数" +
                             "\r\n     ,A.E_回収金額" +
                             "\r\n     ,A.E_チケット束代" +
                             "\r\n     ,A.E_本日E合計" +
                             "\r\n     ,A.E_食数" +
                             "\r\n     ,A.合計_本日総売上" +
                             "\r\n     ,A.合計_本日総値引" +
                             "\r\n     ,A.合計_チケット売上" +
                             "\r\n     ,A.合計_バークレー売上" +
                             "\r\n     ,A.合計_実質総売上" +
                             "\r\n     ,A.消費チケ枚数_チケット" +
                             "\r\n     ,A.消費チケ枚数_ボーナス" +
                             "\r\n     ,A.消費チケ金額_チケット" +
                             "\r\n     ,A.消費チケ金額_ボーナス" +
                             "\r\n     ,A.消費チケ枚数_ＢＶ300" +
                             "\r\n     ,A.消費チケ枚数_ＢＶ200" +
                             "\r\n     ,A.消費チケ金額_ＢＶ300" +
                             "\r\n     ,A.消費チケ金額_ＢＶ200" +
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
                             "\r\n     ,A.チケット束_売上枚数" +
                             "\r\n     ,A.チケット束_売上金額" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     売上精算_ヘッダ AS A" +
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
                DataTable dt = new DataTable("売上精算ヘッダ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    uriageSeisanHeader = new UriageSeisanHeader(dt.Rows[0]);
                }
            }

            return uriageSeisanHeader;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private DateTime? _精算日;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private int? _配送担当者ＣＤ;
        private string _配送担当者名;
        private string _部署名;
        private string _コース名;
        private int? _得意先数;
        private decimal? _現金_売上合計;
        private decimal? _現金_値引合計;
        private int? _現金_回収現金件数;
        private decimal? _現金_回収現金;
        private decimal? _現金_チケット束代;
        private decimal? _現金_本日現金合計;
        private decimal? _現金_食数;
        private decimal? _掛売_売上合計;
        private decimal? _掛売_値引合計;
        private decimal? _掛売_未集金;
        private decimal? _掛売_未集金外売上;
        private decimal? _掛売_本日掛売合計;
        private decimal? _掛売_食数;
        private decimal? _E_売上合計;
        private decimal? _E_値引合計;
        private int? _E_回収金額件数;
        private decimal? _E_回収金額;
        private decimal? _E_チケット束代;
        private decimal? _E_本日E合計;
        private decimal? _E_食数;
        private decimal? _合計_本日総売上;
        private decimal? _合計_本日総値引;
        private decimal? _合計_チケット売上;
        private decimal? _合計_バークレー売上;
        private decimal? _合計_実質総売上;
        private int? _消費チケ枚数_チケット;
        private int? _消費チケ枚数_ボーナス;
        private decimal? _消費チケ金額_チケット;
        private decimal? _消費チケ金額_ボーナス;
        private int? _消費チケ枚数_ＢＶ300;
        private int? _消費チケ枚数_ＢＶ200;
        private decimal? _消費チケ金額_ＢＶ300;
        private decimal? _消費チケ金額_ＢＶ200;
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
        private int? _チケット束_売上枚数;
        private decimal? _チケット束_売上金額;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageSeisanHeader()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageSeisanHeader(UriageSeisanHeader src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            精算日 = src.精算日;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;
            配送担当者ＣＤ = src.配送担当者ＣＤ;
            配送担当者名 = src.配送担当者名;
            部署名 = src.部署名;
            コース名 = src.コース名;
            得意先数 = src.得意先数;
            現金_売上合計 = src.現金_売上合計;
            現金_値引合計 = src.現金_値引合計;
            現金_回収現金件数 = src.現金_回収現金件数;
            現金_回収現金 = src.現金_回収現金;
            現金_チケット束代 = src.現金_チケット束代;
            現金_本日現金合計 = src.現金_本日現金合計;
            現金_食数 = src.現金_食数;
            掛売_売上合計 = src.掛売_売上合計;
            掛売_値引合計 = src.掛売_値引合計;
            掛売_未集金 = src.掛売_未集金;
            掛売_未集金外売上 = src.掛売_未集金外売上;
            掛売_本日掛売合計 = src.掛売_本日掛売合計;
            掛売_食数 = src.掛売_食数;
            E_売上合計 = src.E_売上合計;
            E_値引合計 = src.E_値引合計;
            E_回収金額件数 = src.E_回収金額件数;
            E_回収金額 = src.E_回収金額;
            E_チケット束代 = src.E_チケット束代;
            E_本日E合計 = src.E_本日E合計;
            E_食数 = src.E_食数;
            合計_本日総売上 = src.合計_本日総売上;
            合計_本日総値引 = src.合計_本日総値引;
            合計_チケット売上 = src.合計_チケット売上;
            合計_バークレー売上 = src.合計_バークレー売上;
            合計_実質総売上 = src.合計_実質総売上;
            消費チケ枚数_チケット = src.消費チケ枚数_チケット;
            消費チケ枚数_ボーナス = src.消費チケ枚数_ボーナス;
            消費チケ金額_チケット = src.消費チケ金額_チケット;
            消費チケ金額_ボーナス = src.消費チケ金額_ボーナス;
            消費チケ枚数_ＢＶ300 = src.消費チケ枚数_ＢＶ300;
            消費チケ枚数_ＢＶ200 = src.消費チケ枚数_ＢＶ200;
            消費チケ金額_ＢＶ300 = src.消費チケ金額_ＢＶ300;
            消費チケ金額_ＢＶ200 = src.消費チケ金額_ＢＶ200;
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
            チケット束_売上枚数 = src.チケット束_売上枚数;
            チケット束_売上金額 = src.チケット束_売上金額;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// 売上精算入力データ一時保存からのコピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageSeisanHeader(UriageSeisanTemporary src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

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

            //base.Copy(src);       // 新規扱いなので、作成/修正担当者はコピーしない

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public UriageSeisanHeader(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

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
            部署名 = dr["部署名", version] as string;
            コース名 = dr["コース名", version] as string;
            得意先数 = dr["得意先数", version] as int?;
            現金_売上合計 = dr["現金_売上合計", version] as decimal?;
            現金_値引合計 = dr["現金_値引合計", version] as decimal?;
            現金_回収現金件数 = dr["現金_回収現金件数", version] as int?;
            現金_回収現金 = dr["現金_回収現金", version] as decimal?;
            現金_チケット束代 = dr["現金_チケット束代", version] as decimal?;
            現金_本日現金合計 = dr["現金_本日現金合計", version] as decimal?;
            現金_食数 = dr["現金_食数", version] as decimal?;
            掛売_売上合計 = dr["掛売_売上合計", version] as decimal?;
            掛売_値引合計 = dr["掛売_値引合計", version] as decimal?;
            掛売_未集金 = dr["掛売_未集金", version] as decimal?;
            掛売_未集金外売上 = dr["掛売_未集金外売上", version] as decimal?;
            掛売_本日掛売合計 = dr["掛売_本日掛売合計", version] as decimal?;
            掛売_食数 = dr["掛売_食数", version] as decimal?;
            E_売上合計 = dr["E_売上合計", version] as decimal?;
            E_値引合計 = dr["E_値引合計", version] as decimal?;
            E_回収金額件数 = dr["E_回収金額件数", version] as int?;
            E_回収金額 = dr["E_回収金額", version] as decimal?;
            E_チケット束代 = dr["E_チケット束代", version] as decimal?;
            E_本日E合計 = dr["E_本日E合計", version] as decimal?;
            E_食数 = dr["E_食数", version] as decimal?;
            合計_本日総売上 = dr["合計_本日総売上", version] as decimal?;
            合計_本日総値引 = dr["合計_本日総値引", version] as decimal?;
            合計_チケット売上 = dr["合計_チケット売上", version] as decimal?;
            合計_バークレー売上 = dr["合計_バークレー売上", version] as decimal?;
            合計_実質総売上 = dr["合計_実質総売上", version] as decimal?;
            消費チケ枚数_チケット = dr["消費チケ枚数_チケット", version] as int?;
            消費チケ枚数_ボーナス = dr["消費チケ枚数_ボーナス", version] as int?;
            消費チケ金額_チケット = dr["消費チケ金額_チケット", version] as decimal?;
            消費チケ金額_ボーナス = dr["消費チケ金額_ボーナス", version] as decimal?;
            消費チケ枚数_ＢＶ300 = dr["消費チケ枚数_ＢＶ300", version] as int?;
            消費チケ枚数_ＢＶ200 = dr["消費チケ枚数_ＢＶ200", version] as int?;
            消費チケ金額_ＢＶ300 = dr["消費チケ金額_ＢＶ300", version] as decimal?;
            消費チケ金額_ＢＶ200 = dr["消費チケ金額_ＢＶ200", version] as decimal?;
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
            チケット束_売上枚数 = dr["チケット束_売上枚数", version] as int?;
            チケット束_売上金額 = dr["チケット束_売上金額", version] as decimal?;

            base.FromDataRow(dr, version);

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 精算日
        {
            get { return _精算日; }
            set
            {
                _精算日 = value;
                _errors.Remove("精算日");
                if (value == null)
                {
                    _errors["精算日"] = "精算日を入力してください。";
                }
            }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set
            {
                _部署ＣＤ = value;
                _errors.Remove("部署ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
            }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                _errors.Remove("コースＣＤ");
                if (value == null)
                {
                    _errors["コースＣＤ"] = "コースＣＤを入力してください。";
                }
            }
        }

        public int? 配送担当者ＣＤ
        {
            get { return _配送担当者ＣＤ; }
            set
            {
                _配送担当者ＣＤ = value;
                _errors.Remove("配送担当者ＣＤ");
                配送担当者名 = null;
                if (value != null && _stopOverwrite == false)
                {
                    // 配送担当者ＣＤに該当する配送担当者名を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_配送担当者ＣＤ);
                    if (tantousya != null)
                    {
                        配送担当者名 = tantousya.担当者名;
                    }
                    else
                    {
                        _errors["配送担当者ＣＤ"] = "配送担当者ＣＤが見つかりません。";
                    }
                }
            }
        }

        public string 配送担当者名
        {
            get { return _配送担当者名; }
            set { _配送担当者名 = value; }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public int? 得意先数
        {
            get { return _得意先数; }
            set { _得意先数 = value; }
        }

        public decimal? 現金_売上合計
        {
            get { return _現金_売上合計; }
            set { _現金_売上合計 = value; }
        }

        public decimal? 現金_値引合計
        {
            get { return _現金_値引合計; }
            set { _現金_値引合計 = value; }
        }

        public int? 現金_回収現金件数
        {
            get { return _現金_回収現金件数; }
            set { _現金_回収現金件数 = value; }
        }

        public decimal? 現金_回収現金
        {
            get { return _現金_回収現金; }
            set { _現金_回収現金 = value; }
        }

        public decimal? 現金_チケット束代
        {
            get { return _現金_チケット束代; }
            set { _現金_チケット束代 = value; }
        }

        public decimal? 現金_本日現金合計
        {
            get { return _現金_本日現金合計; }
            set { _現金_本日現金合計 = value; }
        }

        public decimal? 現金_食数
        {
            get { return _現金_食数; }
            set { _現金_食数 = value; }
        }

        public decimal? 掛売_売上合計
        {
            get { return _掛売_売上合計; }
            set { _掛売_売上合計 = value; }
        }

        public decimal? 掛売_値引合計
        {
            get { return _掛売_値引合計; }
            set { _掛売_値引合計 = value; }
        }

        public decimal? 掛売_未集金
        {
            get { return _掛売_未集金; }
            set { _掛売_未集金 = value; }
        }

        public decimal? 掛売_未集金外売上
        {
            get { return _掛売_未集金外売上; }
            set { _掛売_未集金外売上 = value; }
        }

        public decimal? 掛売_本日掛売合計
        {
            get { return _掛売_本日掛売合計; }
            set { _掛売_本日掛売合計 = value; }
        }

        public decimal? 掛売_食数
        {
            get { return _掛売_食数; }
            set { _掛売_食数 = value; }
        }

        public decimal? E_売上合計
        {
            get { return _E_売上合計; }
            set { _E_売上合計 = value; }
        }

        public decimal? E_値引合計
        {
            get { return _E_値引合計; }
            set { _E_値引合計 = value; }
        }

        public int? E_回収金額件数
        {
            get { return _E_回収金額件数; }
            set { _E_回収金額件数 = value; }
        }

        public decimal? E_回収金額
        {
            get { return _E_回収金額; }
            set { _E_回収金額 = value; }
        }

        public decimal? E_チケット束代
        {
            get { return _E_チケット束代; }
            set { _E_チケット束代 = value; }
        }

        public decimal? E_本日E合計
        {
            get { return _E_本日E合計; }
            set { _E_本日E合計 = value; }
        }

        public decimal? E_食数
        {
            get { return _E_食数; }
            set { _E_食数 = value; }
        }

        public decimal? 合計_本日総売上
        {
            get { return _合計_本日総売上; }
            set { _合計_本日総売上 = value; }
        }

        public decimal? 合計_本日総値引
        {
            get { return _合計_本日総値引; }
            set { _合計_本日総値引 = value; }
        }

        public decimal? 合計_チケット売上
        {
            get { return _合計_チケット売上; }
            set { _合計_チケット売上 = value; }
        }

        public decimal? 合計_バークレー売上
        {
            get { return _合計_バークレー売上; }
            set { _合計_バークレー売上 = value; }
        }

        public decimal? 合計_実質総売上
        {
            get { return _合計_実質総売上; }
            set { _合計_実質総売上 = value; }
        }

        public int? 消費チケ枚数_チケット
        {
            get { return _消費チケ枚数_チケット; }
            set { _消費チケ枚数_チケット = value; }
        }

        public int? 消費チケ枚数_ボーナス
        {
            get { return _消費チケ枚数_ボーナス; }
            set { _消費チケ枚数_ボーナス = value; }
        }

        public decimal? 消費チケ金額_チケット
        {
            get { return _消費チケ金額_チケット; }
            set { _消費チケ金額_チケット = value; }
        }

        public decimal? 消費チケ金額_ボーナス
        {
            get { return _消費チケ金額_ボーナス; }
            set { _消費チケ金額_ボーナス = value; }
        }

        public int? 消費チケ枚数_ＢＶ300
        {
            get { return _消費チケ枚数_ＢＶ300; }
            set
            {
                _消費チケ枚数_ＢＶ300 = value;
                消費チケ金額_ＢＶ300 = (value == null ? null : value * 300);
            }
        }

        public int? 消費チケ枚数_ＢＶ200
        {
            get { return _消費チケ枚数_ＢＶ200; }
            set
            {
                _消費チケ枚数_ＢＶ200 = value;
                消費チケ金額_ＢＶ200 = (value == null ? null : value * 200);
            }
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

        public int? チケット束_売上枚数
        {
            get { return _チケット束_売上枚数; }
            set { _チケット束_売上枚数 = value; }
        }

        public decimal? チケット束_売上金額
        {
            get { return _チケット束_売上金額; }
            set { _チケット束_売上金額 = value; }
        }


        public decimal? 現金合計 { get; set;}

        public decimal? 掛売合計 { get; set;}

        public decimal? E合計 { get; set;}

        public decimal? チケット代合計
        {
            get
            {
                return (_現金_チケット束代 ?? 0m) + (_E_チケット束代 ?? 0m);
            }
        }

        public int? 金額_10000円
        {
            get { return _金銭内訳_10000円 == null ? null : _金銭内訳_10000円 * 10000; }
            set { }
        }

        public int? 金額_5000円
        {
            get { return _金銭内訳_5000円 == null ? null : _金銭内訳_5000円 * 5000; }
            set { }
        }

        public int? 金額_2000円
        {
            get { return _金銭内訳_2000円 == null ? null : _金銭内訳_2000円 * 2000; }
            set { }
        }

        public int? 金額_1000円
        {
            get { return _金銭内訳_1000円 == null ? null : _金銭内訳_1000円 * 1000; }
            set { }
        }

        public int? 金額_500円
        {
            get { return _金銭内訳_500円 == null ? null : _金銭内訳_500円 * 500; }
            set { }
        }

        public int? 金額_100円
        {
            get { return _金銭内訳_100円 == null ? null : _金銭内訳_100円 * 100; }
            set { }
        }

        public int? 金額_50円
        {
            get { return _金銭内訳_50円 == null ? null : _金銭内訳_50円 * 50; }
            set { }
        }

        public int? 金額_10円
        {
            get { return _金銭内訳_10円 == null ? null : _金銭内訳_10円 * 10; }
            set { }
        }

        public int? 金額_5円
        {
            get { return _金銭内訳_5円 == null ? null : _金銭内訳_5円 * 5; }
            set { }
        }

        public int? 金額_1円
        {
            get { return _金銭内訳_1円 == null ? null : _金銭内訳_1円; }
            set { }
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
        /// 合計を計算
        /// </summary>
        public void CalcTotal()
        {
            // 売上精算入力画面の売上管理タブ
            合計_本日総売上 = (現金_売上合計 ?? 0m) + (掛売_売上合計 ?? 0m) + (E_売上合計 ?? 0m) + (消費チケ金額_チケット ?? 0m);

            // 合計_本日総値引は、売上区分を問わず全ての売上の値引の合計を画面で取得している

            // 現金_チケット束代とE_チケット束代と合計_バークレー売上は画面にて算出する
            // 合計_チケット売上は総売上には使用しない

            合計_実質総売上 = (合計_本日総売上 ?? 0m) - (合計_本日総値引 ?? 0m) + (現金_チケット束代 ?? 0m) + (E_チケット束代 ?? 0m) + (合計_バークレー売上 ?? 0m);

            // 同じく金銭明細入力タブ
            // 金種ごとの金額は、「金額_?円」プロパティで算出済み
            金銭内訳_合計 = (金額_10000円 ?? 0m) + (金額_5000円 ?? 0m) + (金額_2000円 ?? 0m) + (金額_1000円 ?? 0m) +
                            (金額_500円 ?? 0m) + (金額_100円 ?? 0m) + (金額_50円 ?? 0m) + (金額_10円 ?? 0m) + (金額_5円 ?? 0m) + (金額_1円 ?? 0m) +
                            (金銭内訳_小切手 ?? 0m) + (金銭内訳_領収書 ?? 0m);

            // 現金_本日現金合計は、新規の場合は画面側で算出済み
            金銭内訳_過剰金 = (金銭内訳_合計 ?? 0m) - (現金_本日現金合計 ?? 0m);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(UriageSeisanHeader target)
        {
            if (精算日 == target.精算日 &&
                部署ＣＤ == target.部署ＣＤ &&
                コースＣＤ == target.コースＣＤ &&
                配送担当者ＣＤ == target.配送担当者ＣＤ &&
                配送担当者名 == target.配送担当者名 &&
                部署名 == target.部署名 &&
                コース名 == target.コース名 &&
                得意先数 == target.得意先数 &&
                現金_売上合計 == target.現金_売上合計 &&
                現金_値引合計 == target.現金_値引合計 &&
                現金_回収現金件数 == target.現金_回収現金件数 &&
                現金_回収現金 == target.現金_回収現金 &&
                現金_チケット束代 == target.現金_チケット束代 &&
                現金_本日現金合計 == target.現金_本日現金合計 &&
                現金_食数 == target.現金_食数 &&
                掛売_売上合計 == target.掛売_売上合計 &&
                掛売_値引合計 == target.掛売_値引合計 &&
                掛売_未集金 == target.掛売_未集金 &&
                掛売_未集金外売上 == target.掛売_未集金外売上 &&
                掛売_本日掛売合計 == target.掛売_本日掛売合計 &&
                掛売_食数 == target.掛売_食数 &&
                E_売上合計 == target.E_売上合計 &&
                E_値引合計 == target.E_値引合計 &&
                E_本日E合計 == target.E_本日E合計 &&
                E_食数 == target.E_食数 &&
                E_回収金額 == target.E_回収金額 &&
                合計_本日総売上 == target.合計_本日総売上 &&
                合計_本日総値引 == target.合計_本日総値引 &&
                合計_チケット売上 == target.合計_チケット売上 &&
                合計_バークレー売上 == target.合計_バークレー売上 &&
                合計_実質総売上 == target.合計_実質総売上 &&
                消費チケ枚数_チケット == target.消費チケ枚数_チケット &&
                消費チケ枚数_ボーナス == target.消費チケ枚数_ボーナス &&
                消費チケ金額_チケット == target.消費チケ金額_チケット &&
                消費チケ金額_ボーナス == target.消費チケ金額_ボーナス &&
                消費チケ枚数_ＢＶ300 == target.消費チケ枚数_ＢＶ300 &&
                消費チケ枚数_ＢＶ200 == target.消費チケ枚数_ＢＶ200 &&
                消費チケ金額_ＢＶ300 == target.消費チケ金額_ＢＶ300 &&
                消費チケ金額_ＢＶ200 == target.消費チケ金額_ＢＶ200 &&
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
                チケット束_売上枚数 == target.チケット束_売上枚数 &&
                チケット束_売上金額 == target.チケット束_売上金額)
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
            string sql = "\r\n INSERT INTO 売上精算_ヘッダ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,精算日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,配送担当者ＣＤ" +
                         "\r\n     ,配送担当者名" +
                         "\r\n     ,部署名" +
                         "\r\n     ,コース名" +
                         "\r\n     ,得意先数" +
                         "\r\n     ,現金_売上合計" +
                         "\r\n     ,現金_値引合計" +
                         "\r\n     ,現金_回収現金件数" +
                         "\r\n     ,現金_回収現金" +
                         "\r\n     ,現金_チケット束代" +
                         "\r\n     ,現金_本日現金合計" +
                         "\r\n     ,現金_食数" +
                         "\r\n     ,掛売_売上合計" +
                         "\r\n     ,掛売_値引合計" +
                         "\r\n     ,掛売_未集金" +
                         "\r\n     ,掛売_未集金外売上" +
                         "\r\n     ,掛売_本日掛売合計" +
                         "\r\n     ,掛売_食数" +
                         "\r\n     ,E_売上合計" +
                         "\r\n     ,E_値引合計" +
                         "\r\n     ,E_回収金額件数" +
                         "\r\n     ,E_回収金額" +
                         "\r\n     ,E_チケット束代" +
                         "\r\n     ,E_本日E合計" +
                         "\r\n     ,E_食数" +
                         "\r\n     ,合計_本日総売上" +
                         "\r\n     ,合計_本日総値引" +
                         "\r\n     ,合計_チケット売上" +
                         "\r\n     ,合計_バークレー売上" +
                         "\r\n     ,合計_実質総売上" +
                         "\r\n     ,消費チケ枚数_チケット" +
                         "\r\n     ,消費チケ枚数_ボーナス" +
                         "\r\n     ,消費チケ金額_チケット" +
                         "\r\n     ,消費チケ金額_ボーナス" +
                         "\r\n     ,消費チケ枚数_ＢＶ300" +
                         "\r\n     ,消費チケ枚数_ＢＶ200" +
                         "\r\n     ,消費チケ金額_ＢＶ300" +
                         "\r\n     ,消費チケ金額_ＢＶ200" +
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
                         "\r\n     ,チケット束_売上枚数" +
                         "\r\n     ,チケット束_売上金額" +
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
                         "\r\n     ,@部署名" +
                         "\r\n     ,@コース名" +
                         "\r\n     ,@得意先数" +
                         "\r\n     ,@現金_売上合計" +
                         "\r\n     ,@現金_値引合計" +
                         "\r\n     ,@現金_回収現金件数" +
                         "\r\n     ,@現金_回収現金" +
                         "\r\n     ,@現金_チケット束代" +
                         "\r\n     ,@現金_本日現金合計" +
                         "\r\n     ,@現金_食数" +
                         "\r\n     ,@掛売_売上合計" +
                         "\r\n     ,@掛売_値引合計" +
                         "\r\n     ,@掛売_未集金" +
                         "\r\n     ,@掛売_未集金外売上" +
                         "\r\n     ,@掛売_本日掛売合計" +
                         "\r\n     ,@掛売_食数" +
                         "\r\n     ,@E_売上合計" +
                         "\r\n     ,@E_値引合計" +
                         "\r\n     ,@E_回収金額件数" +
                         "\r\n     ,@E_回収金額" +
                         "\r\n     ,@E_チケット束代" +
                         "\r\n     ,@E_本日E合計" +
                         "\r\n     ,@E_食数" +
                         "\r\n     ,@合計_本日総売上" +
                         "\r\n     ,@合計_本日総値引" +
                         "\r\n     ,@合計_チケット売上" +
                         "\r\n     ,@合計_バークレー売上" +
                         "\r\n     ,@合計_実質総売上" +
                         "\r\n     ,@消費チケ枚数_チケット" +
                         "\r\n     ,@消費チケ枚数_ボーナス" +
                         "\r\n     ,@消費チケ金額_チケット" +
                         "\r\n     ,@消費チケ金額_ボーナス" +
                         "\r\n     ,@消費チケ枚数_ＢＶ300" +
                         "\r\n     ,@消費チケ枚数_ＢＶ200" +
                         "\r\n     ,@消費チケ金額_ＢＶ300" +
                         "\r\n     ,@消費チケ金額_ＢＶ200" +
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
                         "\r\n     ,@チケット束_売上枚数" +
                         "\r\n     ,@チケット束_売上金額" +
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
            string sql = "\r\n UPDATE 売上精算_ヘッダ" +
                         "\r\n SET  配送担当者ＣＤ        = @配送担当者ＣＤ" +
                         "\r\n     ,配送担当者名          = @配送担当者名" +
                         "\r\n     ,部署名                = @部署名" +
                         "\r\n     ,コース名              = @コース名" +
                         "\r\n     ,得意先数              = @得意先数" +
                         "\r\n     ,現金_売上合計         = @現金_売上合計" +
                         "\r\n     ,現金_値引合計         = @現金_値引合計" +
                         "\r\n     ,現金_回収現金件数     = @現金_回収現金件数" +
                         "\r\n     ,現金_回収現金         = @現金_回収現金" +
                         "\r\n     ,現金_チケット束代     = @現金_チケット束代" +
                         "\r\n     ,現金_本日現金合計     = @現金_本日現金合計" +
                         "\r\n     ,現金_食数             = @現金_食数" +
                         "\r\n     ,掛売_売上合計         = @掛売_売上合計" +
                         "\r\n     ,掛売_値引合計         = @掛売_値引合計" +
                         "\r\n     ,掛売_未集金           = @掛売_未集金" +
                         "\r\n     ,掛売_未集金外売上     = @掛売_未集金外売上" +
                         "\r\n     ,掛売_本日掛売合計     = @掛売_本日掛売合計" +
                         "\r\n     ,掛売_食数             = @掛売_食数" +
                         "\r\n     ,E_売上合計            = @E_売上合計" +
                         "\r\n     ,E_値引合計            = @E_値引合計" +
                         "\r\n     ,E_回収金額件数        = @E_回収金額件数" +
                         "\r\n     ,E_回収金額            = @E_回収金額" +
                         "\r\n     ,E_チケット束代        = @E_チケット束代" +
                         "\r\n     ,E_本日E合計           = @E_本日E合計" +
                         "\r\n     ,E_食数                = @E_食数" +
                         "\r\n     ,合計_本日総売上       = @合計_本日総売上" +
                         "\r\n     ,合計_本日総値引       = @合計_本日総値引" +
                         "\r\n     ,合計_チケット売上     = @合計_チケット売上" +
                         "\r\n     ,合計_バークレー売上   = @合計_バークレー売上" +
                         "\r\n     ,合計_実質総売上       = @合計_実質総売上" +
                         "\r\n     ,消費チケ枚数_チケット = @消費チケ枚数_チケット" +
                         "\r\n     ,消費チケ枚数_ボーナス = @消費チケ枚数_ボーナス" +
                         "\r\n     ,消費チケ金額_チケット = @消費チケ金額_チケット" +
                         "\r\n     ,消費チケ金額_ボーナス = @消費チケ金額_ボーナス" +
                         "\r\n     ,消費チケ枚数_ＢＶ300  = @消費チケ枚数_ＢＶ300" +
                         "\r\n     ,消費チケ枚数_ＢＶ200  = @消費チケ枚数_ＢＶ200" +
                         "\r\n     ,消費チケ金額_ＢＶ300  = @消費チケ金額_ＢＶ300" +
                         "\r\n     ,消費チケ金額_ＢＶ200  = @消費チケ金額_ＢＶ200" +
                         "\r\n     ,金銭内訳_10000円      = @金銭内訳_10000円" +
                         "\r\n     ,金銭内訳_5000円       = @金銭内訳_5000円" +
                         "\r\n     ,金銭内訳_2000円       = @金銭内訳_2000円" +
                         "\r\n     ,金銭内訳_1000円       = @金銭内訳_1000円" +
                         "\r\n     ,金銭内訳_500円        = @金銭内訳_500円" +
                         "\r\n     ,金銭内訳_100円        = @金銭内訳_100円" +
                         "\r\n     ,金銭内訳_50円         = @金銭内訳_50円" +
                         "\r\n     ,金銭内訳_10円         = @金銭内訳_10円" +
                         "\r\n     ,金銭内訳_5円          = @金銭内訳_5円" +
                         "\r\n     ,金銭内訳_1円          = @金銭内訳_1円" +
                         "\r\n     ,金銭内訳_小切手       = @金銭内訳_小切手" +
                         "\r\n     ,金銭内訳_領収書       = @金銭内訳_領収書" +
                         "\r\n     ,金銭内訳_合計         = @金銭内訳_合計" +
                         "\r\n     ,金銭内訳_過剰金       = @金銭内訳_過剰金" +
                         "\r\n     ,チケット束_売上枚数   = @チケット束_売上枚数" +
                         "\r\n     ,チケット束_売上金額   = @チケット束_売上金額" +
                         "\r\n     ,修正担当者ＣＤ        = @修正担当者ＣＤ" +
                         "\r\n     ,修正日                = getdate()" +
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
            string sql = "\r\n DELETE FROM 売上精算_ヘッダ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 精算日     = @精算日" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ" +
                         "\r\n   AND 修正日     = @修正日";

            return Save(con, tran, sql, History.ProcessType.Delete);
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

                    if (部署名 != null)
                    {
                        cmd.Parameters.Add("@部署名", SqlDbType.VarChar).Value = 部署名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (コース名 != null)
                    {
                        cmd.Parameters.Add("@コース名", SqlDbType.VarChar).Value = コース名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コース名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (得意先数 != null)
                    {
                        cmd.Parameters.Add("@得意先数", SqlDbType.Int).Value = 得意先数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (現金_売上合計 != null)
                    {
                        cmd.Parameters.Add("@現金_売上合計", SqlDbType.Decimal).Value = 現金_売上合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@現金_売上合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (現金_値引合計 != null)
                    {
                        cmd.Parameters.Add("@現金_値引合計", SqlDbType.Decimal).Value = 現金_値引合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@現金_値引合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (現金_回収現金件数 != null)
                    {
                        cmd.Parameters.Add("@現金_回収現金件数", SqlDbType.Int).Value = 現金_回収現金件数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@現金_回収現金件数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (現金_回収現金 != null)
                    {
                        cmd.Parameters.Add("@現金_回収現金", SqlDbType.Decimal).Value = 現金_回収現金;
                    }
                    else
                    {
                        cmd.Parameters.Add("@現金_回収現金", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (現金_チケット束代 != null)
                    {
                        cmd.Parameters.Add("@現金_チケット束代", SqlDbType.Decimal).Value = 現金_チケット束代;
                    }
                    else
                    {
                        cmd.Parameters.Add("@現金_チケット束代", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (現金_本日現金合計 != null)
                    {
                        cmd.Parameters.Add("@現金_本日現金合計", SqlDbType.Decimal).Value = 現金_本日現金合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@現金_本日現金合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (現金_食数 != null)
                    {
                        cmd.Parameters.Add("@現金_食数", SqlDbType.Decimal).Value = 現金_食数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@現金_食数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (掛売_売上合計 != null)
                    {
                        cmd.Parameters.Add("@掛売_売上合計", SqlDbType.Decimal).Value = 掛売_売上合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@掛売_売上合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (掛売_値引合計 != null)
                    {
                        cmd.Parameters.Add("@掛売_値引合計", SqlDbType.Decimal).Value = 掛売_値引合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@掛売_値引合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (掛売_未集金 != null)
                    {
                        cmd.Parameters.Add("@掛売_未集金", SqlDbType.Decimal).Value = 掛売_未集金;
                    }
                    else
                    {
                        cmd.Parameters.Add("@掛売_未集金", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (掛売_未集金外売上 != null)
                    {
                        cmd.Parameters.Add("@掛売_未集金外売上", SqlDbType.Decimal).Value = 掛売_未集金外売上;
                    }
                    else
                    {
                        cmd.Parameters.Add("@掛売_未集金外売上", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (掛売_本日掛売合計 != null)
                    {
                        cmd.Parameters.Add("@掛売_本日掛売合計", SqlDbType.Decimal).Value = 掛売_本日掛売合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@掛売_本日掛売合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (掛売_食数 != null)
                    {
                        cmd.Parameters.Add("@掛売_食数", SqlDbType.Decimal).Value = 掛売_食数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@掛売_食数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (E_売上合計 != null)
                    {
                        cmd.Parameters.Add("@E_売上合計", SqlDbType.Decimal).Value = E_売上合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@E_売上合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (E_値引合計 != null)
                    {
                        cmd.Parameters.Add("@E_値引合計", SqlDbType.Decimal).Value = E_値引合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@E_値引合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (E_回収金額件数 != null)
                    {
                        cmd.Parameters.Add("@E_回収金額件数", SqlDbType.Int).Value = E_回収金額件数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@E_回収金額件数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (E_回収金額 != null)
                    {
                        cmd.Parameters.Add("@E_回収金額", SqlDbType.Decimal).Value = E_回収金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@E_回収金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (E_チケット束代 != null)
                    {
                        cmd.Parameters.Add("@E_チケット束代", SqlDbType.Decimal).Value = E_チケット束代;
                    }
                    else
                    {
                        cmd.Parameters.Add("@E_チケット束代", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (E_本日E合計 != null)
                    {
                        cmd.Parameters.Add("@E_本日E合計", SqlDbType.Decimal).Value = E_本日E合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@E_本日E合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (E_食数 != null)
                    {
                        cmd.Parameters.Add("@E_食数", SqlDbType.Decimal).Value = E_食数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@E_食数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (合計_本日総売上 != null)
                    {
                        cmd.Parameters.Add("@合計_本日総売上", SqlDbType.Decimal).Value = 合計_本日総売上;
                    }
                    else
                    {
                        cmd.Parameters.Add("@合計_本日総売上", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (合計_本日総値引 != null)
                    {
                        cmd.Parameters.Add("@合計_本日総値引", SqlDbType.Decimal).Value = 合計_本日総値引;
                    }
                    else
                    {
                        cmd.Parameters.Add("@合計_本日総値引", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (合計_チケット売上 != null)
                    {
                        cmd.Parameters.Add("@合計_チケット売上", SqlDbType.Decimal).Value = 合計_チケット売上;
                    }
                    else
                    {
                        cmd.Parameters.Add("@合計_チケット売上", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (合計_バークレー売上 != null)
                    {
                        cmd.Parameters.Add("@合計_バークレー売上", SqlDbType.Decimal).Value = 合計_バークレー売上;
                    }
                    else
                    {
                        cmd.Parameters.Add("@合計_バークレー売上", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (合計_実質総売上 != null)
                    {
                        cmd.Parameters.Add("@合計_実質総売上", SqlDbType.Decimal).Value = 合計_実質総売上;
                    }
                    else
                    {
                        cmd.Parameters.Add("@合計_実質総売上", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (消費チケ枚数_チケット != null)
                    {
                        cmd.Parameters.Add("@消費チケ枚数_チケット", SqlDbType.Int).Value = 消費チケ枚数_チケット;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費チケ枚数_チケット", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (消費チケ枚数_ボーナス != null)
                    {
                        cmd.Parameters.Add("@消費チケ枚数_ボーナス", SqlDbType.Int).Value = 消費チケ枚数_ボーナス;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費チケ枚数_ボーナス", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (消費チケ金額_チケット != null)
                    {
                        cmd.Parameters.Add("@消費チケ金額_チケット", SqlDbType.Decimal).Value = 消費チケ金額_チケット;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費チケ金額_チケット", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (消費チケ金額_ボーナス != null)
                    {
                        cmd.Parameters.Add("@消費チケ金額_ボーナス", SqlDbType.Decimal).Value = 消費チケ金額_ボーナス;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費チケ金額_ボーナス", SqlDbType.Decimal).Value = DBNull.Value;
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

                    if (チケット束_売上枚数 != null)
                    {
                        cmd.Parameters.Add("@チケット束_売上枚数", SqlDbType.Int).Value = チケット束_売上枚数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@チケット束_売上枚数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (チケット束_売上金額 != null)
                    {
                        cmd.Parameters.Add("@チケット束_売上金額", SqlDbType.Decimal).Value = チケット束_売上金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@チケット束_売上金額", SqlDbType.Decimal).Value = DBNull.Value;
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
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("売上精算_ヘッダを{0}しました。", historyProcess.DisplayName());
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
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        #endregion
    }
}
