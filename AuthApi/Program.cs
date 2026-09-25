using AuthApi.Api.Middleware;
using AuthApi.Application.Auth;
using AuthApi.Application.Common.Interfaces;
using AuthApi.Application.Common.Models;
using AuthApi.Application.Portfolio.Contacts;
using AuthApi.Application.Portfolio.Projects;
using AuthApi.Application.Portfolio.Skills;
using AuthApi.Domain.Entitities;
using AuthApi.Infrastracture.Captcha;
using AuthApi.Infrastracture.Persistence;
using AuthApi.Infrastracture.Persistence.Repositories;
using AuthApi.Infrastracture.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

//THIS RESULTS IN OVERRIDING OF DOCKER ENVIRONMENT VARIABLE IF UNCOMMENTED
//builder.Configuration
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("Api/appsettings.json", optional: false, reloadOnChange: true)
//    .AddEnvironmentVariables();


//ASP.NET Core's rate limiter
//builder.Services.AddRateLimiter(options =>
//{
//    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

//    options.AddFixedWindowLimiter("auth-strict", limiterOptions =>
//    {
//        limiterOptions.PermitLimit = 5;
//        limiterOptions.Window = TimeSpan.FromMinutes(1);
//        limiterOptions.QueueLimit = 0;
//    });

//    options.AddFixedWindowLimiter("api-moderate", limiterOptions =>
//    {
//        limiterOptions.PermitLimit = 60;
//        limiterOptions.Window = TimeSpan.FromMinutes(1);
//        limiterOptions.QueueLimit = 0;
//    });
//});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                error = "rate_limit_exceeded",
                message = "Too many requests. Please try again later."
            },
            cancellationToken);
    };

    options.AddPolicy("auth-strict", httpContext =>
    {
        var ip =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.AddPolicy("api-strict", httpContext =>
    {
        var ip =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 2,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.AddPolicy("api-moderate", httpContext =>
    {
        var ip =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
    
});

// DB
//POSTGRE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    npgsql =>
    {
        npgsql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    }));

//SQLSERVER
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        sql =>
//        {
//            sql.EnableRetryOnFailure(
//                maxRetryCount: 5,
//                maxRetryDelay: TimeSpan.FromSeconds(10),
//                errorNumbersToAdd: null);
//            sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
//        }));

// Identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredUniqueChars = 1;
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var jwtSettings = builder.Configuration
        .GetSection("Jwt")
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "Jwt signing key is not configured.");

var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);

if (keyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must be at least 32 bytes.");
}

// JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        //ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

        //IssuerSigningKey = new SymmetricSecurityKey(
        //Encoding.UTF8.GetBytes(
        //    builder.Configuration["Jwt:Key"]
        //    ?? throw new InvalidOperationException("JWT key is not configured.")
        //)),
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        //ClockSkew = TimeSpan.FromMinutes(1)

        RequireExpirationTime = true,
        RequireSignedTokens = true,

        ClockSkew = TimeSpan.FromSeconds(30),

        ValidAlgorithms = new[]
        {
            SecurityAlgorithms.HmacSha256
        }
    };
});

//CAPTCHA
builder.Services.AddHttpClient<ICaptchaValidator, CaptchaValidator>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddAuthorization();

//CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("MvcPortFApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5010") // for docker mvc portfolio
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CaptchaSettings>(builder.Configuration.GetSection("Captcha"));

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddHealthChecks();
//.AddNpgSql(
//        builder.Configuration.GetConnectionString("DefaultConnection")!); <-- will need the appropriate PostgreSQL health-check package for AddNpgSql

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<HttpRequestLoggingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

//app.UseHttpsRedirection(); <-- comment for now until docker container is configured with an HTTPS endpoint/certificate.
//current flow: Browser -> https -> mvcportf -> http -> apiauth -> postgre protocol - postgres

app.UseRouting();
app.UseCors("MvcPortFApp");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<RateLimitMiddleware>(); <-- NO NEED. Will use ASP.Net Core RateLimiter

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    //seed only in development
    await SeedDatabaseAsync(app);
}

app.Run();
static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Applying migrations...");
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "User", "Admin" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                logger.LogInformation("Creating role {Role}", role);

                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(e => $"{e.Code}: {e.Description}"));

                    throw new InvalidOperationException(
                        $"Failed to create role '{role}': {errors}");
                }
            }
            else
            {
                logger.LogInformation(
                    "Role {Role} already exists.",
                    role);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database initialization failed.");
        throw;
    }
}