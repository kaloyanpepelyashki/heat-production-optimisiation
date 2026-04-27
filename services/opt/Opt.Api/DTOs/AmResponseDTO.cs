namespace Opt.Api.DTOs;

public sealed class AmGasBoilerResponseDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public float MaxHeat { get; set; }
	public float ProductionCost { get; set; }
	public int Co2Emissions { get; set; }
	public float GasConsumption { get; set; }
}

public sealed class AmOilBoilerResponseDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public float MaxHeat { get; set; }
	public float ProductionCost { get; set; }
	public int Co2Emissions { get; set; }
	public float OilConsumption { get; set; }
}

public sealed class AmElectricBoilerResponseDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public float MaxHeat { get; set; }
	public float ProductionCost { get; set; }
	public float MaxElectricity { get; set; }
}

public sealed class AmGasMotorResponseDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public float MaxHeat { get; set; }
	public float MaxElectricity { get; set; }
	public float ProductionCost { get; set; }
	public int Co2Emissions { get; set; }
	public float GasConsumption { get; set; }
}

public sealed class AmMaintenanceScheduleResponseDto
{
	public int UnitId { get; set; }
	public DateTime StartUtc { get; set; }
	public DateTime EndUtc { get; set; }
}
