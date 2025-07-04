public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public List<string> Roles { get; set; } = new List<string>();
}
