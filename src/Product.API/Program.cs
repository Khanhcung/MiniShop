using BuildingBlocks.Data;
using BuildingBlocks.Messaging;
using Product.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<SqlConnectionFactory>();

builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration["Redis:Connection"]);
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
