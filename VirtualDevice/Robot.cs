namespace virtualdevice {
    public class Robot : VirtualDevice {
        public Robot(string name) : base(name) {
        }

        public override void Read(byte[] outBytes) {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] inBytes) {
            throw new System.NotImplementedException();
        }

        public override void Run() {
            throw new System.NotImplementedException();
        }
    }
}