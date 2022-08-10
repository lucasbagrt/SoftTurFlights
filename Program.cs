using SoftTurFlights.Interfaces;
using SoftTurFlights.Repositories;
using Microsoft.AspNetCore.Mvc;
using SoftTurFlights.Models;
using Microsoft.OpenApi.Models;
using SoftTurFlights.Background;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddTransient<IFlightRepository, FlightRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SoftTurFlights", Description = "Soft Tur Flights", Version = "v1" });
});
builder.Host.UseWindowsService();
var services = builder.Services;
services.AddControllers().AddNewtonsoftJson();
services.AddHostedService<Processador>();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Soft Tur Flights"));

/// <summary>
/// CLIENT
/// </summary>
app.MapGet("/v1/client", ([FromServices] IClientRepository repository, string api_key) =>
{    
    return repository.Get(api_key);
});

app.MapPost("/v1/client", ([FromServices] IClientRepository repository, ClientModel client) =>
{
    var result = repository.Insert(client);
    return (result ? Results.Ok("Cliente inserido com sucesso") : Results.BadRequest("Nao foi possivel inserir o cliente"));
});

app.MapPut("/v1/client", ([FromServices] IClientRepository repository, string api_key, string client, string url) =>
{
    var result = repository.Update(api_key, client, url);
    return (result ? Results.Ok("Cliente alterado com sucesso") : Results.BadRequest("Nao foi possivel alterar a url desse cliente"));
});

app.MapDelete("/v1/client", ([FromServices] IClientRepository repository, string api_key) =>
{  
    var result = repository.Delete(api_key);
    return (result ? Results.Ok("Cliente excluido com sucesso") : Results.BadRequest("Nao foi possivel excluir este cliente"));
});

/// <summary>
/// FLIGHT
/// </summary>
app.MapGet("/v1/flight", ([FromServices] IFlightRepository repository, string api_key, DateTime start, DateTime end, string airport, string api_name) =>
{
    return repository.Get(api_key, start, end, airport, api_name);
});

app.MapPost("/v1/flight/send", ([FromServices] IFlightRepository repository, List<RequestSendFlight> req) =>
{
    return repository.SendFlight(req);
});
app.Run();
