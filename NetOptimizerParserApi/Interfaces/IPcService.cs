using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.Dto_s;

namespace NetOptimizerParserApi.Interfaces
{
    public interface IPcService
    {

        public Task<ServiceResponse<bool>> AddPcToDatabase(PcModelRequestDto model);
        public Task<ServiceResponse<bool>> AddPcsToDatabase(List<PcModelRequestDto> listOfModel);
        Task<ServiceResponse<bool>> RemovePcFromDbAsync(string ExternalId);
        Task<ServiceResponse<bool>> RemovePcsFromDbAsync(List<string> ExternalIds);
        Task<ServiceResponse<bool>> UpdatePcAsync(string pcExternalId, PcModelRequestDto PcModelRequestDto);
        public Task<ServiceResponse<List<PcResponceDto>>> GetAllPcsFromDatabaseAsync();
        public Task<ServiceResponse<List<PcResponceDto>>> GetPcsByPriceRange(PriceRangeRequestDto priceRange);

    }
}
