using System.Linq;
using AFPParser.StructuredFields;
using System.Collections.Generic;

namespace AFPParser.Containers
{
    public class IMImageContainer : Container
    {
        // An image is being represented as a two dimensional array [Xlen, Ylen]
        public bool[,] ImageData { get; private set; }
        public IReadOnlyList<Cell> Cells { get; private set; }

        public IMImageContainer()
        {
            Cells = new List<Cell>();
        }

        public override void ParseContainerData()
        {
            IID imageDescriptor = GetStructure<IID>();

            if (GetStructure<ICP>() == null)
            {
                // If there are no cells, just parse all the IRD bytes into two dimensions
                ImageData = new bool[imageDescriptor.XSize, imageDescriptor.YSize];

                int concatCounter = 0;
                byte[] concatData = GetStructures<IRD>().SelectMany(d => d.Data).ToArray();

                for (int y = 0; y < imageDescriptor.YSize; y++)
                    for (int x = 0; x < imageDescriptor.XSize; x += 8)
                        for (int b = 0; b < 8; b++)
                            ImageData[x + b, y] = (concatData[concatCounter++] & (1 << (7 - b))) > 0;
            }
            else
            {
                // Manually parse a list of cells since they don't have their own container
                List<Cell> cellList = new List<Cell>();
                for (int i = 0; i < Structures.Count; i++)
                {
                    if (Structures[i].GetType() != typeof(ICP)) continue;

                    // Get list of IRDs up to the next ICP or end of structures
                    List<IRD> rasterData = new List<IRD>();
                    for (int r = i + 1; r < Structures.Count; r++)
                    {
                        if (Structures[r].GetType() != typeof(IRD)) break;
                        rasterData.Add((IRD)Structures[r]);
                    }

                    cellList.Add(new Cell((ICP)Structures[i], rasterData));
                }

                // Set x and y extent by finding max possible values
                int maxX = cellList.Max(c => c.CellPosition.XOffset + c.CellPosition.XSize);
                int maxY = cellList.Max(c => c.CellPosition.YOffset + c.CellPosition.YSize);
                int minX = cellList.Min(c => c.CellPosition.XOffset);
                int minY = cellList.Min(c => c.CellPosition.YOffset);

                int width = maxX - minX;
                int height = maxY - minY;
                ImageData = new bool[width, height];

                // Each cell has offset and size info - populate our 2 dimensional array based on that
                foreach (Cell c in cellList)
                {
                    int concatCounter = 0;
                    byte[] data = c.RasterData.SelectMany(d => d.Data).ToArray();
                    
                    for (int y = 0; y < c.CellPosition.YSize; y++)
                        for (int x = 0; x < c.CellPosition.XSize; x += 8)
                        {
                            byte curByte = data[concatCounter++];

                            for (int b = 0; b < 8; b++)
                            {
                                // Adjust offsets in case the minimum X or Y values are above 0. This allows us to capture the minimum area possible
                                int trueXOffset = c.CellPosition.XOffset - minX;
                                int trueYOffset = c.CellPosition.YOffset - minY;
                                ImageData[trueXOffset + x + b, trueYOffset + y] = (curByte & (1 << (7 - b))) > 0;
                            }
                        }
                }

                Cells = cellList;
            }
        }

        public class Cell
        {
            public ICP CellPosition { get; private set; }
            public List<IRD> RasterData { get; private set; }
            
            public Cell(ICP pos, List<IRD> rasterData)
            {
                CellPosition = pos;
                RasterData = rasterData;
            }
        }
    }
}