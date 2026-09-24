using AuthApi.Domain.Entitities.Portfolio;

namespace AuthApi.Application.Portfolio.Projects
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllProjectsAsync(
            CancellationToken cancellationToken = default);

        Task<Project?> GetProjectByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<Project>> GetFeaturedProjectsAsync(
            CancellationToken cancellationToken = default);
    }
}
