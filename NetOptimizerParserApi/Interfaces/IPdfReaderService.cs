namespace NetOptimizerParserApi.Interfaces
{
    public interface IPdfReaderService
    {
        string ExtractText(byte[] pdfBytes);
    }
}
