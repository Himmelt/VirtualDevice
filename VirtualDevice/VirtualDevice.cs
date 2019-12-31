using System.Collections.Generic;
using S7.Net;

namespace virtualdevice
{
    public abstract class VirtualDevice
    {
        protected Plc Plc { get; }
        public string Name { get; }

        protected Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();

        protected VirtualDevice(Plc plc, string name)
        {
            Plc = plc;
            Name = name;
        }

        public abstract void Read();

        public abstract void Write();

        public abstract void Run();

        public void ReadWriteSymbols()
        {
            foreach (var symbol in _symbols.Values)
            {
                if (symbol.Type == DataType.Input)
                {
                    
                }
            }
        }
    }

    public class Symbol
    {
        public DataType Type { get; }
        public int Index { get; }
        public int Bit { get; }
        public bool Value { get; set; }
    }
}