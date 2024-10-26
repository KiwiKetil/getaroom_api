using RoomSchedulerAPI.Core.Dapper;
using RoomSchedulerAPI.Core.Diagnostics;
using RoomSchedulerAPI.Core.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);
//builder.RegisterMappers();

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
app.MapApiHealthCheckEndpoint();
app.MapDBHealthCheckEndpoint();

app.Run();