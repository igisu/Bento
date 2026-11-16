namespace Emu.Bento.WebAPI
{
    /// <summary>
    /// JSONレスポンスヘッダーモデル
    /// </summary>
    public class ResponseHeaderModel
    {
        //**************************************
        //* 列挙値
        //**************************************
        #region 列挙値
        public enum ResultCode
        {
            OK = 1,         // 成功
            NG = 2,         // 失敗
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 処理結果（1:成功 2:失敗）
        /// </summary>
        public int result { get; set; }

        /// <summary>
        /// エラーメッセージ等の文字列
        /// </summary>
        public string message { get; set; }
        #endregion
    }
}
