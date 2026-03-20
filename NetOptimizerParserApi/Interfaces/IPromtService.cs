namespace NetOptimizerParserApi.Interfaces
{
    public interface IPromtService
    {
        string GetPricePrompt(string vendor, string model, Dictionary<string, string> specs);
    }
}
