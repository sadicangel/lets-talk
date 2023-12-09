using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgresContainer("postgres", 5432)
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithVolumeMount("./docker_local/postgresql/data", "/var/lib/postgresql/data")
    .AddDatabase("lets_talk");

builder.AddContainer("pgadmin", "elestio/pgadmin")
    .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "postgres@email.com")
    .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "postgres")
    .WithEnvironment("PGADMIN_LISTEN_PORT", "8080")
    .WithVolumeMount("./docker_local/pgadmin4/servers.json", "/pgadmin4/servers.json")
    .WithServiceBinding(8080, 5433);

var redis = builder.AddRedisContainer("redis");

var webapi = builder.AddProject<LetsTalk_WebApi>("webapi")
    .WithReference(postgres, "postgres")
    .WithReference(redis, "redis");

builder.AddProject<LetsTalk_WebApp>("webapp")
    .WithReference(webapi);

builder.Build().Run();
