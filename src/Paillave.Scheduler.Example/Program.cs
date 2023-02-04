using Paillave.Scheduler.Core;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddScheduler<MyJobDefinition, int, MyBatchSetup>())
    .Build();

host.RunAsync();
Console.ReadKey();
host.StopAsync();

public class MyJobDefinition
{
    public int Id { get; set; }
    public string? CronExpression { get; set; }
}

public class MyBatchSetup : IBatchSetup<MyJobDefinition, int>
{
    public void Execute(MyJobDefinition source, CancellationToken stoppingToken)
        => Console.WriteLine($"executing {source.Id}");
    public IEnumerable<MyJobDefinition> GetAll()
        => new[] {
            new MyJobDefinition
            {
                Id = 1,
                CronExpression = "*/2 * * * *"
            } };
    public string? GetCronExpression(MyJobDefinition source)
        => source.CronExpression;
    public int GetKey(MyJobDefinition source)
        => source.Id;
}
