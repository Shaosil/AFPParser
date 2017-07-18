using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AFPParser
{
    public abstract class DataStructure
    {
        private byte[] _hexID = new byte[0];
        private byte[] _data = new byte[0];

        // Properties directly converted from raw hex data
        public byte[] HexID
        {
            get { return _hexID; }
            set
            {
                _hexID = value;
                if (Introducer != null) SyncIntroducer();
            }
        }
        public string HexIDStr => BitConverter.ToString(HexID).Replace("-", "");
        public byte[] Introducer { get; protected set; }
        public byte[] Data
        {
            get { return _data; }
            set
            {
                _data = value;
                SyncIntroducer();
            }
        }
        public virtual ushort Length => (ushort)(Introducer.Length + Data.Length);

        // Readable information, usually looked up or hard coded by referencing documentation
        public virtual string Title { get; }
        public virtual string Description { get; }
        public virtual IReadOnlyList<Offset> Offsets { get; }

        // Dynamically calculated properties
        public string DataHex => BitConverter.ToString(Data).Replace("-", " ");
        public string DataEBCDIC => GetReadableDataPiece(0, Data.Length);
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

        // Container info
        public List<Container> Containers { get; set; }
        public Container LowestLevelContainer => Containers.Any() ? Containers.Last() : null;
        public Container ParentContainer
        {
            get
            {
                int parentIndex = Containers.Count - 2;
                return parentIndex >= 0 ? Containers[parentIndex] : null;
            }
        }
        public Container NewContainer
        {
            get
            {
                // Returns either a generic container, or custom typed container by specific attribute
                Container c = new Container();
                ContainerTypeAttribute containerAttribyte = GetType().GetCustomAttribute<ContainerTypeAttribute>();
                if (containerAttribyte != null)
                    c = (Container)Activator.CreateInstance(containerAttribyte.AssignedType);

                return c;
            }
        }

        public DataStructure(byte[] hexID, byte[] data)
        {
            HexID = hexID ?? new byte[0];
            Data = data ?? new byte[0];
            Containers = new List<Container>();
        }

        public abstract void ParseData();

        public virtual string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{Title} ({StructureName} 0x{HexIDStr})");
            sb.Append(GetOffsetDescriptions());

            return sb.ToString();
        }

        // This should be called any time an introducer property is changed, and should update the raw introducer data
        protected abstract void SyncIntroducer();

        // Override this to custom handle all offsets at once
        protected virtual string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            if (Offsets.Any())
                // Write out each implemented offset's description
                foreach (Offset oSet in Offsets.Where(o => o.StartingIndex < Data.Length))
                {
                    // Get sectioned data
                    IEnumerable<Offset> nextOffsets = Offsets.Where(o => o.StartingIndex > oSet.StartingIndex);
                    int nextIndex = nextOffsets.Any() ? nextOffsets.Min(o => o.StartingIndex) : 0;
                    if (nextIndex == 0) nextIndex = Data.Length;
                    int bytesToTake = nextIndex - oSet.StartingIndex;
                    byte[] sectionedData = GetSectionedData(oSet.StartingIndex, bytesToTake);

                    sb.Append(GetSingleOffsetDescription(oSet, sectionedData));
                }
            else
                sb.AppendLine($"Not yet implemented...{Environment.NewLine}Raw Data: {DataHex}");

            return sb.ToString();
        }

        // Override this to custom handle one or more offsets at a time.
        protected virtual string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            if (string.IsNullOrWhiteSpace(oSet.Description)) return string.Empty;
            StringBuilder sb = new StringBuilder(oSet.DisplayDataByType(sectionedData));
            sb.AppendLine();
            return sb.ToString();
        }

        protected byte[] GetSectionedData(int startIndex, int length)
        {
            byte[] sectionedData = new byte[length];
            for (int i = 0; i < length; i++)
            {
                if (Data.Length <= startIndex + i) break;
                sectionedData[i] = Data[startIndex + i];
            }

            return sectionedData;
        }

        protected string GetReadableDataPiece(int startIndex, int length)
        {
            // Convert to EBCDIC, only grabbing valid characters
            return Extensions.RegexReadableText.Replace(Converters.EBCDIC.GetString(GetSectionedData(startIndex, length)), "");
        }

        // Returns a proper Endian array of booleans. 8 bits per byte
        public static bool[] GetBitArray(params byte[] bytes)
        {
            bool[] allBits = new bool[8 * bytes.Length];

            // Append all 8 bits from each byte to our boolean array
            for (int i = 0; i < bytes.Length; i++)
            {
                IEnumerable<bool> curArray = new BitArray(new[] { bytes[i] }).Cast<bool>();
                if (BitConverter.IsLittleEndian) curArray = curArray.Reverse();
                Array.ConstrainedCopy(curArray.ToArray(), 0, allBits, 8 * i, 8);
            }

            return allBits;
        }

        public static T GetNumericValue<T>(byte[] bytes)
        {
            Type t = typeof(T);

            // Currently only support up to 4 byte numeric types
            if (bytes.Length > 4) return default(T);

            // If there are three bytes, add a byte to the beginning and manually add the signed bit if needed
            if (bytes.Length == 3)
            {
                bool isSigned = t == typeof(sbyte) || t == typeof(short) || t == typeof(int);
                byte extraByte = (byte)(isSigned && (bytes[0] & (1 << 7)) > 0 ? 0x80 : 0x00);
                bytes = new byte[1] { extraByte }.Concat(bytes).ToArray();
            }

            // Use correct Endianness
            if (BitConverter.IsLittleEndian)
                bytes = bytes.Reverse().ToArray();

            // Return signed/unsigned int16/int32 based on type
            return (T)(t == typeof(byte) ? Convert.ChangeType(Convert.ToByte(bytes[0]), t)
                : t == typeof(sbyte) ? Convert.ChangeType(Convert.ToSByte(bytes[0]), t)
                : t == typeof(ushort) ? Convert.ChangeType(BitConverter.ToUInt16(bytes, 0), t)
                : t == typeof(short) ? Convert.ChangeType(BitConverter.ToInt16(bytes, 0), t)
                : t == typeof(uint) ? Convert.ChangeType(BitConverter.ToUInt32(bytes, 0), t)
                : t == typeof(int) ? Convert.ChangeType(BitConverter.ToInt32(bytes, 0), t)
                : default(T));
        }

        public T GetNumericValueFromData<T>(int startIndex, int len)
        {
            return GetNumericValue<T>(GetSectionedData(startIndex, len));
        }

        protected void PutStringInData(string text, int startIndex, int length)
        {
            if (!string.IsNullOrWhiteSpace(text) && Data.Length >= startIndex + length)
            {
                string trimmed = text.Trim().PadRight(length);
                if (trimmed.Length > length) trimmed = trimmed.Substring(0, length);
                byte[] stringBytes = Converters.EBCDIC.GetBytes(trimmed);

                // Plop trimmed string into the specified place in Data
                Array.ConstrainedCopy(stringBytes, 0, Data, startIndex, stringBytes.Length);
            }
        }

        protected void PutNumberInData(object num, int startIndex, int specificLength = 0)
        {
            // Convert to whatever numeric type it is
            byte[] numBytes = null;
            if (num is byte) numBytes = BitConverter.GetBytes(Convert.ToByte(num));
            else if (num is sbyte) numBytes = BitConverter.GetBytes(Convert.ToSByte(num));
            else if (num is ushort) numBytes = BitConverter.GetBytes(Convert.ToUInt16(num));
            else if (num is short) numBytes = BitConverter.GetBytes(Convert.ToInt16(num));
            else if (num is uint) numBytes = BitConverter.GetBytes(Convert.ToUInt32(num));
            else if (num is int) numBytes = BitConverter.GetBytes(Convert.ToInt32(num));

            if (numBytes != null && Data.Length >= startIndex + numBytes.Length)
            {
                if (BitConverter.IsLittleEndian) numBytes = numBytes.Reverse().ToArray();
                if (specificLength > 0) numBytes = numBytes.Take(specificLength).ToArray();
                Array.ConstrainedCopy(numBytes, 0, Data, startIndex, numBytes.Length);
            }
        }
    }
}
