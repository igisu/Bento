using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// Nullも許容でき、Enterキーで次のコントロールに移る日付ピッカー
    /// </summary>
    /// <remarks>
    /// NullableDateTimePickerは背景色が設定できないので、TextBoxAと組み合わせて１つのコントロールにしている。
    /// ではなぜ組み合わせるテキストがTextBoxAYMDではないのか？
    /// データバインドしているのが、TextBoxAYMDはTextBoxAYMDであり、DateTimePickerAはDateTimePickerAなので、
    /// TextBoxAYMDにて実装しているコードは改めてDateTimePickerAにて実装し直さないといけなかったので。
    /// 
    /// これを利用してデザインする際、すでにフォームに貼り付いているこれをコピーして貼り付けると、
    /// textBoxA1とnullableDateTimePicker1が追加されてしまう。
    /// ※Designer.csを見ると分かるが、上記コントロールがControls.Addされている。
    /// この現象を回避するには、コピー＆ペーストするのではなく、ツールボックスから貼り付けること。
    /// </remarks>
    //[System.ComponentModel.DefaultBindingProperty("Text")]
    public partial class DateTimePickerA : UserControl
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public DateTimePickerA()
        {
            InitializeComponent();
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        [Browsable(true)]
        [DefaultValue("")]
        [Description("コントロールに関連づけられたテキスト。")]
        [System.ComponentModel.Bindable(true)]
        public override string Text
        {
            get
            {
                return textBoxA1.Text;
            }
            set
            {
                textBoxA1.Text = value;
                if (string.IsNullOrEmpty(value))
                {
                    nullableDateTimePicker1.Value = null;
                }
                else
                {
                    DateTime d;
                    string s = textBoxA1.Text;

                    // 'yyyy/MM/dd'のみを取得
                    if (s.Length > 10)
                    {
                        s = s.Substring(0, 10);
                    }
                    if (DateTime.TryParse(s, out d) == true)
                    {
                        nullableDateTimePicker1.Value = d;
                    }
                    else
                    {
                        nullableDateTimePicker1.Value = null;
                    }
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(false)]
        [Description("エディットコントロールの中の文字列を変更できるかどうかを設定します。")]
        public bool ReadOnly
        {
            get
            {
                return textBoxA1.ReadOnly;
            }
            set
            {
                textBoxA1.ReadOnly = value;
                nullableDateTimePicker1.Enabled = !(value);
            }
        }

        #endregion

        //**************************************
        //* イベント定義
        //**************************************
        #region イベント定義
        [Browsable(true)]
        [Description("Textプロパティの値がコントロールで変更されたときに発生するイベントです。")]
        public event EventHandler<EventArgs> TextDateChanged;
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// テキストボックス フォーカス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxA1_Enter(object sender, EventArgs e)
        {
            Binding b = this.DataBindings["Text"];
            if (b != null)
            {
                b.FormatString = "yyyy/MM/dd";      // 曜日を表示しない
            }
            base.OnEnter(e);
        }

        /// <summary>
        /// テキストボックス テキスト変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxA1_TextChanged(object sender, EventArgs e)
        {
            this.OnTextChanged(e);

            // TextChangedイベントを発生
            EventHandler<EventArgs> eventHandler = TextDateChanged;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// テキストボックス 検証中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxA1_Validating(object sender, CancelEventArgs e)
        {
            this.OnValidating(e);
        }

        /// <summary>
        /// テキストボックス 検証終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxA1_Validated(object sender, EventArgs e)
        {
            // 確定した日付をピッカーに設定する
            DateTime d;
            string s = textBoxA1.Text;

            // 'yyyy/MM/dd'のみを取得
            if (s.Length > 10)
            {
                s = s.Substring(0, 10);
            }

            if (DateTime.TryParse(s, out d) == true)
            {
                nullableDateTimePicker1.Value = d;
            }
            else
            {
                nullableDateTimePicker1.Value = null;
            }

            Binding b = this.DataBindings["Text"];
            if (b != null)
            {
                b.FormatString = "yyyy/MM/dd（ddd）";     // 曜日を表示する
            }
            this.OnValidated(e);

            if (this.DataBindings.Count > 0)
            {
                ((CurrencyManager)this.DataBindings[0].BindingManagerBase).Refresh();
            }
        }

        /// <summary>
        /// カレンダー 日付選択終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nullableDateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            if (nullableDateTimePicker1.Value != null)
            {
                textBoxA1.Text = string.Format("{0:yyyy/MM/dd}", nullableDateTimePicker1.Value);
            }

            Binding b = this.DataBindings["Text"];
            if (b != null)
            {
                b.WriteValue();
            }

            // テキストボックスにフォーカス
            textBoxA1.Focus();
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
            try
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
            catch (Exception)
            {
                // アプリケーション終了時に原因不明の例外がまれに発生する。無視する。
            }
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
