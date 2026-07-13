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
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new MyDashboardAuthorizationFilter() }
});

app.UseAuthorization();
app.MapControllers();

RecurringJob.AddOrUpdate<IRecuperarLeadsSemReceitaUseCase>(
    "lembrete-receita-5-dias",
    useCase => useCase.ExecutarLembreteAutomaticoAsync(TipoLembreteReceita.CincoDias),
    "0 9 * * *", // Expressão Cron: Roda todo dia às 09:00
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local
    });

RecurringJob.AddOrUpdate<IRecuperarLeadsSemReceitaUseCase>(
    "lembrete-receita-14-dias",
    useCase => useCase.ExecutarLembreteAutomaticoAsync(TipoLembreteReceita.QuatorzeDias), 
    "30 9 * * *", // Expressão Cron: Roda todo dia às 09:30 
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