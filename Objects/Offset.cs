using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AFPParser
{
    [DebuggerDisplay("{StartingIndex}:{DataType}:{Description}")]
    public class Offset
    {
        public int StartingIndex { get; set; }
        public Lookups.DataTypes DataType { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Normal mappings simply map a description to a single byte value
        /// 
        /// Bit mappings are looked up by byte (corresponding to big endian bit position)
        /// and flag-checked. The description contains info for both positions, such as
        /// "Off condition|On condition", respectively, separated by pipe
        /// </summary>
        public Dictionary<byte, string> Mappings { get; set; }

        public Offset(int startingIdx, Lookups.DataTypes dataType, string description)
        {
            StartingIndex = startingIdx;
            DataType = dataType;
            Description = description;
            Mappings = new Dictionary<byte, string>();
        }

        public string DisplayDataByType(byte[] data)
        {
            StringBuilder sb = new StringBuilder($"{Description}: ");

            if (data.Length > 0)
            {
                if (Mappings.Any())
                    sb.Append(DisplayMappedInfo(data[0]));
                else
                    switch (DataType)
                    {
                        case Lookups.DataTypes.UBIN:
                        case Lookups.DataTypes.SBIN:
                            bool isSigned = DataType == Lookups.DataTypes.SBIN;
                            sb.Append(data.Length > 4 ? "(Unknown Numeric Value)"
                                : DataStructure.GetNumericValue(data, isSigned).ToString());
                            break;

                        case Lookups.DataTypes.CHAR:
                        case Lookups.DataTypes.CODE:
                            string decoded = Encoding.GetEncoding(DataStructure.EBCDIC).GetString(data);
                            if (string.IsNullOrWhiteSpace(Extensions.RegexReadableText.Match(decoded).Value))
                                decoded = "(BLANK)";
                            sb.Append(decoded);
                            break;

                        default:
                            string hexValue = BitConverter.ToString(data).Replace("-", " ");
                            sb.Append($"({hexValue})");
                            break;
                    }
            }
            else
                sb.Append("(BLANK)");

            return sb.ToString();
        }

        private string DisplayMappedInfo(byte data)
        {
            StringBuilder sb = new StringBuilder();

            switch (DataType)
            {
                case Lookups.DataTypes.BITS:
                    // Mappings correspond to bit positions
                    Dictionary<byte, string[]> bitsAndDescriptions = Mappings.ToDictionary(k => k.Key, v => v.Value.Split('|'));

                    // Check bit flag and determine which description to display
                    foreach (KeyValuePair<byte, string[]> kvp in bitsAndDescriptions)
                    {
                        int shift = 7 - kvp.Key; // 0 = leftmost bit in our mappings
                        int descIndex = Convert.ToInt32((data & (1 << shift)) > 0); // If bit is flagged, use index 1
                        sb.AppendLine();
                        sb.Append($"* {kvp.Value[descIndex]}");
                    }
                    break;

                default:
                    sb.Append(Mappings[data]);
                    break;
            }

            return sb.ToString();
        }
    }
}
