using CSVMetrics.Application;
using Microsoft.AspNetCore.Mvc;

namespace CSVMetrics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController : ControllerBase
{
    private readonly CsvUploadService _csvUploadService;
    private readonly ResultsQueryService _resultsQueryService;

    public MeasurementsController(CsvUploadService csvUploadService, ResultsQueryService resultsQueryService)
    {
        _csvUploadService = csvUploadService;
        _resultsQueryService = resultsQueryService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var result = await _csvUploadService.UploadAsync(stream, file.FileName);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Result);
    }
    [HttpGet("results")]
    public async Task<IActionResult> GetResults([FromQuery] ResultsFilterDto filter)
    {
        var results = await _resultsQueryService.GetResultsAsync(filter);
        return Ok(results);
    }
}