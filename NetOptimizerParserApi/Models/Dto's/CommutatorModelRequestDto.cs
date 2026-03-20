namespace NetOptimizerParserApi.Models
{
    public class CommutatorModelRequestDto : NetworkDeviceBase
    {
        public bool IsManaged { get; set; }
        public int Layer { get; set; }
        public List<Port> Ports { get; set; }
        public decimal ThroughputGbps { get; set; }
        public int MacTableSize { get; set; }
        public int MaxVlans { get; set; }
        public bool SupportsPoe { get; set; }
        public int PoeBudgetW { get; set; }
    }
}
