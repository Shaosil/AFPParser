using System;
using System.Collections.Generic;

namespace AFPParser
{
    // Mappings which appear more than once should be stored here
    public static class CommonMappings
    {
        public enum eRotations { Zero = 0, Ninety = 0x2D, OneEighty = 0x5A, TwoSeventy = 0x87 }
        public static Dictionary<byte, string> Rotations = new Dictionary<byte, string>()
            {
                { 0x00, "0 Degrees" },
                { 0x2D, "90 Degrees" },
                { 0x5A, "180 Degrees" },
                { 0x87, "270 Degrees" }
            };

        public static Dictionary<byte, string> CharacterUseFlags = new Dictionary<byte, string>()
            {
                { 0x00, "Valid Coded Character|Invalid Coded Character" },
                { 0x01, "To be printed|Not to be printed" },
                { 0x02, "To be incremented|Not to be incremented" }
            };

        public static Dictionary<byte, string> UnitBase = new Dictionary<byte, string>()
            {
                { 0x00, "10 Inches" },
                { 0x01, "10 Centimeters" }
            };

        public enum eWidthClass { NotSpecified, Ultralight, Extralight, Light, Semilight, Medium, Semibold, Bold, Extrabold, Ultrabold }
        public static Dictionary<byte, string> WidthClass = new Dictionary<byte, string>()
            {
                { 0x00, "Not Specified" },
                { 0x01, "Ultralight" },
                { 0x02, "Extralight" },
                { 0x03, "Light" },
                { 0x04, "Semilight" },
                { 0x05, "Medium (normal)" },
                { 0x06, "Semibold" },
                { 0x07, "Bold" },
                { 0x08, "Extrabold" },
                { 0x09, "Ultrabold" }
            };

        public enum eWeightClass { NotSpecified, Ultracondensed, Extracondensed, Condensed, Semicondensed, Medium, Semiexpanded, Expanded, Extraexpanded, Ultraexpanded }
        public static Dictionary<byte, string> WeightClass = new Dictionary<byte, string>()
            {
                { 0x00, "Not Specified" },
                { 0x01, "Ultracondensed" },
                { 0x02, "Extracondensed" },
                { 0x03, "Condensed" },
                { 0x04, "Semicondensed" },
                { 0x05, "Medium (normal)" },
                { 0x06, "Semiexpanded" },
                { 0x07, "Expanded" },
                { 0x08, "Extraexpanded" },
                { 0x09, "Ultraexpanded" }
            };

        [Flags]
        public enum eFontDesignFlags { Italic = 128, Underscored = 64, Hollow = 16, Overstruck = 8, Proportioned = 4, PairwiseKerned = 2 }
        public static Dictionary<byte, string> FontDesignFlags = new Dictionary<byte, string>()
            {
                { 0x00, "Not Italic|Italic" },
                { 0x01, "Not Underscored|Underscored" },
                { 0x03, "Solid|Hollow" },
                { 0x04, "Not Overstruck|Overstruck" },
                { 0x05, "Uniformly Spaced|Proportionally Spaced" },
                { 0x06, "Not Pairwise Kerned|Pairwise Kerned" }
            };
    }
}
