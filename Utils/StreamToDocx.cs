using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace DocumentValidator.Utils
{
    public class StreamToDocx
    {

        public async Task<DocX> StreamToDocxConverter(Stream documentStream)
        {
            // Create a new MemoryStream with the entire content
            documentStream.Position = 0;
            byte[] docBytes = new byte[documentStream.Length];
            await documentStream.ReadAsync(docBytes, 0, docBytes.Length);

            // Create a MemoryStream copy to ensure we have a complete stream to work with
            var memoryStream = new MemoryStream(docBytes);

            DocX document = DocX.Load(memoryStream);

            return document;
        }
    }
}
