using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var _jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (_jwtSettings == null || string.IsNullOrEmpty(_jwtSettings.SecretKey))
{
    throw new Exception("JwtSettings is not configured properly. Please check your appsettings.json file.");
}

builder.Services.AddDbContext<AnalyticPlatformContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey))
        };
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Invalid or expired token.\"}");
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Unauthorized access. Invalid or missing token.\"}");
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"You do not have permission to access this resource.\"}");
            }
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Admin seed

using (var scope = app.Services.CreateScope())
{

    var context = scope.ServiceProvider.GetRequiredService<AnalyticPlatformContext>();
    context.Database.Migrate();

    if (!context.AppRoles.Any(r => r.RoleName == "Administrator"))
    {
        var adminRoleEntry = context.AppRoles.Add(new AppRole
        {
            RoleName = "Administrator"
        });
        context.SaveChanges();

        var adminRoleId = adminRoleEntry.Entity.ID;

        if (!context.AppUsers.Any(u => u.Email == "admin@domain.com"))
        {
            var passwordHasher = new PasswordHasher<AppUser>();

            var adminUser = new AppUser
            {
                Email = "admin@domain.com",
                FullName = "Administrator",
                PhoneNumber = "99999999999",
                DocumentId = "12345678900",
                CreationDate = DateTime.Now,
                BirthDate = new DateTime(1990, 1, 1),
                Region = "Porto"
            };

            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "admin123");

            context.AppUsers.Add(adminUser);
            context.SaveChanges();

            context.AppUserRoles.Add(new AppUserRole
            {
                AppUser_ID = adminUser.ID,
                AppRole_ID = adminRoleId
            });
            context.SaveChanges();
        }
    }
    else
    {
        var adminRoleId = context.AppRoles.Single(r => r.RoleName == "Administrator").ID;

        if (!context.AppUsers.Any(u => u.Email == "admin@domain.com"))
        {
            var passwordHasher = new PasswordHasher<AppUser>();

            var adminUser = new AppUser
            {
                Email = "admin@domain.com",
                FullName = "Administrator",
                PhoneNumber = "99999999999",
                DocumentId = "12345678900",
                CreationDate = DateTime.Now,
                BirthDate = new DateTime(1990, 1, 1),
                Region = "Porto"
            };

            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "admin123");

            context.AppUsers.Add(adminUser);
            context.SaveChanges();

            context.AppUserRoles.Add(new AppUserRole
            {
                AppUser_ID = adminUser.ID,
                AppRole_ID = adminRoleId
            });
            context.SaveChanges();
        }
    }
}

app.Run();
