using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// コースマスタ（検索条件用）
    /// </summary>
    public class CourseCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _コースＣＤ;
        private int? _コース区分;
        private string _コース名;
        private int? _部署ＣＤ;
        private int? _担当者ＣＤ;
        private int? _得意先ＣＤ;
        private bool? _工場フラグ;
        private bool? _無効フラグ;

        private string _担当者名;
        private string _得意先名;

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set { _コースＣＤ = value; }
        }

        public int? コース区分
        {
            get { return _コース区分; }
            set { _コース区分 = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set
            {
                _担当者ＣＤ = value;
                _errors.Remove("担当者ＣＤ");
                担当者名 = null;
                if (value != null)
                {
                    // 担当者ＣＤに該当する担当者を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_担当者ＣＤ);
                    if (tantousya != null)
                    {
                        担当者名 = tantousya.担当者名;
                    }
                }
            }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                _errors.Remove("得意先ＣＤ");
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

        public bool? 工場フラグ
        {
            get { return _工場フラグ; }
            set { _工場フラグ = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public string 担当者名
        {
            get { return _担当者名; }
            set { _担当者名 = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
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
    }
}
