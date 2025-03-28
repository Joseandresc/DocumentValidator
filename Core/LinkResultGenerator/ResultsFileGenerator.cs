using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentValidator.Core.DocumentProcessor;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DocumentValidator.Core.ResultGenerator
{
    internal class ResultsFileGenerator
    {


        public async Task GenerateWorkbookAsync(List<LinkValidationResult> results)
        {
            // Retrieve the Downloads folder path
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsPath = System.IO.Path.Combine(userProfile, "Downloads");
           // string downloadsPath = @"C:\temp";

            // Define the file name and path
            string fileName = Guid.NewGuid().ToString()+".xlsx";
            string filePath = Path.Combine(downloadsPath, fileName);
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                // Add a WorkbookPart to the document.
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                // Create a basic stylesheet (fonts, fills, borders, and cell formats)
                stylesPart.Stylesheet = new Stylesheet(
                    new Fonts(new DocumentFormat.OpenXml.Spreadsheet.Font()),
                    new Fills(
                        new Fill(new PatternFill() { PatternType = PatternValues.None }),
                        new Fill(new PatternFill() { PatternType = PatternValues.Gray125 })
                    ),
                    new Borders(new DocumentFormat.OpenXml.Spreadsheet.Border()),
                    new CellFormats(new CellFormat()) // default cell format
                );
                stylesPart.Stylesheet.Save();

                // Get the style index for our red style.
                uint redStyleIndex = SheetStyleGenerator.EnsureRedStyle(stylesPart);

                // Append the WorksheetPart to the Workbook.
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                // Create a new sheet and append it to the sheets collection
                Sheet sheet = new Sheet()
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Validation Results"
                };
                sheets.Append(sheet);
                // Create the header row
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                Row headerRow = new Row();
                headerRow.Append(
                    new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue("URL"), DataType = CellValues.String },
                    new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue("LinkText"), DataType = CellValues.String },
                    new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue("Is Valid"), DataType = CellValues.String },
                    new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue("Status Code"), DataType = CellValues.String },
                    new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue("Error Message"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow);
                // Populate the data rows
                foreach (var result in results)
                {
                    
                    Row dataRow = new Row();
                   
                    dataRow.Append(
                        new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue(result.Url), DataType = CellValues.String },
                        new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue(result.LinkText), DataType = CellValues.String },
                        new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue(result.IsValid.ToString()), DataType = CellValues.Boolean },
                        new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue(result.StatusCode.ToString()), DataType = CellValues.Number },
                        new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellValue = new CellValue(result.ErrorMessage), DataType = CellValues.String }
                    );
                    if (result.IsValid == false)
                    {
                        foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in dataRow.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
                        {
                            cell.StyleIndex = redStyleIndex;
                        }
                    }
                    sheetData.Append(dataRow);
                }
                // Save and close the document
                workbookPart.Workbook.Save();
                OpenExcelFile(filePath); 

            }

        }


       

        public void OpenExcelFile(string filePath)
    {
        var process = new Process();
        process.StartInfo.FileName = filePath;
        process.StartInfo.UseShellExecute = true; // This is important in .NET Core/MAUI
        process.Start();
    }


}
}
