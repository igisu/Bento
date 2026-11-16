using System;
using System.Collections.Generic;
using System.ComponentModel;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 受注発信一覧検索条件
    /// </summary>
    public class HassinListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _担当者ＣＤ;
        private DateTime? _注文日;
        private bool? _不要非表示;
        private bool? _注文済み非表示;
        private bool? _配送済み非表示;
        private int? _再表示間隔;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HassinListCriteria()
        {
            担当者ＣＤ = UserData.getInstance().担当者ＣＤ;
            注文日 = DateTime.Today;
            不要非表示 = false;
            注文済み非表示 = false;
            配送済み非表示 = false;
            再表示間隔 = SystemMaster.Get204RefreshInterval();      // システム設定テーブルから取得
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public HassinListCriteria(HassinListCriteria src)
        {
            担当者ＣＤ = src.担当者ＣＤ;
            注文日 = src.注文日;
            不要非表示 = src.不要非表示;
            注文済み非表示 = src.注文済み非表示;
            配送済み非表示 = src.配送済み非表示;
            再表示間隔 = src.再表示間隔;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set { _担当者ＣＤ = value; }
        }

        public DateTime? 注文日
        {
            get { return _注文日; }
            set
            {
                _注文日 = value;
                _errors.Remove("注文日");
                if (value == null)
                {
                    _errors["注文日"] = "注文日を入力してください。";
                }
            }
        }

        public bool? 不要非表示
        {
            get { return _不要非表示; }
            set { _不要非表示 = value; }
        }

        public bool? 注文済み非表示
        {
            get { return _注文済み非表示; }
            set { _注文済み非表示 = value; }
        }

        public bool? 配送済み非表示
        {
            get { return _配送済み非表示; }
            set { _配送済み非表示 = value; }
        }

        public int? 再表示間隔
        {
            get { return _再表示間隔; }
            set { _再表示間隔 = value; }
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
        public bool Equals(HassinListCriteria target)
        {
            // 再表示間隔は無視する
            if (担当者ＣＤ == target.担当者ＣＤ &&
                注文日 == target.注文日 &&
                (不要非表示 ?? false) == (target.不要非表示 ?? false) &&
                (注文済み非表示 ?? false) == (target.注文済み非表示 ?? false) &&
                (配送済み非表示 ?? false) == (target.配送済み非表示 ?? false))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
