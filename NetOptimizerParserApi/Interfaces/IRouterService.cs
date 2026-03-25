using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.Dto_s;

namespace NetOptimizerParserApi.Interfaces
{
    public interface IRouterService
    {
        Task<ServiceResponse<bool>> AddRouterToDbAsync(RouterModelRequestDto model);
        Task<ServiceResponse<bool>> AddRoutersToDbAsync(List<RouterModelRequestDto> models);
        Task<ServiceResponse<bool>> RemoveRouterFromDbAsync(string ExternalId);
        Task<ServiceResponse<bool>> RemoveRoutersFromDbAsync(List<string> ExternalIds);
        Task<ServiceResponse<bool>> UpdateRouterAsync(string routerExternalId, RouterModelRequestDto routerModelDto);
        Task<ServiceResponse<List<RouterResponceDto>>> GetAllRoutersAsync();
        Task<ServiceResponse<List<RouterResponceDto>>> GetRoutersByPriceRange(PriceRangeRequestDto priceRangeModel);
    }
}
