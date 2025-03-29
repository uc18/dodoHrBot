using DodoBot.BackgroundServices;
using DodoBot.Options;
using DodoBot.Services;

var builder = WebApplication.CreateBuilder(args);
var c = builder.Configuration;

builder.Services.Configure<ApplicationOptions>(c.GetSection(nameof(ApplicationOptions)));
builder.Services.AddScoped<MessageService>();
builder.Services.AddHostedService<TelegramBotBackgroundService>();
builder.Services.AddHttpClient();

var app = builder.Build();
app.Run();