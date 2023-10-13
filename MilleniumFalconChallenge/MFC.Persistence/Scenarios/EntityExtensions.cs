using MFC.Domain;

namespace MFC.Persistence.Scenarios
{
    public static class EntityExtensions
    {
        public static Scenario ToDomain(this ScenarioEntity entity)
        {
            var bountyHunters = entity.BountyHunters
                .Select(bh => new BountyHunter(bh.Planet, bh.Day))
                .ToArray();

            return new Scenario(entity.Id, entity.Name, entity.Countdown, entity.SuccessProbability, bountyHunters);
        }
    }
}