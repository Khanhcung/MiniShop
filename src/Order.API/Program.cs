using BuildingBlocks.Data;
using BuildingBlocks.Messaging;
using Order.API.Consumers;
using Order.API.Repositories;
using Order.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SqlConnectionFactory>();

builder.Services.AddScoped<IOrderRepository,OrderRepository>();

builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration["Redis:Connection"]);
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHttpClient<ProductClient>(client => client.BaseAddress = new Uri(builder.Configuration["Product:BaseUrl"]!));
builder.Services.AddHostedService<InventoryEventsConsumer>();
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
