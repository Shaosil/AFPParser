using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFPParser.StructuredFields
{
    public class FNO : StructuredField
    {
        private static string _abbr = "FNO";
        private static string _title = "Font Orientation";
        private static string _desc = "Each group in this field applies to a single character rotation of a font character set.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(2, Lookups.DataTypes.UBIN, "Character Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(4, Lookups.DataTypes.SBIN, "Max Baseline Offset"),
            new Offset(6, Lookups.DataTypes.UBIN, "Max Character Increment"),
            new Offset(8, Lookups.DataTypes.UBIN, "Space Character Increment"),
            new Offset(10, Lookups.DataTypes.UBIN, "Max Baseline Extent"),
            new Offset(12, Lookups.DataTypes.BITS, "CUSTOM PARSED! SHOULD NOT SEE THIS TEXT"),
            new Offset(13, Lookups.DataTypes.EMPTY, ""),
            new Offset(14, Lookups.DataTypes.UBIN, "Em-Space Increment"),
            new Offset(16, Lookups.DataTypes.EMPTY, ""),
            new Offset(18, Lookups.DataTypes.UBIN, "Figure Space Increment"),
            new Offset(20, Lookups.DataTypes.UBIN, "Nominal Character Increment"),
            new Offset(22, Lookups.DataTypes.UBIN, "Default Baseline Increment"),
            new Offset(24, Lookups.DataTypes.SBIN, "Minimum A-Space")
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
                    length = control.Data[14];

                return length;
            }
        }
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private List<Info> _FNOInfo = new List<Info>();
        public IReadOnlyList<Info> FNOInfo
        {
            get { return _FNOInfo; }
            set
            {
                _FNOInfo = value.ToList();

                // Update the data stream
                Data = new byte[value.Count * 26];
                for (int i = 0; i < value.Count; i++)
                {
                    PutNumberInData((ushort)value[i].Rotation, (i * 26) + 2);
                    PutNumberInData(value[i].MaxBaselineOffset, (i * 26) + 4);
                    PutNumberInData(value[i].MaxCharIncrement, (i * 26) + 6);
                    PutNumberInData(value[i].SpaceCharIncrement, (i * 26) + 8);
                    PutNumberInData(value[i].MaxBaselineExtent, (i * 26) + 10);
                    PutNumberInData((byte)value[i].ControlFlags, (i * 26) + 12);
                    PutNumberInData(value[i].EmSpaceIncrement, (i * 26) + 14);
                    PutNumberInData(value[i].FigureSpaceIncrement, (i * 26) + 18);
                    PutNumberInData(value[i].NominalCharIncrement, (i * 26) + 20);
                    PutNumberInData(value[i].DefaultBaselineIncrement, (i * 26) + 22);
                    PutNumberInData(value[i].MinASpace, (i * 26) + 24);
                }
            }
        }

        public FNO(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public FNO(List<Info> allInfos) : base(Lookups.StructuredFieldID<FNO>(), 0, 0, null)
        {
            FNOInfo = allInfos;
        }

        public override void ParseData()
        {
            // Get all groups
            int curIndx = 0;
            List<Info> allInfo = new List<Info>();

            while (curIndx < Data.Length)
            {
                CommonMappings.eRotations rot = (CommonMappings.eRotations)Data[curIndx + 3];
                short maxBaseOSet = GetNumericValueFromData<short>(curIndx + 4, 2);
                ushort maxCharInc = GetNumericValueFromData<ushort>(curIndx + 6, 2);
                ushort spaceCharInc = GetNumericValueFromData<ushort>(curIndx + 8, 2);
                ushort maxBaseExt = GetNumericValueFromData<ushort>(curIndx + 10, 2);
                Info.eControlFlags cFlags = (Info.eControlFlags)Data[curIndx + 12];
                ushort emSpaceInc = GetNumericValueFromData<ushort>(curIndx + 14, 2);
                ushort figSpaceInc = GetNumericValueFromData<ushort>(curIndx + 18, 2);
                ushort nomCharInc = GetNumericValueFromData<ushort>(curIndx + 20, 2);
                ushort defaultBaseInc = GetNumericValueFromData<ushort>(curIndx + 22, 2);
                short minASpace = GetNumericValueFromData<short>(curIndx + 24, 2);

                allInfo.Add(new Info(rot, maxBaseOSet, maxCharInc, spaceCharInc, maxBaseExt, cFlags,
                    emSpaceInc, figSpaceInc, nomCharInc, defaultBaseInc, minASpace));
                curIndx += RepeatingGroupLength;
            }

            _FNOInfo = allInfo;
        }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            StringBuilder sb = new StringBuilder();

            // Parse everything except offset 12 - handle that one ourselves
            if (oSet.StartingIndex != 12)
                sb.Append(base.GetSingleOffsetDescription(oSet, sectionedData));
            else
            {
                sb.AppendLine($"Control Flags:");

                // The first three bits represent a numeric value, so add them after 5 blank bytes
                IEnumerable<bool> indexNumBits = new bool[5] { false, false, false, false, false }.Concat(GetBitArray(sectionedData).Take(3));
                if (BitConverter.IsLittleEndian) indexNumBits = indexNumBits.Reverse();
                int[] FNINumInt = new int[1];
                new BitArray(indexNumBits.ToArray()).CopyTo(FNINumInt, 0);
                sb.AppendLine($"* Font Index Number: {FNINumInt[0]}");

                sb.Append("* ");
                if ((sectionedData[0] & (1 << 3)) == 0) sb.Append("No ");
                sb.AppendLine("Kerning Data");

                sb.Append("* ");
                if ((sectionedData[0] & (1 << 2)) == 0) sb.Append("Minimum ");
                else sb.Append("Uniform ");
                sb.AppendLine("A-space");

                sb.Append("* ");
                if ((sectionedData[0] & (1 << 1)) == 0) sb.Append("Maximum ");
                else sb.Append("Uniform ");
                sb.AppendLine("baseline offset");

                sb.Append("* ");
                if ((sectionedData[0] & 1) == 0) sb.Append("Maximum ");
                else sb.Append("Uniform ");
                sb.AppendLine("character increment");
            }

            return sb.ToString();
        }

        public class Info
        {
            // Weird, but only specify one "FNI" flag at a time, if any. It specifies the matching index of the FNI group. See FOCA documentation for details
            public enum eControlFlags { FNI1 = 32, FNI2 = 64, FNI3 = 96, Kerning = 8, UniformASpace = 4, UniformBaseline = 2, UniformCharIncrement = 1 }

            public CommonMappings.eRotations Rotation { get; private set; }
            public short MaxBaselineOffset { get; private set; }
            public ushort MaxCharIncrement { get; private set; }
            public ushort SpaceCharIncrement { get; private set; }
            public ushort MaxBaselineExtent { get; private set; }
            public eControlFlags ControlFlags { get; private set; }
            public ushort EmSpaceIncrement { get; private set; }
            public ushort FigureSpaceIncrement { get; private set; }
            public ushort NominalCharIncrement { get; private set; }
            public ushort DefaultBaselineIncrement { get; private set; }
            public short MinASpace { get; private set; }

            public Info(CommonMappings.eRotations rot, short maxBaseOSet, ushort maxCharInc, ushort spaceCharInc, ushort maxBaseExt,
                eControlFlags cFlags, ushort emSpaceInc, ushort figSpaceInc, ushort nomCharInc, ushort defaultBaseInc, short minASpace)
            {
                Rotation = rot;
                MaxBaselineOffset = maxBaseOSet;
                MaxCharIncrement = maxCharInc;
                SpaceCharIncrement = spaceCharInc;
                MaxBaselineExtent = maxBaseExt;
                ControlFlags = cFlags;
                EmSpaceIncrement = emSpaceInc;
                FigureSpaceIncrement = figSpaceInc;
                NominalCharIncrement = nomCharInc;
                DefaultBaselineIncrement = defaultBaseInc;
                MinASpace = minASpace;
            }
        }
    }
}