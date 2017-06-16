using System.Linq;
using System.Collections.Generic;

namespace AFPParser
{
    public static class Extensions
    {
        public static List<string> GetNamesOfType(this IEnumerable<AFPFile.Resource> self, AFPFile.Resource.eResourceType rType)
        {
            // Return all non-blank names of resources of the specified type
            return self.Where(r => r.ResourceType == rType && !string.IsNullOrWhiteSpace(r.ResourceName))
                .Select(r => r.ResourceName).ToList();
        }

        public static AFPFile.Resource OfTypeAndName(this IEnumerable<AFPFile.Resource> self, AFPFile.Resource.eResourceType rType, string rName)
        {
            return self.FirstOrDefault(r => r.ResourceType == rType && r.ResourceName == rName);
        }
    }
}
