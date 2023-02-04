# Scheduler.Net

[![Scheduler.NET](streams.jpg)](https://paillave.github.io/SimpleScheduler/)
[Go to full documentation](https://paillave.github.io/SimpleScheduler/)

## Presentation

Scheduler.NET is a **scheduler for .NET** workers that heavily relies on **Dependency Injection**. Therefore, the scheduling setup is directly linked to the business layer. Any business layer can interact with the scheduler as well.

| Package | nuget version | nuget downloads |
|-|-|-|
| Paillave.Scheduler.Core | [![NuGet](https://img.shields.io/nuget/v/Paillave.Scheduler.Core.svg)](https://www.nuget.org/packages/Paillave.Scheduler.Core) | [![NuGet](https://img.shields.io/nuget/dt/Paillave.Scheduler.Core.svg)](https://www.nuget.org/packages/Paillave.Scheduler.Core) |

## Examples

### Simple Job Runner

```
dotnet new worker -o TestWorker
cd TestWorker
dotnet add package Paillave.Scheduler.Core
```

```csharp
using Paillave.Scheduler.Core;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddScheduler<MyJobDefinition, int, MyBatchSetup>())
    .Build()
    .RunAsync();

public class MyJobDefinition
{
    public int Id { get; set; }
    public string? CronExpression { get; set; }
}

public class MyBatchSetup : IBatchSetup<MyJobDefinition, int>
{
    public void Execute(MyJobDefinition source, CancellationToken stoppingToken)
        => Console.WriteLine($"{source.Id}: Write something on console, but could be anything");
    public IEnumerable<MyJobDefinition> GetAll()
        => new[] { new MyJobDefinition { Id = 1, CronExpression = "*/2 * * * *" } };
    public string? GetCronExpression(MyJobDefinition source)
        => source.CronExpression;
    public int GetKey(MyJobDefinition source)
        => source.Id;
}
```

### Job Runner driven by web api

```
dotnet new webapi -o TestWebApi
cd TestWebApi
dotnet add package Paillave.Scheduler.Core
```

```csharp
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Paillave.Scheduler.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton(new List<BatchDefinition>())
    .AddTransient<IBatchService, BatchService>()
    .AddScheduler<BatchDefinition, int, MyBatchConfiguration>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", (
    [FromServices] IBatchService batchService)
    => batchService.GetAll());

app.MapDelete("/{id}", (
    [FromServices] IBatchService batchService,
    [FromRoute(Name = "id")] int id)
    => batchService.Delete(id));

app.MapPost("/", (
    [FromServices] IBatchService batchService,
    [FromBody] BatchDefinition batchDefinition)
    => batchService.Save(batchDefinition));

app.MapPut("/{id}", (
    [FromServices] IJobDefinitionService<BatchDefinition, int> jobDefinitionService,
    [FromRoute(Name = "id")] int id)
    => jobDefinitionService.Trigger(id));
app.Run();

/// <summary>
/// Description of batch definition
/// It can be anything: an EF entity, a business object... anything
/// </summary>
public class BatchDefinition
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CronExpression { get; set; }
    public string? Command { get; set; }
    public string? Arguments { get; set; }
}

/// <summary>
/// Contract of persistence layer for batch definitions
/// </summary>
public interface IBatchService
{
    int Save(BatchDefinition batch);
    void Delete(int batchId);
    List<BatchDefinition> GetAll();
}

/// <summary>
/// Persistence layer for batch definitions
/// It is done in-memory, but obviously any persistence can be injected and used
/// </summary>
public class BatchService : IBatchService
{
    private readonly List<BatchDefinition> _batchDefinitions;
    private readonly IJobDefinitionService<BatchDefinition, int> _jobDefinitionService;
    public BatchService(List<BatchDefinition> batchDefinitions, IJobDefinitionService<BatchDefinition, int> jobDefinitionService)
        => (_batchDefinitions, _jobDefinitionService)
        = (batchDefinitions, jobDefinitionService);
    public void Delete(int batchId)
    {
        _batchDefinitions.RemoveAll(i => i.Id == batchId);
        _jobDefinitionService.ResyncJobDefinitions();
    }
    public List<BatchDefinition> GetAll() => _batchDefinitions;
    public int Save(BatchDefinition batch)
    {
        _batchDefinitions.RemoveAll(i => i.Id == batch.Id);
        batch.Id = _batchDefinitions.DefaultIfEmpty().Max(i => i?.Id ?? 0) + 1;
        _batchDefinitions.Add(batch);
        _jobDefinitionService.ResyncJobDefinitions();
        return batch.Id;
    }
}

/// <summary>
/// Setup how the scheduler handles definitions of batches:
///  - How to get the initial list of batches to be run. 
///    Here, it is retrieved using the persistence layer of batch definitions.
///  - How to get the cron expression
///  - How to get the identifier
///  - What to do when the batch must be run
/// </summary>
public class MyBatchConfiguration : IBatchSetup<BatchDefinition, int>
{
    private readonly IBatchService _batchService;
    public MyBatchConfiguration(IBatchService batchService)
        => (_batchService) = (batchService);
    public void Execute(BatchDefinition batchDefinition, CancellationToken stoppingToken)
    {
        Console.WriteLine($"[{batchDefinition.Name}] executing...");
        var process = new Process();
        if (batchDefinition.Command == null)
        {
            Console.WriteLine($"[{batchDefinition.Name}] Nothing to run.");
            return;
        }
        if (batchDefinition.Arguments != null)
            process.StartInfo = new ProcessStartInfo(batchDefinition.Command, batchDefinition.Arguments);
        else
            process.StartInfo = new ProcessStartInfo(batchDefinition.Command);
        process.Start();
        process.WaitForExitAsync(stoppingToken).Wait(stoppingToken);
        Console.WriteLine($"[{batchDefinition.Name}] executed. Exit code: {process.ExitCode}");
    }
    public IEnumerable<BatchDefinition> GetAll() => _batchService.GetAll();
    public string? GetCronExpression(BatchDefinition source) => source.CronExpression;
    public int GetKey(BatchDefinition source) => source.Id;
}
```
