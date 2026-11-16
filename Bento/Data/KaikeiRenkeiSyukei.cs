using System;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 会計連携集計テーブル
    /// </summary>
    public class KaikeiRenkeiSyukei : TableBase2
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private DateTime? _集計日;
        private int? _部署ＣＤ;
        private int? _伝票番号;
        private DateTime? _伝票日付;
        private int? _仕訳ＣＤ;
        private int? _集計ＣＤ;
        private string _借方ＣＤ;
        private string _借方部門ＣＤ;
        private string _借方補助ＣＤ;
        private string _貸方ＣＤ;
        private string _貸方部門ＣＤ;
        private string _貸方補助ＣＤ;
        private decimal? _金額;
        private bool? _差分フラグ;
        private bool? _マイナスフラグ;
        private string _税率;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KaikeiRenkeiSyukei()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public KaikeiRenkeiSyukei(KaikeiRenkeiSyukei src)
        {
            集計日 = src.集計日;
            部署ＣＤ = src.部署ＣＤ;
            伝票番号 = src.伝票番号;
            伝票日付 = src.伝票日付;
            仕訳ＣＤ = src.仕訳ＣＤ;
            集計ＣＤ = src.集計ＣＤ;
            借方ＣＤ = src.借方ＣＤ;
            借方部門ＣＤ = src.借方部門ＣＤ;
            借方補助ＣＤ = src.借方補助ＣＤ;
            貸方ＣＤ = src.貸方ＣＤ;
            貸方部門ＣＤ = src.貸方部門ＣＤ;
            貸方補助ＣＤ = src.貸方補助ＣＤ;
            金額 = src.金額;
            差分フラグ = src.差分フラグ;
            マイナスフラグ = src.マイナスフラグ;
            税率 = src.税率;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public KaikeiRenkeiSyukei(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            集計日 = dr["集計日", version] as DateTime?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            伝票番号 = dr["伝票番号", version] as int?;
            伝票日付 = dr["伝票日付", version] as DateTime?;
            仕訳ＣＤ = dr["仕訳ＣＤ", version] as int?;
            集計ＣＤ = dr["集計ＣＤ", version] as int?;
            借方ＣＤ = dr["借方ＣＤ", version] as string;
            借方部門ＣＤ = dr["借方部門ＣＤ", version] as string;
            借方補助ＣＤ = dr["借方補助ＣＤ", version] as string;
            貸方ＣＤ = dr["貸方ＣＤ", version] as string;
            貸方部門ＣＤ = dr["貸方部門ＣＤ", version] as string;
            貸方補助ＣＤ = dr["貸方補助ＣＤ", version] as string;
            金額 = dr["金額", version] as decimal?;
            差分フラグ = dr["差分フラグ", version] as bool?;
            マイナスフラグ = dr["マイナスフラグ", version] as bool?;
            税率 = dr["税率", version] as string;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 集計日
        {
            get { return _集計日; }
            set { _集計日 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 伝票番号
        {
            get { return _伝票番号; }
            set { _伝票番号 = value; }
        }

        public DateTime? 伝票日付
        {
            get { return _伝票日付; }
            set { _伝票日付 = value; }
        }

        public int? 仕訳ＣＤ
        {
            get { return _仕訳ＣＤ; }
            set { _仕訳ＣＤ = value; }
        }

        public int? 集計ＣＤ
        {
            get { return _集計ＣＤ; }
            set { _集計ＣＤ = value; }
        }

        public string 借方ＣＤ
        {
            get { return _借方ＣＤ; }
            set { _借方ＣＤ = value; }
        }

        public string 借方部門ＣＤ
        {
            get { return _借方部門ＣＤ; }
            set { _借方部門ＣＤ = value; }
        }

        public string 借方補助ＣＤ
        {
            get { return _借方補助ＣＤ; }
            set { _借方補助ＣＤ = value; }
        }

        public string 貸方ＣＤ
        {
            get { return _貸方ＣＤ; }
            set { _貸方ＣＤ = value; }
        }

        public string 貸方部門ＣＤ
        {
            get { return _貸方部門ＣＤ; }
            set { _貸方部門ＣＤ = value; }
        }

        public string 貸方補助ＣＤ
        {
            get { return _貸方補助ＣＤ; }
            set { _貸方補助ＣＤ = value; }
        }

        public decimal? 金額
        {
            get { return _金額; }
            set { _金額 = value; }
        }

        public bool? 差分フラグ
        {
            get { return _差分フラグ; }
            set { _差分フラグ = value; }
        }

        public bool? マイナスフラグ
        {
            get { return _マイナスフラグ; }
            set { _マイナスフラグ = value; }
        }

        public string 税率
        {
            get { return _税率; }
            set { _税率 = value; }
        }

        public override string Code
        {
            get { return string.Format("集計日=[{0}] 部署ＣＤ=[{1}] 伝票番号=[{2}] 伝票日付=[{3}] 仕訳ＣＤ=[{4}]", 集計日, 部署ＣＤ, 伝票番号, 伝票日付, 仕訳ＣＤ); }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 一致する列情報を持つDataRowからインポートする
        /// </summary>
        /// <param name="dr"></param>
        public void Import(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            // 本来はテーブルを取得した結果のDataRowであることが前提だが、
            // このテーブルに限っては違うテーブルから取得した結果のDataRowが有り得るので
            // 全ての列について存在確認を行う(★)

            if (dr.Table.Columns.Contains("集計日"))
            {
                集計日 = dr["集計日", version] as DateTime?;
            }

            if (dr.Table.Columns.Contains("部署ＣＤ"))
            {
                部署ＣＤ = dr["部署ＣＤ", version] as int?;
            }

            if (dr.Table.Columns.Contains("伝票番号"))
            {
                伝票番号 = dr["伝票番号", version] as int?;
            }

            if (dr.Table.Columns.Contains("伝票日付"))
            {
                伝票日付 = dr["伝票日付", version] as DateTime?;
            }

            if (dr.Table.Columns.Contains("仕訳ＣＤ"))
            {
                仕訳ＣＤ = dr["仕訳ＣＤ", version] as int?;
            }

            if (dr.Table.Columns.Contains("集計ＣＤ"))
            {
                集計ＣＤ = dr["集計ＣＤ", version] as int?;
            }

            if (dr.Table.Columns.Contains("借方ＣＤ"))
            {
                借方ＣＤ = dr["借方ＣＤ", version] as string;
            }

            if (dr.Table.Columns.Contains("借方部門ＣＤ"))
            {
                借方部門ＣＤ = dr["借方部門ＣＤ", version] as string;
            }

            if (dr.Table.Columns.Contains("貸方ＣＤ"))
            {
                貸方ＣＤ = dr["貸方ＣＤ", version] as string;
            }

            if (dr.Table.Columns.Contains("貸方部門ＣＤ"))
            {
                貸方部門ＣＤ = dr["貸方部門ＣＤ", version] as string;
            }

            if (dr.Table.Columns.Contains("金額"))
            {
                金額 = dr["金額", version] as decimal?;
            }

            if (dr.Table.Columns.Contains("差分フラグ"))
            {
                // intかもしれない
                差分フラグ = Convert.ToBoolean(dr["差分フラグ", version]);
            }

            if (dr.Table.Columns.Contains("マイナスフラグ"))
            {
                // intかもしれない
                マイナスフラグ = Convert.ToBoolean(dr["マイナスフラグ", version]);
            }

            if (dr.Table.Columns.Contains("税率"))
            {
                税率 = dr["税率", version] as string;
            }

            // 借方／貸方補助ＣＤについては、Shiwake.HOJO_OPTION が格納されていることがあるので、それを変換する
            if (dr.Table.Columns.Contains("借方補助ＣＤ"))
            {
                借方補助ＣＤ = ConvertHojo(dr, version, "借方補助ＣＤ");
            }

            if (dr.Table.Columns.Contains("貸方補助ＣＤ"))
            {
                貸方補助ＣＤ = ConvertHojo(dr, version, "貸方補助ＣＤ");
            }

            // (★)の理由により、メタデータについては取得しない
            //base.FromDataRow(dr, version);
        }

        /// <summary>
        /// 金額がマイナスの場合、借方と貸方のコードを交換します
        /// </summary>
        [Obsolete("金額がマイナスの場合に借方と貸方を逆にするのは会計連携集計テーブルへの登録時ではなく、CSV出力時とします。会計連携集計テーブルの登録時はマイナスフラグを立てるだけです。代わりにSetMinusFlg()を使用してください。")]
        public void SwapAutomatically()
        {
            //******************************************************************************************************************************
            // 本来はマイナスだった値がプラスになっていると、差分算出する集計時に再度マイナスにしないといけない。
            // 集計時にそれをやるのは処理が複雑になるので、会計連携集計テーブルにはマイナスのまま出力し、
            // CSV出力時に金額をプラス（にして借方と貸方を逆）にする
            // Ｑ．CSV出力時に逆にする判断材料は、マイナスフラグではなく金額がマイナスなら、でいいのでは（つまりマイナスフラグは不要では）？
            // Ａ．逆にするのがCSV出力時ではなくやっぱり会計連携集計テーブル出力時にすべき となったときの判断材料としてマイナスフラグは必要
            //     仮にそうなった場合、集計時には処理が複雑になるが仕方ない
            //******************************************************************************************************************************

            try
            {
                if ((_金額 ?? 0m) < 0m)
                {
                    _金額 *= -1;
                    Swap(ref _借方ＣＤ, ref _貸方ＣＤ);
                    Swap(ref _借方部門ＣＤ, ref _貸方部門ＣＤ);
                    Swap(ref _借方補助ＣＤ, ref _貸方補助ＣＤ);
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
        /// 金額がマイナスの場合、マイナスフラグをONにします
        /// </summary>
        public void SetMinusFlg()
        {
            try
            {
                if ((_金額 ?? 0m) < 0m)
                {
                    _マイナスフラグ = true;
                }
                else
                {
                    _マイナスフラグ = false;
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
        /// スワップ
        /// </summary>
        /// <typeparam name="T">変数の型</typeparam>
        /// <param name="lhs">交換対象1</param>
        /// <param name="rhs">交換対象2</param>
        private void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        /// <summary>
        /// 補助ＣＤがデータ取得元を示している場合、そのデータを返す。
        /// </summary>
        /// <param name="dr">レコード</param>
        /// <param name="version">レコードのバージョン</param>
        /// <param name="hojoColumnName">補助ＣＤの列名</param>
        /// <returns>該当するデータ</returns>
        private string ConvertHojo(DataRow dr, DataRowVersion version, string hojoColumnName)
        {
            string value = null;

            // 列名に該当する補助ＣＤを取得
            string hojoCD = dr[hojoColumnName, version] as string;

            // 補助ＣＤが Shiwake.HOJO_OPTION のいずれか？
            if (Shiwake.HOJO_OPTION.Contains(hojoCD))
            {
                // 前後の角括弧を除去して列名にする
                hojoCD = hojoCD.Replace("[", "");
                hojoCD = hojoCD.Replace("]", "");

                value = dr[hojoCD, version].ToString();
            }
            else
            {
                // 補助ＣＤが Shiwake.HOJO_OPTION に該当しない場合は、その値を文字列としてそのまま返す
                if (string.IsNullOrWhiteSpace(hojoCD) == false)
                {
                    value = hojoCD;
                }
            }

            return value;
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(KaikeiRenkeiSyukei target)
        {
            if (集計日 == target.集計日 &&
                部署ＣＤ == target.部署ＣＤ &&
                伝票番号 == target.伝票番号 &&
                伝票日付 == target.伝票日付 &&
                仕訳ＣＤ == target.仕訳ＣＤ &&
                集計ＣＤ == target.集計ＣＤ &&
                借方ＣＤ == target.借方ＣＤ &&
                借方部門ＣＤ == target.借方部門ＣＤ &&
                借方補助ＣＤ == target.借方補助ＣＤ &&
                貸方ＣＤ == target.貸方ＣＤ &&
                貸方部門ＣＤ == target.貸方部門ＣＤ &&
                貸方補助ＣＤ == target.貸方補助ＣＤ &&
                金額 == target.金額 &&
                (差分フラグ ?? false) == (target.差分フラグ ?? false) &&
                (マイナスフラグ ?? false) == (target.マイナスフラグ ?? false) &&
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
            string sql = "\r\n INSERT INTO 会計連携集計テーブル(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,集計日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,伝票番号" +
                         "\r\n     ,伝票日付" +
                         "\r\n     ,仕訳ＣＤ" +
                         "\r\n     ,集計ＣＤ" +
                         "\r\n     ,借方ＣＤ" +
                         "\r\n     ,借方部門ＣＤ" +
                         "\r\n     ,借方補助ＣＤ" +
                         "\r\n     ,貸方ＣＤ" +
                         "\r\n     ,貸方部門ＣＤ" +
                         "\r\n     ,貸方補助ＣＤ" +
                         "\r\n     ,金額" +
                         "\r\n     ,差分フラグ" +
                         "\r\n     ,マイナスフラグ" +
                         "\r\n     ,税率" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@集計日" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@伝票番号" +
                         "\r\n     ,@伝票日付" +
                         "\r\n     ,@仕訳ＣＤ" +
                         "\r\n     ,@集計ＣＤ" +
                         "\r\n     ,@借方ＣＤ" +
                         "\r\n     ,@借方部門ＣＤ" +
                         "\r\n     ,@借方補助ＣＤ" +
                         "\r\n     ,@貸方ＣＤ" +
                         "\r\n     ,@貸方部門ＣＤ" +
                         "\r\n     ,@貸方補助ＣＤ" +
                         "\r\n     ,@金額" +
                         "\r\n     ,@差分フラグ" +
                         "\r\n     ,@マイナスフラグ" +
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
            string sql = "\r\n UPDATE 会計連携集計テーブル" +
                         "\r\n SET 伝票日付       = @伝票日付" +
                         "\r\n    ,仕訳ＣＤ       = @仕訳ＣＤ" +
                         "\r\n    ,集計ＣＤ       = @集計ＣＤ" +
                         "\r\n    ,借方ＣＤ       = @借方ＣＤ" +
                         "\r\n    ,借方部門ＣＤ   = @借方部門ＣＤ" +
                         "\r\n    ,借方補助ＣＤ   = @借方補助ＣＤ" +
                         "\r\n    ,貸方ＣＤ       = @貸方ＣＤ" +
                         "\r\n    ,貸方部門ＣＤ   = @貸方部門ＣＤ" +
                         "\r\n    ,貸方補助ＣＤ   = @貸方補助ＣＤ" +
                         "\r\n    ,金額           = @金額" +
                         "\r\n    ,差分フラグ     = @差分フラグ" +
                         "\r\n    ,マイナスフラグ = @マイナスフラグ" +
                         "\r\n    ,税率           = @税率" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getDate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 集計日   = @集計日" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
                         "\r\n   AND 伝票番号 = @伝票番号" +
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
            string sql = "\r\n DELETE FROM 会計連携集計テーブル" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 集計日   = @集計日" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
                         "\r\n   AND 伝票番号 = @伝票番号" +
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

                    if (集計日 != null)
                    {
                        cmd.Parameters.Add("@集計日", SqlDbType.Date).Value = 集計日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (伝票番号 != null)
                    {
                        cmd.Parameters.Add("@伝票番号", SqlDbType.Int).Value = 伝票番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@伝票番号", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (伝票日付 != null)
                    {
                        cmd.Parameters.Add("@伝票日付", SqlDbType.Date).Value = 伝票日付;
                    }
                    else
                    {
                        cmd.Parameters.Add("@伝票日付", SqlDbType.Date).Value = DBNull.Value;
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
                        cmd.Parameters.Add("@集計ＣＤ", SqlDbType.Int).Value = DBNull.Value;
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

                    if (金額 != null)
                    {
                        cmd.Parameters.Add("@金額", SqlDbType.Decimal).Value = 金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (差分フラグ != null)
                    {
                        cmd.Parameters.Add("@差分フラグ", SqlDbType.Bit).Value = 差分フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@差分フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (マイナスフラグ != null)
                    {
                        cmd.Parameters.Add("@マイナスフラグ", SqlDbType.Bit).Value = マイナスフラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@マイナスフラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("会計連携集計テーブルを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "会計連携集計テーブルが重複しています。";
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
