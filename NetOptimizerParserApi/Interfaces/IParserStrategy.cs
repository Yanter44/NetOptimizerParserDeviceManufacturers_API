using NetOptimizerParserApi.Enums;
using NetOptimizerParserApi.Models;

namespace NetOptimizerParserApi.Interfaces
{
    public interface IParserStrategy
    {
        SitesToParse Site { get; }
        Task<List<ProductsModel>> ParseAsync(string url, ParserOptions options, CancellationToken cancellationToken);
    }
}
