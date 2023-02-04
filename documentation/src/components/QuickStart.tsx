import React from 'react';
// import clsx from 'clsx';
import styles from './HomepageFeatures.module.css';
import "prismjs"; // eslint-disable-line
require(`prismjs/components/prism-csharp`); // eslint-disable-line
import { WithLineNumbers } from './WithLineNumbers';
// https://emojipedia.org/

interface IQuickStart {
  title: string;
  code: {
    batch: string;
    program: string;
  }
}
const quickStarts: IQuickStart[] = [
  {
    title: "Simple Job Runner",
    code: {
      batch: `dotnet new worker -o TestWorker
cd TestWorker
dotnet add package Paillave.Scheduler.Core`,
      program: `using Paillave.Scheduler.Core;

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
}`
    }
  },
  {
    title: "Job Runner driven by web api",
    code: {
      batch: `dotnet new webapi -o TestWebApi
cd TestWebApi
dotnet add package Paillave.Scheduler.Core`,
      program: `using System.Diagnostics;
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
}`
    }
  },
]


export default function QuickStarts() {
  if (!quickStarts?.length) return null;
  return <section className={styles.features}>
    <div className="container">
      <div className="row">
        <div className="col col--10 col--offset-1">
          {quickStarts.map(quickStart => <QuickStart key={quickStart.title} quickStart={quickStart} />)}
        </div>
      </div>
    </div>
  </section>
}


function QuickStart({ quickStart }: { quickStart: IQuickStart }) {
  return <div className='card margin--md shadow--tl'>
    <div className="card__header">
      <h3>{quickStart.title}</h3>
    </div>
    <div className="card__body">
      <p>Create the project</p>
      <WithLineNumbers sourceCode={quickStart.code.batch} />
      <p>Program.cs</p>
      <WithLineNumbers sourceCode={quickStart.code.program} />
    </div>
  </div>
}