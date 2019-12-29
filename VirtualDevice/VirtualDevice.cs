using S7.Net;

namespace virtualdevice
{
    public abstract class VirtualDevice
    {
        public Plc Plc { get; }
        public string Name { get; }

        protected VirtualDevice(Plc plc, string name)
        {
            Plc = plc;
            Name = name;
        }

        public abstract void Read();

        public abstract void Write();

        public abstract void Run();
    }
}