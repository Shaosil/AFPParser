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
        public BindingList<StructuredField> AfpFile { get; set; }

        public void Parse(string fileName)
        {
            // First, load all semantics from text files
            Identifier.Load();
            Triplets.Load();
            PTXCSIFunctions.Load();

            // Then, read all AFP file bytes into memory
            byte[] byteList = File.ReadAllBytes(fileName);

            // Prepare some variables and dynamic funcs...
            int curIdx = 0;
            AfpFile = new BindingList<StructuredField>();
            Func<byte[], byte[]> getProperEndianArray = (byte[] array) =>
            {
                if (BitConverter.IsLittleEndian)
                {
                    // Reverse the array
                    byte[] reversedArray = new byte[array.Length];
                    for (int i = 0; i < array.Length; i++)
                        reversedArray[i] = array[array.Length - 1 - i];

                    array = reversedArray;
                }

                return array;
            };

            // Next, loop through each 5A block and store a StructuredField object
            while (curIdx < byteList.Length - 1)
            {
                if (byteList[curIdx] != 0x5A)
                {
                    MessageBox.Show($"Unexpected byte at offset 0x{curIdx.ToString("X").PadLeft(8, '0')}. Is it a true AFP file?", "AFP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Grab the raw bytes for some of the sections
                byte[] lengthBytes = new byte[2], sequenceBytes = new byte[2], hexBytes = new byte[3];
                Array.ConstrainedCopy(byteList, curIdx + 1, lengthBytes, 0, 2);
                Array.ConstrainedCopy(byteList, curIdx + 3, hexBytes, 0, 3);
                Array.ConstrainedCopy(byteList, curIdx + 7, sequenceBytes, 0, 2);

                // Set properties of structured field
                StructuredField field = new StructuredField();
                field.Length = BitConverter.ToInt16(getProperEndianArray(lengthBytes), 0);
                field.HexCode = BitConverter.ToString(hexBytes).Replace("-", "");
                field.Flag = byteList[curIdx + 6];
                field.Sequence = BitConverter.ToInt16(getProperEndianArray(sequenceBytes), 0);
                field.Data = new byte[field.Length - 8];
                for (int i = 0; i < field.Data.Length; i++)
                    field.Data[i] = byteList[curIdx + 8 + i];

                // Append to AFP file
                AfpFile.Add(field);

                // Go to next 5A
                curIdx += field.Length + 1;
            }
        }
    }
}
