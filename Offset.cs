﻿using System;
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

        // Mappings are looked up by byte (corresponding to big endian bit position)
        // and flag-checked. The description contains info for both positions, such as
        // "Off condition|On condition", respectively, separated by pipe
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
                            sb.Append(string.IsNullOrWhiteSpace(decoded) ? "(BLANK)" : decoded);
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
                    Dictionary<byte, string[]> bitsAndDescriptions = new Dictionary<byte, string[]>();
                    foreach (KeyValuePair<byte, string> mapping in Mappings)
                        bitsAndDescriptions.Add(mapping.Key, mapping.Value.Split('|').ToArray());

                    // Display in big Endian if not already
                    BitArray bitInfo = new BitArray(new[] { data });
                    if (BitConverter.IsLittleEndian)
                        bitInfo = new BitArray(bitInfo.Cast<bool>().Reverse().ToArray());

                    // Get the position of the bit, and determine which description to display
                    foreach (KeyValuePair<byte, string[]> kvp in bitsAndDescriptions)
                    {
                        int descIndex = Convert.ToInt32(bitInfo[kvp.Key]); // 0 or 1
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
