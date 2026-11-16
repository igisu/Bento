
namespace Emu.Bento.Common.Encrypt
{
    /// <summary>
    /// KeyGeneratorの実装です。
    /// </summary>
    class KeyGeneratorImpl : KeyGenerator
    {
        private const string Password0 = @"]}xcN5Z2'BV=vKz$n6|FzoX}0[H,@$Vh";
        private const string Password1 = @"XboGuyfw$V`@dspfs1jrUh~rnG?o}L(1";
        private const string Password2 = @"*Ps<B4uIPn21**y/B\_*-!aIEvi[o.Tv";
        private const string Password3 = @"C[rH8gB~uFca]sr3/nW6Kk2Tvm5+C+A*";
        private const string Password4 = @"t}q&Uqe3='^(us8'SC*-f._3`Fu#vMM$";
        private const string Password5 = @"F+n[+dNB#&iy7Jm6GG8'z:y^U3M!A7q:";
        private const string Password6 = @"Y?5EE/`WUo;7[>u0!R{sAQGJ1/k00yO9";
        private const string Password7 = @"7S`ZE^@<3V/<fg>1H(6d=w[B$,Ee~muh";
        private const string Password8 = @">qa'3f+WN90lst,<ZX75A%dbgi$Is.v,";
        private const string Password9 = @"~d5;@m9=`fl\a)gE?]J]MF(s@x$1;Ids";

        private readonly byte[] Salt0 = { 0x99, 0x89, 0xD4, 0x6A, 0x28, 0x18, 0x8E, 0xB3};
        private readonly byte[] Salt1 = { 0xC1, 0x7A, 0x33, 0xA0, 0x08, 0xF4, 0x99, 0x3B};
        private readonly byte[] Salt2 = { 0x7F, 0xC8, 0x01, 0xF3, 0xF3, 0xA8, 0x6B, 0x06};
        private readonly byte[] Salt3 = { 0x27, 0xA3, 0x8F, 0x0E, 0x18, 0x0C, 0xA3, 0x30};
        private readonly byte[] Salt4 = { 0xAF, 0xA4, 0xC8, 0x79, 0x8D, 0x59, 0x02, 0x4C};
        private readonly byte[] Salt5 = { 0xF7, 0x67, 0xAA, 0x8E, 0x40, 0x4C, 0x1F, 0x8A};
        private readonly byte[] Salt6 = { 0x02, 0x30, 0x95, 0xA4, 0xCE, 0x93, 0xCC, 0x7A};
        private readonly byte[] Salt7 = { 0xE2, 0x72, 0xBE, 0xE6, 0x85, 0xCD, 0xB9, 0xC6};
        private readonly byte[] Salt8 = { 0x41, 0x7D, 0x8B, 0x33, 0x17, 0xA8, 0x0A, 0x37};
        private readonly byte[] Salt9 = { 0x1C, 0x29, 0x31, 0x1B, 0x5D, 0x79, 0x48, 0xBE};

        private const int Iterations0 = 2985;
        private const int Iterations1 = 7279;
        private const int Iterations2 = 8518;
        private const int Iterations3 = 7771;
        private const int Iterations4 = 4811;
        private const int Iterations5 = 1449;
        private const int Iterations6 = 2556;
        private const int Iterations7 = 8831;
        private const int Iterations8 = 3641;
        private const int Iterations9 = 7695;

        /// <summary>
        /// KeyGeneratorImpl クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="keyGeneratorParameter">キーと初期化ベクタ(IV)の生成に必要なパラメータ。</param>
        public KeyGeneratorImpl(KeyGeneratorParameter keyGeneratorParameter)
            : base(keyGeneratorParameter)
        {
        }

        /// <summary>
        /// 加工したパスワードを返します。
        /// </summary>
        /// <param name="password">加工前のパスワード。</param>
        /// <returns>加工後のパスワード。</returns>
        protected override string ArrangePassword(string password)
        {
            return Password3 + password;
        }

        /// <summary>
        /// 加工したソルトを返します。
        /// </summary>
        /// <param name="salt">加工前のソルト。</param>
        /// <returns>加工後のソルト。</returns>
        protected override byte[] ArrangeSalt(byte[] salt)
        {
            int size = 0;
            int i = 0;

            // 既存のバイト配列のサイズを求める
            if (salt != null)
            {
                size = salt.Length;
            }

            // 新しいバイト配列を生成
            byte[] newSalt = new byte[size + Salt4.Length];

            // 先頭の8バイトを Salt4 で埋める
            foreach (byte b in Salt4)
            {
                newSalt[i] = b;
                i++;
            }

            // 既存のバイト配列の値で、残りを埋める
            if (salt != null)
            {
                foreach (byte b in salt)
                {
                    newSalt[i] = b;
                    i++;
                }
            }

            return newSalt;
        }

        /// <summary>
        /// 加工した反復回数を返します。
        /// </summary>
        /// <param name="iterations">加工前の反復回数。</param>
        /// <returns>加工後の反復回数。</returns>
        protected override int ArrangeIterations(int iterations)
        {
            return Iterations0 + iterations;
        }
    }
}
