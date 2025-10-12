using Microsoft.EntityFrameworkCore;
using Walter.Evaluacion.ApiPedidos.Configuration;
using Walter.Evaluacion.ApiPedidos.Data;
using Walter.Evaluacion.ApiPedidos.DTOs;
using Walter.Evaluacion.ApiPedidos.Models;
using Walter.Evaluacion.ApiPedidos.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TopicConfig>(builder.Configuration.GetSection("topic"));

builder.Services.AddSingleton<ITopicProducerService, TopicProducerService>();
builder.Services.AddHttpClient<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PedidosDbContext>();
    context.Database.EnsureCreated();
}

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast")
//.WithOpenApi();

app.MapPost("/procesa", async (CreatePedidoDto pedidoDto,
    IPedidoService service,
    ITopicProducerService topicProducerService) =>
{
    if (pedidoDto == null)
    {
        return Results.BadRequest(new { message = "El mensaje no puede estar vacio" });
    }
    var pedido = await service.CreatePedidoAsync(pedidoDto);
    var result = await topicProducerService.SendMessageAsync(
    new TopicMessage
    {
        IdPedido = pedido.IdPedido,
        NombreCliente = "traer cliente",
        IdPago = 1,
        FormaPago = pedido.FormaPago,
        MontoPago = pedido.MontoPedido
    });
    return Results.Ok(result);

}).WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
