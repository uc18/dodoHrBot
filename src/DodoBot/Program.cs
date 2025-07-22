using DodoBot.Extensions;
using DodoBot.Integrations;
using DodoBot.Interfaces;
using DodoBot.Interfaces.Integrations;
using DodoBot.Interfaces.Repositories;
using DodoBot.Interfaces.Services;
using DodoBot.Options;
using DodoBot.Providers;
using DodoBot.Repositories;
using DodoBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<ApplicationOptions>(configuration.GetSection(nameof(ApplicationOptions)));
builder.Services.AddLogging();
builder.Services.AddSupabaseContext(configuration);
builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();
builder.Services.AddScoped<DodoBotMessageService>();
builder.Services.AddScoped<ButtonProvider>();
builder.Services.AddScoped<AnswerBotService>();
builder.Services.AddScoped<IHuntflowApi, HuntflowApi>();
builder.Services.AddScoped<ISupabaseRepository, SupabaseRepository>();
builder.Services.AddScoped<HuntflowService>();
builder.Services.AddScoped<INotifyService, NotifyService>();
builder.Services.AddBackgroundsService();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();