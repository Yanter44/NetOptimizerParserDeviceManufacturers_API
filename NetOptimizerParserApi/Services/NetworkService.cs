using NetOptimizerParserApi.Interfaces;

namespace NetOptimizerParserApi.Services
{
    public class NetworkService : INetworkService
    {
        private readonly HttpClient _httpClient;
        public NetworkService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<byte[]> DownloadFileAsync(string url, CancellationToken ct = default)
        {
            return await _httpClient.GetByteArrayAsync(url, ct);
        }
    }
}
