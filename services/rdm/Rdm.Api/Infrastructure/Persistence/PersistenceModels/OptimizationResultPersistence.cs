using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Rdm.Api.Infrastructure.Persistence.PersistenceModels;

[Table("optimization_results")]
public class OptimizationResultPersistence : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

    [Column("total_cost")]
    public double TotalCost { get; set; }

    [Column("total_co2_emissions")]
    public double TotalCo2Emissions { get; set; }
}