namespace NetOptimizerParserApi.Interfaces
{
    public interface INetworkService
    {
        Task<byte[]> DownloadFileAsync(string url, CancellationToken ct);
    }
}
