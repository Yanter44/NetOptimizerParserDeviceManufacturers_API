using NetOptimizerParserApi.Interfaces;

namespace NetOptimizerParserApi.Models
{
    public class RouterProductDetailsModel : ISpecificationProvider
    {
        public bool IsManaged { get; set; }
        public List<Port> Ports { get; set; } = new();
        public WifiOptions WifiOptions { get; set; } = new();
        public PerformanceSpecs Performance { get; set; } = new();
        public ProtocolSupport ProtocolSupport { get; set; } = new();
        public Dictionary<string, string> GetSpecificationsForAi()
        {
            var portsSummary = Ports != null && Ports.Any()
                ? string.Join(", ", Ports.GroupBy(p => new { p.Speed, p.Type })
                    .Select(g => $"{g.Sum(p => p.Count)} x {g.Key.Speed} {g.Key.Type}"))
                : "Данные о портах отсутствуют";

            return new Dictionary<string, string>
            {
                   { "Порты (интерфейсы):", portsSummary },
                   { "ОЗУ:", $"{Performance?.RamMb} MB" },
                   { "Flash-память:", $"{Performance?.FlashMb} MB" },
                   { "Пропускная способность:", $"{Performance?.RoutingThroughputGbps} Gbps" },
                   { "Wi-Fi:", WifiOptions?.HasWiFi == true ? $"Есть ({WifiOptions.WiFiStandard}, до {WifiOptions.MaxWirelessSpeed} Mbps)" : "Нет" },
                   { "Антенны:", WifiOptions?.HasWiFi == true ? $"{WifiOptions.AntennaCount} шт. ({WifiOptions.AntennaGain} dBi)" : "N/A" },
                   { "Поддержка NAT/Firewall:", ProtocolSupport?.SupportsNat == true ? "Да" : "Нет" },
                   { "Поддержка OSPF:", ProtocolSupport?.SupportsOspf == true ? "Да" : "Нет" },
                   { "Поддержка VRRP:", ProtocolSupport?.SupportsVrrp == true ? "Да" : "Нет" },
                   { "Поддержка IPsec:", ProtocolSupport?.SupportsIpsec == true ? "Да" : "Нет" }
            };
        }
    }
}
