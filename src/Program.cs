using Dapper;
using GetARoomAPI.Core.DB.TypeHandlers;
using GetARoomAPI.Core.Extensions;
using GetARoomAPI.Core.Middleware;
using GetARoomAPI.Features.Endpoints;
using GetARoomAPI.Features.Endpoints.Diagnostics;
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

public partial class Program { }; // for integration testing (?) Trengs pga toplevel statement må ha entry point (pga er internal default når ikke setter public i main)