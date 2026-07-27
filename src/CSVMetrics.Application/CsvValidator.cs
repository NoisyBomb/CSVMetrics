using FluentValidation;

namespace CSVMetrics.Application;

public class CsvValidator : AbstractValidator<CsvDto>
{
    public CsvValidator()
    {
        RuleFor(x => x.Date).LessThanOrEqualTo(DateTimeOffset.UtcNow).GreaterThanOrEqualTo(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));
        RuleFor(x => x.ExecutionTime).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
    }
}