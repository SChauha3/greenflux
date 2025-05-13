using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Api.Models
{
    public class Group
    {
        [Key]
        public Guid Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ICollection<ChargeStation> ChargeStations { get; set; } = new List<ChargeStation>();
    }
}
