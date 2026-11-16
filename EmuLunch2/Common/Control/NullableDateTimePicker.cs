using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// Nullも許容でき、Enterキーで次のコントロールに移る日付ピッカー
    /// ただし、背景色が設定できないという制限がある（※）ので、これ単体での使用は想定せず、
    /// TextBoxAYMDと重ねたユーザーコントロールでの使用を推奨する。
    /// ※onPaintをオーバーライドして描画する方法もあるが、全ての描画を自力がやる必要があるので、年・月・日ごとの選択ができない
    /// </summary>
    public partial class NullableDateTimePicker : DateTimePicker
    {
        private static Color SELECTED_BACK_COLOR = Color.FromArgb(255, 224, 192);

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// Nullかどうか
        /// </summary>
        private bool _isNull;

        /// <summary>
        /// Null値の時の画面表示
        /// </summary>
        private string _nullValue;

        /// <summary>
        /// DateTimePickerのFormat
        /// </summary>
        private DateTimePickerFormat _pickerFormat;

        /// <summary>
        /// DateTimePickerのCustomFormat
        /// </summary>
        private string _customFormat;

        /// <summary>
        /// CustomFormatの値
        /// </summary>
        private string _formatAsString;

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// DateTimePickerのCustomFormat
        /// </summary>
        public new String CustomFormat
        {
            get
            {
                return this._customFormat;
            }
            set
            {
                this._customFormat = value;
            }
        }

        /// <summary>
        /// DateTimePickerのFormat
        /// </summary>
        public new DateTimePickerFormat Format
        {
            get
            {
                return this._pickerFormat;
            }
            set
            {
                this._pickerFormat = value;
                this.SetFormat();
                base.OnFormatChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Null値の時の画面表示
        /// </summary>
        [Browsable(true)]
        [DefaultValue("")]
        [Description("null値の場合のテキスト表示")]
        public String NullValue
        {
            get
            {
                return this._nullValue;
            }
            set
            {
                this._nullValue = value;
            }
        }

        /// <summary>
        /// 値
        /// </summary>
        public new DateTime? Value
        {
            get
            {
                if (this._isNull)
                {
                    return null;
                }
                return base.Value;
            }
            set
            {
                if (value == null ||
                    Convert.IsDBNull(value) ||
                    (!string.IsNullOrWhiteSpace(this._nullValue) &&
                    this._nullValue == value.ToString()))
                {
                    this.SetToNullValue();
                }
                else
                {
                    // 表現できる範囲を超えている場合は補正する
                    if (value < this.MinDate)
                    {
                        value = this.MinDate;
                    }
                    if (value > this.MaxDate)
                    {
                        value = this.MaxDate;
                    }
                    this.SetToDateTimeValue();
                    base.Value = (DateTime)value;
                }
            }
        }

        /// <summary>
        /// CustomFormatの値
        /// </summary>
        private string FormatAsString
        {
            get
            {
                return this._formatAsString;
            }
            set
            {
                this._formatAsString = value;
                base.CustomFormat = value;
            }
        }

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public NullableDateTimePicker()
            : base()
        {
            InitializeComponent();

            base.Format = DateTimePickerFormat.Custom;
            this._nullValue = null;
            this.Format = DateTimePickerFormat.Long;
        }

        public NullableDateTimePicker(IContainer container) : base()
        {
            container.Add(this);
            InitializeComponent();

            base.Format = DateTimePickerFormat.Custom;
            this._nullValue = null;
            this.Format = DateTimePickerFormat.Long;
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        [System.Security.Permissions.UIPermission(
        System.Security.Permissions.SecurityAction.Demand,
        Window = System.Security.Permissions.UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //Returnキーが押されているか調べる
            //AltかCtrlキーが押されている時は、本来の動作をさせる
            if (((keyData & Keys.KeyCode) == Keys.Return) &&
                ((keyData & (Keys.Alt | Keys.Control)) == Keys.None))
            {
                Form f = (Form)this.FindForm();
                //次のタブオーダーのコントロールにフォーカスを移動させる
                //Shiftキーが押されている時は、逆順にする
                bool result = f.SelectNextControl(this, ((keyData & Keys.Shift) != Keys.Shift), true, true, true);

                //Tabキーを押した時と同じ動作をさせる
                //Shiftキーが押されている時は、逆順にする
                //f.ProcessTabKey((keyData & Keys.Shift) != Keys.Shift);


                //本来の処理はさせない
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// OnCloseUpのオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCloseUp(EventArgs e)
        {
            if (System.Windows.Forms.Control.MouseButtons == MouseButtons.None && this._isNull)
            {
                this.SetToDateTimeValue();
                this._isNull = false;
            }
            base.OnCloseUp(e);
        }

        /// <summary>
        /// OnKeyUpのオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    this.Value = null;
                    base.OnValueChanged(EventArgs.Empty);
                    e.Handled = true;
                    break;
            }
            base.OnKeyUp(e);
        }

        /// <summary>
        /// OnKeyPressのオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (this._isNull && Char.IsDigit(e.KeyChar))
            {
                // 数字
                this.Value = base.Value;
                e.Handled = true;
                SendKeys.Send("{RIGHT}");
                SendKeys.Send(e.KeyChar.ToString());
                return;
            }
            else if (e.KeyChar == '\b')
            {
                // backspace key
                this.Value = null;
                base.OnValueChanged(EventArgs.Empty);
                e.Handled = true;
            }
            base.OnKeyPress(e);
        }

        /// <summary>
        /// フォーカス取得
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            this.BackColor = SELECTED_BACK_COLOR;
            //this.SelectAll();
            base.OnEnter(e);
        }

        /// <summary>
        /// 検証終了（フォーカスが離れる最後のイベント）
        /// </summary>
        /// <param name="e"></param>
        protected override void OnValidated(EventArgs e)
        {
            this.ResetBackColor();
            base.OnValidated(e);
        }

        /// <summary>
        /// Formatプロパティの設定
        /// </summary>
        private void SetFormat()
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            DateTimeFormatInfo dateTimeFormat = cultureInfo.DateTimeFormat;
            switch (this._pickerFormat)
            {
                case DateTimePickerFormat.Long:
                    this.FormatAsString = dateTimeFormat.LongDatePattern;
                    break;
                case DateTimePickerFormat.Short:
                    this.FormatAsString = dateTimeFormat.ShortDatePattern;
                    break;
                case DateTimePickerFormat.Time:
                    this.FormatAsString = dateTimeFormat.ShortTimePattern;
                    break;
                case DateTimePickerFormat.Custom:
                    this.FormatAsString = this.CustomFormat;
                    break;
            }
        }

        /// <summary>
        /// DateTime時の値の設定
        /// </summary>
        private void SetToDateTimeValue()
        {
            if (this._isNull)
            {
                this.SetFormat();
                this._isNull = false;
                base.OnValueChanged(new EventArgs());
            }
        }

        /// <summary>
        /// null時の値の設定
        /// </summary>
        private void SetToNullValue()
        {
            this._isNull = true;
            base.CustomFormat = string.IsNullOrEmpty(this._nullValue) ? " " : this._nullValue;
        }
        #endregion
    }
}
