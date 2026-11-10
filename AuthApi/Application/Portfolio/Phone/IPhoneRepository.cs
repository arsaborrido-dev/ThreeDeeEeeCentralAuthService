using AuthApi.Application.Portfolio.Phone.DTOs;

namespace AuthApi.Application.Portfolio.Phone
{
    public interface IPhoneRepository
    {
        Task<PhoneResponseRequestDto> AddPhoneAsync(PhoneResponseRequestDto dto, CancellationToken cancellationToken = default);
        Task<List<PhoneResponseRequestDto>> GetAllPhonesAsync(CancellationToken cancellationToken);
        Task<bool> UpdatePhoneAsync(PhoneResponseRequestDto dto, CancellationToken cancellationToken = default);
        Task<bool> SoftDeletePhoneAsync(long id, CancellationToken cancellationToken = default);
        Task<PhoneResponseRequestDto?> GetPhoneByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}
