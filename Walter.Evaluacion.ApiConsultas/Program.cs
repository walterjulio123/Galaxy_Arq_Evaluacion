using MongoDB.Driver;
using Walter.Evaluacion.ApiConsultas.Configuration;
using Walter.Evaluacion.ApiConsultas.Data;
using Walter.Evaluacion.ApiConsultas.Services;

var builder = WebApplication.CreateBuilder(args);

//// Add MongoDB
//BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
//BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
//// Registrar ClassMap para forzar serializer en la propiedad concreta
//BsonClassMap.RegisterClassMap<Consulta>(cm =>
//{
//    cm.AutoMap();
//    cm.MapMember(c => c.MontoPago).SetSerializer(new DecimalSerializer(BsonType.Decimal128));
//});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var mongoClient = new MongoClient(connectionString);
var databaseName = "Consultas";
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddScoped<ConsultasDbContext>(provider =>
    new ConsultasDbContext(mongoClient, databaseName));

builder.Services.Configure<TopicConfig>(builder.Configuration.GetSection("Topic"));

builder.Services.AddHostedService<TopicConsumerService>();
builder.Services.AddScoped<IConsultasService, ConsultasService>();

// Add services to the container.
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

app.MapGet("/api/consulta", async (IConsultasService service) =>
{
    return await service.GetRegistrosAsync();
})
.WithOpenApi();


app.Run();

