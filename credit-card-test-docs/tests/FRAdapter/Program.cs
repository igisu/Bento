using System;
using System.Text.Json;
using Emu.Bento.Data;
using Emu.Bento.WebAPI;

internal static class Program
{
    private static SeikyuGokei Invoice(decimal amount = 10000m) => new SeikyuGokei
    {
        請求番号 = 17,
        今回請求額 = amount,
        請求日 = new DateTime(2026, 11, 18),
        部署ＣＤ = 1,
        請求先ＣＤ = 2,
        請求先名 = "TEST"
    };

    private static int Main()
    {
        var cases = new (string Id, Action Test)[]
        {
            ("TC-CC-026", SettlementAmount),
            ("TC-CC-027", CancellationSequence),
            ("TC-CC-028", DeclineResponse),
            ("TC-CC-029", SettlementTimeout),
            ("TC-CC-030", CancellationTimeout),
            ("TC-CC-031", FeeRateResponse)
        };

        int failures = 0;
        foreach (var test in cases)
        {
            try
            {
                test.Test();
                Console.WriteLine($"{test.Id} PASS");
            }
            catch (Exception ex)
            {
                failures++;
                Console.WriteLine($"{test.Id} FAIL {ex}");
            }
        }

        Console.WriteLine($"Executed {cases.Length}; failed {failures}");
        return failures == 0 ? 0 : 1;
    }

    private static void SettlementAmount()
    {
        foreach (decimal amount in new[] { 10000m, 100m })
        {
            int calls = 0;
            var adapter = new FRSettlement
            {
                Respond = (route, json) =>
                {
                    calls++;
                    Equal("sale/settlement", route);
                    using var request = JsonDocument.Parse(json);
                    Equal(17m, request.RootElement.GetProperty("billingNumber").GetDecimal());
                    Equal(amount, request.RootElement.GetProperty("billingAmount").GetDecimal());
                    return """{"header":{"result":true},"body":{"seqNo":"SYNTHETIC-SALE"}}""";
                }
            };
            Equal(true, adapter.SaleSettlement(Invoice(amount), out var history, out _));
            Equal("SYNTHETIC-SALE", history.取引番号);
            Equal(1, calls);
        }
    }

    private static void CancellationSequence()
    {
        int calls = 0;
        var adapter = new FRSettlement
        {
            Respond = (route, json) =>
            {
                calls++;
                Equal("sale/cancel", route);
                using var request = JsonDocument.Parse(json);
                Equal("SYNTHETIC-SALE", request.RootElement.GetProperty("seqNo").GetString());
                Equal(17m, request.RootElement.GetProperty("billingNumber").GetDecimal());
                return """{"header":{"result":true},"body":{"cancelSeqNo":"SYNTHETIC-CANCEL"}}""";
            }
        };
        var history = new FRSettlementHistory { 取引番号 = "SYNTHETIC-SALE" };
        Equal(true, adapter.SaleCancel(Invoice(), ref history, out _));
        Equal("SYNTHETIC-CANCEL", history.取引番号);
        Equal(1, calls);
    }

    private static void DeclineResponse()
    {
        var adapter = new FRSettlement
        {
            Respond = (_, _) => """{"header":{"result":false,"message":"SYNTHETIC-DECLINE"}}"""
        };
        Equal(false, adapter.SaleSettlement(Invoice(), out var history, out var message));
        Equal("SYNTHETIC-DECLINE", message);
        Equal<string>(null, history.取引番号);
    }

    private static void SettlementTimeout() => CheckTimeout("sale/settlement",
        adapter => adapter.SaleSettlement(Invoice(), out _, out _));

    private static void CancellationTimeout() => CheckTimeout("sale/cancel", adapter =>
    {
        var history = new FRSettlementHistory { 取引番号 = "SYNTHETIC-SALE" };
        adapter.SaleCancel(Invoice(), ref history, out _);
    });

    private static void CheckTimeout(string expectedRoute, Action<FRSettlement> call)
    {
        int calls = 0;
        var adapter = new FRSettlement
        {
            Respond = (route, _) =>
            {
                Equal(expectedRoute, route);
                calls++;
                throw new TimeoutException("SYNTHETIC-TIMEOUT");
            }
        };
        try
        {
            call(adapter);
        }
        catch (TimeoutException ex)
        {
            Equal("SYNTHETIC-TIMEOUT", ex.Message);
            Equal(1, calls);
            return;
        }
        throw new InvalidOperationException("Expected the injected timeout to propagate");
    }

    private static void FeeRateResponse()
    {
        var adapter = new FRSettlement
        {
            Respond = (route, _) =>
            {
                Equal("info/getFeeRateList", route);
                return """{"header":{"result":true},"body":{"feeRateList":[{"feeRate":4}]}}""";
            }
        };
        Equal(true, adapter.InfoGetFeeRateList(1, 2, out var rates, out _));
        Equal(1, rates.Count);
        Equal<decimal?>(4m, rates[0].手数料率);
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!Equals(expected, actual))
            throw new InvalidOperationException($"Expected {expected}, observed {actual}");
    }
}
