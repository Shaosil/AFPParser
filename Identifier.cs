using System;
using System.Linq;
using System.Diagnostics;
using AFPParser.Properties;
using System.Collections.Generic;

namespace AFPParser
{
    [DebuggerDisplay("{Abbreviation} - {HexCode}")]
    public class Identifier
    {
        public static Dictionary<string, Identifier> All { get; set; }

        public string HexCode { get; set; }
        public string Abbreviation { get; set; }
        public SemanticsInfo Semantics { get; set; }

        public Identifier(string title, string hexCode, string abbreviation)
        {
            Semantics = new SemanticsInfo() { Title = title };
            HexCode = hexCode;
            Abbreviation = abbreviation;
        }

        public static void Load()
        {
            // Load the list of basic Hex/Abbreviation/Description into memory
            List<string> identifierFileLines = Resources.Field_Identifiers.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            All = new Dictionary<string, Identifier>();

            foreach (string identifier in identifierFileLines)
            {
                List<string> values = identifier.Split(',').ToList();
                All.Add(values[1], new Identifier(abbreviation: values[0], hexCode: values[1], title: values[2]));
            }

            // Load the list of identifier semantics into memory
            List<string> stringIdentifierInfos = Resources.Identifier_Info.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int curIdx = 0, nextIdx = 0;
            while (curIdx >= 0)
            {
                curIdx = stringIdentifierInfos.IndexOf(stringIdentifierInfos.Skip(curIdx).FirstOrDefault(i => i[0] == ':'));
                nextIdx = stringIdentifierInfos.IndexOf(stringIdentifierInfos.Skip(curIdx + 1).FirstOrDefault(i => i[0] == ':'));

                // Find identifier with this hex code
                Identifier target = All[stringIdentifierInfos[curIdx].Substring(1, 6)];

                // Set description based on constant line number
                target.Semantics.Description = stringIdentifierInfos[curIdx + 1];

                // Get the list of lines that are offsets
                List<string> offsetRows = new List<string>();
                for (int i = curIdx + 2; i < stringIdentifierInfos.Count; i++)
                {
                    if (stringIdentifierInfos[i][0] == ':') break;
                    offsetRows.Add(stringIdentifierInfos[i]);
                }

                target.Semantics.Offsets = Offset.Load(offsetRows, target.Semantics);

                curIdx = nextIdx;
            }
        }
    }
}
