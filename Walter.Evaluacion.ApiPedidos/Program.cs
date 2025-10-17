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

app.MapPost("/procesa", async (CreatePedidoDto createPedidoDto,
    IPedidoService service,
    ITopicProducerService topicProducerService) =>
{
    if (createPedidoDto == null)
    {
        return Results.BadRequest(new { message = "El mensaje no puede estar vacio" });
    }

    // valida precision (decimal(9,2) => max 9999999.99)
    if (Math.Abs(createPedidoDto.MontoPedido) > 9999999.99m || createPedidoDto.MontoPedido <= 0m)
    {
        throw new ArgumentOutOfRangeException(nameof(createPedidoDto.MontoPedido), "Monto fuera del rango permitido (0.01 - 9999999.99)");
    }

    // valida que FormaPago solo pueda ser 1, 2 o 3
    if (createPedidoDto.FormaPago < 1 || createPedidoDto.FormaPago > 3)
    {
        return Results.BadRequest(new { message = "FormaPago inválida. Valores permitidos: 1, 2, 3." });
    }

    //Valida que el cliente exista
    var cliente = await service.GetClienteByIdAsync(createPedidoDto.IdCliente);
    if (cliente == null)
    {
        return Results.BadRequest(new { message = $"El cliente con id {createPedidoDto.IdCliente} no está registrado" });
    }

    //Crea un registro en pedido
    var pedido = await service.CreatePedidoAsync(createPedidoDto);

    //Crea un registro en pago
    var pago = await service.CreatePagoAsync(
        new CreatePagoDto
        {
            IdPedido = pedido.IdPedido,
            IdCliente = pedido.IdCliente,
            FormaPago = pedido.FormaPago,
            MontoPago = pedido.MontoPedido
        });

    //Genera mensaje para el topic
    var result = await topicProducerService.SendMessageAsync(
    new TopicMessageDto
    {
        IdPedido = pedido.IdPedido,
        NombreCliente = cliente.NombreCliente,
        IdPago = pago.IdPago,
        FormaPago = pedido.FormaPago,
        MontoPago = pedido.MontoPedido
    });
    return Results.Ok(result);

})
.WithName("ProcesaPedido")
.WithOpenApi();

app.MapGet("/clientes", async (IPedidoService service) =>
{
    return await service.GetClientesAsync();
})
.WithName("GetClientes")
.WithOpenApi();

app.MapGet("/clientes/{id}", async (int id, IPedidoService service) =>
{
    var cliente = await service.GetClienteByIdAsync(id);
    return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
})
.WithName("GetCliente")
.WithOpenApi();

app.Run();
