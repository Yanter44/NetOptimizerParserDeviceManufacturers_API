using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models.Components
{
    [Owned]
    public class PcWifiOptions
    {
        public bool HasWiFi { get; set; }
    }
}
