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
            switch (value) {
                case 1 << 0: return 0;
                case 1 << 1: return 1;
                case 1 << 2: return 2;
                case 1 << 3: return 3;
                case 1 << 4: return 4;
                case 1 << 5: return 5;
                case 1 << 6: return 6;
                case 1 << 7: return 7;
                default: return -1;
            }
        }
    }
}