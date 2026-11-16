using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// 小数入力テキスト
    /// </summary>
    public partial class TextBoxAD : TextBoxA
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 小数点以下桁数
        /// </summary>
        private int _decimalDigit;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public TextBoxAD()
        {
            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
        }

        public TextBoxAD(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        /// <summary>
        /// 小数点以下桁数
        /// </summary>
        [Category("動作")]
        [DefaultValue(2)]
        [Description("小数点以下桁数を指定します。")]
        public int DecimalDigit
        {
            get { return _decimalDigit; }
            set
            {
                if (value < 0 || 10 < value)
                {
                    throw new ArgumentOutOfRangeException("DecimalDigit", value, "小数点以下桁数には0以上10以下の値を指定してください。");
                }
                _decimalDigit = value;
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// キー入力
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '0':
                case '-':
                case '.':
                case (char)Keys.Back:
                case (char)Keys.Return:
                    break;

                default:
                    e.Handled = true;
                    break;
            }
            base.OnKeyPress(e);
        }

        /// <summary>
        /// キー押下
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    if (e.Control)
                    {
                        // Ctrl+C
                        this.Copy();
                    }
                    break;

                case Keys.V:
                    if (e.Control)
                    {
                        // Ctrl+V
                        if (Clipboard.ContainsText())
                        {
                            this.Paste();
                        }
                    }
                    break;

                case Keys.X:
                    if (e.Control)
                    {
                        // Ctrl+X
                        this.Cut();
                    }
                    break;

                case Keys.Z:
                    if (e.Control)
                    {
                        // Ctrl+Z
                        // アンドゥができるか調べる
                        if (this.CanUndo)
                        {
                            this.Undo();        // アンドゥする
                            this.ClearUndo();   // アンドゥバッファを削除する
                        }
                    }
                    break;
            }
            base.OnKeyDown(e);
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
                if (_decimalDigit == 0)
                {
                    b.FormatString = "0";
                }
                else
                {
                    b.FormatString = "0." + new string('0', _decimalDigit); // 小数点を表示する
                }
            }
            base.OnValidated(e);
        }

        /// <summary>
        /// BindingContextが変わった
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            try
            {
                base.OnBindingContextChanged(e);

                Binding b = this.DataBindings["Text"];
                if (b != null)
                {
                    b.Parse += new ConvertEventHandler(Decimal_Parse);
                }
            }
            catch (Exception)
            {
                // アプリケーション終了時に原因不明の例外がまれに発生する。無視する。
            }
        }

        /// <summary>
        /// 小数点以下2桁で四捨五入する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Decimal_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value as decimal? != null)
            {
                e.Value = Math.Round((decimal)e.Value, _decimalDigit, MidpointRounding.AwayFromZero);
            }
        }

        #endregion


    }
}
