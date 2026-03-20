namespace NetOptimizerParserApi.Interfaces
{
    public interface IGigaChatAiService
    {
        Task<string> AskQuestionAndGetAnswer(string question);
    }
}
