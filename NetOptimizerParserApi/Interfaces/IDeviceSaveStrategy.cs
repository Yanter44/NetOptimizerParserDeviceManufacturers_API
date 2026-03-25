using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Interfaces
{
    public interface IDeviceSaveStrategy
    {
        ParseDevice DeviceType { get; }
        Task<ServiceResponse<bool>> ProcessAndSaveAsync(List<ProductsModel> products, SitesToParse site);
    }
}
