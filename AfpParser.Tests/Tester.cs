using AFPParser.StructuredFields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            BFN newBFN = new BFN("BARCOD39");
            FND newFND = new FND("Code 39 Barcode", 36);
            FNC newFNC = new FNC();
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