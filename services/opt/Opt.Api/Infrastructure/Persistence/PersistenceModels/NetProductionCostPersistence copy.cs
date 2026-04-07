using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Opt.Api.Infrastructure.Persistence.PersistenceModels;

[Table("net_production_cost")]
public class NetProductionCostPersistence : BaseModel
{
    [PrimaryKey("Id")]
    public int Id { get; set; }
    [Column("period_id")]
    public int PeriodId { get; set; }
    [Column("time_from")]
    public DateTime TimeFrom { get; set; }
    [Column("time_to")]
    public DateTime TimeTo { get; set; }
    [Column("net_production_cost")]
    public double NetProdcutionCost { get; set; }
}