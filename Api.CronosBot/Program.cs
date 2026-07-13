using Api.CronosBot.FIlters;
using Application.CronosBot;
using Application.CronosBot.UseCases.CallApiEvolution;
using Application.CronosBot.UseCases.FlowEngine;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
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

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Connection"))
));

builder.Services.AddHangfireServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
// Adicione essa opção para liberar o acesso ao painel de fora do container Docker
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new MyDashboardAuthorizationFilter() }
});

app.UseAuthorization();
app.MapControllers();

RecurringJob.AddOrUpdate<RecuperarLeadsSemReceitaUseCase>(
    "lembrete-receita-14-dias",
    useCase => useCase.ExecutarLembreteAutomaticoAsync(),
    "0 9 * * *",
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local
    });

await MigrateDatabase();
app.Run();

async Task MigrateDatabase()
{
    await using var scope = app.Services.CreateAsyncScope();
    await DataBaseMigration.MigrateDatabase(scope.ServiceProvider);
}