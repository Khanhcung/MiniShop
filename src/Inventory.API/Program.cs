using BuildingBlocks.Data;
using BuildingBlocks.Messaging;
using Inventory.API.Consumers;
using Inventory.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddSingleton<SqlConnectionFactory>();
builder.Services.AddHostedService<OrderCreatedConsumer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
