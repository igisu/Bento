using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Emu.Bento.Common.Encrypt
{
    /// <summary>
    /// データを暗号化し、また、復号します。
    /// </summary>
    public class SecretData
    {
        /// <summary>
        /// 高度暗号化標準 (AES: Advanced Encryption Standard) 対称アルゴリズムのマネージ実装
        /// </summary>
        private AesManaged _AES;

        /// <summary>
        /// ソルトの素
        /// </summary>
        private const string SALT_BASE = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

        /// <summary>
        /// ソルトのサイズ
        /// </summary>
        private const int SALT_SIZE = 10;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SecretData()
        {
            KeyGeneratorParameter parameter = new KeyGeneratorParameter();
            KeyGeneratorImpl generator = new KeyGeneratorImpl(parameter);

            _AES = new AesManaged();
            _AES.Key = generator.Key(_AES.KeySize / 8);
        }


        /// <summary>
        /// 文字列を暗号化します。(HMAC付き)
        /// </summary>
        /// <param name="plainText">暗号化対象の文字列（平文）。</param>
        /// <returns>暗号化→BASE64でエンコードされた文字列。</returns>
        /// <remarks>
        /// 暗号化するとバイナリデータ（バイト配列）になってしまいます。
        /// それを文字列として取得すべく、BASE64でエンコードしています。
        /// さらに改竄が検出できるようにハッシュ メッセージ認証コード(HMAC)が付いています。
        /// </remarks>
        public string HashedEncrypt(string plainText)
        {
            MemoryStream ms = null;     // 暗号化されたバイト配列が格納されるストリーム
            CryptoStream cs = null;     // データストリームを暗号変換にリンクするストリーム
            StreamWriter sw = null;     // 文字を書き込むストリーム

            try
            {
                // 平文にソルトを付加
                plainText = GetSalt(SALT_SIZE) + plainText;

                // 初期化ベクタ(IV)を生成
                _AES.GenerateIV();

                // 暗号化オブジェクトを生成
                ICryptoTransform encryptor = _AES.CreateEncryptor(_AES.Key, _AES.IV);

                // 暗号化に使用するストリームを生成
                ms = new MemoryStream();
                cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                sw = new StreamWriter(cs);

                // 平文のHMAC(ハッシュ メッセージ認証コード)を取得し、先頭に付加する。
                byte[] hashValue = GetHash(_AES.Key, plainText);
                ms.Write(hashValue, 0, hashValue.Length);

                // 暗号化のたびに生成したIVを出力する。
                // (IVは秘密情報とは見なされない)
                ms.Write(_AES.IV, 0, _AES.IV.Length);

                // 暗号化
                sw.Write(plainText);
            }
            finally
            {
                // ストリームを閉じる
                if (sw != null) sw.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }

            // バイト配列をBASE64でエンコード
            byte[] encrypted = ms.ToArray();
            string cipherText = Convert.ToBase64String(encrypted);
            return cipherText;
        }

        /// <summary>
        /// 暗号化されている文字列を復号します。(HMAC付き)
        /// </summary>
        /// <param name="cipherText">暗号化→BASE64でエンコードされた文字列。</param>
        /// <returns>復号された文字列（平文）。</returns>
        /// <remarks>
        /// ハッシュ メッセージ認証コード(HMAC)付きの文字列を復号します。
        /// </remarks>
        /// <exception cref="CryptographicException">暗号文が改竄されています。</exception>
        public string HashedDecrypt(string cipherText)
        {
            MemoryStream ms = null;     // 暗号化されたバイト配列が格納されるストリーム
            CryptoStream cs = null;     // データストリームを暗号変換にリンクするストリーム
            StreamReader sr = null;     // 文字を読み込むストリーム

            // 無効な文字列はそのまま返す
            if (string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            try
            {
                // 暗号文をBASE64でデコード
                byte[] encrypted = Convert.FromBase64String(cipherText);

                ms = new MemoryStream(encrypted);

                // 暗号文に含まれているHMACを取得
                byte[] storedHash = new byte[new HMACSHA256().HashSize / 8];
                ms.Read(storedHash, 0, storedHash.Length);

                // 復号に使用するIVを暗号文の先頭から取得
                byte[] IV = new byte[_AES.IV.Length];
                ms.Read(IV, 0, IV.Length);

                // 復号オブジェクトを生成
                ICryptoTransform decryptor = _AES.CreateDecryptor(_AES.Key, IV);

                // 復号に使用するストリームを生成
                cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                sr = new StreamReader(cs);

                // 復号
                string plainText = sr.ReadToEnd();

                // 復号した平文のHMAC(ハッシュ メッセージ認証コード)を取得し、暗号文に含まれていたそれと比較
                byte[] computedHash = GetHash(_AES.Key, plainText);
                for (int i = 0; i < storedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        System.Diagnostics.Debug.Print("storedHash   = {0}", BitConverter.ToString(storedHash));
                        System.Diagnostics.Debug.Print("computedHash = {0}", BitConverter.ToString(computedHash));
                        throw new CryptographicException("HMACが一致しません。暗号文が改竄されています。");
                    }
                }

                // 復号した平文からソルト(に該当する箇所)を除去
                plainText = plainText.Substring(SALT_SIZE);

                return plainText;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // ストリームを閉じる
                if (sr != null) sr.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }

        /// <summary>
        /// ハッシュ メッセージ認証コード(HMAC)を取得します。
        /// </summary>
        /// <param name="key">キー。</param>
        /// <param name="target">メッセージ。</param>
        /// <returns>キーとメッセージから求めたハッシュ メッセージ認証コード。</returns>
        private byte[] GetHash(byte[] key, string target)
        {
            HMACSHA256 hmac = new HMACSHA256(key);      // HMAC(ハッシュ メッセージ認証コード)生成オブジェクト
            byte[] hashValue;
            MemoryStream ms = null;
            StreamWriter sw = null;

            try
            {
                ms = new MemoryStream();
                sw = new StreamWriter(ms);
                sw.Write(target);
                sw.Flush();

                ms.Position = 0;
                hashValue = hmac.ComputeHash(ms);

                return hashValue;
            }
            finally
            {
                if (sw != null) sw.Close();
                if (ms != null) ms.Close();
            }
        }

        /// <summary>
        /// 任意の長さのランダムな文字列を返します。
        /// </summary>
        /// <param name="size">返す文字列の長さ。</param>
        /// <returns>指定した長さを持つランダムな文字列。</returns>
        private string GetSalt(int size)
        {
            StringBuilder salt = new StringBuilder();
            byte[] randomNumber = new byte[1];
            RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();

            for (int i = 0; i < size; i++)
            {
                gen.GetBytes(randomNumber);
                int rand = (int)randomNumber[0];
                int index = rand % SALT_BASE.Length;
                salt.Append(SALT_BASE.Substring(index, 1));
            }
            System.Diagnostics.Debug.Print("Salt={0}", salt);
            return salt.ToString();
        }

    }
}
