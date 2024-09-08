using System.Reflection;
using System.Security.Claims;
using System.Text;
using Backend.Application.Contracts;
using Backend.Application.Extensions;
using Backend.Domain.ConfigurationModels;
using Backend.Domain.Entities;
using Backend.Extensions;
using Backend.Infrastructure.DataAccess;
using Backend.Infrastructure.Extensions;
using Backend.OptionsSetup;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Setup authorization.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole",
        policy => policy.RequireRole(Backend.Application.Enums.Role.Admin.ToString()));
});

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.UseMemberCasing();
    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
});

// Configure JWT authentication.
// Configure strongly typed settings objects.
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
var appSettings = appSettingsSection.Get<AppSettingsOptions>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);

builder.Services
    .AddAuthentication(configuration =>
    {
        configuration.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        configuration.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(configuration =>
    {
        configuration.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.Identity?.Name;
                if (userId == null)
                {
                    context.Fail("Unauthorized");
                    return Task.CompletedTask;
                }

                var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var user = db.Users.AsNoTracking().Include(x => x.Role)
                    .FirstOrDefault(x => x.Id == Guid.Parse(userId));

                if (user == null)
                {
                    context.Fail("Unauthorized");
                    return Task.CompletedTask;
                }

                if (user.RoleId != null)
                {
                    var identity = context.Principal?.Identity as ClaimsIdentity;
                    identity?.AddClaim(new Claim(ClaimTypes.Role, user.Role.Name));
                }

                return Task.CompletedTask;
            }
        };
        configuration.RequireHttpsMetadata = false;
        configuration.SaveToken = true;
        configuration.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    })
    .AddOpenIdConnect("oidc-google", options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = "https://accounts.google.com";
        options.RequireHttpsMetadata = false;
        options.ClientId = appSettings.OidcGoogleClientId;
        options.ClientSecret = appSettings.OidcGoogleClientSecret;
        options.ResponseType = $"{OpenIdConnectParameterNames.Code} {OpenIdConnectParameterNames.IdToken}";
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
    })
    .AddCookie();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter word 'Bearer ' followed by space and JWT into the field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "BackendClient",
        Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
        Description = "Backend",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

    var apiAssembly = Assembly.GetExecutingAssembly();
    var xmlApiFile = $"{apiAssembly.GetName().Name}.xml";
    var xmlApiPath = Path.Combine(AppContext.BaseDirectory, xmlApiFile);
    options.IncludeXmlComments(xmlApiPath);
});

builder.Host.AddServiceLogging();
builder.Host.UseServiceLogging();

builder.Services.ConfigureOptions<AppSettingsOptionsSetup>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed admin user and role.
using var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope();

if (serviceScope == null)
    throw new ApplicationException("Cannot create service scope.");

var db = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
var passwordHelper = serviceScope.ServiceProvider.GetService<IPasswordHelper>();

if (db == null || passwordHelper == null)
    throw new ApplicationException("Cannot get service.");

// Migrate any database changes on startup (includes initial database creation).
db.Database.Migrate();
if (db.Users.FirstOrDefault(x => x.Username == appSettings.AdminUsername) == null)
{
    var (passwordHash, passwordSalt) = passwordHelper.CreateHash(appSettings.AdminPassword);

    var user = new User
    {
        Id = Guid.NewGuid(),
        Username = appSettings.AdminUsername,
        Email = appSettings.AdminEmail,
        CreatedAt = DateTime.UtcNow,
        IsActive = true,
        PasswordHash = passwordHash,
        PasswordSalt = passwordSalt
    };

    db.Users.Add(user);
    db.SaveChanges();

    var adminRole = new Role
    {
        Id = Guid.NewGuid(),
        Name = Backend.Application.Enums.Role.Admin.ToString(),
        CreatedById = user.Id
    };

    var userRole = new Role
    {
        Id = Guid.NewGuid(),
        Name = Backend.Application.Enums.Role.User.ToString(),
        CreatedById = user.Id
    };

    var driverRole = new Role
    {
        Id = Guid.NewGuid(),
        Name = Backend.Application.Enums.Role.Driver.ToString(),
        CreatedById = user.Id
    };

    db.Roles.Add(adminRole);
    db.Roles.Add(userRole);
    db.Roles.Add(driverRole);
    user.RoleId = adminRole.Id;

    db.SaveChanges();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseApplicationMiddleware();

app.MapControllers();

app.Run();