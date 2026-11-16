using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 生産予定入力検索条件
    /// </summary>
    public class SeisanYoteiCriteria : IDataErrorInfo
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
        private DateTime? _注文日;
        private int? _グループ種別ＣＤ;
        private int? _計算対象週数;

        private DateTime? _期間開始日;
        private DateTime? _期間終了日;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeisanYoteiCriteria()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            注文日 = DateTime.Today;
            グループ種別ＣＤ = null;
            計算対象週数 = 5;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SeisanYoteiCriteria(SeisanYoteiCriteria src)
        {
            部署ＣＤ = src.部署ＣＤ;
            注文日 = src.注文日;
            グループ種別ＣＤ = src.グループ種別ＣＤ;
            計算対象週数 = src.計算対象週数;
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

                // 期間開始日／終了日を求める
                GenerateKikan();
            }
        }

        public int? グループ種別ＣＤ
        {
            get { return _グループ種別ＣＤ; }
            set { _グループ種別ＣＤ = value; }
        }

        public int? 計算対象週数
        {
            get { return _計算対象週数; }
            set
            {
                _計算対象週数 = value;
                _errors.Remove("計算対象週数");
                if (value == null)
                {
                    _errors["計算対象週数"] = "計算対象週数を入力してください。";
                }
                else
                {
                    if (value <= 0)
                    {
                        _errors["計算対象週数"] = "計算対象週数は1以上を入力してください。";
                    }
                }

                // 期間開始日／終了日を求める
                GenerateKikan();
            }
        }

        public string 期間
        {
            get
            {
                if (_期間開始日 == null || _期間終了日 == null)
                {
                    return null;
                }
                return string.Format("{0:yyyy/MM/dd} ～ {1:yyyy/MM/dd}", _期間開始日, _期間終了日);
            }
        }

        public DateTime? 期間開始日
        {
            get { return _期間開始日; }
        }

        public DateTime? 期間終了日
        {
            get { return _期間終了日; }
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

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(SeisanYoteiCriteria target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                注文日 == target.注文日 &&
                (グループ種別ＣＤ ?? -1) == (target.グループ種別ＣＤ ?? -1))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 期間開始日／終了日を求める
        /// </summary>
        private void GenerateKikan()
        {
            _期間開始日 = null;
            _期間終了日 = null;

            // 注文日と計算対象週数のいずれかにエラーがある？
            if (_errors.ContainsKey("注文日") || _errors.ContainsKey("計算対象週数"))
            {
                return;
            }

            if (_注文日 == null || 計算対象週数 == null || 計算対象週数 <= 0)
            {
                return;
            }

            _期間終了日 = ((DateTime)注文日).AddDays(-7);
            _期間開始日 = ((DateTime)注文日).AddDays(-7 * (int)計算対象週数);
        }
        #endregion
    }
}
