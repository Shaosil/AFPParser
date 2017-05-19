using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class CPI : StructuredField
    {
        private static string _abbr = "CPI";
        private static string _title = "Code Page Index";
        private static string _desc = "Associates one or more character IDs with code points.";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => true;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;

        public CPI(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            // CPI will have one or more repeating groups. The length of each is fixed, but found in the CPC field
            //StructuredField CPC = ParentList.FirstOrDefault(a => a.GetType() == typeof(CPC));
            //if (CPC == null)
            //    return "ERROR: Code Page Control field not found in file! Could not determine repeating group length.";

            //int standardLength = CPC.Data[9];

            return sb.ToString();
        }
    }
}