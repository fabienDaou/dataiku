using MFC.Domain;
using MFC.Domain.Runners;
using MFC.Persistence.Empire;
using MFC.Persistence.MilleniumFalcon;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger("milleniumfalconcmdline");

if (args.Length != 2)
{
    logger.LogError("Expecting 2 parameters, got {ArgsLength}.", args.Length);
    Environment.Exit(1);
}

var milleniumPath = args[0];
if (!Path.IsPathRooted(milleniumPath))
{
    milleniumPath = Path.Combine(Environment.CurrentDirectory, milleniumPath);
}

var empirePath = args[1];
if (!Path.IsPathRooted(empirePath))
{
    empirePath = Path.Combine(Environment.CurrentDirectory, empirePath);
}

MilleniumFalconConfiguration? conf =
    new MilleniumFalconConfigurationLoader(NullLoggerFactory.Instance)
    .Load(milleniumPath);
if (conf is null)
{
    Environment.Exit(1);
}

EmpireConfiguration? empireConfiguration =
    new EmpireConfigurationLoader(NullLoggerFactory.Instance)
    .Load(empirePath);
if (empireConfiguration is null)
{
    Environment.Exit(1);
}
RoutesRepository routesRepository = new(new RoutesDbContextFactory(conf.RoutesDbPath));
HashSetScenarioRunner runner = new(routesRepository, new(conf.Autonomy, conf.Departure, conf.Arrival), NullLoggerFactory.Instance);

var bountyHunters = empireConfiguration.BountyHunters.Select(bh => new BountyHunter(bh.Planet, bh.Day)).ToArray();
Scenario scenario = new(1, string.Empty, empireConfiguration.Countdown, 0, bountyHunters);
var probability = await runner.RunAsync(scenario);

Console.WriteLine(probability);
