using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Am.Api.Model.DTOs;

[Table("maintenance_periods")]
public class MaintenancePeriodPersistence : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("optimization_run_id")]
    public int? OptimizationRunId { get; set; }

    [Column("start_time")]
    public DateTime StartTime { get; set; }

    [Column("end_time")]
    public DateTime EndTime { get; set; }

    [Column("is_emergency")]
    public bool IsEmergency { get; set; }

    [Column("status")]
    public string Status { get; set; }
}
