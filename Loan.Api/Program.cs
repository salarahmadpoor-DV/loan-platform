using Loan.Infrastructure.DependencyInjection;
using Loan.Application.DependencyInjection;
using Loan.Application.Features.Banks.Queries.GetBanks;
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