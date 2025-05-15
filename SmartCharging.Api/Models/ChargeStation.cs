using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartCharging.Api.Models
{
    public class ChargeStation
    {
        [Key]
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public ICollection<Connector> Connectors {  get; set; } = new List<Connector>();
        public Guid GroupId { get; set; }
        [JsonIgnore]
        public Group? Group { get; set; }

        protected ChargeStation() { }

        private ChargeStation(Guid id, string name, ICollection<Connector> connectors, Guid groupId) 
        {
            Id = id;
            Name = name;
            Connectors = connectors;
            GroupId = groupId;
        }

        public static ChargeStation Create(Guid id, string name, ICollection<Connector> connectors, Guid groupId) =>
            new ChargeStation(id, name, connectors, groupId);

        public void Update(string name)
        {
            Name = name;
        }

        public int GetCurrentLoad() =>
            Connectors.Sum(c => c.MaxCurrent);

        public bool CanAddConnector(int maxCurrent) =>
            Group?.CanAcceptAdditionalCurrent(maxCurrent) ?? false;

        public bool IsChargeStationContextIdUnique(int chargeStationContextId) =>
            !Connectors.Any(c => c.ChargeStationContextId == chargeStationContextId);

        public void AddConnector(Connector connector)
        {
            Connectors.Add(connector);
        }

        public bool CanRemoveConnector() => Connectors.Count > 1;

        public void RemoveConnector(Connector connector)
        {
            Connectors.Remove(connector);
        }
    }
}