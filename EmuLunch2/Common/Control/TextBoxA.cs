using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// Enterキーで次のコントロールに移るテキストボックス
    /// </summary>
    public partial class TextBoxA : TextBox
    {
        private static Color SELECTED_BACK_COLOR = Color.FromArgb(255,224,192);

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public TextBoxA()
        {
            InitializeComponent();
        }

        public TextBoxA(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        //protected override void OnPaint(PaintEventArgs pe)
        //{
        //    base.OnPaint(pe);
        //}

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        /// <summary>
        /// Textコピー元コントロール
        /// </summary>
        [Category("動作")]
        [Description("このプロパティを設定すると、コントロールがフォーカスを持っていてTextプロパティが空の時に右カーソルキーの押下で、このプロパティのTextでコントロールのTextが設定されます。")]
        public System.Windows.Forms.Control CopyFromWhenEmpty { get; set; }
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
                    b.Parse += new ConvertEventHandler(NullableText_Parse);
                }
            }
            catch (Exception)
            {
                // アプリケーション終了時に原因不明の例外がまれに発生する。無視する。
            }
        }

        /// <summary>
        /// フォーカス取得
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            if (this.DesignMode) return;

            this.BackColor = SELECTED_BACK_COLOR;
            this.SelectAll();
            base.OnEnter(e);
        }

        /// <summary>
        /// キー押下
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // ・自分自身のTextがブランク
            // ・コピー元コントロールが取得できている
            // ・コピー元コントロールのTextがブランクではない
            // ・右カーソルキーが押下された
            // これらがすべて成立する場合にコピー元コントロールのTextを自分自身のTextに設定する
            if (e.KeyCode == Keys.Right)
            {
                if (string.IsNullOrEmpty(this.Text))
                {
                    if (CopyFromWhenEmpty != null && string.IsNullOrEmpty(CopyFromWhenEmpty.Text) == false)
                    {
                        this.Text = CopyFromWhenEmpty.Text;
                        this.SelectionStart = this.Text.Length;     // カーソルを末尾に移動する（これをしないと先頭から1文字目に位置される）
                    }
                }
            }
            base.OnKeyDown(e);
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
        /// Nullを許容するTextboxへの空文字列の入力をNullに変換する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void NullableText_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value as string == "")
            {
                e.Value = null;
            }
        }
        #endregion
    }
}
