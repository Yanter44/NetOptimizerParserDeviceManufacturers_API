using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Playwright;
using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Extensions;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.DeviceDetails;
using NetOptimizerParserApi.Models.Enums;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;


namespace NetOptimizerParserApi.Services.External
{
    public class EltexParserService : BasePlaywrightParser<ProductsModel, ParserOptions>, IParserStrategy
    {
        private readonly IGigaChatAiService _gigaChatAiService;
        private readonly IPromtService _promtService;

        public SitesToParse Site => SitesToParse.Eltex;

        public EltexParserService(IGigaChatAiService gigachatAiService, IPromtService promtService)
        {
            _gigaChatAiService = gigachatAiService;
            _promtService = promtService;
        }

        public Task<ServiceResponse<List<ProductsModel>>> ParseAsync(string url, ParserOptions options, CancellationToken cancellationToken)
        {
           var result = this.ExecuteAsync(url, options, cancellationToken);
           return result;
        }

        protected async override Task<ServiceResponse<List<ProductsModel>>> ParsePageAsync(IPage page, ParserOptions options, CancellationToken cancellationToken)
        {
            List<ProductsModel> data = options.ParsedDevices switch
            {
                ParseDevice.Switches => await ParseCommutators(page, cancellationToken),
                ParseDevice.Routers => await ParseRouters(page, cancellationToken),
                _ => null
            };

            if (data == null)
                return new ServiceResponse<List<ProductsModel>> { Success = false, Message = $"Тип устройства {options.ParsedDevices} не поддерживается для {Site}"};
            
            return new ServiceResponse<List<ProductsModel>> { Data = data, Success = true, Message = $"Успешно спаршено. Найдено: {data.Count} шт."};
        }

