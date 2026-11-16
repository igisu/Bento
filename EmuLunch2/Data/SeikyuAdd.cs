using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求追加
    /// </summary>
    public class SeikyuAdd : IDataErrorInfo
    {
        // ┌───────────┬────────────┬───────┐
        // │請求追加              │請求ヘッダ              │請求合計      │
        // ├───────────┼────────────┼───────┤
        // │                      │*企業ＣＤ               │*企業ＣＤ     │
        // ├───────────┼────────────┼───────┤
        // │                      │*請求番号               │*請求番号     │
        // ├───────────┼────────────┼───────┤
        // │部署ＣＤ              │*部署ＣＤ               │部署ＣＤ      │
        // ├───────────┼────────────┼───────┤
        // │請求先ＣＤ            │*得意先ＣＤ・請求先ＣＤ │請求先ＣＤ    │
        // ├───────────┼────────────┼───────┤
        // │請求先名              │得意先名・請求先名      │請求先名      │
        // ├───────────┼────────────┼───────┤
        // │締区分                │                        │締区分        │
        // ├───────────┼────────────┼───────┤
        // │請求日                │請求日                  │請求日        │
        // ├───────────┼────────────┼───────┤
        // │計算税率              │*計算税率               │              │
        // ├───────────┼────────────┼───────┤
        // │前回請求額            │                        │前回請求額    │
        // ├───────────┼────────────┼───────┤
        // │入金額                │                        │入金額        │
        // ├───────────┼────────────┼───────┤
        // │繰越額                │                        │繰越額        │
        // ├───────────┼────────────┼───────┤
        // │当月売上額            │売上金額                │買上金額      │
        // ├───────────┼────────────┼───────┤
        // │調整額                │                        │調整額        │
        // ├───────────┼────────────┼───────┤
        // │消費税額              │消費税                  │消費税        │
        // ├───────────┼────────────┼───────┤
        // │当月請求額            │                        │今回請求額    │
        // ├───────────┼────────────┼───────┤
        // │(当月売上額＋消費税額)│売上金額税込            │              │
        // ├───────────┼────────────┼───────┤
        // │                      │                        │回収予定日    │
        // ├───────────┼────────────┼───────┤
        // │                      │                        │印刷者ＣＤ    │
        // ├───────────┼────────────┼───────┤
        // │                      │                        │印刷日時      │
        // ├───────────┼────────────┼───────┤
        // │                      │                        │EB作成済フラグ│
        // └───────────┴────────────┴───────┘

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _部署ＣＤ;
        private int? _請求先ＣＤ;
        private string _請求先名;
        private int? _締区分;
        private DateTime? _請求日;
        private decimal? _計算税率;
        private decimal? _前回請求額;
        private decimal? _入金額;
        private decimal? _繰越額;
        private decimal? _当月売上額;
        private decimal? _調整額;
        private decimal? _消費税額;
        private decimal? _当月請求額;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuAdd()
        {
            部署ＣＤ = null;
            請求先ＣＤ = null;
            締区分 = null;
            請求日 = null;
            計算税率 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SeikyuAdd(SeikyuAdd src)
        {
            部署ＣＤ = src.部署ＣＤ;
            請求先ＣＤ = src.請求先ＣＤ;
            請求先名 = src.請求先名;
            締区分 = src.締区分;
            請求日 = src.請求日;
            計算税率 = src.計算税率;
            前回請求額 = src.前回請求額;
            入金額 = src.入金額;
            繰越額 = src.繰越額;
            当月売上額 = src.当月売上額;
            調整額 = src.調整額;
            消費税額 = src.消費税額;
            当月請求額 = src.当月請求額;
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

        public int? 請求先ＣＤ
        {
            get { return _請求先ＣＤ; }
            set
            {
                _請求先ＣＤ = value;
                _errors.Remove("請求先ＣＤ");
                請求先名 = null;
                if (value == null)
                {
                    _errors["請求先ＣＤ"] = "請求先ＣＤを入力してください。";
                }
                else
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤ);
                    if (tokuisaki != null)
                    {
                        請求先名 = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["請求先ＣＤ"] = "請求先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
        }

        public int? 締区分
        {
            get { return _締区分; }
            set
            {
                _締区分 = value;
                _errors.Remove("締区分");
                if (value == null || value < 0)
                {
                    _errors["締区分"] = "締区分を入力してください。";
                }
            }
        }

        public DateTime? 請求日
        {
            get { return _請求日; }
            set
            {
                _請求日 = value;
                _errors.Remove("請求日");
                if (value == null)
                {
                    _errors["請求日"] = "請求日を入力してください。";
                }
            }
        }

        public decimal? 計算税率
        {
            get { return _計算税率; }
            set
            {
                _計算税率 = value;
                _errors.Remove("計算税率");
                if (value == null || value < 0m)
                {
                    _errors["計算税率"] = "計算税率を入力してください。";
                }
            }
        }

        public decimal? 前回請求額
        {
            get { return _前回請求額; }
            set { _前回請求額 = value; }
        }

        public decimal? 入金額
        {
            get { return _入金額; }
            set { _入金額 = value; }
        }

        public decimal? 繰越額
        {
            get { return _繰越額; }
            set { _繰越額 = value; }
        }

        public decimal? 当月売上額
        {
            get { return _当月売上額; }
            set { _当月売上額 = value; }
        }

        public decimal? 調整額
        {
            get { return _調整額; }
            set { _調整額 = value; }
        }

        public decimal? 消費税額
        {
            get { return _消費税額; }
            set { _消費税額 = value; }
        }

        public decimal? 当月請求額
        {
            get { return _当月請求額; }
            set { _当月請求額 = value; }
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
    }
}
