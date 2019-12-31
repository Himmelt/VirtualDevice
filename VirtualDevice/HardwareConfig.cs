using System.Collections.Generic;

namespace virtualdevice
{
    public class HardwareConfig
    {
        private readonly Dictionary<string, VirtualDevice> _devices = new Dictionary<string, VirtualDevice>();

        public HardwareConfig Register(VirtualDevice device)
        {
            _devices.Add(device.Name, device);
            return this;
        }

        public HardwareConfig UnRegister(string name)
        {
            _devices.Remove(name);
            return this;
        }

        public HardwareConfig Register(params VirtualDevice[] devices)
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