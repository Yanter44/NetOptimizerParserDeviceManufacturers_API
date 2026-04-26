using NetOptimizerParserApi.Models.Components;

namespace NetOptimizerParserApi.Models.Dto_s
{
    public class RouterModelRequestDto : NetworkDeviceBase
    {
        public bool IsManaged { get; set; }
        public List<Port> Ports { get; set; } = new();
        public WifiOptions WifiOptions { get; set; } = new();
        public RouterPerformanceSpecs Performance { get; set; } = new();
        public RouterProtocolSupport ProtocolSupport { get; set; } = new();
    }
}
