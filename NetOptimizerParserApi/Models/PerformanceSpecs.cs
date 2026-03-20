using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models
{
    [Owned]
    public class PerformanceSpecs
    {
        public double RoutingThroughputGbps { get; set; }
        public int RamMb { get; set; }
        public int FlashMb { get; set; }
    }
}
