using Market.API.Data;
using Market.API.Hubs;
using Market.API.Services;
using Market.API.Services.Interfaces;
using Market.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;

    x.SaveToken = true;

    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey =
            new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(builder.Configuration["Jwt:PrivateKey"]!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection"))
        .UseAsyncSeeding(async (context, _, ct) =>
        {
            var hasData = await context.Set<User>().AnyAsync(x => true, cancellationToken: ct);
            if (!hasData)
            {
                context.Set<User>().Add(new User
                {
                    Id = new Guid("A1B2C3D4-E5F6-4789-ABCD-1234567890AB"),
                    FirstName = "Admin",
                    LastName = "User",
                    Email =
                        builder.Configuration.GetValue<string>("AdminUser:Email") ?? "admin@admin.com",
                    CPF = "000.000.000-00",
                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(builder.Configuration.GetValue<string>("AdminUser:Password") ??
                                                       "admin123"),
                    Birth = new DateOnly(1990, 1, 1),
                    Unit = "0",
                    Tower = "0"
                });

                await context.SaveChangesAsync(ct);
            }
        });
});

AddServices(builder.Services);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
await using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
{
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");

app.Run();

static void AddServices(IServiceCollection services)
{
    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => a.FullName != null && a.FullName.StartsWith("Market"))
        .ToArray();

    var types = assemblies
        .SelectMany(a => a.GetTypes())
        .Where(t => t.IsClass && !t.IsAbstract)
        .ToArray();

    foreach (var type in types)
    {
        var interfaces = type.GetInterfaces()
            .Where(i => i.Name == $"I{type.Name}")
            .ToArray();

        foreach (var @interface in interfaces)
        {
            services.AddTransient(@interface, type);
        }
    }
}