var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LetsTalk_Chat_WebApi>("letstalk-webapi")
    .WithOpenApiReference("scalar-api-reference", "Scalar API Reference", "scalar/v1");

builder.Build().Run();
