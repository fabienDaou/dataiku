using Newtonsoft.Json;

namespace MilleniumFalconChallenge.Persistence.Empire
{
    public record BountyHunterConfiguration(
        [JsonProperty(Required = Required.Always)] string Planet,
        [JsonProperty(Required = Required.Always)] int Day);
}