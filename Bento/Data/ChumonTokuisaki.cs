using System;
using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 注文入力得意先情報
    /// </summary>
    public class ChumonTokuisaki : ITokuisaki
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _得意先ＣＤ;
        private string _得意先名略称;
        private string _電話番号;
        private string _電話番号備考;
        private int? _支店ＣＤ;
        private string _支店名;
        private int? _税区分;
        private int? _丸め処理区分;
        private int? _売掛現金区分;
        private int? _受注得意先ＣＤ;
        private string _配送状態;
        private string _備考1;
        private string _備考2;
        private string _備考3;
        private string _受注時特記事項;
        private string _休日設定;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChumonTokuisaki()
        {
            得意先ＣＤ = null;
            得意先名略称 = null;
            電話番号 = null;
            電話番号備考 = null;
            支店ＣＤ = null;
            支店名 = null;
            税区分 = null;
            丸め処理区分 = null;
            売掛現金区分 = null;
            受注得意先ＣＤ = null;
            配送状態 = null;
            備考1 = null;
            備考2 = null;
            備考3 = null;
            受注時特記事項 = null;
            休日設定 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public ChumonTokuisaki(ChumonTokuisaki src)
        {
            得意先ＣＤ = src.得意先ＣＤ;
            得意先名略称 = src.得意先名略称;
            電話番号 = src.電話番号;
            電話番号備考 = src.電話番号備考;
            支店ＣＤ = src.支店ＣＤ;
            支店名 = src.支店名;
            税区分 = src.税区分;
            丸め処理区分 = src.丸め処理区分;
            売掛現金区分 = src.売掛現金区分;
            受注得意先ＣＤ = src.受注得意先ＣＤ;
            配送状態 = src.配送状態;
            備考1 = src.備考1;
            備考2 = src.備考2;
            備考3 = src.備考3;
            受注時特記事項 = src.受注時特記事項;
            休日設定 = src.休日設定;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public ChumonTokuisaki(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            得意先名略称 = dr["得意先名略称", version] as string;
            電話番号 = dr["電話番号", version] as string;
            電話番号備考 = dr["電話番号備考", version] as string;
            支店ＣＤ = dr["支店ＣＤ", version] as int?;
            支店名 = dr["支店名", version] as string;
            税区分 = dr["税区分", version] as int?;
            丸め処理区分 = dr["丸め処理区分", version] as int?;
            売掛現金区分 = dr["売掛現金区分", version] as int?;
            受注得意先ＣＤ = dr["受注得意先ＣＤ", version] as int?;
            配送状態 = dr["配送状態", version] as string;
            備考1 = dr["備考1", version] as string;
            備考2 = dr["備考2", version] as string;
            備考3 = dr["備考3", version] as string;
            受注時特記事項 = dr["受注時特記事項", version] as string;
            休日設定 = dr["休日設定", version] as string;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
        }

        public string 得意先名略称
        {
            get { return _得意先名略称; }
            set { _得意先名略称 = value; }
        }

        public string 電話番号
        {
            get { return _電話番号; }
            set { _電話番号 = value; }
        }

        public string 電話番号備考
        {
            get { return _電話番号備考; }
            set { _電話番号備考 = value; }
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

        public int? 受注得意先ＣＤ
        {
            get { return _受注得意先ＣＤ; }
            set { _受注得意先ＣＤ = value; }
        }

        public string 配送状態
        {
            get { return _配送状態; }
            set { _配送状態 = value; }
        }

        public string 備考1
        {
            get { return _備考1; }
            set { _備考1 = value; }
        }

        public string 備考2
        {
            get { return _備考2; }
            set { _備考2 = value; }
        }

        public string 備考3
        {
            get { return _備考3; }
            set { _備考3 = value; }
        }

        public string 受注時特記事項
        {
            get { return _受注時特記事項; }
            set { _受注時特記事項 = value; }
        }

        public string 休日設定
        {
            get { return _休日設定; }
            set { _休日設定 = value; }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 休日設定の該当の曜日の値を返す
        /// </summary>
        /// <param name="dayOfWeek">休日設定の曜日インデックス</param>
        /// <returns>休日の場合はtrue。それ以外はfalse。</returns>
        public bool GetHoliday(DayOfWeek dayOfWeek)
        {
            return Tokuisaki.GetHoliday(休日設定, dayOfWeek);
        }
        #endregion

    }
}
