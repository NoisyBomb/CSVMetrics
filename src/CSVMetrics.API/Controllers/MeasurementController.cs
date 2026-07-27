using CSVMetrics.Application;
using Microsoft.AspNetCore.Mvc;

namespace CSVMetrics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController : ControllerBase
{
    private readonly CsvParser _csvParser;
    public MeasurementsController(CsvParser csvParser)
    {
        _csvParser = csvParser;
    }

    [HttpPost("upload")]
    public IActionResult Upload(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var rows = _csvParser.Parse(stream);
        return Ok(rows);
    }
}