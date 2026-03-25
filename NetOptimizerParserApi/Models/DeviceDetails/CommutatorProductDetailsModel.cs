using NetOptimizerParserApi.Interfaces;

namespace NetOptimizerParserApi.Models.DeviceDetails
{
    public class CommutatorProductDetailsModel : ISpecificationProvider
    {
        public bool IsManaged { get; set; }
        public int Layer { get; set; }
        public List<Port> Ports { get; set; } = new List<Port>();
        public decimal ThroughputGbps { get; set; }
        public int MacTableSize { get; set; }
        public int MaxVlans { get; set; }
        public bool SupportsPoe { get; set; }
        public int PoeBudgetW { get; set; }
        public Dictionary<string, string> GetSpecificationsForAi()
        {
            var portsSummary = Ports != null && Ports.Any()
                ? string.Join(", ", Ports.GroupBy(p => new { p.Speed, p.Type })
                    .Select(g => $"{g.Sum(p => p.Count)} x {g.Key.Speed} {g.Key.Type}"))
                : "Данные о портах отсутствуют";

            return new Dictionary<string, string>
            {
                { "Тип:", IsManaged ? $"Управляемый (L{Layer})" : "Неуправляемый" },
                { "Уровень коммутации:", $"L{Layer}" },
                { "Интерфейсы:", portsSummary },
                { "Пропускная способность:", $"{ThroughputGbps} Gbps" },
                { "Таблица MAC-адресов:", $"{MacTableSize} записей" },
                { "Макс. количество VLAN:", MaxVlans.ToString() },
                { "Поддержка PoE:", SupportsPoe ? $"Да (Бюджет: {PoeBudgetW}W)" : "Нет" }
            };
        }

    }
}
