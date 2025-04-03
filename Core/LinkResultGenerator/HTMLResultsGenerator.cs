using System.Text;
using System.Diagnostics;
using DocumentValidator.Core.DocumentProcessor;
using DocumentValidator.Utils;

namespace DocumentValidator.Core.ResultGenerator
{
    internal static class HTMLResultsGenerator
    {
        public static async Task GenerateHtmlReportAsync(List<LinkValidationResult> results)
        {
            // Retrieve the Downloads folder path
            string fileName = "LinkValidation.html";
            string cssFileName = "styles.css";
            string filePath = Path.Combine(GetDownloadLocation.GetDownloadsFolderPath(), fileName);
            string cssFilePath = Path.Combine(GetDownloadLocation.GetDownloadsFolderPath(), cssFileName);

            // Create the HTML content
            StringBuilder htmlContent = new StringBuilder();
            htmlContent.AppendLine("<!DOCTYPE html>");
            htmlContent.AppendLine("<html lang='en'>");
            htmlContent.AppendLine("<head>");
            htmlContent.AppendLine("    <meta charset='UTF-8'>");
            htmlContent.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            htmlContent.AppendLine("    <title>Validation Results</title>");
            htmlContent.AppendLine($"    <link rel='stylesheet' type='text/css' href='{cssFileName}'>");
            htmlContent.AppendLine("</head>");
            htmlContent.AppendLine("<body>");


            htmlContent.AppendLine("    <div class='container'>");
            htmlContent.AppendLine("    <div class='nav-buttons'>");
            htmlContent.AppendLine($"        <a href='{fileName}' class='nav-button'>Link Results</a>");
            htmlContent.AppendLine("        <a href='AIResults.html' class='nav-button'>AI Results</a>");
            htmlContent.AppendLine("    </div>");
            htmlContent.AppendLine("        <h2>Validation Results</h2>");
            htmlContent.AppendLine("        <table>");
            htmlContent.AppendLine("            <tr>");
            htmlContent.AppendLine("                <th>URL</th>");
            htmlContent.AppendLine("                <th>Link Text</th>");
            htmlContent.AppendLine("                <th>Is Valid</th>");
            htmlContent.AppendLine("                <th>Status Code</th>");
            htmlContent.AppendLine("                <th>Error Message</th>");
            htmlContent.AppendLine("            </tr>");

            foreach (var result in results)
            {
                string rowClass = result.IsValid ? "" : " class='invalid'";
                htmlContent.AppendLine($"            <tr{rowClass}>");
                htmlContent.AppendLine($"                <td><a href='{result.Url}' target='_blank'>{result.Url}</a></td>");
                htmlContent.AppendLine($"                <td>{result.LinkText}</td>");
                htmlContent.AppendLine($"                <td>{result.IsValid}</td>");
                htmlContent.AppendLine($"                <td>{result.StatusCode}</td>");
                htmlContent.AppendLine($"                <td>{result.ErrorMessage}</td>");
                htmlContent.AppendLine("            </tr>");
            }

            htmlContent.AppendLine("        </table>");
            htmlContent.AppendLine("    </div>");
            htmlContent.AppendLine("</body>");
            htmlContent.AppendLine("</html>");


            // Write the HTML content to a file
            await File.WriteAllTextAsync(filePath, htmlContent.ToString());

            // Create the CSS file
            string cssContent = @"/* Modern Typography */
/* Modern Typography */
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600&display=swap');

/* Reset and Base Styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html, body {
  height: 100%;
  width: 100%;
}

html {
  font-size: 16px;
}

body {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, sans-serif;
  background: linear-gradient(135deg, #f5f7fa 0%, #e4edf5 100%);
  color: #4a4a4a;
  line-height: 1.6;
  display: flex;
  justify-content: center;
  align-items: flex-start;
  padding: 0;
  height: 100%;
}

/* Container Styling */
.container {
  width: 100%;
  max-width: 1800px;
  background: #ffffff;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.07);
  margin: 1rem 0;
  flex-grow: 1;
  overflow: visible;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  min-height: 80vh;
}

/* Navigation Buttons */
.nav-buttons {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  width: 100%;
  padding: 1rem 0;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.nav-button {
  padding: 12px 24px;
  background: #3182ce;
  color: white;
  border: none;
  cursor: pointer;
  border-radius: 6px;
  font-size: 16px;
  text-decoration: none;
  font-weight: 500;
  transition: background 0.3s ease, transform 0.1s ease-in-out;
  min-width: 150px;
  text-align: center;
  display: inline-block;
}

.nav-button:hover {
  background: #2b6cb0;
  transform: scale(1.05);
}

.nav-button:active {
  transform: scale(0.98);
}

/* Typography */
h2 {
  text-align: center;
  color: #2d3748;
  font-size: 1.75rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
  position: relative;
  padding-bottom: 0.75rem;
}

h2:after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 70px;
  height: 3px;
  background: #3182ce;
  border-radius: 2px;
}

/* Table Styling */
.table-wrapper {
  overflow-x: auto;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  width: 100%;
}

table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  margin: 0;
}

th, td {
  padding: 0.875rem 1rem;
  text-align: left;
}

th {
  background-color: #3182ce;
  color: white;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-size: 0.85rem;
  white-space: nowrap;
  position: sticky;
  top: 0;
}

tr {
  border-bottom: 1px solid rgba(0, 0, 0, 0.08);
}

tr:last-child {
  border-bottom: none;
}

tr:hover {
  background-color: rgba(249, 250, 251, 0.7);
}

tr:nth-child(even) {
  background-color: #f7fafc;
}

tr:nth-child(even):hover {
  background-color: #f0f5fa;
}

/* Status Styles */
.invalid {
  background-color: #fff5f5;
  color: #e53e3e;
  font-weight: 500;
  border-left: 3px solid #e53e3e;
}

/* Link Styling */
a {
  text-decoration: none;
  color: #3182ce;
  font-weight: 500;
  transition: color 0.2s ease;
}

a:hover {
  color: #2b6cb0;
  text-decoration: underline;
}

/* Ensure buttons and controls have proper styling */
button, select, input {
  font-family: inherit;
  font-size: 0.9rem;
  padding: 0.5rem 0.75rem;
  border-radius: 4px;
  border: 1px solid #cbd5e0;
  background: white;
}

button {
  background-color: #3182ce;
  color: white;
  border: none;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

button:hover {
  background-color: #2b6cb0;
}

/* Responsive Design */
@media screen and (max-width: 1800px) {
  .container {
    max-width: 95%;
    margin: 1rem auto;
  }
}

@media screen and (max-width: 1024px) {
  html {
    font-size: 15px;
  }

  .container {
    max-width: 95%;
  }
}

@media screen and (max-width: 768px) {
  html {
    font-size: 14px;
  }

  body {
    padding: 0.75rem;
  }

  .container {
    padding: 1.25rem;
    margin: 0.5rem auto;
    border-radius: 8px;
  }

  h2 {
    font-size: 1.5rem;
    margin-bottom: 1.25rem;
  }

  th, td {
    padding: 0.75rem 0.875rem;
  }

  .nav-buttons {
    flex-direction: column;
    align-items: center;
  }

  .nav-button {
    width: 90%;
    text-align: center;
  }
}

@media screen and (max-width: 480px) {
  html {
    font-size: 13px;
  }

  body {
    padding: 0.5rem;
  }

  .container {
    padding: 1rem;
    margin: 0.25rem auto;
    border-radius: 6px;
  }

  h2 {
    font-size: 1.35rem;
    margin-bottom: 1rem;
    padding-bottom: 0.5rem;
  }

  th, td {
    padding: 0.675rem 0.75rem;
    font-size: 0.85rem;
  }
}

";
            await File.WriteAllTextAsync(cssFilePath, cssContent);

            // Open the HTML file in the default browser
            OpenHtmlFile(filePath);
        }

        public static void OpenHtmlFile(string filePath)
        {
            var process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.UseShellExecute = true; // Open with default browser
            process.Start();
        }
    }
}