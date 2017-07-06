﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class LocalDateAndTimeStamp : Triplet
    {
        private static string _desc = "Specifies a date and time stamp to be associated with an object.";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public LocalDateAndTimeStamp(byte id, byte[] data) : base(id, data) { }

        protected override string GetOffsetDescriptions()
        {
            // Offset is NULL since we are handling the whole data section (only one offset defined)

            StringBuilder sb = new StringBuilder();

            if (Data.Length < 15)
                return $"Invalid data length of {Data.Length}. Expected 15.";

            // Define our stamp types
            Dictionary<byte, string> stampTypes = new Dictionary<byte, string>()
                { { 0x00, "Creation" }, { 0x01, "Retired Value" }, { 0x03, "Revision" } };

            string stampType = stampTypes.ContainsKey(Data[0]) ? stampTypes[Data[0]] : "(INVALID STAMP TYPE)";
            sb.AppendLine($"Stamp Type: {stampType}");

            // Get an EBCDIC string of the rest of the data
            string ebcdic = GetReadableDataPiece(1, Data.Length - 1);

            // Determine whether it's the 1900s or 2000s
            string year = ebcdic.Substring(0, 1);
            if (year == " ") year = "19";
            else year = $"2{year}";

            // Get the second half of the year and combine them to an int
            year += ebcdic.Substring(1, 2);
            int intYear = int.Parse(year);

            // Get the day of the year (1-366)
            int day = int.Parse(ebcdic.Substring(3, 3));

            // Get the hour, minute, second, and hundredth second
            int hour = int.Parse(ebcdic.Substring(6, 2));
            int minute = int.Parse(ebcdic.Substring(8, 2));
            int second = int.Parse(ebcdic.Substring(10, 2));
            int hundredth = int.Parse(ebcdic.Substring(12, 2));

            // Convert everything to a wonderfully formatted date string
            DateTime formattedDateTime = new DateTime(intYear, 1, 1)
                .AddDays(day)
                .AddHours(hour)
                .AddMinutes(minute)
                .AddSeconds(second)
                .AddMilliseconds(hundredth * 10);

            sb.AppendLine($"Date: {formattedDateTime}");

            return sb.ToString();
        }
    }
}