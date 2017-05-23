using System.Text;
using System.Collections.Generic;
using System.Linq;

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

        //protected override string GetOffsetDescriptions()
        //{
        //    StringBuilder sb = new StringBuilder();
            
        //    // Loop through each repeating group of patterns
        //    int curIndex = 0;
        //    do
        //    {
        //        long groupLength = GetNumericValue(GetSectionedData(0, 4), false);
        //        uint checksum = (uint)GetNumericValue(GetSectionedData(4, 4), false);

        //        // Out of curiosity, verify the checksum WE calculate matches up with the one stored!


        //        int idLength = (int)GetNumericValue(GetSectionedData(8, 2), false);

        //        if (idLength > 2)
        //        {
        //            string id = GetReadableDataPiece(10, idLength);
        //        }

        //    } while (curIndex < Data.Length);

        //    return sb.ToString();
        //}

        private uint GetChecksum(byte[] objectData)
        {
            // Calculates the checksum for the tech object data section of a font pattern. The algorithm is as follows:
            /*
                Checksum is an array of 4 unsigned bytes
                The first four bytes are placed into the array from index 0 (most significant) to index 3 (least significant)
                Remaining bytes are added to the array starting back at 0 and incrementing/looping around

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