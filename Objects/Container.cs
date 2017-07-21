using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AFPParser
{
    // A basic container simply holds a group of fields.
    [DebuggerDisplay("{Structures[0].HexIDStr}, ({Structures.Count})")]
    public class Container
    {
        // A list of data structures in this container
        public List<DataStructure> Structures { get; set; }
        // A list of data structures that have this container as the lowest level container
        public IReadOnlyList<DataStructure> DirectStructures => Structures.Where(s => s.LowestLevelContainer == this).ToList();

        public Container()
        {
            Structures = new List<DataStructure>();
        }

        // Get the single field in this container of a specific type
        public T GetStructure<T>() where T : DataStructure
        {
            return Structures.OfType<T>().FirstOrDefault();
        }

        // Get a list of fields in this container which are of a specific type
        public List<T> GetStructures<T>() where T : DataStructure
        {
            return Structures.OfType<T>().ToList();
        }

        // Get the single field in this container of a specific type
        public T DirectGetStructure<T>() where T : DataStructure
        {
            return DirectStructures.OfType<T>().FirstOrDefault();
        }

        // Get a list of fields in this container which are of a specific type
        public List<T> DirectGetStructures<T>() where T : DataStructure
        {
            return DirectStructures.OfType<T>().ToList();
        }
    }
}