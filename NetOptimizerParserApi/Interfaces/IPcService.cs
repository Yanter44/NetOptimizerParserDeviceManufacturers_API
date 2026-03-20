using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Models.Dto_s;

namespace NetOptimizerParserApi.Interfaces
{
    public interface IPcService
    {

        public Task<ServiceResponse<bool>> AddPcToDatabase(PcModelRequestDto model);
        public Task<ServiceResponse<bool>> AddPcsToDatabase(List<PcModelRequestDto> listOfModel);
        public Task<ServiceResponse<bool>> RemovePcFromDatabase(int pcId);
        public Task<ServiceResponse<bool>> UpdatePcInDatabase(int pcId,PcModelRequestDto model);
        public Task<ServiceResponse<PcResponceDto>> GetPcFromDatabase(int pcId);
        public Task<ServiceResponse<List<PcResponceDto>>> GetAllPcsFromDatabaseAsync();
        public Task<ServiceResponse<List<PcResponceDto>>> GetPcsByPriceRange(PriceRangeRequestDto priceRange);

    }
}
