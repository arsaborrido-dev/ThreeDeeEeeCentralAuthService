using AuthApi.Application.Portfolio.Skills.DTOs;
using AuthApi.Domain.Entitities.Portfolio;

namespace AuthApi.Application.Portfolio.Skills
{
    public interface ISkillRepository
    {
        Task<List<SkillListResponse>> GetAllSkillsAsync(
            CancellationToken cancellationToken = default);

        Task<SkillResponse?> GetSkillByIdAsync(
            int id,
            CancellationToken cancellationToken = default);
    }

}
