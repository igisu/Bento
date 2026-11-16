using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 一括入金入力条件
    /// </summary>
    public class IkkatsuNyukin : IDataErrorInfo
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
        private int? _入金方法;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IkkatsuNyukin()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            入金日 = DateTime.Today;
            入金方法 = null;
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

        public int? 入金方法
        {
            get { return _入金方法; }
            set
            {
                _入金方法 = value;
                _errors.Remove("入金方法");
                if (value == null || value < 0)
                {
                    _errors["入金方法"] = "入金方法を入力してください。";
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
            get { return null;}
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
