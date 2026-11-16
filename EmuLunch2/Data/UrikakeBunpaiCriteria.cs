using System;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売掛分配検索条件
    /// </summary>
    public class UrikakeBunpaiCriteria
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署ＣＤ;
        private string _部署名;
        private DateTime? _売上日;
        private int? _得意先ＣＤ;
        private string _得意先名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UrikakeBunpaiCriteria()
        {
            部署ＣＤ = null;
            部署名 = null;
            売上日 = null;
            得意先ＣＤ = null;
            得意先名 = null;
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

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public DateTime? 売上日
        {
            get { return _売上日; }
            set { _売上日 = value; }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                得意先名 = null;
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤ);
                    if (tokuisaki != null)
                    {
                        得意先名 = tokuisaki.得意先名;
                    }
                }
            }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        #endregion
    }
}
