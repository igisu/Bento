
namespace Emu.Bento.Common.Encrypt
{
    /// <summary>
    /// KeyGenerator(=Rfc2898DeriveBytes) に渡すパラメータ
    /// </summary>
    class KeyGeneratorParameter
    {
        /// <summary>
        /// パスワード
        /// </summary>
        public string Password = "";
        public readonly string Password0 = @")-^9N,hU&nzVpycqix</SRNcrV91m_cW";
        public readonly string Password1 = @"Jvg]ano%5*!<P{2&szsCbz>`Gh6]p>/`";
        public readonly string Password2 = @"k0S)rYA;s.1W;)V-,^yC""piNXX^+8XlM";
        public readonly string Password3 = @"[Q@JY_=fUl+odm<~'?}Q""ukhO!LMhS24";
        public readonly string Password4 = @"7JtiK@.iZuNT)N<W-+z|vZ4hO+R}}olg";
        public readonly string Password5 = @"~$ur!pr5h(098y|""%s{v0N^Iz]7)eR7D";
        public readonly string Password6 = @"4[ptes%RS~9LE6u:nws5jQp/L:T1*`lo";
        public readonly string Password7 = @"OgcK~g=-}$4F&Kb-i7H&]KYaT!Hgs9|m";
        public readonly string Password8 = @"}x@6..@5Na1GQqBP!AX""uhe7)}5Yl?ja";
        public readonly string Password9 = @"fdj>]PBJ6BC*|AhU/H>%N3K:{rMcJYV~";

        /// <summary>
        /// ソルト（Rfc2898DeriveBytes に渡す際には 8バイト以上の長さが必要）
        /// </summary>
        public byte[] Salt = null;
        public readonly byte[] Salt0 = { 0x0E, 0xBD, 0xC2, 0x55, 0xB1, 0x3F, 0x1E, 0x05 };
        public readonly byte[] Salt1 = { 0x1A, 0x3E, 0x0E, 0x7C, 0xB0, 0x87, 0xF3, 0x95 };
        public readonly byte[] Salt2 = { 0x4D, 0xD1, 0xB5, 0xDE, 0xB5, 0x58, 0x09, 0x81 };
        public readonly byte[] Salt3 = { 0x37, 0x57, 0x38, 0x71, 0x77, 0xFE, 0x3A, 0xFD };
        public readonly byte[] Salt4 = { 0x0F, 0xA9, 0x52, 0x8B, 0x83, 0x7D, 0x38, 0x3F };
        public readonly byte[] Salt5 = { 0xA7, 0x23, 0x4F, 0xE4, 0xB3, 0xC4, 0x5B, 0x5F };
        public readonly byte[] Salt6 = { 0x3D, 0x22, 0xC6, 0xEC, 0xF4, 0xE0, 0xA3, 0x23 };
        public readonly byte[] Salt7 = { 0x8E, 0xF8, 0x3B, 0x34, 0x37, 0xED, 0x7D, 0xBC };
        public readonly byte[] Salt8 = { 0xBA, 0xFF, 0x06, 0x12, 0x94, 0x94, 0x7D, 0xD8 };
        public readonly byte[] Salt9 = { 0x36, 0x05, 0x30, 0xC0, 0x41, 0xE5, 0xEE, 0x59 };

        /// <summary>
        /// 反復回数(Rfc2898DeriveBytes に渡す際には 1000 未満だと例外)
        /// </summary>
        public int Iterations = 1000;
        public readonly int Iterations0 = 7675;
        public readonly int Iterations1 = 4820;
        public readonly int Iterations2 = 40175;
        public readonly int Iterations3 = 72796;
        public readonly int Iterations4 = 2256;
        public readonly int Iterations5 = 34423;
        public readonly int Iterations6 = 6724;
        public readonly int Iterations7 = 13257;
        public readonly int Iterations8 = 38575;
        public readonly int Iterations9 = 1922;
    }
}
