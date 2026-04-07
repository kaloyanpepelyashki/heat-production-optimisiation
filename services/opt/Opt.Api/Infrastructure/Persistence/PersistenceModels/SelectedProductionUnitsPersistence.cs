using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Opt.Api.Infrastructure.Persistence.PersistenceModels;

[Table("selected_production_units")]
public class SelectedProductionUnitsPersistence : BaseModel
{
    [PrimaryKey("Id")]
    public int Id { get; set; }
    [Column("period_id")]
    public int PeriodId { get; set; }
    [Column("time_from")]
    public DateTime TimeFrom { get; set; }
    [Column("time_to")]
    public DateTime TimeTo { get; set; }
    [Column("selected_prodcution_units")]
    public List<string> SelectedProductionUnitsNames { get; set; }
}