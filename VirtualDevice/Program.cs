using S7.Net;

namespace virtualdevice {
    public class Program {
        public static void Main() {
            var plc = new Plc(CpuType.S7300, "127.0.0.1", 0, 2);
            var hardware = new Hardware(plc);
            hardware.AddDevice(new Ama0801("ama0801", 100));
            hardware.Run();
        }
    }
}