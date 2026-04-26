using NetOptimizerParserApi.Interfaces;
using System.Text;
using UglyToad.PdfPig;

namespace NetOptimizerParserApi.Services
{
    public class PdfReaderService : IPdfReaderService
    {
        public string ExtractText(byte[] pdfBytes)
        {
            using var stream = new MemoryStream(pdfBytes);
            using var doc = PdfDocument.Open(stream);

            var sb = new StringBuilder();

            foreach (var page in doc.GetPages())
                sb.AppendLine(page.Text);

            return sb.ToString();
        }
    }
}
