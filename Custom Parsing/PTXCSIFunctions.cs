using System;
using System.Linq;
using AFPParser.Properties;
using System.Collections.Generic;

namespace AFPParser
{
    public class PTXCSIFunctions
    {
        public static Dictionary<byte, SemanticsInfo> All { get; set; }

        public static void Load()
        {
            All = new Dictionary<byte, SemanticsInfo>();

            // Load the CSI offset data into memory
            List<string> csiFile = Resources.PTX_Control_Sequences.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int curIndex = 0, nextIndex = 0;
            while (curIndex >= 0)
            {
                curIndex = csiFile.IndexOf(csiFile.Skip(curIndex).FirstOrDefault(i => i[0] == ':'));
                nextIndex = csiFile.IndexOf(csiFile.Skip(curIndex + 1).FirstOrDefault(i => i[0] == ':'));

                // Build semantics info on our current section
                int take = (nextIndex >= 1 ? nextIndex : csiFile.Count) - curIndex;
                List<string> currentSection = csiFile.Skip(curIndex).Take(take).ToList();
                SemanticsInfo semantics = new SemanticsInfo();
                semantics.Description = currentSection[1];
                semantics.Offsets = Parser.LoadOffsets(currentSection.Skip(2).ToList(), semantics);

                // Insert one or two entries into our dictionary depending on if both chained/unchained function bytes are present
                List<byte> functions = currentSection[0].Substring(1).Split('-').Select(s => byte.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToList();
                foreach (byte b in functions)
                    All.Add(b, semantics);

                curIndex = nextIndex;
            }
        }
    }
}
