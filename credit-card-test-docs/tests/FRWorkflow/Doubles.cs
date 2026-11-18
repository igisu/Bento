using System.Data;
using System.Drawing;

sealed class Scenario
{
    private static readonly AsyncLocal<Scenario> Current = new();
    internal static Scenario Active => Current.Value ?? throw new InvalidOperationException("No scenario");
    internal void Activate() => Current.Value = this;

    internal Func<SeikyuGokei, SeikyuGokei> Read = invoice => invoice;
    internal Func<decimal, bool> Sale = _ => true;
    internal Func<decimal, bool> Cancel = _ => true;
    internal Func<decimal, bool> Inquiry = _ => false;
    internal Func<decimal, bool> Save = _ => true;
    internal readonly System.Collections.Concurrent.ConcurrentQueue<string> Events = new();
    internal void Record(string operation, decimal invoice) => Events.Enqueue($"{operation}:{invoice}");
}

sealed class SeikyuGokei
{
    internal SeikyuGokei(decimal number, bool completed = false)
    {
        請求番号 = number;
        クレカ決済完了フラグ = completed;
    }

    internal SeikyuGokei(SeikyuGokei source)
        : this(source.請求番号.Value, source.クレカ決済完了フラグ)
    {
        クレカ決済エラー区分 = source.クレカ決済エラー区分;
    }

    internal decimal? 請求番号;
    internal bool クレカ決済完了フラグ;
    internal int クレカ決済エラー区分;
    internal string Code => 請求番号.ToString();
    internal static SeikyuGokei GetFRRecord(SeikyuGokei invoice) => Scenario.Active.Read(invoice);
    internal bool Update()
    {
        Scenario.Active.Record("update", 請求番号.Value);
        return true;
    }
}

sealed class FRSettlementHistory
{
    internal FRSettlementHistory() { }
    internal FRSettlementHistory(DataRow row) => 請求番号 = (decimal)row[0];
    internal decimal? 請求番号;
    internal int 処理結果;
    internal bool IsMatch(FRSettlementHistory other) =>
        throw new InvalidOperationException("History comparison is outside this harness");
}

sealed class FRSettlement
{
    internal bool SaleSettlement(SeikyuGokei invoice, out FRSettlementHistory history, out string message)
    {
        var number = invoice.請求番号.Value;
        Scenario.Active.Record("sale", number);
        bool success = Scenario.Active.Sale(number);
        history = new FRSettlementHistory
        {
            請求番号 = number,
            処理結果 = (int)(success ? Consts.FRProcessResult.Success : Consts.FRProcessResult.Fail)
        };
        message = success ? null : "synthetic decline";
        return success;
    }

    internal bool SaleCancel(SeikyuGokei invoice, ref FRSettlementHistory history, out string message)
    {
        var number = invoice.請求番号.Value;
        Scenario.Active.Record("cancel", number);
        message = null;
        return Scenario.Active.Cancel(number);
    }

    internal bool InfoGetSettlement(SeikyuGokei invoice, out FRSettlementHistory history, out string message)
    {
        var number = invoice.請求番号.Value;
        Scenario.Active.Record("inquiry", number);
        history = new FRSettlementHistory { 請求番号 = number };
        message = "synthetic inquiry response";
        return Scenario.Active.Inquiry(number);
    }
}

sealed class ProgressState
{
    internal int Value;
    internal string Message;
}

sealed class Binding
{
    internal object DataSource;
    internal int Position;
    internal object this[int index] => ((DataView)DataSource)[index];
    internal void ResetItem(int index) { }
}

sealed class Sheet
{
    internal sealed class Cell
    {
        internal string Note;
        internal Color ForeColor;
    }

    internal sealed class CellCollection
    {
        private readonly Dictionary<(int, int), Cell> cells = new();
        internal Cell this[int row, int column]
        {
            get
            {
                if (!cells.TryGetValue((row, column), out var cell))
                {
                    cell = new Cell();
                    cells.Add((row, column), cell);
                }
                return cell;
            }
        }
    }

    internal int ActiveRowIndex;
    internal readonly CellCollection Cells = new();
    internal void AddSelection(int row, int column, int rows, int columns) { }
}

sealed class EmuException(Exception cause) : Exception("Workflow failed", cause);
sealed class TestLog
{
    internal void Error(Exception error) { }
}
static class History
{
    internal static void Save(string message) { }
    internal static void Error(Exception error) { }
}

partial class Form513B
{
    private readonly List<SeikyuGokei> _seikyuGokeiList;
    private readonly Dictionary<decimal, bool> _settleResponseResultList = new();
    private readonly List<FRSettlementHistory> _saveHistoryList = new();
    private readonly Sheet fpSpread1_Sheet1 = new();
    private readonly Binding bindingSource1;
    private readonly TestLog log = new();

    internal Form513B(params SeikyuGokei[] invoices)
    {
        _seikyuGokeiList = invoices.ToList();
        bindingSource1 = new Binding { DataSource = _seikyuGokeiList };
    }

    internal Task Run() => SettleExec(new ProgressSink(), new ProgressState());
    internal Task Reconcile() => ConsistencyExec(null, new ProgressState());
    internal string Note(int row) => fpSpread1_Sheet1.Cells[row, SETTLEMENT_STATUS_COLUMN].Note;
    private bool Save(SeikyuGokei invoice, FRSettlementHistory history)
    {
        Scenario.Active.Record("save", invoice.請求番号.Value);
        return Scenario.Active.Save(invoice.請求番号.Value);
    }

    private sealed class ProgressSink : IProgress<ProgressState>
    {
        public void Report(ProgressState value) { }
    }
}

partial class Form513D
{
    private SeikyuGokei _seikyuGokei;
    private bool _cancelResponseResult;
    private readonly Binding bindingSource1;
    private readonly TestLog log = new();

    internal Form513D(SeikyuGokei invoice)
    {
        _seikyuGokei = invoice;
        var table = new DataTable();
        table.Columns.Add("SyntheticInvoice", typeof(decimal));
        table.Rows.Add(invoice.請求番号.Value);
        bindingSource1 = new Binding { DataSource = table.DefaultView };
    }

    internal Task<(bool, string)> Run() => CancelExec();
    private bool Save(SeikyuGokei invoice, FRSettlementHistory history)
    {
        Scenario.Active.Record("save", invoice.請求番号.Value);
        return Scenario.Active.Save(invoice.請求番号.Value);
    }
}
