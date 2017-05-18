using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ObjectFunctionSetSpecification : Triplet
	{
		private static string _desc = "Identifies the type of object enveloped by the BRS and ERS fields.";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
		{
			new Offset(2, Lookups.DataTypes.CODE, "Object Type")
			{
				Mappings = new Dictionary<byte, string>()
				{
					{ 0x03, "Graphics" },
					{ 0x05, "Bar Code" },
					{ 0x06, "Image" },
					{ 0x40, "Font Character Set" },
					{ 0x41, "Code Page" },
					{ 0x42, "Coded Font" },
					{ 0x92, "Object Container" },
					{ 0xA8, "Document" },
					{ 0xFB, "Page Segment" },
					{ 0xFC, "Overlay" },
					{ 0xFD, "Reserved" },
					{ 0xFE, "Form Map" }
				}
			},
			new Offset(3, Lookups.DataTypes.CODE, "")
		};

		public ObjectFunctionSetSpecification(byte[] allData) : base(allData) { }
	}
}