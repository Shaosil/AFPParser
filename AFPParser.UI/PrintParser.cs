using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Reflection;
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
        private AFPFile afpFile;
        private List<Container> pageContainers;
        private int curPageIndex = 0;

        // PTX Storage
        private Container aeContainer = null;
        private float curXPosition = 0;
        private float curYPosition = 0;
        private int xUnitsPerBase = 0;
        private int yUnitsPerBase = 0;
        private float interCharAdjInch = 0;
        private float varSpaceCharInch = 0;
        private Converters.eMeasurement measurement = Converters.eMeasurement.Inches;
        private Dictionary<string, byte> codePageMapping = CodePages.C1252;
        private AFPFile.Resource codePage = null;
        private AFPFile.Resource fontCharacterSet = null;
        private Color curColor = Color.Black;

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
                    float xInch = (float)Converters.GetInches(pageSegment.XOrigin, pageDescriptor.UnitsPerYBase, pageDescriptor.BaseUnit) * 100;
                    float yInch = (float)Converters.GetInches(pageSegment.YOrigin, pageDescriptor.UnitsPerYBase, pageDescriptor.BaseUnit) * 100;

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
            float xInchPos = xStartingInch + (float)(Converters.GetInches(xPos, imDescriptor.XUnitsPerBase, imDescriptor.BaseUnit) * 100);
            float yInchPos = yStartingInch + (float)(Converters.GetInches(yPos, imDescriptor.YUnitsPerBase, imDescriptor.BaseUnit) * 100);
            double heightInches = Converters.GetInches(bmp.Height, imDescriptor.YUnitsPerBase, imDescriptor.BaseUnit);

            float dpi = (float)Math.Round(bmp.Height / heightInches);
            bmp.SetResolution(dpi, dpi);
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
                    float xInchOrigin = xStartingInch + (float)(Converters.GetInches(xUnitOrigin, mu.XUnitsPerBase, mu.BaseUnit) * 100);
                    float yInchOrigin = yStartingInch + (float)(Converters.GetInches(yUnitOrigin, mu.YUnitsPerBase, mu.BaseUnit) * 100);

                    // Get inch scaling values
                    double heightInches = Converters.GetInches(oaSize.YExtent, mu.YUnitsPerBase, mu.BaseUnit);

                    // We have the inch value and number of pixels, so set DPI based on those values
                    Bitmap bmp = new Bitmap(new MemoryStream(image.Data));
                    float dpi = (float)Math.Round(bmp.Height / heightInches);
                    bmp.SetResolution(dpi, dpi);

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
            // Store presentation text/page descriptor information
            GetDescriptorInfo();

            // Parse each PTX's control sequences
            foreach (PTX text in pageContainers[curPageIndex].GetStructures<PTX>())
            {
                // Reset PTX variables at the beginning of each PTX field
                curXPosition = 0;
                curYPosition = 0;
                interCharAdjInch = 0;
                varSpaceCharInch = 0;
                codePageMapping = CodePages.C1252;
                fontCharacterSet = null;
                curColor = Color.Black;

                foreach (PTXControlSequence sequence in text.CSIs)
                {
                    Type sequenceType = sequence.GetType();

                    if (sequenceType == typeof(SCFL)) SetCodePageAndFont((SCFL)sequence, out codePageMapping, out codePage, out fontCharacterSet);
                    else if (sequenceType == typeof(AMI)) curXPosition = (float)Converters.GetInches(((AMI)sequence).Displacement, xUnitsPerBase, measurement) * 100;
                    else if (sequenceType == typeof(AMB)) curYPosition = (float)Converters.GetInches(((AMB)sequence).Displacement, yUnitsPerBase, measurement) * 100;
                    else if (sequenceType == typeof(RMI)) curXPosition += (float)Converters.GetInches(((RMI)sequence).Increment, xUnitsPerBase, measurement) * 100;
                    else if (sequenceType == typeof(RMB)) curYPosition += (float)Converters.GetInches(((RMB)sequence).Increment, yUnitsPerBase, measurement) * 100;
                    else if (sequenceType == typeof(STC)) curColor = ((STC)sequence).TextColor;
                    else if (sequenceType == typeof(SEC)) curColor = ((SEC)sequence).TextColor;
                    else if (sequenceType == typeof(SIA)) interCharAdjInch = ((((SIA)sequence).Adjustment * (((SIA)sequence).Forward ? 1 : -1)) / 1440f) * 100;
                    else if (sequenceType == typeof(SVI)) varSpaceCharInch = (((SVI)sequence).Increment / 1440f) * 100;
                    else if (sequenceType == typeof(DIR) || sequenceType == typeof(DBR)) DrawLine(sequence, e);
                    else if (sequenceType == typeof(TRN)) DrawStringAsImage(sequence.Data, e);
                }
            }
        }

        private void DrawLine(PTXControlSequence sequence, PrintPageEventArgs e)
        {
            // Inline (horizontal) or Baseline (vertical)?
            bool isInline = sequence.GetType() == typeof(DIR);

            // Get the width of the line first (1/1440 of an inch)
            int width = (int)sequence.GetType().GetProperty("RuleWidth").GetValue(sequence);

            // One dot will be visible if the width is greater than....?
            float widthInches = (width / 1440f) * 100;
            float dotInches = (1 / (float)(isInline ? e.PageSettings.PrinterResolution.X : e.PageSettings.PrinterResolution.Y)) * 100;

            // Prepare X,Y origin/destination
            int length = (int)sequence.GetType().GetProperty("RuleLength").GetValue(sequence);
            float xOrig = curXPosition, yOrig = curYPosition, xDest = curXPosition, yDest = curYPosition;

            // If the width > 1 dot, shift the line points by half the width to un-center it
            if (widthInches > dotInches)
            {
                if (isInline)
                {
                    // Shift Y
                    yOrig -= widthInches / 2f;
                    yDest -= widthInches / 2f;
                }
                else
                {
                    // Shift X
                    xOrig -= widthInches / 2f;
                    xDest -= widthInches / 2f;
                }
            }

            // Set destination
            if (isInline)
                xDest += (length / 1440f) * 100;
            else
                yDest += (length / 1440f) * 100;

            e.Graphics.DrawLine(new Pen(curColor), xOrig, yOrig, xDest, yDest);
        }

        private void GetDescriptorInfo()
        {
            BAG activeEnvironmentGroup = pageContainers[curPageIndex].GetStructure<BAG>();

            if (activeEnvironmentGroup != null)
            {
                aeContainer = activeEnvironmentGroup.LowestLevelContainer;

                // Grab the Presentation Text Descriptor and store the units per base and base unit
                PGD pageDescriptor = pageContainers[curPageIndex].GetStructure<PGD>();
                PTD1 descriptor1 = pageContainers[curPageIndex].GetStructure<PTD1>();
                PTD2 descriptor2 = pageContainers[curPageIndex].GetStructure<PTD2>();

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
            }
            else
            {
                throw new NotImplementedException("Presentation text could not be displayed - no active environment group found.");
            }
        }

        private void DrawStringAsImage(byte[] data, PrintPageEventArgs e)
        {
            if (fontCharacterSet != null)
            {
                // Build a list of images that will be used (all unique points of data should be associated with raster data by GID)
                FontObjectContainer foc = (FontObjectContainer)fontCharacterSet.Fields[0].LowestLevelContainer;
                List<KeyValuePair<Bitmap, FNI.Info>> bmps = new List<KeyValuePair<Bitmap, FNI.Info>>();

                // Point size = (nominal font size / 10). There are 72 points in an inch
                FND descriptor = foc.GetStructure<FND>();
                float emInchSize = descriptor.EmInches;

                foreach (byte b in data)
                {
                    // Find GID
                    if (codePageMapping.ContainsValue(b))
                    {
                        string gid = codePageMapping.First(c => c.Value == b).Key;

                        // If this is a variable space character, only increment our X position by the defined value
                        if ((codePage != null && codePage.IsLoaded && b == codePage.Fields.OfType<CPC>().First().VariableSpaceCharacter) || gid == "SP010000")
                            bmps.Add(new KeyValuePair<Bitmap, FNI.Info>());
                        else
                        {
                            // Get raster pattern of this GID
                            KeyValuePair<FNI.Info, bool[,]> thisPattern = foc.RasterPatterns.FirstOrDefault(p => p.Key.GCGID == gid);
                            if (thisPattern.Key != null)
                            {
                                // Build a basic bitmap out of raster information
                                Bitmap bmp = new Bitmap(thisPattern.Value.GetUpperBound(0) + 1, thisPattern.Value.GetUpperBound(1) + 1);
                                for (int y = 0; y < bmp.Height; y++)
                                    for (int x = 0; x < bmp.Width; x++)
                                        if (thisPattern.Value[x, y])
                                            bmp.SetPixel(x, y, curColor);

                                // Since we know how many inches 1 em is, we can determine inch width and height of each character
                                float heightInches = emInchSize * ((thisPattern.Key.AscenderHeight + thisPattern.Key.DescenderDepth) / 1000f);
                                float dpi = (float)Math.Round(bmp.Height / heightInches);
                                bmp.SetResolution(dpi, dpi);

                                bmps.Add(new KeyValuePair<Bitmap, FNI.Info>(bmp, thisPattern.Key));
                            }
                        }
                    }
                }

                // For each byte in our data string, lookup which bitmap to display and throw it on the screen
                foreach (KeyValuePair<Bitmap, FNI.Info> kvp in bmps)
                {
                    // If the BMP is null, increment our X position by (variable space character increment)
                    if (kvp.Key == null)
                        curXPosition += GetVariableSpaceIncrementInch();
                    else
                    {
                        float aSpaceInches = emInchSize * (kvp.Value.ASpace / 1000f) * 100;
                        float cSpaceInches = emInchSize * (kvp.Value.CSpace / 1000f) * 100;
                        float baselineOffsetInches = emInchSize * (kvp.Value.BaselineOffset / 1000f) * 100;
                        float charIncrement = emInchSize * (kvp.Value.CharIncrement / 1000f) * 100;

                        // Draw image
                        e.Graphics.DrawImage(kvp.Key, curXPosition - aSpaceInches, curYPosition - baselineOffsetInches);

                        // Increment our spacing by our character increment (+- adjustment) for this byte
                        curXPosition += charIncrement + interCharAdjInch;
                    }
                }
            }
        }

        private float GetVariableSpaceIncrementInch()
        {
            // 1 - The current variable space character increment
            if (varSpaceCharInch != 0) return varSpaceCharInch;

            if (fontCharacterSet != null)
            {
                float emInches = fontCharacterSet.Fields.OfType<FND>().First().EmInches;

                // 2 - The default variable space character increment of the active coded font
                int fontCharInc = fontCharacterSet.Fields.OfType<FNO>().First().FNOInfo[0].SpaceCharIncrement;
                if (fontCharInc != 0) return emInches * (fontCharInc / 1000f) * 100;

                // 3 - The character increment of the default variable space character code point
                FNI.Info info = fontCharacterSet.Fields.OfType<FNI>().First().InfoList.FirstOrDefault(i => i.GCGID == "SP010000");
                if (info != null) return emInches * (info.CharIncrement / 1000f) * 100;
            }

            return 0;
        }

        private void SetCodePageAndFont(SCFL sfcl, out Dictionary<string, byte> codePageMappings, out AFPFile.Resource codePage, out AFPFile.Resource fontCharacterSet)
        {
            // Default code page to 1252
            string curCodePageStr = "1252";
            codePageMappings = CodePages.C1252;
            codePage = null;
            fontCharacterSet = null;

            MCF1 map1 = aeContainer.GetStructure<MCF1>();

            // MCF2 is not supported yet...
            if (map1 != null)
            {
                // Get mapping info with the ID specified in the SCFL field
                MCF1.MCF1Data mcfData = map1.MappedData.FirstOrDefault(m => m.ID == sfcl.FontId);

                if (mcfData != null)
                {
                    // If it already has a code page/font character set specified, use that.
                    if (!string.IsNullOrWhiteSpace(mcfData.CodePageName) && !string.IsNullOrWhiteSpace(mcfData.FontCharacterSetName))
                    {
                        curCodePageStr = mcfData.CodePageName;
                        fontCharacterSet = afpFile.Resources.OfTypeAndName(AFPFile.Resource.eResourceType.FontCharacterSet, mcfData.FontCharacterSetName);
                    }
                    else
                    {
                        // Otherwise, we need to load it from the coded font resource
                        AFPFile.Resource codedFont = afpFile.Resources.OfTypeAndName(AFPFile.Resource.eResourceType.CodedFont, mcfData.CodedFontName);

                        if (codedFont.IsLoaded)
                        {
                            CFI cfi = codedFont.Fields.OfType<CFI>().FirstOrDefault();
                            if (cfi != null && cfi.FontInfoList.Any())
                            {
                                curCodePageStr = cfi.FontInfoList[0].CodePageName;
                                fontCharacterSet = afpFile.Resources.OfTypeAndName(AFPFile.Resource.eResourceType.FontCharacterSet, cfi.FontInfoList[0].FontCharacterSetName);
                            }
                        }
                    }

                    // If code page is a resource, build the GID mapping dictionary manually.
                    codePage = afpFile.Resources.OfTypeAndName(AFPFile.Resource.eResourceType.CodePage, curCodePageStr);
                    if (codePage != null && codePage.IsLoaded)
                    {
                        codePageMappings = new Dictionary<string, byte>();
                        foreach (CPI.Info cpiInfo in codePage.Fields.OfType<CPI>().First().CPIInfos)
                            codePageMappings.Add(cpiInfo.GID, cpiInfo.CodePoints[0]); // Only single byte code points are supported for now
                    }
                    // Else, if we have a predefined lookup table for the code page ID, use that
                    else
                    {
                        string sectionedCodePage = string.Empty;
                        if (curCodePageStr.Length >= 4)
                            sectionedCodePage = $"C{curCodePageStr.Substring(curCodePageStr.Length - 4)}";

                        // Find the matching lookup method in our code page helper class
                        FieldInfo field = typeof(CodePages).GetField(sectionedCodePage);
                        if (field != null)
                            codePageMappings = (Dictionary<string, byte>)field.GetValue(null);
                    }
                }
            }
        }
    }
}