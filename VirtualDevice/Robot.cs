using S7.Net;

namespace virtualdevice
{
    public class Robot : VirtualDevice
    {
        public Robot(Plc plc, string name) : base(plc, name)
        {
        }

        public override void Read()
        {
            throw new System.NotImplementedException();
        }

        public override void Write()
        {
            throw new System.NotImplementedException();
        }

        public override void Run()
        {
            throw new System.NotImplementedException();
        }
    }
}