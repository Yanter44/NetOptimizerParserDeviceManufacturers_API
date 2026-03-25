using Microsoft.Playwright;
using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Extensions;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.Components;
using NetOptimizerParserApi.Models.DeviceDetails;
using NetOptimizerParserApi.Models.Enums;
using NetOptimizerParserApi.Services.Utility;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace NetOptimizerParserApi.Services.External
{
    public class GravitonParserService : BasePlaywrightParser<ProductsModel, ParserOptions>, IParserStrategy
    {
        private readonly IGigaChatAiService _gigaChatAiService;
        private readonly IPromtService _promtService;
        public SitesToParse Site => SitesToParse.Graviton;

        public GravitonParserService(IGigaChatAiService gigachatAiService, IPromtService promtService)
        {
            _gigaChatAiService = gigachatAiService;
            _promtService = promtService;
        }
        public Task<ServiceResponse<List<ProductsModel>>> ParseAsync(string url, ParserOptions options, CancellationToken cancellationToken)
        {
            return this.ExecuteAsync(url, options, cancellationToken);
        }
        protected async override Task<ServiceResponse<List<ProductsModel>>> ParsePageAsync(IPage page, ParserOptions options, CancellationToken cancellationToken)
        {
            List<ProductsModel> data = options.ParsedDevices switch
            {
                ParseDevice.PС => await ParsePcs(page, cancellationToken),
                _ => null
            };
            if (data == null)
                return new ServiceResponse<List<ProductsModel>> { Success = false, Message = $"Тип устройства {options.ParsedDevices} не поддерживается для {Site}" };

            return new ServiceResponse<List<ProductsModel>> { Data = data, Success = true, Message = $"Успешно спаршено. Найдено: {data.Count} шт." };
        }

        public async Task<List<ProductsModel>> ParsePcs(IPage page, CancellationToken cancellationToken)
        {
            var finalBuilds = new List<ProductsModel>();
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            var catalogBtn = page.GetByRole(AriaRole.Button, new() { Name = "Каталог" }).First;
            var clientDevices = page.GetByRole(AriaRole.Button, new() { Name = "Клиентские устройства" }).First;
            var pcLink = page.Locator("li").GetByRole(AriaRole.Link, new() { Name = "ПК" }).First;

            await catalogBtn.ClickWithRetryAsync(clientDevices);
            await clientDevices.HoverAsync();
            await pcLink.ClickAsync();

            var allCards = page.Locator(".catalog__item");
            await allCards.First.WaitForAsync();
            int count = await allCards.CountAsync();

            string currentProductCode = "";
            TaskCompletionSource<bool> tcs = null;

            EventHandler<IResponse> universalHandler = async (sender, response) =>
            {
                if (response.Url.Contains($"/api/app/catalog/product/{currentProductCode}"))
                {
                    try
                    {
                        var jsonText = await response.TextAsync();
                        var data = JObject.Parse(jsonText)["data"];
                        if (data == null)
                        {
                            tcs?.TrySetResult(true);
                            return;
                        }

                        var itemName = data["name"]?.ToString().Replace("ПК «Гравитон» ", "") ?? "Unknown";
                        var tabs = data["tabs"];
                        if (tabs == null)
                        {
                            tcs?.TrySetResult(true);
                            return;
                        }

                        var availableCpus = new List<string>();
                        var ramOptions = new List<(int Amount, string Type)>();
                        var storageOptions = new List<(int Amount, string Type)>();
                        var parsedPorts = new List<Port>();
                        bool hasWiFiFromSite = false;

                        var review = tabs["review"];
                        var blocks = review?["firstBlocks"] as JArray;

                        foreach (var block in blocks ?? new JArray())
                        {
                            var blockName = block["name"]?.ToString();
                            var items = block["items"] as JArray;

                            if (blockName == "ПРОИЗВОДИТЕЛЬНОСТЬ")
                            {
                                foreach (var prop in items)
                                {
                                    var propName = prop["name"]?.ToString();
                                    var textArray = prop["text"] as JArray;
                                    var firstText = textArray?.FirstOrDefault()?.ToString() ?? "";
                                    var secondText = textArray?.Count > 1 ? textArray[1]?.ToString() : "";

                                    if (propName == "Процессор")
                                    {
                                        var chipset = secondText.Replace("Чипсет ", "").Trim();
                                        if (SupportedChipsets.ChipsetsAndProccessors.TryGetValue(chipset, out var cpus))
                                            availableCpus.AddRange(cpus);
                                        else
                                            availableCpus.Add(chipset);
                                    }
                                    else if (propName == "Оперативная память")
                                    {
                                        string ramType = Regex.Match(firstText, @"DDR\d").Value;
                                        var matches = Regex.Matches(secondText, @"(\d+)");
                                        if (matches.Count >= 2)
                                        {
                                            int min = int.Parse(matches[0].Value);
                                            int max = int.Parse(matches[matches.Count - 1].Value);
                                            for (int s = min; s <= max; s *= 2) ramOptions.Add((s, ramType));
                                        }
                                        else if (matches.Count == 1) ramOptions.Add((int.Parse(matches[0].Value), ramType));
                                    }
                                    else if (propName == "Дисковая подсистема")
                                    {
                                        string type = firstText.Contains("SSD") ? "SSD" : "HDD";
                                        storageOptions.Add((256, type));
                                        storageOptions.Add((512, type));
                                        storageOptions.Add((1024, type));
                                    }
                                }
                            }
                            else if (blockName == "СВЯЗЬ")
                            {
                                foreach (var prop in items)
                                {
                                    var pName = prop["name"]?.ToString();
                                    var pText = prop["text"]?.FirstOrDefault()?.ToString() ?? "";

                                    if (pName == "Проводное соединение")
                                    {
                                        var port = new Port
                                        {
                                            Count = Regex.Match(pText, @"(\d+)\s*[xх]").Success ? int.Parse(Regex.Match(pText, @"(\d+)\s*[xх]").Groups[1].Value) : 1,
                                            Speed = Regex.Match(pText, @"(\d+)\s*(Гбит/с|Мбит/с|Gbps)").Value,
                                            Type = pText.Contains("RJ-45") ? PortType.RJ45 : PortType.Unknown
                                        };
                                        parsedPorts.Add(port);
                                    }
                                    else if (pName == "Беспроводное соединение")
                                    {
                                        hasWiFiFromSite = pText.Contains("Wi-Fi", StringComparison.OrdinalIgnoreCase);
                                    }
                                }
                            }
                        }
                        var allBuilds = new List<ProductsModel>();
                        foreach (var cpu in availableCpus)
                        {
                            foreach (var ram in ramOptions)
                            {
                                foreach (var storage in storageOptions)
                                {

                                    var currentWifiOptions = new PcWifiOptions
                                    {
                                        HasWiFi = hasWiFiFromSite
                                    };
                                    var specs = new PcProductDetailsModel
                                    {
                                        Ports = parsedPorts.Select(p => new Port
                                        {
                                            Count = p.Count,
                                            Speed = p.Speed,
                                            Type = p.Type
                                        }).ToList(),
                                        WifiOptions = currentWifiOptions,
                                        HardwareSpecs = new PcHardwareSpecs
                                        {
                                            CpuModel = cpu,
                                            RamAmountGb = ram.Amount,
                                            RamType = ram.Type,
                                            StorageType = storage.Type,
                                            StorageAmountGb = storage.Amount
                                            
                                        }
                                    };
                                    var newBuild = new ProductsModel
                                    {
                                        ProductModel = itemName,
                                        DeviceType = DeviceType.PC,
                                        DeviceDetails = specs
                                    };

                                    var pcPrice = await GetPriceByAi(newBuild);
                                    if (pcPrice > 0)
                                    {
                                        newBuild.AveragePrice = pcPrice;
                                        allBuilds.Add(newBuild);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"[Skip] Не удалось получить цену для: {newBuild.ProductModel}. Пропускаем...");
                                    }
                                }
                            }
                        }
                        finalBuilds.AddRange(allBuilds);
                        tcs?.TrySetResult(true); 
                    }
                    catch (Exception ex)
                    {
                        tcs?.TrySetException(new Exception($"Фатальная ошибка парсинга JSON/AI: {ex.Message}"));
                    }
                }
            };
            page.Response += universalHandler;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var link = allCards.Nth(i).Locator("a").First;
                    var href = await link.GetAttributeAsync("href");
                    currentProductCode = href.Trim('/').Split('/').Last();

                    tcs = new TaskCompletionSource<bool>();

                    await link.ClickAsync();
                    await tcs.Task;
                    await page.GoBackAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });
                    await allCards.First.WaitForAsync();
                }
            }
            finally
            {
                page.Response -= universalHandler;
            }

            return finalBuilds;
        }

        private async Task<decimal> GetPriceByAi<T>(T device) where T : ProductsModel
        {
            try
            {
                var specsDict = new Dictionary<string, string>();
                if (device.DeviceDetails is ISpecificationProvider provider)
                {
                    specsDict = provider.GetSpecificationsForAi();
                }
                else
                {
                    specsDict.Add("Конфигурация", "Стандартная");
                }

                var prompt = _promtService.GetPricePrompt(
                    Site.ToString(),
                    device.ProductModel,
                    specsDict
                );

                var priceRaw = await _gigaChatAiService.AskQuestionAndGetAnswer(prompt);
                var cleanPrice = Regex.Replace(priceRaw, @"[^\d]", "");
                return decimal.TryParse(cleanPrice, out var p) ? p : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AI Error] {ex.Message}");
                return 0;
            }
        }
    }
}