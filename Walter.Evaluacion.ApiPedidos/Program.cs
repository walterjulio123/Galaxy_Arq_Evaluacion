using Microsoft.EntityFrameworkCore;
using Walter.Evaluacion.ApiPedidos.Configuration;
using Walter.Evaluacion.ApiPedidos.Data;
using Walter.Evaluacion.ApiPedidos.DTOs;
using Walter.Evaluacion.ApiPedidos.Models;
using Walter.Evaluacion.ApiPedidos.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TopicConfig>(builder.Configuration.GetSection("Topic"));

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

app.MapPost("/procesa", async (CreatePedidoDto pedidoDto,
    IPedidoService service,
    ITopicProducerService topicProducerService) =>
{
    if (pedidoDto == null)
    {
        return Results.BadRequest(new { message = "El mensaje no puede estar vacio" });
    }
    //Valida que el cliente exista
    var cliente = await service.GetClienteByIdAsync(pedidoDto.IdCliente);
    if (cliente == null)
    {
        return Results.BadRequest(new { message = $"El cliente con id {pedidoDto.IdCliente} no está registrado" });
    }
    //Crea un registro en pedido
    var pedido = await service.CreatePedidoAsync(pedidoDto);

    //Crea un registro en pago
    var pago = await service.CreatePagoAsync(
        new CreatePagoDto
        {
            IdPedido = pedido.IdPedido,
            IdCliente = pedido.IdCliente,
            FormaPago = pedido.FormaPago,
            MontoPago = pedido.MontoPedido
        });

    var result = await topicProducerService.SendMessageAsync(
    new TopicMessage
    {
        IdPedido = pedido.IdPedido,
        NombreCliente = cliente.NombreCliente,
        IdPago = pago.IdPago,
        FormaPago = pedido.FormaPago,
        MontoPago = pedido.MontoPedido
    });
    return Results.Ok(result);

}).WithOpenApi();

app.Run();
