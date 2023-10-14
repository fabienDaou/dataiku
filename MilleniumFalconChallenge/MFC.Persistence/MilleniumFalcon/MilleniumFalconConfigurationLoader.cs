using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MFC.Persistence.MilleniumFalcon
{
    public class MilleniumFalconConfigurationLoader
    {
        private readonly ILogger _logger;

        public MilleniumFalconConfigurationLoader(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<MilleniumFalconConfigurationLoader>()
                ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public MilleniumFalconConfiguration? Load(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Environment.CurrentDirectory, path);
                _logger.LogInformation("Relative path passed for the millenium configuration file, absolute path is '{Path}'.", path);
            }

            if (!File.Exists(path))
            {
                _logger.LogError("Millenium configuration file does not exist at '{Path}'.", path);
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<MilleniumFalconConfiguration>(File.ReadAllText(path),
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Error
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