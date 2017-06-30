using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class CodedGraphicCharacterSetGlobalIdentifier : Triplet
    {
        private static string _desc = "Specifies the code page and character set for a coded font.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Graphic Character Set GID"),
            new Offset(2, Lookups.DataTypes.UBIN, "Code Page GID")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public CodedGraphicCharacterSetGlobalIdentifier(string id, byte[] introcuder, byte[] data) : base(id, introcuder, data) { }
	}
}