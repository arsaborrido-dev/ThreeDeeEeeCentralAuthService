using AuthApi.Domain.Entitities.Portfolio;

namespace AuthApi.Application.Portfolio.Contacts
{
    public interface IContactRepository
    {
        //Task<int> SaveContactMessageAsync(
        Task SaveContactMessageAsync(
            ContactMessage message,
            CancellationToken cancellationToken = default);
    }
}
