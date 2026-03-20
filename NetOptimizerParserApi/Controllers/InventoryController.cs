using Microsoft.AspNetCore.Mvc;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models.Dto_s;

namespace NetOptimizerParserApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IPcService _pcService;
        private readonly ICommutatorService _commutatorService;
        private readonly IRouterService _routerService;
        public InventoryController(IPcService pcService,ICommutatorService commutatorService, IRouterService routerService)
        {
            _pcService = pcService;
            _commutatorService = commutatorService;
            _routerService = routerService;
        }

        [HttpGet("GetAllCommutators")]
        public async Task<IActionResult> GetAllCommutators()
        {
            var result = await _commutatorService.GetAllCommutatorsAsync();
            if (result.Success)
                return Ok(result.Data);
            else return BadRequest(result.Message);
        }
        [HttpGet("GetAllPcs")]
        public async Task<IActionResult> GetAllPcs()
        {
            var result = await _pcService.GetAllPcsFromDatabaseAsync();
            if (result.Success)
                return Ok(result.Data);
            else return BadRequest(result.Message);
        }
        [HttpGet("GetAllRouters")]
        public async Task<IActionResult> GetAllRouters()
        {
            var result = await _routerService.GetAllRoutersAsync();
            if (result.Success)
                return Ok(result.Data);
            else return BadRequest(result.Message);
        }
        [HttpGet("GetPcsByPriceRange")] 
        public async Task<IActionResult> GetPcsByPriceRange([FromQuery] PriceRangeRequestDto priceRangeRequestDto)
        {
            var result = await _pcService.GetPcsByPriceRange(priceRangeRequestDto);
            if (result.Success)
                return Ok(result.Data);
            else return BadRequest(result.Message);
        }
        [HttpGet("GetCommutatorsByPriceRange")]
        public async Task<IActionResult> GetCommutatorsByPriceRange([FromQuery] PriceRangeRequestDto priceRangeModel)
        {
            var result = await _commutatorService.GetCommutatorsByPriceRange(priceRangeModel);
            if (result.Success)
                return Ok(result.Data);
            else return BadRequest(result.Message);
        }
    }
}
