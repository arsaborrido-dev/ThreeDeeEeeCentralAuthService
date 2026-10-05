
using AuthApi.Application.Portfolio.Technologies.DTOs;

namespace AuthApi.Application.Portfolio.Technologies;
public interface ITechnologyRepository
{
    Task<List<TechnologiesListResponse>> GetAllTechnologiesAsync(
        CancellationToken cancellationToken = default);
    Task<TechnologyResponse?> GetTechnologyByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
}
