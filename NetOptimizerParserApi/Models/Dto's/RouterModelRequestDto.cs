using NetOptimizerParserApi.Models.Components;

namespace NetOptimizerParserApi.Models.Dto_s
{
    public class RouterModelRequestDto : NetworkDeviceBase
    {
        public bool IsManaged { get; set; }
        public List<Port> Ports { get; set; } = new();
        public WifiOptions WifiOptions { get; set; } = new();
        public PerformanceSpecs Performance { get; set; } = new();
        public ProtocolSupport ProtocolSupport { get; set; } = new();
    }
}
