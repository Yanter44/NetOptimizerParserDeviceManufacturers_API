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
        Task<ServiceResponse<bool>> RemoveCommutatorFromDbAsync(string ExternalId);
        Task<ServiceResponse<bool>> RemoveCommutatorsFromDbAsync(List<string> ExternalIds);
        Task<ServiceResponse<bool>> UpdateCommutatorAsync(string commutatorExternalId, CommutatorModelRequestDto commutatorModelRequestDto);
        Task<ServiceResponse<List<CommutatorResponceDto>>> GetAllCommutatorsAsync();
        Task<ServiceResponse<List<CommutatorResponceDto>>> GetCommutatorsByPriceRange(PriceRangeRequestDto priceRangeModel);
    } 
}
