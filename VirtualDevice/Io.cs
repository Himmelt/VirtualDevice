namespace virtualdevice {
    internal static class Io {
        public static bool ReadBit(byte[] data, int index, byte bit) {
            return (data[index] & (0x01 << bit)) != 0;
        }

        public static void WriteBit(byte[] data, int index, byte bit, bool flag) {
            data[index] = flag ? (byte) (data[index] | (0x01 << bit)) : (byte) (data[index] & ~(0x01 << bit));
        }

        public static void WriteBits(byte[] data, int index, bool[] values) {
            for (var i = 0; i < values.Length; i++) {
                var offset = i / 8;
                data[index + offset] = values[i]
                    ? (byte) (data[index + offset] | (0x01 << i % 8))
                    : (byte) (data[index + offset] & ~(0x01 << i % 8));
            }
        }

        public static short ReadShort(byte[] data, int index) {
            int value = data[index];
            return (short) (value << 8 + data[index + 1]);
        }

        public static void WriteInt(byte[] data, int index, int value) {
            data[index] = (byte) (value & 0xff);
            data[index + 1] = (byte) (value >> 8 & 0xff);
            data[index + 2] = (byte) (value >> 16 & 0xff);
            data[index + 3] = (byte) (value >> 24 & 0xff);
        }

        public static sbyte seekOne(byte value) {
            for (sbyte i = 0; i < 8; i++) {
                value >>= 1;
                if (value != 0) {
                    continue;
                }

                return i;
            }

            return -1;
        }
    }
}