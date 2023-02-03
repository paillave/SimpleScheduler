using Paillave.Scheduler.Core;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services
        .AddSingleton(new MyTickEmitterConnection() as ITickEmitterConnection<MyJobDefinition, int>)
        .AddScheduler<MyJobDefinition, int>())
        // .AddScheduler(new MyTickEmitterConnection()))
    .Build();

host.RunAsync();

var jobDefinitionService = host.Services.GetService<IJobDefinitionService<MyJobDefinition, int>>();

jobDefinitionService?.SetJobDefinition(new MyJobDefinition { CronExpression = "*/2 * * * *", Id = 1 });
jobDefinitionService?.SetJobDefinition(new MyJobDefinition { CronExpression = "* * * * *", Id = 1 });

Console.ReadKey();

host.StopAsync();

public class MyJobDefinition
{
    public int Id { get; set; }
    public string? CronExpression { get; set; }
}

public class MyTickEmitterConnection : ITickEmitterConnection<MyJobDefinition, int>
{
    public void Execute(MyJobDefinition source, CancellationToken stoppingToken) => Console.WriteLine($"executing {source.Id}");
    public IEnumerable<MyJobDefinition> GetAll() => new MyJobDefinition[] {  };
    public string? GetCronExpression(MyJobDefinition source) => source.CronExpression;
    public int GetKey(MyJobDefinition source) => source.Id;
}
