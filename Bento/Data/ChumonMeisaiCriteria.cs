using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 注文入力検索条件
    /// </summary>
    public class ChumonMeisaiCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        public bool stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _得意先ＣＤ;
        private string _得意先名;
        private int? _部署ＣＤ;
        private DateTime? _注文日;

        private int? _コースＣＤ;               // コース注文入力／コース注文一覧／受注発信一覧から注文入力を開いたときにデフォルト表示するコース

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChumonMeisaiCriteria()
        {
            stopOverwrite = false;

            得意先ＣＤ = null;
            部署ＣＤ = null;        // 得意先の部署を表示するので、所属部署を初期値にしても意味が無い
            注文日 = null;
            得意先名 = null;

            コースＣＤ = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public ChumonMeisaiCriteria(ChumonMeisaiCriteria src)
        {
            stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            得意先ＣＤ = src.得意先ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            得意先名 = src.得意先名;
            注文日 = src.注文日;

            コースＣＤ = src.コースＣＤ;

            stopOverwrite = false;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                _errors.Remove("得意先ＣＤ");
                得意先名 = null;
                部署ＣＤ = null;
                if (value == null)
                {
                    _errors["得意先ＣＤ"] = "得意先ＣＤを入力してください。";
                }
                else
                {
                    if (stopOverwrite == false)
                    {
                        // 得意先ＣＤに該当する得意先名を取得
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            得意先名 = tokuisaki.得意先名;
                            部署ＣＤ = tokuisaki.部署ＣＤ;
                        }
                        else
                        {
                            _errors["得意先ＣＤ"] = "得意先が見つかりません。";
                        }
                    }
                }
            }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

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

        public DateTime? 注文日
        {
            get { return _注文日; }
            set
            {
                _注文日 = value;
                _errors.Remove("注文日");
                if (value == null)
                {
                    _errors["注文日"] = "注文日を入力してください。";
                }
            }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set { _コースＣＤ = value; }
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
        public bool Equals(ChumonMeisaiCriteria target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                得意先名 == target.得意先名 &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                注文日 == target.注文日)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
