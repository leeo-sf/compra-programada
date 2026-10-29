var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("DefaultDb")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(port: 5432, name: "tcp")
    .WithDataVolume()
    .AddDatabase("postgres", databaseName: "compra-programada-db");

var kafka = builder.AddKafka("kafka");

builder.AddProject<Projects.CompraProgramada_Api>("api")
    .WithReference(postgres, "DefaultDb")
    .WithReference(kafka)
    .WaitFor(postgres)
    .WaitFor(kafka)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

builder.AddProject<Projects.CompraProgramada_MotorCompra_Worker>("motor-compra")
    .WithReference(postgres, "DefaultDb")
    .WithReference(kafka)
    .WaitFor(postgres)
    .WaitFor(kafka)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

await builder.Build().RunAsync();