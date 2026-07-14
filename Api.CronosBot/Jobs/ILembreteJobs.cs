namespace Api.CronosBot.Jobs
{
    public interface ILembreteJobs
    {
        Task ExecutarLembreteCincoDiasAsync();
        Task ExecutarLembreteQuatorzeDiasAsync();
    }
}
