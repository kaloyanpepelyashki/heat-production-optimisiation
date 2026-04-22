using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Am.Api.Model.DTOs;

/// <summary>
/// Represents the polymorphic junction table connecting Production Units and Maintenance Periods.
/// </summary>
[Table("production_unit_maintenance")]
public class ProductionUnitMaintenancePersistence : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("maintenance_period_id")]
    public int MaintenancePeriodId { get; set; }

    [Column("unit_type_id")]
    public int UnitTypeId { get; set; }

    [Column("unit_id")]
    public int UnitId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}