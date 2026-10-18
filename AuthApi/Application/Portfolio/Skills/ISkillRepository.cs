using AuthApi.Application.Portfolio.Skills.DTOs;

namespace AuthApi.Application.Portfolio.Skills;
public interface ISkillRepository
{
    Task<List<SkillsListResponse>> GetAllSkillsAsync(
        CancellationToken cancellationToken = default);
    Task<SkillResponse?> GetSkillByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
}
