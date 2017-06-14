using System.Linq;
using System.Collections.Generic;
using AFPParser.StructuredFields;

namespace AFPParser.Containers
{
    public class IOCAImageContainer : Container
    {
        public IReadOnlyList<ImageSelfDefiningField> SDFs { get; private set; }
        public IReadOnlyList<ImageContentContainer.ImageInfo> Images { get; private set; }

        public IOCAImageContainer()
        {
            SDFs = new List<ImageSelfDefiningField>();
        }

        public override void ParseContainerData()
        {
            // Combine all self defining fields from zero or more IPD fields
            byte[] allIPDData = GetStructures<IPD>().SelectMany(f => f.Data).ToArray();
            SDFs = ImageSelfDefiningField.GetAllSDFs(allIPDData);

            // Get all images in our self defining field list
            List<ImageContentContainer> imageContainers = SDFs.Select(s => s.LowestLevelContainer).OfType<ImageContentContainer>().Distinct().ToList();
            Images = imageContainers.SelectMany(c => c.ImageList).ToList();
        }
    }
}