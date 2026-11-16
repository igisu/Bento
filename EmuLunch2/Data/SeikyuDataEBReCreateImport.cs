using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求EBデータ再作成_EB結果ファイル読み込み
    /// </summary>
    public class SeikyuDataEBReCreateImport : IDataErrorInfo
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
        private string _取込ファイル;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuDataEBReCreateImport()
        {
            取込ファイル = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SeikyuDataEBReCreateImport(SeikyuDataEBReCreateImport src)
        {
            部署ＣＤ = src.部署ＣＤ;
            取込ファイル = src.取込ファイル;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public string 取込ファイル
        {
            get { return _取込ファイル; }
            set
            {
                // EB結果ファイルを読み込む場合のみ必須
                _取込ファイル = value;
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
                if (_取込ファイル == null)
                {
                    return "取込ファイルを入力してください。";
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
