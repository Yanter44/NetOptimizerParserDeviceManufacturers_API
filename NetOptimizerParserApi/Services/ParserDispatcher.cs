using NetOptimizerParserApi.Enums;
using NetOptimizerParserApi.Interfaces;

namespace NetOptimizerParserApi.Services
{
    public class ParserDispatcher
    {
        private readonly IEnumerable<IParserStrategy> _strategies;

        public ParserDispatcher(IEnumerable<IParserStrategy> strategies)
        {
            _strategies = strategies;
        }
        public IParserStrategy GetStrategy(SitesToParse site)
        {
            return _strategies.FirstOrDefault(s => s.Site == site)
                   ?? throw new Exception("Парсер для данного сайта не реализован");
        }
    }
}
