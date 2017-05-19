using System;
using System.Text;

namespace AFPParser
{
    public abstract class DataStructure
    {
        public const string EBCDIC = "IBM037";

        // Properties directly converted from raw hex data
        public int Length { get; private set; }
        public string ID { get; private set; }
        public byte[] Data { get; private set; }
        public SemanticsInfo Semantics { get; protected set; }

        // Dynamically calculated properties
        public string DataHex { get { return BitConverter.ToString(Data).Replace("-", " "); } }
        public string DataEBCDIC { get { return Encoding.GetEncoding(EBCDIC).GetString(Data); } }
        protected string SpacedClassName
        {
            get
            {
                // The name of the class but with spaces (add a space before each new uppercased character)
                string className = GetType().Name;
                string spacedName = string.Empty;
                for (int i = 0; i < className.Length; i++)
                    spacedName += i > 0 && char.IsUpper(className[i]) && !char.IsUpper(className[i - 1])
                        ? $" {className[i]}"
                        : className[i].ToString();

                return spacedName;
            }
        }

        protected abstract string StructureName { get; }

        public DataStructure(int length, string id, int introducerLength)
        {
            Length = length;
            ID = id;
            Data = new byte[Length - introducerLength];
        }

        public virtual string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{Semantics.Title} ({StructureName} 0x{ID})");
            sb.Append(GetOffsetDescriptions());

            return sb.ToString();
        }

        // This method should be overwritten by inheriting classes
        protected abstract string GetOffsetDescriptions();
        
        protected byte[] GetSectionedData(int startIndex, int length)
        {
            byte[] sectionedData = new byte[length];
            for (int i = 0; i < length; i++)
                sectionedData[i] = Data[startIndex + i];

            return sectionedData;
        }

        protected string GetReadableDataPiece(int startIndex, int length)
        {
            return Encoding.GetEncoding(EBCDIC).GetString(GetSectionedData(startIndex, length));
        }
    }
}
