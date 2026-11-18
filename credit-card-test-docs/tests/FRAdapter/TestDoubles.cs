using System;
using System.Text.Json;

namespace Emu.Bento.WebAPI
{
    public class FRResponseHeaderModel
    {
        public bool result { get; set; }
        public string message { get; set; }
    }

    // The real transport and application configuration are outside this assembly.
    public abstract class WebAPIBase
    {
        internal Func<string, string, string> Respond { get; set; } =
            (_, _) => throw new InvalidOperationException("No test response configured");

        protected abstract string GetController();
        protected string SerializeJson<T>(T data) => JsonSerializer.Serialize(data);
        protected T DeserializeJson<T>(string json) => JsonSerializer.Deserialize<T>(json);
        protected string CallMainFR(string route, string json) => Respond(route, json);
    }
}

namespace Emu.Bento.Data
{
    internal static class TableBase
    {
        internal const int _企業ＣＤ = 1;
    }

    internal class SeikyuGokei
    {
        public decimal? 請求番号 { get; set; }
        public DateTime? 請求日 { get; set; }
        public int? 部署ＣＤ { get; set; }
        public int? 請求先ＣＤ { get; set; }
        public string 請求先名 { get; set; }
        public decimal? 今回請求額 { get; set; }
    }

    internal class FRSettlementHistory : SeikyuGokei
    {
        public decimal? 履歴番号 { get; set; }
        public string 店舗ID { get; set; }
        public int? 企業ＣＤ { get; set; }
        public string 伝票番号 { get; set; }
        public int? 処理タイプ { get; set; }
        public int? 処理結果 { get; set; }
        public string 処理結果内容 { get; set; }
        public string 承認番号 { get; set; }
        public string 取引番号 { get; set; }
        public string クレカ顧客番号 { get; set; }
        public DateTime? 作成日 { get; set; }
        public DateTime? 修正日 { get; set; }
    }

    internal class FRFeeRate
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

    internal class Tokuisaki
    {
        public int? 得意先ＣＤ { get; set; }
        public string 得意先名 { get; set; }
    }
}
