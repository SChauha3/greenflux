using Microsoft.EntityFrameworkCore;
using SmartCharging.Api.Dtos.Group;
using SmartCharging.Api.Models;
using SmartCharging.Api.Repositories;
using System.Linq.Expressions;

namespace SmartCharging.Api.Services.Groups
{
    public class GroupService : IGroupService
    {
        private const string CapacityErrorMessage = "The Capacity in Amps of a Group should always be greater than or equal to the sum of the Max current in Amps of all Connectors indirectly belonging to the Group.";
        private const string GroupNotFound = "Group not found";

        private readonly IRepository<Group> _groupRepository;

        public GroupService(IRepository<Group> groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<Result<Guid>> CreateGroupAsync(CreateGroup createGroup)
        {
            var group = new Group
            {
                Capacity = createGroup.Capacity,
                Name = createGroup.Name,
                Id = Guid.NewGuid(),
            };
            
            await _groupRepository.SaveChangesAsync(group);
            return Result<Guid>.Success(group.Id);
        }

        public async Task<Result> UpdateGroupAsync(Guid id, UpdateGroup updateGroup)
        {
            Expression<Func<Group, bool>> condition = g => g.Id == id;

            var storedGroup = await _groupRepository.FindAsync(
                condition,
                query => query.Include(g => g.ChargeStations).ThenInclude(cs => cs.Connectors));

            if (storedGroup == null)
                return Result.Fail($"Could not find group with Id {id}.", ErrorType.NotFound);

            var isCapacityValid = ValidateCapacity(storedGroup, updateGroup);

            if (!isCapacityValid)
                return Result.Fail(CapacityErrorMessage, ErrorType.InValidCapacity);

            storedGroup.Name = updateGroup.Name;
            storedGroup.Capacity = updateGroup.Capacity;

            await _groupRepository.UpdateChangesAsync(storedGroup);
            return Result.Success();
        }

        public async Task<Result> DeleteGroupAsync(Guid id)
        {
            var storedGroup = await _groupRepository.FindAsync(id);
            if (storedGroup == null)
                return Result.Fail($"group not found with Group Id {id}", ErrorType.NotFound);

            await _groupRepository.RemoveAsync(storedGroup);
            return Result.Success();
        }

        private bool ValidateCapacity(Group storedGroup, UpdateGroup updatedGroup)
        {
            var totalConnectorCapacity = storedGroup.ChargeStations
                .SelectMany(g => g.Connectors)
                .Sum(c => c.MaxCurrent);

            if (updatedGroup.Capacity < totalConnectorCapacity)
                return false;
            return true;
        }
    }
}
