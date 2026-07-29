using CSVMetrics.Application;
using Xunit;

namespace CSVMetrics.UnitTests;

public class AggregateCalculatorTests
{
    private readonly AggregateCalculator _calculator = new();

    [Fact]
    public void OddNumberOfValuesMedian()
    {
        var rows = new List<CsvDto>
        {
            new() { Date = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 10 },
            new() { Date = new DateTimeOffset(2024, 1, 1, 11, 0, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 30 },
            new() { Date = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 20 }
        };
        var result = _calculator.Calculate(rows, "test.csv");
        Assert.Equal(20, result.MedianValue);
    }

    [Fact]
    public void EvenNumberOfValuesMedian()
    {
        var rows = new List<CsvDto>
        {
            new() { Date = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 10 },
            new() { Date = new DateTimeOffset(2024, 1, 1, 11, 0, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 20 },
            new() { Date = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 30 },
            new() { Date = new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 40 }
        };
        var result = _calculator.Calculate(rows, "test.csv");
        Assert.Equal(25, result.MedianValue);
    }

    [Fact]
    public void SingleValueEqualOfAll()
    {
        var rows = new List<CsvDto>
        {
            new() { Date = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero), ExecutionTime = 2, Value = 15 }
        };
        var result = _calculator.Calculate(rows, "test.csv");
        Assert.Equal(15, result.MedianValue);
        Assert.Equal(15, result.AvgValue);
        Assert.Equal(15, result.MaxValue);
        Assert.Equal(15, result.MinValue);
        Assert.Equal(0, result.TimeDeltaSeconds); 
    }

    [Fact]
    public void TimeDeltaTest()
    {
        var rows = new List<CsvDto>
        {
            new() { Date = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 10 },
            new() { Date = new DateTimeOffset(2024, 1, 1, 12, 30, 0, TimeSpan.Zero), ExecutionTime = 1, Value = 20 }
        };
        var result = _calculator.Calculate(rows, "test.csv");
        Assert.Equal(9000, result.TimeDeltaSeconds);
    }
}