using CompraProgramada.Api.Config;
using CompraProgramada.Infra;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerConfiguration();
builder.Services.AddControllers();
builder.Services.ConfigurarServicos(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseSwaggerConfiguration();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();