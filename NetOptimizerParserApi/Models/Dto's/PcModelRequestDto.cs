namespace NetOptimizerParserApi.Models.Dto_s
{
    public class PcModelRequestDto : NetworkDeviceBase
    {
        public List<Port> Ports { get; set; }
        public PcHardwareSpecs HardwareSpecs { get; set; }
        public PcWifiOptions WifiOptions { get; set; }
    }
}
