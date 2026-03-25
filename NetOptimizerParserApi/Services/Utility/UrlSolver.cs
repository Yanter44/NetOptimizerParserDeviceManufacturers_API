using NetOptimizerParserApi.Constants;
using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Services.Utility
{
    public static class UrlSolver
    {
        public static readonly Dictionary<SitesToParse, string> UrlMapper = new()
        {
              { SitesToParse.Eltex, SitesToParseAddresses.Eltex },
              { SitesToParse.Graviton, SitesToParseAddresses.Graviton}
        };
    }
}
