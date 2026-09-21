using System.Threading.Tasks;

namespace CatFactsApp.Services
{
    public interface IFactStatisticsService
    {
        Task<FactStatistics> GetStatisticsAsync();
    }
}