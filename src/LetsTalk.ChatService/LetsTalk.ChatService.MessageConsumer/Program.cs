using LetsTalk.ChatService.Domain;
using LetsTalk.ChatService.MessageConsumer;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddMassTransitRabbitMq("letstalk-rabbitmq", massTransitConfiguration: config => config.AddConsumer<ChannelMessageConsumer>());

builder.AddNpgsqlDbContext<ChatDbContext>("letstalk-chat-service-db");

var app = builder.Build();

app.Run();
