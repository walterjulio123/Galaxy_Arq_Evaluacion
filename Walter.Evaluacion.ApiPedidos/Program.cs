using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Walter.Evaluacion.ApiPedidos.Configuration;
using Walter.Evaluacion.ApiPedidos.Data;
using Walter.Evaluacion.ApiPedidos.DTOs;
using Walter.Evaluacion.ApiPedidos.Models;
using Walter.Evaluacion.ApiPedidos.Services;

var builder = WebApplication.CreateBuilder(args);

///configurando el tracing distribuido
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("api-pedidos"))
    .WithTracing(t => {
        t.AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri("http://localhost:4317");
            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });

    });

builder.Services.Configure<TopicConfig>(builder.Configuration.GetSection("Topic"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ITopicProducerService, TopicProducerService>();
builder.Services.AddHttpClient<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = "http://localhost:8080/realms/realm-pepito";
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "http://localhost:8080/realms/realm-pepito",
            ValidateIssuer = true,
            ValidAudience = "api-pedidos",
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

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

app.MapPost("/api/procesa", async (CreatePedidoDto createPedidoDto,
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
        return Results.BadRequest(new { message = "FormaPago inv�lida. Valores permitidos: 1, 2, 3." });
    }

    //Valida que el cliente exista
    var cliente = await service.GetClienteByIdAsync(createPedidoDto.IdCliente);
    if (cliente == null)
    {
        return Results.BadRequest(new { message = $"El cliente con id {createPedidoDto.IdCliente} no est� registrado" });
    }

    //Crea un registro en pedido
    var pedido = await service.CreatePedidoAsync(createPedidoDto);

    //Crea un registro en pago
    var idPago = await service.CreatePagoAsync(
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
        IdPago = idPago,
        FormaPago = pedido.FormaPago,
        MontoPago = pedido.MontoPedido
    });
    return Results.Ok(new TopicMessageDto
    {
        IdPedido = pedido.IdPedido,
        NombreCliente = cliente.NombreCliente,
        IdPago = idPago,
        FormaPago = pedido.FormaPago,
        MontoPago = pedido.MontoPedido
    });

})
.RequireAuthorization()
.WithName("ProcesaPedido")
.WithOpenApi();

//app.MapGet("/api/clientes", async (IPedidoService service) =>
//{
//    return await service.GetClientesAsync();
//})
//.WithName("GetClientes")
//.WithOpenApi();

//app.MapGet("/api/clientes/{id}", async (int id, IPedidoService service) =>
//{
//    var cliente = await service.GetClienteByIdAsync(id);
//    return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
//})
//.WithName("GetCliente")
//.WithOpenApi();

app.Run();
