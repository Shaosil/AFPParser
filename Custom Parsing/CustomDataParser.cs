using System.Reflection;

/*
    Some data is too complicated to parse just by reading in some offset data from text files.
    In these cases, the datatype will be "CUSTOM", and there should exist a partial class with
    a single method for said identifier/triplet. The naming scheme will always be the same so
    that it can be looked up via reflection.
*/

namespace AFPParser
{
    public partial class CustomDataParser
    {
        public static string ParseData(string objHex, Offset curOffset, byte[] data)
        {
            // Use reflection to find the method name that will correctly parse this field or triplet's data
            MethodInfo ourMethod = typeof(CustomDataParser).GetMethod($"X{objHex}", BindingFlags.Static | BindingFlags.NonPublic);
            ParameterInfo[] methodParms = ourMethod?.GetParameters();

            // Return its result
            if (methodParms != null && methodParms.Length == 2 && methodParms[0].ParameterType == typeof(Offset) && methodParms[1].ParameterType == typeof(byte[]))
                return (string)ourMethod.Invoke(null, new object[] { curOffset, data });
            else
                return $"(CUSTOM PARSER METHOD NOT FOUND FOR X{objHex})";
        }
    }
}
