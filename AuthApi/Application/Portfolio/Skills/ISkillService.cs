using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Skills.DTOs;

namespace AuthApi.Application.Portfolio.Skills;
public interface ISkillService
{
    Task<ResponseResult<List<SkillsListResponse>>> GetAllSkillsAsync(
        CancellationToken cancellationToken = default);
    Task<ResponseResult<SkillResponse>?> GetSkillByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
}
