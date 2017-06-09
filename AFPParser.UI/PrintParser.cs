using System;
using System.IO;
using System.Linq;
using System.Drawing;
using AFPParser.Triplets;
using AFPParser.Containers;
using System.Drawing.Printing;
using System.Collections.Generic;
using AFPParser.StructuredFields;
using AFPParser.PTXControlSequences;

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
            // Draw each image on the page first
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

            // Get the active environment group
            BAG aeGroup = pageContainers[curPageIndex].GetStructure<BAG>();
            if (aeGroup != null)
            {
                // Grab the Presentation Text Descriptor and store the units per base and base unit
                PGD pageDescriptor = pageContainers[curPageIndex].GetStructure<PGD>();
                PTD1 descriptor1 = pageContainers[curPageIndex].GetStructure<PTD1>();
                PTD2 descriptor2 = pageContainers[curPageIndex].GetStructure<PTD2>();
                int xUnitsPerBase, yUnitsPerBase;
                Lookups.eMeasurement measurement;
                if (descriptor2 != null)
                {
                    xUnitsPerBase = descriptor2.UnitsPerXBase;
                    yUnitsPerBase = descriptor2.UnitsPerYBase;
                    measurement = descriptor2.BaseUnit;
                }
                else if (descriptor1 != null)
                {
                    xUnitsPerBase = descriptor1.UnitsPerXBase;
                    yUnitsPerBase = descriptor1.UnitsPerYBase;
                    measurement = descriptor1.BaseUnit;
                }
                else
                {
                    xUnitsPerBase = pageDescriptor.UnitsPerXBase;
                    yUnitsPerBase = pageDescriptor.UnitsPerYBase;
                    measurement = pageDescriptor.BaseUnit;
                }

                // Keep track of the current font
                Font curFont = new Font(FontFamily.GenericMonospace, 10);
                Brush curBrush = Brushes.Black;

                // Parse each PTX's control sequences
                foreach (PTX text in pageContainers[curPageIndex].GetStructures<PTX>())
                {
                    // Keep a running tab of positional information by running through all of the control sequences
                    double curXPosition = 0;
                    double curYPosition = 0;

                    foreach (PTXControlSequence sequence in text.CSIs)
                    {
                        Type sequenceType = sequence.GetType();

                        if (sequenceType == typeof(SCFL)) curFont = GetFont((SCFL)sequence, aeGroup);
                        if (sequenceType == typeof(AMI)) curXPosition = Lookups.GetInches(((AMI)sequence).Displacement, xUnitsPerBase, measurement) * 100;
                        else if (sequenceType == typeof(AMB)) curYPosition = Lookups.GetInches(((AMB)sequence).Displacement, yUnitsPerBase, measurement) * 100;
                        else if (sequenceType == typeof(RMI)) curXPosition += Lookups.GetInches(((RMI)sequence).Increment, xUnitsPerBase, measurement) * 100;
                        else if (sequenceType == typeof(RMB)) curYPosition += Lookups.GetInches(((RMB)sequence).Increment, yUnitsPerBase, measurement) * 100;
                        else if (sequenceType == typeof(STC)) curBrush = new SolidBrush(((STC)sequence).TextColor);
                        else if (sequenceType == typeof(SEC)) curBrush = new SolidBrush(((SEC)sequence).TextColor);
                        else if (sequenceType == typeof(TRN)) e.Graphics.DrawString(sequence.DataEBCDIC, curFont, curBrush, (float)curXPosition, (float)curYPosition);
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Presentation text could not be displayed - no active environment group found.");
            }


            // Increment the current page index and check to see if there are any more
            e.HasMorePages = ++curPageIndex < pageContainers.Count;
        }

        private Font GetFont(SCFL sfcl, BAG activeEnvironment)
        {
            MCF2 map2 = activeEnvironment.LowestLevelContainer.GetStructure<MCF2>();
            MCF1 map1 = activeEnvironment.LowestLevelContainer.GetStructure<MCF1>();

            // 

            return new Font(FontFamily.GenericMonospace, 6);
        }
    }
}
