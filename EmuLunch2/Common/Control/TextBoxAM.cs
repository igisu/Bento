using System.ComponentModel;
using System.Windows.Forms;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// 数字とマイナス記号のみ入力を受け付けるテキスト
    /// </summary>
    public partial class TextBoxAM : TextBoxA
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public TextBoxAM()
        {
            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
        }

        public TextBoxAM(IContainer container)
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
        /// キー入力
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            // 数字とマイナスとBackSpaceとEnterを受け付ける
            // (DeleteはKeyDownで受け付けている)
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
        #endregion
    }
}
