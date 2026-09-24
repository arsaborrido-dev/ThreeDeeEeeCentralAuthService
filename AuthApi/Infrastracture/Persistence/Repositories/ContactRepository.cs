
using AuthApi.Domain.Entitities.Portfolio;
using AuthApi.Application.Portfolio.Contacts;

namespace AuthApi.Infrastracture.Persistence.Repositories
{
    public sealed class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<int> SaveContactMessageAsync(
        public async Task SaveContactMessageAsync(
            ContactMessage message,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(message);

            await _context.ContactMessages.AddAsync(
                message,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            //return message.ContactMessageId;
        }
    }
}
