using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// 時刻入力テキスト
    /// </summary>
    public partial class TextBoxAHMS : TextBoxA
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public TextBoxAHMS()
        {
            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
        }

        public TextBoxAHMS(IContainer container)
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
                b.FormatString = "HH:mm:ss";    // コロンを表示する
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
                b.FormatString = "HHmmss";      // コロンを表示しない
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
                b.FormatString = "HH:mm:ss";    // コロンを表示する
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
                string s = (string)e.Value;
                if (Regex.IsMatch(s, "^[0-9]{1,2}$"))   // 時
                {
                    e.Value = new DateTime(1, 1, 1, int.Parse(s), 0, 0);
                }
                if (Regex.IsMatch(s, "^[0-9]{4}$"))     // 時分
                {
                    e.Value = new DateTime(1, 1, 1, int.Parse(s.Substring(0, 2)), int.Parse(s.Substring(2, 2)), 0);
                }
                if (Regex.IsMatch(s, "^[0-9]{6}$"))     // 時分秒
                {
                    e.Value = new DateTime(1, 1, 1, int.Parse(s.Substring(0, 2)), int.Parse(s.Substring(2, 2)), int.Parse(s.Substring(4, 2)));
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
