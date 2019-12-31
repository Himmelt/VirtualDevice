namespace virtualdevice
{
    internal static class Io
    {
        public static bool ReadBit(byte[] data, int index, byte bit)
        {
            if (index >= 0 && index < data.Length && bit < 8)
            {
                return (data[index] & (0x01 << bit)) != 0;
            }

            return false;
        }

        public static void WriteBit(byte[] data, int index, byte bit, bool flag)
        {
            if (index >= 0 && index < data.Length && bit < 8)
            {
                data[index] = flag ? (byte) (data[index] | (0x01 << bit)) : (byte) (data[index] & ~(0x01 << bit));
            }
        }

        public static void WriteBits(byte[] data, int index, bool[] values)
        {
            var len = (values.Length + 7) / 8;
            if (index >= 0 && index + len < data.Length)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    var offset = i / 8;
                    data[index + offset] = values[i]
                        ? (byte) (data[index + offset] | (0x01 << i % 8))
                        : (byte) (data[index + offset] & ~(0x01 << i % 8));
                }
            }
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

        public static void WriteInt(byte[] data, int index, int value)
        {
            if (index >= 0 && index < data.Length - 4)
            {
                data[index] = (byte) (value & 0xff);
                data[index + 1] = (byte) (value >> 8 & 0xff);
                data[index + 2] = (byte) (value >> 16 & 0xff);
                data[index + 3] = (byte) (value >> 24 & 0xff);
            }
        }

        public static int GetOneIndex(short value)
        {
            for (var i = 0; i < 16; i++)
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