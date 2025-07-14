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

builder.Services.Configure<TransactionSettings>(
    builder.Configuration.GetSection("TransactionSettings"));

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

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

using (var scope = app.Services.CreateScope())
{

    var context = scope.ServiceProvider.GetRequiredService<AnalyticPlatformContext>();
    context.Database.Migrate();

    // MARKET ASSETS SEED
    await DataSeeder.SeedMarketAssetsFromCoinGecko(context);
    var asset = context.Dim_Market_Asset.FirstOrDefault(a => a.Symbol == "BTC");

    // MARKET ASSET HISTORY SEED
    await DataSeeder.SeedFactMarketAssetHistory(context);

    // CLIENT ROLE SEED
    if (!context.AppRoles.Any(r => r.RoleName == "Client"))
    {
        context.AppRoles.Add(new AppRole { RoleName = "Client" });
        context.SaveChanges();
    }

    var clientRoleId = context.AppRoles.Single(r => r.RoleName == "Client").ID;

    // PASSWORD HASHER
    var passwordHasher = new PasswordHasher<AppUser>();

    // TIME SEED
    if (!context.Dim_Time.Any(t => t.ID == 1))
    {
        var date1 = new DateTime(2024, 3, 15);

        var timeEntry1 = new Dim_Time
        {
            date_Date = date1,
            date_Year = date1.Year,
            date_Month = date1.Month,
            date_Quarter = (date1.Month - 1) / 3 + 1,
            Weekday_Name = date1.DayOfWeek.ToString(),
            Is_Weekend = date1.DayOfWeek == DayOfWeek.Saturday || date1.DayOfWeek == DayOfWeek.Sunday
        };

        context.Dim_Time.Add(timeEntry1);
        context.SaveChanges();
    }

    if (!context.Dim_Time.Any(t => t.ID == 2))
    {
        var date2 = new DateTime(2024, 4, 22);

        var timeEntry2 = new Dim_Time
        {
            date_Date = date2,
            date_Year = date2.Year,
            date_Month = date2.Month,
            date_Quarter = (date2.Month - 1) / 3 + 1,
            Weekday_Name = date2.DayOfWeek.ToString(),
            Is_Weekend = date2.DayOfWeek == DayOfWeek.Saturday || date2.DayOfWeek == DayOfWeek.Sunday
        };

        context.Dim_Time.Add(timeEntry2);
        context.SaveChanges();
    }

    // CLIENT USERS SEED
    var client1 = context.AppUsers.FirstOrDefault(u => u.Email == "client1@domain.com");
    var client2 = context.AppUsers.FirstOrDefault(u => u.Email == "client2@domain.com");

    if (client1 == null)
    {
        client1 = new AppUser
        {
            Email = "client1@domain.com",
            FullName = "Client One",
            PhoneNumber = "1234567890",
            DocumentId = "11111111111",
            BirthDate = new DateTime(1995, 5, 10),
            Region = "Lisbon",
            CreationDate = DateTime.UtcNow,
        };
        client1.PasswordHash = passwordHasher.HashPassword(client1, "client123");
        context.AppUsers.Add(client1);
        context.SaveChanges();

        context.AppUserRoles.Add(new AppUserRole
        {
            AppUser_ID = client1.ID,
            AppRole_ID = clientRoleId
        });
        context.SaveChanges();
    }

    if (client2 == null)
    {
        client2 = new AppUser
        {
            Email = "client2@domain.com",
            FullName = "Client Two",
            PhoneNumber = "0987654321",
            DocumentId = "22222222222",
            BirthDate = new DateTime(1993, 8, 20),
            Region = "Porto",
            CreationDate = DateTime.UtcNow,
        };
        client2.PasswordHash = passwordHasher.HashPassword(client2, "client123");
        context.AppUsers.Add(client2);
        context.SaveChanges();

        context.AppUserRoles.Add(new AppUserRole
        {
            AppUser_ID = client2.ID,
            AppRole_ID = clientRoleId
        });
        context.SaveChanges();
    }

    // ACCOUNT SEED
    var account1 = context.Dim_Accounts.FirstOrDefault(a => a.AppUser_ID == client1.ID);
    var account2 = context.Dim_Accounts.FirstOrDefault(a => a.AppUser_ID == client2.ID);

    if (account1 == null)
    {
        account1 = new Dim_Account
        {
            Account_Type = "Checking",
            Account_Status = "Active",
            AppUser_ID = client1.ID,
            Opening_Date = DateTime.UtcNow.Date,
            Currency = "USD"
        };
        context.Dim_Accounts.Add(account1);
        context.SaveChanges();
    }

    if (account2 == null)
    {
        account2 = new Dim_Account
        {
            Account_Type = "Checking",
            Account_Status = "Active",
            AppUser_ID = client2.ID,
            Opening_Date = DateTime.UtcNow.Date,
            Currency = "USD"
        };
        context.Dim_Accounts.Add(account2);
        context.SaveChanges();
    }

    // ACCOUNT CREATION NOTIFICATIONS SEED
    if (!context.Fact_Notifications.Any(n =>
        (n.AppUser_ID == client1.ID && n.Notification_Type == "Account Created") ||
        (n.AppUser_ID == client2.ID && n.Notification_Type == "Account Created")))
    {

        // Notification for Client1 account creation
        context.Fact_Notifications.Add(new Fact_Notifications
        {
            AppUser_ID = client1.ID,
            Time_ID = 1, // Use appropriate existing or new time entry
            Notification_Type = "Account Created",
            Channel = "Seeder",
            Fact_Notifications_Status = "Completed"
        });

        // Notification for Client2 account creation
        context.Fact_Notifications.Add(new Fact_Notifications
        {
            AppUser_ID = client2.ID,
            Time_ID = 1,
            Notification_Type = "Account Created",
            Channel = "Seeder",
            Fact_Notifications_Status = "Completed"
        });

        context.SaveChanges();
    }

    // TRANSACTION TYPE SEED
    if (!context.Dim_Transaction_Types.Any(t => t.Dim_Transaction_Type_Description == "Transfer"))
    {
        context.Dim_Transaction_Types.Add(new Dim_Transaction_Type
        {
            Dim_Transaction_Type_Description = "Transfer"
        });
        context.SaveChanges();
    }
    var transferTypeId = context.Dim_Transaction_Types
        .First(t => t.Dim_Transaction_Type_Description == "Transfer").ID;

     if (!context.Dim_Transaction_Types.Any(t => t.Dim_Transaction_Type_Description == "Deposit"))
    {
        context.Dim_Transaction_Types.Add(new Dim_Transaction_Type
        {
            Dim_Transaction_Type_Description = "Deposit"
        });
        context.SaveChanges();
    }
    var depositTypeId = context.Dim_Transaction_Types
        .First(t => t.Dim_Transaction_Type_Description == "Deposit").ID;

    // TRANSACTIONS SEED
    if (!context.Fact_Transactions.Any(t => t.Source_Account_ID == account1.ID && t.Destination_Account_ID == account2.ID))
    {
        // Transaction 1: Client1 deposits 1000 USD
        context.Fact_Transactions.Add(new Fact_Transactions
        {
            Source_Account_ID = account1.ID,
            Destination_Account_ID = account1.ID, // Deposit to self
            Time_ID = 1,
            Transaction_Type_ID = depositTypeId,
            AppUser_ID = client1.ID,
            Transaction_Amount = 1000m,
            Balance_After_Transaction = 1000m, // Assume initial balance was 0
            Execution_Channel = "MobileApp",
            Transaction_Status = "Completed"
        });

        // Transaction 2: Client2 deposits 1100 USD
        context.Fact_Transactions.Add(new Fact_Transactions
        {
            Source_Account_ID = account2.ID,
            Destination_Account_ID = account2.ID, // Deposit to self
            Time_ID = 2,
            Transaction_Type_ID = depositTypeId,
            AppUser_ID = client2.ID,
            Transaction_Amount = 1100m,
            Balance_After_Transaction = 1100m, // Assume initial balance was 0
            Execution_Channel = "WebPortal",
            Transaction_Status = "Completed"
        });

        // Transaction 3: Client1 transfers 200 USD to Client2
        context.Fact_Transactions.Add(new Fact_Transactions
        {
            Source_Account_ID = account1.ID,
            Destination_Account_ID = account2.ID,
            Time_ID = 3,
            Transaction_Type_ID = transferTypeId,
            AppUser_ID = client1.ID,
            Transaction_Amount = 200m,
            Balance_After_Transaction = 800m, // After transfer
            Execution_Channel = "MobileApp",
            Transaction_Status = "Completed"
        });

        // Transaction 4: Client2 transfers 300 USD to Client1
        context.Fact_Transactions.Add(new Fact_Transactions
        {
            Source_Account_ID = account2.ID,
            Destination_Account_ID = account1.ID,
            Time_ID = 4,
            Transaction_Type_ID = transferTypeId,
            AppUser_ID = client2.ID,
            Transaction_Amount = 300m,
            Balance_After_Transaction = 1400m, // After transfer
            Execution_Channel = "WebPortal",
            Transaction_Status = "Completed"
        });

        context.SaveChanges();
    }

    // NOTIFICATIONS FOR TRANSACTIONS SEED
    if (!context.Fact_Notifications.Any(n => n.Notification_Type == "Transaction Sent") &&
        !context.Fact_Notifications.Any(n => n.Notification_Type == "Transaction Received"))
    {

        // Notification for Client1 depositing money
        context.Fact_Notifications.Add(new Fact_Notifications
        {
            AppUser_ID = client1.ID,
            Time_ID = 1,
            Notification_Type = "Deposit Sent",
            Channel = "MobileApp",
            Fact_Notifications_Status = "Processed"
        });

        // Notification for Client2 depositing money
        context.Fact_Notifications.Add(new Fact_Notifications
        {
            AppUser_ID = client2.ID,
            Time_ID = 2,
            Notification_Type = "Deposit Sent",
            Channel = "WebPortal",
            Fact_Notifications_Status = "Processed"
        });
        
        // Notification for Client1 sending money to Client2
        context.Fact_Notifications.Add(new Fact_Notifications
        {
            AppUser_ID = client1.ID,
            Time_ID = 3,
            Notification_Type = "Transaction Sent",
            Channel = "MobileApp",
            Fact_Notifications_Status = "Processed"
        });

        // Notification for Client2 receiving money from Client1
        context.Fact_Notifications.Add(new Fact_Notifications
        {
            AppUser_ID = client2.ID,
            Time_ID = 3,
            Notification_Type = "Transaction Received",
            Channel = "MobileApp",
            Fact_Notifications_Status = "Processed"
        });

        // Notification for Client2 sending money to Client1
        context.Fact_Notifications.Add(new Fact_Notifications
        {
            AppUser_ID = client2.ID,
            Time_ID = 4,
            Notification_Type = "Transaction Sent",
            Channel = "WebPortal",
            Fact_Notifications_Status = "Processed"
        });

        // Notification for Client1 receiving money from Client2
        context.Fact_Notifications.Add(new Fact_Notifications
        {
            AppUser_ID = client1.ID,
            Time_ID = 4,
            Notification_Type = "Transaction Received",
            Channel = "WebPortal",
            Fact_Notifications_Status = "Processed"
        });

        context.SaveChanges();
    }

    // ADMIN
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

        // MARKET ASSET HISTORY SEED
    if (asset == null)
    {
        Console.WriteLine("Bitcoin asset not found. Seed the asset first.");
        return;
    }

}

app.Run();
