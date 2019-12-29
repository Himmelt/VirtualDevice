using S7.Net;
using System;

namespace virtualdevice
{
    public class Ventil : VirtualDevice
    {
        public Ventil(Plc plc,string name) : base(plc,name)
        {
        }

        public override void Read()
        {
            throw new NotImplementedException();
        }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}