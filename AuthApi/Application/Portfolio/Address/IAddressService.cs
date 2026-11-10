using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Address.DTOs;

namespace AuthApi.Application.Portfolio.Address;

public interface IAddressService
{
    Task<ResponseResult<List<AddressResponseDto>>> GetAllAddressesAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<List<AddressResponseDto>>> GetAddressesByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<ResponseResult<AddressResponseDto>?> GetAddressByIdAsync(long addressId, CancellationToken cancellationToken = default);
    Task<AddressResponseDto> AddAddressAsync(AddressResponseDto address, CancellationToken cancellationToken = default);
    Task<bool> UpdateAddressAsync(AddressResponseDto address, CancellationToken cancellationToken = default);
    Task<bool> DeleteAddressAsync(long id, CancellationToken cancellationToken = default);

}
