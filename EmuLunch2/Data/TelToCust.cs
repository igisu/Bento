using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 電話テーブル
    /// </summary>
    public class TelToCust : TableBase, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private string _Tel_TelNo;          // 電話番号
        private int? _Tel_CustNo;           // 得意先CD
        private bool? _Tel_RepFlg;          // 主副区分
        private bool? _Tel_DelFlg;          // 削除フラグ（未使用）
        private string _Tel_Remarks;        // メモ
        private DateTime? _Tel_NewDate;
        private DateTime? _Tel_UpdDate;

        private string _Tel_TelNoORG;       // 電話番号（レコード読込直後の値（更新・削除時に使用する））
        private int? _Tel_CustNoORG;        // 得意先CD（レコード読込直後の値（更新・削除時に使用する））

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TelToCust()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TelToCust(TelToCust src)
        {
            Tel_TelNo = src.Tel_TelNo;
            Tel_CustNo = src.Tel_CustNo;
            Tel_RepFlg = src.Tel_RepFlg;
            Tel_DelFlg = src.Tel_DelFlg;
            Tel_Remarks = src.Tel_Remarks;
            Tel_NewDate = src.Tel_NewDate;
            Tel_UpdDate = src.Tel_UpdDate;

            Tel_TelNoORG = src.Tel_TelNoORG;
            Tel_CustNoORG = src.Tel_CustNoORG;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TelToCust(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            Tel_TelNo = dr["Tel_TelNo", version] as string;
            Tel_CustNo = dr["Tel_CustNo", version] as int?;
            Tel_RepFlg = dr["Tel_RepFlg", version] as bool?;
            Tel_DelFlg = dr["Tel_DelFlg", version] as bool?;
            Tel_Remarks = dr["Tel_Remarks", version] as string;
            Tel_NewDate = dr["Tel_NewDate", version] as DateTime?;
            Tel_UpdDate = dr["Tel_UpdDate", version] as DateTime?;

            if (dr.Table.Columns.Contains("Tel_TelNoORG"))
            {
                Tel_TelNoORG = dr["Tel_TelNoORG", version] as string;
            }
            else
            {
                Tel_TelNoORG = dr["Tel_TelNo", version] as string;
            }

            if (dr.Table.Columns.Contains("Tel_CustNoORG"))
            {
                Tel_CustNoORG = dr["Tel_CustNoORG", version] as int?;
            }
            else
            {
                Tel_CustNoORG = dr["Tel_CustNo", version] as int?;
            }

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string Tel_TelNo
        {
            get { return _Tel_TelNo; }
            set
            {
                _Tel_TelNo = value;
                _errors.Remove("Tel_TelNo");
                if (value == null)
                {
                    _errors["Tel_TelNo"] = "電話番号を入力してください。";
                }
                else
                {
                    if (Regex.IsMatch(value, @"^[0-9]*$") == false)
                    {
                        _errors["Tel_TelNo"] = "電話番号は半角数字で入力してください。";
                    }
                }
            }
        }

        public int? Tel_CustNo
        {
            get { return _Tel_CustNo; }
            set { _Tel_CustNo = value; }
        }

        public bool? Tel_RepFlg
        {
            get { return _Tel_RepFlg; }
            set
            {
                _Tel_RepFlg = value;
                _errors.Remove("Tel_RepFlg");
                if (value == null)
                {
                    _errors["Tel_RepFlg"] = "区分を入力してください。";
                }
            }
        }

        public bool? Tel_DelFlg
        {
            get { return _Tel_DelFlg; }
            set { _Tel_DelFlg = value; }
        }

        public string Tel_Remarks
        {
            get { return _Tel_Remarks; }
            set { _Tel_Remarks = value; }
        }

        public DateTime? Tel_NewDate
        {
            get { return _Tel_NewDate; }
            set { _Tel_NewDate = value; }
        }

        public DateTime? Tel_UpdDate
        {
            get { return _Tel_UpdDate; }
            set { _Tel_UpdDate = value; }
        }

        public string Tel_TelNoORG
        {
            get { return _Tel_TelNoORG; }
            set { _Tel_TelNoORG = value; }
        }

        public int? Tel_CustNoORG
        {
            get { return _Tel_CustNoORG; }
            set { _Tel_CustNoORG = value; }
        }

        public override string Code
        {
            get { return string.Format("Tel_TelNo={0}", Tel_TelNo); }
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
        /// データ行への設定
        /// </summary>
        /// <param name="dr"></param>
        public void SetDataRow(DataRow dr)
        {
            // DataRow への設定は SetField メソッドを使って書くと DBNull を意識しなくて済む
            dr.SetField<string>("Tel_TelNo", _Tel_TelNo);
            dr.SetField<int?>("Tel_CustNo", _Tel_CustNo);
            dr.SetField<bool?>("Tel_RepFlg", _Tel_RepFlg);
            dr.SetField<bool?>("Tel_DelFlg", _Tel_DelFlg);
            dr.SetField<string>("Tel_Remarks", _Tel_Remarks);
            dr.SetField<DateTime?>("Tel_NewDate", _Tel_NewDate);
            dr.SetField<DateTime?>("Tel_UpdDate", _Tel_UpdDate);

            base.ToDataRow(dr);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(TelToCust target)
        {
            if (Tel_TelNo == target.Tel_TelNo &&
                Tel_CustNo == target.Tel_CustNo &&
                (Tel_RepFlg ?? false) == (target.Tel_RepFlg ?? false) &&
                (Tel_DelFlg ?? false) == (target.Tel_DelFlg ?? false) &&
                Tel_Remarks == target.Tel_Remarks)
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
            string sql = "\r\n INSERT INTO C_TelToCust(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,Tel_TelNo" +
                         "\r\n     ,Tel_CustNo" +
                         "\r\n     ,Tel_RepFlg" +
                         "\r\n     ,Tel_DelFlg" +
                         "\r\n     ,Tel_Remarks" +
                         "\r\n     ,Tel_NewDate" +
                         "\r\n     ,Tel_UpdDate" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@Tel_TelNo" +
                         "\r\n     ,@Tel_CustNo" +
                         "\r\n     ,@Tel_RepFlg" +
                         "\r\n     ,@Tel_DelFlg" +
                         "\r\n     ,@Tel_Remarks" +
                         "\r\n     ,getdate()" +
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
            string sql = "\r\n UPDATE C_TelToCust" +
                         "\r\n SET Tel_TelNo    = @Tel_TelNo" +
                         "\r\n     ,Tel_CustNo  = @Tel_CustNo" +
                         "\r\n     ,Tel_RepFlg  = @Tel_RepFlg" +
                         "\r\n     ,Tel_DelFlg  = @Tel_DelFlg" +
                         "\r\n     ,Tel_Remarks = @Tel_Remarks" +
                         "\r\n     ,Tel_UpdDate = getdate()" +
                         "\r\n WHERE 企業ＣＤ    = @企業ＣＤ" +
                         "\r\n   AND Tel_TelNo   = @Tel_TelNoORG" +
                         "\r\n   AND Tel_CustNo  = @Tel_CustNoORG" +
                         "\r\n   AND Tel_UpdDate = @Tel_UpdDate";

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
            string sql = "\r\n DELETE FROM C_TelToCust" +
                         "\r\n WHERE 企業ＣＤ    = @企業ＣＤ" +
                         "\r\n   AND Tel_TelNo   = @Tel_TelNoORG" +
                         "\r\n   AND Tel_CustNo  = @Tel_CustNoORG" +
                         "\r\n   AND Tel_UpdDate = @Tel_UpdDate";

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

                    if (Tel_TelNo != null)
                    {
                        cmd.Parameters.Add("@Tel_TelNo", SqlDbType.VarChar).Value = Tel_TelNo;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Tel_TelNo", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (Tel_CustNo != null)
                    {
                        cmd.Parameters.Add("@Tel_CustNo", SqlDbType.Int).Value = Tel_CustNo;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Tel_CustNo", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (Tel_RepFlg != null)
                    {
                        cmd.Parameters.Add("@Tel_RepFlg", SqlDbType.Bit).Value = Tel_RepFlg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Tel_RepFlg", SqlDbType.Bit).Value = false;
                    }

                    if (Tel_DelFlg != null)
                    {
                        cmd.Parameters.Add("@Tel_DelFlg", SqlDbType.Bit).Value = Tel_DelFlg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Tel_DelFlg", SqlDbType.Bit).Value = false;
                    }

                    if (Tel_Remarks != null)
                    {
                        cmd.Parameters.Add("@Tel_Remarks", SqlDbType.VarChar).Value = Tel_Remarks;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Tel_Remarks", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (Tel_TelNoORG != null)
                    {
                        cmd.Parameters.Add("@Tel_TelNoORG", SqlDbType.VarChar).Value = Tel_TelNoORG;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Tel_TelNoORG", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (Tel_CustNoORG != null)
                    {
                        cmd.Parameters.Add("@Tel_CustNoORG", SqlDbType.Int).Value = Tel_CustNoORG;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Tel_CustNoORG", SqlDbType.Int).Value = DBNull.Value;
                    }

                    // 更新の場合のみ編集日時を設定（新規の場合は設定しない）
                    if (Tel_UpdDate != null)
                    {
                        cmd.Parameters.Add("@Tel_UpdDate", SqlDbType.DateTime).Value = Tel_UpdDate;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("電話テーブルを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "キーが重複しています。";
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
