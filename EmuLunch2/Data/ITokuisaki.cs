using System;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先インターフェース
    /// </summary>
    /// <remarks>
    /// CalcHaisobi(), AdjustSeizobiHaisobi()用に、Tokuisaki と ChumonTokuisaki に実装
    /// </remarks>
    public interface ITokuisaki
    {
        int? 得意先ＣＤ
        {
            get;
            set;
        }

        string 休日設定
        {
            get;
            set;
        }

        /// <summary>
        /// 休日設定の該当の曜日の値を返す
        /// </summary>
        /// <param name="dayOfWeek">休日設定の曜日インデックス</param>
        /// <returns>休日の場合はtrue。それ以外はfalse。</returns>
        bool GetHoliday(DayOfWeek dayOfWeek);
    }
}
