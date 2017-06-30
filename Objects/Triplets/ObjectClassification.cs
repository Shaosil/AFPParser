using System.Collections.Generic;

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
			new Offset(4, Lookups.DataTypes.EMPTY, "Structure Flags (NEED MULTI-BYTE BITS PARSING SUPPORT)"),
			new Offset(6, Lookups.DataTypes.CODE, "Registered Object ID"),
			new Offset(22, Lookups.DataTypes.CHAR, "Object Type Name"),
			new Offset(54, Lookups.DataTypes.CHAR, "Object Level/Version Number"),
			new Offset(62, Lookups.DataTypes.CHAR, "Company Name")
		};

		public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

		public ObjectClassification(string id, byte[] introcuder, byte[] data) : base(id, introcuder, data) { }
	}
}