using Hangfire.Dashboard;

namespace Api.CronosBot.FIlters
{
    public class MyDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Em desenvolvimento/Docker local, permite que qualquer um acesse
            return true;
        }
    }
}
