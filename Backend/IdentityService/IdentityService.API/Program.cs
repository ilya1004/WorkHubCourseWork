using IdentityService.API;
using IdentityService.API.Middlewares;
using IdentityService.BLL;
using IdentityService.DAL;
using HealthChecks.UI.Client;
using IdentityService.API.GrpcServices;
using IdentityService.BLL.Abstractions.AzuriteStartupService;
using IdentityService.DAL.Abstractions.DbStartupService;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHttpContextAccessor();

services.AddAPI(builder.Configuration);
services.AddBll(builder.Configuration);
services.AddDAL(builder.Configuration);

services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var azuriteStartupService = scope.ServiceProvider.GetRequiredService<IAzuriteStartupService>();
    await azuriteStartupService.CreateContainerIfNotExistAsync();
    
    var dbStartupService = scope.ServiceProvider.GetRequiredService<IDbStartupService>();
    await dbStartupService.InitializeDb();
}

app.UseRouting();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<GlobalLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<EmployersGrpcService>();
app.MapGrpcService<FreelancersGrpcService>();

app.MapControllers();

app.Run();