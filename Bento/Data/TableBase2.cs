using System;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// ƒe[ƒuƒ‹Šî–{ƒNƒ‰ƒX‚Q
    /// </summary>
    public abstract class TableBase2 : TableBase
    {

        //**************************************
        //* ƒCƒ“ƒXƒ^ƒ“ƒX•Ï”
        //**************************************
        #region ƒCƒ“ƒXƒ^ƒ“ƒX•Ï”

        /// <summary>
        /// ì¬’S“–Ò‚b‚c
        /// </summary>
        protected int? _ì¬’S“–Ò‚b‚c;

        /// <summary>
        /// ì¬’S“–Ò–¼
        /// </summary>
        protected string _ì¬’S“–Ò–¼;

        /// <summary>
        /// ì¬“ú
        /// </summary>
        protected DateTime? _ì¬“ú;

        /// <summary>
        /// C³’S“–Ò‚b‚c
        /// </summary>
        protected int? _C³’S“–Ò‚b‚c;

        /// <summary>
        /// C³’S“–Ò–¼
        /// </summary>
        protected string _C³’S“–Ò–¼;

        /// <summary>
        /// C³“ú
        /// </summary>
        protected DateTime? _C³“ú;

        #endregion

        //**************************************
        //* ƒvƒƒpƒeƒB
        //**************************************
        #region ƒvƒƒpƒeƒB

        /// <summary>
        /// ì¬’S“–Ò‚b‚c
        /// </summary>
        public int? ì¬’S“–Ò‚b‚c
        {
            get
            {
                return _ì¬’S“–Ò‚b‚c;
            }
            set
            {
                _ì¬’S“–Ò‚b‚c = value;
            }
        }

        /// <summary>
        /// ì¬’S“–Ò–¼
        /// </summary>
        public string ì¬’S“–Ò–¼
        {
            get
            {
                return _ì¬’S“–Ò–¼;
            }
            set
            {
                _ì¬’S“–Ò–¼ = value;
            }
        }

        /// <summary>
        /// ì¬“ú
        /// </summary>
        public DateTime? ì¬“ú
        {
            get
            {
                return _ì¬“ú;
            }
            set
            {
                _ì¬“ú = value;
            }
        }

        /// <summary>
        /// C³’S“–Ò‚b‚c
        /// </summary>
        public int? C³’S“–Ò‚b‚c
        {
            get
            {
                return _C³’S“–Ò‚b‚c;
            }
            set
            {
                _C³’S“–Ò‚b‚c = value;
            }
        }

        /// <summary>
        /// C³’S“–Ò–¼
        /// </summary>
        public string C³’S“–Ò–¼
        {
            get
            {
                return _C³’S“–Ò–¼;
            }
            set
            {
                _C³’S“–Ò–¼ = value;
            }
        }

        /// <summary>
        /// C³“ú
        /// </summary>
        public DateTime? C³“ú
        {
            get
            {
                return _C³“ú;
            }
            set
            {
                _C³“ú = value;
            }
        }

        #endregion

        //**************************************
        //* ƒƒ\ƒbƒh
        //**************************************
        #region ƒƒ\ƒbƒh
        /// <summary>
        /// ƒRƒs[
        /// </summary>
        /// <param name="src"></param>
        protected void Copy(TableBase2 src)
        {
            base.Copy(src);
            ì¬’S“–Ò‚b‚c = src.ì¬’S“–Ò‚b‚c;
            ì¬’S“–Ò–¼ = src.ì¬’S“–Ò–¼;
            ì¬“ú = src.ì¬“ú;

            C³’S“–Ò‚b‚c = src.C³’S“–Ò‚b‚c;
            C³’S“–Ò–¼ = src.C³’S“–Ò–¼;
            C³“ú = src.C³“ú;
        }

        /// <summary>
        /// ƒf[ƒ^s‚©‚ç¶¬
        /// </summary>
        /// <param name="dr"></param>
        protected new void FromDataRow(DataRow dr)
        {
            FromDataRow(dr, DataRowVersion.Default);
        }

        /// <summary>
        /// ƒf[ƒ^s‚©‚ç¶¬
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="version"></param>
        protected new void FromDataRow(DataRow dr, DataRowVersion version)
        {
            base.FromDataRow(dr, version);
            ì¬’S“–Ò‚b‚c = dr["ì¬’S“–Ò‚b‚c", version] as int?;
            if (dr.Table.Columns.Contains("ì¬’S“–Ò–¼"))
            {
                ì¬’S“–Ò–¼ = dr["ì¬’S“–Ò–¼", version] as string;
            }
            ì¬“ú = dr["ì¬“ú", version] as DateTime?;

            C³’S“–Ò‚b‚c = dr["C³’S“–Ò‚b‚c", version] as int?;
            if (dr.Table.Columns.Contains("C³’S“–Ò–¼"))
            {
                C³’S“–Ò–¼ = dr["C³’S“–Ò–¼", version] as string;
            }
            C³“ú = dr["C³“ú", version] as DateTime?;
        }

        /// <summary>
        /// ƒf[ƒ^s‚Öo—Í
        /// </summary>
        /// <param name="dr"></param>
        protected new void ToDataRow(DataRow dr)
        {
            base.ToDataRow(dr);
            dr.SetField<int?>("ì¬’S“–Ò‚b‚c", _ì¬’S“–Ò‚b‚c);
            // ì¬’S“–Ò–¼‚Íƒf[ƒ^s‚Ö‚Ìo—Í‚Í‚µ‚È‚¢
            dr.SetField<DateTime?>("ì¬“ú", _ì¬“ú);

            dr.SetField<int?>("C³’S“–Ò‚b‚c", _C³’S“–Ò‚b‚c);
            // C³’S“–Ò–¼‚Íƒf[ƒ^s‚Ö‚Ìo—Í‚Í‚µ‚È‚¢
            dr.SetField<DateTime?>("C³“ú", _C³“ú);
        }

        /// <summary>
        /// DB“o˜^
        /// </summary>
        /// <returns>³íI—¹‚Ìê‡‚Í trueB‚»‚êˆÈŠO‚Í false</returns>
        public bool Insert()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    return Insert(con);
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

        /// <summary>
        /// DB“o˜^
        /// </summary>
        /// <param name="con">DBÚ‘±</param>
        /// <returns>³íI—¹‚Ìê‡‚Í trueB‚»‚êˆÈŠO‚Í false</returns>
        public bool Insert(SqlConnection con)
        {
            try
            {
                SqlTransaction tran = con.BeginTransaction();
                bool ret = Insert(con, tran);
                if (ret)
                {
                    tran.Commit();
                }
                return ret;
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

        /// <summary>
        /// DBXV
        /// </summary>
        /// <returns>³íI—¹‚Ìê‡‚Í trueB‚»‚êˆÈŠO‚Í false</returns>
        public bool Update()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    return Update(con);
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

        /// <summary>
        /// DBXV
        /// </summary>
        /// <param name="con">DBÚ‘±</param>
        /// <returns>³íI—¹‚Ìê‡‚Í trueB‚»‚êˆÈŠO‚Í false</returns>
        public bool Update(SqlConnection con)
        {
            try
            {
                SqlTransaction tran = con.BeginTransaction();
                bool ret = Update(con, tran);
                if (ret)
                {
                    tran.Commit();
                }
                return ret;
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

        /// <summary>
        /// DBíœ
        /// </summary>
        /// <returns>³íI—¹‚Ìê‡‚Í trueB‚»‚êˆÈŠO‚Í false</returns>
        public bool Delete()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    return Delete(con);
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

        /// <summary>
        /// DBíœ
        /// </summary>
        /// <param name="con">DBÚ‘±</param>
        /// <returns>³íI—¹‚Ìê‡‚Í trueB‚»‚êˆÈŠO‚Í false</returns>
        public bool Delete(SqlConnection con)
        {
            try
            {
                SqlTransaction tran = con.BeginTransaction();
                bool ret = Delete(con, tran);
                if (ret)
                {
                    tran.Commit();
                }
                return ret;
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
