using Newtonsoft.Json;

namespace MFC.Persistence.Empire
{
    public record BountyHunterConfiguration(
        [JsonProperty(Required = Required.Always)] string Planet,
        [JsonProperty(Required = Required.Always)] int Day);
}