namespace Ffa.Api.Controllers
{
    using System.Text.Json;
    using Ffa.Api.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/source-data")]
    public sealed class SourceDataController : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public SourceDataController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SourceDataDto>>> GetByRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from.Date > to.Date)
            {
                return this.BadRequest("The from date must be before or equal to the to date.");
            }

            var sdmBaseUrl = this.configuration["SDM_API_URL"] ?? "http://localhost:5002";
            var sdmUri = $"{sdmBaseUrl.TrimEnd('/')}/getAll";

            try
            {
                var client = this.httpClientFactory.CreateClient();
                using var response = await client.GetAsync(sdmUri, this.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync(this.HttpContext.RequestAborted);
                var allSourceData = JsonSerializer.Deserialize<List<SourceDataDto>>(responseBody, JsonOptions) ?? new List<SourceDataDto>();

                var filtered = allSourceData
                    .Where(item => item.TimeFrom >= from.Date && item.TimeTo <= to.Date.AddDays(1))
                    .OrderBy(item => item.TimeFrom)
                    .ToList();

                return this.Ok(filtered);
            }
            catch (Exception ex)
            {
                return this.StatusCode(
                    StatusCodes.Status502BadGateway,
                    $"Failed to retrieve source data from SDM endpoint '{sdmUri}'. Error: {ex.Message}");
            }
        }
    }
}
