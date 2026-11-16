using System.Data.SqlClient;
using System.Text;

namespace Emu.Bento.Common
{
    /// <summary>
    /// SqlCommand拡張メソッドクラス
    /// </summary>
    internal static class SqlCommandExtensions
    {
        /// <summary>
        /// パラメータ情報を付加したCommandTextを返します。
        /// </summary>
        /// <param name="cmd">SqlCommand</param>
        /// <returns>末尾にパラメータ情報を付加したCommandText</returns>
        public static string CommandTextWithParameters(this SqlCommand cmd)
        {
            StringBuilder commandTextBuilder = new StringBuilder(cmd.CommandText);
            commandTextBuilder.Append(";");
            foreach (SqlParameter p in cmd.Parameters)
            {
                commandTextBuilder.AppendFormat("\r\n{0} = {1}", p.ParameterName, p.Value);
            }
            return commandTextBuilder.ToString();
        }
    }
}
