using bankAccount;
using bankAccount.Controllers;
using bankAccount.Data;
using bankAccount.Interfaces;
using bankAccount.Validation;
using bankAccount.Validators;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


    // Для Docker-окружения используем переменные окружения
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "db";
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "accounts";
    var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
    // ReSharper disable once StringLiteralTypo
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgrespw";

    var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";


// Add services to the container.

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddValidatorsFromAssemblyContaining<AccountValidator>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.CustomSchemaIds(id => id.FullName!.Replace('+', '.'));

    c.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://localhost:8081/realms/bank/protocol/openid-connect/auth"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "User profile" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Keycloak"
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer"
            },
            []
        }
    });
});

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IInterestAccrualService, InterestAccrualService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddValidatorsFromAssembly(typeof (Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient<GlobalExceptionHandler>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npg =>
    {
        npg.EnableRetryOnFailure();
        npg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
    }));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MetadataAddress = $"{builder.Configuration["Jwt:Authority"]}/.well-known/openid-configuration";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudiences = ["account", "account-service"], // Оба варианта
        ValidateIssuer = true,
        ValidIssuers =
        [
            "http://localhost:8081/realms/bank",  // Для запросов с хоста
                "http://keycloak:8080/realms/bank"   // Для запросов внутри Docker-сети
        ]
    };
    options.RequireHttpsMetadata = false;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

if (!builder.Environment.IsEnvironment("Test"))
{
#pragma warning disable CS0618 // Type or member is obsolete
    builder.Services.AddHangfire(h => h.UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions
    {
        QueuePollInterval = TimeSpan.FromSeconds(15),
        InvisibilityTimeout = TimeSpan.FromHours(3),
        PrepareSchemaIfNecessary = true,
    }));
#pragma warning restore CS0618 // Type or member is obsolete
    builder.Services.AddHangfireServer();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Проверка существования БД
        if (!dbContext.Database.CanConnect())
        {
            throw new Exception("Database is not available");
        }

        // Применяем миграции
        dbContext.Database.Migrate();

        // Проверка существования таблицы
        //if (!dbContext.Database.GetAppliedMigrations().Any())
        //{
        //    throw new Exception("Migrations not applied");
        //}
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "Database migration failed");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

if (!builder.Environment.IsEnvironment("Test"))
{
    app.UseHangfireDashboard("/dashboard");


    RecurringJob.AddOrUpdate<IInterestAccrualService>(
        "daily-interest-accrual",
        service => service.AccrueInterestForAllAccountsAsync(),
        Cron.Daily,
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Local,
            MisfireHandling = MisfireHandlingMode.Relaxed
        });
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
// не понял 
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// ReSharper disable once RedundantTypeDeclarationBody
public partial class Program {}