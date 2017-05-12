using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AFPParser
{
    public partial class CustomDataParser
    {
        private static string XF6_F7_PTX(Offset oSet, byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            // I/B Axis orientation stored in four total bytes.
            // Each two byte set has 3 parts. A (9 bits), B (6 bits), and C (1 bit)
            // A = 0 -359 degrees
            // B = 0 - 59 minutes
            // C = Reserved, always 0

            // Loop through both bytes
            for (int i = 0; i <= 2; i += 2)
            {
                // Get the bit values of the two bytes
                IEnumerable<bool> formattedBA;
                if (BitConverter.IsLittleEndian)
                {
                    // To flip the bytes, take each byte separately and cast it to a reversed bool array
                    IEnumerable<bool> firstBA = new BitArray(new[] { data[i] }).Cast<bool>().Reverse();
                    IEnumerable<bool> secondBA = new BitArray(new[] { data[i + 1] }).Cast<bool>().Reverse();

                    // Then merge them all in the right order to a new bit array
                    formattedBA = new BitArray(firstBA.Concat(secondBA).ToArray()).Cast<bool>();
                }
                else
                    formattedBA = new BitArray(new[] { data[i], data[i + 1] }).Cast<bool>();

                // Prepare the degree and minute bits
                IEnumerable<bool> degreeBits = formattedBA.Take(9), minuteBits = formattedBA.Skip(9).Take(6);
                if (BitConverter.IsLittleEndian)
                {
                    degreeBits = degreeBits.Reverse();
                    minuteBits = minuteBits.Reverse();
                }

                // Parse the bits out to an int array (BitArray.CopyTo sums them up for us)
                int[] values = new int[2];
                new BitArray(degreeBits.ToArray()).CopyTo(values, 0);
                new BitArray(minuteBits.ToArray()).CopyTo(values, 1);

                // Write out our values
                if (i == 0)
                    sb.AppendLine($"I Orientation: {values[0]}:{values[1]}");
                else
                    sb.Append($"B Orientation: {values[0]}:{values[1]}");
            }

            return sb.ToString();
        }
    }
}