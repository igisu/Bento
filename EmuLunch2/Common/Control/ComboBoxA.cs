using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// Enterキーで次のコントロールに移るコンボボックス
    /// </summary>
    public partial class ComboBoxA : ComboBox
    {
        private static Color DEFAULT_BACK_COLOR = SystemColors.Control;
        private static Brush DEFAULT_BACK_BRUSH = SystemBrushes.Control;
        private static Color SELECTED_BACK_COLOR = Color.FromArgb(255,224,192);

        public ComboBoxA()
        {
            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.BackColor = DEFAULT_BACK_COLOR;
        }

        public ComboBoxA(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.BackColor = DEFAULT_BACK_COLOR;
        }

        [System.Security.Permissions.UIPermission(
        System.Security.Permissions.SecurityAction.Demand,
        Window = System.Security.Permissions.UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.DroppedDown == false)
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
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Enterイベント発生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(System.EventArgs e)
        {
            if (this.DesignMode) return;

            this.BackColor = SELECTED_BACK_COLOR;
            base.OnEnter(e);
        }

        /// <summary>
        /// EnabledChangedイベント発生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnabledChanged(System.EventArgs e)
        {
            if (this.Enabled)
            {
                this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            }
            else
            {
                this.DrawMode = System.Windows.Forms.DrawMode.Normal;
            }
            base.OnEnabledChanged(e);
        }

        /// <summary>
        /// Leaveイベント発生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLeave(System.EventArgs e)
        {
            this.BackColor = DEFAULT_BACK_COLOR;
            base.OnLeave(e);
        }

        /// <summary>
        /// アイテム描画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Graphics grfx = e.Graphics;
            Rectangle rect = e.Bounds;

            if (e.Index < 0)
            {
                return;
            }

            Brush brush = SystemBrushes.Window;

            if ((e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit)
            {
                brush = DEFAULT_BACK_BRUSH;
                //brush = new LinearGradientBrush(rect, SystemColors.Control, SystemColors.ControlLight, LinearGradientMode.Vertical);
            }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                brush = new SolidBrush(this.BackColor);
            }

            grfx.FillRectangle(brush, rect);

            string text = "";
            if (this.Items[e.Index] is DataRowView)
            {
                DataRowView drv = (DataRowView)this.Items[e.Index];
                text = drv.Row[this.DisplayMember].ToString();
            }
            else
            {
                text = this.Items[e.Index].ToString();
            }

            // e.ForeColor は HilightText(デフォルトでは白)なので、明るい背景色では見えなくなる。
            // this.ForeColor は WindowText(デフォルトでは黒)なので、その色で描画する。
            //grfx.DrawString(text, e.Font, new SolidBrush(e.ForeColor), rect);
            grfx.DrawString(text, e.Font, new SolidBrush(this.ForeColor), rect);
            e.DrawFocusRectangle();
        }
    }
}
