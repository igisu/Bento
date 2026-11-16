using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求書出力条件
    /// </summary>
    public class SeikyuCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private bool? _単価ゼロフラグ;          // 単価が0の売上を出力するかどうか
        private bool? _今回売上のみを出力;      // 前回請求額、入金額、繰越金額を出力しない
        private bool? _回収予定日を出力しない;  // 回収予定日を出力するか(空欄にするかどうか)
        private bool? _振替口座印字フラグ;      // 請求先の支払方法が「4:振替」 かつ これがONの場合は、振替口座を印字する

        #endregion


        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuCriteria()
        {
            単価ゼロフラグ = false;
            今回売上のみを出力 = false;
            回収予定日を出力しない = false;
            振替口座印字フラグ = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SeikyuCriteria(SeikyuCriteria src)
        {
            単価ゼロフラグ = src.単価ゼロフラグ;
            今回売上のみを出力 = src.今回売上のみを出力;
            回収予定日を出力しない = src.回収予定日を出力しない;
            振替口座印字フラグ = src.振替口座印字フラグ;
        }

        #endregion


        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public bool? 単価ゼロフラグ
        {
            get { return _単価ゼロフラグ; }
            set { _単価ゼロフラグ = value; }
        }

        public bool? 今回売上のみを出力
        {
            get { return _今回売上のみを出力; }
            set { _今回売上のみを出力 = value; }
        }

        public bool? 回収予定日を出力しない
        {
            get { return _回収予定日を出力しない; }
            set { _回収予定日を出力しない = value; }
        }

        public bool? 振替口座印字フラグ
        {
            get { return _振替口座印字フラグ; }
            set { _振替口座印字フラグ = value; }
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
    }
}
