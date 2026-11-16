
namespace Emu.Bento
{
    /// <summary>
    /// レポートのベース
    /// </summary>
    public class BaseReport : GrapeCity.ActiveReports.SectionReport
    {
        /// <summary>
        /// 帳票のタイトル
        /// </summary>
        public virtual string Title
        {
            get
            {
                // サブクラスで override されるのを期待する。
                // Ｑ．virtual ではなく abstract にすれば必ず実装されるのに、なぜしない？
                // Ａ．abstract クラスにする必要があり、そうするとレポートがデザイナで開けなくなる。
                return "※※※ タイトル未設定 ※※※";
            }
        }

        /// <summary>
        /// 帳票のタイトル（用紙サイズと向き）
        /// </summary>
        public virtual string TitleWithSize
        {
            get
            {
                // サブクラスで override されるのを期待する。
                // Ｑ．virtual ではなく abstract にすれば必ず実装されるのに、なぜしない？
                // Ａ．abstract クラスにする必要があり、そうするとレポートがデザイナで開けなくなる。
                return "※※※ タイトル未設定 ※※※（用紙サイズ不明）";
            }
        }
    }
}
