using NetOptimizerParserApi.Models.Components;

namespace NetOptimizerParserApi.Models.Dto_s
{
    public class RouterResponceDto
    {
        public Guid ExternalId { get; set; }
        public string Vendor { get; set; }
        public string Model { get; set; }
        public bool IsManaged { get; set; }
        public int TotalPorts { get; set; }
        public List<Port> Ports { get; set; } = new();
        public WifiOptions WifiOptions { get; set; } = new();
        public PerformanceSpecs Performance { get; set; } = new();
        public ProtocolSupport ProtocolSupport { get; set; } = new();
        public decimal AveragePrice { get; set; }
    }
}
