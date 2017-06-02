using System.Linq;
using AFPParser.StructuredFields;
using System.Collections.Generic;

namespace AFPParser.Containers
{
    public class ImageObjectContainer : Container
    {
        public byte[] ImageData { get; set; }

        public override void ParseContainerData()
        {
            // Combine all IPD data bytes
            byte[] ipdData = GetFields<IPD>().SelectMany(f => f.Data).ToArray();

            // Get IPD SDF list
            List<ImageSelfDefiningField> allIPDFields = ImageSelfDefiningField.GetAllSDFs(ipdData);

            // Load image data from SDF list
            ImageData = allIPDFields.SelectMany(f => f.Data).ToArray();
        }
    }
}