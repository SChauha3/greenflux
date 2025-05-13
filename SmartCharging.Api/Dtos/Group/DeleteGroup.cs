namespace SmartCharging.Api.Dtos.Group
{
    public class DeleteGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}