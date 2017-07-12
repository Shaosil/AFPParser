using AFPParser.Containers;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    [ContainerType(typeof(FontObjectContainer))]
    public class BFN : StructuredField
    {
        private static string _abbr = "BFN";
        private static string _title = "Begin Font";
        private static string _desc = "Begins the font character set object.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Font Character Set Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private string _objectName;
        public string ObjectName
        {
            get { return _objectName; }
            set
            {
                _objectName = value.Trim();
                PutStringInData(_objectName, 0, 8);
            }
        }

        public BFN(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public BFN(string name = "") : base(Lookups.StructuredFieldID<BFN>(), 0, 0, null)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Data = new byte[8];
                ObjectName = name;
            }

        }

        public override void ParseData()
        {
            base.ParseData();

            _objectName = GetReadableDataPiece(0, 8);
        }
    }
}