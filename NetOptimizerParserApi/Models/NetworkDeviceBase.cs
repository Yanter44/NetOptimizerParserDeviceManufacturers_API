namespace NetOptimizerParserApi.Models
{
    public abstract class NetworkDeviceBase
    {
        public string Vendor { get; set; }
        public string Model { get; set; }
        public decimal AveragePrice { get; set; }
        public int TotalPorts { get; set; }
    }
}
