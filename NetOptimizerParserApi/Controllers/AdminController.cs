using Microsoft.AspNetCore.Mvc;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.Dto_s;

namespace NetOptimizerParserApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ICommutatorService _commutatorService;
        private readonly IRouterService _routerService;
        public AdminController(ICommutatorService commutatorService, IRouterService routerService)
        {
            _commutatorService = commutatorService;
            _routerService = routerService;
        }

        [HttpPost("AddNewCommutator")]
        public async Task<IActionResult> AddNewCommutator([FromBody] CommutatorModelRequestDto modelDto)
        {
            var result = await _commutatorService.AddCommutatorToDbAsync(modelDto);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
  
        [HttpPost("AddNewRouter")]
        public async Task<IActionResult> AddNewRouter([FromBody] RouterModelRequestDto modelDto)
        {
            var result = await _routerService.AddRouterToDbAsync(modelDto);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        
    }
}

