
namespace Emu.Bento.Data
{
    /// <summary>
    /// ログインユーザーデータ
    /// </summary>
    public class UserData : Tantousya
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        private static UserData _instance = null;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private UserData()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        private UserData(Tantousya tantousya) : base(tantousya)
        {
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// インスタンスを返します。
        /// </summary>
        /// <returns></returns>
        public static UserData getInstance()
        {
            return _instance;
        }

        /// <summary>
        /// 担当者マスタの内容でインスタンスを生成します。
        /// </summary>
        /// <param name="tantousya">担当者マスタ</param>
        public static void setInstance(Tantousya tantousya)
        {
            _instance = new UserData(tantousya);
        }
        #endregion
    }
}