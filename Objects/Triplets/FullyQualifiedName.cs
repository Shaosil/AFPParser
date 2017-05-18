using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class FullyQualifiedName : Triplet
	{
		private static string _desc = "Enables referencing of objects using Global Identifiers (GIDs).";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Type")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x01, "Replace first GID name" },
                    { 0x07, "Font family name" },
                    { 0x08, "Font typeface name" },
                    { 0x09, "MODCA Resource" },
                    { 0x0A, "Begin resource group" },
                    { 0x0B, "Attribute GID" },
                    { 0x0C, "Process Element GID" },
                    { 0x0D, "Begin Page Group Reference" },
                    { 0x11, "Media Type" },
                    { 0x12, "Media Destination" },
                    { 0x41, "Color Management Resource" },
                    { 0x6E, "Data object Font Base Font Identifier" },
                    { 0x7E, "Data object Font Linked Font Identifier" },
                    { 0x83, "Begin Document" },
                    { 0x84, "Resource Object" },
                    { 0x85, "Code Page Name" },
                    { 0x86, "Font Character Set Name" },
                    { 0x87, "Begin Page" },
                    { 0x8D, "Begin Medium Map" },
                    { 0x8E, "Coded Font Name" },
                    { 0x98, "Begin Document Index" },
                    { 0xB0, "Begin Overlay" },
                    { 0xBE, "Data Object Internal Resource" },
                    { 0xCA, "Index Element GID" },
                    { 0xCE, "Other Object Data" },
                    { 0xDE, "Data Object External Resource" }
                }
            },
            new Offset(1, Lookups.DataTypes.CODE, "Format")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Character string" },
                    { 0x10, "Object Identifier (OID)" },
                    { 0x20, "URL" }
                }
            },
            new Offset(2, Lookups.DataTypes.CODE, "Name")
        };

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

		public FullyQualifiedName(byte[] allData) : base(allData) { }
	}
}