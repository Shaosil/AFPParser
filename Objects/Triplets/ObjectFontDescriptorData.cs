using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ObjectFontDescriptorData : Triplet
	{
		private static string _desc = "Specifies the parameters needed to render a data-object font (non-FOCA font resources).";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.BITS, "Font Info Flags")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Non-MICR Printing|MICR Printing" },
                    { 0x01, "All fonts located anywhere in MODCA resources|All fonts located in resource group for print file" }
                }
            },
            new Offset(1, Lookups.DataTypes.CODE, "Font Tech") { Mappings = new Dictionary<byte, string>() { { 0x20, "TrueType/OpenType" } } },
            new Offset(2, Lookups.DataTypes.UBIN, "Vertical Font Size"),
            new Offset(4, Lookups.DataTypes.UBIN, "Horizontal Scale Factor"),
            new Offset(6, Lookups.DataTypes.CODE, "Character Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(8, Lookups.DataTypes.EMPTY, ""),
            new Offset(9, Lookups.DataTypes.CODE, "Encoding Environment") { Mappings = new Dictionary<byte, string>() { { 0x03, "Microsoft" } } },
            new Offset(10, Lookups.DataTypes.EMPTY, ""),
            new Offset(11, Lookups.DataTypes.CODE, "Encoding Environment") { Mappings = new Dictionary<byte, string>() { { 0x01, "Unicode" } } }
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public ObjectFontDescriptorData(byte id, byte[] data) : base(id, data) { }
	}
}