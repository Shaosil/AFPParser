using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class STO : PTXControlSequence
    {
        private static string _abbr = "STO";
        private static string _desc = "Set Text Orientation";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int IDegrees { get; private set; }
        public int IMinutes { get; private set; }
        public int BDegrees { get; private set; }
        public int BMinutes { get; private set; }

        public STO(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {

            // I/B Axis orientation stored in four total bytes.
            // Each two byte set has 3 parts. A (9 bits), B (6 bits), and C (1 bit)
            // A = 0 - 359 degrees
            // B = 0 - 59 minutes
            // C = Reserved, always 0

            // Loop through both sets of bytes
            for (int i = 0; i <= 2; i += 2)
            {
                // Get the bit values of the two bytes
                bool[] formattedBA = GetBitArray(Data[i], Data[i + 1]);

                // Prepare the degree and minute bits
                IEnumerable<bool> degreeBits = formattedBA.Take(9), minuteBits = formattedBA.Skip(9).Take(6);
                if (BitConverter.IsLittleEndian)
                {
                    // Flip back to little endian so the new bit arrays below will convert to the correct integer
                    degreeBits = degreeBits.Reverse();
                    minuteBits = minuteBits.Reverse();
                }

                // Parse the bits out to an int array (BitArray.CopyTo sums them up for us)
                int[] values = new int[2];
                new BitArray(degreeBits.ToArray()).CopyTo(values, 0);
                new BitArray(minuteBits.ToArray()).CopyTo(values, 1);

                // Store our values
                if (i == 0)
                {
                    IDegrees = values[0];
                    IMinutes = values[1];
                }
                else
                {
                    BDegrees = values[0];
                    BMinutes = values[1];
                }
            }
        }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"I Orientation: {IDegrees}:{IMinutes}");
            sb.AppendLine($"B Orientation: {BDegrees}:{BMinutes}");

            return sb.ToString();
        }
    }
}