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

        public static short ReadShort(byte[] data, int index)
        {
            if (index >= 0 && index < data.Length - 1)
            {
                short value = data[index];
                return (short) (value << 8 + data[index + 1]);
            }

            return 0x00;
        }

        public static int GetOneIndex(short value)
        {
            for (int i = 0; i < 16; i++)
            {
                value >>= 1;
                if (value != 0)
                {
                    continue;
                }

                return i;
            }

            return -1;
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