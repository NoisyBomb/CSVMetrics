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
        if (rows.Count < 1 || rows.Count > 10000)
        {
            return BadRequest("File must contain between 1 and 10000 rows");
        }
        var validator = new CsvValidator();
        var errors = new List<string>();
        foreach (var row in rows)
        {
            var result = validator.Validate(row);
            if (!result.IsValid)
            {
                errors.AddRange(result.Errors.Select(e => e.ErrorMessage));
            }
        }

        if (errors.Count > 0)
        {
            return BadRequest(errors);
        }

        return Ok(rows);
    }
}