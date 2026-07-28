using CSVMetrics.Application;
using Microsoft.AspNetCore.Mvc;

namespace CSVMetrics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController : ControllerBase
{
    private readonly CsvUploadService _csvUploadService;
    public MeasurementsController(CsvUploadService csvUploadService)
    {
        _csvUploadService = csvUploadService;
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
}