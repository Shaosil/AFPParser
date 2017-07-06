using AFPParser.Triplets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFPParser.StructuredFields
{
    public class OCD : StructuredField
    {
        private static string _abbr = "OCD";
        private static string _title = "Object Container Data";
        private static string _desc = "Contains the data for an object carried in an object container.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, "CUSTOM PARSED")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public OCD(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            // Get the object classification triplet from the BOC field in our container
            ObjectClassification oc = LowestLevelContainer.GetStructure<BOC>()?.Triplets.OfType<ObjectClassification>().FirstOrDefault();

            if (oc != null)
            {
                StringBuilder sb = new StringBuilder();

                string byteLevel = " bytes";
                float byteSize = Data.Length;
                if (byteSize > 1024) { byteSize /= 1024; byteLevel = "KB"; }
                if (byteSize > 1024) { byteSize /= 1024; byteLevel = "MB"; };
                if (byteSize % 1 > 0) byteSize = (float)Math.Round(byteSize, 2);

                string dataType = "UNKNOWN";
                if (Lookups.OIDs.ContainsKey(oc.RegisteredObjectID))
                    dataType = Lookups.OIDs[oc.RegisteredObjectID];
                sb.AppendLine($"Data Type: {dataType}");
                sb.AppendLine($"Data Length: {byteSize} {byteLevel}");

                return sb.ToString();
            }

            return "Could not find BOC/Object Classification Triplet";
        }
    }
}