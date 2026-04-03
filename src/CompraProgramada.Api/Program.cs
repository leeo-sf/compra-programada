using CompraProgramada.Api.Config;
using CompraProgramada.Infra;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerConfiguration();
builder.Services.ConfigurarServicosApi(builder.Configuration);

var app = builder.Build();

app.UseHttpMetrics();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseSwaggerConfiguration();

app.UseHttpsRedirection();
app.MapControllers();

app.MapMetrics();

app.Run();