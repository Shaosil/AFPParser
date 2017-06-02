using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser
{
    public abstract class PTXControlSequence : DataStructure
    {
        // Properties which must be implemented by individual control sequences
        public abstract string Abbreviation { get; }
        protected abstract string Description { get; }
        protected abstract List<Offset> Offsets { get; }
        protected override string StructureName => "Control Sequence";

        public PTXControlSequence(byte[] allData) : base(allData[0], allData[1].ToString("X2"), 2)
        {
            // Control sequences never have repeating groups
            Semantics = new SemanticsInfo(SpacedClassName, Description, false, 0, Offsets);

            // Set data starting at offset 2
            for (int i = 0; i < Data.Length; i++)
                Data[i] = allData[2 + i];
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            // Use description instead of title
            sb.AppendLine($"{Semantics.Description} ({StructureName} 0x{ID})");
            sb.Append(GetOffsetDescriptions());

            return sb.ToString();
        }

        public override void ParseData()
        {
            // TODO: Remove this if and when each control sequence parses the data in their own way
        }
    }
}