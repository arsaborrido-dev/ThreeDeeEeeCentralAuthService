using AuthApi.Application.Portfolio.Projects.DTOs;

namespace AuthApi.Application.Portfolio.Projects;
public interface IProjectRepository
{
    Task<List<ProjectResponse>> GetAllProjectsAsync(
        CancellationToken cancellationToken = default);
    Task<ProjectResponse?> GetProjectByIdAsync(
        long id,
        CancellationToken cancellationToken = default);
    Task<List<ProjectResponse>> GetFeaturedProjectsAsync(
        CancellationToken cancellationToken = default);
    Task<ProjectResponse> AddProjectAsync(ProjectResponse projDto, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectAsync(ProjectResponse projDto, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteProjectAsync(long id, CancellationToken cancellationToken = default);
}
