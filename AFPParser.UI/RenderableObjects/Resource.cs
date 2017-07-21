using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFPParser.UI
{
    public class Resource
    {
        // Store .NET's code pages that are 4 digits or less
        private static IReadOnlyList<int> netEncodings = Encoding.GetEncodings().Where(e => e.CodePage <= 9999).Select(e => e.CodePage).ToList();

        public enum eResourceType { Unknown, IMImage, IOCAImage, CodePage, CodedFont, FontCharacterSet, PageSegment }

        public IReadOnlyList<StructuredField> Fields { get; set; }
        public string ResourceName { get; private set; }
        public eResourceType ResourceType { get; private set; }
        public bool IsLoaded => Fields.Any();
        public bool IsEmbedded { get; private set; }
        public bool IsNETCodePage { get; private set; }
        public string Message
        {
            get
            {
                return
                    IsEmbedded ? "Embedded"
                    : IsLoaded ? "Loaded"
                    : IsNETCodePage ? "File Not Found - Defaulting to .NET's definition"
                    : "File Not Found";
            }
        }

        public Resource(string fName, eResourceType rType, bool embedded = false)
        {
            Fields = new List<StructuredField>();
            ResourceName = fName.ToUpper().Trim();
            ResourceType = rType;
            IsEmbedded = embedded;

            // If we are a code page resource, see if we also exist in .NET
            if (ResourceType == eResourceType.CodePage)
            {
                // Compare the last four digits of our code page to .NET's existing list.
                int ourCodePage = 0;
                if (ResourceName.Length >= 4)
                    int.TryParse(ResourceName.Substring(ResourceName.Length - 4), out ourCodePage);

                // If we have a match, we will use .NET's, since it's likely a custom file doesn't exist
                if (ourCodePage > 0) IsNETCodePage = true;
            }
        }
    }
}
