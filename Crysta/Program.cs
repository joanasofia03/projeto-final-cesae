using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar a string de conexão (ajuste conforme seu ambiente)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Adicionar DbContext
builder.Services.AddDbContext<AnalyticPlatformContext>(options =>
    options.UseSqlServer(connectionString));

// Adicionar OpenAPI (Swagger)
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Endpoint para criar Role
app.MapPost("/api/roles", async (CreateRoleDto dto, AnalyticPlatformContext context) =>
{
    if (string.IsNullOrWhiteSpace(dto.role_name))
        return Results.BadRequest("RoleName is required.");

    var exists = await context.AppRoles.AnyAsync(r => r.role_name == dto.role_name);
    if (exists)
        return Results.Conflict("Role already exists.");

    var role = new AppRole { role_name = dto.role_name };

    context.AppRoles.Add(role);
    await context.SaveChangesAsync();

    return Results.Created($"/api/roles/{role.id}", role);
});

// Endpoint para obter role por id (opcional)
app.MapGet("/api/roles/{id:int}", async (int id, AnalyticPlatformContext context) =>
{
    var role = await context.AppRoles.FindAsync(id);
    return role is not null ? Results.Ok(role) : Results.NotFound();
});

app.Run();