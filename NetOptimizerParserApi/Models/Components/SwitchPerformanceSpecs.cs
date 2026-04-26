using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models.Components
{
    [Owned]
    public class SwitchPerformanceSpecs
    {
        public decimal ThroughputGbps { get; set; }
        public int MacTableSize { get; set; }
        public int MaxVlans { get; set; }
    }
}
