using S7.Net;

namespace virtualdevice
{
    public abstract class VirtualDevice
    {
        private bool _inited = false;
        protected readonly Plc Plc;

        protected VirtualDevice(Plc plc)
        {
            Plc = plc;
        }

        private void _init()
        {
            Init();
            _inited = true;
        }

        public void _run()
        {
            if (!_inited)
            {
                _init();
            }

            Run();
        }

        public abstract void Init();

        public abstract void Run();
    }
}