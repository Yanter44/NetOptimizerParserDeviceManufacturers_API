using NetOptimizerParserApi.Models.Components;

namespace NetOptimizerParserApi.Models.Dto_s
{
    public class PcResponceDto
    {
        public Guid ExternalId { get; set; }
        public string Vendor { get; set; }
        public string Model { get; set; }
        public List<Port> Ports { get; set; }
        public PcHardwareSpecs HardwareSpecs { get; set; }
        public PcWifiOptions WifiOptions { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
