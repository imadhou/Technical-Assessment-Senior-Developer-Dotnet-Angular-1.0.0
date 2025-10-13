var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL with resource limits
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var mainDatabase = postgres.AddDatabase("database");

// Redis with configuration
var cache = builder.AddRedis("cache")
    .WithRedisCommander();

// API with full telemetry
var webapi = builder.AddProject<Projects.N2J_WebApi>("webapi")
    .WithReference(cache)
    .WithReference(mainDatabase)
    .WithOtlpExporter()
    .WithEnvironment("OTEL_SERVICE_NAME", "webapi")
    .WaitFor(postgres)
    .WaitFor(cache);

// Frontend
var angular = builder.AddNpmApp("angular", "../../../frontend/", "start")
    .WithReference(webapi)
    .WaitFor(webapi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();