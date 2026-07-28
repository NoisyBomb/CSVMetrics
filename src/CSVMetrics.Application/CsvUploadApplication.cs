using CSVMetrics.Domain.Entities;

namespace CSVMetrics.Application;

public class CsvUploadResult
{
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new();
    public FileResult? Result { get; set; }
}

public class CsvUploadService
{
    private readonly CsvParser _csvParser;
    private readonly CsvValidator _csvValidator;
    private readonly AggregateCalculator _aggregateCalculator;
    private readonly IMeasurementRepository _measurementRepository;
    private readonly IFileResultRepository _fileResultRepository;
    private readonly ITransaction _transaction;

    public CsvUploadService(
        CsvParser csvParser,
        CsvValidator csvValidator,
        AggregateCalculator aggregateCalculator,
        IMeasurementRepository measurementRepository,
        IFileResultRepository fileResultRepository,
        ITransaction transaction)
    {
        _csvParser = csvParser;
        _csvValidator = csvValidator;
        _aggregateCalculator = aggregateCalculator;
        _measurementRepository = measurementRepository;
        _fileResultRepository = fileResultRepository;
        _transaction = transaction;
    }

    public async Task<CsvUploadResult> UploadAsync(Stream fileStream, string fileName)
    {
        var rows = _csvParser.Parse(fileStream);

        if (rows.Count < 1 || rows.Count > 10000)
        {
            return new CsvUploadResult
            {
                IsSuccess = false,
                Errors = { "File must contain between 1 and 10000 rows" }
            };
        }

        var errors = new List<string>();
        foreach (var row in rows)
        {
            var validationResult = _csvValidator.Validate(row);
            if (!validationResult.IsValid)
            {
                errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));
            }
        }

        if (errors.Count > 0)
        {
            return new CsvUploadResult { IsSuccess = false, Errors = errors };
        }

        var fileResult = _aggregateCalculator.Calculate(rows, fileName);

        using var transactionHandle = await _transaction.BeginTransactionAsync();

        var existingResult = await _fileResultRepository.GetByFileNameAsync(fileName);
        if (existingResult != null)
        {
            await _measurementRepository.DeleteByFileNameAsync(fileName);
            await _fileResultRepository.DeleteAsync(existingResult);
            await _transaction.SaveChangesAsync();
        }

        var measurementValues = rows.Select(r => new MeasurementValue
        {
            FileName = fileName,
            Date = r.Date,
            ExecutionTime = r.ExecutionTime,
            Value = r.Value
        }).ToList();

        await _measurementRepository.AddRangeAsync(measurementValues);
        await _fileResultRepository.AddAsync(fileResult);
        await _transaction.SaveChangesAsync();

        await _transaction.CommitAsync();

        return new CsvUploadResult { IsSuccess = true, Result = fileResult };
    }
}