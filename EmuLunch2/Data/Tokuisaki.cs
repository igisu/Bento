using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先マスタ
    /// </summary>
    /// <remarks>
    /// 以下の項目は必須とはせずに未入力の警告のみとする。
    /// ・得意先名
    /// ・売掛現金区分
    /// ・税区分
    /// ・丸め処理区分
    /// ※ホントは必須にすべきだが、未登録の電話番号からの着信で新規得意先として登録する際は必須にはしていないので
    /// </remarks>
    public class Tokuisaki : TableBase2, IDataErrorInfo, ITokuisaki
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// 全てON（休日）
        /// </summary>
        public static string ALL_ON = "1111111";

        /// <summary>
        /// 全てOFF（平日）
        /// </summary>
        public static string ALL_OFF = "0000000";
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="tokcd">得意先ＣＤ</param>
        /// <returns></returns>
        public static Tokuisaki GetTokuisaki(int tokcd)
        {
            return GetTokuisaki(tokcd, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="tokcd">得意先ＣＤ</param>
        /// <returns></returns>
        public static Tokuisaki GetTokuisakiReload(int tokcd)
        {
            return GetTokuisaki(tokcd, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="tokcd">得意先ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static Tokuisaki GetTokuisaki(int tokcd, bool isValid)
        {
            Tokuisaki tokuisaki = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.得意先ＣＤ" +
                             "\r\n     ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                             "\r\n     ,A.得意先名" +
                             "\r\n     ,A.得意先名カナ" +
                             "\r\n     ,A.得意先名略称" +
                             "\r\n     ,A.郵便番号" +
                             "\r\n     ,A.住所1" +
                             "\r\n     ,A.住所2" +
                             "\r\n     ,A.電話番号1" +
                             "\r\n     ,A.ＦＡＸ1" +
                             "\r\n     ,A.電話番号2" +
                             "\r\n     ,A.ＦＡＸ2" +
                             "\r\n     ,A.お届け先郵便番号" +
                             "\r\n     ,A.お届け先住所1" +
                             "\r\n     ,A.お届け先住所2" +
                             "\r\n     ,A.お届け先電話番号1" +
                             "\r\n     ,A.お届け先ＦＡＸ1" +
                             "\r\n     ,A.お届け先電話番号2" +
                             "\r\n     ,A.お届け先ＦＡＸ2" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,B.部署名" +
                             "\r\n     ,A.売掛現金区分" +
                             "\r\n     ,A.電話確認区分" +
                             "\r\n     ,A.締区分" +
                             "\r\n     ,A.月締日" +
                             "\r\n     ,A.週締曜日" +
                             "\r\n     ,A.支払サイト" +
                             "\r\n     ,A.支払日" +
                             "\r\n     ,A.請求先ＣＤ" +
                             "\r\n     ,S.得意先名 AS 請求先名" +
                             "\r\n     ,A.支払方法" +
                             "\r\n     ,A.クレカ顧客番号" +
                             "\r\n     ,A.クレカ番号下4桁" +
                             "\r\n     ,A.クレカ有効期限" +
                             "\r\n     ,A.クレカ無効フラグ" +
                             "\r\n     ,A.クレカ有効性確認日" +
                             "\r\n     ,A.振込手数料フラグ" +
                             "\r\n     ,A.部署合算請求フラグ" +
                             "\r\n     ,A.金融機関ＣＤ" +
                             "\r\n     ,K.銀行名 AS 金融機関名" +
                             "\r\n     ,A.金融機関支店ＣＤ" +
                             "\r\n     ,KB.支店名 AS 金融機関支店名" +
                             "\r\n     ,A.記号番号" +
                             "\r\n     ,A.口座種別" +
                             "\r\n     ,A.口座番号" +
                             "\r\n     ,A.口座名義人" +
                             "\r\n     ,A.チケット枚数" +
                             "\r\n     ,A.サービスチケット枚数" +
                             "\r\n     ,A.営業担当者ＣＤ" +
                             "\r\n     ,A3.担当者名 AS 営業担当者名" +
                             "\r\n     ,A.獲得営業者ＣＤ" +
                             "\r\n     ,A4.担当者名 AS 獲得営業者名" +
                             "\r\n     ,A.登録担当者ＣＤ" +
                             "\r\n     ,A5.担当者名 AS 登録担当者名" +
                             "\r\n     ,A.受注得意先ＣＤ" +
                             "\r\n     ,J.得意先名 AS 受注得意先名" +
                             "\r\n     ,A.受注方法" +
                             "\r\n     ,A.電話確認時間_時" +
                             "\r\n     ,A.電話確認時間_分" +
                             "\r\n     ,A.味噌汁区分" +
                             "\r\n     ,A.ふりかけ区分" +
                             "\r\n     ,A.納品書発行区分" +
                             "\r\n     ,A.空き容器回収区分" +
                             "\r\n     ,A.請求書敬称" +
                             "\r\n     ,A.備考１" +
                             "\r\n     ,A.備考２" +
                             "\r\n     ,A.備考３" +
                             "\r\n     ,A.状態区分" +
                             "\r\n     ,A.承認日" +
                             "\r\n     ,A.承認者ＣＤ" +
                             "\r\n     ,A6.担当者名 AS 承認者名" +
                             "\r\n     ,A.状態理由区分" +
                             "\r\n     ,A.休日設定" +
                             "\r\n     ,A.新規登録日" +
                             "\r\n     ,A.税区分" +
                             "\r\n     ,A.丸め処理区分" +
                             "\r\n     ,A.祝日配送区分" +
                             "\r\n     ,A.誕生日1" +
                             "\r\n     ,A.誕生日2" +
                             "\r\n     ,A.失客日" +
                             "\r\n     ,A.EB銀行支店ＣＤ" +
                             "\r\n     ,A.受注時特記事項" +
                             "\r\n     ,A.請求書郵便マークフラグ" +
                             "\r\n     ,A.集計ＣＤ" +
                             "\r\n     ,A.配送得意先ＣＤ" +
                             "\r\n     ,H.得意先名 AS 配送先名" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     得意先マスタ AS A" +
                             "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.部署ＣＤ = B.部署ＣＤ" +
                             "\r\n     LEFT JOIN 得意先マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND ISNULL(S.無効フラグ, 0) = 0 AND A.請求先ＣＤ = S.得意先ＣＤ" +
                             "\r\n     LEFT JOIN 金融機関名称 AS K ON A.金融機関ＣＤ = K.銀行ＣＤ" +
                             "\r\n     LEFT JOIN 金融機関支店名称 AS KB ON A.金融機関ＣＤ = KB.銀行ＣＤ AND A.金融機関支店ＣＤ = KB.支店ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A3.企業ＣＤ = @企業ＣＤ AND A.営業担当者ＣＤ = A3.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A4 ON A4.企業ＣＤ = @企業ＣＤ AND A.獲得営業者ＣＤ = A4.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A5 ON A5.企業ＣＤ = @企業ＣＤ AND A.登録担当者ＣＤ = A5.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A6 ON A6.企業ＣＤ = @企業ＣＤ AND A.承認者ＣＤ = A6.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 得意先マスタ AS J ON S.企業ＣＤ = @企業ＣＤ AND A.受注得意先ＣＤ = J.得意先ＣＤ" +
                             "\r\n     LEFT JOIN 得意先マスタ AS H ON H.企業ＣＤ = @企業ＣＤ AND ISNULL(H.無効フラグ, 0) = 0 AND A.配送得意先ＣＤ = H.得意先ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                             "{0}";

                string where = "";
                if (isValid)
                {
                    where = "\r\n AND ISNULL(A.無効フラグ, 0) = 0";
                }

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = string.Format(sql, where);
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokcd;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("得意先マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    tokuisaki = new Tokuisaki(dt.Rows[0]);
                }
            }

            return tokuisaki;
        }

        /// <summary>
        /// 休日設定の該当の曜日の値を返す（スタティックメソッド版）
        /// </summary>
        /// <param name="kyujitsuSettei">休日設定</param>
        /// <param name="dayOfWeek">休日設定の曜日インデックス</param>
        /// <returns>休日の場合はtrue。それ以外はfalse。</returns>
        public static bool GetHoliday(string kyujitsuSettei, DayOfWeek dayOfWeek)
        {
            if (string.IsNullOrEmpty(kyujitsuSettei))
            {
                return false;
            }
            // 日:0～土:6として、該当の曜日の位置の文字を変換する
            int index = (int)dayOfWeek;
            if (kyujitsuSettei.Length > index)
            {
                string s = kyujitsuSettei.ToCharArray()[index].ToString();
                if (s == "1")
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        public bool stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        /// <summary>
        /// 警告情報マップ
        /// </summary>
        private Dictionary<string, string> _warns = new Dictionary<string, string>();

        private int? _得意先ＣＤ;
        private int? _得意先ＣＤorg;            // レコード読込直後の値（更新・削除時に使用する）
        private string _得意先名;
        private string _得意先名カナ;
        private string _得意先名略称;
        private string _郵便番号;
        private string _住所1;
        private string _住所2;
        private string _電話番号1;
        private string _ＦＡＸ1;
        private string _電話番号2;
        private string _ＦＡＸ2;
        private string _お届け先郵便番号;
        private string _お届け先住所1;
        private string _お届け先住所2;
        private string _お届け先電話番号1;
        private string _お届け先ＦＡＸ1;
        private string _お届け先電話番号2;
        private string _お届け先ＦＡＸ2;
        private int? _部署ＣＤ;
        private int? _売掛現金区分;
        private int? _電話確認区分;
        private int? _締区分;
        private int? _月締日;
        private int? _週締曜日;
        private int? _支払サイト;
        private int? _支払日;
        private int? _請求先ＣＤ;
        private int? _支払方法;
        private string _クレカ顧客番号;
        private string _クレカ番号下4桁;
        private string _クレカ有効期限;
        private bool? _クレカ無効フラグ;
        private DateTime? _クレカ有効性確認日;
        private bool? _振込手数料フラグ;
        private bool? _部署合算請求フラグ;
        private string _金融機関ＣＤ;
        private string _金融機関支店ＣＤ;
        private string _記号番号;
        private int? _口座種別;
        private string _口座番号;
        private string _口座名義人;
        private decimal? _チケット枚数;
        private decimal? _サービスチケット枚数;
        private int? _営業担当者ＣＤ;
        private int? _獲得営業者ＣＤ;
        private int? _登録担当者ＣＤ;
        private int? _受注得意先ＣＤ;
        private int? _受注方法;
        private int? _電話確認時間_時;
        private int? _電話確認時間_分;
        private bool? _味噌汁区分;
        private bool? _ふりかけ区分;
        private bool? _納品書発行区分;
        private int? _空き容器回収区分;
        private int? _請求書敬称;
        private string _備考１;
        private string _備考２;
        private string _備考３;
        private int? _状態区分;
        private DateTime? _承認日;
        private int? _承認者ＣＤ;
        private int? _状態理由区分;
        private string _休日設定;
        private DateTime? _新規登録日;
        private int? _税区分;
        private int? _丸め処理区分;
        private bool? _祝日配送区分;
        private DateTime? _誕生日1;
        private DateTime? _誕生日2;
        private DateTime? _失客日;
        private string _EB銀行支店ＣＤ;
        private string _受注時特記事項;
        private bool? _請求書郵便マークフラグ;
        private int? _集計ＣＤ;
        private int? _配送得意先ＣＤ;
        private bool? _無効フラグ;

        private string _部署名;
        private string _請求先名;
        private string _金融機関名;
        private string _金融機関支店名;
        private string _営業担当者名;
        private string _獲得営業者名;
        private string _登録担当者名;
        private string _受注得意先名;
        private string _配送先名;
        private string _承認者名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Tokuisaki()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Tokuisaki(Tokuisaki src)
        {
            stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            得意先ＣＤ = src.得意先ＣＤ;
            得意先ＣＤorg = src.得意先ＣＤorg;
            得意先名 = src.得意先名;
            得意先名カナ = src.得意先名カナ;
            得意先名略称 = src.得意先名略称;
            郵便番号 = src.郵便番号;
            住所1 = src.住所1;
            住所2 = src.住所2;
            電話番号1 = src.電話番号1;
            ＦＡＸ1 = src.ＦＡＸ1;
            電話番号2 = src.電話番号2;
            ＦＡＸ2 = src.ＦＡＸ2;
            お届け先郵便番号 = src.お届け先郵便番号;
            お届け先住所1 = src.お届け先住所1;
            お届け先住所2 = src.お届け先住所2;
            お届け先電話番号1 = src.お届け先電話番号1;
            お届け先ＦＡＸ1 = src.お届け先ＦＡＸ1;
            お届け先電話番号2 = src.お届け先電話番号2;
            お届け先ＦＡＸ2 = src.お届け先ＦＡＸ2;
            部署ＣＤ = src.部署ＣＤ;
            売掛現金区分 = src.売掛現金区分;
            電話確認区分 = src.電話確認区分;
            締区分 = src.締区分;
            月締日 = src.月締日;
            週締曜日 = src.週締曜日;
            支払サイト = src.支払サイト;
            支払日 = src.支払日;
            請求先ＣＤ = src.請求先ＣＤ;
            支払方法 = src.支払方法;
            クレカ顧客番号 = src.クレカ顧客番号;
            クレカ番号下4桁 = src.クレカ番号下4桁;
            クレカ有効期限 = src.クレカ有効期限;
            クレカ無効フラグ = src.クレカ無効フラグ;
            クレカ有効性確認日 = src.クレカ有効性確認日;
            振込手数料フラグ = src.振込手数料フラグ;
            部署合算請求フラグ = src.部署合算請求フラグ;
            金融機関ＣＤ = src.金融機関ＣＤ;
            金融機関支店ＣＤ = src.金融機関支店ＣＤ;
            記号番号 = src.記号番号;
            口座種別 = src.口座種別;
            口座番号 = src.口座番号;
            口座名義人 = src.口座名義人;
            チケット枚数 = src.チケット枚数;
            サービスチケット枚数 = src.サービスチケット枚数;
            営業担当者ＣＤ = src.営業担当者ＣＤ;
            獲得営業者ＣＤ = src.獲得営業者ＣＤ;
            登録担当者ＣＤ = src.登録担当者ＣＤ;
            受注得意先ＣＤ = src.受注得意先ＣＤ;
            受注方法 = src.受注方法;
            電話確認時間_時 = src.電話確認時間_時;
            電話確認時間_分 = src.電話確認時間_分;
            味噌汁区分 = src.味噌汁区分;
            ふりかけ区分 = src.ふりかけ区分;
            納品書発行区分 = src.納品書発行区分;
            空き容器回収区分 = src.空き容器回収区分;
            請求書敬称 = src.請求書敬称;
            備考１ = src.備考１;
            備考２ = src.備考２;
            備考３ = src.備考３;
            状態区分 = src.状態区分;
            承認日 = src.承認日;
            承認者ＣＤ = src.承認者ＣＤ;
            状態理由区分 = src.状態理由区分;
            休日設定 = src.休日設定;
            新規登録日 = src.新規登録日;
            税区分 = src.税区分;
            丸め処理区分 = src.丸め処理区分;
            祝日配送区分 = src.祝日配送区分;
            誕生日1 = src.誕生日1;
            誕生日2 = src.誕生日2;
            失客日 = src.失客日;
            EB銀行支店ＣＤ = src.EB銀行支店ＣＤ;
            受注時特記事項 = src.受注時特記事項;
            請求書郵便マークフラグ = src.請求書郵便マークフラグ;
            集計ＣＤ = src.集計ＣＤ;
            配送得意先ＣＤ = src.配送得意先ＣＤ;
            無効フラグ = src.無効フラグ;

            部署名 = src.部署名;
            請求先名 = src.請求先名;
            金融機関名 = src.金融機関名;
            金融機関支店名 = src.金融機関支店名;
            営業担当者名 = src.営業担当者名;
            獲得営業者名 = src.獲得営業者名;
            登録担当者名 = src.登録担当者名;
            受注得意先名 = src.受注得意先名;
            配送先名 = src.配送先名;
            承認者名 = src.承認者名;

            base.Copy(src);

            stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Tokuisaki(DataRow dr)
        {
            stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("得意先ＣＤorg"))
            {
                得意先ＣＤorg = dr["得意先ＣＤorg", version] as int?;
            }
            else
            {
                得意先ＣＤorg = dr["得意先ＣＤ", version] as int?;
            }

            得意先名 = dr["得意先名", version] as string;
            得意先名カナ = dr["得意先名カナ", version] as string;
            得意先名略称 = dr["得意先名略称", version] as string;
            郵便番号 = dr["郵便番号", version] as string;
            住所1 = dr["住所1", version] as string;
            住所2 = dr["住所2", version] as string;
            電話番号1 = dr["電話番号1", version] as string;
            ＦＡＸ1 = dr["ＦＡＸ1", version] as string;
            電話番号2 = dr["電話番号2", version] as string;
            ＦＡＸ2 = dr["ＦＡＸ2", version] as string;
            お届け先郵便番号 = dr["お届け先郵便番号", version] as string;
            お届け先住所1 = dr["お届け先住所1", version] as string;
            お届け先住所2 = dr["お届け先住所2", version] as string;
            お届け先電話番号1 = dr["お届け先電話番号1", version] as string;
            お届け先ＦＡＸ1 = dr["お届け先ＦＡＸ1", version] as string;
            お届け先電話番号2 = dr["お届け先電話番号2", version] as string;
            お届け先ＦＡＸ2 = dr["お届け先ＦＡＸ2", version] as string;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            売掛現金区分 = dr["売掛現金区分", version] as int?;
            電話確認区分 = dr["電話確認区分", version] as int?;
            締区分 = dr["締区分", version] as int?;
            月締日 = dr["月締日", version] as int?;
            週締曜日 = dr["週締曜日", version] as int?;
            支払サイト = dr["支払サイト", version] as int?;
            支払日 = dr["支払日", version] as int?;
            請求先ＣＤ = dr["請求先ＣＤ", version] as int?;
            支払方法 = dr["支払方法", version] as int?;
            クレカ顧客番号 = dr["クレカ顧客番号", version] as string;
            クレカ番号下4桁 = dr["クレカ番号下4桁", version] as string;
            クレカ有効期限 = dr["クレカ有効期限", version] as string;
            クレカ無効フラグ = dr["クレカ無効フラグ"] as bool?;
            クレカ有効性確認日 = dr["クレカ有効性確認日"] as DateTime?;
            振込手数料フラグ = dr["振込手数料フラグ", version] as bool?;
            部署合算請求フラグ = dr["部署合算請求フラグ", version] as bool?;
            金融機関ＣＤ = dr["金融機関ＣＤ", version] as string;
            金融機関支店ＣＤ = dr["金融機関支店ＣＤ", version] as string;
            記号番号 = dr["記号番号", version] as string;
            口座種別 = dr["口座種別", version] as int?;
            口座番号 = dr["口座番号", version] as string;
            口座名義人 = dr["口座名義人", version] as string;
            チケット枚数 = dr["チケット枚数", version] as decimal?;
            サービスチケット枚数 = dr["サービスチケット枚数", version] as decimal?;
            営業担当者ＣＤ = dr["営業担当者ＣＤ", version] as int?;
            獲得営業者ＣＤ = dr["獲得営業者ＣＤ", version] as int?;
            登録担当者ＣＤ = dr["登録担当者ＣＤ", version] as int?;
            受注得意先ＣＤ = dr["受注得意先ＣＤ", version] as int?;
            受注方法 = dr["受注方法", version] as int?;
            電話確認時間_時 = dr["電話確認時間_時", version] as int?;
            電話確認時間_分 = dr["電話確認時間_分", version] as int?;
            if ((dr["味噌汁区分", version] as int?) == 1)
            {
                味噌汁区分 = true;
            }
            else
            {
                味噌汁区分 = false;
            }
            if ((dr["ふりかけ区分", version] as int?) == 1)
            {
                ふりかけ区分 = true;
            }
            else
            {
                ふりかけ区分 = false;
            }

            if ((dr["納品書発行区分", version] as int?) == 1)
            {
                納品書発行区分 = true;
            }
            else
            {
                納品書発行区分 = false;
            }
            空き容器回収区分 = dr["空き容器回収区分", version] as int?;
            請求書敬称 = dr["請求書敬称", version] as int?;
            備考１ = dr["備考１", version] as string;
            備考２ = dr["備考２", version] as string;
            備考３ = dr["備考３", version] as string;
            状態区分 = dr["状態区分", version] as int?;
            承認日 = dr["承認日", version] as DateTime?;
            承認者ＣＤ = dr["承認者ＣＤ", version] as int?;
            状態理由区分 = dr["状態理由区分", version] as int?;
            休日設定 = dr["休日設定", version] as string;
            新規登録日 = dr["新規登録日", version] as DateTime?;
            税区分 = dr["税区分", version] as int?;
            丸め処理区分 = dr["丸め処理区分", version] as int?;
            if ((dr["祝日配送区分", version] as int?) == 1)
            {
                祝日配送区分 = true;
            }
            else
            {
                祝日配送区分 = false;
            }
            誕生日1 = dr["誕生日1", version] as DateTime?;
            誕生日2 = dr["誕生日2", version] as DateTime?;
            失客日 = dr["失客日", version] as DateTime?;
            EB銀行支店ＣＤ = dr["EB銀行支店ＣＤ", version] as string;
            受注時特記事項 = dr["受注時特記事項", version] as string;
            請求書郵便マークフラグ = dr["請求書郵便マークフラグ", version] as bool?;
            集計ＣＤ = dr["集計ＣＤ", version] as int?;
            配送得意先ＣＤ = dr["配送得意先ＣＤ", version] as int?;
            無効フラグ = dr["無効フラグ", version] as bool?;

            部署名 = dr["部署名", version] as string;

            if (dr.Table.Columns.Contains("請求先名"))
            {
                請求先名 = dr["請求先名", version] as string;
            }

            if (dr.Table.Columns.Contains("金融機関名"))
            {
                金融機関名 = dr["金融機関名", version] as string;
            }

            if (dr.Table.Columns.Contains("金融機関支店名"))
            {
                金融機関支店名 = dr["金融機関支店名", version] as string;
            }

            if (dr.Table.Columns.Contains("営業担当者名"))
            {
                営業担当者名 = dr["営業担当者名", version] as string;
            }

            if (dr.Table.Columns.Contains("獲得営業者名"))
            {
                獲得営業者名 = dr["獲得営業者名", version] as string;
            }

            if (dr.Table.Columns.Contains("登録担当者名"))
            {
                登録担当者名 = dr["登録担当者名", version] as string;
            }

            if (dr.Table.Columns.Contains("受注得意先名"))
            {
                受注得意先名 = dr["受注得意先名", version] as string;
            }

            if (dr.Table.Columns.Contains("配送先名"))
            {
                配送先名 = dr["配送先名", version] as string;
            }

            if (dr.Table.Columns.Contains("承認者名"))
            {
                承認者名 = dr["承認者名", version] as string;
            }

            base.FromDataRow(dr, version);

            stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                _errors.Remove("得意先ＣＤ");
                if (value == null)
                {
                    _errors["得意先ＣＤ"] = "得意先ＣＤを入力してください。";
                }
                else if (value < 0)
                {
                    _errors["得意先ＣＤ"] = "得意先ＣＤにマイナスは入力できません。";
                }
            }
        }

        public int? 得意先ＣＤorg
        {
            get { return _得意先ＣＤorg; }
            set { _得意先ＣＤorg = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set
            {
                _得意先名 = value;
                _warns.Remove("得意先名");
                if (value == null)
                {
                    _warns["得意先名"] = "得意先が未入力です。";
                }
            }
        }

        public string 得意先名カナ
        {
            get { return _得意先名カナ; }
            set { _得意先名カナ = value; }
        }

        public string 得意先名略称
        {
            get { return _得意先名略称; }
            set { _得意先名略称 = value; }
        }

        public string 郵便番号
        {
            get { return _郵便番号; }
            set { _郵便番号 = value; }
        }

        public string 住所1
        {
            get { return _住所1; }
            set { _住所1 = value; }
        }

        public string 住所2
        {
            get { return _住所2; }
            set { _住所2 = value; }
        }

        public string 電話番号1
        {
            get { return _電話番号1; }
            set { _電話番号1 = value; }
        }

        public string ＦＡＸ1
        {
            get { return _ＦＡＸ1; }
            set { _ＦＡＸ1 = value; }
        }

        public string 電話番号2
        {
            get { return _電話番号2; }
            set { _電話番号2 = value; }
        }

        public string ＦＡＸ2
        {
            get { return _ＦＡＸ2; }
            set { _ＦＡＸ2 = value; }
        }

        public string お届け先郵便番号
        {
            get { return _お届け先郵便番号; }
            set { _お届け先郵便番号 = value; }
        }

        public string お届け先住所1
        {
            get { return _お届け先住所1; }
            set { _お届け先住所1 = value; }
        }

        public string お届け先住所2
        {
            get { return _お届け先住所2; }
            set { _お届け先住所2 = value; }
        }

        public string お届け先電話番号1
        {
            get { return _お届け先電話番号1; }
            set { _お届け先電話番号1 = value; }
        }

        public string お届け先ＦＡＸ1
        {
            get { return _お届け先ＦＡＸ1; }
            set { _お届け先ＦＡＸ1 = value; }
        }

        public string お届け先電話番号2
        {
            get { return _お届け先電話番号2; }
            set { _お届け先電話番号2 = value; }
        }

        public string お届け先ＦＡＸ2
        {
            get { return _お届け先ＦＡＸ2; }
            set { _お届け先ＦＡＸ2 = value; }
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

        public int? 売掛現金区分
        {
            get { return _売掛現金区分; }
            set
            {
                _売掛現金区分 = value;
                _warns.Remove("売掛現金区分");
                if (value == null || value < 0)
                {
                    _warns["売掛現金区分"] = "売掛現金区分が未選択です。";
                }
            }
        }

        public int? 電話確認区分
        {
            get { return _電話確認区分; }
            set { _電話確認区分 = value; }
        }

        public int? 締区分
        {
            get { return _締区分; }
            set
            {
                _締区分 = value;
                switch (value)
                {
                    case (int)Consts.ShimeKubun.Weekly:     // 週締め
                        // 月締め日（月締日）をクリア
                        月締日 = null;
                        break;

                    case (int)Consts.ShimeKubun.Monthly:    // 月締め
                        // 週締め曜日（週締曜日）をクリア
                        週締曜日 = null;
                        break;

                    default:                                // 上記以外（日締め・未選択）
                        // どちらもクリア
                        月締日 = null;
                        週締曜日 = null;
                        break;
                }
            }
        }

        public int? 月締日
        {
            get { return _月締日; }
            set
            {
                _月締日 = value;
                _errors.Remove("月締日");
                if (value != null)
                {
                    if (1 <= value && value <= 28)
                    {
                    }
                    else if (value == 29 || value == 30 || value == 31)
                    {
                        _errors["月締日"] = "月末の場合は99を入力してください。";
                    }
                    else if (value == 99)
                    {
                    }
                    else
                    {
                        _errors["月締日"] = "1～28 または 月末の場合は99を入力してください。";
                    }
                }
            }
        }

        public int? 週締曜日
        {
            get { return _週締曜日; }
            set { _週締曜日 = value; }
        }

        public int? 支払サイト
        {
            get { return _支払サイト; }
            set { _支払サイト = value; }
        }

        public int? 支払日
        {
            get { return _支払日; }
            set
            {
                _支払日 = value;
                _errors.Remove("支払日");
                if (value != null)
                {
                    if (1 <= value && value <= 28)
                    {
                    }
                    else if (value == 29 || value == 30 || value == 31)
                    {
                        _errors["支払日"] = "月末の場合は99を入力してください。";
                    }
                    else if (value == 99)
                    {
                    }
                    else
                    {
                        _errors["支払日"] = "1～28 または 月末の場合は99を入力してください。";
                    }
                }
            }
        }

        public int? 請求先ＣＤ
        {
            get { return _請求先ＣＤ; }
            set
            {
                _請求先ＣＤ = value;
                _errors.Remove("請求先ＣＤ");
                請求先名 = null;
                if (value != null && stopOverwrite == false)
                {
                    // 請求先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤ);
                    if (tokuisaki != null)
                    {
                        請求先名 = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["請求先ＣＤ"] = "請求先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 支払方法
        {
            get { return _支払方法; }
            set { _支払方法 = value; }
        }

        public string クレカ顧客番号
        {
            get { return _クレカ顧客番号; }
            set { _クレカ顧客番号 = value; }
        }

        public string クレカ番号下4桁
        {
            get { return _クレカ番号下4桁; }
            set { _クレカ番号下4桁 = value; }
        }

        public string クレカ有効期限
        {
            get { return _クレカ有効期限; }
            set { _クレカ有効期限 = value; }
        }

        public bool? クレカ無効フラグ
        {
            get { return _クレカ無効フラグ; }
            set { _クレカ無効フラグ = value; }
        }

        public DateTime? クレカ有効性確認日
        {
            get { return _クレカ有効性確認日; }
            set { _クレカ有効性確認日 = value; }
        }

        public bool? 振込手数料フラグ
        {
            get { return _振込手数料フラグ; }
            set { _振込手数料フラグ = value; }
        }

        public bool? 部署合算請求フラグ
        {
            get { return _部署合算請求フラグ; }
            set { _部署合算請求フラグ = value; }
        }

        public string 金融機関ＣＤ
        {
            get { return _金融機関ＣＤ; }
            set
            {
                _金融機関ＣＤ = value;
                _errors.Remove("金融機関ＣＤ");
                金融機関名 = null;
                if (value != null && stopOverwrite == false)
                {
                    金融機関支店ＣＤ = null;  // 銀行支店名を消す

                    // 金融機関ＣＤに該当する銀行名を取得
                    Bank bank = Bank.GetBank(_金融機関ＣＤ);
                    if (bank != null)
                    {
                        金融機関名 = bank.銀行名;

                        // ゆうちょ以外の場合は記号番号をクリアする
                        if (_金融機関ＣＤ != Consts.YUCHO)
                        {
                            記号番号 = null;
                        }
                    }
                    else
                    {
                        _errors["金融機関ＣＤ"] = "金融機関ＣＤが見つかりません。";
                    }
                }
            }
        }

        public string 金融機関支店ＣＤ
        {
            get { return _金融機関支店ＣＤ; }
            set
            {
                _金融機関支店ＣＤ = value;
                _errors.Remove("金融機関支店ＣＤ");
                金融機関支店名 = null;
                if (value != null && _金融機関ＣＤ != null && stopOverwrite == false)
                {
                    // 金融機関ＣＤと金融機関支店ＣＤに該当する銀行支店名を取得
                    BankBranch bankBranch = BankBranch.GetBankBranch(_金融機関ＣＤ, _金融機関支店ＣＤ);
                    if (bankBranch != null)
                    {
                        金融機関支店名 = bankBranch.支店名;
                    }
                    else
                    {
                        _errors["金融機関支店ＣＤ"] = "金融機関支店ＣＤが見つかりません。";
                    }
                }
            }
        }

        public string 記号番号
        {
            get { return _記号番号; }
            set
            {
                _記号番号 = value;
                _errors.Remove("記号番号");
                if (value != null)
                {
                    if (Regex.IsMatch(value, @"^[0-9-]*$") == true)
                    {
                        Regex regexIppan = new Regex(@"^\d{5,5}-\d{6,6}$");
                        Regex regexCashCard = new Regex(@"^\d{5,5}-\d{8,8}$");
                        if ((value.Length == 12 && value.StartsWith("0") && regexIppan.IsMatch(value)) ||
                            (value.Length == 14 && value.StartsWith("1") && regexCashCard.IsMatch(value)))
                        {
                            string shitenCD = "";
                            string kozaNo = "";
                            if (value.StartsWith("0"))
                            {
                                // 一般
                                shitenCD = value.Substring(1, 2) + "9";
                                kozaNo = value.Substring(value.Length - 6, 6);

                            }
                            else
                            {
                                // キャッシュカード
                                shitenCD = value.Substring(1, 2) + "8";
                                kozaNo = value.Substring(value.Length - 8, 7);
                            }

                            金融機関支店ＣＤ = shitenCD;
                            口座番号 = kozaNo;
                        }
                    }
                    else
                    {
                        _errors["記号番号"] = "記号番号は半角数字かハイフンで入力してください。";
                    }
                }
            }
        }

        public int? 口座種別
        {
            get { return _口座種別; }
            set { _口座種別 = value; }
        }

        public string 口座番号
        {
            get { return _口座番号; }
            set { _口座番号 = value; }
        }

        public string 口座名義人
        {
            get { return _口座名義人; }
            set
            {
                _口座名義人 = value;
                _errors.Remove("口座名義人");
                if (value != null)
                {
                    if (Regex.IsMatch(value, @"^[0-9A-Zｱ-ﾟ\.\(\)\- ]*$") == false)  // 「ｦ」～「ｰ」は使えない
                    {
                        _errors["口座名義人"] = "口座名義人は半角英数か半角カタカナで入力してください。";
                    }
                }
            }
        }

        public decimal? チケット枚数
        {
            get { return _チケット枚数; }
            set { _チケット枚数 = value; }
        }

        public decimal? サービスチケット枚数
        {
            get { return _サービスチケット枚数; }
            set
            {
                _サービスチケット枚数 = value;
                _errors.Remove("サービスチケット枚数");
                if (value != null)
                {
                    if (value < 0)
                    {
                        _errors["サービスチケット枚数"] = "サービスチケット枚数にマイナスは入力できません。";
                        return;
                    }
                    if (1000 <= value)
                    {
                        _errors["サービスチケット枚数"] = "サービスチケット枚数は1000未満で0.5単位で入力してください。";
                        return;
                    }
                    if (value != Decimal.Floor((decimal)value))
                    {
                        if ((double)value % 0.5 != 0d)
                        {
                            _errors["サービスチケット枚数"] = "小数は0.5単位で入力してください。";
                        }
                    }
                }
            }
        }

        public int? 営業担当者ＣＤ
        {
            get { return _営業担当者ＣＤ; }
            set
            {
                _営業担当者ＣＤ = value;
                _errors.Remove("営業担当者ＣＤ");
                営業担当者名 = null;
                if (value != null && stopOverwrite == false)
                {
                    // 営業担当者ＣＤに該当する担当者名を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_営業担当者ＣＤ);
                    if (tantousya != null)
                    {
                        営業担当者名 = tantousya.担当者名;
                    }
                    else
                    {
                        _errors["営業担当者ＣＤ"] = "営業担当者ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 獲得営業者ＣＤ
        {
            get { return _獲得営業者ＣＤ; }
            set
            {
                _獲得営業者ＣＤ = value;
                _errors.Remove("獲得営業者ＣＤ");
                獲得営業者名 = null;
                if (value != null && stopOverwrite == false)
                {
                    // 獲得営業者ＣＤに該当する担当者名を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_獲得営業者ＣＤ);
                    if (tantousya != null)
                    {
                        獲得営業者名 = tantousya.担当者名;
                    }
                    else
                    {
                        _errors["獲得営業者ＣＤ"] = "獲得営業者ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 登録担当者ＣＤ
        {
            get { return _登録担当者ＣＤ; }
            set
            {
                _登録担当者ＣＤ = value;
                _errors.Remove("登録担当者ＣＤ");
                登録担当者名 = null;
                if (value != null && stopOverwrite == false)
                {
                    // 登録担当者ＣＤに該当する担当者名を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_登録担当者ＣＤ);
                    if (tantousya != null)
                    {
                        登録担当者名 = tantousya.担当者名;
                    }
                    else
                    {
                        _errors["登録担当者ＣＤ"] = "登録担当者ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 受注得意先ＣＤ
        {
            get { return _受注得意先ＣＤ; }
            set
            {
                _受注得意先ＣＤ = value;
                _errors.Remove("受注得意先ＣＤ");
                受注得意先名 = null;
                if (value != null && stopOverwrite == false)
                {
                    // 受注得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_受注得意先ＣＤ);
                    if (tokuisaki != null)
                    {
                        受注得意先名 = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["受注得意先ＣＤ"] = "受注得意先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 受注方法
        {
            get { return _受注方法; }
            set { _受注方法 = value; }
        }

        public int? 電話確認時間_時
        {
            get { return _電話確認時間_時; }
            set { _電話確認時間_時 = value; }
        }

        public int? 電話確認時間_分
        {
            get { return _電話確認時間_分; }
            set { _電話確認時間_分 = value; }
        }

        public bool? 味噌汁区分
        {
            get { return _味噌汁区分; }
            set { _味噌汁区分 = value; }
        }

        public bool? ふりかけ区分
        {
            get { return _ふりかけ区分; }
            set { _ふりかけ区分 = value; }
        }

        public bool? 納品書発行区分
        {
            get { return _納品書発行区分; }
            set { _納品書発行区分 = value; }
        }

        public int? 空き容器回収区分
        {
            get { return _空き容器回収区分; }
            set { _空き容器回収区分 = value; }
        }

        public int? 請求書敬称
        {
            get { return _請求書敬称; }
            set { _請求書敬称 = value; }
        }

        public string 備考１
        {
            get { return _備考１; }
            set { _備考１ = value; }
        }

        public string 備考２
        {
            get { return _備考２; }
            set { _備考２ = value; }
        }

        public string 備考３
        {
            get { return _備考３; }
            set { _備考３ = value; }
        }

        public int? 状態区分
        {
            get { return _状態区分; }
            set { _状態区分 = value; }
        }

        public DateTime? 承認日
        {
            get { return _承認日; }
            set { _承認日 = value; }
        }

        public int? 承認者ＣＤ
        {
            get { return _承認者ＣＤ; }
            set
            {
                _承認者ＣＤ = value;
                _errors.Remove("承認者ＣＤ");
                承認者名 = null;
                if (value != null && stopOverwrite == false)
                {
                    // 承認者ＣＤに該当する担当者名を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_承認者ＣＤ);
                    if (tantousya != null)
                    {
                        承認者名 = tantousya.担当者名;
                    }
                    else
                    {
                        _errors["承認者ＣＤ"] = "承認者ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 状態理由区分
        {
            get { return _状態理由区分; }
            set { _状態理由区分 = value; }
        }

        public string 休日設定
        {
            get { return _休日設定; }
            set { _休日設定 = value; }
        }

        public DateTime? 新規登録日
        {
            get { return _新規登録日; }
            set { _新規登録日 = value; }
        }

        public int? 税区分
        {
            get { return _税区分; }
            set
            {
                _税区分 = value;
                _warns.Remove("税区分");
                if (value == null || value < 0)
                {
                    _warns["税区分"] = "税区分が未選択です。";
                }
            }
        }

        public int? 丸め処理区分
        {
            get { return _丸め処理区分; }
            set
            {
                _丸め処理区分 = value;
                _warns.Remove("丸め処理区分");
                if (value == null || value < 0)
                {
                    _warns["丸め処理区分"] = "丸め処理区分が未選択です。";
                }
            }
        }

        public bool? 祝日配送区分
        {
            get { return _祝日配送区分; }
            set { _祝日配送区分 = value; }
        }

        public DateTime? 誕生日1
        {
            get { return _誕生日1; }
            set { _誕生日1 = value; }
        }

        public DateTime? 誕生日2
        {
            get { return _誕生日2; }
            set { _誕生日2 = value; }
        }

        public DateTime? 失客日
        {
            get { return _失客日; }
            set { _失客日 = value; }
        }

        public string EB銀行支店ＣＤ
        {
            get { return _EB銀行支店ＣＤ; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                _EB銀行支店ＣＤ = value;
            }
        }

        public string 受注時特記事項
        {
            get { return _受注時特記事項; }
            set { _受注時特記事項 = value; }
        }

        public bool? 請求書郵便マークフラグ
        {
            get { return _請求書郵便マークフラグ; }
            set { _請求書郵便マークフラグ = value; }
        }

        public int? 集計ＣＤ
        {
            get { return _集計ＣＤ; }
            set { _集計ＣＤ = value; }
        }

        public int? 配送得意先ＣＤ
        {
            get { return _配送得意先ＣＤ; }
            set
            {
                _配送得意先ＣＤ = value;
                _errors.Remove("配送得意先ＣＤ");
                配送先名 = null;
                if (value != null && stopOverwrite == false)
                {
                    // 配送得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_配送得意先ＣＤ);
                    if (tokuisaki != null)
                    {
                        配送先名 = tokuisaki.得意先名;
                    }
                    else
                    {
                        // 自分自身を指定可能にしているが、新規の場合は自分自身もDBに存在しないので、自分自身が入力された場合はエラーとはみなさない(★)
                        if (value != _得意先ＣＤ)
                        {
                            _errors["配送得意先ＣＤ"] = "配送得意先ＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public bool? 休日設定日曜
        {
            get
            {
                return GetHoliday(DayOfWeek.Sunday);
            }
            set
            {
                SetHoliday(DayOfWeek.Sunday, value);
            }
        }

        public bool? 休日設定月曜
        {
            get
            {
                return GetHoliday(DayOfWeek.Monday);
            }
            set
            {
                SetHoliday(DayOfWeek.Monday, value);
            }
        }

        public bool? 休日設定火曜
        {
            get
            {
                return GetHoliday(DayOfWeek.Tuesday);
            }
            set
            {
                SetHoliday(DayOfWeek.Tuesday, value);
            }
        }

        public bool? 休日設定水曜
        {
            get
            {
                return GetHoliday(DayOfWeek.Wednesday);
            }
            set
            {
                SetHoliday(DayOfWeek.Wednesday, value);
            }
        }

        public bool? 休日設定木曜
        {
            get
            {
                return GetHoliday(DayOfWeek.Thursday);
            }
            set
            {
                SetHoliday(DayOfWeek.Thursday, value);
            }
        }

        public bool? 休日設定金曜
        {
            get
            {
                return GetHoliday(DayOfWeek.Friday);
            }
            set
            {
                SetHoliday(DayOfWeek.Friday, value);
            }
        }

        public bool? 休日設定土曜
        {
            get
            {
                return GetHoliday(DayOfWeek.Saturday);
            }
            set
            {
                SetHoliday(DayOfWeek.Saturday, value);
            }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
        }

        public string 金融機関名
        {
            get { return _金融機関名; }
            set { _金融機関名 = value; }
        }

        public string 金融機関支店名
        {
            get { return _金融機関支店名; }
            set { _金融機関支店名 = value; }
        }

        public string 営業担当者名
        {
            get { return _営業担当者名; }
            set { _営業担当者名 = value; }
        }

        public string 獲得営業者名
        {
            get { return _獲得営業者名; }
            set { _獲得営業者名 = value; }
        }

        public string 登録担当者名
        {
            get { return _登録担当者名; }
            set { _登録担当者名 = value; }
        }

        public string 受注得意先名
        {
            get { return _受注得意先名; }
            set { _受注得意先名 = value; }
        }

        public string 配送先名
        {
            get { return _配送先名; }
            set { _配送先名 = value; }
        }

        public string 承認者名
        {
            get { return _承認者名; }
            set { _承認者名 = value; }
        }

        public override string Code
        {
            get { return string.Format("得意先ＣＤ=[{0}]", (得意先ＣＤ == 得意先ＣＤorg ? 得意先ＣＤ : 得意先ＣＤorg ?? 得意先ＣＤ)); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get
            {
                // 状態が承認の場合は承認日と承認者を、
                //       失客の場合は状態理由と失客日をそれぞれ必須にする
                switch (_状態区分)
                {
                    case (int)Consts.JotaiKubun.Syonin:
                        if (承認日 == null || 承認者ＣＤ == null)
                        {
                            return "状態が承認の場合は、承認日と承認者を入力してください。";
                        }
                        break;

                    case (int)Consts.JotaiKubun.Shikkyaku:
                        if ((状態理由区分 == null || 状態理由区分 < 0) || 失客日 == null)
                        {
                            return "状態が失客の場合は、状態理由と失客日を入力してください。";
                        }
                        break;
                }

                // 売掛現金区分が売掛の場合は以下を必須にする
                // ・締区分
                // ・月締日／週締め曜日
                if (売掛現金区分 == (int)Consts.UriGenKubun.Urikake)
                {
                    if (締区分 == null || 締区分 < 0)
                    {
                        return "売掛現金区分が売掛です。締区分を入力してください。";
                    }
                    else
                    {
                        switch (締区分)
                        {
                            case (int)Consts.ShimeKubun.Weekly:     // 週締め
                                if (週締曜日 == null || 週締曜日 < 0)
                                {
                                    return "売掛現金区分が売掛（週締）です。週締曜日を入力してください。";
                                }
                                break;

                            case (int)Consts.ShimeKubun.Monthly:    // 月締め
                                if (月締日 == null)
                                {
                                    return "売掛現金区分が売掛（月締）です。月締め日を入力してください。";
                                }
                                break;
                        }
                    }
                }

                // 支払方法が振替の場合は、以下の項目を必須にする
                // ・月締日
                // ・EB銀行支店ＣＤ
                // ・金融機関ＣＤ
                // ・金融機関支店ＣＤ
                // ・口座種別
                // ・口座番号
                // ・口座名義人
                if (支払方法 == (int)Consts.ShiharaiHohoKubun.Furikae)
                {
                    if (月締日 == null ||
                        EB銀行支店ＣＤ == null ||
                        金融機関ＣＤ == null ||
                        金融機関支店ＣＤ == null ||
                        口座種別 == null ||
                        口座番号 == null ||
                        口座名義人 == null)
                    {
                        return "支払方法が振替の場合は、以下の項目を入力してください。\n月締日\n振替用EB口座\n金融機関ＣＤ\n金融機関支店ＣＤ\n口座種別\n口座番号\n口座名義人";
                    }
                }

                // 配送得意先ＣＤに自分自身を設定してはダメ
                // …としていたが、モバイルのSQLで「ISNULL(配送得意先ＣＤ, 得意先ＣＤ) AS 配送得意先ＣＤ」とやることでインデックスが効かないことでタイムアウトが頻発
                // →配送得意先を指定しない場合はNULLの代わりに自分の得意先ＣＤを設定してもらうことになった（モバイルのSQLも配送得意先ＣＤだけを参照するように改修済み）(2022/06/30)
                // if (_配送得意先ＣＤ != null)
                // {
                //     if (_配送得意先ＣＤ == _得意先ＣＤ)
                //     {
                //         return "配送先CDに自分自身を指定することはできません。";
                //     }
                // }
                if (_配送得意先ＣＤ == null)
                {
                    // 新規で配送得意先ＣＤが未入力の場合は登録時に自分自身を設定する(★)
                    if (修正日 != null)
                    {
                        return "配送得意先ＣＤを入力してください。\n※不明な場合は自分自身の得意先ＣＤを入力してください。";
                    }
                }
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                return (_errors.ContainsKey(columnName) ? _errors[columnName] : (_warns.ContainsKey(columnName) ? _warns[columnName] : null));
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 休日設定の該当の曜日の値を返す
        /// </summary>
        /// <param name="dayOfWeek">休日設定の曜日インデックス</param>
        /// <returns>休日の場合はtrue。それ以外はfalse。</returns>
        public bool GetHoliday(DayOfWeek dayOfWeek)
        {
            return GetHoliday(休日設定, dayOfWeek);
        }

        /// <summary>
        /// 休日設定の該当の曜日の値を設定する
        /// </summary>
        /// <param name="dayOfWeek">休日設定の曜日インデックス</param>
        /// <param name="value">休日の場合はtrue。</param>
        private void SetHoliday(DayOfWeek dayOfWeek, bool? value)
        {
            if (string.IsNullOrEmpty(休日設定))
            {
                休日設定 = ALL_OFF;
            }
            char[] c = 休日設定.ToCharArray();
            int index = (int)dayOfWeek;         // 日:0～土:6
            if (value == true)
            {
                c[index] = '1';
            }
            else
            {
                c[index] = '0';
            }
            休日設定 = string.Join("", c);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(Tokuisaki target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                得意先名 == target.得意先名 &&
                得意先名カナ == target.得意先名カナ &&
                得意先名略称 == target.得意先名略称 &&
                郵便番号 == target.郵便番号 &&
                住所1 == target.住所1 &&
                住所2 == target.住所2 &&
                電話番号1 == target.電話番号1 &&
                ＦＡＸ1 == target.ＦＡＸ1 &&
                電話番号2 == target.電話番号2 &&
                ＦＡＸ2 == target.ＦＡＸ2 &&
                お届け先郵便番号 == target.お届け先郵便番号 &&
                お届け先住所1 == target.お届け先住所1 &&
                お届け先住所2 == target.お届け先住所2 &&
                お届け先電話番号1 == target.お届け先電話番号1 &&
                お届け先ＦＡＸ1 == target.お届け先ＦＡＸ1 &&
                お届け先電話番号2 == target.お届け先電話番号2 &&
                お届け先ＦＡＸ2 == target.お届け先ＦＡＸ2 &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                (売掛現金区分 ?? -1) == (target.売掛現金区分 ?? -1) &&
                (電話確認区分 ?? -1) == (target.電話確認区分 ?? -1) &&
                (締区分 ?? -1) == (target.締区分 ?? -1) &&
                月締日 == target.月締日 &&
                (週締曜日 ?? -1) == (target.週締曜日 ?? -1) &&
                (支払サイト ?? -1) == (target.支払サイト ?? -1) &&
                支払日 == target.支払日 &&
                請求先ＣＤ == target.請求先ＣＤ &&
                (支払方法 ?? -1) == (target.支払方法 ?? -1) &&
                クレカ顧客番号 == target.クレカ顧客番号 &&
                クレカ番号下4桁 == target.クレカ番号下4桁 &&
                クレカ有効期限 == target.クレカ有効期限 &&
                (クレカ無効フラグ ?? false) == (target.クレカ無効フラグ ?? false) &&
                クレカ有効性確認日 == target.クレカ有効性確認日 &&
                (振込手数料フラグ ?? false) == (target.振込手数料フラグ ?? false) &&
                (部署合算請求フラグ ?? false) == (target.部署合算請求フラグ ?? false) &&
                金融機関ＣＤ == target.金融機関ＣＤ &&
                金融機関支店ＣＤ == target.金融機関支店ＣＤ &&
                記号番号 == target.記号番号 &&
                (口座種別 ?? -1) == (target.口座種別 ?? -1) &&
                口座番号 == target.口座番号 &&
                口座名義人 == target.口座名義人 &&
                チケット枚数 == target.チケット枚数 &&
                サービスチケット枚数 == target.サービスチケット枚数 &&
                (営業担当者ＣＤ ?? -1) == (target.営業担当者ＣＤ ?? -1) &&
                (獲得営業者ＣＤ ?? -1) == (target.獲得営業者ＣＤ ?? -1) &&
                (登録担当者ＣＤ ?? -1) == (target.登録担当者ＣＤ ?? -1) &&
                (受注得意先ＣＤ ?? -1) == (target.受注得意先ＣＤ ?? -1) &&
                (受注方法 ?? -1) == (target.受注方法 ?? -1) &&
                電話確認時間_時 == target.電話確認時間_時 &&
                電話確認時間_分 == target.電話確認時間_分 &&
                (味噌汁区分 ?? false) == (target.味噌汁区分 ?? false) &&
                (ふりかけ区分 ?? false) == (target.ふりかけ区分 ?? false) &&
                (納品書発行区分 ?? false) == (target.納品書発行区分 ?? false) &&
                (空き容器回収区分 ?? -1) == (target.空き容器回収区分 ?? -1) &&
                (請求書敬称 ?? -1) == (target.請求書敬称 ?? -1) &&
                備考１ == target.備考１ &&
                備考２ == target.備考２ &&
                備考３ == target.備考３ &&
                (状態区分 ?? -1) == (target.状態区分 ?? -1) &&
                承認日 == target.承認日 &&
                (承認者ＣＤ ?? -1) == (target.承認者ＣＤ ?? -1) &&
                (状態理由区分 ?? -1) == (target.状態理由区分 ?? -1) &&
                休日設定 == target.休日設定 &&
                新規登録日 == target.新規登録日 &&
                (税区分 ?? -1) == (target.税区分 ?? -1) &&
                (丸め処理区分 ?? -1) == (target.丸め処理区分 ?? -1) &&
                (祝日配送区分 ?? false) == (target.祝日配送区分 ?? false) &&
                誕生日1 == target.誕生日1 &&
                誕生日2 == target.誕生日2 &&
                失客日 == target.失客日 &&
                EB銀行支店ＣＤ == target.EB銀行支店ＣＤ &&
                受注時特記事項 == target.受注時特記事項 &&
                (請求書郵便マークフラグ ?? false) == (target.請求書郵便マークフラグ ?? false) &&
                (集計ＣＤ ?? -1) == (target.集計ＣＤ ?? -1) &&
                (配送得意先ＣＤ ?? -1) == (target.配送得意先ＣＤ ?? -1) &&
                (無効フラグ ?? false) == (target.無効フラグ ?? false))
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
            string sql = "\r\n INSERT INTO 得意先マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,得意先名" +
                         "\r\n     ,得意先名カナ" +
                         "\r\n     ,得意先名略称" +
                         "\r\n     ,郵便番号" +
                         "\r\n     ,住所1" +
                         "\r\n     ,住所2" +
                         "\r\n     ,電話番号1" +
                         "\r\n     ,ＦＡＸ1" +
                         "\r\n     ,電話番号2" +
                         "\r\n     ,ＦＡＸ2" +
                         "\r\n     ,お届け先郵便番号" +
                         "\r\n     ,お届け先住所1" +
                         "\r\n     ,お届け先住所2" +
                         "\r\n     ,お届け先電話番号1" +
                         "\r\n     ,お届け先ＦＡＸ1" +
                         "\r\n     ,お届け先電話番号2" +
                         "\r\n     ,お届け先ＦＡＸ2" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,売掛現金区分" +
                         "\r\n     ,電話確認区分" +
                         "\r\n     ,締区分" +
                         "\r\n     ,月締日" +
                         "\r\n     ,週締曜日" +
                         "\r\n     ,支払サイト" +
                         "\r\n     ,支払日" +
                         "\r\n     ,請求先ＣＤ" +
                         "\r\n     ,支払方法" +
                         "\r\n     ,クレカ顧客番号" +
                         "\r\n     ,クレカ番号下4桁" +
                         "\r\n     ,クレカ有効期限" +
                         "\r\n     ,クレカ無効フラグ" +
                         "\r\n     ,クレカ有効性確認日" +
                         "\r\n     ,振込手数料フラグ" +
                         "\r\n     ,部署合算請求フラグ" +
                         "\r\n     ,金融機関ＣＤ" +
                         "\r\n     ,金融機関支店ＣＤ" +
                         "\r\n     ,記号番号" +
                         "\r\n     ,口座種別" +
                         "\r\n     ,口座番号" +
                         "\r\n     ,口座名義人" +
                         "\r\n     ,チケット枚数" +
                         "\r\n     ,サービスチケット枚数" +
                         "\r\n     ,営業担当者ＣＤ" +
                         "\r\n     ,獲得営業者ＣＤ" +
                         "\r\n     ,登録担当者ＣＤ" +
                         "\r\n     ,受注得意先ＣＤ" +
                         "\r\n     ,受注方法" +
                         "\r\n     ,電話確認時間_時" +
                         "\r\n     ,電話確認時間_分" +
                         "\r\n     ,味噌汁区分" +
                         "\r\n     ,ふりかけ区分" +
                         "\r\n     ,納品書発行区分" +
                         "\r\n     ,空き容器回収区分" +
                         "\r\n     ,請求書敬称" +
                         "\r\n     ,備考１" +
                         "\r\n     ,備考２" +
                         "\r\n     ,備考３" +
                         "\r\n     ,状態区分" +
                         "\r\n     ,承認日" +
                         "\r\n     ,承認者ＣＤ" +
                         "\r\n     ,状態理由区分" +
                         "\r\n     ,休日設定" +
                         "\r\n     ,新規登録日" +
                         "\r\n     ,税区分" +
                         "\r\n     ,丸め処理区分" +
                         "\r\n     ,祝日配送区分" +
                         "\r\n     ,誕生日1" +
                         "\r\n     ,誕生日2" +
                         "\r\n     ,失客日" +
                         "\r\n     ,EB銀行支店ＣＤ" +
                         "\r\n     ,受注時特記事項" +
                         "\r\n     ,請求書郵便マークフラグ" +
                         "\r\n     ,集計ＣＤ" +
                         "\r\n     ,配送得意先ＣＤ" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@得意先名" +
                         "\r\n     ,@得意先名カナ" +
                         "\r\n     ,@得意先名略称" +
                         "\r\n     ,@郵便番号" +
                         "\r\n     ,@住所1" +
                         "\r\n     ,@住所2" +
                         "\r\n     ,@電話番号1" +
                         "\r\n     ,@ＦＡＸ1" +
                         "\r\n     ,@電話番号2" +
                         "\r\n     ,@ＦＡＸ2" +
                         "\r\n     ,@お届け先郵便番号" +
                         "\r\n     ,@お届け先住所1" +
                         "\r\n     ,@お届け先住所2" +
                         "\r\n     ,@お届け先電話番号1" +
                         "\r\n     ,@お届け先ＦＡＸ1" +
                         "\r\n     ,@お届け先電話番号2" +
                         "\r\n     ,@お届け先ＦＡＸ2" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@売掛現金区分" +
                         "\r\n     ,@電話確認区分" +
                         "\r\n     ,@締区分" +
                         "\r\n     ,@月締日" +
                         "\r\n     ,@週締曜日" +
                         "\r\n     ,@支払サイト" +
                         "\r\n     ,@支払日" +
                         "\r\n     ,@請求先ＣＤ" +
                         "\r\n     ,@支払方法" +
                         "\r\n     ,@クレカ顧客番号" +
                         "\r\n     ,@クレカ番号下4桁" +
                         "\r\n     ,@クレカ有効期限" +
                         "\r\n     ,@クレカ無効フラグ" +
                         "\r\n     ,@クレカ有効性確認日" +
                         "\r\n     ,@振込手数料フラグ" +
                         "\r\n     ,@部署合算請求フラグ" +
                         "\r\n     ,@金融機関ＣＤ" +
                         "\r\n     ,@金融機関支店ＣＤ" +
                         "\r\n     ,@記号番号" +
                         "\r\n     ,@口座種別" +
                         "\r\n     ,@口座番号" +
                         "\r\n     ,@口座名義人" +
                         "\r\n     ,@チケット枚数" +
                         "\r\n     ,@サービスチケット枚数" +
                         "\r\n     ,@営業担当者ＣＤ" +
                         "\r\n     ,@獲得営業者ＣＤ" +
                         "\r\n     ,@登録担当者ＣＤ" +
                         "\r\n     ,@受注得意先ＣＤ" +
                         "\r\n     ,@受注方法" +
                         "\r\n     ,@電話確認時間_時" +
                         "\r\n     ,@電話確認時間_分" +
                         "\r\n     ,@味噌汁区分" +
                         "\r\n     ,@ふりかけ区分" +
                         "\r\n     ,@納品書発行区分" +
                         "\r\n     ,@空き容器回収区分" +
                         "\r\n     ,@請求書敬称" +
                         "\r\n     ,@備考１" +
                         "\r\n     ,@備考２" +
                         "\r\n     ,@備考３" +
                         "\r\n     ,@状態区分" +
                         "\r\n     ,@承認日" +
                         "\r\n     ,@承認者ＣＤ" +
                         "\r\n     ,@状態理由区分" +
                         "\r\n     ,@休日設定" +
                         "\r\n     ,@新規登録日" +
                         "\r\n     ,@税区分" +
                         "\r\n     ,@丸め処理区分" +
                         "\r\n     ,@祝日配送区分" +
                         "\r\n     ,@誕生日1" +
                         "\r\n     ,@誕生日2" +
                         "\r\n     ,@失客日" +
                         "\r\n     ,@EB銀行支店ＣＤ" +
                         "\r\n     ,@受注時特記事項" +
                         "\r\n     ,@請求書郵便マークフラグ" +
                         "\r\n     ,@集計ＣＤ" +
                         "\r\n     ,@配送得意先ＣＤ" +
                         "\r\n     ,@無効フラグ" +
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
            string sql = "\r\n UPDATE 得意先マスタ" +
                         "\r\n SET  得意先名               = @得意先名" +
                         "\r\n     ,得意先名カナ           = @得意先名カナ" +
                         "\r\n     ,得意先名略称           = @得意先名略称" +
                         "\r\n     ,郵便番号               = @郵便番号" +
                         "\r\n     ,住所1                  = @住所1" +
                         "\r\n     ,住所2                  = @住所2" +
                         "\r\n     ,電話番号1              = @電話番号1" +
                         "\r\n     ,ＦＡＸ1                = @ＦＡＸ1" +
                         "\r\n     ,電話番号2              = @電話番号2" +
                         "\r\n     ,ＦＡＸ2                = @ＦＡＸ2" +
                         "\r\n     ,お届け先郵便番号       = @お届け先郵便番号" +
                         "\r\n     ,お届け先住所1          = @お届け先住所1" +
                         "\r\n     ,お届け先住所2          = @お届け先住所2" +
                         "\r\n     ,お届け先電話番号1      = @お届け先電話番号1" +
                         "\r\n     ,お届け先ＦＡＸ1        = @お届け先ＦＡＸ1" +
                         "\r\n     ,お届け先電話番号2      = @お届け先電話番号2" +
                         "\r\n     ,お届け先ＦＡＸ2        = @お届け先ＦＡＸ2" +
                         "\r\n     ,部署ＣＤ               = @部署ＣＤ" +
                         "\r\n     ,売掛現金区分           = @売掛現金区分" +
                         "\r\n     ,電話確認区分           = @電話確認区分" +
                         "\r\n     ,締区分                 = @締区分" +
                         "\r\n     ,月締日                 = @月締日" +
                         "\r\n     ,週締曜日               = @週締曜日" +
                         "\r\n     ,支払サイト             = @支払サイト" +
                         "\r\n     ,支払日                 = @支払日" +
                         "\r\n     ,請求先ＣＤ             = @請求先ＣＤ" +
                         "\r\n     ,支払方法               = @支払方法" +
                         "\r\n     ,クレカ顧客番号         = @クレカ顧客番号" +
                         "\r\n     ,クレカ番号下4桁         = @クレカ番号下4桁" +
                         "\r\n     ,クレカ有効期限         = @クレカ有効期限" +
                         "\r\n     ,クレカ無効フラグ         = @クレカ無効フラグ" +
                         "\r\n     ,クレカ有効性確認日         = @クレカ有効性確認日" +
                         "\r\n     ,振込手数料フラグ       = @振込手数料フラグ" +
                         "\r\n     ,部署合算請求フラグ     = @部署合算請求フラグ" +
                         "\r\n     ,金融機関ＣＤ           = @金融機関ＣＤ" +
                         "\r\n     ,金融機関支店ＣＤ       = @金融機関支店ＣＤ" +
                         "\r\n     ,記号番号               = @記号番号" +
                         "\r\n     ,口座種別               = @口座種別" +
                         "\r\n     ,口座番号               = @口座番号" +
                         "\r\n     ,口座名義人             = @口座名義人" +
                         "\r\n     ,チケット枚数           = @チケット枚数" +
                         "\r\n     ,サービスチケット枚数   = @サービスチケット枚数" +
                         "\r\n     ,営業担当者ＣＤ         = @営業担当者ＣＤ" +
                         "\r\n     ,獲得営業者ＣＤ         = @獲得営業者ＣＤ" +
                         "\r\n     ,登録担当者ＣＤ         = @登録担当者ＣＤ" +
                         "\r\n     ,受注得意先ＣＤ         = @受注得意先ＣＤ" +
                         "\r\n     ,受注方法               = @受注方法" +
                         "\r\n     ,電話確認時間_時        = @電話確認時間_時" +
                         "\r\n     ,電話確認時間_分        = @電話確認時間_分" +
                         "\r\n     ,味噌汁区分             = @味噌汁区分" +
                         "\r\n     ,ふりかけ区分           = @ふりかけ区分" +
                         "\r\n     ,納品書発行区分         = @納品書発行区分" +
                         "\r\n     ,空き容器回収区分       = @空き容器回収区分" +
                         "\r\n     ,請求書敬称             = @請求書敬称" +
                         "\r\n     ,備考１                 = @備考１" +
                         "\r\n     ,備考２                 = @備考２" +
                         "\r\n     ,備考３                 = @備考３" +
                         "\r\n     ,状態区分               = @状態区分" +
                         "\r\n     ,承認日                 = @承認日" +
                         "\r\n     ,承認者ＣＤ             = @承認者ＣＤ" +
                         "\r\n     ,状態理由区分           = @状態理由区分" +
                         "\r\n     ,休日設定               = @休日設定" +
                         "\r\n     ,新規登録日             = @新規登録日" +
                         "\r\n     ,税区分                 = @税区分" +
                         "\r\n     ,丸め処理区分           = @丸め処理区分" +
                         "\r\n     ,祝日配送区分           = @祝日配送区分" +
                         "\r\n     ,誕生日1                = @誕生日1" +
                         "\r\n     ,誕生日2                = @誕生日2" +
                         "\r\n     ,失客日                 = @失客日" +
                         "\r\n     ,EB銀行支店ＣＤ         = @EB銀行支店ＣＤ" +
                         "\r\n     ,受注時特記事項         = @受注時特記事項" +
                         "\r\n     ,請求書郵便マークフラグ = @請求書郵便マークフラグ" +
                         "\r\n     ,集計ＣＤ               = @集計ＣＤ" +
                         "\r\n     ,配送得意先ＣＤ         = @配送得意先ＣＤ" +
                         "\r\n     ,無効フラグ             = @無効フラグ" +
                         "\r\n     ,修正担当者ＣＤ         = @修正担当者ＣＤ" +
                         "\r\n     ,修正日                 = getdate()" +
                         "\r\n WHERE 企業ＣＤ       = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ     = @得意先ＣＤorg" +
                         "\r\n   AND 修正日         = @修正日";

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
            string sql = "\r\n DELETE 得意先マスタ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤorg" +
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

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤorg", SqlDbType.Int).Value = 得意先ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先名 != null)
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = 得意先名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (得意先名カナ != null)
                    {
                        cmd.Parameters.Add("@得意先名カナ", SqlDbType.VarChar).Value = 得意先名カナ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名カナ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (得意先名略称 != null)
                    {
                        cmd.Parameters.Add("@得意先名略称", SqlDbType.VarChar).Value = 得意先名略称;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名略称", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (郵便番号 != null)
                    {
                        cmd.Parameters.Add("@郵便番号", SqlDbType.VarChar).Value = 郵便番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@郵便番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (住所1 != null)
                    {
                        cmd.Parameters.Add("@住所1", SqlDbType.VarChar).Value = 住所1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@住所1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (住所2 != null)
                    {
                        cmd.Parameters.Add("@住所2", SqlDbType.VarChar).Value = 住所2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@住所2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電話番号1 != null)
                    {
                        cmd.Parameters.Add("@電話番号1", SqlDbType.VarChar).Value = 電話番号1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (ＦＡＸ1 != null)
                    {
                        cmd.Parameters.Add("@ＦＡＸ1", SqlDbType.VarChar).Value = ＦＡＸ1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ＦＡＸ1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電話番号2 != null)
                    {
                        cmd.Parameters.Add("@電話番号2", SqlDbType.VarChar).Value = 電話番号2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (ＦＡＸ2 != null)
                    {
                        cmd.Parameters.Add("@ＦＡＸ2", SqlDbType.VarChar).Value = ＦＡＸ2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ＦＡＸ2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (お届け先郵便番号 != null)
                    {
                        cmd.Parameters.Add("@お届け先郵便番号", SqlDbType.VarChar).Value = お届け先郵便番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@お届け先郵便番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (お届け先住所1 != null)
                    {
                        cmd.Parameters.Add("@お届け先住所1", SqlDbType.VarChar).Value = お届け先住所1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@お届け先住所1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (お届け先住所2 != null)
                    {
                        cmd.Parameters.Add("@お届け先住所2", SqlDbType.VarChar).Value = お届け先住所2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@お届け先住所2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (お届け先電話番号1 != null)
                    {
                        cmd.Parameters.Add("@お届け先電話番号1", SqlDbType.VarChar).Value = お届け先電話番号1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@お届け先電話番号1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (お届け先ＦＡＸ1 != null)
                    {
                        cmd.Parameters.Add("@お届け先ＦＡＸ1", SqlDbType.VarChar).Value = お届け先ＦＡＸ1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@お届け先ＦＡＸ1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (お届け先電話番号2 != null)
                    {
                        cmd.Parameters.Add("@お届け先電話番号2", SqlDbType.VarChar).Value = お届け先電話番号2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@お届け先電話番号2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (お届け先ＦＡＸ2 != null)
                    {
                        cmd.Parameters.Add("@お届け先ＦＡＸ2", SqlDbType.VarChar).Value = お届け先ＦＡＸ2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@お届け先ＦＡＸ2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null && 部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (売掛現金区分 != null && 売掛現金区分 != -1)
                    {
                        cmd.Parameters.Add("@売掛現金区分", SqlDbType.Int).Value = 売掛現金区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売掛現金区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (電話確認区分 != null && 電話確認区分 != -1)
                    {
                        cmd.Parameters.Add("@電話確認区分", SqlDbType.Int).Value = 電話確認区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話確認区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (締区分 != null && 締区分 != -1)
                    {
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = 締区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (月締日 != null)
                    {
                        cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = 月締日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (週締曜日 != null && 週締曜日 != -1)
                    {
                        cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = 週締曜日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (支払サイト != null && 支払サイト != -1)
                    {
                        cmd.Parameters.Add("@支払サイト", SqlDbType.Int).Value = 支払サイト;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支払サイト", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (支払日 != null)
                    {
                        cmd.Parameters.Add("@支払日", SqlDbType.Int).Value = 支払日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支払日", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (請求先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = 請求先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (支払方法 != null && 支払方法 != -1)
                    {
                        cmd.Parameters.Add("@支払方法", SqlDbType.Int).Value = 支払方法;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支払方法", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (クレカ顧客番号 != null)
                    {
                        cmd.Parameters.Add("@クレカ顧客番号", SqlDbType.VarChar).Value = クレカ顧客番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレカ顧客番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (クレカ番号下4桁 != null)
                    {
                        cmd.Parameters.Add("@クレカ番号下4桁", SqlDbType.VarChar).Value = クレカ番号下4桁;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレカ番号下4桁", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (クレカ有効期限 != null)
                    {
                        cmd.Parameters.Add("@クレカ有効期限", SqlDbType.VarChar).Value = クレカ有効期限;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレカ有効期限", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (クレカ無効フラグ != null)
                    {
                        cmd.Parameters.Add("@クレカ無効フラグ", SqlDbType.Bit).Value = クレカ無効フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレカ無効フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (クレカ有効性確認日 != null)
                    {
                        cmd.Parameters.Add("@クレカ有効性確認日", SqlDbType.Date).Value = クレカ有効性確認日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレカ有効性確認日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (振込手数料フラグ != null)
                    {
                        cmd.Parameters.Add("@振込手数料フラグ", SqlDbType.Bit).Value = 振込手数料フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@振込手数料フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (部署合算請求フラグ != null)
                    {
                        cmd.Parameters.Add("@部署合算請求フラグ", SqlDbType.Bit).Value = 部署合算請求フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署合算請求フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (金融機関ＣＤ != null)
                    {
                        cmd.Parameters.Add("@金融機関ＣＤ", SqlDbType.VarChar).Value = 金融機関ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金融機関ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (金融機関支店ＣＤ != null)
                    {
                        cmd.Parameters.Add("@金融機関支店ＣＤ", SqlDbType.VarChar).Value = 金融機関支店ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金融機関支店ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (記号番号 != null)
                    {
                        cmd.Parameters.Add("@記号番号", SqlDbType.VarChar).Value = 記号番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@記号番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (口座種別 != null && 口座種別 != -1)
                    {
                        cmd.Parameters.Add("@口座種別", SqlDbType.Int).Value = 口座種別;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座種別", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (口座番号 != null)
                    {
                        cmd.Parameters.Add("@口座番号", SqlDbType.VarChar).Value = 口座番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (口座名義人 != null)
                    {
                        cmd.Parameters.Add("@口座名義人", SqlDbType.VarChar).Value = 口座名義人;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座名義人", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (チケット枚数 != null)
                    {
                        cmd.Parameters.Add("@チケット枚数", SqlDbType.Decimal).Value = チケット枚数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@チケット枚数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (サービスチケット枚数 != null)
                    {
                        cmd.Parameters.Add("@サービスチケット枚数", SqlDbType.Decimal).Value = サービスチケット枚数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@サービスチケット枚数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (営業担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@営業担当者ＣＤ", SqlDbType.Int).Value = 営業担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@営業担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (獲得営業者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@獲得営業者ＣＤ", SqlDbType.Int).Value = 獲得営業者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@獲得営業者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (登録担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@登録担当者ＣＤ", SqlDbType.Int).Value = 登録担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@登録担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (受注得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@受注得意先ＣＤ", SqlDbType.Int).Value = 受注得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@受注得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (受注方法 != null && 受注方法 != -1)
                    {
                        cmd.Parameters.Add("@受注方法", SqlDbType.Int).Value = 受注方法;
                    }
                    else
                    {
                        cmd.Parameters.Add("@受注方法", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (電話確認時間_時 != null)
                    {
                        cmd.Parameters.Add("@電話確認時間_時", SqlDbType.Int).Value = 電話確認時間_時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話確認時間_時", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (電話確認時間_分 != null)
                    {
                        cmd.Parameters.Add("@電話確認時間_分", SqlDbType.Int).Value = 電話確認時間_分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話確認時間_分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (味噌汁区分 != null && 味噌汁区分 == true)
                    {
                        cmd.Parameters.Add("@味噌汁区分", SqlDbType.Int).Value = 1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@味噌汁区分", SqlDbType.Int).Value = 0;
                    }

                    if (ふりかけ区分 != null && ふりかけ区分 == true)
                    {
                        cmd.Parameters.Add("@ふりかけ区分", SqlDbType.Int).Value = 1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ふりかけ区分", SqlDbType.Int).Value = 0;
                    }

                    if (納品書発行区分 != null && 納品書発行区分 == true)
                    {
                        cmd.Parameters.Add("@納品書発行区分", SqlDbType.Int).Value = 1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@納品書発行区分", SqlDbType.Int).Value = 0;
                    }

                    if (空き容器回収区分 != null && 空き容器回収区分 != -1)
                    {
                        cmd.Parameters.Add("@空き容器回収区分", SqlDbType.Int).Value = 空き容器回収区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@空き容器回収区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (請求書敬称 != null && 請求書敬称 != -1)
                    {
                        cmd.Parameters.Add("@請求書敬称", SqlDbType.Int).Value = 請求書敬称;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求書敬称", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (備考１ != null)
                    {
                        cmd.Parameters.Add("@備考１", SqlDbType.VarChar).Value = 備考１;
                    }
                    else
                    {
                        cmd.Parameters.Add("@備考１", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (備考２ != null)
                    {
                        cmd.Parameters.Add("@備考２", SqlDbType.VarChar).Value = 備考２;
                    }
                    else
                    {
                        cmd.Parameters.Add("@備考２", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (備考３ != null)
                    {
                        cmd.Parameters.Add("@備考３", SqlDbType.VarChar).Value = 備考３;
                    }
                    else
                    {
                        cmd.Parameters.Add("@備考３", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (状態区分 != null && 状態区分 != -1)
                    {
                        cmd.Parameters.Add("@状態区分", SqlDbType.Int).Value = 状態区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@状態区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (承認日 != null)
                    {
                        cmd.Parameters.Add("@承認日", SqlDbType.DateTime).Value = 承認日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@承認日", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (承認者ＣＤ != null && 承認者ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@承認者ＣＤ", SqlDbType.Int).Value = 承認者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@承認者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (状態理由区分 != null && 状態理由区分 != -1)
                    {
                        cmd.Parameters.Add("@状態理由区分", SqlDbType.Int).Value = 状態理由区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@状態理由区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (休日設定 != null)
                    {
                        cmd.Parameters.Add("@休日設定", SqlDbType.VarChar).Value = 休日設定;
                    }
                    else
                    {
                        cmd.Parameters.Add("@休日設定", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (新規登録日 != null)
                    {
                        cmd.Parameters.Add("@新規登録日", SqlDbType.DateTime).Value = 新規登録日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@新規登録日", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (税区分 != null && 税区分 != -1)
                    {
                        cmd.Parameters.Add("@税区分", SqlDbType.Int).Value = 税区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@税区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (丸め処理区分 != null && 丸め処理区分 != -1)
                    {
                        cmd.Parameters.Add("@丸め処理区分", SqlDbType.Int).Value = 丸め処理区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@丸め処理区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (祝日配送区分 != null && 祝日配送区分 == true)
                    {
                        cmd.Parameters.Add("@祝日配送区分", SqlDbType.Int).Value = 1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@祝日配送区分", SqlDbType.Int).Value = 0;
                    }

                    if (誕生日1 != null)
                    {
                        cmd.Parameters.Add("@誕生日1", SqlDbType.Date).Value = 誕生日1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@誕生日1", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (誕生日2 != null)
                    {
                        cmd.Parameters.Add("@誕生日2", SqlDbType.Date).Value = 誕生日2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@誕生日2", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (失客日 != null)
                    {
                        cmd.Parameters.Add("@失客日", SqlDbType.Date).Value = 失客日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@失客日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (EB銀行支店ＣＤ != null)
                    {
                        cmd.Parameters.Add("@EB銀行支店ＣＤ", SqlDbType.VarChar).Value = EB銀行支店ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@EB銀行支店ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (受注時特記事項 != null)
                    {
                        cmd.Parameters.Add("@受注時特記事項", SqlDbType.VarChar).Value = 受注時特記事項;
                    }
                    else
                    {
                        cmd.Parameters.Add("@受注時特記事項", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (請求書郵便マークフラグ != null)
                    {
                        cmd.Parameters.Add("@請求書郵便マークフラグ", SqlDbType.Bit).Value = 請求書郵便マークフラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求書郵便マークフラグ", SqlDbType.Bit).Value = false;
                    }

                    if (集計ＣＤ != null)
                    {
                        cmd.Parameters.Add("@集計ＣＤ", SqlDbType.Int).Value = 集計ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (配送得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@配送得意先ＣＤ", SqlDbType.Int).Value = 配送得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;    // (★)
                    }

                    if (無効フラグ != null)
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = 無効フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("得意先マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "得意先ＣＤが重複しています。";
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
