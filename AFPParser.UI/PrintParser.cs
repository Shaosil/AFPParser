using System.Linq;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using AFPParser.StructuredFields;
using AFPParser.Containers;
using System.IO;

namespace AFPParser.UI
{
    public class PrintParser
    {
        private List<StructuredField> structuredFields;
        private List<Container> pageContainers;
        private int curPageIndex = 0;

        public PrintParser(List<StructuredField> fields)
        {
            structuredFields = fields;

            // Capture all pages' containers
            pageContainers = fields.OfType<BPG>().Select(p => p.LowestLevelContainer).ToList();
            if (pageContainers.Count == 0) pageContainers = new List<Container>() { fields[0].LowestLevelContainer };
        }

        public void BuildPrintPage(object sender, PrintPageEventArgs e)
        {
            // TEMP
            int offset = e.PageBounds.Width / 10;
            int fontSize = 10; // Default to font size 10
            e.Graphics.DrawString("Work in progress...", new Font(FontFamily.GenericMonospace, fontSize), Brushes.Black, offset, offset);
            
            //// Parse each PTX's control sequences
            //foreach (PTX text in pageContainers[curPageIndex].GetStructures<PTX>())
            //{

            //}

            //// Draw each image on this page
            //foreach (ImageContentContainer.ImageInfo image in pageContainers[curPageIndex].GetStructures<BIM>()
            //    .SelectMany(i => ((ImageObjectContainer)i.LowestLevelContainer).Images))
            //{
            //    // Not using transparency masks yet...
            //    Bitmap bmp = new Bitmap(new MemoryStream(image.Data));

            //    e.Graphics.DrawImage(bmp, 0, 0);
            //}
            

            // Increment the current page index and check to see if there are any more
            e.HasMorePages = ++curPageIndex < pageContainers.Count;
        }
    }
}
