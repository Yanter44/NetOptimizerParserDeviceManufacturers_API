using NetOptimizerParserApi.Interfaces;

namespace NetOptimizerParserApi.Services.Utility
{
    public class PromptGeneratorService : IPromtService
    {
        public string GetPricePrompt(string vendor, string model, Dictionary<string, string> specs)
        {
            string specsString = string.Join(", ", specs.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

            return "Инструкция по формату ответа:\n" +
                   "1. Выдай только целое число.\n" +
                   "2. Запрещено использовать любые разделители: пробелы, точки, запятые или нижние подчеркивания (_).\n" +
                   "3. Запрещено писать валюту (руб, руб., ₽) или любые пояснения.\n" +
                   "Задание: Дай ориентировочную цену устройства для России на 2026 год (с НДС).\n" +
                   $"Производитель: {vendor}. Модель: {model}. Характеристики: {specsString}.";
        }
    }
}
