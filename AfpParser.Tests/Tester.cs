using AFPParser.StructuredFields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;

namespace AFPParser.Tests
{
    [TestClass]
    public class Tester
    {
        [TestMethod]
        public void Misc()
        {
            //SaveFontAsBitmaps();

            // Open each bitmap, and convert pure black pixels to an array of bools
            Dictionary<char, bool[,]> charsAndBoolVals = new Dictionary<char, bool[,]>();
            foreach (FileInfo f in new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Sample Files\\Barcode Chars")).GetFiles())
            {
                Bitmap png = new Bitmap(f.FullName);
                char thisChar = f.Name == "Asterisk.png" ? '*' : f.Name[0];
                bool[,] boolVals = new bool[png.Width, png.Height];

                for (int y = 0; y < png.Height; y++)
                    for (int x = 0; x < png.Width; x++)
                        boolVals[x, y] = png.GetPixel(x, y).GetBrightness() < 0.5;

                charsAndBoolVals.Add(thisChar, boolVals);
            }

            // Create an AFP font file (BFN, FND, FNC, FNM, FNO, FNP, FNIs, FNGs, EFN) using bool arrays as FNGs
            Func<Type, byte[]> getId = (Type t) => { return Extensions.GetByteArrayFromHexString(Lookups.StructuredFields.First(s => s.Value == t).Key); };
            BFN newBFN = new BFN(getId(typeof(BFN)), 0, 0, new byte[0]);

            List<byte> fndData = new List<byte>();
            fndData.AddRange(Encoding.GetEncoding(DataStructure.EBCDIC).GetBytes("Code 39 Barcode".PadRight(32)));
            fndData.Add(0x05);
            fndData.Add(0x05);
            fndData.AddRange(BitConverter.GetBytes((ushort)36).Reverse());
            fndData.AddRange(BitConverter.GetBytes((ushort)36).Reverse());
            fndData.AddRange(BitConverter.GetBytes((ushort)36).Reverse());
            fndData.AddRange(new byte[2] { 0, 0 });
            fndData.AddRange(new byte[2] { 0, 0 });
            fndData.AddRange(new byte[2] { 0, 0 });
            fndData.Add(0);
            fndData.Add(0);
            fndData.Add(0);
            for (int i = 1; i <= 15; i++) fndData.Add(0);
            fndData.AddRange(BitConverter.GetBytes((ushort)4096).Reverse());
            for (int i = 1; i <= 10; i++) fndData.Add(0);
            fndData.AddRange(new byte[2] { 0, 0 });
            fndData.AddRange(new byte[2] { 0, 0 });
            FND newFND = new FND(getId(typeof(FND)), 0, 0, fndData.ToArray());

            List<byte> fncData = new List<byte>();
            fncData.Add(0);
            fncData.Add(0x05);
            fncData.Add(0);
            fncData.Add(0);
            fncData.Add(0x02);
            fncData.Add(0x02);
            fncData.AddRange(BitConverter.GetBytes((ushort)1000).Reverse());
            fncData.AddRange(BitConverter.GetBytes((ushort)1000).Reverse());
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
    }
}