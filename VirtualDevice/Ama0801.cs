using System;

namespace virtualdevice {
    class Ama0801 : VirtualDevice {
        // 固有属性
        private readonly string _name;
        private readonly int _addr;
        private int _window = 0;
        private int _offset = 0;
        private int _speed = 0;

        private int _leftLimit, _rightLimit;

        // 外部信号
        private bool _k100 = false;

        // 内部变量
        private bool start = false; // A0.0 start
        private bool jogCW = false; // A0.1 jog cw

        private bool jogCCW = false; // A0.2 jog ccw

        // private bool mode_0 = false; // A0.3 mode 2^0
        // private bool mode_1 = false; // A0.4 mode 2^1
        // private bool mode_2 = false; // A0.5 mode 2^2
        private int mode = -1;
        private bool synchronize = false; // A0.6 synchronize offset
        private bool softLimit = false; // A0.7 software limit switch off
        private bool inhibit = false; // A1.0 controller inhibit/enable
        private bool notEStop = false; // A1.1 enable/rapid stop
        private int targetPos = -1;
        private bool notStop = false; // A1.2 enable/stop
        private bool jogCw = false;
        private bool jogCcw = false;

        //private bool reserve1 = false; // A1.3 reserved
        private bool rampScaling = false; // A1.4 ramp scaling

        //private bool reserve2 = false; // A1.5 reserved
        private bool reset = false; // A1.6 reset
        private bool fastMode = false; // A1.7 fast fast/slow fast

        private bool invReady = false;
        const bool iposRef = true;
        private bool notBreak = false;
        const bool fault = false;
        private bool limitCw = false;
        private bool limitCcw = false;
        private bool inPosition = false;

        private readonly int[] _posValues = new int[16];
        private readonly int[] _camValues = new int[16];

        private long lastTick = 0;
        private int actualValue = 0;
        private double _actualPos = 0.0;

        // OUT
        private bool _axisSync = false;
        private bool _k100K = false;
        private bool _nK100K = false;
        private bool _beRef = false;
        private bool _qkRxK = false;
        private bool _fqm = false;
        private bool _fqr = false;
        private bool _bbm = false;
        private bool _bbr = false;

        private readonly bool[] _signals = new bool[16];
        private readonly bool[] _sensors = new bool[16];
        private readonly bool[] _cameras = new bool[16];

        private const byte Jog = 0b_0000_0000;
        private const byte Pos = 0b_0001_0000;
        private const byte Ref = 0b_0010_0000;
        private const byte Tech = 0b_0010_1000;

        public Ama0801(string name, int addr) : base(name) {
            _addr = addr;
        }

        public Ama0801 WithPos(int index, int value) {
            if (index >= 0 && index < _posValues.Length) {
                _posValues[index] = value;
            }

            return this;
        }

        public Ama0801 WithWindow(int window) {
            _window = window;
            return this;
        }

        public Ama0801 WithOffset(int offset) {
            _offset = offset;
            return this;
        }

        public Ama0801 WithSpeed(int speed) {
            _speed = speed;
            return this;
        }

        public Ama0801 WithLimit(int left, int right) {
            _leftLimit = left;
            _rightLimit = right;
            return this;
        }

        public override void Read(byte[] outBytes) {
            start     = Io.ReadBit(outBytes, _addr + 0, 0);
            jogCw     = Io.ReadBit(outBytes, _addr + 0, 1);
            jogCcw    = Io.ReadBit(outBytes, _addr + 0, 2);
            inhibit   = Io.ReadBit(outBytes, _addr + 1, 0);
            notEStop  = Io.ReadBit(outBytes, _addr + 1, 1);
            notStop   = Io.ReadBit(outBytes, _addr + 1, 2);
            fastMode  = Io.ReadBit(outBytes, _addr + 1, 7);

            mode = outBytes[_addr + 0] & 0b_0011_1000;

            targetPos = Io.seekOne(outBytes[_addr + 3]);
            if (targetPos == -1) {
                targetPos = Io.seekOne(outBytes[_addr + 4]);
                if (targetPos != -1) {
                    targetPos += 8;
                }
            }
        }

        public override void Write(byte[] inBytes) {
            inBytes[_addr + 0] = 0; // EB 0
            Io.WriteBit(inBytes, _addr + 1, 0, _axisSync); // E1.0
            Io.WriteBit(inBytes, _addr + 1, 1, invReady); // E1.1
            Io.WriteBit(inBytes, _addr + 1, 2, iposRef); // E1.2
            Io.WriteBit(inBytes, _addr + 1, 3, inPosition); // E1.3
            Io.WriteBit(inBytes, _addr + 1, 4, notBreak); // E1.4
            Io.WriteBit(inBytes, _addr + 1, 5, fault); // E1.5
            Io.WriteBit(inBytes, _addr + 1, 6, limitCw); // E1.6
            Io.WriteBit(inBytes, _addr + 1, 7, limitCcw); // E1.7

            Io.WriteInt(inBytes, _addr + 2, actualValue); // ED 2
            Io.WriteBits(inBytes, _addr + 7, _signals); // ED 7
            Io.WriteBits(inBytes, _addr + 9, _cameras); // ED 9
        }

        public override void Run() {
            var tick =  DateTime.Now.Ticks;
            if (tick - lastTick >= 1000 && _k100 && !inhibit && notEStop && notStop) {
                lastTick = tick;
                if (start && mode == Pos) {
                    var speed = fastMode ? _speed : (_speed * 0.1);

                    if (targetPos < 0 || targetPos >= 16) return;
                    var targetValue = _posValues[targetPos];

                    if (_actualPos > _leftLimit && _actualPos < _rightLimit && !(Math.Abs(_actualPos - targetValue) < 0.01)) {
                        if (_actualPos - targetValue > speed) {
                            _actualPos -= speed;
                        } else if (targetValue - _actualPos > speed) {
                            _actualPos += speed;
                        } else {
                            _actualPos = targetValue;
                        }

                        _axisSync = true;
                    } else {
                        return;
                    }
                } else if (mode == Jog) {
                    var speed = _speed * 0.01;
                    if (jogCw) {
                        _actualPos += speed;
                        _axisSync = true;
                    } else if (jogCcw) {
                        _actualPos -= speed;
                        _axisSync = true;
                    }
                }

                actualValue = (int) Math.Round(_actualPos);
                _beRef = actualValue > -_offset && actualValue < _offset;
                inPosition = false;
                for (var i = 0; i < 16; i++) {
                    _signals[i] = Math.Abs(actualValue - _posValues[i]) <= _window && (targetPos == i || !start);
                    _sensors[i] = Math.Abs(actualValue - _posValues[i]) <= _offset;
                    _cameras[i] = Math.Abs(actualValue - _camValues[i]) <= _window;
                    inPosition = inPosition || _signals[i];
                }

                limitCw = actualValue >= _rightLimit;
                limitCcw = actualValue <= _leftLimit;
            }

            // FINISH
            _k100K = _k100;
            _nK100K = !_k100;
            _qkRxK = false;
            _fqm = true;
            _fqr = true;
            _bbm = true;
            _bbr = true;
            invReady = _k100;
            notBreak = start;
        }
    }
}