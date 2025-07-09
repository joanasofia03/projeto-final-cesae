public class AppUserRole
{
    public int AppUser_ID { get; set; }
    public AppUser AppUser { get; set; } = null!;

    public int AppRole_ID { get; set; }
    public AppRole AppRole { get; set; } = null!;
}
