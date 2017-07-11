using System.Collections.Generic;
using System.Linq;

namespace AFPParser.StructuredFields
{
    public class IDD : StructuredField
    {
        private static string _abbr = "IDD";
        private static string _title = "Image Data Descriptor";
        private static string _desc = "Carries the parameters that define the size and resolution of the Image Presentation Space, and the parameters required to interpret the Image Segment.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Unit Base") { Mappings = CommonMappings.UnitBase },
            new Offset(1, Lookups.DataTypes.UBIN, "Horizontal Resolution"),
            new Offset(3, Lookups.DataTypes.UBIN, "Vertical Resolution"),
            new Offset(5, Lookups.DataTypes.UBIN, "Horizontal Size"),
            new Offset(7, Lookups.DataTypes.UBIN, "Vertical Size"),
            new Offset(9, Lookups.DataTypes.IMAGESDFS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public Converters.eMeasurement BaseUnit { get; private set; }
        public ushort HResolution { get; private set; }
        public ushort VResolution { get; private set; }
        public ushort XSize { get; set; }
        public ushort YSize { get; set; }
        public IReadOnlyList<ImageSelfDefiningField> SDFs { get; private set; }

        public IDD(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            BaseUnit = Converters.GetBaseUnit(Data[0]);
            HResolution = GetNumericValueFromData<ushort>(1, 2);
            VResolution = GetNumericValueFromData<ushort>(3, 2);
            XSize = GetNumericValueFromData<ushort>(5, 2);
            YSize = GetNumericValueFromData<ushort>(7, 2);

            // Since there is one IDD per image object container, it is efficient enough to load them at this point
            SDFs = ImageSelfDefiningField.GetAllSDFs(Data.Skip(9).ToArray());
        }
    }
}