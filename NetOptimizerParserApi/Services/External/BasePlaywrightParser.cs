using Microsoft.Playwright;
using NetOptimizerParserApi.Common;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;

namespace NetOptimizerParserApi.Services.External
{
    public abstract class BasePlaywrightParser<TProduct, TOptions> where TProduct : ProductsModel
    {
        public async Task<ServiceResponse<List<TProduct>>> ExecuteAsync(string url, TOptions options, CancellationToken cancellationToken)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = false,
                Args = new[] { "--disable-blink-features=AutomationControlled" }
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            try
            {
                var response = await page.GotoAsync(url, new()
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 60000
                });

                if (response == null || !response.Ok)
                {
                    return new ServiceResponse<List<TProduct>>
                    {
                        Success = false,
                        Message = $"Не удалось загрузить страницу: {url}. Статус: {response?.Status}"
                    };
                }
                var data = await ParsePageAsync(page, options, cancellationToken);
                return data;
            }
            catch (OperationCanceledException)
            {
                return new ServiceResponse<List<TProduct>> { Success = false, Message = "Операция была отменена пользователем." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<TProduct>> { Success = false, Message = $"Ошибка при работе Playwright: {ex.Message}" };
            }
            finally
            {
                await context.CloseAsync();
                await browser.CloseAsync();
            }
        }
        protected abstract Task<ServiceResponse<List<TProduct>>> ParsePageAsync(IPage page, TOptions options, CancellationToken cancellationToken);
    }
}
