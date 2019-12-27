using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualdevice
{
    public abstract class VirtualDevice
    {
        private bool inited = false;
        protected readonly Plc plc;
        public VirtualDevice(Plc plc)
        {
            this.plc = plc;
        }
        private void _init()
        {
            init();
            inited = true;
        }
        public void _run()
        {
            if (!inited)
            {
                _init();
            }
            run();
        }

        public abstract void init();

        public abstract void run();
    }

}
