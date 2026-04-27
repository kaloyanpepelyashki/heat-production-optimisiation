namespace Opt.Api.Infrastructure.Options;

public sealed class ExternalApiOptions
{
    public const string SectionName = "ExternalApis";

    public AmApiOptions Am { get; set; } = new();
    public SdmApiOptions Sdm { get; set; } = new();
}

public sealed class AmApiOptions
{
    public string BaseUrl { get; set; } = "https://heat-production-optimisiation.onrender.com/";
    public string GasBoilersEndpoint { get; set; } = "api/getproductionunits/allGasBoilers";
    public string OilBoilersEndpoint { get; set; } = "api/getproductionunits/allOilBoilers";
    public string ElectricBoilersEndpoint { get; set; } = "api/getproductionunits/allElectricBoilers";
    public string GasMotorsEndpoint { get; set; } = "api/getproductionunits/allGasMotors";
    public string MaintenanceSchedulesEndpoint { get; set; } = "api/getproductionunits/maintenanceSchedules";
}

public sealed class SdmApiOptions
{
    public string BaseUrl { get; set; } = "https://sdm-api.onrender.com/";
    public string SourceDataEndpoint { get; set; } = "getAll";
}
