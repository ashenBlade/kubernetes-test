using Confluent.Kafka;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Platform.Web;
using Platform.Web.Helpers;
using Platform.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PlatformDbContext>(context =>
{
    context.UseNpgsql(builder.Configuration.GetConnectionString("Postgresql"));
});

builder.Services.AddSingleton<IProducer<Null, byte[]>>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var bootstrapServers = config.GetValue<string>("KAFKA_BOOTSTRAP_SERVERS");
    return new ProducerBuilder<Null, byte[]>(new ProducerConfig()
        {
            BootstrapServers = bootstrapServers,
        })
       .Build();
});

builder.Services.AddScoped<IPlatformRepository>(sp =>
{
    IPlatformRepository repo = new DatabasePlatformRepository(sp.GetRequiredService<PlatformDbContext>());
    repo = new EventPublisherPlatformRepositoryDecorator(sp.GetRequiredService<IProducer<Null, byte[]>>(), repo);
    return repo;
});

// Mapster
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

var app = builder.Build();

await DatabaseSeeder.SeedDbAsync(app);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();