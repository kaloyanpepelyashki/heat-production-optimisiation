namespace Sdm.Api.Infrastructure.Persistence.PersistenceModels;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("source_data")]
public class SourceDataPersistence : BaseModel
{
    [PrimaryKey("Id")]
    public int Id { get; set; }

    [Column("period_id")]
    public int PeriodId { get; set; }

    [Column("time_from")]
    public DateTime TimeFrom { get; set; }

    [Column("time_to")]
    public DateTime TimeTo { get; set; }

    [Column("heat_demand")]
    public double HeatDemand { get; set; }

    [Column("electricity_price")]
    public double ElectricityPrice { get; set; }
}