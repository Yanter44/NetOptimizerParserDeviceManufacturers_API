using Microsoft.AspNetCore.Mvc;
using NetOptimizerParserApi.Constants;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.DbEntities;
using NetOptimizerParserApi.Models.Dto_s;
using NetOptimizerParserApi.Models.Enums;
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
        private readonly ParserDispatcher _parserDispatcher;
        private readonly DeviceDispatcher _deviceDispatcher;

        public ParserController(ParserDispatcher parserDispatcher,
                                DeviceDispatcher deviceDispatcher)
        {
            _parserDispatcher = parserDispatcher;
            _deviceDispatcher = deviceDispatcher;
        }
      
        [HttpGet("ParseSite")]
        public async Task<IActionResult> ParseSite(SitesToParse site, ParseDevice device, CancellationToken cancellationToken)
        {
            if (!UrlSolver.UrlMapper.TryGetValue(site, out var url))
                return BadRequest("URL не найден");

            cancellationToken.ThrowIfCancellationRequested();
            var strategy = _parserDispatcher.GetStrategy(site);
            var parseResult = await strategy.ParseAsync(url, new ParserOptions { ParsedDevices = device}, cancellationToken);
            if(parseResult != null)
            {
                var serviceWhatSave = _deviceDispatcher.GetProcessor(device);
                var result = await serviceWhatSave.ProcessAndSaveAsync(parseResult.Data, site);
                if (result.Success) { return Ok(result.Message); }
                else { return BadRequest(result.Message); }
            }
            return BadRequest(parseResult.Message);
       
        }
  

    }
}
