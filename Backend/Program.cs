using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Application.Contracts;
using Backend.Application.Extensions;
using Backend.Domain.ConfigurationModels;
using Backend.Domain.Entities;
using Backend.Extensions;
using Backend.Infrastructure.DataAccess;
using Backend.Infrastructure.Extensions;
using Backend.OptionsSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Setup authorization.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole",
        policy => policy.RequireRole(Backend.Application.Enums.Role.Administrator.ToString()));
});

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // You can customize the JSON serializer here (optional)
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Add Swagger.
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

// Configure strongly typed settings objects.
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
var appSettings = appSettingsSection.Get<AppSettingsOptions>();

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
        Name = Backend.Application.Enums.Role.Administrator.ToString(),
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

app.MapControllers();

app.Run();
