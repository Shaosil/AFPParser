using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class UniversalDateandTimeStamp : Triplet
    {
        private static string _desc = "Specifies a date and time in accordance with the format defined in ISO 8601: 1988 (E).";

        protected override string Description { get { return _desc; } }
        protected override List<Offset> Offsets { get { return null; } }

        public UniversalDateandTimeStamp(byte[] allData) : base(allData) { }

        protected override string GetDescription()
        {
            // Offset is NULL since we are handling the whole data section (only one offset defined)

            StringBuilder sb = new StringBuilder();

            if (Data.Length < 13)
                return $"Invalid data length of {Data.Length}. Expected 13.";

            // Get the year by proper endian
            byte[] orderedBytes = Data.Skip(3).Take(2).ToArray();
            if (BitConverter.IsLittleEndian) orderedBytes = orderedBytes.Reverse().ToArray();
            int year = BitConverter.ToUInt16(orderedBytes, 0);

            // Store everything else
            int month = Data[5];
            int day = Data[6];
            int hour = Data[7];
            int minute = Data[8];
            int second = Data[9];

            // Calculate the date
            DateTime formattedDate = new DateTime(year, month, day, hour, minute, second);

            // Add/subtract time zone info if there is an offset
            if ((int)Data[10] > 0)
            {
                int hoursAhead = Data[11];
                int minutesAhead = Data[12];

                formattedDate = formattedDate.AddHours(hoursAhead).AddMinutes(minutesAhead);
            }

            return sb.ToString();
        }
    }
}
