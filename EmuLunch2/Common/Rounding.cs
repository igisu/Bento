using System;

namespace Emu.Bento.Common
{
    /// <summary>
    /// 丸め処理クラス
    /// </summary>
    public static class Rounding
    {
        //**************************************
        //* 列挙値
        //**************************************
        #region 列挙値
        /// <summary>
        /// 端数区分
        /// </summary>
        public enum HasuKubun
        {
            /// <summary>
            /// 切り上げ
            /// </summary>
            Ceiling = 1,

            /// <summary>
            /// 切り捨て
            /// </summary>
            Floor = 2,

            /// <summary>
            /// 四捨五入
            /// </summary>
            Round = 3
        }
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド

        /// <summary>
        /// 端数処理
        /// </summary>
        /// <param name="target">端数処理対象の数値</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>丸めた数値</returns>
        public static decimal Round(decimal target, int? hasuKubun)
        {
            HasuKubun hasuKubunEnum = HasuKubun.Floor;    // 初期値は切り捨て
            if (hasuKubun != null)
            {
                hasuKubunEnum = (HasuKubun)(int)hasuKubun;
            }
            return Round(target, hasuKubunEnum);
        }

        /// <summary>
        /// 端数処理
        /// </summary>
        /// <param name="target">端数処理対象の数値</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>丸めた数値</returns>
        public static decimal Round(decimal target, HasuKubun hasuKubun)
        {
            decimal result = target;

            switch (hasuKubun)
            {
                case HasuKubun.Floor:        // 切り捨て
                    result = Math.Floor(target);
                    break;

                case HasuKubun.Ceiling:      // 切り上げ
                    result = Math.Ceiling(target);
                    break;

                default:                    // 四捨五入
                    result = Math.Round(target, MidpointRounding.AwayFromZero);
                    break;
            }
            return result;
        }
        #endregion


    }
}
