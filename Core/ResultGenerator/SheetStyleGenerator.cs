using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Xceed.Document.NET;

namespace DocumentValidator.Core.ResultGenerator
{
    public static class SheetStyleGenerator
    {
        /// <summary>
        /// Ensures that the workbook has a custom style with a red background.
        /// Returns the style index for that custom style.
        /// </summary>
        public static uint EnsureRedStyle(WorkbookStylesPart stylesPart)
        {
            // Create a new Stylesheet if it doesn't exist
            if (stylesPart.Stylesheet == null)
            {
                stylesPart.Stylesheet = new Stylesheet();
                stylesPart.Stylesheet.Append(new Fonts(new DocumentFormat.OpenXml.Spreadsheet.Font()), new Fills(new Fill(new PatternFill() { PatternType = PatternValues.None }),
                                                                                  new Fill(new PatternFill() { PatternType = PatternValues.Gray125 })),
                                                  new DocumentFormat.OpenXml.Spreadsheet.Borders(new DocumentFormat.OpenXml.Spreadsheet.Border()),
                                                  new CellFormats(new CellFormat()));
            }

            // Get the current stylesheet elements.
            Fonts fonts = stylesPart.Stylesheet.Fonts;
            Fills fills = stylesPart.Stylesheet.Fills;
            DocumentFormat.OpenXml.Spreadsheet.Borders borders = stylesPart.Stylesheet.Borders;
            CellFormats cellFormats = stylesPart.Stylesheet.CellFormats;

            // Create a new fill for red if not already present.
            // We assume that if a fill with our red color exists, its ForegroundColor.Rgb equals "FFFF0000"
            uint redFillIndex = 0;
            var redFill = fills.Elements<Fill>()
                               .FirstOrDefault(f =>
                               {
                                   var pf = f.PatternFill;
                                   return pf != null && pf.PatternType == PatternValues.Solid &&
                                          pf.ForegroundColor != null && pf.ForegroundColor.Rgb?.Value == "FFFF0000";
                               });

            if (redFill != null)
            {
                redFillIndex = (uint)fills.Elements<Fill>().ToList().IndexOf(redFill);
            }
            else
            {
                // Create a new Fill with a red background.
                Fill newFill = new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFFF0000" } })
                {
                    PatternType = PatternValues.Solid
                });
                fills.Append(newFill);
                fills.Count = (uint)fills.Elements<Fill>().Count();
                redFillIndex = (uint)fills.Elements<Fill>().ToList().IndexOf(newFill);
            }

            // Create a new cell format that uses our red fill.
            // It's a good idea to start from a base cell format.
            CellFormat redCellFormat = new CellFormat()
            {
                FontId = 0,
                FillId = redFillIndex,
                BorderId = 0,
                ApplyFill = true
            };

            cellFormats.Append(redCellFormat);
            cellFormats.Count = (uint)cellFormats.Elements<CellFormat>().Count();

            // The style index is the last index (count - 1)
            return cellFormats.Count - 1;
        }
    }
}