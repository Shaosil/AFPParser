using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AFPParser
{
    // A basic container simply holds a group of fields.
    [DebuggerDisplay("{Fields[0].Abbreviation}, {Fields.Count} fields")]
    public class Container
    {
        public List<StructuredField> Fields { get; set; }

        public Container()
        {
            Fields = new List<StructuredField>();
        }

        // Get the single field in this container of a specific type
        public T GetField<T>() where T : StructuredField
        {
            return (T)Fields.FirstOrDefault(f => f.GetType() == typeof(T));
        }

        // Get a list of fields in this container which are of a specific type
        public List<T> GetFields<T>() where T : StructuredField
        {
            return Fields.Where(f => f.GetType() == typeof(T)).Cast<T>().ToList();
        }

        // Called after all fields have been added to the container
        public virtual void ParseContainerData()
        {
            // Nothing to do in a generic container
        }
    }
}