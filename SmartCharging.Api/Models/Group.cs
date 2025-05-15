using System.ComponentModel.DataAnnotations;

namespace SmartCharging.Api.Models
{
    public class Group
    {
        [Key]
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int Capacity { get; private set; }
        public ICollection<ChargeStation> ChargeStations { get; private set; } = new List<ChargeStation>();

        protected Group() { }

        private Group(string name, int capacity)
        {
            Id = Guid.NewGuid();
            Name = name;
            Capacity = capacity;
        }

        public static Group Create(string name, int capacity) =>
            new Group(name, capacity);

        public int GetUsedCapacity()
        {
            return ChargeStations
                .SelectMany(cs => cs.Connectors)
                .Sum(c => c.MaxCurrent);
        }

        public bool CanUpdateCapacity(int newCapacity)
        {
            return newCapacity >= GetUsedCapacity();
        }

        public bool TryUpdate(string name, int newCapacity)
        {
            if (!CanUpdateCapacity(newCapacity))
                return false;

            Name = name;
            Capacity = newCapacity;

            return true;
        }

        public bool CanAddChargeStation(ChargeStation newStation)
        {
            int newLoad = newStation.Connectors.Sum(c => c.MaxCurrent);
            return GetUsedCapacity() + newLoad <= Capacity;
        }

        public bool TryAddChargeStation(ChargeStation station)
        {
            if (!CanAddChargeStation(station))
                return false;

            ChargeStations.Add(station);
            return true;
        }

        public bool CanAcceptAdditionalCurrent(int additionalCurrent)
        {
            return GetUsedCapacity() + additionalCurrent <= Capacity;
        }
    }
}
