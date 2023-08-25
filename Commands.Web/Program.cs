using Commands.Web.Services;
using Commands.Web.Workers;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<KafkaListenerWorker>();
builder.Services.AddSingleton<ICommandRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var multiplexer = ConnectionMultiplexer.Connect(config.GetValue<string>("REDIS_HOST"));
    return new RedisCommandRepository(multiplexer, sp.GetRequiredService<ILogger<RedisCommandRepository>>());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();