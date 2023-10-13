using System.ComponentModel.DataAnnotations;
using System.Net;
using MFC.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MFC.Api.Controllers
{
    [ApiController]
    [Route("api/scenarios")]
    public class ScenariosController : ControllerBase
    {
        private readonly IReadOnlyScenarioRepository _scenarioRepository;
        private readonly INewScenarioHandler _newScenarioHandler;
        private readonly ILogger<ScenariosController> _logger;

        public ScenariosController(
            INewScenarioHandler newScenarioHandler,
            IReadOnlyScenarioRepository scenarioRepository,
            ILogger<ScenariosController> logger)
        {
            _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
            _newScenarioHandler = newScenarioHandler ?? throw new ArgumentNullException(nameof(newScenarioHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(GetScenariosPageResponse), 200)]
        public async Task<GetScenariosPageResponse> GetScenarioPageAsync(
            [Range(1, int.MaxValue, ErrorMessage = "The value must be at least 1.")][Required][FromQuery] int page,
            [Range(1, int.MaxValue, ErrorMessage = "The value must be at least 1.")][Required][FromQuery] int pageSize)
        {
            var total = await _scenarioRepository.CountAsync();

            var scenarios = await _scenarioRepository.GetAsync(page, pageSize)
                .Select(s => new GetScenariosPageResponse.Scenario(s))
                .ToArrayAsync();

            return new GetScenariosPageResponse(total, scenarios);
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(CreateScenarioResponse), 200)]
        public async Task<IActionResult> CreateScenarioAsync([Required][FromBody] CreateScenarioRequest dto)
        {
            var newScenario = new NewScenario(
                dto.Name,
                dto.Countdown,
                dto.BountyHunters.Select(b => new BountyHunter(b.Planet, b.Day)).ToArray());

            var result = await _newScenarioHandler.HandleAsync(newScenario);

            return result switch
            {
                INewScenarioHandler.Success s => Ok(s.Id),
                INewScenarioHandler.InvalidScenario i => BadRequest(i.Message),
                _ => StatusCode((int)HttpStatusCode.InternalServerError)
            };
        }
    }
}