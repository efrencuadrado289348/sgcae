using System.Text.Json.Serialization;
using SGCAE.Api.ErrorHandling;
using SGCAE.Application;
using SGCAE.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Composición: la API es el único lugar que conoce todas las capas.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("Sgcae")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Sgcae'."));

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "SGCAE v1")); // /swagger
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
