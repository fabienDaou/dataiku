using Newtonsoft.Json;

namespace MFC.Persistence.MilleniumFalcon
{
    public record MilleniumFalconConfiguration(
        [JsonProperty(Required = Required.Always)] int Autonomy,
        [JsonProperty(Required = Required.Always)] string Departure,
        [JsonProperty(Required = Required.Always)] string Arrival,
        [JsonProperty(Required = Required.Always, PropertyName = "routes_db")] string RoutesDbPath);
}