using System;
using System.Collections.Generic;
using System.ComponentModel;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 大袋ラベル印刷条件
    /// </summary>
    public class BigBagLabelPrintCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _注文日;
        private int? _部署ＣＤ;
        private string _部署名;
        private int? _食事区分ＣＤ;
        private string _食事区分名;                     // 大袋ラベル印刷から受け取って、大袋ラベル印刷確認ダイアログに表示する
        private Consts.BigBagPrintKind _印刷種別ＣＤ;   // 1:セット印刷 2:単品印刷

        private int? _食数;                             // 大袋ラベル手動印刷で使用する（大袋ラベル印刷確認ダイアログでは使用しない）
        private int? _印刷枚数;                         // 大袋ラベル手動印刷で使用する（大袋ラベル印刷確認ダイアログでは使用しない）

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BigBagLabelPrintCriteria()
        {
            食事区分ＣＤ = null;
            食数 = 1;                   // 大袋ラベル印刷確認ダイアログでエラーとしないため
            印刷枚数 = 1;               // 大袋ラベル印刷確認ダイアログでエラーとしないため
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bigBagLabelCriteria">大袋ラベル表示条件</param>
        public BigBagLabelPrintCriteria(BigBagLabelCriteria bigBagLabelCriteria)
        {
            注文日 = bigBagLabelCriteria.注文日;
            部署ＣＤ = bigBagLabelCriteria.部署ＣＤ;
            部署名 = bigBagLabelCriteria.部署名;
            食事区分ＣＤ = null;
            食数 = 1;                   // 大袋ラベル手動印刷の初期値（兼 大袋ラベル印刷確認ダイアログでエラーとしないため）
            印刷枚数 = 1;               // 大袋ラベル手動印刷の初期値（兼 大袋ラベル印刷確認ダイアログでエラーとしないため）
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
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

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public int? 食事区分ＣＤ
        {
            get { return _食事区分ＣＤ; }
            set
            {
                _食事区分ＣＤ = value;
                _errors.Remove("食事区分ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["食事区分ＣＤ"] = "区分を入力してください。";
                }
            }
        }

        public string 食事区分名
        {
            get { return _食事区分名; }
            set { _食事区分名 = value; }
        }

        public Consts.BigBagPrintKind 印刷種別ＣＤ
        {
            get { return _印刷種別ＣＤ; }
            set { _印刷種別ＣＤ = value; }
        }

        public string 印刷種別名
        {
            get
            {
                return _印刷種別ＣＤ.DisplayName();
            }
        }

        public int? 食数
        {
            get { return _食数; }
            set
            {
                _食数 = value;
                _errors.Remove("食数");
                if (value == null || value < 0)
                {
                    _errors["食数"] = "食数を入力してください。";
                }
            }
        }

        public int? 印刷枚数
        {
            get { return _印刷枚数; }
            set
            {
                _印刷枚数 = value;
                _errors.Remove("印刷枚数");
                if (value == null || value < 0)
                {
                    _errors["印刷枚数"] = "印刷枚数を入力してください。";
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
            get{ return null;}
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
