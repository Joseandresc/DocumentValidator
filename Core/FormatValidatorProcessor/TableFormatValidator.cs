using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentValidator.Utils;
using System.Diagnostics;
using Xceed.Document.NET;
using Xceed.Words.NET;


namespace DocumentValidator.Core.FormatValidatorProcessor
{
    public class TableFormatValidator
    {
        StreamToDocx converter = new StreamToDocx();

        public async Task TableFormatValidation(Stream documentStream)
        {
            DocX document = await converter.StreamToDocxConverter(documentStream);

            // Load your document

            foreach (Table table in document.Tables)
            {
                Debug.WriteLine(table.AutoFit);
                // Set column widths
                float[] columnWidths = new float[table.ColumnCount];
                for (int i = 0; i < table.ColumnCount; i++)
                {
                    columnWidths[i] = 100; // Set each column width to 100 points
                }
                table.SetWidths(columnWidths, true);

                // Set row heights
                foreach (Row row in table.Rows)
                {
                    row.Height = 20; // Set row height to 20 points
                }
            }
        }

    }
}
