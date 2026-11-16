using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Emu.Bento.Common.Control
{
    /// <summary>
    /// ふりがな取得テキストボックス
    /// </summary>
    public partial class TextBoxAF : TextBoxA
    {
        //**************************************
        //* デリゲート
        //**************************************
        #region デリゲート
        public delegate void ImeCompositionEventHandler(object sender, ImeCompositionEventArgs e);
        #endregion

        //**************************************
        //* イベント定義
        //**************************************
        #region イベント定義
        /// <summary>
        /// IMEで編集中の文字列 (ひらがな)
        /// </summary>
        [Description("IMEで編集中の文字列 (ひらがな)")]
        public event ImeCompositionEventHandler ImeCompositionHira = null;

        /// <summary>
        /// IMEで編集中の文字列 (1バイトカタカナ)
        /// </summary>
        [Description("IMEで編集中の文字列 (1バイトカタカナ)")]
        public event ImeCompositionEventHandler ImeCompositionKata = null;

        /// <summary>
        /// 変換確定(ひらがな)
        /// </summary>
        [Description("変換確定(ひらがな)")]
        public event ImeCompositionEventHandler ImeCompositedHira = null;

        /// <summary>
        /// 変換確定(1バイトカタカナ)
        /// </summary>
        [Description("変換確定(1バイトカタカナ)")]
        public event ImeCompositionEventHandler ImeCompositedKata = null;
        #endregion

        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体
        /// <summary>
        /// 取得したふりがなを渡すイベントデータ
        /// </summary>
        [Serializable]
        public class ImeCompositionEventArgs : System.EventArgs
        {
            private string str;

            public ImeCompositionEventArgs()
            {
            }
            public ImeCompositionEventArgs(string str)
            {
                this.str = str;
            }

            public string InputString
            {
                set
                {
                    this.str = value;
                }
                get
                {
                    return this.str;
                }
            }
        }
        #endregion

        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// IMEでキーが押されたかのフラグ
        /// </summary>
        private const int WM_IME_COMPOSITION = 0x010F;

        /// <summary>
        /// 変換確定後文字取得に使用する値(ひらがな)
        /// </summary>
        private const int GCS_RESULTSTR = 0x0800;

        /// <summary>
        /// 変換確定後文字取得に使用する値(1バイトカタカナ)
        /// </summary>
        private const int GCS_RESULTREADSTR = 0x0200;

        /// <summary>
        /// IME入力中文字取得に使用する値(ひらがな)
        /// </summary>
        private const int GCS_COMPSTR = 0x0008;

        /// <summary>
        /// IME入力中文字取得に使用する値(1バイトカタカナ)
        /// </summary>
        private const int GCS_COMPREADSTR = 0x0001;
        #endregion

        //**************************************
        //* API
        //**************************************
        #region API
        [DllImport("Imm32.dll")]
        private static extern int ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll")]
        private static extern int ImmGetCompositionString(
            int hIMC, int dwIndex, StringBuilder lpBuf, int dwBufLen);

        [DllImport("Imm32.dll")]
        private static extern bool ImmReleaseContext(IntPtr hWnd, int hIMC);
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public TextBoxAF()
        {
            InitializeComponent();
        }

        public TextBoxAF(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            try
            {
                // ふり仮名取得処理
                // 変換確定(1バイトカタカナ)
                if (ImeCompositedKata != null && m.Msg == WM_IME_COMPOSITION)
                {
                    if (((int)m.LParam & GCS_RESULTREADSTR) > 0)
                    {
                        int hIMC = ImmGetContext(this.Handle);
                        int strLen = ImmGetCompositionString(hIMC, GCS_RESULTREADSTR, null, 0);
                        StringBuilder str = new StringBuilder(strLen);

                        ImmGetCompositionString(hIMC, GCS_RESULTREADSTR, str, str.Capacity);
                        ImmReleaseContext(this.Handle, hIMC);

                        if (ImeCompositedKata != null)
                        {
                            // 環境によって文字コードが違うので、それにあわせる
                            byte[] tmp1 = System.Text.Encoding.Default.GetBytes(str.ToString());
                            byte[] tmp2 = new byte[strLen];
                            Array.Copy(tmp1, 0, tmp2, 0, strLen);
                            ImeCompositedKata(this, new ImeCompositionEventArgs(System.Text.Encoding.Default.GetString(tmp2)));
                        }
                    }
                }

                // 変換確定(ひらがな)
                if (ImeCompositedHira != null && m.Msg == WM_IME_COMPOSITION)
                {
                    if (((int)m.LParam & GCS_RESULTSTR) > 0)
                    {
                        int hIMC = ImmGetContext(this.Handle);
                        int strLen = ImmGetCompositionString(hIMC, GCS_RESULTSTR, null, 0);
                        StringBuilder str = new StringBuilder(strLen);

                        ImmGetCompositionString(hIMC, GCS_RESULTSTR, str, str.Capacity);
                        ImmReleaseContext(this.Handle, hIMC);

                        if (ImeCompositedHira != null)
                        {
                            // 環境によって文字コードが違うので、それにあわせる
                            byte[] tmp1 = System.Text.Encoding.Default.GetBytes(str.ToString());
                            byte[] tmp2 = new byte[strLen];
                            Array.Copy(tmp1, 0, tmp2, 0, strLen);
                            ImeCompositedHira(this, new ImeCompositionEventArgs(System.Text.Encoding.Default.GetString(tmp2)));
                        }
                    }
                }

                // IMEで編集中の文字列 (1バイトカタカナ)
                if (ImeCompositionKata != null && m.Msg == WM_IME_COMPOSITION)
                {
                    if (((int)m.LParam & GCS_COMPREADSTR) > 0)
                    {
                        int hIMC = ImmGetContext(this.Handle);
                        int strLen = ImmGetCompositionString(hIMC, GCS_COMPREADSTR, null, 0);
                        StringBuilder str = new StringBuilder(strLen);

                        ImmGetCompositionString(hIMC, GCS_COMPREADSTR, str, str.Capacity);
                        ImmReleaseContext(this.Handle, hIMC);

                        if (ImeCompositionKata != null)
                        {
                            // 環境によって文字コードが違うので、それにあわせる
                            byte[] tmp1 = System.Text.Encoding.Default.GetBytes(str.ToString());
                            byte[] tmp2 = new byte[strLen];
                            Array.Copy(tmp1, 0, tmp2, 0, strLen);
                            ImeCompositionKata(this, new ImeCompositionEventArgs(System.Text.Encoding.Default.GetString(tmp2)));
                        }
                    }
                }

                // IMEで編集中の文字列 (ひらがな)
                if (ImeCompositionHira != null && m.Msg == WM_IME_COMPOSITION)
                {
                    if (((int)m.LParam & GCS_COMPSTR) > 0)
                    {
                        int hIMC = ImmGetContext(this.Handle);
                        int strLen = ImmGetCompositionString(hIMC, GCS_COMPSTR, null, 0);
                        StringBuilder str = new StringBuilder(strLen);

                        ImmGetCompositionString(hIMC, GCS_COMPSTR, str, str.Capacity);
                        ImmReleaseContext(this.Handle, hIMC);

                        if (ImeCompositionHira != null)
                        {
                            // 環境によって文字コードが違うので、それにあわせる
                            byte[] tmp1 = System.Text.Encoding.Default.GetBytes(str.ToString());
                            byte[] tmp2 = new byte[strLen];
                            Array.Copy(tmp1, 0, tmp2, 0, strLen);
                            ImeCompositionHira(this, new ImeCompositionEventArgs(System.Text.Encoding.Default.GetString(tmp2)));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 例外が発生しても無視する
            }

            base.WndProc(ref m);
        }
        #endregion
    }
}
