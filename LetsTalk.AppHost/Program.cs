var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgresContainer("postgres")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    //.WithEnvironment("POSTGRES_DB", "postgres")
    .WithVolumeMount("./docker_local/postgresql/data", "/var/lib/postgresql/data")
    .AddDatabase("lets_talk");

var redis = builder.AddRedisContainer("redis");

builder.AddProject<Projects.LetsTalk_WebApi>("webapi")
    .WithReference(postgres, "postgres")
    .WithReference(redis, "redis");


builder.Build().Run();
