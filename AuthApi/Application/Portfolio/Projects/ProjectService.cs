using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Projects.DTOs;

namespace AuthApi.Application.Portfolio.Projects;
public sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;       
    public ProjectService(IProjectRepository projectRepository) => _projectRepository = projectRepository;
    public async Task<ResponseResult<List<ProjectResponse>>> GetAllProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _projectRepository
            .GetAllProjectsAsync(cancellationToken);
        
        return ResponseResult<List<ProjectResponse>>.Success(response);
    }
    public async Task<ResponseResult<List<ProjectResponse>>> GetFeaturedProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _projectRepository
            .GetFeaturedProjectsAsync(cancellationToken);
        
        return ResponseResult<List<ProjectResponse>>.Success(response);
    }
    public async Task<ResponseResult<ProjectResponse>?> GetProjectByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var response = await _projectRepository
            .GetProjectByIdAsync(id, cancellationToken);

        return ResponseResult<ProjectResponse>.Success(response);
    }
}    
