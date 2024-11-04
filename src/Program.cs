using RoomSchedulerAPI.Core.Extensions;
using RoomSchedulerAPI.Core.Middleware;
using RoomSchedulerAPI.Features.Endpoints;
using RoomSchedulerAPI.Features.Endpoints.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapUserEndpoints();
app.MapApiHealthCheckEndpoint();
app.MapDbHealthCheckEndpoint();

app.Run();