using System.ComponentModel.DataAnnotations.Schema;

namespace MilleniumFalconChallenge.Persistence.MilleniumFalcon
{
    [Table("routes")]
    public class RouteEntity
    {
        [Column("origin")]
        public string Origin { get; set; }

        [Column("destination")]
        public string Destination { get; set; }

        [Column("travel_time")]
        public int TravelTime { get; set; }
    }
}