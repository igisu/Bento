using System.ComponentModel;
using System.Windows.Forms;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// Enterキーで次のコントロールに移るチェックボックス
    /// </summary>
    public partial class CheckBoxA : CheckBox
    {
        public CheckBoxA()
        {
            InitializeComponent();
        }

        public CheckBoxA(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

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
    }
}
