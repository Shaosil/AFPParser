using System.Text;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MMC : StructuredField
	{
		private static string _abbr = "MMC";
		private static string _title = "Medium Modification Control";
		private static string _desc = "Specifies the medium modifications to be applied for a copy subgroup specified in the MCC structured field.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "ID"),
            new Offset(1, Lookups.DataTypes.EMPTY, ""),
            new Offset(2, Lookups.DataTypes.EMPTY, "KEYWORDS")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MMC(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            if (oSet.StartingIndex < 2)
                return base.GetSingleOffsetDescription(oSet, sectionedData);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Keywords:");

            // Each keyword is two bytes (ID and Value).
            for (int i = oSet.StartingIndex; i < Data.Length; i += 2)
            {
                byte keyword = Data[i];
                byte value = Data[i + 1];

                if (Lookups.MMCKeywords.ContainsKey(keyword))
                {
                    // Integer unless mappings exist in special cases
                    switch (keyword)
                    {
                        // Todo: Special case

                        default:
                            sb.AppendLine($"* {Lookups.MMCKeywords[keyword]}: {value}");
                            break;
                    }
                }
                else
                    sb.AppendLine($"* Unknown keyword ({keyword.ToString("X")}): {value.ToString("X")}");
            }

            return sb.ToString();
        }
    }
}