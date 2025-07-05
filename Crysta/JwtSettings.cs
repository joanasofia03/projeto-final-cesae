public class JwtSettings
{
    public string SecretKey { get; set; } = null!;
    public double TokenTTLMinutes { get; internal set; }
}
