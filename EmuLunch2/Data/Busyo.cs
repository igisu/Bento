using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;
using System.Text.RegularExpressions;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 部署マスタ
    /// </summary>
    public class Busyo : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// 全てON（稼働日）
        /// </summary>
        public static string ALL_ON = "11111111";

        /// <summary>
        /// 全てOFF（休日）
        /// </summary>
        public static string ALL_OFF = "00000000";
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <returns></returns>
        public static Busyo GetBusyo(int busyoCd)
        {
            return GetBusyo(busyoCd, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <returns></returns>
        public static Busyo GetBusyoReload(int busyoCd)
        {
            return GetBusyo(busyoCd, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static Busyo GetBusyo(int busyoCd, bool isValid)
        {
            Busyo busyo = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,A.部署グループ" +
                             "\r\n     ,A.部署名" +
                             "\r\n     ,A.部署名カナ" +
                             "\r\n     ,A.会社名称" +
                             "\r\n     ,A.郵便番号" +
                             "\r\n     ,A.住所" +
                             "\r\n     ,A.電話番号" +
                             "\r\n     ,A.ＦＡＸ" +
                             "\r\n     ,A.登録番号" +
                             "\r\n     ,A.モバイル_メッセージ" +
                             "\r\n     ,A.金融機関ＣＤ1" +
                             "\r\n     ,B1.銀行名 AS 金融機関名称1" +
                             "\r\n     ,A.金融機関支店ＣＤ1" +
                             "\r\n     ,BR1.支店名 AS 金融機関支店名称1" +
                             "\r\n     ,A.口座種別1" +
                             "\r\n     ,A.口座番号1" +
                             "\r\n     ,A.口座名義人1" +
                             "\r\n     ,A.金融機関ＣＤ2" +
                             "\r\n     ,B2.銀行名 AS 金融機関名称2" +
                             "\r\n     ,A.金融機関支店ＣＤ2" +
                             "\r\n     ,BR2.支店名 AS 金融機関支店名称2" +
                             "\r\n     ,A.口座種別2" +
                             "\r\n     ,A.口座番号2" +
                             "\r\n     ,A.口座名義人2" +
                             "\r\n     ,A.請求書タイプ区分" +
                             "\r\n     ,A.請求書備考1" +
                             "\r\n     ,A.請求書備考2" +
                             "\r\n     ,A.納品書備考1" +
                             "\r\n     ,A.納品書備考2" +
                             "\r\n     ,A.支店ＣＤ" +
                             "\r\n     ,A.配送日算出日数" +
                             "\r\n     ,A.配送稼働日" +
                             "\r\n     ,A.得意先休日考慮フラグ" +
                             "\r\n     ,A.製造日算出日数" +
                             "\r\n     ,A.製造稼働日" +
                             "\r\n     ,A.配送集計表出力区分" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者CD" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     部署マスタ AS A" +
                             "\r\n     LEFT JOIN 金融機関名称     AS B1  ON A.金融機関ＣＤ1 = B1.銀行ＣＤ" +
                             "\r\n     LEFT JOIN 金融機関支店名称 AS BR1 ON A.金融機関ＣＤ1 = BR1.銀行ＣＤ AND A.金融機関支店ＣＤ1 = BR1.支店ＣＤ" +
                             "\r\n     LEFT JOIN 金融機関名称     AS B2  ON A.金融機関ＣＤ2 = B2.銀行ＣＤ" +
                             "\r\n     LEFT JOIN 金融機関支店名称 AS BR2 ON A.金融機関ＣＤ2 = BR2.銀行ＣＤ AND A.金融機関支店ＣＤ2 = BR2.支店ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.部署ＣＤ = @部署ＣＤ" +
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
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCd;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("部署マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    busyo = new Busyo(dt.Rows[0]);
                }
            }

            return busyo;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _部署ＣＤ;
        private int? _部署グループ;
        private string _部署名;
        private string _部署名カナ;
        private string _会社名称;
        private string _郵便番号;
        private string _住所;
        private string _電話番号;
        private string _ＦＡＸ;
        private string _登録番号;
        private string _モバイル_メッセージ;
        private string _金融機関ＣＤ1;
        private string _金融機関支店ＣＤ1;
        private int? _口座種別1;
        private string _口座番号1;
        private string _口座名義人1;
        private string _金融機関ＣＤ2;
        private string _金融機関支店ＣＤ2;
        private int? _口座種別2;
        private string _口座番号2;
        private string _口座名義人2;
        private int? _請求書タイプ区分;
        private string _請求書備考1;
        private string _請求書備考2;
        private string _納品書備考1;
        private string _納品書備考2;
        private int? _支店ＣＤ;
        private int? _配送日算出日数;               // 0以下の値
        private string _配送稼働日;
        private bool? _得意先休日考慮フラグ;
        private int? _製造日算出日数;               // 0以下の値
        private string _製造稼働日;
        private int? _配送集計表出力区分;
        private bool? _無効フラグ;

        private string _金融機関名称1;
        private string _金融機関支店名称1;
        private string _金融機関名称2;
        private string _金融機関支店名称2;
        private int? _入力用配送日算出日数;         // 入力時は0以上
        private int? _入力用製造日算出日数;         // 入力時は0以上

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Busyo()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Busyo(Busyo src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署グループ = src.部署グループ;
            部署ＣＤ = src.部署ＣＤ;
            部署名 = src.部署名;
            部署名カナ = src.部署名カナ;
            会社名称 = src.会社名称;
            郵便番号 = src.郵便番号;
            住所 = src.住所;
            電話番号 = src.電話番号;
            ＦＡＸ = src.ＦＡＸ;
            登録番号 = src.登録番号;
            モバイル_メッセージ = src.モバイル_メッセージ;
            金融機関ＣＤ1 = src.金融機関ＣＤ1;
            金融機関支店ＣＤ1 = src.金融機関支店ＣＤ1;
            口座種別1 = src.口座種別1;
            口座番号1 = src.口座番号1;
            口座名義人1 = src.口座名義人1;
            金融機関ＣＤ2 = src.金融機関ＣＤ2;
            金融機関支店ＣＤ2 = src.金融機関支店ＣＤ2;
            口座種別2 = src.口座種別2;
            口座番号2 = src.口座番号2;
            口座名義人2 = src.口座名義人2;
            請求書タイプ区分 = src.請求書タイプ区分;
            請求書備考1 = src.請求書備考1;
            請求書備考2 = src.請求書備考2;
            納品書備考1 = src.納品書備考1;
            納品書備考2 = src.納品書備考2;
            支店ＣＤ = src.支店ＣＤ;
            配送日算出日数 = src.配送日算出日数;
            配送稼働日 = src.配送稼働日;
            得意先休日考慮フラグ = src.得意先休日考慮フラグ;
            製造日算出日数 = src.製造日算出日数;
            製造稼働日 = src.製造稼働日;
            配送集計表出力区分 = src.配送集計表出力区分;
            無効フラグ = src.無効フラグ;

            金融機関名称1 = src.金融機関名称1;
            金融機関支店名称1 = src.金融機関支店名称1;
            金融機関名称2 = src.金融機関名称2;
            金融機関支店名称2 = src.金融機関支店名称2;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Busyo(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署グループ = dr["部署グループ"] as int?;
            部署ＣＤ = dr["部署ＣＤ"] as int?;
            部署名 = dr["部署名"] as string;
            部署名カナ = dr["部署名カナ"] as string;
            会社名称 = dr["会社名称"] as string;
            郵便番号 = dr["郵便番号"] as string;
            住所 = dr["住所"] as string;
            電話番号 = dr["電話番号"] as string;
            ＦＡＸ = dr["ＦＡＸ"] as string;
            登録番号 = dr["登録番号"] as string;
            モバイル_メッセージ = dr["モバイル_メッセージ"] as string;
            金融機関ＣＤ1 = dr["金融機関ＣＤ1"] as string;
            金融機関支店ＣＤ1 = dr["金融機関支店ＣＤ1"] as string;
            口座種別1 = dr["口座種別1"] as int?;
            口座番号1 = dr["口座番号1"] as string;
            口座名義人1 = dr["口座名義人1"] as string;
            金融機関ＣＤ2 = dr["金融機関ＣＤ2"] as string;
            金融機関支店ＣＤ2 = dr["金融機関支店ＣＤ2"] as string;
            口座種別2 = dr["口座種別2"] as int?;
            口座番号2 = dr["口座番号2"] as string;
            口座名義人2 = dr["口座名義人2"] as string;
            請求書タイプ区分 = dr["請求書タイプ区分"] as int?;
            請求書備考1 = dr["請求書備考1"] as string;
            請求書備考2 = dr["請求書備考2"] as string;
            納品書備考1 = dr["納品書備考1"] as string;
            納品書備考2 = dr["納品書備考2"] as string;
            支店ＣＤ = dr["支店ＣＤ"] as int?;
            配送日算出日数 = dr["配送日算出日数"] as int?;
            配送稼働日 = dr["配送稼働日"] as string;
            得意先休日考慮フラグ = dr["得意先休日考慮フラグ"] as bool?;
            製造日算出日数 = dr["製造日算出日数"] as int?;
            製造稼働日 = dr["製造稼働日"] as string;
            配送集計表出力区分 = dr["配送集計表出力区分"] as int?;
            無効フラグ = dr["無効フラグ"] as bool?;

            if (dr.Table.Columns.Contains("金融機関名称1"))
            {
                金融機関名称1 = dr["金融機関名称1"] as string;
            }

            if (dr.Table.Columns.Contains("金融機関支店名称1"))
            {
                金融機関支店名称1 = dr["金融機関支店名称1"] as string;
            }

            if (dr.Table.Columns.Contains("金融機関名称2"))
            {
                金融機関名称2 = dr["金融機関名称2"] as string;
            }

            if (dr.Table.Columns.Contains("金融機関支店名称2"))
            {
                金融機関支店名称2 = dr["金融機関支店名称2"] as string;
            }

            base.FromDataRow(dr);

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署グループ
        {
            get { return _部署グループ; }
            set
            {
                _部署グループ = value;
                _errors.Remove("部署グループ");
                if (value == null || value < 0)
                {
                    _errors["部署グループ"] = "部署グループを入力してください。";
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
                if (value == null)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
                else if(value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤにマイナスは入力できません。";
                }
            }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public string 部署名カナ
        {
            get { return _部署名カナ; }
            set { _部署名カナ = value; }
        }

        public string 会社名称
        {
            get { return _会社名称; }
            set { _会社名称 = value; }
        }

        public string 郵便番号
        {
            get { return _郵便番号; }
            set { _郵便番号 = value; }
        }

        public string 住所
        {
            get { return _住所; }
            set { _住所 = value; }
        }

        public string 電話番号
        {
            get { return _電話番号; }
            set { _電話番号 = value; }
        }

        public string ＦＡＸ
        {
            get { return _ＦＡＸ; }
            set { _ＦＡＸ = value; }
        }

        public string 登録番号
        {
            get { return _登録番号; }
            set
            {
                _登録番号 = value;
                _errors.Remove("登録番号");

                if (value != null && !Regex.IsMatch(value, @"^[T0-9]+$"))
                {
                    _errors["登録番号"] = "登録番号が正しい形式ではありません。";
                }
            }
        }

        public string モバイル_メッセージ
        {
            get { return _モバイル_メッセージ; }
            set { _モバイル_メッセージ = value; }
        }

        public string 金融機関ＣＤ1
        {
            get { return _金融機関ＣＤ1; }
            set
            {
                _金融機関ＣＤ1 = value;
                _errors.Remove("金融機関ＣＤ1");
                金融機関名称1 = null;
                if (value != null && _stopOverwrite == false)
                {
                    金融機関支店ＣＤ1 = null;  // 銀行支店名を消す

                    // 金融機関ＣＤに該当する銀行名を取得
                    Bank bank = Bank.GetBank(_金融機関ＣＤ1);
                    if (bank != null)
                    {
                        金融機関名称1 = bank.銀行名;
                    }
                    else
                    {
                        _errors["金融機関ＣＤ1"] = "金融機関ＣＤ1が見つかりません。";
                    }
                }
            }
        }

        public string 金融機関支店ＣＤ1
        {
            get { return _金融機関支店ＣＤ1; }
            set
            {
                _金融機関支店ＣＤ1 = value;
                _errors.Remove("金融機関支店ＣＤ1");
                金融機関支店名称1 = null;
                if (value != null && _金融機関ＣＤ1 != null && _stopOverwrite == false)
                {
                    // 金融機関ＣＤと金融機関支店ＣＤに該当する銀行支店名を取得
                    BankBranch bankBranch = BankBranch.GetBankBranch(_金融機関ＣＤ1, _金融機関支店ＣＤ1);
                    if (bankBranch != null)
                    {
                        金融機関支店名称1 = bankBranch.支店名;
                    }
                    else
                    {
                        _errors["金融機関支店ＣＤ1"] = "金融機関支店ＣＤ1が見つかりません。";
                    }
                }
            }
        }

        public int? 口座種別1
        {
            get { return _口座種別1; }
            set { _口座種別1 = value; }
        }

        public string 口座番号1
        {
            get { return _口座番号1; }
            set { _口座番号1 = value; }
        }

        public string 口座名義人1
        {
            get { return _口座名義人1; }
            set { _口座名義人1 = value; }
        }

        public string 金融機関ＣＤ2
        {
            get { return _金融機関ＣＤ2; }
            set
            {
                _金融機関ＣＤ2 = value;
                _errors.Remove("金融機関ＣＤ2");
                金融機関名称2 = null;
                if (value != null && _stopOverwrite == false)
                {
                    金融機関支店ＣＤ2 = null;  // 銀行支店名を消す

                    // 請求銀行CDに該当する銀行名を取得
                    Bank bank = Bank.GetBank(_金融機関ＣＤ2);
                    if (bank != null)
                    {
                        金融機関名称2 = bank.銀行名;
                    }
                    else
                    {
                        _errors["金融機関ＣＤ2"] = "金融機関ＣＤ2が見つかりません。";
                    }
                }
            }
        }

        public string 金融機関支店ＣＤ2
        {
            get { return _金融機関支店ＣＤ2; }
            set
            {
                _金融機関支店ＣＤ2 = value;
                _errors.Remove("金融機関支店ＣＤ2");
                金融機関支店名称2 = null;
                if (value != null && _金融機関ＣＤ2 != null && _stopOverwrite == false)
                {
                    // 金融機関ＣＤと金融機関支店ＣＤに該当する銀行支店名を取得
                    BankBranch bankBranch = BankBranch.GetBankBranch(_金融機関ＣＤ2, _金融機関支店ＣＤ2);
                    if (bankBranch != null)
                    {
                        金融機関支店名称2 = bankBranch.支店名;
                    }
                    else
                    {
                        _errors["金融機関支店ＣＤ2"] = "金融機関支店ＣＤ2が見つかりません。";
                    }
                }
            }
        }

        public int? 口座種別2
        {
            get { return _口座種別2; }
            set { _口座種別2 = value; }
        }

        public string 口座番号2
        {
            get { return _口座番号2; }
            set { _口座番号2 = value; }
        }

        public string 口座名義人2
        {
            get { return _口座名義人2; }
            set { _口座名義人2 = value; }
        }

        public int? 請求書タイプ区分
        {
            get { return _請求書タイプ区分; }
            set { _請求書タイプ区分 = value; }
        }

        public string 請求書備考1
        {
            get { return _請求書備考1; }
            set { _請求書備考1 = value; }
        }

        public string 請求書備考2
        {
            get { return _請求書備考2; }
            set { _請求書備考2 = value; }
        }

        public string 納品書備考1
        {
            get { return _納品書備考1; }
            set { _納品書備考1 = value; }
        }

        public string 納品書備考2
        {
            get { return _納品書備考2; }
            set { _納品書備考2 = value; }
        }

        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set
            {
                _支店ＣＤ = value;
                _errors.Remove("支店ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["支店ＣＤ"] = "支店ＣＤを入力してください。";
                }
            }
        }

        public int? 配送日算出日数
        {
            get { return _配送日算出日数; }
            set
            {
                _配送日算出日数 = null;
                _入力用配送日算出日数 = null;
                if (value != null)
                {
                    _配送日算出日数 = Math.Abs((int)value) * -1;
                    _入力用配送日算出日数 = Math.Abs((int)value);
                }
            }
        }

        public string 配送稼働日
        {
            get { return _配送稼働日; }
            set { _配送稼働日 = value; }
        }

        public bool? 得意先休日考慮フラグ
        {
            get { return _得意先休日考慮フラグ; }
            set { _得意先休日考慮フラグ = value; }
        }

        public int? 製造日算出日数
        {
            get { return _製造日算出日数; }
            set
            {
                _製造日算出日数 = null;
                _入力用製造日算出日数 = null;
                if (value != null)
                {
                    _製造日算出日数 = Math.Abs((int)value) * -1;
                    _入力用製造日算出日数 = Math.Abs((int)value);
                }
            }
        }

        public string 製造稼働日
        {
            get { return _製造稼働日; }
            set { _製造稼働日 = value; }
        }

        public int? 配送集計表出力区分
        {
            get { return (_配送集計表出力区分 ?? 0); }
            set { _配送集計表出力区分 = (value ?? 0); }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public string 金融機関名称1
        {
            get { return _金融機関名称1; }
            set { _金融機関名称1 = value; }
        }

        public string 金融機関名称2
        {
            get { return _金融機関名称2; }
            set { _金融機関名称2 = value; }
        }

        public string 金融機関支店名称1
        {
            get { return _金融機関支店名称1; }
            set { _金融機関支店名称1 = value; }
        }

        public string 金融機関支店名称2
        {
            get { return _金融機関支店名称2; }
            set { _金融機関支店名称2 = value; }
        }

        public int? 入力用製造日算出日数
        {
            get { return _入力用製造日算出日数; }
            set
            {
                _製造日算出日数 = null;
                _入力用製造日算出日数 = null;
                if (value != null)
                {
                    _製造日算出日数 = Math.Abs((int)value) * -1;
                    _入力用製造日算出日数 = Math.Abs((int)value);
                }
            }
        }

        public int? 入力用配送日算出日数
        {
            get { return _入力用配送日算出日数; }
            set
            {
                _配送日算出日数 = null;
                _入力用配送日算出日数 = null;
                if (value != null)
                {
                    _配送日算出日数 = Math.Abs((int)value) * -1;
                    _入力用配送日算出日数 = Math.Abs((int)value);
                }
            }
        }

        public bool? 配送稼働日設定日曜
        {
            get
            {
                return GetHaiso(Consts.EmuDayOfWeek.Sunday);
            }
            set
            {
                SetHaiso(Consts.EmuDayOfWeek.Sunday, value);
            }
        }

        public bool? 配送稼働日設定月曜
        {
            get
            {
                return GetHaiso(Consts.EmuDayOfWeek.Monday);
            }
            set
            {
                SetHaiso(Consts.EmuDayOfWeek.Monday, value);
            }
        }

        public bool? 配送稼働日設定火曜
        {
            get
            {
                return GetHaiso(Consts.EmuDayOfWeek.Tuesday);
            }
            set
            {
                SetHaiso(Consts.EmuDayOfWeek.Tuesday, value);
            }
        }

        public bool? 配送稼働日設定水曜
        {
            get
            {
                return GetHaiso(Consts.EmuDayOfWeek.Wednesday);
            }
            set
            {
                SetHaiso(Consts.EmuDayOfWeek.Wednesday, value);
            }
        }

        public bool? 配送稼働日設定木曜
        {
            get
            {
                return GetHaiso(Consts.EmuDayOfWeek.Thursday);
            }
            set
            {
                SetHaiso(Consts.EmuDayOfWeek.Thursday, value);
            }
        }

        public bool? 配送稼働日設定金曜
        {
            get
            {
                return GetHaiso(Consts.EmuDayOfWeek.Friday);
            }
            set
            {
                SetHaiso(Consts.EmuDayOfWeek.Friday, value);
            }
        }

        public bool? 配送稼働日設定土曜
        {
            get
            {
                return GetHaiso(Consts.EmuDayOfWeek.Saturday);
            }
            set
            {
                SetHaiso(Consts.EmuDayOfWeek.Saturday, value);
            }
        }

        public bool? 配送稼働日設定祝日
        {
            get
            {
                return GetHaiso(Consts.EmuDayOfWeek.Holiday);
            }
            set
            {
                SetHaiso(Consts.EmuDayOfWeek.Holiday, value);
            }
        }

        public bool? 製造稼働日設定日曜
        {
            get
            {
                return GetSeizo(Consts.EmuDayOfWeek.Sunday);
            }
            set
            {
                SetSeizo(Consts.EmuDayOfWeek.Sunday, value);
            }
        }

        public bool? 製造稼働日設定月曜
        {
            get
            {
                return GetSeizo(Consts.EmuDayOfWeek.Monday);
            }
            set
            {
                SetSeizo(Consts.EmuDayOfWeek.Monday, value);
            }
        }

        public bool? 製造稼働日設定火曜
        {
            get
            {
                return GetSeizo(Consts.EmuDayOfWeek.Tuesday);
            }
            set
            {
                SetSeizo(Consts.EmuDayOfWeek.Tuesday, value);
            }
        }

        public bool? 製造稼働日設定水曜
        {
            get
            {
                return GetSeizo(Consts.EmuDayOfWeek.Wednesday);
            }
            set
            {
                SetSeizo(Consts.EmuDayOfWeek.Wednesday, value);
            }
        }

        public bool? 製造稼働日設定木曜
        {
            get
            {
                return GetSeizo(Consts.EmuDayOfWeek.Thursday);
            }
            set
            {
                SetSeizo(Consts.EmuDayOfWeek.Thursday, value);
            }
        }

        public bool? 製造稼働日設定金曜
        {
            get
            {
                return GetSeizo(Consts.EmuDayOfWeek.Friday);
            }
            set
            {
                SetSeizo(Consts.EmuDayOfWeek.Friday, value);
            }
        }

        public bool? 製造稼働日設定土曜
        {
            get
            {
                return GetSeizo(Consts.EmuDayOfWeek.Saturday);
            }
            set
            {
                SetSeizo(Consts.EmuDayOfWeek.Saturday, value);
            }
        }

        public bool? 製造稼働日設定祝日
        {
            get
            {
                return GetSeizo(Consts.EmuDayOfWeek.Holiday);
            }
            set
            {
                SetSeizo(Consts.EmuDayOfWeek.Holiday, value);
            }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}]", 部署ＣＤ); }
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
                // 製造日算出日数の絶対値より配送日算出日数の絶対値が大きいとエラー
                // ※製造日より前に配送はできない
                if (Math.Abs((製造日算出日数 ?? 0)) < Math.Abs((配送日算出日数 ?? 0)))
                {
                    return "配送日算出日数は、製造日算出日数以下で入力してください。";
                }
                return null;
            }
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
        /// 配送稼働日設定の該当の曜日の値を返す
        /// </summary>
        /// <param name="dayOfWeek">配送稼働日設定の曜日インデックス</param>
        /// <returns>配送稼働日の場合はtrue。それ以外はfalse。</returns>
        public bool GetHaiso(Consts.EmuDayOfWeek dayOfWeek)
        {
            if (string.IsNullOrEmpty(配送稼働日))
            {
                return false;
            }
            // 日:0～土:6, 祝日:7として、該当の曜日の位置の文字を変換する
            int index = (int)dayOfWeek;
            if (配送稼働日.Length > index)
            {
                string s = 配送稼働日.ToCharArray()[index].ToString();
                if (s == "1")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 配送稼働日設定の該当の曜日の値を設定する
        /// </summary>
        /// <param name="dayOfWeek">配送稼働日設定の曜日インデックス</param>
        /// <param name="value">配送稼働日の場合はtrue。</param>
        private void SetHaiso(Consts.EmuDayOfWeek dayOfWeek, bool? value)
        {
            if (string.IsNullOrEmpty(配送稼働日))
            {
                配送稼働日 = ALL_OFF;
            }
            char[] c = 配送稼働日.ToCharArray();
            int index = (int)dayOfWeek;         // 日:0～土:6, 祝日:7
            if (value == true)
            {
                c[index] = '1';
            }
            else
            {
                c[index] = '0';
            }
            配送稼働日 = string.Join("", c);
        }

        /// <summary>
        /// 製造稼働日設定の該当の曜日の値を返す
        /// </summary>
        /// <param name="dayOfWeek">製造稼働日設定の曜日インデックス</param>
        /// <returns>製造稼働日の場合はtrue。それ以外はfalse。</returns>
        public bool GetSeizo(Consts.EmuDayOfWeek dayOfWeek)
        {
            if (string.IsNullOrEmpty(製造稼働日))
            {
                return false;
            }
            // 日:0～土:6, 祝日:7として、該当の曜日の位置の文字を変換する
            int index = (int)dayOfWeek;
            if (製造稼働日.Length > index)
            {
                string s = 製造稼働日.ToCharArray()[index].ToString();
                if (s == "1")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 製造稼働日設定の該当の曜日の値を設定する
        /// </summary>
        /// <param name="dayOfWeek">製造稼働日設定の曜日インデックス</param>
        /// <param name="value">製造稼働日の場合はtrue。</param>
        private void SetSeizo(Consts.EmuDayOfWeek dayOfWeek, bool? value)
        {
            if (string.IsNullOrEmpty(製造稼働日))
            {
                製造稼働日 = ALL_OFF;
            }
            char[] c = 製造稼働日.ToCharArray();
            int index = (int)dayOfWeek;         // 日:0～土:6, 祝日:7
            if (value == true)
            {
                c[index] = '1';
            }
            else
            {
                c[index] = '0';
            }
            製造稼働日 = string.Join("", c);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(Busyo target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                (部署グループ ?? -1) == (target.部署グループ ?? -1) &&
                部署名 == target.部署名 &&
                部署名カナ == target.部署名カナ &&
                会社名称 == target.会社名称 &&
                郵便番号 == target.郵便番号 &&
                住所 == target.住所 &&
                電話番号 == target.電話番号 &&
                ＦＡＸ == target.ＦＡＸ &&
                登録番号 == target.登録番号 &&
                モバイル_メッセージ == target.モバイル_メッセージ &&
                金融機関ＣＤ1 == target.金融機関ＣＤ1 &&
                金融機関支店ＣＤ1 == target.金融機関支店ＣＤ1 &&
                (口座種別1 ?? -1) == (target.口座種別1 ?? -1) &&
                口座番号1 == target.口座番号1 &&
                口座名義人1 == target.口座名義人1 &&
                金融機関ＣＤ2 == target.金融機関ＣＤ2 &&
                金融機関支店ＣＤ2 == target.金融機関支店ＣＤ2 &&
                (口座種別2 ?? -1) == (target.口座種別2 ?? -1) &&
                口座番号2 == target.口座番号2 &&
                口座名義人2 == target.口座名義人2 &&
                (請求書タイプ区分 ?? -1)== (target.請求書タイプ区分 ?? -1)&&
                請求書備考1 == target.請求書備考1 &&
                請求書備考2 == target.請求書備考2 &&
                納品書備考1 == target.納品書備考1 &&
                納品書備考2 == target.納品書備考2 &&
                (支店ＣＤ ?? -1) == (target.支店ＣＤ ?? -1) &&
                配送日算出日数 == target.配送日算出日数 &&
                配送稼働日 == target.配送稼働日 &&
                (得意先休日考慮フラグ ?? false) == (target.得意先休日考慮フラグ ?? false) &&
                製造日算出日数 == target.製造日算出日数 &&
                製造稼働日 == target.製造稼働日 &&
                配送集計表出力区分 == target.配送集計表出力区分 &&
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
            string sql = "\r\n INSERT INTO 部署マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,部署グループ" +
                         "\r\n     ,部署名" +
                         "\r\n     ,部署名カナ" +
                         "\r\n     ,会社名称" +
                         "\r\n     ,郵便番号" +
                         "\r\n     ,住所" +
                         "\r\n     ,電話番号" +
                         "\r\n     ,ＦＡＸ" +
                         "\r\n     ,登録番号" +
                         "\r\n     ,モバイル_メッセージ" +
                         "\r\n     ,金融機関ＣＤ1" +
                         "\r\n     ,金融機関支店ＣＤ1" +
                         "\r\n     ,口座種別1" +
                         "\r\n     ,口座番号1" +
                         "\r\n     ,口座名義人1" +
                         "\r\n     ,金融機関ＣＤ2" +
                         "\r\n     ,金融機関支店ＣＤ2" +
                         "\r\n     ,口座種別2" +
                         "\r\n     ,口座番号2" +
                         "\r\n     ,口座名義人2" +
                         "\r\n     ,請求書タイプ区分" +
                         "\r\n     ,請求書備考1" +
                         "\r\n     ,請求書備考2" +
                         "\r\n     ,納品書備考1" +
                         "\r\n     ,納品書備考2" +
                         "\r\n     ,支店ＣＤ" +
                         "\r\n     ,配送日算出日数" +
                         "\r\n     ,配送稼働日" +
                         "\r\n     ,得意先休日考慮フラグ" +
                         "\r\n     ,製造日算出日数" +
                         "\r\n     ,製造稼働日" +
                         "\r\n     ,配送集計表出力区分" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者CD" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@部署グループ" +
                         "\r\n     ,@部署名" +
                         "\r\n     ,@部署名カナ" +
                         "\r\n     ,@会社名称" +
                         "\r\n     ,@郵便番号" +
                         "\r\n     ,@住所" +
                         "\r\n     ,@電話番号" +
                         "\r\n     ,@ＦＡＸ" +
                         "\r\n     ,@登録番号" +
                         "\r\n     ,@モバイル_メッセージ" +
                         "\r\n     ,@金融機関ＣＤ1" +
                         "\r\n     ,@金融機関支店ＣＤ1" +
                         "\r\n     ,@口座種別1" +
                         "\r\n     ,@口座番号1" +
                         "\r\n     ,@口座名義人1" +
                         "\r\n     ,@金融機関ＣＤ2" +
                         "\r\n     ,@金融機関支店ＣＤ2" +
                         "\r\n     ,@口座種別2" +
                         "\r\n     ,@口座番号2" +
                         "\r\n     ,@口座名義人2" +
                         "\r\n     ,@請求書タイプ区分" +
                         "\r\n     ,@請求書備考1" +
                         "\r\n     ,@請求書備考2" +
                         "\r\n     ,@納品書備考1" +
                         "\r\n     ,@納品書備考2" +
                         "\r\n     ,@支店ＣＤ" +
                         "\r\n     ,@配送日算出日数" +
                         "\r\n     ,@配送稼働日" +
                         "\r\n     ,@得意先休日考慮フラグ" +
                         "\r\n     ,@製造日算出日数" +
                         "\r\n     ,@製造稼働日" +
                         "\r\n     ,@配送集計表出力区分" +
                         "\r\n     ,@無効フラグ" +
                         "\r\n     ,@作成担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n     ,@修正担当者CD" +
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
            string sql = "\r\n UPDATE 部署マスタ" +
                         "\r\n SET 部署グループ         = @部署グループ" +
                         "\r\n    ,部署名               = @部署名" +
                         "\r\n    ,部署名カナ           = @部署名カナ" +
                         "\r\n    ,会社名称             = @会社名称" +
                         "\r\n    ,郵便番号             = @郵便番号" +
                         "\r\n    ,住所                 = @住所" +
                         "\r\n    ,電話番号             = @電話番号" +
                         "\r\n    ,ＦＡＸ               = @ＦＡＸ" +
                         "\r\n    ,登録番号             = @登録番号" +
                         "\r\n    ,モバイル_メッセージ  = @モバイル_メッセージ" +
                         "\r\n    ,金融機関ＣＤ1        = @金融機関ＣＤ1" +
                         "\r\n    ,金融機関支店ＣＤ1    = @金融機関支店ＣＤ1" +
                         "\r\n    ,口座種別1            = @口座種別1" +
                         "\r\n    ,口座番号1            = @口座番号1" +
                         "\r\n    ,口座名義人1          = @口座名義人1" +
                         "\r\n    ,金融機関ＣＤ2        = @金融機関ＣＤ2" +
                         "\r\n    ,金融機関支店ＣＤ2    = @金融機関支店ＣＤ2" +
                         "\r\n    ,口座種別2            = @口座種別2" +
                         "\r\n    ,口座番号2            = @口座番号2" +
                         "\r\n    ,口座名義人2          = @口座名義人2" +
                         "\r\n    ,請求書タイプ区分     = @請求書タイプ区分" +
                         "\r\n    ,請求書備考1          = @請求書備考1" +
                         "\r\n    ,請求書備考2          = @請求書備考2" +
                         "\r\n    ,納品書備考1          = @納品書備考1" +
                         "\r\n    ,納品書備考2          = @納品書備考2" +
                         "\r\n    ,支店ＣＤ             = @支店ＣＤ" +
                         "\r\n    ,配送日算出日数       = @配送日算出日数" +
                         "\r\n    ,配送稼働日           = @配送稼働日" +
                         "\r\n    ,得意先休日考慮フラグ = @得意先休日考慮フラグ" +
                         "\r\n    ,製造日算出日数       = @製造日算出日数" +
                         "\r\n    ,製造稼働日           = @製造稼働日" +
                         "\r\n    ,配送集計表出力区分   = @配送集計表出力区分" +
                         "\r\n    ,無効フラグ           = @無効フラグ" +
                         "\r\n    ,修正担当者CD         = @修正担当者CD" +
                         "\r\n    ,修正日               = getdate()" +
                         "\r\n WHERE 企業ＣＤ           = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ           = @部署ＣＤ" +
                         "\r\n   AND 修正日             = @修正日";

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
            string sql = "\r\n DELETE FROM 部署マスタ" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
                         "\r\n   AND 修正日   = @修正日";

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

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (部署グループ != null && 部署グループ != -1)
                    {
                        cmd.Parameters.Add("@部署グループ", SqlDbType.Int).Value = 部署グループ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署グループ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (部署名 != null)
                    {
                        cmd.Parameters.Add("@部署名", SqlDbType.VarChar).Value = 部署名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (部署名カナ != null)
                    {
                        cmd.Parameters.Add("@部署名カナ", SqlDbType.VarChar).Value = 部署名カナ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署名カナ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (会社名称 != null)
                    {
                        cmd.Parameters.Add("@会社名称", SqlDbType.VarChar).Value = 会社名称;
                    }
                    else
                    {
                        cmd.Parameters.Add("@会社名称", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (郵便番号 != null)
                    {
                        cmd.Parameters.Add("@郵便番号", SqlDbType.VarChar).Value = 郵便番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@郵便番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (住所 != null)
                    {
                        cmd.Parameters.Add("@住所", SqlDbType.VarChar).Value = 住所;
                    }
                    else
                    {
                        cmd.Parameters.Add("@住所", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (ＦＡＸ != null)
                    {
                        cmd.Parameters.Add("@ＦＡＸ", SqlDbType.VarChar).Value = ＦＡＸ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ＦＡＸ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (登録番号 != null)
                    {
                        cmd.Parameters.Add("@登録番号", SqlDbType.VarChar).Value = 登録番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@登録番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (モバイル_メッセージ != null)
                    {
                        cmd.Parameters.Add("@モバイル_メッセージ", SqlDbType.VarChar).Value = モバイル_メッセージ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@モバイル_メッセージ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (金融機関ＣＤ1 != null)
                    {
                        cmd.Parameters.Add("@金融機関ＣＤ1", SqlDbType.VarChar).Value = 金融機関ＣＤ1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金融機関ＣＤ1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (金融機関支店ＣＤ1 != null)
                    {
                        cmd.Parameters.Add("@金融機関支店ＣＤ1", SqlDbType.VarChar).Value = 金融機関支店ＣＤ1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金融機関支店ＣＤ1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (口座種別1 != null)
                    {
                        cmd.Parameters.Add("@口座種別1", SqlDbType.Decimal).Value = 口座種別1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座種別1", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (口座番号1 != null)
                    {
                        cmd.Parameters.Add("@口座番号1", SqlDbType.VarChar).Value = 口座番号1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座番号1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (口座名義人1 != null)
                    {
                        cmd.Parameters.Add("@口座名義人1", SqlDbType.VarChar).Value = 口座名義人1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座名義人1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (金融機関ＣＤ2 != null)
                    {
                        cmd.Parameters.Add("@金融機関ＣＤ2", SqlDbType.VarChar).Value = 金融機関ＣＤ2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金融機関ＣＤ2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (金融機関支店ＣＤ2 != null)
                    {
                        cmd.Parameters.Add("@金融機関支店ＣＤ2", SqlDbType.VarChar).Value = 金融機関支店ＣＤ2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金融機関支店ＣＤ2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (口座種別2 != null)
                    {
                        cmd.Parameters.Add("@口座種別2", SqlDbType.Decimal).Value = 口座種別2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座種別2", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (口座番号2 != null)
                    {
                        cmd.Parameters.Add("@口座番号2", SqlDbType.VarChar).Value = 口座番号2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座番号2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (口座名義人2 != null)
                    {
                        cmd.Parameters.Add("@口座名義人2", SqlDbType.VarChar).Value = 口座名義人2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座名義人2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (請求書タイプ区分 != null && 請求書タイプ区分 != -1)
                    {
                        cmd.Parameters.Add("@請求書タイプ区分", SqlDbType.Int).Value = 請求書タイプ区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求書タイプ区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (請求書備考1 != null)
                    {
                        cmd.Parameters.Add("@請求書備考1", SqlDbType.VarChar).Value = 請求書備考1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求書備考1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (請求書備考2 != null)
                    {
                        cmd.Parameters.Add("@請求書備考2", SqlDbType.VarChar).Value = 請求書備考2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求書備考2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (納品書備考1 != null)
                    {
                        cmd.Parameters.Add("@納品書備考1", SqlDbType.VarChar).Value = 納品書備考1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@納品書備考1", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (納品書備考2 != null)
                    {
                        cmd.Parameters.Add("@納品書備考2", SqlDbType.VarChar).Value = 納品書備考2;
                    }
                    else
                    {
                        cmd.Parameters.Add("@納品書備考2", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (支店ＣＤ != null && 支店ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = 支店ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (配送日算出日数 != null)
                    {
                        cmd.Parameters.Add("@配送日算出日数", SqlDbType.Int).Value = 配送日算出日数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送日算出日数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (配送稼働日 != null)
                    {
                        cmd.Parameters.Add("@配送稼働日", SqlDbType.VarChar).Value = 配送稼働日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送稼働日", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (得意先休日考慮フラグ != null)
                    {
                        cmd.Parameters.Add("@得意先休日考慮フラグ", SqlDbType.Bit).Value = 得意先休日考慮フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先休日考慮フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (製造日算出日数 != null)
                    {
                        cmd.Parameters.Add("@製造日算出日数", SqlDbType.Int).Value = 製造日算出日数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@製造日算出日数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (製造稼働日 != null)
                    {
                        cmd.Parameters.Add("@製造稼働日", SqlDbType.VarChar).Value = 製造稼働日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@製造稼働日", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (配送集計表出力区分 != null)
                    {
                        cmd.Parameters.Add("@配送集計表出力区分", SqlDbType.Int).Value = 配送集計表出力区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送集計表出力区分", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("部署マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "部署ＣＤが重複しています。";
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
