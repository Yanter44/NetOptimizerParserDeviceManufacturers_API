using Microsoft.Playwright;
using NetOptimizerParserApi.Interfaces;
using NetOptimizerParserApi.Models;

namespace NetOptimizerParserApi.Services.External
{
    public abstract class BasePlaywrightParser<TProduct, TOptions> where TProduct : ProductsModel
    {
        public async Task<List<TProduct>> ExecuteAsync(string url, TOptions options, CancellationToken cancellationToken)
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = false,
                Args = new[] { "--disable-blink-features=AutomationControlled" }
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            try
            {
                await page.GotoAsync(url, new()
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded
                });
                var result = await ParsePageAsync(page, options, cancellationToken);
                return result;
            }
            finally
            {
                await context.CloseAsync();
                await browser.CloseAsync();
            }
        }
        protected abstract Task<List<TProduct>> ParsePageAsync(IPage page, TOptions options, CancellationToken cancellationToken);
    }
}
