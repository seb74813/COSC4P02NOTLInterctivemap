using System;
using System.Security.Cryptography;
using System.Text;

namespace Notl.MuseumMap.Core.Tools
{
    /// <summary>
    /// Extension class for GUID to generate partition keys.
    /// </summary>
    public static class PartitionExtention
    {
        /// <summary>
        /// Generates a partition key for the specific type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string PartitionKey(this Guid id)
        {
            return Partition.Calculate(id.ToString());
        }
    }

    /// <summary>
    /// Partition calculation tool.
    /// </summary>
    public class Partition
    {
        private const int NumberOfPartitions = 1000;
        readonly static MD5 md5 = MD5.Create();

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Partition()
        {
        }

        /// <summary>
        /// Generates a synthetic partition key based on the ID and class type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string Calculate(Guid id)
        {
            return Calculate(id.ToString());
        }

        /// <summary>
        /// Generates a synthetic partition key based on the ID and class type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string Calculate(string id)
        {
            var bytes = Encoding.UTF8.GetBytes(id);
            var hashedValue = md5.ComputeHash(bytes);
            var asInt = BitConverter.ToInt32(hashedValue, 0);
            asInt = asInt == int.MinValue ? asInt + 1 : asInt;
            return $"{Math.Abs(asInt) % NumberOfPartitions}";
        }
    }
}
