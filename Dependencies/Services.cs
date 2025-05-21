using System.Reflection;
using System.Text;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.Contracts.Authentication;
using SurveyBasket.Persistence;

namespace SurveyBasket.Dependencies;

/// <summary>
///     Extension methods for registering services in the dependency injection container
/// </summary>
public static class Services
{
    /// <summary>
    ///     Adds all application dependencies to the service collection
    /// </summary>
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Add database context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPollService, PollService>();

        // Add validators and controllers
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddControllers();

        // Add Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthServicesConfig(configuration);
        return services;
    }

    private static IServiceCollection AddSwaggerServicesConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddCors();

        return services;
    }

    private static IServiceCollection AddPollServicesConfig(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();
        return services;
    }

    private static IServiceCollection AddValidationServicesConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    private static IServiceCollection AddMapsterServicesConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        services.AddMapster();

        return services;
    }

    private static IServiceCollection AddAuthServicesConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddAuthentication(options =>
        {
            // here we say that the default is bearer token
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!))
            };
        });

        return services;
    }
}