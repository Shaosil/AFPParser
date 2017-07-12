using System.Collections.Generic;
using System.Linq;

namespace AFPParser.StructuredFields
{
    public class FNP : StructuredField
    {
        private static string _abbr = "FNP";
        private static string _title = "Font Position";
        private static string _desc = "Describes the common characteristics of all the characters in a font character set.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(2, Lookups.DataTypes.SBIN, "Lowercase Height"),
            new Offset(4, Lookups.DataTypes.SBIN, "Cap-M Height"),
            new Offset(6, Lookups.DataTypes.SBIN, "Max Ascender Height"),
            new Offset(8, Lookups.DataTypes.SBIN, "Max Descender Depth"),
            new Offset(10, Lookups.DataTypes.EMPTY, ""),
            new Offset(17, Lookups.DataTypes.UBIN, "Underscore Width (Units)"),
            new Offset(19, Lookups.DataTypes.UBIN, "Underscore Width (Fraction)"),
            new Offset(20, Lookups.DataTypes.SBIN, "Underscore Position")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => true;
        protected override int RepeatingGroupStart => 0;
        protected override int RepeatingGroupLength
        {
            get
            {
                int length = Data.Length;
                FNC control = LowestLevelContainer.GetStructure<FNC>();
                if (control != null)
                    length = control.Data[20];

                return length;
            }
        }
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private List<Info> _allInfo = new List<Info>();
        public IReadOnlyList<Info> AllInfo
        {
            get { return _allInfo; }
            private set
            {
                _allInfo = value.ToList();

                // Update the data stream
                Data = new byte[value.Count * 22];
                for (int i = 0; i < value.Count; i++)
                {
                    PutNumberInData(value[i].LowercaseHeight, (i * 22) + 2);
                    PutNumberInData(value[i].CapMHeight, (i * 22) + 4);
                    PutNumberInData(value[i].MaxAscenderHeight, (i * 22) + 6);
                    PutNumberInData(value[i].MaxDescenderDepth, (i * 22) + 8);
                    PutNumberInData(value[i].UnderscoreWidth, (i * 22) + 17);
                    PutNumberInData(value[i].UnderscorePosition, (i * 22) + 20);
                }
            }
        }

        public FNP(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public FNP(List<Info> allInfo) : base(Lookups.StructuredFieldID<FNP>(), 0, 0, null)
        {
            AllInfo = allInfo;
        }

        public class Info
        {
            public short LowercaseHeight { get; private set; }
            public short CapMHeight { get; private set; }
            public short MaxAscenderHeight { get; private set; }
            public short MaxDescenderDepth { get; private set; }
            public ushort UnderscoreWidth { get; private set; }
            public short UnderscorePosition { get; private set; }

            public Info(short lwcHeight, short capMHeight, short maxAscHeight, short maxDescDepth, ushort uScoreWidth, short uScorePos)
            {
                LowercaseHeight = lwcHeight;
                CapMHeight = capMHeight;
                MaxAscenderHeight = maxAscHeight;
                MaxDescenderDepth = maxDescDepth;
                UnderscoreWidth = uScoreWidth;
                UnderscorePosition = uScorePos;
            }
        }
    }
}