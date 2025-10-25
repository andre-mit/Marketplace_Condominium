using Azure.Storage.Blobs;
using Market.API.Data;
using Market.API.Data.Configurations;
using Market.API.Data.Repositories;
using Market.API.Hubs;
using Market.API.Services;
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

AddDataServices(builder);
AddServices(builder);
AddRepositories(builder.Services);

builder.Services.AddScoped<IEntityTypeConfiguration<User>, UserConfiguration>();

var app = builder.Build();

var shouldSeed = args.Contains("--seed-data");
if (shouldSeed)
{
    await using var scope = app.Services.CreateAsyncScope();
    await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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
return;

#region Local Functions

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<IPushNotificationService, PushNotificationService>();
    builder.Services.AddSingleton<IRedisKeyService, RedisKeyService>();

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    builder.Services.AddTransient<IAuthService, AuthService>();
    builder.Services.AddTransient<IProductService, ProductService>();

    builder.Services.AddSingleton<IUploadFileService, UploadFileService>();

    builder.Services.AddTransient<BlobServiceClient>(_ =>
        new BlobServiceClient(builder.Configuration.GetConnectionString("BlobStorageConnection")!));

    builder.Services.AddHostedService<OrphanedItemsProcessorService>();
}

static void AddRepositories(IServiceCollection services)
{
    services.AddScoped<IUsersRepository, UsersRepository>();
    services.AddScoped<IProductsRepository, ProductRepository>();
    services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
    services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
}

static void AddDataServices(WebApplicationBuilder builder)
{
    builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection"))
            .UseAsyncSeeding(async (context, _, ct) =>
            {
                var hasUserData = await context.Set<User>().AnyAsync(x => true, cancellationToken: ct);
                if (!hasUserData)
                {
                    var role = await context.Set<Role>().Include(r => r.Users)
                        .FirstAsync(r => r.Id == Constants.AdminRoleId,
                            cancellationToken: ct);
                    var user = new User
                    {
                        Id = new Guid("A1B2C3D4-E5F6-4789-ABCD-1234567890AB"),
                        FirstName = "Admin",
                        LastName = "User",
                        Email =
                            builder.Configuration.GetValue<string>("AdminUser:Email") ?? "admin@admin.com",
                        Cpf = "000.000.000-00",
                        PasswordHash =
                            BCrypt.Net.BCrypt.HashPassword(
                                builder.Configuration.GetValue<string>("AdminUser:Password") ??
                                "admin123"),
                        Birth = new DateOnly(1990, 1, 1),
                        Unit = "0",
                        Tower = "0",
                        CreatedAt = new DateTime(2025, 1, 1),
                        UpdatedAt = new DateTime(2025, 1, 1),
                        Roles = [role]
                    };

                    context.Set<User>().Add(user);

                    await context.SaveChangesAsync(ct);
                }
            });
    });

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
        options.InstanceName = builder.Configuration["RedisInstanceName"] ?? "MarketAPI_";
    });

    builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        var redisConnectionString = configuration.GetConnectionString("RedisConnection")!;
        return StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnectionString);
    });
}

#endregion