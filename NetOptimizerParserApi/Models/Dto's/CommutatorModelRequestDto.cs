using NetOptimizerParserApi.Models.Components;
using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Models
{
    public class CommutatorModelRequestDto : NetworkDeviceBase
    {
        public bool IsManaged { get; set; }
        public int Layer { get; set; }
        public List<Port> Ports { get; set; }
        public SwitchPerformanceSpecs PerformanceSpecs { get; set; }
        public PoeSpecs PoeSpecs { get; set; } 
        public SwitchProtocolSupport ProtocolSupport { get; set; }
        public SwitchRoleType SwitchRoleType { get; set; }
    }
}
