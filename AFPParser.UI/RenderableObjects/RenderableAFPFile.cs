using AFPParser.ImageSelfDefiningFields;
using AFPParser.StructuredFields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AFPParser.UI
{
    public class RenderableAFPFile : AFPFile
    {
        public List<string> ResourceDirectories { get; set; }
        public IReadOnlyList<Resource> Resources { get; private set; }
        public IReadOnlyDictionary<Container, IReadOnlyList<IMImageCell>> ParsedIMImages { get; private set; }
        public IReadOnlyDictionary<Container, IReadOnlyList<ImageInfo>> ParsedImages { get; private set; }
        public IReadOnlyDictionary<Container, IReadOnlyDictionary<FNI.Info, bool[,]>> ParsedFontPatterns { get; private set; }

        public RenderableAFPFile()
        {
            ResourceDirectories = new List<string>() { Environment.CurrentDirectory };
            Resources = new List<Resource>();
        }

        public override bool LoadData(string path, bool parseData = false)
        {
            bool success = base.LoadData(path, parseData);

            // Only load resources if we have parsed the data
            if (success && parseData)
            {
                try
                {
                    // Store embedded resources
                    Resources = new List<Resource>();
                    List<Resource> embeddedResources = new List<Resource>();
                    List<Container> allContainers = Fields.Where(f => f.LowestLevelContainer != null).Select(f => f.LowestLevelContainer).Distinct().ToList();

                    // Gather embedded IM, IOCA, and Font containers
                    IEnumerable<Container> IOCAContainers = allContainers.Where(c => c.Structures[0] is BIM);
                    IEnumerable<Container> IMContainers = allContainers.Where(c => c.Structures[0] is BII);
                    IEnumerable<Container> fontContainers = allContainers.Where(c => c.Structures[0] is BFN);
                    IEnumerable<Container> codedFontContainers = allContainers.Where(c => c.Structures[0] is BCF);
                    IEnumerable<Container> codePageContainers = allContainers.Where(c => c.Structures[0] is BCP);

                    // Add all embedded resources to the resource list
                    foreach (Container c in IOCAContainers.Concat(IMContainers).Concat(fontContainers).Concat(codedFontContainers).Concat(codePageContainers))
                    {
                        // Each container has a property called "ObjectName" that can be used to get the resource name here
                        string resName = c.Structures[0].GetType().GetProperty("ObjectName").GetValue(c.Structures[0]).ToString();
                        Resource newResource = new Resource(resName, GetResourceTypeByContainer(c), true);
                        newResource.Fields = c.Structures.Cast<StructuredField>().ToList();
                        embeddedResources.Add(newResource);
                    }

                    // Add embedded and external referenced resources
                    Resources = embeddedResources; // Do this first as to avoid duplicates
                    List<Resource> referencedResources = LoadReferencedResources(Fields);

                    // Try to find all referenced files
                    ScanDirectoriesForResources(referencedResources);

                    // Combine and assign to readonly property
                    Resources = Resources.Concat(referencedResources).ToList();

                    // Load image and font data by their containers and store them in our derived properties
                    ParseFontAndImageData();
                }
                catch (Exception ex)
                {
                    InvokeErrorEvent($"Error: {ex.Message}");
                    success = false;
                }
            }

            return success;
        }

        private List<Resource> LoadReferencedResources(IEnumerable<StructuredField> fileFields)
        {
            List<Resource> allResources = new List<Resource>();

            // Add referenced page segments
            foreach (string s in fileFields.OfType<IPS>().Where(f => f.SegmentName != null).Select(f => f.SegmentName).Distinct())
                allResources.Add(new Resource(s, Resource.eResourceType.PageSegment));

            // Helper lists
            List<MCF1.MCF1Data> mcf1Data = fileFields.OfType<MCF1>().SelectMany(f => f.MappedData).ToList();
            List<string> existingCodePages = Resources.GetNamesOfType(Resource.eResourceType.CodePage);
            List<string> existingCodedFonts = Resources.GetNamesOfType(Resource.eResourceType.CodedFont);
            List<string> existingFontCharacterSets = Resources.GetNamesOfType(Resource.eResourceType.FontCharacterSet);

            // Get MCF1 code pages that are not already embedded
            List<string> codePages = mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.CodePageName)).Select(m => m.CodePageName)
                .Except(existingCodePages).ToList();

            // Get MCF1 coded fonts that are not already embedded
            List<string> codedFonts = mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.CodedFontName)).Select(m => m.CodedFontName)
                .Except(existingCodedFonts).ToList();

            // Get MCF1 font character sets that are not already embedded
            List<string> fontCharacterSets = mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.FontCharacterSetName)).Select(m => m.FontCharacterSetName)
                .Except(existingFontCharacterSets).ToList();

            // Get Coded Fonts code pages that are not already embedded
            codePages.AddRange(fileFields.OfType<CFI>().SelectMany(f => f.FontInfoList.Select(i => i.CodePageName)).Except(existingCodePages));

            // Get Coded Fonts font character sets that are not already embedded
            fontCharacterSets.AddRange(fileFields.OfType<CFI>().SelectMany(f => f.FontInfoList.Select(i => i.FontCharacterSetName)).Except(existingCodedFonts));

            // Add referenced font infos
            foreach (string s in codePages)
                allResources.Add(new Resource(s, Resource.eResourceType.CodePage));
            foreach (string s in codedFonts)
                allResources.Add(new Resource(s, Resource.eResourceType.CodedFont));
            foreach (string s in fontCharacterSets)
                allResources.Add(new Resource(s, Resource.eResourceType.FontCharacterSet));

            return allResources;
        }

        public void ScanDirectoriesForResources(IEnumerable<Resource> resources)
        {
            // Attempt to find a matching file for each resource and load it
            List<FileInfo> allFiles = ResourceDirectories.SelectMany(d => new DirectoryInfo(d).GetFiles()).ToList();
            foreach (Resource r in resources)
            {
                FileInfo matchingFile = allFiles.FirstOrDefault(f => f.Name.ToUpper().Trim() == r.ResourceName);
                if (matchingFile != null)
                {
                    r.Fields = LoadFields(matchingFile.FullName, true);

                    // Also see if there are any additional resources to load from within those fields
                    List<Resource> extraResources = LoadReferencedResources(r.Fields);

                    // If there are, add them to the total resources list and recursively search for files within known directories
                    if (extraResources.Any())
                    {
                        Resources = Resources.Concat(extraResources).ToList();
                        ScanDirectoriesForResources(extraResources);
                    }
                }
            }
        }

        private Resource.eResourceType GetResourceTypeByContainer(Container c)
        {
            Resource.eResourceType rType = Resource.eResourceType.Unknown;

            if (c.Structures[0] is BII) rType = Resource.eResourceType.IMImage;
            else if (c.Structures[0] is BIM) rType = Resource.eResourceType.IOCAImage;
            else if (c.Structures[0] is BFN) rType = Resource.eResourceType.FontCharacterSet;
            else if (c.Structures[0] is BCP) rType = Resource.eResourceType.CodePage;
            else if (c.Structures[0] is BCF) rType = Resource.eResourceType.CodedFont;
            else if (c.Structures[0] is IPS) rType = Resource.eResourceType.PageSegment;

            return rType;
        }

        private void ParseFontAndImageData()
        {
            // FONT RASTER PATTERNS
            Dictionary<Container, IReadOnlyDictionary<FNI.Info, bool[,]>> rasterPatterns = new Dictionary<Container, IReadOnlyDictionary<FNI.Info, bool[,]>>();
            foreach (Container c in Resources.Where(r => r.ResourceType == Resource.eResourceType.FontCharacterSet && r.IsLoaded)
                .Select(r => r.Fields[0].LowestLevelContainer))
            {
                // If we have a pattern map, gather raster data
                FNM patternsMap = c.GetStructure<FNM>();
                if (patternsMap != null)
                {
                    FNI firstFNI = c.GetStructure<FNI>();
                    Dictionary<FNI.Info, bool[,]> patternsDictionary = new Dictionary<FNI.Info, bool[,]>();
                    byte[] allFNGData = c.GetStructures<FNG>().SelectMany(f => f.Data).ToArray();
                    int indexCounter = 0;

                    for (int i = 0; i < patternsMap.AllPatternData.Count; i++)
                    {
                        // Subtract the next offset (or length of data if at end) by this one to find out how many bytes to take
                        int bytesToTake = (int)((i < patternsMap.AllPatternData.Count - 1 ? patternsMap.AllPatternData[i + 1].DataOffset : (uint)allFNGData.Length)
                            - patternsMap.AllPatternData[i].DataOffset);

                        // Create an empty array of bools from our box width and height
                        // The array sizes are the number of bits in the minimum number of bytes required to support the bit size
                        int numBitsWide = (int)Math.Ceiling((patternsMap.AllPatternData[i].BoxMaxWidthIndex + 1) / 8.0) * 8;
                        int numRows = bytesToTake / (numBitsWide / 8);
                        bool[,] curPattern = new bool[numBitsWide, numRows];
                        for (int y = 0; y < numRows; y++)
                            for (int x = 0; x < numBitsWide; x += 8)
                            {
                                byte curByte = allFNGData[indexCounter++];
                                for (int b = 0; b < 8; b++)
                                    curPattern[x + b, y] = (curByte & (1 << (7 - b))) > 0;
                            }

                        // Lookup the GCGID from the first FNI for this pattern
                        patternsDictionary.Add(firstFNI.InfoList.First(fni => fni.FNMIndex == i), curPattern);
                    }
                    rasterPatterns.Add(c, patternsDictionary);
                }
            }
            ParsedFontPatterns = rasterPatterns;

            // IM IMAGES
            Dictionary<Container, IReadOnlyList<IMImageCell>> imImages = new Dictionary<Container, IReadOnlyList<IMImageCell>>();
            foreach (Container c in Resources
                .Where(r => r.IsLoaded && (r.ResourceType == Resource.eResourceType.IMImage || (r.ResourceType == Resource.eResourceType.PageSegment && r.Fields[1] is BII)))
                .Select(r => r.ResourceType == Resource.eResourceType.PageSegment ? r.Fields[1].LowestLevelContainer : r.Fields[0].LowestLevelContainer))
            {
                IID imageDescriptor = c.GetStructure<IID>();
                List<IMImageCell> cellList = new List<IMImageCell>();

                if (c.GetStructure<ICP>() == null)
                {
                    // Since there are no cells, create one
                    ICP newCellPos = new ICP(imageDescriptor.XSize, imageDescriptor.YSize);
                    IMImageCell newCell = new IMImageCell(newCellPos, c.GetStructures<IRD>());
                    cellList.Add(newCell);
                }
                else
                {
                    // Manually parse a list of cells since they don't have their own container
                    for (int i = 0; i < c.Structures.Count; i++)
                    {
                        if (c.Structures[i].GetType() != typeof(ICP)) continue;

                        // Get list of IRDs up to the next ICP or end of structures
                        List<IRD> rasterData = new List<IRD>();
                        for (int r = i + 1; r < c.Structures.Count; r++)
                        {
                            if (c.Structures[r].GetType() != typeof(IRD)) break;
                            rasterData.Add((IRD)c.Structures[r]);
                        }

                        cellList.Add(new IMImageCell((ICP)c.Structures[i], rasterData));
                    }
                }
                imImages.Add(c, cellList);
            }
            ParsedIMImages = imImages;

            // IOCA IMAGES
            Dictionary<Container, IReadOnlyList<ImageInfo>> iocaImages = new Dictionary<Container, IReadOnlyList<ImageInfo>>();
            foreach (Container c in Resources
                .Where(r => r.IsLoaded && (r.ResourceType == Resource.eResourceType.IMImage || (r.ResourceType == Resource.eResourceType.PageSegment && r.Fields[1] is BIM)))
                .Select(r => r.ResourceType == Resource.eResourceType.PageSegment ? r.Fields[1].LowestLevelContainer : r.Fields[0].LowestLevelContainer))
            {
                // Combine all self defining fields from zero or more IPD fields
                byte[] allIPDData = c.GetStructures<IPD>().SelectMany(f => f.Data).ToArray();
                List<ImageSelfDefiningField> SDFs = ImageSelfDefiningField.GetAllSDFs(allIPDData);

                // Get all images in our self defining field list
                foreach (Container sc in SDFs.OfType<BeginImageContent>().Select(s => s.LowestLevelContainer))
                {
                    List<Container> allContainers = new List<Container>() { sc };
                    List<ImageInfo> infoList = new List<ImageInfo>();

                    // Along with ourself, add any tiles to the list of containers
                    if (sc.Structures.Any(s => s.GetType() == typeof(BeginTile)))
                        allContainers.AddRange(sc.Structures.OfType<BeginTile>().Select(s => s.LowestLevelContainer));

                    // For each container, get image and transparency bytes
                    foreach (Container tc in allContainers)
                    {
                        ImageInfo info = new ImageInfo();
                        info.Data = tc.DirectGetStructures<ImageData>().SelectMany(s => s.Data).ToArray();

                        // If there are tiles, store offset information
                        if (tc.Structures[0].GetType() == typeof(BeginTile))
                        {
                            TilePosition tp = tc.GetStructure<TilePosition>();
                            info.XOffset = tp.XOffset;
                            info.YOffset = tp.YOffset;
                        }

                        // Add transparency data if needed
                        ImageSelfDefiningField BTM = tc.GetStructure<BeginTransparencyMask>();
                        if (BTM != null)
                            info.TransparencyMask = BTM.LowestLevelContainer.GetStructures<ImageData>().SelectMany(s => s.Data).ToArray();

                        infoList.Add(info);
                    }
                    iocaImages.Add(c, infoList);
                }
            }
            ParsedImages = iocaImages;
        }
    }
}
