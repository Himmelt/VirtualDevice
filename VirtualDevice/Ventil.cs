using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualdevice
{
    public class Ventil : VirtualDevice
    {
        public Ventil(Plc plc) : base(plc) { }
        public override void Init()
        {
            throw new NotImplementedException();
        }
        public override void Run()
        {
            throw new NotImplementedException();
        }

    }
}
