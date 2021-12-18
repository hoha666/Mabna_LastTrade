using MabnaLibrary;

/// <summary>
/// Adding procedure to DB and then calling it
/// this is one of the fastest ways
/// </summary>
await DbContext.CreateProcedure();
DbContext.CallProcedure("LastTradeProc");

Console.Clear();
Console.WriteLine("LastTrade Created !!!");

await JsonUtil.GetAsync(null, new CancellationToken());
Console.WriteLine("LastTrade JSON File Created Too --> C:\\temp\\LastTrade.txt");