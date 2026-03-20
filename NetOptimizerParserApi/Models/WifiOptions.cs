using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models
{
    [Owned]
    public class WifiOptions
    {
        public bool HasWiFi { get; set; }
        public string WiFiStandard { get; set; }
        public double MaxWirelessSpeed { get; set; } // Суммарная скорость (1167 Мбит/с)
        public int AntennaCount { get; set; }      //кол-во антенн
        public double AntennaGain { get; set; }    // 6 dBi
        public string FrequencyBands { get; set; } // "2.4GHz, 5GHz"
    }
}
