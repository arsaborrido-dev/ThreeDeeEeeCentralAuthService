using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Technologies.DTOs;

namespace AuthApi.Application.Portfolio.Technologies;
public class TechnologyService : ITechnologyService
{
    private readonly ITechnologyRepository _technologyRepository;
    public TechnologyService(ITechnologyRepository technologyRepository) => _technologyRepository = technologyRepository;
    public async Task<ResponseResult<List<TechnologiesListResponse>>> GetAllTechnologiesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _technologyRepository
           .GetAllTechnologiesAsync(cancellationToken);

        return ResponseResult<List<TechnologiesListResponse>>.Success(response);
    }
    public async Task<ResponseResult<TechnologyResponse>?> GetTechnologyByIdAsync(
        long id, 
        CancellationToken cancellationToken = default)
    {
        var response = await _technologyRepository
            .GetTechnologyByIdAsync(id, cancellationToken);

        return ResponseResult<TechnologyResponse>.Success(response);
    }
}
