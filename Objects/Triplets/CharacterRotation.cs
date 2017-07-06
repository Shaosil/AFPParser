using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class CharacterRotation : Triplet
    {
        private static string _desc = "Specifies character rotation relative to the character coordinate system.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Character Rotation") { Mappings = CommonMappings.Rotations }
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public CharacterRotation(byte id, byte[] data) : base(id, data) { }
    }
}