using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Authentication.API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Authentication.Contracts.Config;
using Authentication.DAL;
using Authentication.DAL.Implementations;
using Authentication.DAL.Interfaces;
using Authentication.Services;
using Authentication.Services.Interfaces;
using Authentication.Services.Implementations;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Authentication.API;

public class Program
{
    public static void Main(string[] args)
    {
        // Load environment-specific configuration
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        Configure(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication API", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        
        // Rate limiter
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("FixedPolicy", policy =>
            {
                policy.PermitLimit = 100;
                policy.Window = TimeSpan.FromMinutes(1);
            });
        });

        // Custom Middleware
        services.AddTransient<ExceptionHandlingMiddleware>();
        services.AddTransient<RequestLoggingMiddleware>();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:3000", 
                        "http://localhost:3001",
                        "http://localhost:3002"
                        )
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // database
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AuthDb")));
        
        // JWT Authentication
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var settings = context.HttpContext.RequestServices.GetRequiredService<IOptions<JwtSettings>>().Value;
                        var token = context.Request.Cookies[settings.AccessCookie];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = key
                };
            });
        
        // Cookie policy
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.Always;
        });

        #region DI

        // Config Service
        services.Configure<PasswordHasherOptions>(configuration.GetSection("PasswordHasher"));
        services.Configure<SendGridSettings>(configuration.GetSection("SendGrid"));
        services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
        
        // Register Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPhoneService, PhoneService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
        services.AddScoped<IPhoneVerificationRepository, PhoneVerificationRepository>();
        #endregion

        // automapper
        services.AddAutoMapper(typeof(AuthenticationProfile).Assembly);
    }

    private static void Configure(WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("AllowFrontend");

        app.UseHttpsRedirection();
        app.UseCookiePolicy(); 
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers().RequireRateLimiting("FixedPolicy");
    }
}
