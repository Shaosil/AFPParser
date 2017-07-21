using System.Collections.Generic;
using System.Linq;

namespace AFPParser.UI
{
    public static class Extensions
    {
        public static List<string> GetNamesOfType(this IEnumerable<Resource> self, Resource.eResourceType rType)
        {
            // Return all non-blank names of resources of the specified type
            return self.Where(r => r.ResourceType == rType && !string.IsNullOrWhiteSpace(r.ResourceName))
                .Select(r => r.ResourceName).ToList();
        }

        public static Resource OfTypeAndName(this IEnumerable<Resource> self, Resource.eResourceType rType, string rName)
        {
            return self.FirstOrDefault(r => r.ResourceType == rType && r.ResourceName == rName);
        }
    }
}
