using CompraProgramada.Infra;
using CompraProgramada.Worker.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigurarServicosWorker(builder.Configuration);

builder.Services.AddHostedService<MotorDeCompraWorker>();

var host = builder.Build();
host.Run();