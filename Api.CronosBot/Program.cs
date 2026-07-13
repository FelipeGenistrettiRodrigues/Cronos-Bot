using Api.CronosBot.FIlters;
using Application.CronosBot;
using Application.CronosBot.UseCases.CallApiEvolution;
using Application.CronosBot.UseCases.FollowUpLeads;
using Domain.CronosBot.Models.Enums;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.CronosBot;
using Infrastructure.CronosBot.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IWhatsappProvider, EvolutionApiProvider>((provider, client) =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var baseUrl = config["EvolutionApi:BaseUrl"] ?? throw new InvalidOperationException("EvolutionApi:BaseUrl não configurada.");
    var apiKey = config["EvolutionApi:ApiKey"] ?? throw new InvalidOperationException("EvolutionApi:ApiKey não configurada.");

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("apikey", apiKey);
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
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Connection"))));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new MyDashboardAuthorizationFilter() }
});

app.UseAuthorization();
app.MapControllers();

var fusoSaoPaulo = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

RecurringJob.AddOrUpdate<IRecuperarLeadsSemReceitaUseCase>(
    "lembrete-receita-5-dias",
    useCase => useCase.ExecutarLembreteAutomaticoAsync(TipoLembreteReceita.CincoDias),
    "0 9 * * *",
    new RecurringJobOptions { TimeZone = fusoSaoPaulo });

RecurringJob.AddOrUpdate<IRecuperarLeadsSemReceitaUseCase>(
    "lembrete-receita-14-dias",
    useCase => useCase.ExecutarLembreteAutomaticoAsync(TipoLembreteReceita.QuatorzeDias),
    "30 9 * * *",
    new RecurringJobOptions { TimeZone = fusoSaoPaulo });

await MigrateDatabase();

app.Run();

async Task MigrateDatabase()
{
    await using var scope = app.Services.CreateAsyncScope();
    await DataBaseMigration.MigrateDatabase(scope.ServiceProvider);
}