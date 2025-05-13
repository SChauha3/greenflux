using SmartCharging.Api.Dtos.ChargeStation;
using SmartCharging.Api.Models;
using SmartCharging.Api.Repositories;
using System.Linq.Expressions;

namespace SmartCharging.Api.Services.ChargeStations
{
    public class ChargeStationService : IChargeStationService
    {
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<ChargeStation> _chargeStationRepository;
        private readonly IRepository<Connector> _connectorRepository;

        public ChargeStationService(
            IRepository<Group> groupRepository,
            IRepository<ChargeStation> chargeStationRepository,
            IRepository<Connector> connectorRepository)
        {
            _groupRepository = groupRepository;
            _chargeStationRepository = chargeStationRepository;
            _connectorRepository = connectorRepository;
        }

        public async Task<Result<Guid>> CreateChargeStationAsync(CreateChargeStation createChargeStation)
        {
            var groupId = Guid.Parse(createChargeStation.GroupId);
            var storedGroup = await _groupRepository.FindAsync(groupId);
            if (storedGroup == null)
                return Result<Guid>.Fail("The specified group was not found, and charge station cannot be created without a valid group", ErrorType.NotFound);

            var id = Guid.NewGuid();
            var connectors = MapConnectors(createChargeStation.Connectors, id);

            var chargeStation = new ChargeStation
            {
                GroupId = groupId,
                Name = createChargeStation.Name,
                Id = id,
                Connectors = connectors
            };

            storedGroup.Capacity += chargeStation.Connectors.Sum(mc => mc.MaxCurrent);

            await _chargeStationRepository.SaveChangesAsync(chargeStation);

            return Result<Guid>.Success(chargeStation.Id);
        }

        public async Task<Result> UpdateChargeStationAsync(Guid id, UpdateChargeStation updateChargeStation)
        {
            Expression<Func<ChargeStation, bool>> condition = cs => cs.Id == id;

            var storedChargeStation = await _chargeStationRepository.FindAsync(condition, cs => cs.Connectors, cs => cs.Group);

            if (storedChargeStation == null)
                return Result.Fail($"charge station with id {id} not found.", ErrorType.NotFound);

            if (storedChargeStation.GroupId != Guid.Parse(updateChargeStation.GroupId))
            {
                storedChargeStation.Group.Capacity -= storedChargeStation.Connectors.Sum(c => c.MaxCurrent);
                storedChargeStation.GroupId = Guid.Parse(updateChargeStation.GroupId);
            }

            storedChargeStation.Name = updateChargeStation.Name;
            
            await _chargeStationRepository.UpdateChangesAsync(storedChargeStation);

            return Result.Success();
        }

        public async Task<Result> DeleteChargeStationAsync(Guid id)
        {
            var chargeStation = await _chargeStationRepository.FindAsync(id);
            if (chargeStation == null)
                return Result.Fail("Charge station not found.", ErrorType.NotFound);

            await _chargeStationRepository.RemoveAsync(chargeStation);
            return Result.Success();
        }

        private ICollection<Connector> MapConnectors(List<CreateConnectorWithChargeStation> createConnectors, Guid id)
        {
            var connectors = new List<Connector>();
            foreach (var createConnector in createConnectors)
            {
                connectors.Add(new Connector
                {
                    ChargeStationContextId = createConnector.ChargeStationContextId,
                    MaxCurrent = createConnector.MaxCurrent,
                    ChargeStationId = id,
                });
            }
            return connectors;
        }
    }
}
