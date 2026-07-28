using CSVMetrics.Domain.Entities;

namespace CSVMetrics.Application;

public class ResultsQueryService
{
    private readonly IFileResultRepository _fileResultRepository;
    public ResultsQueryService(IFileResultRepository fileResultRepository)
    {
        _fileResultRepository = fileResultRepository;
    }
    public async Task<List<FileResult>> GetResultsAsync(ResultsFilterDto filter)
    {
        return await _fileResultRepository.GetByFilterAsync(filter);
    }
}