using System;

namespace AFPParser
{
    // If a BEGIN tag is decorated with this attribute, it will create a container with the specified type instead of a generic one
    public class ContainerTypeAttribute : Attribute
    {
        public Type AssignedType { get; private set; }

        public ContainerTypeAttribute(Type t)
        {
            AssignedType = t;
        }
    }
}
