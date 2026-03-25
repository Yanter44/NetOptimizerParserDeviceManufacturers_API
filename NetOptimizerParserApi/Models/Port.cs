using NetOptimizerParserApi.Models.Enums;

namespace NetOptimizerParserApi.Models
{
    public class Port
    {
        public int Count { get; set; }          // Количество таких портов
        public string Speed { get; set; }       // Пропускная способность, например "1G", "10G"
        public PortType Type { get; set; }      // Тип интерфейса, например "RJ-45", "SFP", "Combo"
        public bool SupportsPoe { get; set; }   // Может ли питать устройства
    }
}