        public async Task<List<ProductsModel>> ParseRouters(IPage page, CancellationToken cancellationToken)
        {
            var parsedProducts = new List<ProductsModel>(); 
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // 1. Навигация (без изменений)
            var catalogButton = page.Locator("li.menu__item.menu__item--dropdown").GetByRole(AriaRole.Button, new() { Name = "Каталог" });
            var chapterButton = page.GetByRole(AriaRole.Button, new() { Name = "Маршрутизаторы ESR", Exact = true });
            var accessRoutersLink = page.GetByRole(AriaRole.Link, new() { Name = "Сервисные маршрутизаторы", Exact = true });

            await catalogButton.ClickWithRetryAsync(chapterButton);
            await chapterButton.ClickAsync();
            await accessRoutersLink.ClickAsync();

            var linkLocator = page.Locator("div.good.equipment__column a");
            await linkLocator.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

            var allcards = page.Locator("div.good.equipment__column");
            int count = await allcards.CountAsync();

            TaskCompletionSource<bool> tcs = null;
            string currentProductCode = "";

            EventHandler<IResponse> universalHandler = async (sender, response) =>
            {
                if (!string.IsNullOrEmpty(currentProductCode) &&
                    response.Url.Contains($"/api/catalog/good/{currentProductCode}"))
                {
                    try
                    {
                        var jsonText = await response.TextAsync();
                        var innerJson = JObject.Parse(jsonText);
                        var data = innerJson["data"];

                        if (data != null)
                        {
                            var routerDetails = new RouterProductDetailsModel { IsManaged = true };

                            var itemName = data["name"]?.ToString() ?? "";
                            const string prefix = "Сервисный маршрутизатор ";

                            if (itemName.StartsWith(prefix))
                            {
                                itemName = itemName[prefix.Length..];
                            }

                            if (itemName.Contains("Виртуальный", StringComparison.OrdinalIgnoreCase))
                            {
                                tcs?.TrySetResult(true);
                                return;
                            }

                            var product = new ProductsModel
                            {
                                ProductModel = itemName,
                                DeviceType = DeviceType.Router,
                                DeviceDetails = routerDetails 
                            };

                            var properties = data["properties"];
                            if (properties != null)
                            {
                                foreach (var group in properties)
                                {
                                    string groupName = group["name"]?.ToString() ?? "";
                                    var propData = group["data"];
                                    if (propData == null) continue;

                                    foreach (var p in propData)
                                    {
                                        string pName = p["name"]?.ToString()?.ToLower() ?? "";
                                        string pVal = p["value"]?.ToString() ?? "";

                                        if (groupName.Contains("Интерфейсы"))
                                        {
                                            var parsedPort = ParsePort(pName, pVal);
                                            if (parsedPort != null) routerDetails.Ports.Add(parsedPort);
                                        }
                                        else if (groupName.Contains("Производительность"))
                                        {
                                            double val = ExtractNumber(pVal);
                                            if (pName.Contains("firewall") && pName.Contains("1518b"))
                                                routerDetails.Performance.RoutingThroughputGbps = val;
                                        }
                                        else if (groupName.Contains("Физические характеристики"))
                                        {
                                            if (pName == "ram") routerDetails.Performance.RamMb = (int)ExtractNumber(pVal) * 1024;
                                            if (pName.Contains("flash")) routerDetails.Performance.FlashMb = (int)ExtractNumber(pVal) * 1024;
                                        }
                                    }
                                }
                            }

                            var attributes = data["base_attributes"];
                            if (attributes != null)
                            {
                                foreach (var attr in attributes)
                                {
                                    string val = attr["value"]?.ToString()?.ToUpper() ?? "";
                                    if (val.Contains("OSPF")) routerDetails.ProtocolSupport.SupportsOspf = true;
                                    if (val.Contains("VRRP")) routerDetails.ProtocolSupport.SupportsVrrp = true;
                                    if (val.Contains("IPSEC")) routerDetails.ProtocolSupport.SupportsIpsec = true;
                                    if (val.Contains("NAT") || val.Contains("FIREWALL")) routerDetails.ProtocolSupport.SupportsNat = true;
                                }
                            }
                            product.AveragePrice = await GetPriceByAi(product);
                            parsedProducts.Add(product);
                        }
                        tcs?.TrySetResult(true);
                    }
                    catch { tcs?.TrySetResult(false); }
                }
            };

            page.Response += universalHandler;

            for (int i = 0; i < count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine("Парсим карточку роутера " + i);
                var currentCard = allcards.Nth(i);
                var linkElement = currentCard.Locator("a").First;

                string href = await linkElement.GetAttributeAsync("href");
                currentProductCode = href.Trim('/').Split('/').Last();

                tcs = new TaskCompletionSource<bool>();
                await linkElement.ClickAsync();
                await tcs.Task;
                await Task.WhenAny(tcs.Task, Task.Delay(8000));
                await page.GoBackAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });
                await allcards.First.WaitForAsync();
            }
            page.Response -= universalHandler;
            return parsedProducts;
        }
        double ExtractNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            var match = Regex.Match(input.Replace(',', '.'), @"[0-9]+(\.[0-9]+)?");
            return match.Success ? double.Parse(match.Value, System.Globalization.CultureInfo.InvariantCulture) : 0;
        }

        public async Task<List<ProductsModel>> ParseCommutators(IPage page, CancellationToken cancellationToken)
        {
            var parsedProducts = new List<ProductsModel>();
            // 1. Переход в каталог
            var catalogButton = page.Locator("li.menu__item.menu__item--dropdown")
                                    .GetByRole(AriaRole.Button, new() { Name = "Каталог" });

            // 2. Переход в подкаталог "Коммутаторы доступа"
            var accessSwitchesLink = page.GetByRole(AriaRole.Link, new() { Name = "Коммутаторы доступа", Exact = true });

            await catalogButton.ClickWithRetryAsync(accessSwitchesLink);
            await Task.WhenAll(
                page.WaitForURLAsync(url => url.Contains("ethernet-kommutatory_dostupa")),
                accessSwitchesLink.ClickAsync()
            );

            var linkLocator = page.Locator("div.good.equipment__column a");
            await linkLocator.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

            var allcards = page.Locator("div.good.equipment__column");
            int count = await allcards.CountAsync();

            TaskCompletionSource<bool> tcs = null;
            string currentProductCode = "";

            EventHandler<IResponse> universalHandler = async (sender, response) =>
            {
                if (!string.IsNullOrEmpty(currentProductCode) &&
                    response.Url.Contains($"/api/catalog/good/{currentProductCode}"))
                {
                    try
                    {
                        var jsonText = await response.TextAsync();
                        var innerJson = JObject.Parse(jsonText);
                        var data = innerJson["data"];

                        if (data != null)
                        {
                            var switchDetails = new CommutatorProductDetailsModel();
                            switchDetails.IsManaged = true;

                            var itemName = data["name"]?.ToString() ?? "";
                            const string prefix = "Коммутатор доступа ";

                            if (itemName.StartsWith(prefix))
                            {
                                itemName = itemName[prefix.Length..];
                            }
                            var product = new ProductsModel
                            {
                                ProductModel = itemName,
                                DeviceType = DeviceType.Switch,
                                DeviceDetails = switchDetails 
                            };

                            var properties = data["properties"];
                            var attributes = data["attributes"];
                            if (attributes != null)
                            {
                                foreach (var attribute in attributes)
                                {
                                    var attributeName = attribute["attribute_name"]?.ToString();
                                    var attributeValue = attribute["text_value"]?.ToString();

                                    if (attributeName != null && attributeName.Contains("Уровень коммутатора"))
                                    {
                                        var switchLayer = attributeValue.Split("L");
                                        if (switchLayer.Length > 1 && int.TryParse(switchLayer[1], out int layer))
                                        {
                                            switchDetails.Layer = layer;
                                        }
                                        break;
                                    }
                                }
                            }
                            if (properties != null)
                            {
                                foreach (var group in properties)
                                {
                                    string groupName = group["name"]?.ToString() ?? "";
                                    var propData = group["data"];
                                    if (propData == null) continue;

                                    foreach (var p in propData)
                                    {
                                        string pName = p["name"]?.ToString()?.ToLower() ?? "";
                                        string pVal = p["value"]?.ToString() ?? "";

                                        if (groupName.Contains("Интерфейсы"))
                                        {
                                            var parsedPort = ParsePort(pName, pVal);
                                            if (parsedPort != null)
                                            {
                                                switchDetails.Ports.Add(parsedPort);
                                            }
                                        }
                                        if (groupName.Contains("Производительность"))
                                        {
                                            var (throughput, macSize, vlanCount) = ParsePerformanceProperties(propData);
                                            switchDetails.ThroughputGbps = throughput;
                                            switchDetails.MacTableSize = macSize;
                                            switchDetails.MaxVlans = vlanCount;
                                        }
                                    }
                                }

                                product.AveragePrice = await GetPriceByAi(product);
                                parsedProducts.Add(product);
                            }
                            tcs?.TrySetResult(true);
                        }
                    }
                    catch { tcs?.TrySetResult(false); }
                }
            };
            page.Response += universalHandler;
            for (int i = 0; i < count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine("Парсим карточку " + i);

                var currentCard = allcards.Nth(i);
                var linkElement = currentCard.Locator("a").First;

                string href = await linkElement.GetAttributeAsync("href");
                currentProductCode = href.Trim('/').Split('/').Last();

                tcs = new TaskCompletionSource<bool>();
                await linkElement.ClickAsync();

                await Task.WhenAny(tcs.Task, Task.Delay(8000));

                await page.GoBackAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });
                await allcards.First.WaitForAsync();
            }
            page.Response -= universalHandler;
            return parsedProducts;
        }
        private async Task<decimal> GetPriceByAi<T>(T device) where T : ProductsModel
        {
            try
            {
                Dictionary<string, string> specsDict = new();
                if (device.DeviceDetails is ISpecificationProvider provider)
                {
                    specsDict = provider.GetSpecificationsForAi();
                }
                var prompt = _promtService.GetPricePrompt(
                    Site.ToString(),
                    device.ProductModel,
                    specsDict
                );
                var priceRaw = await _gigaChatAiService.AskQuestionAndGetAnswer(prompt);
                var cleanedPrice = Regex.Replace(priceRaw, @"[^\d]", "");
                return decimal.TryParse(cleanedPrice, out var p) ? p : 0;
            }
            catch { return 0; }
        }
        Port ParsePort(string portName, string portValue)
        {
            int count = 0;
            int.TryParse(portValue, out count);

            string lowerName = portName.ToLower();

            PortType type;
            string speedStr = null;

            if (lowerName.Contains("rs-232") || lowerName.Contains("консоль"))
            {
                type = PortType.Console;
                speedStr = "Console";
            }
            else if (lowerName.Contains("sfp+"))
            {
                type = PortType.SFPPlus;
            }
            else if (lowerName.Contains("sfp") || lowerName.Contains("base-x"))
            {
                type = PortType.SFP;
            }
            else if (lowerName.Contains("qsfp"))
            {
                type = PortType.QSFP;
            }
            else if (lowerName.Contains("combo"))
            {
                type = PortType.Combo;
            }
            else if (lowerName.Contains("rj-45"))
            {
                type = PortType.RJ45;
            }
            else
            {
                return null;
            }

            // --- Скорость только для сетевых портов ---
            if (type == PortType.RJ45 || type == PortType.SFP || type == PortType.SFPPlus || type == PortType.QSFP || type == PortType.Combo)
            {
                var matches = Regex.Matches(lowerName, @"(\d+)([mg]?)base", RegexOptions.IgnoreCase);
                int maxSpeed = 0;
                speedStr = "Unknown";

                foreach (Match m in matches)
                {
                    int val = int.Parse(m.Groups[1].Value);
                    string unit = m.Groups[2].Value.ToUpper();
                    int valInM = (unit == "G") ? val * 1000 : val;

                    if (valInM > maxSpeed) maxSpeed = valInM;
                }

                if (maxSpeed >= 1000)
                    speedStr = (maxSpeed / 1000) + "G";
                else if (maxSpeed > 0)
                    speedStr = maxSpeed + "M";
            }

            return new Port
            {
                Count = count,
                Type = type,
                Speed = speedStr
            };
        }
        private (decimal ThroughputGbps, int MacTableSize, int MaxVlans) ParsePerformanceProperties(JToken propData)
        {
            decimal throughput = 0;
            int macTableSize = 0;
            int maxVlans = 0;

            foreach (var prop in propData)
            {
                string propertyName = prop["name"]?.ToString() ?? "";
                string propertyVal = prop["value"]?.ToString() ?? "";

                switch (propertyName)
                {
                    case "Пропускная способность":
                        if (!string.IsNullOrWhiteSpace(propertyVal))
                        {
                            var clean = propertyVal.Replace("Гбит/с", "").Trim(); //похуй на эти мегабайты, таких коммутаторов нет блятЬ, а писать на русском - идите нахуй
                            if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"), out var val))
                                throughput = val;
                        }
                        break;

                    case "Таблица MAC-адресов":
                        if (int.TryParse(propertyVal.Replace(" ", ""), out int macSize))
                            macTableSize = macSize;
                        break;

                    case "Таблица VLAN":
                        if (int.TryParse(propertyVal.Replace(" ", ""), out int vlanCount))
                            maxVlans = vlanCount;
                        break;
                }
            }
            return (throughput, macTableSize, maxVlans);
        }

      
    }
}
