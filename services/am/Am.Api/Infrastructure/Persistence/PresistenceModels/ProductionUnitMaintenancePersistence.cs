namespace Am.Api.Model.DTOs;

using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("production_unit_maintenance")]
public class ProductionUnitMaintenancePersistence : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("unit_type")]
    public string UnitType { get; set; }

    [Column("unit_id")]
    public int UnitId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("from_date")]
    public DateTime FromDate { get; set; }

    [Column("to_date")]
    public DateTime ToDate { get; set; }

    [Column("period_id")]
    public int PeriodId { get; set; }

    [Column("scenario_id")]
    public int ScenarioId { get; set; }
}