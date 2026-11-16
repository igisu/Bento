using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上入力得意先情報
    /// </summary>
    public class UriageTokuisaki
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private string _得意先名略称;
        private int? _部署ＣＤ;
        private string _部署名;
        private int? _支店ＣＤ;
        private string _支店名;
        private int? _税区分;
        private int? _丸め処理区分;
        private int? _売掛現金区分;
        private int? _支払方法;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageTokuisaki()
        {
            得意先名略称 = null;
            部署ＣＤ = null;
            部署名 = null;
            支店ＣＤ = null;
            支店名 = null;
            税区分 = null;
            丸め処理区分 = null;
            売掛現金区分 = null;
            支払方法 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageTokuisaki(UriageTokuisaki src)
        {
            得意先名略称 = src.得意先名略称;
            部署ＣＤ = src.部署ＣＤ;
            部署名 = src.部署名;
            支店ＣＤ = src.支店ＣＤ;
            支店名 = src.支店名;
            税区分 = src.税区分;
            丸め処理区分 = src.丸め処理区分;
            売掛現金区分 = src.売掛現金区分;
            支払方法 = src.支払方法;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public UriageTokuisaki(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先名略称 = dr["得意先名略称", version] as string;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            部署名 = dr["部署名", version] as string;
            支店ＣＤ = dr["支店ＣＤ", version] as int?;
            支店名 = dr["支店名", version] as string;
            税区分 = dr["税区分", version] as int?;
            丸め処理区分 = dr["丸め処理区分", version] as int?;
            売掛現金区分 = dr["売掛現金区分", version] as int?;
            支払方法 = dr["支払方法", version] as int?;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string 得意先名略称
        {
            get { return _得意先名略称; }
            set { _得意先名略称 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set { _支店ＣＤ = value; }
        }

        public string 支店名
        {
            get { return _支店名; }
            set { _支店名 = value; }
        }

        public int? 税区分
        {
            get { return _税区分; }
            set { _税区分 = value; }
        }

        public int? 丸め処理区分
        {
            get { return _丸め処理区分; }
            set { _丸め処理区分 = value; }
        }

        public int? 売掛現金区分
        {
            get { return _売掛現金区分; }
            set { _売掛現金区分 = value; }
        }

        public int? 支払方法
        {
            get { return _支払方法; }
            set { _支払方法 = value; }
        }

        #endregion

    }
}
