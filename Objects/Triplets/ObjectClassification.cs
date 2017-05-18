using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ObjectClassification : Triplet
	{
		private static string _desc = "Used to classify and identify object data.";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
		{
			new Offset(2, Lookups.DataTypes.EMPTY, ""),
			new Offset(3, Lookups.DataTypes.CODE, "Class")
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
			new Offset(4, Lookups.DataTypes.EMPTY, ""),
			new Offset(6, Lookups.DataTypes.BITS, "Structure Flags (NEED MULTI-BYTE BITS PARSING SUPPORT)"),
			new Offset(8, Lookups.DataTypes.CODE, "Registered Object ID"),
			new Offset(24, Lookups.DataTypes.CHAR, "Object Type Name"),
			new Offset(56, Lookups.DataTypes.CHAR, "Object Level/Version Number"),
			new Offset(64, Lookups.DataTypes.CHAR, "Company Name")
		};

		public ObjectClassification(byte[] allData) : base(allData) { }
	}
}