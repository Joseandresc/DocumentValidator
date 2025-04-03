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
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsPath = System.IO.Path.Combine(userProfile, "Downloads");
            // Define the file name and path
            string fileName = Guid.NewGuid().ToString() + ".docx";
            string filePath = Path.Combine(downloadsPath, fileName);
            DocX document = await converter.StreamToDocxConverter(documentStream);

            // Load your document

            foreach (Table table in document.Tables)
            {
                Debug.WriteLine(table.AutoFit);
                table.AutoFit = AutoFit.Fixed;
                //Set column widths
                float[] columnWidths = new float[table.ColumnCount];
                if (table.ColumnCount <= 2)
                {
                    for (int i = 0; i < table.ColumnCount; i++)
                    {
                        columnWidths[i] = 260; 
                    }
                }
                else
                {
                    for (int i = 0; i < table.ColumnCount; i++)
                    {
                        columnWidths[i] = 130; 
                    }
                }
                table.Alignment = Alignment.center;
                table.SetWidths(columnWidths, true);
            }
            document.SaveAs(filePath);

        }

    }
}
