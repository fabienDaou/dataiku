using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilleniumFalconChallenge.Persistence.Scenarios
{
    public class BountyHunterEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Planet { get; set; }

        [Required]
        public int Day { get; set; }

        public ScenarioEntity Scenario { get; set; }
    }
}