using NetOptimizerParserApi.Models.Components;
using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Models.Dto_s
{
    public class CommutatorResponceDto
    {
        public Guid ExternalId { get; set; }
        public string Vendor { get; set; }  
        public string Model { get; set; }
        public int Layer { get; set; }  
        public bool IsManaged { get; set; }  
        public List<Port> Ports { get; set; } 
        public int TotalPorts { get; set; }
        public PoeSpecs PoeSpecs { get; set; }
        public SwitchPerformanceSpecs PerformanceSpecs { get; set; }
        public SwitchProtocolSupport ProtocolSupport { get; set; }
        public decimal AveragePrice { get; set; }
        public SwitchRoleType SwitchRoleType { get; set; }
    }
}
