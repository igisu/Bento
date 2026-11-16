using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// 日付入力テキスト
    /// </summary>
    public partial class TextBoxAYMD : TextBoxA
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public TextBoxAYMD()
        {
            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
        }

        public TextBoxAYMD(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// BindingContextが変わった
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            Binding b = this.DataBindings["Text"];
            if (b != null)
            {
                b.FormatString = "yyyy/MM/dd（ddd）";     // 曜日を表示する
                b.Parse += new ConvertEventHandler(StringToDate);
                b.Format += new ConvertEventHandler(DateToString);
            }
            base.OnBindingContextChanged(e);
        }

        /// <summary>
        /// フォーカス
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            Binding b = this.DataBindings["Text"];
            if (b != null)
            {
                b.FormatString = "yyyy/MM/dd";      // 曜日を表示しない
            }
            base.OnEnter(e);
        }

        /// <summary>
        /// 検証終了
        /// </summary>
        /// <param name="e"></param>
        protected override void OnValidated(EventArgs e)
        {
            Binding b = this.DataBindings["Text"];
            if (b != null)
            {
                b.FormatString = "yyyy/MM/dd（ddd）";     // 曜日を表示する
            }
            base.OnValidated(e);
        }

        /// <summary>
        /// 日付から文字列へのフォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateToString(object sender, ConvertEventArgs e)
        {
            if (e.DesiredType != typeof(string)) return;

            if (e.Value == null) return;

            if (e.Value is DateTime)
            {
                e.Value = ((DateTime)e.Value).ToString(((Binding)sender).FormatString);
            }
        }

        /// <summary>
        /// 文字列から日付へのパース
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StringToDate(object sender, ConvertEventArgs e)
        {
            if (e.DesiredType != typeof(DateTime?)) return;

            if (e.Value as string == "")
            {
                e.Value = null;
                return;
            }

            try
            {
                DateTime today = DateTime.Today;
                string s = (string)e.Value;
                if (Regex.IsMatch(s, "^[0-9]{1,2}$"))   // 日付のみ
                {
                    e.Value = new DateTime(today.Year, today.Month, int.Parse(s));
                }
                if (Regex.IsMatch(s, "^[0-9]{4}$"))     // 月日（3文字は 123 が 12/3 か 1/23 か区別できない）
                {
                    e.Value = new DateTime(today.Year, int.Parse(s.Substring(0, 2)), int.Parse(s.Substring(2, 2)));
                }
                if (Regex.IsMatch(s, "^[0-9]{6}$"))     // 年月日(yymmdd)
                {
                    // ２桁年の扱いはDateTimeに任せる
                    e.Value = DateTime.Parse(s.Substring(0, 2) + "/" + s.Substring(2, 2) + "/" + s.Substring(4, 2));
                }
                if (Regex.IsMatch(s, "^[0-9]{8}$"))     // 年月日(yyyymmdd)
                {
                    e.Value = DateTime.Parse(s.Substring(0, 4) + "/" + s.Substring(4, 2) + "/" + s.Substring(6, 2));
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
