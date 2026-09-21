using System.Net.Http.Json;
using CatFactsApp.Models;

namespace CatFactsApp.Services
{
    public interface ICatFactClient
    {
        Task<CatFactResponse?> GetFactAsync(CancellationToken cancellationToken = default);
    }

    public class CatFactClient(HttpClient httpClient) : ICatFactClient
    {
        public async Task<CatFactResponse?> GetFactAsync(CancellationToken cancellationToken = default)
        {
            return await httpClient.GetFromJsonAsync<CatFactResponse>(
                string.Empty,
                cancellationToken);
        }
    }
}
