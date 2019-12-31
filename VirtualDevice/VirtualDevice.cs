using System.Collections.Concurrent;
using System.Collections.Generic;
using S7.Net;

namespace virtualdevice
{
    public abstract class VirtualDevice
    {
        protected Plc Plc { get; }
        public string Name { get; }

        private ConcurrentDictionary<string, Address> _symbols = new ConcurrentDictionary<string, Address>();

        protected VirtualDevice(Plc plc, string name)
        {
            Plc = plc;
            Name = name;
        }

        public abstract void Read();

        public abstract void Write();

        public abstract void Run();

        public VirtualDevice RegisterSymbol(string symbol, Address address)
        {
            _symbols.TryAdd(symbol, address);
            return this;
        }

        public void ReadWriteSymbols()
        {
            foreach (var symbol in _symbols.Values)
            {
                if (symbol.type == DataType.Input)
                {
                }
            }
        }
    }

    public class Address
    {
        public volatile bool value;
        public readonly DataType type;
        public readonly int index;
        public readonly int bit;

        public Address(DataType type, int index, int bit)
        {
            this.type = type;
            this.index = index;
            this.bit = bit;
        }
    }
}