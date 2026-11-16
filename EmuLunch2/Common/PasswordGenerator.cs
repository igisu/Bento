using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emu.Bento.Common
{
    /// <summary>
    /// パスワード付きzipファイルの解凍用パスワード生成クラス
    /// </summary>
    /// <returns></returns>
    internal static class PasswordGenerator
    {
        // パスワード文字列
        // constにしないのは、コンパイル時の文字列結合の最適化をしないようにするため
        static readonly string PASSWORD0 = "K]F2&O+x9(B=Ai{e";
        static readonly string PASSWORD1 = "p{]YeHxv7zdc0U>L";
        static readonly string PASSWORD2 = "5la[9nD+Wp[nc%jT";
        static readonly string PASSWORD3 = "7G$HS_.R}AiYW1CL";
        static readonly string PASSWORD4 = "Ax(Dvrp:h1$>7u-,";
        static readonly string PASSWORD5 = "bJQ}6`SrLG^5TBN>";
        static readonly string PASSWORD6 = "J3-$tUhXz4Fps3o9";
        static readonly string PASSWORD7 = "s:,Z~teTH`{z9An$";
        static readonly string PASSWORD8 = "`}-tP1Q*bIp($jAE";
        static readonly string PASSWORD9 = "[-;&I7.LI0iZO>@Z";

        /// <summary>
        /// パスワードを返します。
        /// </summary>
        /// <returns>パスワード</returns>
        internal static string GetRP()
        {
            StringBuilder s = new StringBuilder();

            s.Append(PASSWORD2);
            s.Append(PASSWORD6);
            s.Append(PASSWORD4);
            s.Append(PASSWORD3);
            s.Append(PASSWORD8);
            s.Append(PASSWORD9);
            s.Append(PASSWORD7);
            s.Append(PASSWORD1);
            s.Append(PASSWORD0);
            s.Append(PASSWORD5);

            string p = s.ToString();
            s.Clear();

            int[] indexes = { 152, 15, 157, 125, 144, 121, 141, 98, 129, 88, 52, 100, 132, 30, 18, 13, 14, 11, 50, 86, 94, 106, 74, 84, 33, 119, 87, 89, 110, 78, 120, 101 };
            for (int i = 0; i < indexes.Length; i++)
            {
                s.Append(p[indexes[i]]);
            }
            return s.ToString();
        }
    }
}
