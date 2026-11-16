using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求EBデータ再作成
    /// </summary>
    public class SeikyuDataEBReCreate : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _部署ＣＤ;
        private string _EB銀行支店ＣＤ;   // 銀行ＣＤ支店ＣＤ
        private bool? _全部署出力フラグ;

        private DateTime? _引落日;

        // EB結果ファイル読み込み条件
        private SeikyuDataEBReCreateImport _seikyuDataEBReCreateImport = null;

        // 未回収金検索条件
        private SeikyuDataEBReCreateCriteria _seikyuDataEBReCreateCriteria = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuDataEBReCreate()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            全部署出力フラグ = false;
            // 請求EBデータ作成とは異なり、引落日はnull(=空欄)にしておく
            引落日 = null;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuDataEBReCreate(SeikyuDataEBReCreateImport seikyuDataEBReCreateImport, SeikyuDataEBReCreateCriteria seikyuDataEBReCreateCriteria)
        {
            _seikyuDataEBReCreateImport = seikyuDataEBReCreateImport;
            _seikyuDataEBReCreateCriteria = seikyuDataEBReCreateCriteria;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            全部署出力フラグ = false;
            // 請求EBデータ作成とは異なり、引落日はnull(=空欄)にしておく
            引落日 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SeikyuDataEBReCreate(SeikyuDataEBReCreate src)
        {
            部署ＣＤ = src.部署ＣＤ;
            EB銀行支店ＣＤ = src.EB銀行支店ＣＤ;
            全部署出力フラグ = src.全部署出力フラグ;
            引落日 = src.引落日;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        /// <summary>
        /// EB結果ファイル読み込み条件
        /// </summary>
        public SeikyuDataEBReCreateImport SeikyuDataEBReCreateImport
        {
            set { _seikyuDataEBReCreateImport = value; }
        }

        /// <summary>
        /// 未回収金検索条件
        /// </summary>
        public SeikyuDataEBReCreateCriteria SeikyuDataEBReCreateCriteria
        {
            set { _seikyuDataEBReCreateCriteria = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set
            {
                _部署ＣＤ = value;

                // EB結果ファイル読み込み条件と未回収金検索条件の部署ＣＤにも反映
                if (_seikyuDataEBReCreateImport != null)
                {
                    _seikyuDataEBReCreateImport.部署ＣＤ = _部署ＣＤ;
                }

                if (_seikyuDataEBReCreateCriteria != null)
                {
                    _seikyuDataEBReCreateCriteria.部署ＣＤ = _部署ＣＤ;
                }

                _errors.Remove("部署ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
            }
        }

        public string EB銀行支店ＣＤ
        {
            get { return _EB銀行支店ＣＤ; }
            set
            {
                _EB銀行支店ＣＤ = value;

                // 未回収金検索条件のEB銀行支店ＣＤにも反映
                if (_seikyuDataEBReCreateCriteria != null)
                {
                    _seikyuDataEBReCreateCriteria.EB銀行支店ＣＤ = _EB銀行支店ＣＤ;
                }
            }
        }

        public bool? 全部署出力フラグ
        {
            get { return _全部署出力フラグ; }
            set { _全部署出力フラグ = value; }
        }

        public string 銀行ＣＤ
        {
            get
            {
                string ret = null;
                if (string.IsNullOrEmpty(_EB銀行支店ＣＤ) == false)
                {
                    // 先頭4桁を銀行ＣＤとして取得
                    if (_EB銀行支店ＣＤ.Length >= 4)
                    {
                        ret = _EB銀行支店ＣＤ.Substring(0, 4);
                    }
                }
                return ret;
            }
        }

        public string 支店ＣＤ
        {
            get
            {
                string ret = null;
                if (string.IsNullOrEmpty(_EB銀行支店ＣＤ) == false)
                {
                    // ５～７文字目を支店ＣＤとして取得
                    if (_EB銀行支店ＣＤ.Length >= 7)
                    {
                        ret = _EB銀行支店ＣＤ.Substring(4, 3);
                    }
                }
                return ret;
            }
        }


        public DateTime? 引落日
        {
            get { return _引落日; }
            set
            {
                // EBデータファイルを出力する場合のみ必須
                _引落日 = value;
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
        public bool Equals(SeikyuDataEBReCreate target)
        {
            // 引落日は検索条件ではないので比較しない
            if (部署ＣＤ == target.部署ＣＤ &&
                (全部署出力フラグ ?? false) == (target.全部署出力フラグ ?? false) &&
                EB銀行支店ＣＤ == target.EB銀行支店ＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
