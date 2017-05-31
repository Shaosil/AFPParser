using System;
using System.Text;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class FNG : StructuredField
    {
        private static string _abbr = "FNG";
        private static string _title = "Font Patterns";
        private static string _desc = "Carries the character shape data (raster patterns or outline data) for a font character set.";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => true;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;

        public FNG(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            // Outline and raster data are handled much differently. If we have a patterns map, it's raster
            bool isRaster = LowestLevelContainer.GetField<FNM>() != null;

            if (isRaster)
                return GetRasterData();
            else
                return GetOutlineData();
        }

        private string GetOutlineData()
        {
            StringBuilder sb = new StringBuilder();

            // Loop through each repeating group of patterns
            FNC refFNC = LowestLevelContainer.GetField<FNC>();
            int curIndex = 0;
            do
            {
                long groupLength = GetNumericValue(GetSectionedData(0, 4), false);
                uint checksum = (uint)GetNumericValue(GetSectionedData(4, 4), false);
                sb.AppendLine($"Checksum: {checksum}");

                int idLength = (int)GetNumericValue(GetSectionedData(8, 2), false);
                string id = idLength > 2 ? GetReadableDataPiece(10, idLength - 2) : string.Empty;
                sb.AppendLine($"ID: {id}");

                // If FNC's pattern tech identifier isn't PFB Type 1, we should have a description
                int descriptorLength = !string.IsNullOrEmpty(id) && refFNC.PatternTech != FNC.ePatternTech.PFBType1
                    ? (int)GetNumericValue(GetSectionedData(8 + idLength, 2), false) : 0;
                byte[] descriptor = descriptorLength > 2 ? GetSectionedData(8 + idLength + 2, descriptorLength - 2) : new byte[0];
                
                // Object descriptor
                if (descriptor.Length > 0)
                {
                    switch (descriptor[0])
                    {
                        case 1: // CMap file
                            string precedenceCode = descriptor[1] == 0 ? "Primary" : "Auxiliary";
                            string linkageCode = descriptor[2] == 0 ? "Linked" : "Unlinked";
                            string writingDirectionCode = descriptor[3] == 1 ? "Horizontal" : descriptor[3] == 2 ? "Vertical" : "Vertical and Horizontal";
                            string GCSGID = GetReadableDataPiece(8 + idLength + 4, 2);
                            string CPSGID = GetReadableDataPiece(8 + idLength + 6, 2);

                            sb.AppendLine($"Precedence Code: {precedenceCode}");
                            sb.AppendLine($"Linkage Code: {linkageCode}");
                            sb.AppendLine($"Writing Direction Code: {writingDirectionCode}");
                            sb.AppendLine($"GCSGID: {GCSGID}");
                            sb.AppendLine($"CPSGID: {CPSGID}");
                            break;

                        case 5: // CID file
                            precedenceCode = descriptor[1] == 0 ? "Primary" : "Auxiliary";
                            ushort maxV = (ushort)GetNumericValue(GetSectionedData(8 + idLength + 2, 2), false);
                            ushort maxW = (ushort)GetNumericValue(GetSectionedData(8 + idLength + 4, 2), false);

                            sb.AppendLine($"Precedence Code: {precedenceCode}");
                            sb.AppendLine($"Max V(y) value: {maxV}");
                            sb.AppendLine($"Max W(y) value: {maxW}");
                            break;

                        case 6: // PFB file
                        case 7: // AFM file
                        case 8: // Filename map file
                            sb.AppendLine("No descriptor info provided.");
                            break;
                    }

                    // Object data
                    int dataStartIndex = 8 + idLength + descriptorLength;
                    if (Data.Length > dataStartIndex)
                    {
                        byte[] objData = GetSectionedData(dataStartIndex, Data.Length - dataStartIndex);
                        sb.AppendLine($"Raw Data: {BitConverter.ToString(objData).Replace("-", " ")}");

                        // Out of curiosity, verify the checksum WE calculate matches up with the one stored! (No current use)
                        uint testChecksum = GetChecksum(objData);
                    }
                }

            } while (curIndex < Data.Length);

            return sb.ToString();
        }

        private string GetRasterData()
        {
            StringBuilder sb = new StringBuilder();
            FNM patternMap = LowestLevelContainer.GetField<FNM>();

            // Write out each character pattern
            for (int i = 0; i < patternMap.AllPatternData.Count; i++)
            {
                if (patternMap.AllPatternData[i].BoxWidth <= 0) continue;

                // Take up to the next offset (or end of data)
                int bytesToTake = (i == patternMap.AllPatternData.Count - 1 ? Data.Length : (int)patternMap.AllPatternData[i + 1].DataOffset)
                    - (int)patternMap.AllPatternData[i].DataOffset;

                // Since bytes have to have 8 bits, the number of bytes can be easily calculated by knowing the bit width
                int numBytes = (int)Math.Ceiling((patternMap.AllPatternData[i].BoxWidth + 1 )/ 8.0);

                // Get a bit array of n bytes, and print out the pixels line by line
                byte[] sectionBytes = GetSectionedData((int)patternMap.AllPatternData[i].DataOffset, bytesToTake);
                bool[] sectionBits = GetBitArray(sectionBytes);
                for (int b = 0; b < sectionBits.Length; b++)
                {
                    sb.Append(sectionBits[b] ? "#" : ".");
                    if (((b + 1) / 8.0) % numBytes == 0) sb.AppendLine();
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private uint GetChecksum(byte[] objectData)
        {
            // Calculates the checksum for the tech object data section of a font pattern. The algorithm is as follows:
            /*
                Start with an array of 4 unsigned bytes
                The first four bytes of data are placed into the array
                Remaining bytes are added to the array values starting back at 0 and incrementing/looping around

                Checksum is the unsigned integer resolved from the resulting 4 bytes
            */

            byte[] checksumArray = new byte[4] { 0, 0, 0, 0 };
            int curArrayIndex = 0;
            for (int i = 0; i < objectData.Length; i++)
            {
                checksumArray[curArrayIndex++] += objectData[i];
                if (curArrayIndex == 4) curArrayIndex = 0;
            }
            
            return (uint)GetNumericValue(checksumArray, false);
        }
    }
}