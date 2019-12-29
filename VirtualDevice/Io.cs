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

        public static byte ReadByte(byte[] data, int index, byte mask)
        {
            if (index >= 0 && index < data.Length)
            {
                return (byte) (data[index] & mask);
            }

            return 0x00;
        }
    }
}