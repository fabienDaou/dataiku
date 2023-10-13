using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MFC.Persistence.Scenarios
{
    public class ScenarioEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Name { get; set; }

        [Required]
        public int Countdown { get; set; }

        public double? SuccessProbability { get; set; }

        public ICollection<BountyHunterEntity> BountyHunters { get; set; }
    }
}