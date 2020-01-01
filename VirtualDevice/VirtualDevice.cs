using System.Collections.Concurrent;
using S7.Net;

namespace virtualdevice {
    public abstract class VirtualDevice {
        public string Name { get; }

        private ConcurrentDictionary<string, Address> _symbols = new ConcurrentDictionary<string, Address>();

        protected VirtualDevice(string name) {
            Name = name;
        }

        public abstract void Read(byte[] outBytes);

        public abstract void Write(byte[] inBytes);

        public abstract void Run();

        public VirtualDevice AddSymbol(string symbol, Address address) {
            _symbols.TryAdd(symbol, address);
            return this;
        }

        public void ReadWriteSymbols() {
            foreach (var symbol in _symbols.Values) {
                if (symbol.type == DataType.Input) {
                }
            }
        }
    }

    public class Address {
        public volatile bool value;
        public readonly DataType type;
        public readonly int index;
        public readonly int bit;

        public Address(DataType type, int index, int bit) {
            this.type = type;
            this.index = index;
            this.bit = bit;
        }
    }
}