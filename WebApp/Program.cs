using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromMemory([
        new RouteConfig
        {
            RouteId = "api_route",
            ClusterId = "api_cluster",
            Match = new RouteMatch { Path = "{**catch-all}" }
        }
    ],
    [
        new ClusterConfig
        {
            ClusterId = "api_cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["api_destination"] = new DestinationConfig { Address = builder.Configuration.GetRequiredSection("Services:webapi:1").Value! }
            }
        }
    ]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapReverseProxy(/*opts => opts.Use(async (context, next) =>
{
    var accessTokenProvider = context.RequestServices.GetRequiredService<AccessTokenProvider>();
    var accessTokenResponse = await accessTokenProvider.ProvideAccessTokenAsync();
    context.Request.Headers.Authorization = $"Bearer {accessTokenResponse.AccessToken}";
    await next(context);
})*/);

app.MapFallbackToFile("/index.html");

app.Run();
