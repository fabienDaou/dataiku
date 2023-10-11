using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MilleniumFalconChallenge.Persistence.Empire
{
    public class EmpireConfigurationLoader
    {
        private readonly ILogger _logger;

        public EmpireConfigurationLoader(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<EmpireConfigurationLoader>()
                ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public EmpireConfiguration? Load(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                _logger.LogError("Expected an absolute path for the empire configuration file, but got '{Path}'.", path);
                return null;
            }

            if (!File.Exists(path))
            {
                _logger.LogError("Empire configuration file does not exist at '{Path}'.", path);
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<EmpireConfiguration>(File.ReadAllText(path),
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Error,
                    });
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex, "Error when deserializing file at '{Path}'.", path);
                return null;
            }
        }
    }
}