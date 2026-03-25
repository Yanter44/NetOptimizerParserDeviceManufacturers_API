using Microsoft.Playwright;

namespace NetOptimizerParserApi.Extensions
{
    public static class PlaywrightExtensions
    {
        public static async Task ClickWithRetryAsync(this ILocator locator, ILocator targetIndicator, int maxAttempts = 8)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                await locator.ClickAsync();
                try
                {
                    await targetIndicator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 2000 });
                    return;
                }
                catch { if (i == maxAttempts - 1) throw; }
                await Task.Delay(2000);
            }
        }
    }
}
