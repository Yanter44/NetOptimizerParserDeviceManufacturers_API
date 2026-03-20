using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models
{
    [Owned]
    public class PcWifiOptions
    {
        public bool HasWiFi { get; set; }
    }
}
