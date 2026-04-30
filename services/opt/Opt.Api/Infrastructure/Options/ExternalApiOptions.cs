namespace Opt.Api.Infrastructure.Options;

public sealed class ExternalApiOptions
{
    public const string SectionName = "ExternalApis";

    public AmApiOptions Am { get; set; } = new();
    public SdmApiOptions Sdm { get; set; } = new();
}
public sealed class AmApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string GasBoilersEndpoint { get; set; } = string.Empty;
    public string OilBoilersEndpoint { get; set; } = string.Empty;
    public string ElectricBoilersEndpoint { get; set; } = string.Empty;
    public string GasMotorsEndpoint { get; set; } = string.Empty;
    public string MaintenanceSchedulesEndpoint { get; set; } = string.Empty;

    public string ResolveMaintenanceSchedulesEndpoint(int id)
        => MaintenanceSchedulesEndpoint.Replace("{id}", id.ToString());
}

public sealed class SdmApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string SourceDataEndpoint { get; set; } = string.Empty;
}
