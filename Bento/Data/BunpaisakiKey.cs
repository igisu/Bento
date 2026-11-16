using System;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 分配先キー（売掛分配入力で使用する）
    /// </summary>
    public class BunpaisakiKey : IEquatable<BunpaisakiKey>
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <param name="uriageKubun">売上区分</param>
        public BunpaisakiKey(int tokuisakiCD, int syohinCD, int uriageKubun)
        {
            得意先ＣＤ = tokuisakiCD;
            商品ＣＤ = syohinCD;
            売上区分 = uriageKubun;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ { get; set; }
        public int? 商品ＣＤ { get; set; }
        public int? 売上区分 { get; set; }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。（IEquatable<T>.Equalsの実装）
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(BunpaisakiKey target)
        {
            if (target == null)
            {
                return false;
            }

            if (得意先ＣＤ == target.得意先ＣＤ &&
                商品ＣＤ == target.商品ＣＤ &&
                売上区分 == target.売上区分)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。（Object.Equals()のオーバーライド）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return base.Equals(obj);

            if ((obj is BunpaisakiKey) == false)
            {
                throw new InvalidCastException("The 'obj' argument is not a BunpaisakiKey object.");
            }
            else
            {
                return Equals(obj as BunpaisakiKey);
            }
        }

        /// <summary>
        /// 特定の型のハッシュ関数として機能します。
        /// </summary>
        /// <returns>現在の BunpaisakiKey のハッシュ コード。</returns>
        public override int GetHashCode()
        {
            return string.Format("{0},{1},{2}", 得意先ＣＤ, 商品ＣＤ, 売上区分).GetHashCode();
        }
        #endregion
    }
}
