using System;

namespace Emu.Bento.Common
{
    /// <summary>
    /// ログ出力済みを示す例外
    /// </summary>
    class BentoException : Exception
    {
        public BentoException() : base() { }

        public BentoException(Exception ex) : base("例外が発生しました。", ex) { }

        public BentoException(string s) : base(s) { }
    }
}
