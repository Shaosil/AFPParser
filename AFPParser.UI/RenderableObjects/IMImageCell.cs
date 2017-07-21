using AFPParser.StructuredFields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AFPParser.UI
{
    public class IMImageCell
    {
        public ICP CellPosition { get; private set; }
        public List<IRD> RasterData { get; private set; }

        // An image is being represented as a two dimensional array [Xlen, Ylen]
        public bool[,] ImageData { get; private set; }

        public IMImageCell(ICP pos, List<IRD> rasterData)
        {
            CellPosition = pos;
            RasterData = rasterData;

            // Auto populate image data
            ImageData = new bool[pos.XSize, pos.YSize];

            int concatCounter = 0;
            byte[] concatData = rasterData.SelectMany(d => d.Data).ToArray();

            for (int y = 0; y <= ImageData.GetUpperBound(1); y++)
                for (int x = 0; x < ImageData.GetUpperBound(0); x += 8)
                {
                    byte curByte = concatData[concatCounter++];

                    for (int b = 0; b < 8; b++)
                        ImageData[x + b, y] = (curByte & (1 << (7 - b))) > 0;
                }
        }

        public static Bitmap GenerateBitmap(IID descriptor, List<IMImageCell> cells)
        {
            // Create a single bitmap from the min/max X and Y extents of all cells
            int maxX = cells.Max(c => c.CellPosition.XOffset + c.CellPosition.XFillSize);
            int minX = cells.Min(c => c.CellPosition.XOffset);
            int maxY = cells.Max(c => c.CellPosition.YOffset + c.CellPosition.YFillSize);
            int minY = cells.Min(c => c.CellPosition.YOffset);

            Bitmap png = new Bitmap(maxX - minX, maxY - minY);

            // Draw each cell in its designated area
            foreach (IMImageCell cell in cells)
                for (int y = 0; y < cell.CellPosition.YFillSize; y++)
                    for (int x = 0; x < cell.CellPosition.XFillSize; x++)
                    {
                        int xMaxIndex = cell.ImageData.GetUpperBound(0);
                        int yMaxIndex = cell.ImageData.GetUpperBound(1);
                        int xIndexToCheck = x;
                        int yIndexToCheck = y;

                        // Handle tiling
                        while (xIndexToCheck > xMaxIndex) xIndexToCheck -= xMaxIndex;
                        while (yIndexToCheck > yMaxIndex) yIndexToCheck -= yMaxIndex;

                        int bmpX = (cell.CellPosition.XOffset + x) - minX;
                        int bmpY = (cell.CellPosition.YOffset + y) - minY;

                        if (cell.ImageData[xIndexToCheck, yIndexToCheck])
                            png.SetPixel(bmpX, bmpY, descriptor.ImageColor);
                    }

            // Get resolution from descriptor
            float xScale = (float)Converters.GetInches(png.Width, descriptor.XUnitsPerBase, descriptor.BaseUnit);
            float yScale = (float)Converters.GetInches(png.Height, descriptor.YUnitsPerBase, descriptor.BaseUnit);
            png.SetResolution((int)Math.Round(png.Width / xScale), (int)Math.Round(png.Height / yScale));

            return png;
        }
    }
}
