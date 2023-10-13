using Newtonsoft.Json;

namespace MFC.Persistence.Empire
{
    public record EmpireConfiguration(
        [JsonProperty(Required = Required.Always)] int Countdown,
        [JsonProperty(Required = Required.Always, PropertyName = "bounty_hunters")] BountyHunterConfiguration[] BountyHunters);
}