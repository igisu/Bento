using System.Data;
using System.Text.RegularExpressions;

namespace Emu.Bento.Common
{
    /// <summary>
    /// DataTable拡張メソッドクラス
    /// </summary>
    internal static class DataTableExtension
    {
        /// <summary>
        /// 条件に当てはまるレコードをDataTableから削除します。
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="filter">削除条件</param>
        /// <remarks>削除対象の行をマークするだけなので、RejectChangesすると復活します。</remarks>
        public static void Delete(this DataTable dt, string filter)
        {
            DataRow[] rows = dt.Select(filter);

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].RowState != DataRowState.Deleted)
                {
                    rows[i].Delete();
                }
            }
        }

        /// <summary>
        /// 条件に当てはまるレコードをDataTableから削除します。
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="filter">削除条件</param>
        /// <remarks>削除した後は RejectChanges しても復活できません。</remarks>
        public static void Remove(this DataTable dt, string filter)
        {
            dt.Delete(filter);
            dt.AcceptChanges();
        }

        /// <summary>
        /// 検索文字列をエスケープします。
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="filter">検索文字列</param>
        /// <returns>エスケープ結果文字列</returns>
        /// <remarks>Like検索に使う検索文字列をエスケープする場合は、EscapeForLikeを使用してください。</remarks>
        public static string Escape(this DataTable dt, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return filter;
            }
            return filter.Replace("'", "''");
        }

        /// <summary>
        /// Like検索文字列をエスケープします。
        /// </summary>
        /// <param name="dt">データテーブル</param>
        /// <param name="filter">検索文字列</param>
        /// <returns>エスケープ結果文字列</returns>
        /// <remarks>前後の「*」または「%」は結果に含まれません。</remarks>
        public static string EscapeForLike(this DataTable dt, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return filter;
            }
            return Regex.Replace(Escape(dt, filter), @"([\[\]*%])", "[$1]");
        }
    }
}
