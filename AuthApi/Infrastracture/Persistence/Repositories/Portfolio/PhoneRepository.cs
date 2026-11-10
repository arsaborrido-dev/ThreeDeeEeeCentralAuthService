using AuthApi.Application.Portfolio.Phone;
using AuthApi.Application.Portfolio.Phone.DTOs;
using AuthApi.Domain.Entitities.Portfolio;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Infrastracture.Persistence.Repositories.Portfolio;

public sealed class PhoneRepository : IPhoneRepository
{

    public readonly ApplicationDbContext _context;
    public PhoneRepository(ApplicationDbContext context) => _context = context;

    public async Task<PhoneResponseRequestDto> AddPhoneAsync(PhoneResponseRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entPhone = new Phone
        {
            AreaCode = dto.AreaCode,
            PhoneNumber = dto.PhoneNumber,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        _context.Phones.Add(entPhone);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entPhone);
    }

    public async Task<List<PhoneResponseRequestDto>> GetAllPhonesAsync(CancellationToken cancellationToken)
    {
        return await _context.Phones
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAtUtc)
            .Select(n => new PhoneResponseRequestDto
                {
                    Id = n.PhoneId,
                    AreaCode = n.AreaCode,
                    PhoneNumber = n.PhoneNumber,
                    CreatedAtUtc = n.CreatedAtUtc,
                    UpdatedAtUtc = n.UpdatedAtUtc
                }
            ).ToListAsync(cancellationToken);
    }

    public async Task<bool> SoftDeletePhoneAsync(long id, CancellationToken cancellationToken = default)
    {
        var entPhone = await _context.Phones.FindAsync(id);

        if (entPhone is null)
        {
            return false;
        }

        entPhone.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }

    public async Task<bool> UpdatePhoneAsync(PhoneResponseRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entPhone = await _context.Phones.FindAsync(dto.Id);

        if (entPhone is null)
        {
            return false;
        }

        entPhone.AreaCode = dto.AreaCode;
        entPhone.PhoneNumber = dto.PhoneNumber;
        entPhone.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }

    private static PhoneResponseRequestDto MapToDto(Phone ent)
    {
        return new PhoneResponseRequestDto
        {
            Id = ent.PhoneId,
            AreaCode = ent.AreaCode,
            PhoneNumber = ent.PhoneNumber,
            CreatedAtUtc = ent.CreatedAtUtc,
            UpdatedAtUtc = ent.UpdatedAtUtc
        };
    }

    public async Task<PhoneResponseRequestDto?> GetPhoneByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entPhone = await _context.Phones.FindAsync(id);

        if (entPhone is null)
        {
            return null;
        }

        return new PhoneResponseRequestDto
        {
            Id = entPhone.PhoneId,
            AreaCode = entPhone.AreaCode,
            PhoneNumber = entPhone.PhoneNumber,
            CreatedAtUtc = entPhone.CreatedAtUtc,
            UpdatedAtUtc = entPhone.UpdatedAtUtc
        };

    }
}
