using S7.Net;

namespace virtualdevice
{
    public class Program
    {
        public static void Main()
        {
            var plc = new Plc(CpuType.S7300, "127.0.0.1", 0, 2);
            plc.Open();
            var config = new HardwareConfig();
            config.Register(new Ama0801(plc, "ama0801", 100));
            while (true)
            {
                config.Run();
            }
        }
    }
}