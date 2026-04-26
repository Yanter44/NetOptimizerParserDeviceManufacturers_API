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
        private readonly IPcService _pcservice;
        private readonly ICommutatorService _commutatorService;
        private readonly IRouterService _routerService;
        public AdminController(IPcService pcService, ICommutatorService commutatorService, IRouterService routerService)
        {
            _pcservice = pcService;
            _commutatorService = commutatorService;
            _routerService = routerService;
        }

        [HttpPost("AddNewCommutator")]
        [Tags("Admin: Commutator")]
        public async Task<IActionResult> AddNewCommutator([FromBody] CommutatorModelRequestDto modelDto)
        {
            var result = await _commutatorService.AddCommutatorToDbAsync(modelDto);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpPatch("UpdateCommutator")]
        [Tags("Admin: Commutator")]
        public async Task<IActionResult> UpdateCommutator(string commutatorExternalId, [FromBody] CommutatorModelRequestDto modelDto)
        {
            var result = await _commutatorService.UpdateCommutatorAsync(commutatorExternalId, modelDto);
            if(result.Success)
                return Ok(result.Message);
            else 
                return BadRequest(result.Message);
        }
        [HttpDelete("RemoveCommutatorById")]
        [Tags("Admin: Commutator")]
        public async Task<IActionResult> RemoveCommutator(string commutatorExternalId)
        {
            var result = await _commutatorService.RemoveCommutatorFromDbAsync(commutatorExternalId);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
        [HttpDelete("RemoveCommutatorsByIds")]
        [Tags("Admin: Commutator")]
        public async Task<IActionResult> RemoveCommutators(List<string> commutatorsExternalIds)
        {
            var result = await _commutatorService.RemoveCommutatorsFromDbAsync(commutatorsExternalIds);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("AddNewRouter")]
        [Tags("Admin: Router")]
        public async Task<IActionResult> AddNewRouter([FromBody] RouterModelRequestDto modelDto)
        {
            var result = await _routerService.AddRouterToDbAsync(modelDto);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
        [HttpPatch("UpdateRouter")]
        [Tags("Admin: Router")]
        public async Task<IActionResult> UpdateRouter(string routerExternalId, [FromBody] RouterModelRequestDto modelDto)
        {
            var result = await _routerService.UpdateRouterAsync(routerExternalId, modelDto);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
        [HttpDelete("RemoveRouterById")]
        [Tags("Admin: Router")]
        public async Task<IActionResult> RemoveRouter(string routerExternalId)
        {
            var result = await _routerService.RemoveRouterFromDbAsync(routerExternalId);
            if (result.Success)
                return Ok(result.Message);
            else return BadRequest(result.Message);
        }
        [HttpDelete("RemoveRoutersByIds")]
        [Tags("Admin: Router")]
        public async Task<IActionResult> RemoveRouters(List<string> routerExternalIds)
        {
            var result = await _routerService.RemoveRoutersFromDbAsync(routerExternalIds);
            if (result.Success)
                return Ok(result.Message);
            else return BadRequest(result.Message);
        }
        [HttpPost("AddNewPc")]
        [Tags("Admin: Pc")]
        public async Task<IActionResult> AddNewPc([FromBody] PcModelRequestDto modelDto)
        {
            var result = await _pcservice.AddPcToDatabase(modelDto);
            if (result.Success)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
        [HttpPatch("UpdatePc")]
        [Tags("Admin: Pc")]
        public async Task<IActionResult> UpdatePc(string pcExternalId, PcModelRequestDto pcmodel)
        {
            var result = await _pcservice.UpdatePcAsync(pcExternalId, pcmodel);
            if (result.Success)
                return Ok(result.Message);
            else return BadRequest(result.Message);
        }
        [HttpDelete("RemovePcById")]
        [Tags("Admin: Pc")]
        public async Task<IActionResult> RemovePc(string pcExternalId)
        {
            var result = await _pcservice.RemovePcFromDbAsync(pcExternalId);
            if (result.Success)
                return Ok(result.Message);
            else return BadRequest(result.Message);

        }
        [HttpDelete("RemovePcsByIds")]
        [Tags("Admin: Pc")]
        public async Task<IActionResult> RemovePcs(List<string> pcExternalIds)
        {
            var result = await _pcservice.RemovePcsFromDbAsync(pcExternalIds);
            if (result.Success)
                return Ok(result.Message);
            else return BadRequest(result.Message);
        }
    }
}

