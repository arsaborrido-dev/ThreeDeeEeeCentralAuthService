using AuthApi.Application.Portfolio.Address.DTOs;

namespace AuthApi.Application.Portfolio.Address;

public interface IAddressRepository
{
    Task<List<AddressResponseDto>> GetAllAddressesAsync(CancellationToken cancellationToken = default);
    Task<AddressResponseDto?> GetAddressByIdAsync(long addressId, CancellationToken cancellationToken = default);
    Task<List<AddressResponseDto>> GetAddressesByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<AddressResponseDto> AddAddressAsync(AddressResponseDto address, CancellationToken cancellationToken = default);
    Task<bool> UpdateAddressAsync(AddressResponseDto address, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAddressAsync(long id, CancellationToken cancellation = default);
}
