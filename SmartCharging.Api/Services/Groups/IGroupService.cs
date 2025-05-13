using SmartCharging.Api.Dtos.Group;

namespace SmartCharging.Api.Services.Groups
{
    public interface IGroupService
    {
        Task<Result<Guid>> CreateGroupAsync(CreateGroup groupDto);
        Task<Result> UpdateGroupAsync(Guid id, UpdateGroup groupDto);
        Task<Result> DeleteGroupAsync(Guid id);
    }
}