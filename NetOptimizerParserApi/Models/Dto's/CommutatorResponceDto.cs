namespace NetOptimizerParserApi.Models.Dto_s
{
    public class CommutatorResponceDto
    {
        public Guid ExternalId { get; set; }
        public string Vendor { get; set; }  
        public string Model { get; set; }

        public int Layer { get; set; }  
        public bool IsManaged { get; set; }  
        public List<Port> Ports { get; set; } 
        public int TotalPorts { get; set; }
        public bool SupportsPoe { get; set; }
        public int PoeBudgetW { get; set; }
        public decimal ThroughputGbps { get; set; }  
        public int MacTableSize { get; set; }      
        public int MaxVlans { get; set; }  
        public decimal AveragePrice { get; set; }
    }
}
