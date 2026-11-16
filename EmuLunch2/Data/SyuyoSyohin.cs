using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 主要商品リストマスタ
    /// </summary>
    public class SyuyoSyohin : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// 全てON（注文可能）
        /// </summary>
        public static string ALL_ON = "1111111";

        /// <summary>
        /// 全てOFF（注文不可）
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
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <param name="syoCd">商品ＣＤ</param>
        /// <returns></returns>
        public static SyuyoSyohin GetSyuyoSyohin(int busyoCd, int syoCd)
        {
            return GetSyuyoSyohin(busyoCd, syoCd, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <param name="syoCd">商品ＣＤ</param>
        /// <returns></returns>
        public static SyuyoSyohin GetSyuyoSyohinReload(int busyoCd, int syoCd)
        {
            return GetSyuyoSyohin(busyoCd, syoCd, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <param name="syoCd">商品ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static SyuyoSyohin GetSyuyoSyohin(int busyoCd, int syoCd, bool isValid)
        {
            SyuyoSyohin syuyoSyohin = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,A.商品ＣＤ" +
                             "\r\n     ,S.商品名略称" +
                             "\r\n     ,A.商品グループＣＤ" +
                             "\r\n     ,A.主要商品フラグ" +
                             "\r\n     ,A.WEB注文不可フラグ" +
                             "\r\n     ,A.WEB注文可能期限日数" +
                             "\r\n     ,A.WEB注文可能曜日" +
                             "\r\n     ,A.WEB注文開始日" +
                             "\r\n     ,A.WEB注文終了日" +

                             "\r\n     ,A.WEB注文締切時間" +
                             "\r\n     ,A.WEB注文締切分" +

                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     主要商品リストマスタ AS A" +
                             "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.部署ＣＤ = @部署ＣＤ" +
                             "\r\n AND A.商品ＣＤ = @商品ＣＤ" +
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
                cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syoCd;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("主要商品リストマスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    syuyoSyohin = new SyuyoSyohin(dt.Rows[0]);
                }
            }

            return syuyoSyohin;
        }

        /// <summary>
        /// 注文可能曜日の該当の曜日の値を返す（スタティックメソッド版）
        /// </summary>
        /// <param name="youbiSettei">注文可能曜日</param>
        /// <param name="dayOfWeek">注文可能曜日の曜日インデックス</param>
        /// <returns>休日の場合はtrue。それ以外はfalse。</returns>
        public static bool GetOrderableWeekday(string youbiSettei, DayOfWeek dayOfWeek)
        {
            if (string.IsNullOrEmpty(youbiSettei))
            {
                return false;
            }
            // 日:0～土:6として、該当の曜日の位置の文字を変換する
            int index = (int)dayOfWeek;
            if (youbiSettei.Length > index)
            {
                string s = youbiSettei.ToCharArray()[index].ToString();
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
        private bool _stopOverwrite;            // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _部署ＣＤ;
        private int? _商品ＣＤ;
        private int? _商品ＣＤorg;              // レコード読込直後の値（更新・削除時に使用する）
        private int? _商品グループＣＤ;
        private bool? _主要商品フラグ;
        private bool? _WEB注文不可フラグ;
        private int? _WEB注文可能期限日数;
        private string _WEB注文可能曜日;
        private DateTime? _WEB注文開始日;
        private DateTime? _WEB注文終了日;

        private int? _WEB注文締切時間;
        private int? _WEB注文締切分;

        private bool? _無効フラグ;

        private string _商品名略称;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SyuyoSyohin()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SyuyoSyohin(SyuyoSyohin src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署ＣＤ = src.部署ＣＤ;
            商品ＣＤ = src.商品ＣＤ;
            商品ＣＤorg = src.商品ＣＤorg;
            商品グループＣＤ = src.商品グループＣＤ;
            主要商品フラグ = src.主要商品フラグ;
            WEB注文不可フラグ = src.WEB注文不可フラグ;
            WEB注文可能期限日数 = src.WEB注文可能期限日数;
            WEB注文可能曜日 = src.WEB注文可能曜日;
            WEB注文開始日 = src.WEB注文開始日;
            WEB注文終了日 = src.WEB注文終了日;

            WEB注文締切時間 = src.WEB注文締切時間;
            WEB注文締切分 = src.WEB注文締切分;

            無効フラグ = src.無効フラグ;

            商品名略称 = src.商品名略称;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SyuyoSyohin(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            商品ＣＤ = dr["商品ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("商品ＣＤorg"))
            {
                商品ＣＤorg = dr["商品ＣＤorg", version] as int?;
            }
            else
            {
                商品ＣＤorg = dr["商品ＣＤ", version] as int?;
            }

            商品グループＣＤ = dr["商品グループＣＤ", version] as int?;
            主要商品フラグ = dr["主要商品フラグ", version] as bool?;
            WEB注文不可フラグ = dr["WEB注文不可フラグ", version] as bool?;
            WEB注文可能期限日数 = dr["WEB注文可能期限日数", version] as int?;
            WEB注文可能曜日 = dr["WEB注文可能曜日", version] as string;
            WEB注文開始日 = dr["WEB注文開始日", version] as DateTime?;
            WEB注文終了日 = dr["WEB注文終了日", version] as DateTime?;

            WEB注文締切時間 = dr["WEB注文締切時間", version] as int?;
            WEB注文締切分 = dr["WEB注文締切分", version] as int?;

            無効フラグ = dr["無効フラグ", version] as bool?;

            if (dr.Table.Columns.Contains("商品名略称"))
            {
                商品名略称 = dr["商品名略称", version] as string;
            }

            // Web注文用商品設定画面向け
            if (dr.Table.Columns.Contains("WEB注文可能日祝"))
            {
                WEB注文可能日祝 = dr["WEB注文可能日祝", version] as bool?;
            }
            if (dr.Table.Columns.Contains("WEB注文可能月曜"))
            {
                WEB注文可能月曜 = dr["WEB注文可能月曜", version] as bool?;
            }
            if (dr.Table.Columns.Contains("WEB注文可能火曜"))
            {
                WEB注文可能火曜 = dr["WEB注文可能火曜", version] as bool?;
            }
            if (dr.Table.Columns.Contains("WEB注文可能水曜"))
            {
                WEB注文可能水曜 = dr["WEB注文可能水曜", version] as bool?;
            }
            if (dr.Table.Columns.Contains("WEB注文可能木曜"))
            {
                WEB注文可能木曜 = dr["WEB注文可能木曜", version] as bool?;
            }
            if (dr.Table.Columns.Contains("WEB注文可能金曜"))
            {
                WEB注文可能金曜 = dr["WEB注文可能金曜", version] as bool?;
            }
            if (dr.Table.Columns.Contains("WEB注文可能土曜"))
            {
                WEB注文可能土曜 = dr["WEB注文可能土曜", version] as bool?;
            }

            base.FromDataRow(dr, version);

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
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

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set
            {
                _商品ＣＤ = value;
                _errors.Remove("商品ＣＤ");
                if (value == null)
                {
                    _errors["商品ＣＤ"] = "商品ＣＤを入力してください。";
                }
                else
                {
                    if (_stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Syohin syohin = Syohin.GetSyohin((int)_商品ＣＤ);
                        if (syohin != null)
                        {
                            商品名略称 = syohin.商品名略称;
                        }
                        else
                        {
                            _errors["商品ＣＤ"] = "商品ＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public int? 商品ＣＤorg
        {
            get { return _商品ＣＤorg; }
            set { _商品ＣＤorg = value; }
        }

        public int? 商品グループＣＤ
        {
            get { return _商品グループＣＤ; }
            set
            {
                _商品グループＣＤ = value;
                _errors.Remove("商品グループＣＤ");
                if (value == null || value < 0)
                {
                    _errors["商品グループＣＤ"] = "商品グループＣＤを入力してください。";
                }
            }
        }

        public bool? 主要商品フラグ
        {
            get { return _主要商品フラグ; }
            set { _主要商品フラグ = value; }
        }

        public bool? WEB注文不可フラグ
        {
            get { return _WEB注文不可フラグ; }
            set { _WEB注文不可フラグ = value; }
        }

        public int? WEB注文可能期限日数
        {
            get { return _WEB注文可能期限日数; }
            set
            {
                if (value < 0)
                {
                    _WEB注文可能期限日数 = null;
                }
                else
                {
                    _WEB注文可能期限日数 = value;
                }
            }
        }

        public string WEB注文可能曜日
        {
            get { return _WEB注文可能曜日; }
            set { _WEB注文可能曜日 = value; }
        }

        public DateTime? WEB注文開始日
        {
            get { return _WEB注文開始日; }
            set { _WEB注文開始日 = value; }
        }

        public DateTime? WEB注文終了日
        {
            get { return _WEB注文終了日; }
            set { _WEB注文終了日 = value; }
        }

        public int? WEB注文締切時間
        {
            get { return _WEB注文締切時間; }
            set
            {
                if (value < 0)
                {
                    _WEB注文締切時間 = null;
                }
                else
                {
                    _WEB注文締切時間 = value;
                }
            }
        }

        public int? WEB注文締切分
        {
            get { return _WEB注文締切分; }
            set
            {
                if (value < 0)
                {
                    _WEB注文締切分 = null;
                }
                else
                {
                    _WEB注文締切分 = value;
                }
            }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public bool? WEB注文可能日祝
        {
            get
            {
                return GetOrderableWeekday(DayOfWeek.Sunday);
            }
            set
            {
                SetOrderableWeekday(DayOfWeek.Sunday, value);
            }
        }

        public bool? WEB注文可能月曜
        {
            get
            {
                return GetOrderableWeekday(DayOfWeek.Monday);
            }
            set
            {
                SetOrderableWeekday(DayOfWeek.Monday, value);
            }
        }

        public bool? WEB注文可能火曜
        {
            get
            {
                return GetOrderableWeekday(DayOfWeek.Tuesday);
            }
            set
            {
                SetOrderableWeekday(DayOfWeek.Tuesday, value);
            }
        }

        public bool? WEB注文可能水曜
        {
            get
            {
                return GetOrderableWeekday(DayOfWeek.Wednesday);
            }
            set
            {
                SetOrderableWeekday(DayOfWeek.Wednesday, value);
            }
        }

        public bool? WEB注文可能木曜
        {
            get
            {
                return GetOrderableWeekday(DayOfWeek.Thursday);
            }
            set
            {
                SetOrderableWeekday(DayOfWeek.Thursday, value);
            }
        }

        public bool? WEB注文可能金曜
        {
            get
            {
                return GetOrderableWeekday(DayOfWeek.Friday);
            }
            set
            {
                SetOrderableWeekday(DayOfWeek.Friday, value);
            }
        }

        public bool? WEB注文可能土曜
        {
            get
            {
                return GetOrderableWeekday(DayOfWeek.Saturday);
            }
            set
            {
                SetOrderableWeekday(DayOfWeek.Saturday, value);
            }
        }

        public string 商品名略称
        {
            get { return _商品名略称; }
            set { _商品名略称 = value; }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] 商品ＣＤ=[{1}]", 部署ＣＤ, 商品ＣＤ); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        public bool HasLimit
        {
            get { return (WEB注文締切時間 != null || WEB注文締切分 != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get
            {
                // 注文可能期限日数が設定されている時に、時刻が設定されていない場合はエラー
                if (WEB注文可能期限日数 != null)
                {
                    if (WEB注文締切時間 == null)
                    {
                        _errors["WEB注文締切時間"] = "締切時間を入力してください。";
                        return "締切時間を入力してください。";
                    }

                    if (WEB注文締切分 == null)
                    {
                        _errors["WEB注文締切分"] = "締切分を入力してください。";
                        return "締切分を入力してください。";
                    }

                    // 0時0分締切はエラーとする(注文可能期限日数を「当日」としていた場合でも期限が実質1日前となるため)
                    if (WEB注文締切時間 == 0 && WEB注文締切分 == 0)
                    {
                        _errors["WEB注文締切時間"] = "0時0分以外の時刻を入力してください。";
                        _errors["WEB注文締切分"] = "0時0分以外の時刻を入力してください。";
                        return "0時0分以外の時刻を入力してください。";
                    }
                }
                // 注文可能期限日数が設定されていない時に、時刻が設定されている場合はエラー
                else
                {
                    if (WEB注文締切時間 != null || WEB注文締切分 != null)
                    {
                        _errors["WEB注文可能期限日数"] = "注文可能期限日数を入力してください。";
                        return "注文可能期限日数を入力してください。";
                    }
                }

                // 注文開始日～注文終了日をチェック
                if (WEB注文開始日 != null && WEB注文終了日 != null)
                {
                    if (WEB注文終了日 < WEB注文開始日)
                    {
                        _errors["WEB注文終了日"] = "WEB注文終了日が開始日より過去になっています。";
                        return "WEB注文開始日とWEB注文終了日が逆です。";
                    }
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
        /// WEB注文可能曜日の該当の曜日の値を返す
        /// </summary>
        /// <param name="dayOfWeek">WEB注文可能曜日の曜日インデックス</param>
        /// <returns>注文可能の場合はtrue。それ以外はfalse。</returns>
        public bool GetOrderableWeekday(DayOfWeek dayOfWeek)
        {
            return GetOrderableWeekday(WEB注文可能曜日, dayOfWeek);
        }

        /// <summary>
        /// WEB注文可能曜日の該当の曜日の値を設定する
        /// </summary>
        /// <param name="dayOfWeek">WEB注文可能曜日の曜日インデックス</param>
        /// <param name="value">注文可能の場合はtrue。</param>
        private void SetOrderableWeekday(DayOfWeek dayOfWeek, bool? value)
        {
            if (string.IsNullOrEmpty(WEB注文可能曜日))
            {
                WEB注文可能曜日 = ALL_OFF;
            }
            char[] c = WEB注文可能曜日.ToCharArray();
            int index = (int)dayOfWeek;         // 日:0～土:6
            if (value == true)
            {
                c[index] = '1';
            }
            else
            {
                c[index] = '0';
            }
            WEB注文可能曜日 = string.Join("", c);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(SyuyoSyohin target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                商品ＣＤ == target.商品ＣＤ &&
                (商品グループＣＤ ?? -1) == (target.商品グループＣＤ ?? -1) &&
                (主要商品フラグ ?? false) == (target.主要商品フラグ ?? false) &&
                (WEB注文不可フラグ ?? false) == (target.WEB注文不可フラグ ?? false) &&
                (WEB注文可能期限日数 ?? -1) == (target.WEB注文可能期限日数 ?? -1) &&
                WEB注文可能曜日 == target.WEB注文可能曜日 &&
                WEB注文開始日 == target.WEB注文開始日 &&
                WEB注文終了日 == target.WEB注文終了日 &&
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
            string sql = "\r\n INSERT INTO 主要商品リストマスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,商品グループＣＤ" +
                         "\r\n     ,主要商品フラグ" +
                         "\r\n     ,WEB注文不可フラグ" +
                         "\r\n     ,WEB注文可能期限日数" +
                         "\r\n     ,WEB注文可能曜日" +
                         "\r\n     ,WEB注文開始日" +
                         "\r\n     ,WEB注文終了日" +

                         "\r\n     ,WEB注文締切時間" +
                         "\r\n     ,WEB注文締切分" +

                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@商品グループＣＤ" +
                         "\r\n     ,@主要商品フラグ" +
                         "\r\n     ,@WEB注文不可フラグ" +
                         "\r\n     ,@WEB注文可能期限日数" +
                         "\r\n     ,@WEB注文可能曜日" +
                         "\r\n     ,@WEB注文開始日" +
                         "\r\n     ,@WEB注文終了日" +

                         "\r\n     ,@WEB注文締切時間" +
                         "\r\n     ,@WEB注文締切分" +

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
            string sql = "\r\n UPDATE 主要商品リストマスタ" +
                         "\r\n SET 商品グループＣＤ    = @商品グループＣＤ" +
                         "\r\n    ,商品ＣＤ            = @商品ＣＤ" +
                         "\r\n    ,主要商品フラグ      = @主要商品フラグ" +
                         "\r\n    ,WEB注文不可フラグ   = @WEB注文不可フラグ" +
                         "\r\n    ,WEB注文可能期限日数 = @WEB注文可能期限日数" +
                         "\r\n    ,WEB注文可能曜日     = @WEB注文可能曜日" +
                         "\r\n    ,WEB注文開始日       = @WEB注文開始日" +
                         "\r\n    ,WEB注文終了日       = @WEB注文終了日" +
                         "\r\n    ,WEB注文締切時間     = @WEB注文締切時間" +
                         "\r\n    ,WEB注文締切分       = @WEB注文締切分" +
                         "\r\n    ,無効フラグ          = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ      = @修正担当者ＣＤ" +
                         "\r\n    ,修正日              = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
                         "\r\n   AND 商品ＣＤ = @商品ＣＤorg" +
                         "\r\n   AND 修正日   = @修正日";

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
            string sql = "\r\n DELETE FROM 主要商品リストマスタ" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ     = @部署ＣＤ" +
                         "\r\n   AND 商品ＣＤ     = @商品ＣＤorg" +
                         "\r\n   AND 修正日       = @修正日";

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

                    if (部署ＣＤ != null && 部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = 商品ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤorg", SqlDbType.Int).Value = 商品ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品グループＣＤ != null && 商品グループＣＤ != -1)
                    {
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = 商品グループＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (主要商品フラグ != null)
                    {
                        cmd.Parameters.Add("@主要商品フラグ", SqlDbType.Bit).Value = 主要商品フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@主要商品フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (WEB注文不可フラグ != null)
                    {
                        cmd.Parameters.Add("@WEB注文不可フラグ", SqlDbType.Bit).Value = WEB注文不可フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@WEB注文不可フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (WEB注文可能期限日数 != null && WEB注文可能期限日数 != -1)
                    {
                        cmd.Parameters.Add("@WEB注文可能期限日数", SqlDbType.Int).Value = WEB注文可能期限日数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@WEB注文可能期限日数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (WEB注文可能曜日 != null)
                    {
                        cmd.Parameters.Add("@WEB注文可能曜日", SqlDbType.VarChar).Value = WEB注文可能曜日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@WEB注文可能曜日", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (WEB注文開始日 != null)
                    {
                        cmd.Parameters.Add("@WEB注文開始日", SqlDbType.Date).Value = WEB注文開始日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@WEB注文開始日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (WEB注文終了日 != null)
                    {
                        cmd.Parameters.Add("@WEB注文終了日", SqlDbType.Date).Value = WEB注文終了日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@WEB注文終了日", SqlDbType.Date).Value = DBNull.Value;
                    }


                    if (WEB注文締切時間 != null)
                    {
                        cmd.Parameters.Add("@WEB注文締切時間", SqlDbType.Int).Value = WEB注文締切時間;
                    }
                    else
                    {
                        cmd.Parameters.Add("@WEB注文締切時間", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (WEB注文締切分 != null)
                    {
                        cmd.Parameters.Add("@WEB注文締切分", SqlDbType.Int).Value = WEB注文締切分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@WEB注文締切分", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("主要商品リストマスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "部署ＣＤ または 商品ＣＤが重複しています。";
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
