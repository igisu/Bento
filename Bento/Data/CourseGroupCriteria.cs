using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// コース別グループマスタ（検索条件用）
    /// </summary>
    public class CourseGroupCriteria : IDataErrorInfo
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
        private int? _グループ種別ＣＤ;
        private string _グループ種別名;
        private int? _コースＣＤ;
        private int? _コース区分;
        private string _コース名;
        private bool? _無効フラグ;

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

        public int? グループ種別ＣＤ
        {
            get { return _グループ種別ＣＤ; }
            set { _グループ種別ＣＤ = value; }
        }

        public string グループ種別名
        {
            get { return _グループ種別名; }
            set { _グループ種別名 = value; }
        }

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

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
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
