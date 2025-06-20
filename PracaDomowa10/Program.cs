using Microsoft.EntityFrameworkCore;
using PracaDomowa10;
using PracaDomowa10.Data;
using PracaDomowa10.DbService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();// W Program.cs
builder.Services.AddScoped<IDbRepository, DbRepository>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddDbContext<TripsDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    opt.UseSqlServer(connectionString);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();