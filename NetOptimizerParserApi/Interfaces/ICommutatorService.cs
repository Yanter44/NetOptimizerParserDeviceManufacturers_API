using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.Dto_s;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetOptimizerParserApi.Interfaces
{
    public interface ICommutatorService
    {
        Task<ServiceResponse<bool>> AddCommutatorToDbAsync(CommutatorModelRequestDto model);
        Task<ServiceResponse<bool>> AddCommutatorsToDbAsync(List<CommutatorModelRequestDto> models);
        Task<ServiceResponse<List<CommutatorResponceDto>>> GetAllCommutatorsAsync();
        Task<ServiceResponse<List<CommutatorResponceDto>>> GetCommutatorsByPriceRange(PriceRangeRequestDto priceRangeModel);
    } 
}
