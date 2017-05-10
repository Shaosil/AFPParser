using System.Diagnostics;

namespace AFPParser
{
    [DebuggerDisplay("{Abbreviation} - {HexCode}")]
    public class Identifier
    {
        public string HexCode { get; set; }
        public string Abbreviation { get; set; }
        public SemanticsInfo Semantics { get; set; }

        public Identifier(string title, string hexCode, string abbreviation)
        {
            Semantics = new SemanticsInfo() { Title = title };
            HexCode = hexCode;
            Abbreviation = abbreviation;
        }
    }
}
