using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 会計連携CSV出力条件
    /// </summary>
    public class KaikeiRenkei : IDataErrorInfo
    {
        //**************************************
        //* 列挙値
        //**************************************
        #region 列挙値
        /// <summary>
        /// 出力対象
        /// </summary>
        public enum KaikeiRenkeiTarget
        {
            Diff,       // 差分
            Full,       // 履歴から再出力
            Recalc      // 出力対象日だけを再集計
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _出力対象日;
        private KaikeiRenkeiTarget _出力対象;

        private DateTime? _集計用出力対象日;
        private DateTime? _出力対象月一日;
        private DateTime? _出力対象月末日;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public KaikeiRenkei()
        {
            出力対象 = KaikeiRenkeiTarget.Diff;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 出力対象日
        {
            get { return _出力対象日; }
            set
            {
                _出力対象日 = value;
                _集計用出力対象日 = value;
                _出力対象月一日 = null;
                _出力対象月末日 = null;
                _errors.Remove("画面用出力対象日");
                if (value == null)
                {
                    _errors["画面用出力対象日"] = "出力対象日を入力してください。";
                }
                else
                {
                    _出力対象月一日 = new DateTime(((DateTime)_出力対象日).Year, ((DateTime)_出力対象日).Month, 1);
                    _出力対象月末日 = _出力対象月一日.Value.AddMonths(1).AddDays(-1);
                }
            }
        }

        public KaikeiRenkeiTarget 出力対象
        {
            get { return _出力対象; }
            set { _出力対象 = value; }
        }

        public DateTime? 集計用出力対象日
        {
            get { return _集計用出力対象日; }
            set { _集計用出力対象日 = value; }
        }

        public DateTime? 出力対象月一日
        {
            get { return _出力対象月一日; }
        }

        public DateTime? 出力対象月末日
        {
            get { return _出力対象月末日; }
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
        /// 集計用出力対象日を出力対象日の翌日にする
        /// </summary>
        public void SetNextDay()
        {
            _集計用出力対象日 = ((DateTime)_出力対象日).AddDays(1);
        }
        #endregion
    }
}
