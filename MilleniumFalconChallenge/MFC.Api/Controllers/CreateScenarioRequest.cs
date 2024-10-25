using System.ComponentModel.DataAnnotations;

namespace MFC.Api.Controllers
{
    public record CreateScenarioRequest(
        [Required][MaxLength(512)] string Name,
        [Required] int Countdown,
        [Required] CreateScenarioRequest.BountyHunter[] BountyHunters)
    {
        public record BountyHunter([Required][MaxLength(512)] string Planet, [Required] int Day);
    }
}