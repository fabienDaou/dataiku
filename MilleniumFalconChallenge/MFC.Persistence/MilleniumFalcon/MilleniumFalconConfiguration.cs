using Newtonsoft.Json;

namespace MFC.Persistence.MilleniumFalcon
{
    public record MilleniumFalconConfiguration
    {
        [JsonProperty(Required = Required.Always)]
        public int Autonomy { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Departure { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Arrival { get; set; }

        [JsonProperty(Required = Required.Always, PropertyName = "routes_db")]
        public string RoutesDbPath { get; set; }

        public MilleniumFalconConfiguration(int autonomy, string departure, string arrival, string routesDbPath)
        {
            Autonomy = autonomy;
            Departure = departure;
            Arrival = arrival;
            RoutesDbPath = routesDbPath;
        }
    }
}