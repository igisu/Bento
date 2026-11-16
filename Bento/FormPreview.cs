using System.Windows.Forms;

namespace Emu.Bento
{
    /// <summary>
    /// アクティブレポートプレビュー
    /// </summary>
    public partial class FormPreview : Form
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// タイトル
        /// </summary>
        private string _title = "";

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public FormPreview()
        {
            InitializeComponent();
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                this.Text = string.Format("プレビュー - {0}", _title);
            }
        }
        #endregion
    }
}
