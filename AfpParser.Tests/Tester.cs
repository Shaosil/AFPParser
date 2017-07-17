using AFPParser.StructuredFields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace AFPParser.Tests
{
    [TestClass]
    public class Tester
    {
        public class CharInfo
        {
            public char Character { get; set; }
            public bool[,] RawBits { get; set; }
            public ushort BitWidth => (ushort)(RawBits.GetUpperBound(0) + 1);
            public ushort BitHeight => (ushort)(RawBits.GetUpperBound(1) + 1);
            public ushort RoundedBitWidth => (ushort)(Math.Ceiling(BitWidth / 8f) * 8);
            public ushort CharIncrement => (ushort)(1000 * ((float)RoundedBitWidth / BitHeight));

            public CharInfo(char character, bool[,] rawBits)
            {
                Character = character;
                RawBits = rawBits;
            }
        }

        [TestMethod]
        public void Misc()
        {
        }

        private void SaveFontAsBitmaps()
        {
            // Get our custom font
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Sample Files\\fre3of9x.ttf"));
            Font ourFont = new Font(pfc.Families[0], 72 * 2);

            // Using the code 39 barcode font, save each specified character as a bitmap
            string chars = "*ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-$%.+";
            List<Bitmap> charImages = new List<Bitmap>();
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                foreach (char c in chars)
                {
                    SizeF size = g.MeasureString(c.ToString(), ourFont);

                    Bitmap charBmp = new Bitmap((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
                    using (Graphics canvas = Graphics.FromImage(charBmp))
                    {
                        canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        canvas.DrawString(c.ToString(), ourFont, Brushes.Black, PointF.Empty);
                    }

                    // Trim edges - crop image to drawn area
                    int minX = charBmp.Width, maxX = 0, minY = charBmp.Height, maxY = 0;
                    for (int y = 0; y < charBmp.Height; y++)
                        for (int x = 0; x < charBmp.Width; x++)
                        {
                            if (charBmp.GetPixel(x, y).A > 0)
                            {
                                if (x < minX) minX = x;
                                if (y < minY) minY = y;
                                if (x > maxX) maxX = x;
                                if (y > maxY) maxY = y;
                            }
                        }
                    Bitmap croppedCharBmp = new Bitmap(maxX - minX, maxY - minY);
                    Point[] points = new Point[] { Point.Empty, new Point(croppedCharBmp.Width, 0), new Point(0, croppedCharBmp.Height) };
                    using (Graphics canvas = Graphics.FromImage(croppedCharBmp))
                        canvas.DrawImage(charBmp, points, new Rectangle(minX, minY, croppedCharBmp.Width, croppedCharBmp.Height), GraphicsUnit.Pixel);

                    charImages.Add(croppedCharBmp);

                    string fileName = c.ToString();
                    if (fileName == "*") fileName = "Asterisk";
                    croppedCharBmp.Save(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\Sample Files\\{fileName}.png"));
                }
            }
        }

        private void SaveBitmapsAsFont()
        {
            // Open each bitmap, and convert pure black pixels to an array of bools
            List<CharInfo> charInfos = new List<CharInfo>();
            foreach (FileInfo f in new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Sample Files\\Barcode Chars")).GetFiles())
            {
                Bitmap png = new Bitmap(f.FullName);
                char thisChar = f.Name == "Asterisk.png" ? '*' : f.Name[0];
                bool[,] boolVals = new bool[png.Width + 1, png.Height];

                for (int y = 0; y < png.Height; y++)
                    for (int x = 0; x < png.Width; x++)
                        boolVals[x + 1, y] = png.GetPixel(x, y).GetBrightness() < 0.5;

                charInfos.Add(new CharInfo(thisChar, boolVals));
            }

            // Create an AFP font file (BFN, FND, FNC, FNM, FNO, FNP, FNIs, FNGs, EFN) using bool arrays as FNGs
            BFN newBFN = new BFN("BARCOD39");

            FND newFND = new FND("Code 39 Barcode", 172);

            ushort maxBitsWideIdx = (ushort)(charInfos.Max(c => c.BitWidth) - 1);
            ushort maxBitsTallIdx = (ushort)(charInfos.Max(c => c.BitHeight) - 1);
            int totalRasterBytes = charInfos.Sum(c => c.RoundedBitWidth * c.BitHeight);
            FNC newFNC = new FNC(maxBitsWideIdx, maxBitsTallIdx, totalRasterBytes);

            uint curFNMIndex = 0;
            List<FNM.PatternData> fnmPatData = new List<FNM.PatternData>();
            foreach (CharInfo c in charInfos)
            {
                fnmPatData.Add(new FNM.PatternData((ushort)(c.BitWidth - 1), (ushort)(c.BitHeight - 1), curFNMIndex));
                curFNMIndex += (uint)((c.RoundedBitWidth * c.BitHeight) / 8);
            }
            FNM newFNM = new FNM(fnmPatData);

            List<FNO.Info> fnoInfos = new List<FNO.Info>();
            ushort maxCharInc = charInfos.Max(c => c.CharIncrement);
            for (int i = 0; i < 4; i++)
            {
                Type eType = typeof(CommonMappings.eRotations);
                CommonMappings.eRotations curRot = (CommonMappings.eRotations)Enum.Parse(eType, Enum.GetNames(eType)[i]);
                ushort curBaseline = (i == 0 || i == 2) ? (ushort)1000 : maxCharInc;
                ushort curCharInc = (i == 0 || i == 2) ? maxCharInc : (ushort)1000;
                FNO.Info.eControlFlags flags = i == 0 ? 0 : i == 1 ? FNO.Info.eControlFlags.FNI1 : i == 2 ? FNO.Info.eControlFlags.FNI2 : FNO.Info.eControlFlags.FNI3;
                fnoInfos.Add(new FNO.Info(curRot, (short)curBaseline, curCharInc, 0, curBaseline, flags, 1000, curCharInc, curCharInc, curBaseline, 0));
            }
            FNO newFNO = new FNO(fnoInfos);

            List<FNP.Info> fnpInfos = new List<FNP.Info>();
            for (int i = 0; i < 4; i++)
            {
                short curAsc = (short)((i == 0 || i == 2) ? maxCharInc : 1000);
                fnpInfos.Add(new FNP.Info(1000, 1000, curAsc, 0, 1000, 100));
            }
            FNP newFNP = new FNP(fnpInfos);

            List<FNI> newFNIs = new List<FNI>();
            for (int i = 0; i < 4; i++)
            {
                ushort fnmIndex = 0;

                List<FNI.Info> fniInfos = new List<FNI.Info>();
                foreach (CharInfo c in charInfos)
                {
                    string gid = CodePages.C1140[Converters.EBCDIC.GetBytes(new[] { c.Character })[0]];
                    ushort curCharInc = (ushort)((i == 0 || i == 2) ? c.CharIncrement : 1000);
                    short curAscHeight = (short)((i == 0 || i == 2) ? 1000 : c.CharIncrement);
                    fniInfos.Add(new FNI.Info(gid, c.CharIncrement, curAscHeight, 0, fnmIndex++, 0, curCharInc, 0, curAscHeight));
                }
                newFNIs.Add(new FNI(fniInfos));
            }

            List<byte> fngBytes = new List<byte>();
            foreach (CharInfo c in charInfos)
            {
                for (int y = 0; y < c.BitHeight; y++)
                {
                    for (int x = 0; x < c.BitWidth; x += 8)
                    {
                        int[] curByte = new int[1];
                        List<bool> bitRow = new List<bool>();
                        for (int i = 0; i < 8; i++)
                            if (c.RawBits.GetUpperBound(0) >= x + i)
                                bitRow.Add(c.RawBits[x + i, y]);
                            else
                                bitRow.Add(false);
                        if (BitConverter.IsLittleEndian) bitRow.Reverse();
                        new BitArray(bitRow.ToArray()).CopyTo(curByte, 0);
                        fngBytes.Add((byte)curByte[0]);
                    }
                }
            }
            FNG newFNG = new FNG(fngBytes.ToArray());

            EFN newEFN = new EFN(newBFN.ObjectName);

            string path = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Sample Files\\FONTS\\FONTTEST");
            List<StructuredField> fields = new List<StructuredField>() { newBFN, newFND, newFNC, newFNM, newFNO, newFNP };
            fields.AddRange(newFNIs);
            fields.AddRange(new StructuredField[] { newFNG, newEFN });
            List<byte> encoded = new List<byte>();
            foreach (StructuredField field in fields)
            {
                encoded.Add(0x5A);
                encoded.AddRange(field.Introducer);
                encoded.AddRange(field.Data);
            }
            File.WriteAllBytes(path, encoded.ToArray());
        }
    }
}