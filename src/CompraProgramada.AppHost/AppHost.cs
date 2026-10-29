var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("DefaultDb")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(port: 3306, name: "tcp")
    .WithDataVolume()
    .AddDatabase("mysql", databaseName: "compra-programada-db");

var kafka = builder.AddKafka("kafka");

builder.AddProject<Projects.CompraProgramada_Api>("api")
    .WithReference(mysql, "DefaultDb")
    .WithReference(kafka)
    .WaitFor(mysql)
    .WaitFor(kafka)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

builder.AddProject<Projects.CompraProgramada_MotorCompra_Worker>("motor-compra")
    .WithReference(mysql, "DefaultDb")
    .WithReference(kafka)
    .WaitFor(mysql)
    .WaitFor(kafka)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

await builder.Build().RunAsync();