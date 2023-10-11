using System.ComponentModel.DataAnnotations.Schema;

namespace MilleniumFalconChallenge.Persistence.Scenarios
{
    public class ScenarioBountyHunterEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ScenarioId { get; set; }
        public ScenarioEntity Scenario { get; set; }

        public int BountyHunterId { get; set; }
        public BountyHunterEntity BountyHunter { get; set; }
    }
}