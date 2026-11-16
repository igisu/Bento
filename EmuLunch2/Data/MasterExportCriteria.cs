using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// マスタエクスポート条件
    /// </summary>
    public class MasterExportCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        /// <summary>
        /// 対象マスタチェック状態
        /// </summary>
        protected HashSet<string> _targetSet = new HashSet<string>();

        private bool? _部署マスタ;
        private bool? _得意先マスタ;
        private bool? _得意先単価マスタ;
        private bool? _得意先単価適用マスタ;
        private bool? _商品マスタ;
        private bool? _商品グループマスタ;
        private bool? _主要商品リストマスタ;
        private bool? _主要部品リストマスタ;
        private bool? _コースマスタ;
        private bool? _コース別グループマスタ;
        private bool? _担当者マスタ;
        private bool? _担当者架電コースマスタ;
        private bool? _コーステーブル;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MasterExportCriteria()
        {
            部署マスタ = false;
            得意先マスタ = false;
            得意先単価マスタ = false;
            得意先単価適用マスタ = false;
            商品マスタ = false;
            商品グループマスタ = false;
            主要商品リストマスタ = false;
            主要部品リストマスタ = false;
            コースマスタ = false;
            コース別グループマスタ = false;
            担当者マスタ = false;
            担当者架電コースマスタ = false;
            コーステーブル = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public bool? 部署マスタ
        {
            get { return _部署マスタ; }
            set
            {
                _部署マスタ = value;
                _targetSet.Remove("部署マスタ");
                if (value == true)
                {
                    _targetSet.Add("部署マスタ");
                }
            }
        }

        public bool? 得意先マスタ
        {
            get { return _得意先マスタ; }
            set
            {
                _得意先マスタ = value;
                _targetSet.Remove("得意先マスタ");
                if (value == true)
                {
                    _targetSet.Add("得意先マスタ");
                }
            }
        }

        public bool? 得意先単価マスタ
        {
            get { return _得意先単価マスタ; }
            set
            {
                _得意先単価マスタ = value;
                _targetSet.Remove("得意先単価マスタ");
                if (value == true)
                {
                    _targetSet.Add("得意先単価マスタ");
                }
            }
        }

        public bool? 得意先単価適用マスタ
        {
            get { return _得意先単価適用マスタ; }
            set
            {
                _得意先単価適用マスタ = value;
                _targetSet.Remove("得意先単価適用マスタ");
                if (value == true)
                {
                    _targetSet.Add("得意先単価適用マスタ");
                }
            }
        }

        public bool? 商品マスタ
        {
            get { return _商品マスタ; }
            set
            {
                _商品マスタ = value;
                _targetSet.Remove("商品マスタ");
                if (value == true)
                {
                    _targetSet.Add("商品マスタ");
                }
            }
        }

        public bool? 商品グループマスタ
        {
            get { return _商品グループマスタ; }
            set
            {
                _商品グループマスタ = value;
                _targetSet.Remove("商品グループマスタ");
                if (value == true)
                {
                    _targetSet.Add("商品グループマスタ");
                }
            }
        }

        public bool? 主要商品リストマスタ
        {
            get { return _主要商品リストマスタ; }
            set
            {
                _主要商品リストマスタ = value;
                _targetSet.Remove("主要商品リストマスタ");
                if (value == true)
                {
                    _targetSet.Add("主要商品リストマスタ");
                }
            }
        }

        public bool? 主要部品リストマスタ
        {
            get { return _主要部品リストマスタ; }
            set
            {
                _主要部品リストマスタ = value;
                _targetSet.Remove("主要部品リストマスタ");
                if (value == true)
                {
                    _targetSet.Add("主要部品リストマスタ");
                }
            }
        }

        public bool? コースマスタ
        {
            get { return _コースマスタ; }
            set
            {
                _コースマスタ = value;
                _targetSet.Remove("コースマスタ");
                if (value == true)
                {
                    _targetSet.Add("コースマスタ");
                }
            }
        }

        public bool? コース別グループマスタ
        {
            get { return _コース別グループマスタ; }
            set
            {
                _コース別グループマスタ = value;
                _targetSet.Remove("コース別グループマスタ");
                if (value == true)
                {
                    _targetSet.Add("コース別グループマスタ");
                }
            }
        }

        public bool? 担当者マスタ
        {
            get { return _担当者マスタ; }
            set
            {
                _担当者マスタ = value;
                _targetSet.Remove("担当者マスタ");
                if (value == true)
                {
                    _targetSet.Add("担当者マスタ");
                }
            }
        }

        public bool? 担当者架電コースマスタ
        {
            get { return _担当者架電コースマスタ; }
            set
            {
                _担当者架電コースマスタ = value;
                _targetSet.Remove("担当者架電コースマスタ");
                if (value == true)
                {
                    _targetSet.Add("担当者架電コースマスタ");
                }
            }
        }

        public bool? コーステーブル
        {
            get { return _コーステーブル; }
            set
            {
                _コーステーブル = value;
                _targetSet.Remove("コーステーブル");
                if (value == true)
                {
                    _targetSet.Add("コーステーブル");
                }
            }
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
                if (_targetSet.Count == 0)
                {
                    return "エクスポート対象のマスタが選択されていません。";
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
    }
}
