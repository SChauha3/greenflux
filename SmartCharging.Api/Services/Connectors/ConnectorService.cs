using Microsoft.EntityFrameworkCore;
using SmartCharging.Api.Dtos.Connector;
using SmartCharging.Api.Models;
using SmartCharging.Api.Repositories;
using System.Linq.Expressions;

namespace SmartCharging.Api.Services.Connectors
{
    public class ConnectorService : IConnectorService
    {
        private readonly IRepository<Group> _groupRepository;
        private readonly IRepository<ChargeStation> _chargeStationRepository;
        private readonly IRepository<Connector> _connectorRepository;

        public ConnectorService(
            IRepository<Group> groupRepository,
            IRepository<ChargeStation> chargeStationRepository,
            IRepository<Connector> connectorRepository)
        {
            _groupRepository = groupRepository;
            _chargeStationRepository = chargeStationRepository;
            _connectorRepository = connectorRepository;
        }

        public async Task<Result<Guid>> CreateConnectorAsync(CreateConnector createConnector)
        {
            Expression<Func<ChargeStation, bool>> condition =
                cs => cs.Id == Guid.Parse(createConnector.ChargeStationId);

            var storedChargeStation = await _chargeStationRepository.FindAsync(condition, cs => cs.Connectors, cs => cs.Group);

            if (storedChargeStation == null)
                return Result<Guid>.Fail("A Connector cannot exist in the domain without a Charge Station.", ErrorType.NotFound);

            if (storedChargeStation.Connectors.Any(c => c.ChargeStationContextId == createConnector.ChargeStationContextId))
                return Result<Guid>.Fail("Id must be unique within the context of a charge station with " +
                    "(possible range of values from 1 to 5)", ErrorType.UniqueConnector);

            var connector = new Connector
            {
                ChargeStationId = Guid.Parse(createConnector.ChargeStationId),
                ChargeStationContextId = createConnector.ChargeStationContextId,
                MaxCurrent = createConnector.MaxCurrent,
            };

            storedChargeStation.Group.Capacity += createConnector.MaxCurrent;

            await _connectorRepository.SaveChangesAsync(connector);
            return Result<Guid>.Success(connector.Id);
        }

        public async Task<Result> UpdateConnectorAsync(Guid id, UpdateConnector updateConnector)
        {
            var storedConnector = await _connectorRepository.FindAsync(
                c => c.Id == id,
                query => query
                    .Include(c => c.ChargeStation)
                    .ThenInclude(cs => cs.Group));

            if (storedConnector == null)
                return Result.Fail($"A Connector with id {id} does not exist.", ErrorType.NotFound);

            var storedGroupCapacity = storedConnector.ChargeStation.Group.Capacity;
            storedConnector.ChargeStation.Group.Capacity = storedGroupCapacity + updateConnector.MaxCurrent - storedConnector.MaxCurrent;
            storedConnector.MaxCurrent = updateConnector.MaxCurrent;

            
            await _connectorRepository.UpdateChangesAsync(storedConnector);
            return Result.Success();
        }

        public async Task<Result> DeleteConnectorAsync(Guid id)
        {
            var storedConnector = await _connectorRepository.FindAsync(
                c => c.Id == id,
                query => query
                    .Include(c => c.ChargeStation)
                    .ThenInclude(cs => cs.Group));

            if (storedConnector == null)
                return Result.Fail($"Connector not found with id {id}.", ErrorType.NotFound);

            if (storedConnector.ChargeStation.Connectors.Count == 1)
            {
                return Result.Fail("Minimal one connector is required for a charge station.", ErrorType.MinimumOneConnector);
            }

            await _connectorRepository.RemoveAsync(storedConnector);
            return Result.Success();
        }
    }
}
