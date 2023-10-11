using System.ComponentModel.DataAnnotations;

namespace MilleniumFalconChallenge.Api.Controllers
{
    public record CreateScenarioRequest(
        [Required][MaxLength(512)] string Name,
        [Required] int Countdown,
        [Required] CreateScenarioRequest.BountyHunter[] BountyHunters)
    {
        public record BountyHunter([Required][MaxLength(512)] string Planet, [Required][Range(1, int.MaxValue)] int Day);
    }
}