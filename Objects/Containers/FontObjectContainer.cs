using System;
using System.Linq;
using AFPParser.StructuredFields;
using System.Collections.Generic;

namespace AFPParser.Containers
{
    public class FontObjectContainer : Container
    {
        // Store each character pattern as a two dimensional array of bools paired to a hex code
        public IReadOnlyDictionary<FNI.Info, bool[,]> RasterPatterns { get; private set; }

        public override void ParseContainerData()
        {
            // If we have a pattern map, gather raster data
            FNM patternsMap = GetStructure<FNM>();
            if (patternsMap != null)
            {
                FNI firstFNI = GetStructure<FNI>();
                Dictionary<FNI.Info, bool[,]> rasterPatterns = new Dictionary<FNI.Info, bool[,]>();
                byte[] allFNGData = GetStructures<FNG>().SelectMany(f => f.Data).ToArray();
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
                    rasterPatterns.Add(firstFNI.InfoList.First(fni => fni.FNMIndex == i), curPattern);
                }

                RasterPatterns = rasterPatterns;
            }
        }
    }
}