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
    /// 短冊設定マスタ
    /// </summary>
    public class TanzakuSettei : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns></returns>
        public static TanzakuSettei GetTanzakuSettei(int? busyoCD)
        {
            TanzakuSettei tanzakuSettei = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ" +
                                  "\r\n     ,B.部署名" +
                                  "\r\n     ,A.袋パターンテキスト" +
                                  "\r\n     ,A.シール送付先" +
                                  "\r\n     ,A.メールアドレス" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     短冊設定マスタ AS A" +
                                  "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND A.部署ＣＤ = B.部署ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND A.部署ＣＤ = @部署ＣＤ";


                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD ?? -1;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("短冊設定マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    tanzakuSettei = new TanzakuSettei(dt.Rows[0]);
                }
            }

            return tanzakuSettei;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署ＣＤ;
        private string _袋パターンテキスト;
        private string _シール送付先;
        private string _メールアドレス;

        private string _部署名;

        private List<int> _袋パターンリスト = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TanzakuSettei()
        {
            袋パターンテキスト = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TanzakuSettei(TanzakuSettei src)
        {
            部署ＣＤ = src.部署ＣＤ;
            袋パターンテキスト = src.袋パターンテキスト;
            シール送付先 = src.シール送付先;
            メールアドレス = src.メールアドレス;

            部署名 = src.部署名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TanzakuSettei(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            袋パターンテキスト = dr["袋パターンテキスト", version] as string;
            シール送付先 = dr["シール送付先", version] as string;
            メールアドレス = dr["メールアドレス", version] as string;

            if (dr.Table.Columns.Contains("部署名"))
            {
                部署名 = dr["部署名", version] as string;
            }

            base.FromDataRow(dr, version);
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
                if (value != null)
                {
                    // 部署ＣＤに該当する部署名を取得
                    Busyo busyo = Busyo.GetBusyo((int)_部署ＣＤ);
                    if (busyo != null)
                    {
                        部署名 = busyo.部署名;
                    }
                }
            }
        }

        public string 袋パターンテキスト
        {
            get { return _袋パターンテキスト; }
            set
            {
                _袋パターンテキスト = value;
                _errors.Remove("袋パターンテキスト");
                if (value == null)
                {
                    _errors["袋パターンテキスト"] = "袋パターンを入力してください。";
                }
                else
                {
                    // カンマ区切りであること（0が含まれないことも同時に確認している）
                    if (Regex.IsMatch(value, @"^([1-9][0-9]*)(,[1-9][0-9]*)*$") == false)
                    {
                        _errors["袋パターンテキスト"] = "袋パターンを「0」を含めずにカンマ区切りで入力してください。";
                    }
                }
            }
        }

        public string シール送付先
        {
            get { return _シール送付先; }
            set { _シール送付先 = value; }
        }

        public string メールアドレス
        {
            get { return _メールアドレス; }
            set { _メールアドレス = value; }
        }


        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public List<int> 袋パターンリスト
        {
            get
            {
                if (_袋パターンリスト == null)
                {
                    _袋パターンリスト = new List<int>();
                    // カンマ区切り文字列を分割
                    string[] tokens = 袋パターンテキスト.Split(',');
                    foreach (string s in tokens)
                    {
                        // 数値だけを取り出してリストに追加
                        int i;
                        if (int.TryParse(s, out i))
                        {
                            _袋パターンリスト.Add(i);
                        }
                    }
                }

                // リストを降順にソート
                _袋パターンリスト.Sort((a, b) => b - a);

                return _袋パターンリスト;
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
        public bool Equals(TanzakuSettei target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                袋パターンテキスト == target.袋パターンテキスト &&
                シール送付先 == target.シール送付先 &&
                メールアドレス == target.メールアドレス)
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
            string sql = "\r\n INSERT INTO 短冊設定マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,袋パターンテキスト" +
                         "\r\n     ,シール送付先" +
                         "\r\n     ,メールアドレス" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@袋パターンテキスト" +
                         "\r\n     ,@シール送付先" +
                         "\r\n     ,@メールアドレス" +
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
            string sql = "\r\n UPDATE 短冊設定マスタ" +
                         "\r\n SET 袋パターンテキスト = @袋パターンテキスト" +
                         "\r\n    ,シール送付先       = @シール送付先" +
                         "\r\n    ,メールアドレス     = @メールアドレス" +
                         "\r\n    ,修正担当者ＣＤ     = @修正担当者ＣＤ" +
                         "\r\n    ,修正日             = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
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
            string sql = "\r\n DELETE FROM 短冊設定マスタ" +
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

                    if (袋パターンテキスト != null)
                    {
                        cmd.Parameters.Add("@袋パターンテキスト", SqlDbType.VarChar).Value = 袋パターンテキスト;
                    }
                    else
                    {
                        cmd.Parameters.Add("@袋パターンテキスト", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (シール送付先 != null)
                    {
                        cmd.Parameters.Add("@シール送付先", SqlDbType.VarChar).Value = シール送付先;
                    }
                    else
                    {
                        cmd.Parameters.Add("@シール送付先", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (メールアドレス != null)
                    {
                        cmd.Parameters.Add("@メールアドレス", SqlDbType.VarChar).Value = メールアドレス;
                    }
                    else
                    {
                        cmd.Parameters.Add("@メールアドレス", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("短冊設定マスタを{0}しました。", historyProcess.DisplayName());
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
