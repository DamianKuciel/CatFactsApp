using System.Net.Http.Json;
using CatFactsApp.Models;

namespace CatFactsApp.Services
{
    public interface ICatFactClient
    {
        Task<CatFactResponse?> GetFactAsync();
    }

    public class CatFactClient(HttpClient httpClient) : ICatFactClient
    {
        public async Task<CatFactResponse?> GetFactAsync() =>
            await httpClient.GetFromJsonAsync<CatFactResponse>("");
    }
}
