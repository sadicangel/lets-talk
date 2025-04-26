var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LetsTalk_WebApi>("letstalk-webapi")
    .WithOpenApiReference("scalar-api-reference", "Scalar API Reference", "scalar/v1");

builder.Build().Run();
