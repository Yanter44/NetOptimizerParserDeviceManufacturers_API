using Microsoft.EntityFrameworkCore;

namespace NetOptimizerParserApi.Models.Components
{
    [Owned]
    public class PcHardwareSpecs
    {
        public string CpuModel { get; set; }
        public int RamAmountGb { get; set; }
        public string RamType { get; set; }
        public string StorageType { get; set; }
        public int StorageAmountGb { get; set; }
    }
}
