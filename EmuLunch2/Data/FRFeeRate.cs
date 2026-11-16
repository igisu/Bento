using System;

namespace Emu.Bento.Data
{
    /// <summary>
    /// FRクレカ手数料率
    /// </summary>
    public class FRFeeRate
    {
        public int? 企業ＣＤ { get; set; }

        public int? 部署ＣＤ { get; set; }

        public int? 支払方法CD { get; set; }

        public DateTime? 開始日 { get; set; }

        public string 店舗ID { get; set; }

        public decimal? 手数料率 { get; set; }

        public DateTime? 作成日 { get; set; }

        public DateTime? 修正日 { get; set; }
    }
}
