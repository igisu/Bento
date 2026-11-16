using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 仕訳マスタ
    /// </summary>
    public class Shiwake : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// 借方／貸方補助ＣＤのコンボボックスの選択肢
        /// </summary>
        public static readonly List<string> HOJO_OPTION = new List<string>() { "[部署ＣＤ]", "[集計ＣＤ]", "[借方部門ＣＤ]", "[貸方部門ＣＤ]" };
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="shiwakeCD">仕訳ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        public static Shiwake GetShiwake(int busyoCD, int shiwakeCD)
        {
            Shiwake shiwake = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,A.仕訳ＣＤ" +
                             "\r\n     ,A.集計ＣＤ" +
                             "\r\n     ,A.集計対象区分" +
                             "\r\n     ,A.売上区分" +
                             "\r\n     ,A.チケット束フラグ" +
                             "\r\n     ,A.入金方法" +
                             "\r\n     ,A.集計ＣＤ使用フラグ" +
                             "\r\n     ,A.処理内容" +
                             "\r\n     ,A.借方ＣＤ" +
                             "\r\n     ,A.借方部門ＣＤ" +
                             "\r\n     ,A.借方補助ＣＤ" +
                             "\r\n     ,A.貸方ＣＤ" +
                             "\r\n     ,A.貸方部門ＣＤ" +
                             "\r\n     ,A.貸方補助ＣＤ" +
                             "\r\n     ,A.摘要" +
                             "\r\n     ,A.税率" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     仕訳マスタ AS A" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.部署ＣＤ = @部署ＣＤ" +
                             "\r\n AND A.仕訳ＣＤ = @仕訳ＣＤ";

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;
                cmd.Parameters.Add("@仕訳ＣＤ", SqlDbType.Int).Value = shiwakeCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("仕訳マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    shiwake = new Shiwake(dt.Rows[0]);
                }
            }

            return shiwake;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署ＣＤ;
        private int? _仕訳ＣＤ;
        private int? _集計ＣＤ;
        private int? _集計対象区分;
        private int? _売上区分;
        private bool? _チケット束フラグ;
        private int? _入金方法;
        private bool? _集計ＣＤ使用フラグ;
        private string _処理内容;
        private string _借方ＣＤ;
        private string _借方部門ＣＤ;
        private string _借方補助ＣＤ;
        private string _貸方ＣＤ;
        private string _貸方部門ＣＤ;
        private string _貸方補助ＣＤ;
        private string _摘要;
        private string _税率;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Shiwake()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Shiwake(Shiwake src)
        {
            部署ＣＤ = src.部署ＣＤ;
            仕訳ＣＤ = src.仕訳ＣＤ;
            集計ＣＤ = src.集計ＣＤ;
            集計対象区分 = src.集計対象区分;
            売上区分 = src.売上区分;
            チケット束フラグ = src.チケット束フラグ;
            入金方法 = src.入金方法;
            集計ＣＤ使用フラグ = src.集計ＣＤ使用フラグ;
            処理内容 = src.処理内容;
            借方ＣＤ = src.借方ＣＤ;
            借方部門ＣＤ = src.借方部門ＣＤ;
            借方補助ＣＤ = src.借方補助ＣＤ;
            貸方ＣＤ = src.貸方ＣＤ;
            貸方部門ＣＤ = src.貸方部門ＣＤ;
            貸方補助ＣＤ = src.貸方補助ＣＤ;
            摘要 = src.摘要;
            税率 = src.税率;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Shiwake(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            仕訳ＣＤ = dr["仕訳ＣＤ", version] as int?;
            集計ＣＤ = dr["集計ＣＤ", version] as int?;
            集計対象区分 = dr["集計対象区分", version] as int?;
            売上区分 = dr["売上区分", version] as int?;
            チケット束フラグ = dr["チケット束フラグ", version] as bool?;
            入金方法 = dr["入金方法", version] as int?;
            集計ＣＤ使用フラグ = dr["集計ＣＤ使用フラグ", version] as bool?;
            処理内容 = dr["処理内容", version] as string;
            借方ＣＤ = dr["借方ＣＤ", version] as string;
            借方部門ＣＤ = dr["借方部門ＣＤ", version] as string;
            借方補助ＣＤ = dr["借方補助ＣＤ", version] as string;
            貸方ＣＤ = dr["貸方ＣＤ", version] as string;
            貸方部門ＣＤ = dr["貸方部門ＣＤ", version] as string;
            貸方補助ＣＤ = dr["貸方補助ＣＤ", version] as string;
            摘要 = dr["摘要", version] as string;
            税率 = dr["税率", version] as string;

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
                _errors.Remove("部署ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
            }
        }

        public int? 仕訳ＣＤ
        {
            get { return _仕訳ＣＤ; }
            set
            {
                _仕訳ＣＤ = value;
                _errors.Remove("仕訳ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["仕訳ＣＤ"] = "仕訳ＣＤを入力してください。";
                }
            }
        }

        public int? 集計ＣＤ
        {
            get { return _集計ＣＤ; }
            set
            {
                _集計ＣＤ = value;
                _errors.Remove("集計ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["集計ＣＤ"] = "集計ＣＤを入力してください。";
                }
            }
        }

        public int? 集計対象区分
        {
            get { return _集計対象区分; }
            set
            {
                _集計対象区分 = value;
                _errors.Remove("集計対象区分");
                if (value == null || value < 0)
                {
                    _errors["集計対象区分"] = "集計対象区分を入力してください。";
                }
            }
        }

        public int? 売上区分
        {
            get { return _売上区分; }
            set { _売上区分 = value; }
        }

        public bool? チケット束フラグ
        {
            get { return _チケット束フラグ; }
            set { _チケット束フラグ = value; }
        }

        public int? 入金方法
        {
            get { return _入金方法; }
            set { _入金方法 = value; }
        }

        public bool? 集計ＣＤ使用フラグ
        {
            get { return _集計ＣＤ使用フラグ; }
            set { _集計ＣＤ使用フラグ = value; }
        }

        public string 処理内容
        {
            get { return _処理内容; }
            set
            {
                _処理内容 = value;
                _errors.Remove("処理内容");
                if (value == null)
                {
                    _errors["処理内容"] = "処理内容を入力してください。";
                }
            }
        }

        public string 借方ＣＤ
        {
            get { return _借方ＣＤ; }
            set
            {
                _借方ＣＤ = value;
                _errors.Remove("借方ＣＤ");
                if (value == null)
                {
                    _errors["借方ＣＤ"] = "借方を入力してください。";
                }
            }
        }

        public string 借方部門ＣＤ
        {
            get { return _借方部門ＣＤ; }
            set { _借方部門ＣＤ = value; }
        }

        public string 借方補助ＣＤ
        {
            get { return _借方補助ＣＤ; }
            set
            {
                _借方補助ＣＤ = value;
                _errors.Remove("借方補助ＣＤ");
                // 入力値がコンボボックスの選択肢に無い場合は、"["と"]"で囲んではダメ（ホントは構わないが、紛らわしいので禁止している）
                if (HOJO_OPTION.Contains(value) == false)
                {
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        if (value.StartsWith("[") && value.EndsWith("]"))
                        {
                            _errors["借方補助ＣＤ"] = "選択肢に無い値を入力する場合は、角カッコ（[]）で囲むことはできません。";
                        }
                    }
                }
            }
        }

        public string 貸方ＣＤ
        {
            get { return _貸方ＣＤ; }
            set
            {
                _貸方ＣＤ = value;
                _errors.Remove("貸方ＣＤ");
                if (value == null)
                {
                    _errors["貸方ＣＤ"] = "貸方を入力してください。";
                }
            }
        }

        public string 貸方部門ＣＤ
        {
            get { return _貸方部門ＣＤ; }
            set { _貸方部門ＣＤ = value; }
        }

        public string 貸方補助ＣＤ
        {
            get { return _貸方補助ＣＤ; }
            set
            {
                _貸方補助ＣＤ = value;
                _errors.Remove("貸方補助ＣＤ");
                // 入力値がコンボボックスの選択肢に無い場合は、"["と"]"で囲んではダメ（ホントは構わないが、紛らわしいので禁止している）
                if (HOJO_OPTION.Contains(value) == false)
                {
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        if (value.Contains("[") && value.Contains("]"))
                        {
                            _errors["貸方補助ＣＤ"] = "選択肢に無い値を入力する場合は、角カッコ（[]）で囲むことことはできません。";
                        }
                    }
                }
            }
        }

        public string 摘要
        {
            get { return _摘要; }
            set { _摘要 = value; }
        }

        public string 税率
        {
            get { return _税率; }
            set { _税率 = value; }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] 仕訳ＣＤ=[{1}]", 部署ＣＤ, 仕訳ＣＤ); }
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
                // 集計対象区分が0:売上の場合は、売上区分が必須（チケット束フラグも必須だが、boolなのでnullはfalseとみなす）
                if (_集計対象区分 == (int)Consts.SyukeiTaisyoKubun.Uriage)
                {
                    if (_売上区分 == null || _売上区分 < 0)
                    {
                        return "集計対象区分が売上の場合は、売上区分を入力してください。";
                    }
                }

                // 集計対象区分が1:入金の場合は、入金方法が必須
                if (_集計対象区分 == (int)Consts.SyukeiTaisyoKubun.Nyukin)
                {
                    if (_入金方法 == null || _入金方法 < 0)
                    {
                        return "集計対象区分が入金の場合は、入金方法を入力してください。";
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
        public bool Equals(Shiwake target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                仕訳ＣＤ == target.仕訳ＣＤ &&
                集計ＣＤ == target.集計ＣＤ &&
                集計対象区分 == target.集計対象区分 &&
                (売上区分 ?? -1) == (target.売上区分 ?? -1) &&
                (チケット束フラグ ?? false) == (target.チケット束フラグ ?? false) &&
                (入金方法 ?? -1) == (target.入金方法 ?? -1) &&
                (集計ＣＤ使用フラグ ?? false) == (target.集計ＣＤ使用フラグ ?? false) &&
                処理内容 == target.処理内容 &&
                借方ＣＤ == target.借方ＣＤ &&
                借方部門ＣＤ == target.借方部門ＣＤ &&
                (借方補助ＣＤ ?? "") == (target.借方補助ＣＤ ?? "") &&
                貸方ＣＤ == target.貸方ＣＤ &&
                貸方部門ＣＤ == target.貸方部門ＣＤ &&
                (貸方補助ＣＤ ?? "") == (target.貸方補助ＣＤ ?? "") &&
                摘要 == target.摘要 &&
                税率 == target.税率)
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
            string sql = "\r\n INSERT INTO 仕訳マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,仕訳ＣＤ" +
                         "\r\n     ,集計ＣＤ" +
                         "\r\n     ,集計対象区分" +
                         "\r\n     ,売上区分" +
                         "\r\n     ,チケット束フラグ" +
                         "\r\n     ,入金方法" +
                         "\r\n     ,集計ＣＤ使用フラグ" +
                         "\r\n     ,処理内容" +
                         "\r\n     ,借方ＣＤ" +
                         "\r\n     ,借方部門ＣＤ" +
                         "\r\n     ,借方補助ＣＤ" +
                         "\r\n     ,貸方ＣＤ" +
                         "\r\n     ,貸方部門ＣＤ" +
                         "\r\n     ,貸方補助ＣＤ" +
                         "\r\n     ,摘要" +
                         "\r\n     ,税率" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@仕訳ＣＤ" +
                         "\r\n     ,@集計ＣＤ" +
                         "\r\n     ,@集計対象区分" +
                         "\r\n     ,@売上区分" +
                         "\r\n     ,@チケット束フラグ" +
                         "\r\n     ,@入金方法" +
                         "\r\n     ,@集計ＣＤ使用フラグ" +
                         "\r\n     ,@処理内容" +
                         "\r\n     ,@借方ＣＤ" +
                         "\r\n     ,@借方部門ＣＤ" +
                         "\r\n     ,@借方補助ＣＤ" +
                         "\r\n     ,@貸方ＣＤ" +
                         "\r\n     ,@貸方部門ＣＤ" +
                         "\r\n     ,@貸方補助ＣＤ" +
                         "\r\n     ,@摘要" +
                         "\r\n     ,@税率" +
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
            string sql = "\r\n UPDATE 仕訳マスタ" +
                         "\r\n SET 集計ＣＤ           = @集計ＣＤ" +
                         "\r\n    ,集計対象区分       = @集計対象区分" +
                         "\r\n    ,売上区分           = @売上区分" +
                         "\r\n    ,チケット束フラグ   = @チケット束フラグ" +
                         "\r\n    ,入金方法           = @入金方法" +
                         "\r\n    ,集計ＣＤ使用フラグ = @集計ＣＤ使用フラグ" +
                         "\r\n    ,処理内容           = @処理内容" +
                         "\r\n    ,借方ＣＤ           = @借方ＣＤ" +
                         "\r\n    ,借方部門ＣＤ       = @借方部門ＣＤ" +
                         "\r\n    ,借方補助ＣＤ       = @借方補助ＣＤ" +
                         "\r\n    ,貸方ＣＤ           = @貸方ＣＤ" +
                         "\r\n    ,貸方部門ＣＤ       = @貸方部門ＣＤ" +
                         "\r\n    ,貸方補助ＣＤ       = @貸方補助ＣＤ" +
                         "\r\n    ,摘要               = @摘要" +
                         "\r\n    ,税率               = @税率" +
                         "\r\n    ,修正担当者ＣＤ     = @修正担当者ＣＤ" +
                         "\r\n    ,修正日             = getDate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
                         "\r\n   AND 仕訳ＣＤ = @仕訳ＣＤ" +
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
            string sql = "\r\n DELETE FROM 仕訳マスタ" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
                         "\r\n   AND 仕訳ＣＤ = @仕訳ＣＤ" +
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
                    // 売上の場合は、入金方法をクリア
                    if ((int)集計対象区分 == (int)Consts.SyukeiTaisyoKubun.Uriage)
                    {
                        入金方法 = -1;
                    }
                    else
                    {
                        // 入金の場合は、売上区分とチケット束フラグをクリア
                        売上区分 = -1;
                        チケット束フラグ = false;
                    }

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

                    if (仕訳ＣＤ != null)
                    {
                        cmd.Parameters.Add("@仕訳ＣＤ", SqlDbType.Int).Value = 仕訳ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@仕訳ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (集計ＣＤ != null)
                    {
                        cmd.Parameters.Add("@集計ＣＤ", SqlDbType.Int).Value = 集計ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計ＣＤ", SqlDbType.Int).Value = 0;
                    }

                    if (集計対象区分 != null)
                    {
                        cmd.Parameters.Add("@集計対象区分", SqlDbType.Int).Value = 集計対象区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計対象区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (売上区分 != null && 売上区分 != -1)
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = 売上区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = -1;
                    }

                    if (チケット束フラグ != null)
                    {
                        cmd.Parameters.Add("@チケット束フラグ", SqlDbType.Bit).Value = チケット束フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@チケット束フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (入金方法 != null && 入金方法 != -1)
                    {
                        cmd.Parameters.Add("@入金方法", SqlDbType.Int).Value = 入金方法;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金方法", SqlDbType.Int).Value = -1;
                    }

                    if (集計ＣＤ使用フラグ != null)
                    {
                        cmd.Parameters.Add("@集計ＣＤ使用フラグ", SqlDbType.Bit).Value = 集計ＣＤ使用フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計ＣＤ使用フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (処理内容 != null)
                    {
                        cmd.Parameters.Add("@処理内容", SqlDbType.VarChar).Value = 処理内容;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理内容", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (借方ＣＤ != null)
                    {
                        cmd.Parameters.Add("@借方ＣＤ", SqlDbType.VarChar).Value = 借方ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@借方ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (借方部門ＣＤ != null)
                    {
                        cmd.Parameters.Add("@借方部門ＣＤ", SqlDbType.VarChar).Value = 借方部門ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@借方部門ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (借方補助ＣＤ != null)
                    {
                        cmd.Parameters.Add("@借方補助ＣＤ", SqlDbType.VarChar).Value = 借方補助ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@借方補助ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (貸方ＣＤ != null)
                    {
                        cmd.Parameters.Add("@貸方ＣＤ", SqlDbType.VarChar).Value = 貸方ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@貸方ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (貸方部門ＣＤ != null)
                    {
                        cmd.Parameters.Add("@貸方部門ＣＤ", SqlDbType.VarChar).Value = 貸方部門ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@貸方部門ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (貸方補助ＣＤ != null)
                    {
                        cmd.Parameters.Add("@貸方補助ＣＤ", SqlDbType.VarChar).Value = 貸方補助ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@貸方補助ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (摘要 != null)
                    {
                        cmd.Parameters.Add("@摘要", SqlDbType.VarChar).Value = 摘要;
                    }
                    else
                    {
                        cmd.Parameters.Add("@摘要", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (税率 != null)
                    {
                        cmd.Parameters.Add("@税率", SqlDbType.VarChar).Value = 税率;
                    }
                    else
                    {
                        cmd.Parameters.Add("@税率", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("仕訳マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627)
                    {
                        Message = "同じ部署に仕訳ＣＤが重複しています。";
                    }
                    else if (ex.Number == 2601)
                    {
                        Message = "同じ部署で売上区分または入金方法が重複しています。";
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
