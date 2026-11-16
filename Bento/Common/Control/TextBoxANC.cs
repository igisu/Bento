using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// カンマ編集テキスト
    /// </summary>
    /// <remarks>これを使用する際は、「フォーマットと詳細バインド」ダイアログ（プロパティの(DataBindings)の(詳細)）にて、[形式の種類]に数字を指定してください。</remarks>
    public partial class TextBoxANC : TextBoxAN
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public TextBoxANC()
        {
            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
        }

        public TextBoxANC(IContainer container)
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
        /// フォーカス
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            Binding b = this.DataBindings["Text"];
            if (b != null)
            {
                b.FormatString = "";        // フォーマットを適用しない
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
                b.FormatString = "N0";      // カンマ区切りのフォーマットを適用
            }
            base.OnValidated(e);
        }
        #endregion
    }
}
