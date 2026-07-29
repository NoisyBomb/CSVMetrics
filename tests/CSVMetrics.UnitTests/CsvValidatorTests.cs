using CSVMetrics.Application;
using Xunit;

namespace CSVMetrics.UnitTests;

public class CsvValidatorTests
{
    private readonly CsvValidator _validator = new();

    [Fact]
    public void ReturnsNoErrors()
    {
        var row = new CsvDto
        {
            Date = new DateTimeOffset(2024, 1, 15, 10, 0, 0, TimeSpan.Zero),
            ExecutionTime = 1.5,
            Value = 42.3
        };
        var result = _validator.Validate(row);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DateInFuture()
    {
        var row = new CsvDto
        {
            Date = DateTimeOffset.UtcNow.AddDays(1),
            ExecutionTime = 1.5,
            Value = 42.3
        };
        var result = _validator.Validate(row);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void DateBefore2000()
    {
        var row = new CsvDto
        {
            Date = new DateTimeOffset(1999, 12, 31, 0, 0, 0, TimeSpan.Zero),
            ExecutionTime = 1.5,
            Value = 42.3
        };
        var result = _validator.Validate(row);
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void NegativeExecutionTime()
    {
        var row = new CsvDto
        {
            Date = new DateTimeOffset(2024, 1, 15, 10, 0, 0, TimeSpan.Zero),
            ExecutionTime = -1,
            Value = 42.3
        };
        var result = _validator.Validate(row);
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void NegativeValue()
    {
        var row = new CsvDto
        {
            Date = new DateTimeOffset(2024, 1, 15, 10, 0, 0, TimeSpan.Zero),
            ExecutionTime = 1.5,
            Value = -1
        };
        var result = _validator.Validate(row);
        Assert.False(result.IsValid);
    }
}