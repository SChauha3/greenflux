using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartCharging.Api.Models
{
    public class Connector
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ChargeStationContextId { get; set; }
        public int MaxCurrent { get; set; }
        public Guid ChargeStationId { get; set; }
        [JsonIgnore]
        public ChargeStation ChargeStation { get; set; }
    }
}