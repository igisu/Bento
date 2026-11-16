using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 部署画像マスタ
    /// </summary>
    public class BusyoImage : TableBase2
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <returns></returns>
        public static BusyoImage GetBusyoImage(int busyoCd)
        {
            BusyoImage busyoImage = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ" +
                                  "\r\n     ,A.ロゴ" +
                                  "\r\n     ,A.印影" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者CD" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     部署画像マスタ AS A" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND A.部署ＣＤ = @部署ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCd;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("部署画像マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    busyoImage = new BusyoImage(dt.Rows[0]);
                }
            }

            return busyoImage;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署ＣＤ;
        private byte[] _ロゴ;
        private byte[] _印影;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BusyoImage()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public BusyoImage(BusyoImage src)
        {
            部署ＣＤ = src.部署ＣＤ;
            ロゴ = src.ロゴ;
            印影 = src.印影;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public BusyoImage(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ"] as int?;
            ロゴ = dr["ロゴ"] as byte[];
            印影 = dr["印影"] as byte[];

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
            set { _部署ＣＤ = value; }
        }

        public byte[] ロゴ
        {
            get { return _ロゴ; }
            set { _ロゴ = value; }
        }

        public byte[] 印影
        {
            get { return _印影; }
            set { _印影 = value; }
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
        /// ロゴ画像イメージを返します
        /// </summary>
        /// <returns></returns>
        public Image GetLogoImage()
        {
            if ((_ロゴ == null))
            {
                return null;
            }
            else
            {
                ImageConverter imgconv = new ImageConverter();
                return (Image)imgconv.ConvertFrom(_ロゴ);
            }
        }

        /// <summary>
        /// ロゴ画像イメージを設定します（バイト配列を更新します）
        /// </summary>
        /// <param name="image"></param>
        public void SetLogoImage(Image image)
        {
            if (image == null)
            {
                _ロゴ = null;
            }
            else
            {
                ImageConverter imgconv = new ImageConverter();
                _ロゴ = (byte[])imgconv.ConvertTo(image, typeof(byte[]));
            }
        }

        /// <summary>
        /// 印影画像イメージを返します
        /// </summary>
        /// <returns></returns>
        public Image GetInkanImage()
        {
            if (_印影 == null)
            {
                return null;
            }
            else
            {
                ImageConverter imgconv = new ImageConverter();
                return (Image)imgconv.ConvertFrom(_印影);
            }
        }

        /// <summary>
        /// 印影画像イメージを設定します（バイト配列を更新します）
        /// </summary>
        /// <param name="image"></param>
        public void SetInkanImage(Image image)
        {
            if (image == null)
            {
                _印影 = null;
            }
            else
            {
                ImageConverter imgconv = new ImageConverter();
                _印影 = (byte[])imgconv.ConvertTo(image, typeof(byte[]));
            }
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(BusyoImage target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                ロゴ == target.ロゴ &&
                印影 == target.印影)
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
            string sql = "\r\n INSERT INTO 部署画像マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,ロゴ" +
                         "\r\n     ,印影" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@ロゴ" +
                         "\r\n     ,@印影" +
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
            string sql = "\r\n UPDATE 部署画像マスタ" +
                         "\r\n SET ロゴ           = @ロゴ" +
                         "\r\n    ,印影           = @印影" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ     = @部署ＣＤ" +
                         "\r\n   AND 修正日       = @修正日";

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
            string sql = "\r\n DELETE FROM 部署画像マスタ" +
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

                    if (ロゴ != null)
                    {
                        cmd.Parameters.Add("@ロゴ", SqlDbType.VarBinary).Value = ロゴ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ロゴ", SqlDbType.VarBinary).Value = DBNull.Value;
                    }

                    if (印影 != null)
                    {
                        cmd.Parameters.Add("@印影", SqlDbType.VarBinary).Value = 印影;
                    }
                    else
                    {
                        cmd.Parameters.Add("@印影", SqlDbType.VarBinary).Value = DBNull.Value;
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

                    log.InfoFormat("部署画像マスタを{0}しました。", historyProcess.DisplayName());
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
