// AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

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
    // For Exception details
    .WithEnvironment("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES", "true")
    // For structured event log data
    .WithEnvironment("OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES", "true")
    // FIXED: Use correct environment variable names for JWT configuration
    .WithEnvironment("JwtSettings__Secret", jwtSettings["Secret"])
    .WithEnvironment("JwtSettings__Issuer", jwtSettings["Issuer"])
    .WithEnvironment("JwtSettings__Audience", jwtSettings["Audience"])
    .WithEnvironment("JwtSettings__ExpirationMinutes", jwtSettings["ExpirationMinutes"])
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