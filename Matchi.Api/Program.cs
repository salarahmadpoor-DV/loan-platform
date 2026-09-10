using FluentValidation;
using FluentValidation.AspNetCore;
using Matchi.Application.Common;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Common.Models;
using Matchi.Application.DependencyInjection;
using Matchi.Infrastructure.DependencyInjection;
using Matchi.Infrastructure.Persistence;
using Matchi.Infrastructure.Persistence.Seed;
using Matchi.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Matchi.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSettings = jwtSection.Get<JwtSettings>() ?? throw new InvalidOperationException("JWT configuration section 'Jwt' is missing.");
if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    throw new InvalidOperationException("JWT configuration missing required Jwt:Key.");
if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
    throw new InvalidOperationException("JWT configuration missing required Jwt:Issuer.");
if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
    throw new InvalidOperationException("JWT configuration missing required Jwt:Audience.");
if (jwtSettings.Key.Length < 32)
    throw new InvalidOperationException("JWT configuration Jwt:Key must be at least 32 characters long.");

builder.Services.Configure<JwtSettings>(jwtSection);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddValidatorsFromAssemblyContaining<Matchi.Application.Features.Auth.Commands.SendOtp.SendOtpCommandValidator>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Matchi API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter 'Bearer {token}' to authorize.",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddSingleton<IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

builder.Services.AddScoped<IAuthorizationHandler,
    PermissionAuthorizationHandler>();
builder.Services.AddAuthorization();

var app = builder.Build();

var seedEnabled = app.Configuration.GetValue("Seed:Enabled", false);
if (seedEnabled && app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MatchiDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}
else if (seedEnabled)
{
    app.Logger.LogWarning("Seed:Enabled is ignored because the environment is not Development.");
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;
        var traceId = context.TraceIdentifier;
        var exposeDetails = context.RequestServices
            .GetRequiredService<IHostEnvironment>()
            .IsDevelopment();

        if (exception is not null
            && exception is not ValidationException
            && exception is not UnauthorizedAccessException
            && exception is not KeyNotFoundException
            && exception is not ConflictException)
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Matchi.Api.ExceptionHandler");
            logger.LogError(exception, "Unhandled exception. TraceId {TraceId}", traceId);
        }

        var mapped = ExceptionHttpMapper.Map(exception, traceId, exposeDetails);

        var problemDetails = new ProblemDetails
        {
            Status = mapped.Status,
            Title = mapped.Title,
            Detail = mapped.Detail
        };
        problemDetails.Extensions["traceId"] = mapped.TraceId;
        if (mapped.Errors is not null)
            problemDetails.Extensions["errors"] = mapped.Errors;

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = mapped.Status;

        await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
