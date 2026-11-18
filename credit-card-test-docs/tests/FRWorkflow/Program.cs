var tests = new (string Id, Func<Scenario, Task> Execute)[]
{
    ("TC-CC-034", DeclineContinues),
    ("TC-CC-035", UnknownContinues),
    ("TC-CC-036", UnknownReconciles),
    ("TC-CC-037", OverlappingSales),
    ("TC-CC-038", UnknownCancellationRecords),
    ("TC-CC-039", SaveFailureThenRetry),
    ("TC-CC-040", CompletedSaleIsSkipped),
    ("TC-CC-041", IncompleteCancellationIsSkipped),
    ("TC-CC-042", OverlappingCancellations)
};

int failures = 0;
foreach (var test in tests)
{
    var scenario = new Scenario();
    scenario.Activate();
    try
    {
        await test.Execute(scenario);
        Console.WriteLine($"{test.Id} {(test.Id == "TC-CC-042" ? "REVIEW_ONLY" : "PASS")}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"{test.Id} FAIL {ex.GetType().Name}: {ex.Message}");
    }
    Console.WriteLine($"  observed: {string.Join(", ", scenario.Events)}");
}

Console.WriteLine($"Executed {tests.Length}; failed {failures}");
return failures == 0 ? 0 : 1;

static Form513B Batch() =>
    new(Enumerable.Range(1, 10).Select(number => new SeikyuGokei(number)).ToArray());

static bool Timeout() => throw new TimeoutException("synthetic missing response");

static async Task DeclineContinues(Scenario scenario)
{
    scenario.Sale = number => number != 5;
    var form = Batch();
    await form.Run();
    Require(SalesInOrder(scenario), "All 10 invoices should reach the API");
    Require(scenario.Events.Contains("save:5"), "Explicit decline should reach the save boundary");
    Require(form.Note(4) == "synthetic decline", "The decline should be attached to the failing row");
}

static async Task UnknownContinues(Scenario scenario)
{
    scenario.Sale = number => number != 5 || Timeout();
    var form = Batch();
    await form.Run();
    Require(SalesInOrder(scenario), "All 10 invoices should be attempted after the timeout");
    Require(form.Note(4)?.Contains("synthetic missing response") == true, "Timeout should be attached to the failing row");
}

static async Task UnknownReconciles(Scenario scenario)
{
    scenario.Sale = _ => Timeout();
    var form = new Form513B(new SeikyuGokei(1));
    await form.Run();
    await form.Reconcile();
    Require(scenario.Events.Contains("save:1") || scenario.Events.Contains("inquiry:1") ||
            scenario.Events.Contains("update:1"),
        "An unknown result needs a persistence or reconciliation path for this invoice");
}

static Task OverlappingSales(Scenario scenario) =>
    Overlap(gate => scenario.Sale = gate, () => new Form513B(new SeikyuGokei(1)).Run(),
        count => Require(count == 1, $"Expected one sale request; observed {count}"));

static Task OverlappingCancellations(Scenario scenario) =>
    Overlap(gate => scenario.Cancel = gate,
        () => new Form513D(new SeikyuGokei(1, completed: true)).Run(),
        count => Console.WriteLine($"TC-CC-042 diagnostic: {count} cancel requests"));

static async Task Overlap(Action<Func<decimal, bool>> configure, Func<Task> run, Action<int> observe)
{
    var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    int count = 0;
    configure(_ =>
    {
        if (Interlocked.Increment(ref count) == 1)
        {
            entered.SetResult();
            release.Task.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        }
        return true;
    });

    var first = run();
    try
    {
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await run().WaitAsync(TimeSpan.FromSeconds(5));
    }
    finally
    {
        release.TrySetResult();
        await first;
    }
    observe(count);
}

static async Task UnknownCancellationRecords(Scenario scenario)
{
    scenario.Cancel = _ => Timeout();
    try
    {
        await new Form513D(new SeikyuGokei(1, completed: true)).Run();
    }
    catch (EmuException ex) when (ex.InnerException is TimeoutException)
    {
        // Check whether this workflow reached its persistence boundary.
    }
    Require(scenario.Events.Contains("save:1") || scenario.Events.Contains("update:1"),
        "The cancel workflow must send the unknown result to a persistence boundary");
}

static async Task SaveFailureThenRetry(Scenario scenario)
{
    scenario.Read = invoice => new SeikyuGokei(invoice);
    scenario.Save = _ => throw new InvalidOperationException("synthetic save failure");
    var first = new Form513B(new SeikyuGokei(1));
    await first.Run();
    await first.Reconcile();
    await new Form513B(new SeikyuGokei(1)).Run();
    Require(scenario.Events.Count(entry => entry == "sale:1") == 1,
        "A second workflow must not submit the invoice after approval and failed local save");
}

static async Task CompletedSaleIsSkipped(Scenario scenario)
{
    scenario.Read = invoice => new SeikyuGokei(invoice.請求番号.Value, completed: true);
    await new Form513B(new SeikyuGokei(1)).Run();
    Require(scenario.Events.IsEmpty, "A completed invoice must not reach sale or save");
}

static async Task IncompleteCancellationIsSkipped(Scenario scenario)
{
    scenario.Read = invoice => new SeikyuGokei(invoice.請求番号.Value);
    var result = await new Form513D(new SeikyuGokei(1, completed: true)).Run();
    Require(!result.Item1 && scenario.Events.IsEmpty, "An incomplete invoice must not reach cancel or save");
}

static bool SalesInOrder(Scenario scenario) =>
    scenario.Events.Where(entry => entry.StartsWith("sale:", StringComparison.Ordinal))
        .SequenceEqual(Enumerable.Range(1, 10).Select(number => $"sale:{number}"));

static void Require(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}
