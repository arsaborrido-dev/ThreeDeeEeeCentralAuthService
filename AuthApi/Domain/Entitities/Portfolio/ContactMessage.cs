namespace AuthApi.Domain.Entitities.Portfolio
{
    public sealed class ContactMessage : SoftDeletableEntity
    {
        public long ContactMessageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset CreatedAtUtc { get; set; }  
        public bool IsRead { get; set; } = false;
    }
}
