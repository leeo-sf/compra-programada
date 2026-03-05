using CompraProgramada.Infra;
using CompraProgramada.Worker.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigurarServicosWorker(builder.Configuration);

builder.Services.AddHostedService<MotorCompraWorker>();

var host = builder.Build();
host.Run();