using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 入金EBデータ取込
    /// </summary>
    public class NyukinEBImport : IDataErrorInfo
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
        private DateTime? _入金日;
        private string _取込ファイル;
        private string _EB先銀行支店ＣＤ;   // 銀行ＣＤ支店ＣＤ

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NyukinEBImport()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            入金日 = DateTime.Today;
            取込ファイル = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public NyukinEBImport(NyukinEBImport src)
        {
            部署ＣＤ = src.部署ＣＤ;
            入金日 = src.入金日;
            取込ファイル = src.取込ファイル;
            EB先銀行支店ＣＤ = src.EB先銀行支店ＣＤ;
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

        public DateTime? 入金日
        {
            get { return _入金日; }
            set
            {
                _入金日 = value;
                _errors.Remove("入金日");
                if (value == null)
                {
                    _errors["入金日"] = "入金日を入力してください。";
                }
            }
        }

        public string 取込ファイル
        {
            get { return _取込ファイル; }
            set
            {
                _取込ファイル = value;
                _errors.Remove("取込ファイル");
                if (value == null)
                {
                    _errors["取込ファイル"] = "取込ファイルを入力してください。";
                }
            }
        }

        public string EB先銀行支店ＣＤ
        {
            get { return _EB先銀行支店ＣＤ; }
            set { _EB先銀行支店ＣＤ = value; }
        }

        public string 銀行ＣＤ
        {
            get
            {
                string ret = null;
                if (string.IsNullOrEmpty(_EB先銀行支店ＣＤ) == false)
                {
                    // 先頭4桁を銀行ＣＤとして取得
                    if (_EB先銀行支店ＣＤ.Length >= 4)
                    {
                        ret = _EB先銀行支店ＣＤ.Substring(0, 4);
                    }
                }
                return ret;
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
            get { return null; }
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
        public bool Equals(NyukinEBImport target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                入金日 == target.入金日 &&
                取込ファイル == target.取込ファイル &&
                EB先銀行支店ＣＤ == target.EB先銀行支店ＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
