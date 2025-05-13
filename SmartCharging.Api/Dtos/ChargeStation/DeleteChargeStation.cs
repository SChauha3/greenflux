namespace SmartCharging.Api.Dtos.ChargeStation
{
    public class DeleteChargeStation
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}