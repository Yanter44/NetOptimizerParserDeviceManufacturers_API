using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Enums;
using NetOptimizerParserApi.Models;

namespace NetOptimizerParserApi.Interfaces
{
    public interface IDeviceSaveStrategy
    {
        ParseDevice DeviceType { get; }
        Task<ServiceResponse<bool>> ProcessAndSaveAsync(List<ProductsModel> products, SitesToParse site);
    }
}
