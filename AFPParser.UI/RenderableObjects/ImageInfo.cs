namespace AFPParser.UI
{
    public class ImageInfo
    {
        // Tiles may have positional offsets relative to the top left of the first tile
        public uint XOffset { get; set; }
        public uint YOffset { get; set; }

        public byte[] Data { get; set; }
        public byte[] TransparencyMask { get; set; }

        public ImageInfo()
        {
            Data = new byte[0];
            TransparencyMask = new byte[0];
        }
    }
}
