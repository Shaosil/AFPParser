using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFPParser.Triplets
{
    public class ObjectClassification : Triplet
    {
        private static string _desc = "Used to classify and identify object data.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(1, Lookups.DataTypes.CODE, "Class")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x01, "Time invariant paginated presentation object" },
                    { 0x10, "Time variant" },
                    { 0x20, "Executable program" },
                    { 0x30, "Set up file" },
                    { 0x40, "Secondary resource" },
                    { 0x41, "Data object font" }
                }
            },
            new Offset(2, Lookups.DataTypes.EMPTY, ""),
            new Offset(4, Lookups.DataTypes.EMPTY, "Structure Flags (CUSTOM PARSED)"),
            new Offset(6, Lookups.DataTypes.EMPTY, "Registered Object ID (CUSTOM PARSED)"),
            new Offset(22, Lookups.DataTypes.CHAR, "Object Type Name"),
            new Offset(54, Lookups.DataTypes.CHAR, "Object Level/Version Number"),
            new Offset(62, Lookups.DataTypes.CHAR, "Company Name")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public string RegisteredObjectID { get; private set; }

        public ObjectClassification(byte id, byte[] data) : base(id, data) { }

        public override void ParseData()
        {
            // Only take the first nine bytes - the rest are padded zeroes
            RegisteredObjectID = BitConverter.ToString(GetSectionedData(6, 9)).Replace("-", "");
        }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            StringBuilder sb = new StringBuilder();

            switch (oSet.StartingIndex)
            {
                // Structure Flags
                case 4:
                    sb.AppendLine("Structure Flags:");
                    // Combine all of our bools into a single string for easy comparison on those doubled up bit checks
                    string bitArray = string.Join("", GetBitArray(sectionedData).Select(b => b ? "1" : "0"));
                    string objContainer = bitArray.Substring(0, 2);
                    string objEnvGroup = bitArray.Substring(2, 2);
                    string objContainerDataFields = bitArray.Substring(4, 2);

                    // Bits 0-1 - Object Container
                    sb.Append("* ");
                    if (objContainer == "00") sb.AppendLine("Reserved");
                    else if (objContainer == "01") sb.AppendLine("Object data not carried in a MO:DCA object container");
                    else if (objContainer == "10") sb.AppendLine("Container structure of object data is unknown");
                    else sb.AppendLine("Object data is carried in a MO:DCA object");

                    // Bits 2-3 - Object Environment Group
                    sb.Append("* ");
                    if (objEnvGroup == "00") sb.AppendLine("Reserved");
                    else if (objEnvGroup == "01") sb.AppendLine("Object container does not include an OEG");
                    else if (objEnvGroup == "10") sb.AppendLine("It is not known whether the object structure includes an OEG");
                    else sb.AppendLine("Object container includes an OEG for the object data");

                    // Bits 4-5 - Object Container Data
                    sb.Append("* ");
                    if (objContainerDataFields == "00") sb.AppendLine("Reserved");
                    else if (objContainerDataFields == "01") sb.AppendLine("Object data is not carried in OCD structured fields");
                    else if (objContainerDataFields == "10") sb.AppendLine("It is not known whether object data is carried in OCD structured fields");
                    else sb.AppendLine("Object data is carried in OCD structured fields");
                    break;

                // Registered object ID
                case 6:
                    sb.Append("Registered Object ID: ");

                    if (Lookups.OIDs.ContainsKey(RegisteredObjectID))
                        sb.AppendLine(Lookups.OIDs[RegisteredObjectID]);
                    else
                        sb.AppendLine("UNKNOWN");
                    break;

                default:
                    return base.GetSingleOffsetDescription(oSet, sectionedData);
            }

            return sb.ToString();
        }
    }
}