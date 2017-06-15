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
using System.Windows.Forms;

namespace AFPParser.UI
{
    public class PrintParser
    {
        private AFPFile afpFile;
        private List<Container> pageContainers;
        private int curPageIndex = 0;

        public PrintParser(AFPFile file)
        {
            afpFile = file;

            // Capture all pages' containers
            pageContainers = afpFile.Fields.OfType<BPG>().Select(p => p.LowestLevelContainer).ToList();
            if (pageContainers.Count == 0) pageContainers = new List<Container>() { afpFile.Fields[0].LowestLevelContainer };
        }

        public void BuildPrintPage(object sender, PrintPageEventArgs e)
        {
            // Draw each embedded IM image in the current page. Positional information is stored inside the IID field of each image container
            foreach (IMImageContainer imc in pageContainers[curPageIndex].Structures.Select(f => f.LowestLevelContainer).Distinct().OfType<IMImageContainer>())
                DrawIMImage(imc, 0, 0, e);

            // Draw each embedded IOCA image in the current page. Positional information is stored inside of the OBD/OBP fields of each image container
            foreach (IOCAImageContainer ioc in pageContainers[curPageIndex].Structures.Select(f => f.LowestLevelContainer).Distinct().OfType<IOCAImageContainer>())
                DrawIOCAImage(ioc, 0, 0, e);

            // Include each IM and IOCA image in IPS by looking up the loaded resource
            foreach (IPS pageSegment in pageContainers[curPageIndex].GetStructures<IPS>())
            {
                // Find the first loaded image resource of the indicated name
                AFPFile.Resource loadedResource = afpFile.Resources.FirstOrDefault(r => r.ResourceName == pageSegment.SegmentName.ToUpper().Trim()
                     && r.IsLoaded && r.ResourceType == AFPFile.Resource.eResourceType.PageSegment);

                if (loadedResource != null)
                {
                    // Get starting positional information by querying the active environment group
                    Container aegContainer = pageContainers[curPageIndex].GetStructure<BAG>().LowestLevelContainer;
                    PGD pageDescriptor = aegContainer.GetStructure<PGD>();
                    float xInch = (float)Lookups.GetInches(pageSegment.XOrigin, pageDescriptor.UnitsPerYBase, pageDescriptor.BaseUnit) * 100;
                    float yInch = (float)Lookups.GetInches(pageSegment.YOrigin, pageDescriptor.UnitsPerYBase, pageDescriptor.BaseUnit) * 100;

                    if (loadedResource.Fields.Any(f => f.LowestLevelContainer.GetType() == typeof(IMImageContainer)))
                    {

                        IMImageContainer imc = loadedResource.Fields.Select(f => f.LowestLevelContainer).OfType<IMImageContainer>().FirstOrDefault();
                        DrawIMImage(imc, xInch, yInch, e);
                    }
                    else if (loadedResource.Fields.Any(f => f.LowestLevelContainer.GetType() == typeof(IOCAImageContainer)))
                    {
                        IOCAImageContainer ioc = loadedResource.Fields.Select(f => f.LowestLevelContainer).OfType<IOCAImageContainer>().FirstOrDefault();
                        DrawIOCAImage(ioc, xInch, yInch, e);
                    }
                }
            }

            DrawPresentationText(e);

            // Increment the current page index and check to see if there are any more
            e.HasMorePages = ++curPageIndex < pageContainers.Count;

            if (!e.HasMorePages) curPageIndex = 0;
        }

        private void DrawIMImage(IMImageContainer imc, float xStartingInch, float yStartingInch, PrintPageEventArgs e)
        {
            // Build a bitmap out of the 2 dimensional array of booleans
            Bitmap bmp = new Bitmap(imc.ImageData.GetUpperBound(0) + 1, imc.ImageData.GetUpperBound(1) + 1);
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                    bmp.SetPixel(x, y, imc.ImageData[x, y] ? Color.Black : Color.White);

            // Get positional/scaling information from min offsets and IID field
            IID imDescriptor = imc.GetStructure<IID>();
            int xPos = imc.Cells.Min(c => c.CellPosition.XOffset);
            int yPos = imc.Cells.Min(c => c.CellPosition.YOffset);
            float xInchPos = xStartingInch + (float)(Lookups.GetInches(xPos, imDescriptor.XUnitsPerBase, imDescriptor.BaseUnit) * 100);
            float yInchPos = yStartingInch + (float)(Lookups.GetInches(yPos, imDescriptor.YUnitsPerBase, imDescriptor.BaseUnit) * 100);
            double xScale = Lookups.GetInches(bmp.Width, imDescriptor.XUnitsPerBase, imDescriptor.BaseUnit);
            double yScale = Lookups.GetInches(bmp.Height, imDescriptor.YUnitsPerBase, imDescriptor.BaseUnit);

            bmp.SetResolution((float)(bmp.Width / xScale), (float)(bmp.Height / yScale));
            e.Graphics.DrawImage(bmp, xInchPos, yInchPos);
        }

