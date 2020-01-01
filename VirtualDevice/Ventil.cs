using System;

namespace virtualdevice {
    public class Ventil : VirtualDevice {
        public Ventil(string name) : base(name) {
        }

        public override void Read(byte[] outBytes) {
            throw new NotImplementedException();
        }

        public override void Write(byte[] inBytes) {
            throw new NotImplementedException();
        }

        public override void Run() {
            throw new NotImplementedException();
        }
    }
}