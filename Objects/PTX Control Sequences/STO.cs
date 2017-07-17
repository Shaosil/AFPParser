using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private ushort _iDegrees;
        private ushort _bDegrees;
        public ushort IDegrees
        {
            get { return _iDegrees; }
            private set
            {
                _iDegrees = value;

                // Update data stream - see ParseData() for info on bits
                PutNumberInData((ushort)(value << 7), 0);
            }
        }
        public ushort BDegrees
        {
            get { return _bDegrees; }
            private set
            {
                _bDegrees = value;

                // Update data stream - see ParseData() for info on bits
                PutNumberInData((ushort)(value << 7), 2);
            }
        }

        public STO(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }

        public STO(CommonMappings.eRotations iOrient, CommonMappings.eRotations bOrient, bool hasPrefix, bool isChained)
            : base(Lookups.PTXControlSequenceID<STO>(isChained), hasPrefix, null)
        {
            Data = new byte[4];

            // The enum values need to be multiplied by 2 to reflect the actual degree values
            IDegrees = (ushort)((ushort)iOrient * 2);
            BDegrees = (ushort)((ushort)bOrient * 2);
        }

        public override void ParseData()
        {
            // I/B Axis orientation stored in four total bytes.
            // Each two byte set has 3 parts. A (9 bits), B (6 bits), and C (1 bit)
            // A = 0 - 359 degrees
            // B = 0 - 59 minutes
            // C = Reserved, always 0

            // Loop through both sets of bytes, only grabbing degrees
            for (int i = 0; i <= 2; i += 2)
            {
                // Get the bit values of the two bytes
                bool[] formattedBA = GetBitArray(Data[i], Data[i + 1]);

                // Prepare the degree and minute bits
                IEnumerable<bool> degreeBits = formattedBA.Take(9); ;
                if (BitConverter.IsLittleEndian)
                {
                    // Flip back to little endian so the new bit arrays below will convert to the correct integer
                    degreeBits = degreeBits.Reverse();
                }

                // Parse the bits out to an int array (BitArray.CopyTo sums them up for us)
                int[] value = new int[1];
                new BitArray(degreeBits.ToArray()).CopyTo(value, 0);
                ushort result = (ushort)value[0];

                // Store our values
                if (i == 0) _iDegrees = result;
                else _bDegrees = result;
            }
        }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"I Orientation: {IDegrees}");
            sb.AppendLine($"B Orientation: {BDegrees}");

            return sb.ToString();
        }
    }
}