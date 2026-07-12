using Api.CronosBot.FIlters;
using Application.CronosBot;
using Application.CronosBot.UseCases.CallApiEvolution;
using Application.CronosBot.UseCases.FlowEngine;
using Infrastructure.CronosBot;
using Infrastructure.CronosBot.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IWhatsappProvider, EvolutionApiProvider>((provider, client) =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["EvolutionApi:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("apikey", config["EvolutionApi:ApiKey"]);
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await MigrateDatabase();
app.Run();

async Task MigrateDatabase()
{
    await using var scope = app.Services.CreateAsyncScope();
    await DataBaseMigration.MigrateDatabase(scope.ServiceProvider);
}