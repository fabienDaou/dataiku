using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MilleniumFalconChallenge;
using MilleniumFalconChallenge.Actors;
using MilleniumFalconChallenge.Domain;
using MilleniumFalconChallenge.Domain.Noop;
using MilleniumFalconChallenge.Persistence.MilleniumFalcon;
using MilleniumFalconChallenge.Persistence.Scenarios;

var builder = WebApplication.CreateBuilder(args);

var pathToDistDirectory = Path.Combine("Client", "MilleniumFalconChallenge", "dist");

// todo replace noop
builder.Services.AddDbContextFactory<ScenarioDbContext>(options => options.UseInMemoryDatabase("scenarios"));
builder.Services.AddDbContextFactory<MilleniumDbContext>(options => options.UseSqlite("Data Source=universe.db"));

builder.Services.AddSingleton<IScenarioRunner, NoopScenarioRunner>();
builder.Services.AddSingleton<ScenarioRepository, ScenarioRepository>();
builder.Services.AddSingleton<IReadOnlyScenarioRepository, ScenarioRepository>();
builder.Services.AddSingleton<IScenarioRepository, ScenarioRepository>();
builder.Services.AddSingleton<IReadOnlyRoutesRepository, RoutesRepository>();

builder.Services.AddSingleton(s => ActorSystem.Create("milleniumfalcon"));
builder.Services.AddSingleton<IScenarioProcessingDispatcher>(s =>
{
    var runner = s.GetRequiredService<IScenarioRunner>();
    var scenarioRepository = s.GetRequiredService<IScenarioRepository>();
    var routesRepository = s.GetRequiredService<IReadOnlyRoutesRepository>();
    var loggerFactory = s.GetRequiredService<ILoggerFactory>();

    var props = Props.Create(() => new ScenarioProcessorsSupervisorActor(
        5,
        runner,
        scenarioRepository,
        loggerFactory));

    var actorSystem = s.GetRequiredService<ActorSystem>();
    var supervisorRef = actorSystem.ActorOf(props, "scenario-processors-supervisor");

    return new ScenarioProcessingDispatcher(supervisorRef);
});
builder.Services.AddSingleton<INewScenarioHandler>(s =>
{
    var runner = s.GetRequiredService<IScenarioRunner>();
    var scenarioRepository = s.GetRequiredService<IScenarioRepository>();
    var routesRepository = s.GetRequiredService<IReadOnlyRoutesRepository>();
    var dispatcher = s.GetRequiredService<IScenarioProcessingDispatcher>();
    var loggerFactory = s.GetRequiredService<ILoggerFactory>();

    return new NewScenarioHandler(dispatcher, scenarioRepository, routesRepository);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSpaStaticFiles(options => options.RootPath = pathToDistDirectory);
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(s => s.FullName.Replace("+", "."));
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "allowDev",
                          policy =>
                          {
                              policy.AllowAnyOrigin().WithMethods(
                                HttpMethod.Get.Method,
                                HttpMethod.Put.Method,
                                HttpMethod.Post.Method,
                                HttpMethod.Delete.Method).AllowAnyHeader();
                          });
    });
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseCors("allowDev");
}
else
{
    StaticFileOptions staticFileOptions = new()
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, pathToDistDirectory))
    };
    app.UseStaticFiles(staticFileOptions);
}

app.MapControllers();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = pathToDistDirectory;
});

app.Run();
