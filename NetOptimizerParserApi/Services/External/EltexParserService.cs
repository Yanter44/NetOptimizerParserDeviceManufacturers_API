using Microsoft.Playwright;
using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Extensions;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;
using NetOptimizerParserApi.Models.Components;
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
        private readonly INetworkService _networkService;
        private readonly IPdfReaderService _pdfReaderService;
        public SitesToParse Site => SitesToParse.Eltex;
        public EltexParserService(
            IGigaChatAiService gigachatAiService, 
            IPromtService promtService, 
            INetworkService networkService, IPdfReaderService pdfreaderService)
        {
            _gigaChatAiService = gigachatAiService;
            _promtService = promtService;
            _networkService = networkService;
            _pdfReaderService = pdfreaderService;
        }

        public Task<ServiceResponse<List<ProductsModel>>> ParseAsync(string url, ParserOptions options, CancellationToken cancellationToken)
        {
            return ExecuteAsync(url, options, cancellationToken);
        }

        protected override async Task<ServiceResponse<List<ProductsModel>>> ParsePageAsync(IPage page, ParserOptions options, CancellationToken cancellationToken)
        {
            var data = options.ParsedDevices switch
            {
                ParseDevice.Switches => await ParseCommutators(page, cancellationToken),
                ParseDevice.Routers => await ParseRouters(page, cancellationToken),
                _ => null
            };

            if (data == null)
                return new ServiceResponse<List<ProductsModel>>
                {
                    Success = false,
                    Message = $"Тип устройства {options.ParsedDevices} не поддерживается для {Site}"
                };

            return new ServiceResponse<List<ProductsModel>>
            {
                Data = data,
                Success = true,
                Message = $"Успешно спаршено. Найдено: {data.Count} шт."
            };
        }

        #region ROUTERS

        public async Task<List<ProductsModel>> ParseRouters(IPage page, CancellationToken ct)
        {
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await NavigateToRouters(page);

            var cards = await GetCards(page);
            var result = new List<ProductsModel>();

            foreach (var card in cards)
            {
                ct.ThrowIfCancellationRequested();

                var product = await ParseRouterCardAsync(page, card);

                if (product != null)
                    result.Add(product);

                await page.GoBackAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });
            }

            return result;
        }

        private async Task NavigateToRouters(IPage page)
        {
            var catalogButton = page.Locator("li.menu__item.menu__item--dropdown")
                .GetByRole(AriaRole.Button, new() { Name = "Каталог" });

            var chapterButton = page.GetByRole(AriaRole.Button, new() { Name = "Маршрутизаторы ESR", Exact = true });
            var accessRoutersLink = page.GetByRole(AriaRole.Link, new() { Name = "Сервисные маршрутизаторы", Exact = true });

            await catalogButton.ClickWithRetryAsync(chapterButton);
            await chapterButton.ClickAsync();
            await accessRoutersLink.ClickAsync();
        }

        private async Task<ProductsModel?> ParseRouterCardAsync(IPage page, ILocator card)
        {
            var link = card.Locator("a").First;

            var href = await link.GetAttributeAsync("href");
            var code = href.Trim('/').Split('/').Last();

            var response = await page.RunAndWaitForResponseAsync(
                async () => await link.ClickAsync(),
                r => r.Url.Contains($"/api/catalog/good/{code}")
            );

            var json = JObject.Parse(await response.TextAsync());

            return await BuildRouter(json["data"]);
        }

        private async Task<ProductsModel?> BuildRouter(JToken data)
        {
            if (data == null) return null;

            var name = NormalizeName(data["name"]?.ToString(), "Сервисный маршрутизатор ");

            if (name.Contains("Виртуальный", StringComparison.OrdinalIgnoreCase))
                return null;

            var details = new RouterProductDetailsModel { IsManaged = true };

            ParseRouterProperties(details, data["properties"]);
            ParseRouterAttributes(details, data["base_attributes"]);

            var product = new ProductsModel
            {
                ProductModel = name,
                DeviceType = DeviceType.Router,
                DeviceDetails = details
            };

            product.AveragePrice = await GetPriceByAi(product);

            return product;
        }

        private void ParseRouterProperties(RouterProductDetailsModel d, JToken props)
        {
            if (props == null) return;

            foreach (var group in props)
            {
                string groupName = group["name"]?.ToString() ?? "";
                var data = group["data"];
                if (data == null) continue;

                foreach (var p in data)
                {
                    string name = p["name"]?.ToString()?.ToLower() ?? "";
                    string val = p["value"]?.ToString() ?? "";

                    if (groupName.Contains("Интерфейсы"))
                    {
                        var port = ParsePort(name, val);
                        if (port != null) d.Ports.Add(port);
                    }
                    else if (groupName.Contains("Производительность"))
                    {
                        if (name.Contains("firewall") && name.Contains("1518b"))
                            d.Performance.RoutingThroughputGbps = ExtractNumber(val);
                    }
                    else if (groupName.Contains("Физические характеристики"))
                    {
                        if (name == "ram")
                            d.Performance.RamMb = (int)ExtractNumber(val) * 1024;

                        if (name.Contains("flash"))
                            d.Performance.FlashMb = (int)ExtractNumber(val) * 1024;
                    }
                }
            }
        }

        private void ParseRouterAttributes(RouterProductDetailsModel d, JToken attrs)
        {
            if (attrs == null) return;

            foreach (var a in attrs)
            {
                var val = a["value"]?.ToString()?.ToUpper() ?? "";

                if (val.Contains("OSPF")) d.ProtocolSupport.SupportsOspf = true;
                if (val.Contains("VRRP")) d.ProtocolSupport.SupportsVrrp = true;
                if (val.Contains("IPSEC")) d.ProtocolSupport.SupportsIpsec = true;
                if (val.Contains("NAT") || val.Contains("FIREWALL"))
                    d.ProtocolSupport.SupportsNat = true;
            }
        }

        #endregion

        #region SWITCHES
        public async Task<List<ProductsModel>> ParseCommutators(IPage page, CancellationToken ct)
        {
            await NavigateToSwitches(page);

            var cards = await GetCards(page);
            var result = new List<ProductsModel>();

            foreach (var card in cards)
            {
                ct.ThrowIfCancellationRequested();

                var product = await ParseSwitchCardAsync(page, card);

                if (product != null)
                    result.Add(product);

                await page.GoBackAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });
            }
            return result;
        }

        private async Task NavigateToSwitches(IPage page)
        {
            var catalogButton = page.Locator("li.menu__item.menu__item--dropdown")
                .GetByRole(AriaRole.Button, new() { Name = "Каталог" });

            var link = page.GetByRole(AriaRole.Link, new() { Name = "Коммутаторы доступа", Exact = true });

            await catalogButton.ClickWithRetryAsync(link);

            await Task.WhenAll(
                page.WaitForURLAsync(u => u.Contains("ethernet-kommutatory_dostupa")),
                link.ClickAsync()
            );
        }

        private async Task<ProductsModel?> ParseSwitchCardAsync(IPage page, ILocator card)
        {
            var link = card.Locator("a").First;

            var href = await link.GetAttributeAsync("href");
            var code = href.Trim('/').Split('/').Last();

            var response = await page.RunAndWaitForResponseAsync(
                async () => await link.ClickAsync(),
                r => r.Url.Contains($"/api/catalog/good/{code}")
            );

            var json = JObject.Parse(await response.TextAsync());

            var product = await BuildSwitch(json["data"]);

            var datasheetUrl = await GetDatasheetUrl(page);
            var resultBytes = await _networkService.DownloadFileAsync(datasheetUrl, default);
            var pdfText =  _pdfReaderService.ExtractText(resultBytes);
            SwitchProtocolSupport supportedProtocols = CheckSupportedSwitchProtocols(pdfText);
            var switchProductDetails = product.DeviceDetails as CommutatorProductDetailsModel;
            switchProductDetails.ProtocolSupport = supportedProtocols;
            return product;
        }
        private SwitchProtocolSupport CheckSupportedSwitchProtocols(string text)
        {
            text = text.ToLowerInvariant();

            var supportsLacp = text.Contains("объединение каналов с использованием lacp");
            var supportsLag = text.Contains("создание групп lag") || supportsLacp;

            var protocolSupportModel = new SwitchProtocolSupport
            {
                SupportsLag = supportsLag,
                SupportsLacp = supportsLacp,
                SupportsLoopProtection = text.Contains("поддержка stp"),
            };
            return protocolSupportModel;
        }
        private async Task<string?> GetDatasheetUrl(IPage page)
        {
            var link = page.Locator("a.column-body[href*='pdf']").First;

            if (await link.CountAsync() == 0)
                return null;

            var href = await link.GetAttributeAsync("href");

            return string.IsNullOrWhiteSpace(href) ? null : href;
        }
        private async Task<ProductsModel?> BuildSwitch(JToken data)
        {
            if (data == null) return null;

            var name = NormalizeName(data["name"]?.ToString(), "Коммутатор доступа ");

            var details = new CommutatorProductDetailsModel
            {
                IsManaged = true,
                SwitchRoleType = SwitchRoleType.Access
            };

            ParseSwitchAttributes(details, data["attributes"]);
            ParseSwitchProperties(details, data["properties"]);
         
            var product = new ProductsModel
            {
                ProductModel = name,
                DeviceType = DeviceType.Switch,
                DeviceDetails = details
            };
            product.AveragePrice = await GetPriceByAi(product);
            return product;
        }
        private void ParseSwitchProtocolSupport()
        {

        }

        private void ParseSwitchAttributes(CommutatorProductDetailsModel d, JToken attrs)
        {
            if (attrs == null) return;

            foreach (var a in attrs)
            {
                var name = a["attribute_name"]?.ToString();
                var val = a["text_value"]?.ToString();

                if (name?.Contains("Уровень коммутатора") == true)
                {
                    var parts = val.Split("L");
                    if (parts.Length > 1 && int.TryParse(parts[1], out int layer))
                        d.Layer = layer;

                    break;
                }
            }
        }

        private void ParseSwitchProperties(CommutatorProductDetailsModel d, JToken props)
        {
            if (props == null) return;

            foreach (var group in props)
            {
                string groupName = group["name"]?.ToString() ?? "";
                var data = group["data"];
                if (data == null) continue;

                if (groupName.Contains("Производительность"))
                {
                    var (t, mac, vlan) = ParsePerformanceProperties(data);
                    d.PerformanceSpecs.ThroughputGbps = t;
                    d.PerformanceSpecs.MacTableSize = mac;
                    d.PerformanceSpecs.MaxVlans = vlan;
                }

                foreach (var p in data)
                {
                    string name = p["name"]?.ToString()?.ToLower() ?? "";
                    string val = p["value"]?.ToString() ?? "";

                    if (groupName.Contains("Интерфейсы"))
                    {
                        var port = ParsePort(name, val);
                        if (port != null) d.Ports.Add(port);
                    }
                }
            }
        }

        #endregion

        #region COMMON

        private async Task<List<ILocator>> GetCards(IPage page)
        {
            var locator = page.Locator("div.good.equipment__column");

            await locator.First.WaitForAsync();

            int count = await locator.CountAsync();

            var list = new List<ILocator>();

            for (int i = 0; i < count; i++)
                list.Add(locator.Nth(i));

            return list;
        }

        private string NormalizeName(string name, string prefix)
        {
            if (string.IsNullOrEmpty(name)) return "";

            return name.StartsWith(prefix)
                ? name[prefix.Length..]
                : name;
        }

        private double ExtractNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;

            var match = Regex.Match(input.Replace(',', '.'), @"[0-9]+(\.[0-9]+)?");

            return match.Success
                ? double.Parse(match.Value, CultureInfo.InvariantCulture)
                : 0;
        }

        private async Task<decimal> GetPriceByAi<T>(T device) where T : ProductsModel
        {
            try
            {
                Dictionary<string, string> specs = new();

                if (device.DeviceDetails is ISpecificationProvider p)
                    specs = p.GetSpecificationsForAi();

                var prompt = _promtService.GetPricePrompt(
                    Site.ToString(),
                    device.ProductModel,
                    specs
                );

                var raw = await _gigaChatAiService.AskQuestionAndGetAnswer(prompt);

                var cleaned = Regex.Replace(raw, @"[^\d]", "");

                return decimal.TryParse(cleaned, out var price) ? price : 0;
            }
            catch
            {
                return 0;
            }
        }

        private Port ParsePort(string name, string value)
        {
            int.TryParse(value, out int count);

            var lower = name.ToLower();

            PortType type;
            string speed = null;

            if (lower.Contains("rs-232") || lower.Contains("консоль"))
            {
                type = PortType.Console;
                speed = "Console";
            }
            else if (lower.Contains("sfp+")) type = PortType.SFPPlus;
            else if (lower.Contains("sfp")) type = PortType.SFP;
            else if (lower.Contains("qsfp")) type = PortType.QSFP;
            else if (lower.Contains("combo")) type = PortType.Combo;
            else if (lower.Contains("rj-45")) type = PortType.RJ45;
            else return null;

            if (type != PortType.Console)
            {
                var matches = Regex.Matches(lower, @"(\d+)([mg]?)base");
                int max = 0;

                foreach (Match m in matches)
                {
                    int val = int.Parse(m.Groups[1].Value);
                    string unit = m.Groups[2].Value.ToUpper();
                    int valM = unit == "G" ? val * 1000 : val;

                    if (valM > max) max = valM;
                }

                speed = max >= 1000 ? (max / 1000) + "G" :
                        max > 0 ? max + "M" : "Unknown";
            }

            return new Port { Count = count, Type = type, Speed = speed };
        }

        private (decimal, int, int) ParsePerformanceProperties(JToken data)
        {
            decimal t = 0;
            int mac = 0;
            int vlan = 0;

            foreach (var p in data)
            {
                string name = p["name"]?.ToString() ?? "";
                string val = p["value"]?.ToString() ?? "";

                switch (name)
                {
                    case "Пропускная способность":
                        var clean = val.Replace("Гбит/с", "").Trim();
                        decimal.TryParse(clean, NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"), out t);
                        break;

                    case "Таблица MAC-адресов":
                        int.TryParse(val.Replace(" ", ""), out mac);
                        break;

                    case "Таблица VLAN":
                        int.TryParse(val.Replace(" ", ""), out vlan);
                        break;
                }
            }

            return (t, mac, vlan);
        }

        #endregion
    }
}