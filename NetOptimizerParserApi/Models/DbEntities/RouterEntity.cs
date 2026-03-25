using NetOptimizerParserApi.Models.Components;

namespace NetOptimizerParserApi.Models.DbEntities
{
    public class RouterEntity
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Vendor { get; set; }  
        public string Model { get; set; }  
        public bool IsManaged { get; set; }
        public List<Port> Ports { get; set; } = new();
        public WifiOptions? WifiOptions { get; set; }
        public PerformanceSpecs? PerformanceSpecs { get; set; }
        public ProtocolSupport? ProtocolSupport { get; set; }
        public decimal AvveragePrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
