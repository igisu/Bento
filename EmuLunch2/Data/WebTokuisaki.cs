using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web得意先マスタ
    /// </summary>
    public class WebTokuisaki : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// パスワードの使用可能な文字
        /// </summary>
        private const string PW_BASE = "!$*-./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <returns></returns>
        public static WebTokuisaki GetWebTokuisaki(int tokuisakiCD)
        {
            return GetWebTokuisaki(tokuisakiCD, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <returns></returns>
        public static WebTokuisaki GetWebTokuisakiReload(int tokuisakiCD)
        {
            return GetWebTokuisaki(tokuisakiCD, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static WebTokuisaki GetWebTokuisaki(int tokuisakiCD, bool isValid)
        {
            WebTokuisaki webTokuisaki = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.得意先ＣＤ" +
                             "\r\n     ,B.得意先名" +
                             "\r\n     ,A.ログインＰＷ" +
                             "\r\n     ,A.メールアドレス" +
                             "\r\n     ,A.個別注文締切時間" +
                             "\r\n     ,A.個別注文締切分" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     WEB_M_得意先 AS A" +
                             "\r\n     LEFT JOIN 得意先マスタ AS B ON B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.得意先ＣＤ = B.得意先ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
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
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("WEB_M_得意先");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    webTokuisaki = new WebTokuisaki(dt.Rows[0]);
                }
            }

            return webTokuisaki;
        }

        /// <summary>
        /// 任意の長さのランダムな文字列を返します。
        /// </summary>
        /// <param name="size">返す文字列の長さ。</param>
        /// <returns>指定した長さを持つランダムな文字列。</returns>
        public static string GeneratePassword(int size)
        {
            StringBuilder salt = new StringBuilder();
            byte[] randomNumber = new byte[size * 4];   // 有効な範囲に収まる値の出現確率を高め、乱数値の取得回数を少なくしたい
            RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();

            // 均等分布しなくなるような値を除外
            int max = byte.MaxValue;
            int remainder = max % PW_BASE.Length;
            int scope = max - remainder;        // 取得する文字の偏りをなすくため、範囲を限定する
            int cnt = 0;
            int index = size;
            int rand;
            while (cnt < size)
            {
                // 乱数値を取得
                if (index >= size)
                {
                    gen.GetBytes(randomNumber);
                    index = 0;
                }
                rand = (int)randomNumber[index];
                index++;

                // 有効な範囲？
                if (rand < scope)
                {
                    // 文字列のインデックスに位置する文字を取得
                    int baseIndex = rand % PW_BASE.Length;
                    salt.Append(PW_BASE.Substring(baseIndex, 1));
                    cnt++;
                }
            }
            return salt.ToString();
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;            // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _得意先ＣＤ;
        private string _ログインＰＷ;
        private string _メールアドレス;
        private int? _個別注文締切時間;
        private int? _個別注文締切分;
        private bool? _無効フラグ;

        private string _得意先名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebTokuisaki()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public WebTokuisaki(WebTokuisaki src)
        {
            _stopOverwrite = true;         // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            得意先ＣＤ = src.得意先ＣＤ;
            ログインＰＷ = src.ログインＰＷ;
            メールアドレス = src.メールアドレス;
            個別注文締切時間 = src.個別注文締切時間;
            個別注文締切分 = src.個別注文締切分;
            無効フラグ = src.無効フラグ;

            得意先名 = src.得意先名;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public WebTokuisaki(DataRow dr)
        {
            _stopOverwrite = true;         // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            ログインＰＷ = dr["ログインＰＷ", version] as string;
            メールアドレス = dr["メールアドレス", version] as string;
            個別注文締切時間 = dr["個別注文締切時間", version] as int?;
            個別注文締切分 = dr["個別注文締切分", version] as int?;
            無効フラグ = dr["無効フラグ", version] as bool?;

            if (dr.Table.Columns.Contains("得意先名"))
            {
                得意先名 = dr["得意先名", version] as string;
            }

            base.FromDataRow(dr, version);

            _stopOverwrite = false;
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
                得意先名 = null;
                if (value == null)
                {
                    _errors["得意先ＣＤ"] = "得意先ＣＤを入力してください。";
                }
                else
                {
                    if (_stopOverwrite == false)
                    {
                        // 得意先ＣＤに該当する得意先名を取得
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            得意先名 = tokuisaki.得意先名;
                        }
                        else
                        {
                            _errors["得意先ＣＤ"] = "得意先ＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public string ログインＰＷ
        {
            get { return _ログインＰＷ; }
            set
            {
                _ログインＰＷ = value;
                _errors.Remove("ログインＰＷ");
                if (value != null)
                {
                    foreach (char c in value)
                    {
                        if (PW_BASE.Contains(c.ToString()) == false)
                        {
                            _errors["ログインＰＷ"] = "パスワードに使えない文字が含まれています。";
                        }
                    }
                }
            }
        }

        public string メールアドレス
        {
            get { return _メールアドレス; }
            set
            {
                _メールアドレス = value;
                _errors.Remove("メールアドレス");
                if (value != null)
                {
                    // http://dobon.net/vb/dotnet/internet/validatemailaddress.html#regex
                    // if (Regex.IsMatch(value,
                    //      @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z",
                    //     RegexOptions.IgnoreCase) == false)
                    // https://qiita.com/sakuro/items/1eaa307609ceaaf51123 をアレンジ
                    if (Regex.IsMatch(value,
                                      @"\A[A-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[A-Z0-9-]+(?:\.[A-Z0-9-]+)+\z",    // メールアドレス
                                      RegexOptions.IgnoreCase) == false)                                    // 大文字小文字を区別しない
                    {
                        _errors["メールアドレス"] = "メールアドレスが正しい形式ではありません。";
                    }
                }
            }
        }

        public int? 個別注文締切時間
        {
            get { return _個別注文締切時間; }
            set
            {
                _errors.Remove("個別注文締切時間");
                _個別注文締切時間 = value;
            }
        }

        public int? 個別注文締切分
        {
            get { return _個別注文締切分; }
            set
            {
                _個別注文締切分 = value;

                // 時も分も未選択なら、時のエラーをクリアする
                if (_個別注文締切分 == null || _個別注文締切分 < 0)
                {
                    if (_個別注文締切時間 == null || _個別注文締切時間 < 0)
                    {
                        _errors.Remove("個別注文締切時間");
                    }
                }
            }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }


        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public override string Code
        {
            get { return string.Format("得意先ＣＤ=[{0}]", 得意先ＣＤ); }
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
                // 個別注文締切分が入力されており、個別注文締切時間が未入力はエラーとする
                // ※個別注文締切時間が入力されており、個別注文締切分が未入力はOK(0分とみなす)(★)
                _errors.Remove("個別注文締切時間");
                if (_個別注文締切分 != null && _個別注文締切分 != -1)
                {
                    if (_個別注文締切時間 == null || _個別注文締切時間 < 0)
                    {
                        _errors["個別注文締切時間"] = "締切時間を入力してください。";
                        return "締切時間を入力してください。";
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
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(WebTokuisaki target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                ログインＰＷ == target.ログインＰＷ &&
                メールアドレス == target.メールアドレス &&
                個別注文締切時間 == target.個別注文締切時間 &&
                個別注文締切分 == target.個別注文締切分 &&
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
            string sql = "\r\n INSERT INTO WEB_M_得意先(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,ログインＰＷ" +
                         "\r\n     ,メールアドレス" +
                         "\r\n     ,個別注文締切時間" +
                         "\r\n     ,個別注文締切分" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@ログインＰＷ" +
                         "\r\n     ,@メールアドレス" +
                         "\r\n     ,@個別注文締切時間" +
                         "\r\n     ,@個別注文締切分" +
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
            string sql = "\r\n UPDATE WEB_M_得意先" +
                         "\r\n SET ログインＰＷ     = @ログインＰＷ" +
                         "\r\n    ,メールアドレス   = @メールアドレス" +
                         "\r\n    ,個別注文締切時間 = @個別注文締切時間" +
                         "\r\n    ,個別注文締切分   = @個別注文締切分" +
                         "\r\n    ,無効フラグ       = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ   = @修正担当者ＣＤ" +
                         "\r\n    ,修正日           = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
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
            string sql = "\r\n DELETE FROM WEB_M_得意先" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
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

                    if (ログインＰＷ != null)
                    {
                        cmd.Parameters.Add("@ログインＰＷ", SqlDbType.VarChar).Value = ログインＰＷ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ログインＰＷ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (メールアドレス != null)
                    {
                        cmd.Parameters.Add("@メールアドレス", SqlDbType.VarChar).Value = メールアドレス;
                    }
                    else
                    {
                        cmd.Parameters.Add("@メールアドレス", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (個別注文締切時間 != null && 個別注文締切時間 != -1)
                    {
                        cmd.Parameters.Add("@個別注文締切時間", SqlDbType.Int).Value = 個別注文締切時間;

                        if (個別注文締切分 != null && 個別注文締切分 != -1)
                        {
                            cmd.Parameters.Add("@個別注文締切分", SqlDbType.Int).Value = 個別注文締切分;
                        }
                        else
                        {
                            cmd.Parameters.Add("@個別注文締切分", SqlDbType.Int).Value = 0;     // (★)
                        }
                    }
                    else
                    {
                        cmd.Parameters.Add("@個別注文締切時間", SqlDbType.Int).Value = DBNull.Value;
                        cmd.Parameters.Add("@個別注文締切分", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("Web得意先マスタを{0}しました。", historyProcess.DisplayName());
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
