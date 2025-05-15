using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartCharging.Api.Models
{
    public class Connector
    {
        [Key]
        public Guid Id { get; private set; }
        public int ChargeStationContextId { get; private set; }
        public int MaxCurrent { get; private set; }
        public Guid ChargeStationId { get; private set; }
        [JsonIgnore]
        public ChargeStation? ChargeStation { get; private set; }

        protected Connector() { }

        private Connector(int chargeStationContextId, int maxCurrent, Guid chargeStationId)
        {
            if (maxCurrent <= 0)
                throw new ArgumentException("MaxCurrent must be greater than zero.", nameof(maxCurrent));

            if (chargeStationContextId < 1 || chargeStationContextId > 5)
                throw new ArgumentException("ChargeStationContextId must be between 1 and 5.", nameof(chargeStationContextId));

            Id = Guid.NewGuid();
            ChargeStationContextId = chargeStationContextId;
            MaxCurrent = maxCurrent;
            ChargeStationId = chargeStationId;
        }

        public static Connector Create(int chargeStationContextId, int maxCurrent, Guid chargeStationId) =>
            new Connector(chargeStationContextId, maxCurrent, chargeStationId);

        public bool CanUpdateMaxCurrent(int newMaxCurrent)
        {
            if (ChargeStation?.Group == null)
                return false;

            var currentTotal = ChargeStation.Group.GetUsedCapacity();
            var proposedTotal = currentTotal - MaxCurrent + newMaxCurrent;

            return proposedTotal <= ChargeStation.Group.Capacity;
        }

        public void UpdateMaxCurrent(int newMaxCurrent)
        {
            MaxCurrent = newMaxCurrent;
        }
    }
}