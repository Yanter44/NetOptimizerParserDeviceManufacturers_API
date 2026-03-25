using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models.Components;

namespace NetOptimizerParserApi.Models.DeviceDetails
{
    public class PcProductDetailsModel : ISpecificationProvider
    {
        public List<Port> Ports { get; set; }
        public PcHardwareSpecs HardwareSpecs { get; set; }
        public PcWifiOptions WifiOptions { get; set; }
        public Dictionary<string, string> GetSpecificationsForAi()
        {
            return new Dictionary<string, string>
            {
                   { "Процессор", HardwareSpecs?.CpuModel ?? "N/A" },
                   { "ОЗУ", $"{HardwareSpecs?.RamAmountGb}GB {HardwareSpecs?.RamType}" },
                   { "Диск", $"{HardwareSpecs?.StorageAmountGb}GB {HardwareSpecs?.StorageType}" },
                   { "Wi-Fi", WifiOptions?.HasWiFi == true ? "Есть" : "Нет" }
            };
        }
    }
}
