using Newtonsoft.Json;

namespace MilleniumFalconChallenge.Persistence.MilleniumFalcon
{
    public record MilleniumFalconConfiguration(
        [JsonProperty(Required = Required.Always)] uint Autonomy,
        [JsonProperty(Required = Required.Always)] string Departure,
        [JsonProperty(Required = Required.Always)] string Arrival,
        [JsonProperty(Required = Required.Always, PropertyName = "routes_db")] string RoutesDbPath);
}