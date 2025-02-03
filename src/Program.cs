using Dapper;
using RoomSchedulerAPI.Core.DB.TypeHandlers;
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

SqlMapper.AddTypeHandler(new UserIdHandler());

app.UseCors("AllowLocalhost");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapUserEndpoints();
app.MapApiHealthCheckEndpoint();
app.MapDbHealthCheckEndpoint();

app.Run();