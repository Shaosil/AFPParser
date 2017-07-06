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
            new Offset(6, Lookups.DataTypes.CODE, "Registered Object ID"),
            new Offset(22, Lookups.DataTypes.CHAR, "Object Type Name"),
            new Offset(54, Lookups.DataTypes.CHAR, "Object Level/Version Number"),
            new Offset(62, Lookups.DataTypes.CHAR, "Company Name")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public ObjectClassification(byte id, byte[] data) : base(id, data) { }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            if (oSet.StartingIndex != 4)
                return base.GetSingleOffsetDescription(oSet, sectionedData);

            StringBuilder sb = new StringBuilder();

            // Combine all of our bools into a single string for easy comparison on those doubled up bit checks
            string bitArray = string.Join("", GetBitArray(sectionedData).Select(b => b ? "1" : "0"));
            string objContainer = bitArray.Substring(0, 2);
            string objEnvGroup = bitArray.Substring(2, 2);
            string objContainerDataFields = bitArray.Substring(4, 2);

            // Bits 0-1 - Object Container
            sb.AppendLine("Object Container (BOC/EOC):");
            sb.Append("* ");
            if (objContainer == "00") sb.AppendLine("Reserved");
            else if (objContainer == "01") sb.AppendLine("Object data not carried in a MO:DCA object container");
            else if (objContainer == "10") sb.AppendLine("Container structure of object data is unknown");
            else sb.AppendLine("Object data is carried in a MO:DCA object");

            // Bits 2-3 - Object Environment Group
            sb.AppendLine("Object Environment Group (OEG):");
            sb.Append("* ");
            if (objEnvGroup == "00") sb.AppendLine("Reserved");
            else if (objEnvGroup == "01") sb.AppendLine("Object container does not include an OEG");
            else if (objEnvGroup == "10") sb.AppendLine("It is not known whether the object structure includes an OEG");
            else sb.AppendLine("Object container includes an OEG for the object data");

            // Bits 4-5 - Object Container Data
            sb.AppendLine("Object Container Data (OCD) Structured Fields:");
            sb.Append("* ");
            if (objContainerDataFields == "00") sb.AppendLine("Reserved");
            else if (objContainerDataFields == "01") sb.AppendLine("Object data is not carried in OCD structured fields");
            else if (objContainerDataFields == "10") sb.AppendLine("It is not known whether object data is carried in OCD structured fields");
            else sb.AppendLine("Object data is carried in OCD structured fields");

            return sb.ToString();
        }
    }
}