using Neo4jClient;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//neo4j 
var client = new BoltGraphClient(new Uri("bolt://localhost:7687"), "neo4j", "edukacija");
client.ConnectAsync();
builder.Services.AddSingleton<IGraphClient>(client);

//redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                  ConnectionMultiplexer.Connect("localhost:6379"));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
