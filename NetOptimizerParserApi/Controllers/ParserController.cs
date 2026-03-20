using Microsoft.AspNetCore.Mvc;
using NetOptimizerParserApi.Constants;
using NetOptimizerParserApi.Enums;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.DbEntities;
using NetOptimizerParserApi.Models.Dto_s;
using NetOptimizerParserApi.Services;
using NetOptimizerParserApi.Services.External;
using NetOptimizerParserApi.Services.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NetOptimizerParserApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParserController : ControllerBase
    {
        private readonly EltexParserService _eltexService;
        private readonly GravitonParserService _gravitonService;
        private readonly ParserDispatcher _parserDispatcher;
        private readonly DeviceDispatcher _deviceDispatcher;
        private readonly ICommutatorService _commutatorService;
        private readonly IRouterService _routerService;

        public ParserController(EltexParserService eltexService, 
                                GravitonParserService gravitonParserService,
                                ParserDispatcher parserDispatcher,
                                DeviceDispatcher deviceDispatcher,
                                ICommutatorService commutatorService,
                                IRouterService routerService)
        {
            _eltexService = eltexService;
            _gravitonService = gravitonParserService;
            _parserDispatcher = parserDispatcher;
            _deviceDispatcher = deviceDispatcher;
            _commutatorService = commutatorService;
            _routerService = routerService;
        }
      
        [HttpGet("ParseSite")]
        public async Task<IActionResult> ParseSite(SitesToParse site, ParseDevice device, CancellationToken cancellationToken)
        {
            if (!UrlSolver.UrlMapper.TryGetValue(site, out var url))
                return BadRequest("URL не найден");

            cancellationToken.ThrowIfCancellationRequested();
            var strategy = _parserDispatcher.GetStrategy(site);
            var products = await strategy.ParseAsync(url, new ParserOptions { ParsedDevices = device}, cancellationToken);

            var serviceWhatSave = _deviceDispatcher.GetProcessor(device);
            var result = await serviceWhatSave.ProcessAndSaveAsync(products, site);
            if (result.Success) { return Ok(result.Message); }
            else { return BadRequest(result.Message); }
        }
  

    }
}
