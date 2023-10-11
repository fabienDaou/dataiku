
using Microsoft.Extensions.Logging;
using MilleniumFalconChallenge;
using MilleniumFalconChallenge.Domain.Noop;
using MilleniumFalconChallenge.Persistence.Empire;
using MilleniumFalconChallenge.Persistence.MilleniumFalcon;

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

var empirePath = args[0];
if (!Path.IsPathRooted(empirePath))
{
    empirePath = Path.Combine(Environment.CurrentDirectory, empirePath);
}

MilleniumFalconConfiguration? milleniumConfiguration = new MilleniumFalconConfigurationLoader(loggerFactory).Load(milleniumPath);
if (milleniumConfiguration is null)
{
    Environment.Exit(1);
}

EmpireConfiguration? empireConfiguration = new EmpireConfigurationLoader(loggerFactory).Load(empirePath);
if (empireConfiguration is null)
{
    Environment.Exit(1);
}

var bountyHunters = empireConfiguration.BountyHunters.Select(bh => new BountyHunter(bh.Planet, bh.Day)).ToArray();
Scenario scenario = new(1, string.Empty, empireConfiguration.Countdown, 0, bountyHunters);
var runner = new NoopScenarioRunner(loggerFactory);

var probability = await runner.RunAsync(scenario);

Console.WriteLine(probability);
