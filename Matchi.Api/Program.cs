using Matchi.Infrastructure.DependencyInjection;
using Matchi.Application.DependencyInjection;
using Matchi.Application.Features.Banks.Queries.GetBanks;
using Matchi.Infrastructure.Persistence;
using Matchi.Infrastructure.Persistence.Seed;
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(GetBanksQuery).Assembly));
        builder.Services.AddControllers();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<MatchiDbContext>();

    await DatabaseSeeder.SeedAsync(dbContext);
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// اگر بعداً JWT اضافه کردیم
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();