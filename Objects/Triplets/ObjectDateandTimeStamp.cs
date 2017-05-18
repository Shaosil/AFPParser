using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class ObjectDateandTimeStamp : Triplet
    {
        private static string _desc = "Specifies a date and time stamp to be associated with an object.";

        protected override string Description => _desc;
        protected override List<Offset> Offsets => new List<Offset>();

        public ObjectDateandTimeStamp(byte[] allData) : base(allData) { }

        public override string GetDescription()
        {
            // Offset is NULL since we are handling the whole data section (only one offset defined)

            StringBuilder sb = new StringBuilder();

            if (Data.Length < 17)
                return $"Invalid data length of {Data.Length}. Expected 17.";

            // Define our stamp types
            Dictionary<byte, string> stampTypes = new Dictionary<byte, string>()
                { { 0x00, "Creation" }, { 0x01, "Retired Value" }, { 0x03, "Revision" } };

            string stampType = stampTypes.ContainsKey(Data[2]) ? stampTypes[Data[2]] : "(INVALID STAMP TYPE)";
            sb.AppendLine($"Stamp Type: {stampType}");

            // Get an EBCDIC string of the rest of the data
            string ebcdic = Encoding.GetEncoding("IBM037").GetString(Data.Skip(3).ToArray());

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