using System;
using System.Linq;
using System.Text;

namespace AFPParser
{
    public partial class CustomDataParser
    {
        private static string X72(Offset oSet, byte[] data)
        {
            // Offset is NULL since we are handling the whole data section (only one offset defined)

            StringBuilder sb = new StringBuilder();

            if (data.Length < 13)
                return $"Invalid data length of {data.Length}. Expected 13.";

            // Get the year by proper endian
            byte[] orderedBytes = data.Skip(3).Take(2).ToArray();
            if (BitConverter.IsLittleEndian) orderedBytes = orderedBytes.Reverse().ToArray();
            int year = BitConverter.ToUInt16(orderedBytes, 0);

            // Store everything else
            int month = data[5];
            int day = data[6];
            int hour = data[7];
            int minute = data[8];
            int second = data[9];

            // Calculate the date
            DateTime formattedDate = new DateTime(year, month, day, hour, minute, second);

            // Add/subtract time zone info if there is an offset
            if ((int)data[10] > 0)
            {
                int hoursAhead = data[11];
                int minutesAhead = data[12];

                formattedDate = formattedDate.AddHours(hoursAhead).AddMinutes(minutesAhead);
            }

            return sb.ToString();
        }
    }
}