        private void DrawIOCAImage(IOCAImageContainer imc, float xStartingInch, float yStartingInch, PrintPageEventArgs e)
        {
            // Each container may hold one image or several, as tiles. Draw each one with consideration to its offset from the original draw point
            foreach (ImageContentContainer.ImageInfo image in imc.Images)
            {
                // Get the positioning and scaling info based on the current object environment container
                OBD oaDescriptor = imc.GetStructure<OBD>();
                OBP oaPosition = imc.GetStructure<OBP>();
                if (oaDescriptor != null && oaPosition != null)
                {
                    // Get sizing info triplets
                    ObjectAreaSize oaSize = oaDescriptor.GetTriplet<ObjectAreaSize>();
                    MeasurementUnits mu = oaDescriptor.GetTriplet<MeasurementUnits>();

                    // Get inch origins based on unit scaling
                    int xUnitOrigin = oaPosition.XOrigin + oaPosition.XContentOrigin;
                    int yUnitOrigin = oaPosition.YOrigin + oaPosition.YContentOrigin;
                    float xInchOrigin = xStartingInch + (float)(Lookups.GetInches(xUnitOrigin, mu.XUnitsPerBase, mu.BaseUnit) * 100);
                    float yInchOrigin = yStartingInch + (float)(Lookups.GetInches(yUnitOrigin, mu.YUnitsPerBase, mu.BaseUnit) * 100);

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

        private void DrawPresentationText(PrintPageEventArgs e)
        {
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

                        if (sequenceType == typeof(SCFL)) curFont = GetFont((SCFL)sequence, aeGroup.LowestLevelContainer);
                        else if (sequenceType == typeof(AMI)) curXPosition = Lookups.GetInches(((AMI)sequence).Displacement, xUnitsPerBase, measurement) * 100;
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
        }

        private Font GetFont(SCFL sfcl, Container aegContainer)
        {
            Font fnt = new Font(FontFamily.GenericMonospace, 6);
            
            MCF1 map1 = aegContainer.GetStructure<MCF1>();

            // MCF2 is not supported yet...
            if (map1 != null)
            {
                // Get mapping info with the ID specified in the SCFL field
                MCF1.MCF1Data mcfData = map1.MappedData.FirstOrDefault(m => m.ID == sfcl.FontId);

                if (mcfData != null)
                {
                    // If it already has a font character set specified, use that.
                    AFPFile.Resource fontCharacterSet = null;
                    if (!string.IsNullOrWhiteSpace(mcfData.FontCharacterSetName))
                        fontCharacterSet = afpFile.Resources.OfTypeAndName(AFPFile.Resource.eResourceType.FontCharacterSet, mcfData.FontCharacterSetName);
                    else
                    {
                        // Otherwise, we need to load it from the coded font resource
                        AFPFile.Resource codedFont = afpFile.Resources.OfTypeAndName(AFPFile.Resource.eResourceType.CodedFont, mcfData.CodedFontName);

                        if (codedFont.IsLoaded)
                        {
                            CFI cfi = codedFont.Fields.OfType<CFI>().FirstOrDefault();
                            if (cfi != null && cfi.FontInfoList.Any())
                                fontCharacterSet = afpFile.Resources.OfTypeAndName(AFPFile.Resource.eResourceType.FontCharacterSet, cfi.FontInfoList[0].FontCharacterSetName);
                        }
                    }

                    if (fontCharacterSet != null)
                    {
                        // Find the raster pattern of....................
                    }
                }
            }

            return fnt;
        }
    }
}
