using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Common.Domain
{
    public static class GuidBuilder
    {
        public static readonly Guid DNSNamespaceId = new Guid("10000000-1111-0000-0000-000000000001");
        public static Guid Create(string name)
        {
            var assemblyName = Assembly.GetExecutingAssembly().FullName!;
            var input = $"{assemblyName}:{name}";

            return CreateUuid5(DNSNamespaceId, input);
        }

        private static Guid CreateUuid5(Guid namespaceId, string name)
        {
            byte[] namespaceBytes = namespaceId.ToByteArray();
            SwapByteOrder(namespaceBytes);

            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            byte[] hash;
            using (var sha1 = SHA1.Create())
            {
                sha1.TransformBlock(namespaceBytes, 0, namespaceBytes.Length, null, 0);
                sha1.TransformFinalBlock(nameBytes, 0, nameBytes.Length);
                hash = sha1.Hash!;
            }

            var newGuid = new byte[16];
            Array.Copy(hash, 0, newGuid, 0, 16);

            // Set version to 5
            newGuid[6] = (byte)((newGuid[6] & 0x0F) | (5 << 4));

            // Set variant
            newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

            SwapByteOrder(newGuid);
            return new Guid(newGuid);
        }

        private static void SwapByteOrder(byte[] guid)
        {
            void Swap(int a, int b)
            {
                (guid[a], guid[b]) = (guid[b], guid[a]);
            }

            Swap(0, 3);
            Swap(1, 2);
            Swap(4, 5);
            Swap(6, 7);
        }
    }
}
