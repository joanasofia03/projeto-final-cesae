public class UserLoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public List<string> Roles { get; set; } = new();
}
