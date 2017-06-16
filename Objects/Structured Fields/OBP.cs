using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class OBP : StructuredField
    {
        private static string _abbr = "OBP";
        private static string _title = "Object Area Position";
        private static string _desc = "The Object Area Position structured field specifies the origin and orientation of the object area, and the origin and orientation of the object content within the object area.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "OBP ID"),
            new Offset(1, Lookups.DataTypes.EMPTY, ""), // RGLength, always 23, and always only one group...
            new Offset(2, Lookups.DataTypes.SBIN, "X Axis Area Origin"),
            new Offset(5, Lookups.DataTypes.SBIN, "Y Axis Area Origin"),
            new Offset(8, Lookups.DataTypes.CODE, "X Axis Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(10, Lookups.DataTypes.CODE, "Y Axis Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(12, Lookups.DataTypes.EMPTY, ""),
            new Offset(13, Lookups.DataTypes.SBIN, "X Axis Content Origin"),
            new Offset(16, Lookups.DataTypes.SBIN, "Y Axis Content Origin"),
            new Offset(19, Lookups.DataTypes.CODE, "X Axis Content Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(21, Lookups.DataTypes.CODE, "Y Axis Content Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(23, Lookups.DataTypes.CODE, "Reference Coordinate System")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Defined in IPS field" },
                    { 0x01, "Standard origin" }
                }
            }
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int OBPID { get; private set; }
        public int XOrigin { get; private set; }
        public int YOrigin { get; private set; }
        public int XContentOrigin { get; private set; }
        public int YContentOrigin { get; private set; }

        public OBP(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        public override void ParseData()
        {
            OBPID = Data[0];
            XOrigin = (int)GetNumericValue(GetSectionedData(2, 3), true);
            YOrigin = (int)GetNumericValue(GetSectionedData(5, 3), true);
            XContentOrigin = (int)GetNumericValue(GetSectionedData(13, 3), true);
            YContentOrigin = (int)GetNumericValue(GetSectionedData(16, 3), true);
        }
    }
}