using Amazon.S3;
using Market.API.Data;
using Market.API.Data.Configurations;
using Market.API.Data.Repositories;
using Market.API.Hubs;
using Market.API.Services;
using Market.API.SettingsModels;
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
    
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/hubs/chatHub")))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    var configuredOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();

        if (builder.Environment.IsDevelopment())
        {
            // Allow any origin in development but keep AllowCredentials enabled by using SetIsOriginAllowed
            policy.SetIsOriginAllowed(_ => true);
        }
        else if (configuredOrigins is { Length: > 0 })
        {
            policy.WithOrigins(configuredOrigins);
        }
        else
        {
            // Fallback to allowing any origin (unsafe) if nothing configured
            policy.SetIsOriginAllowed(_ => true);
        }
    });
});

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

// Ensure CORS runs early in the pipeline so preflight requests and SignalR negotiate
// endpoint return the correct Access-Control-Allow-* headers.
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapHub<ChatHub>("/hubs/chatHub");

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

    AddS3UploadFileService(builder);

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

static void AddS3UploadFileService(WebApplicationBuilder builder)
{
    builder.Services.Configure<S3Config>(
        builder.Configuration.GetSection("S3Config"));

    var s3Config = new AmazonS3Config
    {
        ServiceURL = builder.Configuration["S3Config:ServiceUrl"],
        ForcePathStyle = true,
    };

    var s3Client = new AmazonS3Client(
        builder.Configuration["S3Config:AccessKey"],
        builder.Configuration["S3Config:SecretKey"],
        s3Config
    );

    builder.Services.AddSingleton<IAmazonS3>(s3Client);
}

#endregion