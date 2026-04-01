namespace Ffa.Api.Models
{
    public sealed class SourceDataDto
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double HeatDemand { get; set; }
        public double ElectricityPrice { get; set; }
    }
}
