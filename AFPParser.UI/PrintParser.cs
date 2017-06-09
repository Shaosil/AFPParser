using System;
using System.IO;
using System.Linq;
using System.Drawing;
using AFPParser.Triplets;
using AFPParser.Containers;
using System.Drawing.Printing;
using System.Collections.Generic;
using AFPParser.StructuredFields;

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

        public void Reset()
        {
            curPageIndex = 0;
        }

        public void BuildPrintPage(object sender, PrintPageEventArgs e)
        {
            // TEMP
            int offset = e.PageBounds.Width / 10;
            int fontSize = 10; // Default to font size 10
            e.Graphics.DrawString("Work in progress...", new Font(FontFamily.GenericMonospace, fontSize), Brushes.Black, offset, offset);

            // Parse each PTX's control sequences
            foreach (PTX text in pageContainers[curPageIndex].GetStructures<PTX>())
            {

            }

            // Draw each image on this page
            foreach (ImageObjectContainer ioc in pageContainers[curPageIndex].GetStructures<BIM>()
                .Select(i => (ImageObjectContainer)i.LowestLevelContainer))
            {
                foreach (ImageContentContainer.ImageInfo image in ioc.Images)
                {
                    // Get the positioning and scaling info based on the current object environment container
                    OBD oaDescriptor = ioc.GetStructure<OBD>();
                    OBP oaPosition = ioc.GetStructure<OBP>();
                    if (oaDescriptor != null && oaPosition != null)
                    {
                        // Get sizing info triplets
                        ObjectAreaSize oaSize = oaDescriptor.GetTriplet<ObjectAreaSize>();
                        MeasurementUnits mu = oaDescriptor.GetTriplet<MeasurementUnits>();

                        // Get inch origins based on unit scaling
                        int xUnitOrigin = oaPosition.XOrigin + oaPosition.XContentOrigin;
                        int yUnitOrigin = oaPosition.YOrigin + oaPosition.YContentOrigin;
                        float xInchOrigin = (float)(Lookups.GetInches(xUnitOrigin, mu.XUnitsPerBase, mu.BaseUnit) * 100);
                        float yInchOrigin = (float)(Lookups.GetInches(yUnitOrigin, mu.YUnitsPerBase, mu.BaseUnit) * 100);

                        // Get inch scaling values
                        double xInchScale = Lookups.GetInches(oaSize.XExtent, mu.XUnitsPerBase, mu.BaseUnit);
                        double yInchScale = Lookups.GetInches(oaSize.YExtent, mu.YUnitsPerBase, mu.BaseUnit);

                        // We have the inch value and number of pixels, so set DPI based on those values
                        Bitmap bmp = new Bitmap(new MemoryStream(image.Data));
                        float xDpi = (float)(bmp.Width / xInchScale);
                        float yDpi = (float)(bmp.Height / yInchScale);
                        bmp.SetResolution(xDpi, yDpi);
                        
                        e.Graphics.DrawImage(bmp, xInchOrigin, yInchOrigin);
                    }
                    else
                    {
                        throw new NotImplementedException("Image could not be displayed - no OBD/OBP fields found.");
                    }
                }
            }


            // Increment the current page index and check to see if there are any more
            e.HasMorePages = ++curPageIndex < pageContainers.Count;
        }
    }
}
