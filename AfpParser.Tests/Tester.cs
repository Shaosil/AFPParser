using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace AFPParser.Tests
{
    [TestClass]
    public class Tester
    {
        [DebuggerDisplay("{abbr}")]
        class Identifier
        {
            public string abbr;
            public string title;

            public Identifier(string v1, string v2)
            {
                abbr = v1;
                title = v2;
            }
        }

        [TestMethod]
        public void Misc()
        {
            // Populate identifier list
            Dictionary<string, Identifier> identifiers = GetIdentifiers();

            // Get a list of hex codes to types
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, Identifier> kvp in identifiers)
            {
                sb.AppendLine($"{{ \"{kvp.Key}\", typeof({kvp.Value.abbr}) }}, // {kvp.Value.title}");
            }
            string end = sb.ToString();

            //// Get the raw offset information
            //GetOffsets(identifiers);

            //// Create a code file for each
            //CreateCodeFiles(identifiers);

            //// Store identifiers which have no description or offsets
            //List<Identifier> noDescs = identifiers.Values.Where(v => string.IsNullOrEmpty(v.desc)).ToList();
            //List<Identifier> noOSets = identifiers.Values.Where(v => v.oSets.Count == 0).ToList();
        }

        private static Dictionary<string, Identifier> GetIdentifiers()
        {
            // Load all identifier info into a tuple (HEX, ABBR, TITLE)
            Dictionary<string, Identifier> all = new Dictionary<string, Identifier>();
            string[] lines = File.ReadAllLines(@"C:\Users\X009059\Documents\Visual Studio\AFPParser\Lookups\Field Identifiers.txt");
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                all.Add(values[1], new Identifier(values[0], values[2]));
            }

            return all;
        }

    //    private static void CreateCodeFiles(Dictionary<string, Identifier> identifiers)
    //    {
    //        foreach (KeyValuePair<string, Identifier> kvp in identifiers)
    //        {
    //            if (!new[] { "MCF" }.Contains(kvp.Value.abbr)) continue;

    //            Func<int, string> t = (int numTabs) => { return new string('\t', numTabs); };
    //            StringBuilder sb = new StringBuilder();
    //            sb.AppendLine("using System.Collections.Generic;");
    //            sb.AppendLine();
    //            sb.AppendLine("namespace AFPParser.StructuredFields");
    //            sb.AppendLine("{");
    //            sb.AppendLine($"{t(1)}public class {kvp.Value.abbr} : StructuredField");
    //            sb.AppendLine($"{t(1)}{{");
    //            sb.AppendLine($"{t(2)}private static string _abbr = \"{kvp.Value.abbr}\";");
    //            sb.AppendLine($"{t(2)}private static string _title = \"{kvp.Value.title}\";");
    //            sb.AppendLine($"{t(2)}private static string _desc = \"{kvp.Value.desc}\";");
    //            sb.Append($"{t(2)}private static List<Offset> _oSets = new List<Offset>()");

    //            // Only do offsets if there are some to do
    //            if (kvp.Value.oSets.Any())
    //            {
    //                sb.AppendLine();
    //                sb.AppendLine($"{t(2)}{{");

    //                foreach (string o in kvp.Value.oSets)
    //                {
    //                    string[] sections = o.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
    //                    if (sections.Length == 1 || !new[] { "RSTART", "CUSTOM" }.Contains(sections[1]))
    //                    {
    //                        string startIndex = sections[0];
    //                        string dataType = sections.Length > 1 ? sections[1] : "EMPTY";
    //                        string description = sections.Length > 2 ? sections[2] : string.Empty;

    //                        sb.Append($"{t(3)}new Offset({startIndex}, Lookups.DataTypes.{dataType}, \"{description}\")");

    //                        // If there are mappings, insert them here
    //                        if (sections.Length > 3)
    //                        {
    //                            string[] mappings = sections[3].Split(',');

    //                            sb.AppendLine();
    //                            sb.AppendLine($"{t(3)}{{");
    //                            sb.AppendLine($"{t(4)}Mappings = new Dictionary<byte, string>()");
    //                            sb.AppendLine($"{t(4)}{{");
    //                            foreach (string mapping in mappings)
    //                            {
    //                                string[] mappingSections = mapping.Split('-');
                                    
    //                                // If it's a range, loop
    //                                if (mappingSections[0][0] == 'R')
    //                                {
    //                                    byte first = byte.Parse(mappingSections[0].Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
    //                                    byte second = byte.Parse(mappingSections[0].Substring(3, 2), System.Globalization.NumberStyles.HexNumber);

    //                                    for (byte i = first; i <= second; i++)
    //                                    {
    //                                        sb.Append($"{t(5)}{{ 0x{i.ToString("X")}, \"{mappingSections[1]}\" }}");

    //                                        if (i < second)
    //                                            sb.AppendLine(",");
    //                                        else
    //                                            sb.AppendLine();
    //                                    }
    //                                }
    //                                else
    //                                    sb.Append($"{t(5)}{{ 0x{mappingSections[0]}, \"{mappingSections[1]}\" }}");

    //                                if (mapping != mappings.Last())
    //                                    sb.AppendLine(",");
    //                                else
    //                                    sb.AppendLine();
    //                            }
    //                            sb.AppendLine($"{t(4)}}}");
    //                            sb.AppendLine($"{t(3)}}}");
    //                        }

    //                        if (o != kvp.Value.oSets.Last())
    //                            sb.AppendLine(",");
    //                        else
    //                            sb.AppendLine();
    //                    }
    //                }

    //                sb.AppendLine($"{t(2)}}};");
    //            }
    //            else
    //                sb.AppendLine(";");

    //            sb.AppendLine();

    //            sb.AppendLine($"{t(2)}public override string Abbreviation => _abbr;");
    //            sb.AppendLine($"{t(2)}public override string Title => _title;");
    //            sb.AppendLine($"{t(2)}protected override string Description => _desc;");
    //            sb.AppendLine($"{t(2)}protected override bool IsRepeatingGroup => false;");
    //            sb.AppendLine($"{t(2)}protected override int RepeatingGroupStart => 0;");
    //            sb.AppendLine($"{t(2)}protected override List<Offset> Offsets => _oSets;");

    //            sb.AppendLine();

    //            sb.AppendLine($"{t(2)}public {kvp.Value.abbr}(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) {{ }}");
    //            sb.AppendLine($"{t(1)}}}");
    //            sb.Append("}");

    //            // Create the file
    //            File.WriteAllText($@"C:\Users\X009059\Documents\Visual Studio\AFPParser\Objects\Structured Fields\{kvp.Value.abbr}.cs", sb.ToString());
    //        }
    //    }
    }
}