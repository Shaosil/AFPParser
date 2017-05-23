using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace AFPParser
{
    public class Parser
    {
        public static BindingList<StructuredField> AfpFile { get; set; }

        public void Parse(string fileName)
        {
            // First, read all AFP file bytes into memory
            byte[] byteList = File.ReadAllBytes(fileName);

            // Next, loop through each 5A block and store a StructuredField object
            int curIdx = 0;
            AfpFile = new BindingList<StructuredField>();
            while (curIdx < byteList.Length - 1)
            {
                if (byteList[curIdx] != 0x5A)
                {
                    MessageBox.Show($"Unexpected byte at offset 0x{curIdx.ToString("X8")}. Is it a true AFP file?", "AFP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Grab the raw bytes for some of the sections
                byte[] lengthBytes = new byte[2], sequenceBytes = new byte[2], hexBytes = new byte[3];
                Array.ConstrainedCopy(byteList, curIdx + 1, lengthBytes, 0, 2);
                Array.ConstrainedCopy(byteList, curIdx + 3, hexBytes, 0, 3);
                Array.ConstrainedCopy(byteList, curIdx + 7, sequenceBytes, 0, 2);

                // Get introducer section
                int length = (int)DataStructure.GetNumericValue(lengthBytes, false);
                string hex = BitConverter.ToString(hexBytes).Replace("-", "");
                byte flag = byteList[curIdx + 6];
                int sequence = (int)DataStructure.GetNumericValue(sequenceBytes, false);

                // Lookup what type of field we need by the structured fields hex dictionary
                Type fieldType = typeof(StructuredFields.UNKNOWN);
                if (Lookups.StructuredFields.ContainsKey(hex))
                    fieldType = Lookups.StructuredFields[hex];
                StructuredField field = (StructuredField)Activator.CreateInstance(fieldType, length, hex, flag, sequence);

                // Populate the data byte by byte
                for (int i = 0; i < field.Data.Length; i++)
                    field.Data[i] = byteList[curIdx + 9 + i];

                // Append to AFP file
                AfpFile.Add(field);

                // Go to next 5A
                curIdx += field.Length + 1;
            }
        }
    }
}
