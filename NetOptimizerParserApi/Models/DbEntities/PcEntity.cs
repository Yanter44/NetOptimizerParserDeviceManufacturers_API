namespace NetOptimizerParserApi.Models.DbEntities
{
    public class PcEntity
    {
        public int Id { get; set; }
        public string Vendor { get; set; }
        public string Model { get; set; }
        public List<Port> Ports { get; set; } = new();
        public PcHardwareSpecs HardwareSpecs { get; set; }
        public PcWifiOptions WifiOptions { get; set; }
        public decimal AveragePrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
