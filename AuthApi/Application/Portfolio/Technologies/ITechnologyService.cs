using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Technologies.DTOs;

namespace AuthApi.Application.Portfolio.Technologies;
public interface ITechnologyService
{
    Task<ResponseResult<List<TechnologiesListResponse>>> GetAllTechnologiesAsync(
        CancellationToken cancellationToken = default);
    Task<ResponseResult<TechnologyResponse>?> GetTechnologyByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
}
