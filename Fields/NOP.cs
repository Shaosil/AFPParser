using System.Collections.Generic;

namespace AFPParser
{
    public class NOP : StructuredField
    {
        public override string Abbreviation { get { return "NOP"; } }

        protected override SemanticsInfo LoadSemantics()
        {
            return new SemanticsInfo()
            {
                Title = "No Operation",
                Description = "The data in the No Operation structured field is untyped and undefined. Although not recommended, custom data streams can be utilized in the data field.",
                Offsets = new List<Offset>()
                {
                    new Offset(0, Lookups.DataTypes.Char, "Unused Data")
                }
            };
        }
    }
}
