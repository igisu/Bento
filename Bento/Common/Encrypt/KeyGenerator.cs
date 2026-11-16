using System;
using System.Security.Cryptography;

namespace Emu.Bento.Common.Encrypt
{
    /// <summary>
    /// KeyGeneratorParameterを受け取り、(Rfc2898DeriveBytesを使用して)キーと初期化ベクタ(IV)を生成します。
    /// キーと初期化ベクタ(IV)を生成する前に、暗号強度を高めるべく、必要に応じて、KeyGeneratorParameterが持つパスワード/ソルト/反復回数を
    /// サブクラスにて加工することもできます。
    /// </summary>
    abstract class KeyGenerator
    {
        /// <summary>
        /// キー生成ルーチン
        /// </summary>
        private Rfc2898DeriveBytes _keyDeriver;

        /// <summary>
        /// KeyGenerator クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="keyGeneratorParameter">キーと初期化ベクタ(IV)の生成に必要なパラメータ。</param>
        public KeyGenerator(KeyGeneratorParameter keyGeneratorParameter)
        {
            if (keyGeneratorParameter == null)
            {
                throw new ArgumentNullException("keyGeneratorParameter");
            }

            // (サブクラスにて)加工されたパラメータを保持する
            KeyGeneratorParameter arrangedParameter = new KeyGeneratorParameter();
            arrangedParameter.Password = ArrangePassword(keyGeneratorParameter.Password5);
            arrangedParameter.Salt = ArrangeSalt(keyGeneratorParameter.Salt8);
            arrangedParameter.Iterations = ArrangeIterations(keyGeneratorParameter.Iterations9);

            // キー生成ルーチンを生成
            _keyDeriver = new Rfc2898DeriveBytes(arrangedParameter.Password,
                                                 arrangedParameter.Salt,
                                                 arrangedParameter.Iterations);
        }

        /// <summary>
        /// パスワード/ソルト/反復回数から求めた、暗号化/復号に必要なキーを返します。
        /// </summary>
        /// <param name="size">対称暗号アルゴリズムが必要としているキーの長さ。</param>
        /// <returns>キーを格納したバイト配列。</returns>
        /// <remarks>
        /// パスワード/ソルト/反復回数は、コンストラクタの引数（KeyGeneratorParameter）で渡されたもの
        /// （さらに必要に応じてサブクラスにして加工されたもの）を使用します。
        /// </remarks>
        public virtual byte[] Key(int size)
        {
            return _keyDeriver.GetBytes(size);
        }

        /// <summary>
        /// パスワード/ソルト/反復回数から求めた、暗号化/復号に必要な初期化ベクタ(IV)を返します。
        /// </summary>
        /// <param name="size">対称暗号アルゴリズムが必要としている初期化ベクタ(IV)の長さ。</param>
        /// <returns>初期化ベクタ(IV)を格納したバイト配列。</returns>
        /// <remarks>
        /// パスワード/ソルト/反復回数は、コンストラクタの引数（KeyGeneratorParameter）で渡されたもの
        /// （さらに必要に応じてサブクラスにして加工されたもの）を使用します。
        /// </remarks>
        public virtual byte[] IV(int size)
        {
            return _keyDeriver.GetBytes(size);
        }

        #region 抽象メソッド
        /// <summary>
        /// 加工したパスワードを返します。
        /// </summary>
        /// <param name="password">加工前のパスワード。</param>
        /// <returns>加工後のパスワード。</returns>
        protected abstract string ArrangePassword(string password);

        /// <summary>
        /// 加工したソルトを返します。
        /// </summary>
        /// <param name="salt">加工前のソルト。</param>
        /// <returns>加工後のソルト。</returns>
        /// <remarks>サブクラスにて実装する場合は、8 バイト以上の長さの配列を返すようにしてください。</remarks>
        protected abstract byte[] ArrangeSalt(byte[] salt);

        /// <summary>
        /// 加工した反復回数を返します。
        /// </summary>
        /// <param name="iterations">加工前の反復回数。</param>
        /// <returns>加工後の反復回数。</returns>
        /// <remarks>サブクラスにて実装する場合は、1000 以上の値を返すようにしてください。</remarks>
        protected abstract int ArrangeIterations(int iterations);

        #endregion
    }
}
