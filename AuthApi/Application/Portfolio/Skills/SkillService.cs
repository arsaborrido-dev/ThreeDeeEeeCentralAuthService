using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Skills.DTOs;

namespace AuthApi.Application.Portfolio.Skills;
public sealed class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepository;
    public SkillService(ISkillRepository skillRepository) => _skillRepository = skillRepository;
    public async Task<ResponseResult<List<SkillsListResponse>>> GetAllSkillsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _skillRepository
            .GetAllSkillsAsync(cancellationToken);

        return ResponseResult<List<SkillsListResponse>>.Success(response);
    }
    public async Task<ResponseResult<SkillResponse>?> GetSkillByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var response = await _skillRepository
            .GetSkillByIdAsync(id, cancellationToken);

        return ResponseResult<SkillResponse>.Success(response);
    }
}    
