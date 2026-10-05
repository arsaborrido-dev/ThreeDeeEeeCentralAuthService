using AuthApi.Application.Portfolio.Projects.DTOs;
using AuthApi.Domain.Entitities.Portfolio;

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
}
