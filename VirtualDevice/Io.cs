using System;
using System.Runtime.InteropServices;

namespace virtualdevice
{
    static class Io
    {
        public static bool ReadBit(byte[] data, int index, byte bit)
        {
            if (index >= 0 && index < data.Length && bit < 8)
            {
                return (data[index] & (0x01 << bit)) != 0;
            }

            return false;
        }

        public static byte[] StructToBytes(object instance)
        {
            int length = Marshal.SizeOf(instance);
            IntPtr buffer = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(instance, buffer, false);
            byte[] data = new byte[length];
            Marshal.Copy(buffer, data, 0, length);
            Marshal.FreeHGlobal(buffer);
            return data;
        }

        public static T BytesToStruct<T>(byte[] data)
        {
            int length = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocHGlobal(length);
            Marshal.Copy(data, 0, buffer, length);
            var instance = Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);
            return (T) instance;
        }

        public static void BytesToStruct(byte[] data, object instance)
        {
            int length = Marshal.SizeOf(instance);
            IntPtr buffer = Marshal.AllocHGlobal(length);
            Marshal.Copy(data, 0, buffer, length);
            Marshal.PtrToStructure(buffer, instance);
            Marshal.FreeHGlobal(buffer);
        }
    }
}