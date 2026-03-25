using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Interfaces
{
    public interface IParserStrategy
    {
        SitesToParse Site { get; }
        Task<ServiceResponse<List<ProductsModel>>> ParseAsync(string url, ParserOptions options, CancellationToken cancellationToken);
    }
}
