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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder =>
        {
            builder
                .WithOrigins("http://127.0.0.1:5501", "http://localhost:5501")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
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
app.MapUserEndpoints();
app.MapApiHealthCheckEndpoint();
app.MapDbHealthCheckEndpoint();

app.Run();