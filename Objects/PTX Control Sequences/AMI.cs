using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class AMI : PTXControlSequence
    {
        private static string _abbr = "AMI";
        private static string _desc = "Absolute Move Inline";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Displacement")
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private short _displacement;
        public short Displacement
        {
            get { return _displacement; }
            private set
            {
                _displacement = value;
                PutNumberInData(value, 0);
            }
        }

        public AMI(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }

        public AMI(short displacement, bool hasPrefix, bool isChained) : base(Lookups.PTXControlSequenceID<AMI>(isChained), hasPrefix, null)
        {
            Data = new byte[2];
            Displacement = displacement;
        }

        public override void ParseData()
        {
            _displacement = GetNumericValueFromData<short>(0, 2);
        }
    }
}