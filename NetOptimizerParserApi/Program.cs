using Microsoft.EntityFrameworkCore;
using NetOptimizerParserApi.DbContext;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Services;
using NetOptimizerParserApi.Services.Business;
using NetOptimizerParserApi.Services.External;
using NetOptimizerParserApi.Services.Utility;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["NetOptimizerDb:DefaultConnection"]));

builder.Services.AddScoped<EltexParserService>();
builder.Services.AddScoped<GravitonParserService>();
builder.Services.AddScoped<ParserDispatcher>();
builder.Services.AddScoped<DeviceDispatcher>();

builder.Services.AddScoped<IParserStrategy, EltexParserService>();
builder.Services.AddScoped<IParserStrategy, GravitonParserService>();
builder.Services.AddScoped<IGigaChatAiService, GigaChatAIService>();
builder.Services.AddScoped<IPromtService, PromptGeneratorService>();

builder.Services.AddScoped<CommutatorService>();
builder.Services.AddScoped<ICommutatorService>(sp => sp.GetRequiredService<CommutatorService>());
builder.Services.AddScoped<IDeviceSaveStrategy>(sp => sp.GetRequiredService<CommutatorService>());

builder.Services.AddScoped<PcService>();
builder.Services.AddScoped<IPcService>(sp => sp.GetRequiredService<PcService>());
builder.Services.AddScoped<IDeviceSaveStrategy>(sp => sp.GetRequiredService<PcService>());

builder.Services.AddScoped<RouterService>();
builder.Services.AddScoped<IRouterService>(sp => sp.GetRequiredService<RouterService>());
builder.Services.AddScoped<IDeviceSaveStrategy>(sp => sp.GetRequiredService<RouterService>());
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
