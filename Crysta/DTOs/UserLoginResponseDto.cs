public class UserLoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string Region { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
