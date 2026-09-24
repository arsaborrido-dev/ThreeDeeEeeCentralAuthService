using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Skills.DTOs;
using AuthApi.Domain.Entitities.Portfolio;

namespace AuthApi.Application.Portfolio.Skills
{
    public sealed class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<ResponseResult<List<SkillListResponse>>> GetAllSkillsAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _skillRepository
                .GetAllSkillsAsync(cancellationToken);

            return ResponseResult<List<SkillListResponse>>.Success(response);

        }

        public async Task<ResponseResult<SkillResponse>?> GetSkillByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var response = await _skillRepository
                .GetSkillByIdAsync(id, cancellationToken);

            return ResponseResult<SkillResponse>.Success(response);
        }
    }    
}
