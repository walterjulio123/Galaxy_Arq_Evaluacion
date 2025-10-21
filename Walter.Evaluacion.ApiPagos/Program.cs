using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Walter.Evaluacion.ApiPagos.Data;
using Walter.Evaluacion.ApiPagos.DTOs;
using Walter.Evaluacion.ApiPagos.Services;

var builder = WebApplication.CreateBuilder(args);

///configurando el tracing distribuido
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("api-pagos"))
    .WithTracing(t => {
        t.AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri("http://localhost:4317");
            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });

    });

builder.Services.AddScoped<IPagoService, PagoService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {

        o.Authority = "http://localhost:8080/realms/realm-pepito";
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "http://localhost:8080/realms/realm-pepito",
            ValidateIssuer = true,
            ValidAudience = "api-pagos",
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
builder.Services.AddDbContext<PagosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    var context = scope.ServiceProvider.GetRequiredService<PagosDbContext>();
    context.Database.EnsureCreated();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.RequireAuthorization()
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/api/pago", async (CreatePagoDto createPagoDto,
    IPagoService service) =>
{
    if (createPagoDto == null)
    {
        return Results.BadRequest(new { message = "El pago no puede estar vacio" });
    }
    // valida precision (decimal(9,2) => max 9999999.99)
    if (Math.Abs(createPagoDto.MontoPago) > 9999999.99m || createPagoDto.MontoPago <= 0m)
    {
        throw new ArgumentOutOfRangeException(nameof(createPagoDto.MontoPago), "Monto fuera del rango permitido (0.01 - 9999999.99)");
    }

    // valida que FormaPago solo pueda ser 1, 2 o 3
    if (createPagoDto.FormaPago < 1 || createPagoDto.FormaPago > 3)
    {
        return Results.BadRequest(new { message = "FormaPago inválida. Valores permitidos: 1, 2, 3." });
    }
    var result = await service.CreatePagoAsync(createPagoDto);
    return Results.Ok(result);

})
.RequireAuthorization()
.WithOpenApi();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
