using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class SIA : PTXControlSequence
    {
        private static string _abbr = "SIA";
        private static string _desc = "Set Intercharacter Adjustment";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Adjustment"),
            new Offset(2, Lookups.DataTypes.CODE, "Direction")
            {
                Mappings = new Dictionary<byte, string>() { { 0x00, "Increment" }, { 0x01, "Decrement" } }
            }
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public enum eDirection { Increment = 0, Decrement = 1 };
        private short _adjustment;
        private eDirection _direction;

        public short Adjustment
        {
            get { return _adjustment; }
            private set
            {
                _adjustment = value;
                PutNumberInData(value, 0);
            }
        }
        public eDirection Direction
        {
            get { return _direction; }
            private set
            {
                _direction = value;
                Data[2] = (byte)value;
            }
        }

        public SIA(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }

        public SIA(short adjustment, bool hasPrefix, bool isChained, eDirection direction = eDirection.Increment)
            : base(Lookups.PTXControlSequenceID<SIA>(isChained), hasPrefix, null)
        {
            Data = new byte[3];
            Adjustment = adjustment;
            Direction = direction;
        }

        public override void ParseData()
        {
            _adjustment = GetNumericValueFromData<short>(0, 2);
            _direction = (eDirection)Data[2];
        }
    }
}