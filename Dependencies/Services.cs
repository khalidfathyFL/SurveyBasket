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
/// Provides extension methods for adding and configuring services in the application.
/// </summary>
public static class Services
{
    /// <summary>
    /// Adds all application dependencies to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which dependencies are added.</param>
    /// <param name="configuration">The application configuration used to configure added services.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> with all dependencies added.</returns>
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Add cors
        services.AddCorsPolicyConfig();

        // Add validators and controllers
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddControllers();

        // Add database context
        AddApplicationDbContextConfig(services, configuration);

        // Add Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthServicesConfig(configuration);

        services.AddPollServicesConfig();
        return services;
    }

    /// <summary>
    ///     Configures and adds a CORS policy to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the CORS policy is added.</param>
    /// <returns>The updated <see cref="IServiceCollection" /> with the CORS policy configured.</returns>
    private static IServiceCollection AddCorsPolicyConfig(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("AllowAll",
            builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
        return services;
    }

    /// <summary>
    ///     Configures and adds the application's database context to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the database context is added.</param>
    /// <param name="configuration">The application configuration used to retrieve database connection settings.</param>
    /// <returns>The updated service collection with the database context configured.</returns>
    private static IServiceCollection AddApplicationDbContextConfig(IServiceCollection services,
        IConfiguration configuration)
    {
        // Add database context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    /// <summary>
    ///     Configures and adds Swagger-related services to the service collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to which Swagger services should be added.
    /// </param>
    /// <returns>
    ///     The updated service collection with Swagger services included.
    /// </returns>
    private static IServiceCollection AddSwaggerServicesConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddCors();

        return services;
    }

    /// <summary>
    ///     Configures and registers services related to polls in the dependency injection container.
    /// </summary>
    /// <param name="services">
    ///     The service collection to configure.
    /// </param>
    /// <returns>
    ///     The updated service collection with poll services registered.
    /// </returns>
    private static IServiceCollection AddPollServicesConfig(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();
        return services;
    }

    /// <summary>
    ///     Adds validation services and validators to the service collection.
    /// </summary>
    /// <param name="services">
    ///     The service collection to which the validation services and validators will be added.
    /// </param>
    /// <returns>
    ///     The modified service collection after adding validation services and validators.
    /// </returns>
    private static IServiceCollection AddValidationServicesConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    /// <summary>
    ///     Configures and registers Mapster services for object mapping in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the Mapster services will be added.</param>
    /// <returns>The updated service collection with Mapster services added.</returns>
    private static IServiceCollection AddMapsterServicesConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        services.AddMapster();

        return services;
    }

    /// <summary>
    ///     Configures and adds authentication services, including JWT authentication and identity management, to the service
    ///     collection.
    /// </summary>
    /// <param name="services">The service collection to which the authentication services will be added.</param>
    /// <param name="configuration">The application configuration instance used to bind authentication options.</param>
    /// <returns>The updated service collection with authentication services configured.</returns>
    private static IServiceCollection AddAuthServicesConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();

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