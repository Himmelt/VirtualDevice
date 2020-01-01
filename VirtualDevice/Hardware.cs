using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S7.Net;

namespace virtualdevice {
    public class Hardware {
        private volatile bool _stop = true;
        private volatile byte[] _inBytes;
        private volatile byte[] _outBytes;
        private readonly Plc _plc;

        private readonly Dictionary<string, VirtualDevice> _devices = new Dictionary<string, VirtualDevice>();

        public Hardware(Plc plc) {
            _plc = plc;
        }

        public Hardware AddDevice(VirtualDevice device) {
            if (_stop) {
                _devices.Add(device.Name, device);
            }

            return this;
        }

        public Hardware AddDevice(params VirtualDevice[] devices) {
            foreach (var device in devices) {
                _devices.Add(device.Name, device);
            }

            return this;
        }

        public void Run() {
            _stop = false;
            // 启动 异步 循环读
            ReadAsync();
            // 启动 异步 循环写
            WriteAsync();
            // 启动 异步 循环运行
            CycleAsync();
            //// 当 停止 时 , 停止循环 , 结束异步任务
        }

        public void Stop() {
            _stop = true;
        }

        private async void ReadAsync() {
            await Task.Run(() => {
                while (!_stop) {
                    _outBytes = _plc.ReadBytes(DataType.Output, 0, 0, 3000);
                }
            });
        }

        private async void WriteAsync() {
            await Task.Run(() => {
                while (!_stop) {
                    _plc.WriteBytes(DataType.Input, 0, 0, _inBytes);
                }
            });
        }

        private async void CycleAsync() {
            await Task.Run(() => {
                while (!_stop) {
                    foreach (var device in _devices.Values.Where(device => !_stop)) {
                        device.Read(_outBytes);
                        device.Run();
                        device.Write(_inBytes);
                    }
                }
            });
        }
    }
}