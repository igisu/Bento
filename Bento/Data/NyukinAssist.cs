using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 入金入力・入力補助
    /// </summary>
    public class NyukinAssist : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _入金日;
        private int? _部署ＣＤ;
        private int? _入金方法;
        private int? _依頼人ＣＤ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NyukinAssist()
        {
            入金日 = DateTime.Today;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            入金方法 = null;
            依頼人ＣＤ = null;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 入金日
        {
            get { return _入金日; }
            set { _入金日 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 入金方法
        {
            get { return _入金方法; }
            set { _入金方法 = value; }
        }

        public int? 依頼人ＣＤ
        {
            get { return _依頼人ＣＤ; }
            set { _依頼人ＣＤ = value; }
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
