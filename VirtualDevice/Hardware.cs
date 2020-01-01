using System.Collections.Generic;
using S7.Net;

namespace virtualdevice
{
    public class Hardware
    {
        private byte[] _in_bytes;
        private byte[] _out_bytes;
        private readonly Plc _plc;

        private readonly Dictionary<string, VirtualDevice> _devices = new Dictionary<string, VirtualDevice>();

        public Hardware(Plc plc)
        {
            _plc = plc;
        }

        public Hardware AddDevice(VirtualDevice device)
        {
            _devices.Add(device.Name, device);
            return this;
        }

        public Hardware AddDevice(params VirtualDevice[] devices)
        {
            foreach (var device in devices)
            {
                _devices.Add(device.Name, device);
            }

            return this;
        }

        public void Run()
        {
            /// Thread 1
            foreach (var device in _devices.Values)
            {
                device.Read();
                device.Run();
                device.Write();
            }

            /// Thread 2
            foreach (var device in _devices.Values)
            {
            }
        }
    }
}