using System;

namespace virtualdevice {
    class Ama0801 : VirtualDevice {
        private readonly string _name;
        private readonly int _addr;
        private bool _k100 = false;
        private readonly int[] _posValues = new int[16];
        private readonly int[] _camValues = new int[16];
        private int _window = 0;
        private int _offset = 0;
        private int _speed = 0;
        private int _leftLimit, _rightLimit;
        private double _actualPos;

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

        private byte[] _outData = new byte[4];
        private byte[] _inData = new byte[4];

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

        public Ama0801 WithSensorOffset(int offset) {
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
            for (var i = 0; i < 4; i++) {
                _outData[i] = outBytes[_addr + i];
            }
        }

        public override void Write(byte[] inBytes) {
            for (var i = 0; i < 4; i++) {
                inBytes[_addr + i] = _inData[i];
            }
        }

        /// <summary>
        /// AMA_A
        /// inhibit    : BOOL;// A1.0 controller inhibit/enable
        /// notEStop   : BOOL;// A1.1 enable/rapid stop
        /// notStop    : BOOL;// A1.2 enable/stop
        /// reserve1   : BOOL;// A1.3 reserved
        /// rampScaling: BOOL;// A1.4 ramp scaling
        /// reserve2   : BOOL;// A1.5 reserved
        /// reset      : BOOL;// A1.6 reset
        /// fastMode   : BOOL;// A1.7 fast fast/slow fast
        /// //////////////////////////////////////////////////////
        /// start      : BOOL;// A0.0 start
        /// jogCW      : BOOL;// A0.1 jog cw
        /// jogCCW     : BOOL;// A0.2 jog ccw
        /// mode_0     : BOOL;// A0.3 mode 2^0
        /// mode_1     : BOOL;// A0.4 mode 2^1
        /// mode_2     : BOOL;// A0.5 mode 2^2
        /// synchronize: BOOL;// A0.6 synchronize offset
        /// softLimit  : BOOL;// A0.7 software limit switch off
        /// ///////////////////////////////////////////////////////
        /// targetPos1 : BOOL;// A3.0 single bit position 1
        /// targetPos2 : BOOL;// A3.1 single bit position 2
        /// targetPos3 : BOOL;// A3.2 single bit position 3
        /// targetPos4 : BOOL;// A3.3 single bit position 4
        /// </summary>
        public override void Run() {
            var inhibit = Io.ReadBit(_outData, 1, 0);
            var notEStop = Io.ReadBit(_outData, 1, 1);
            var notStop = Io.ReadBit(_outData, 1, 2);
            var start = Io.ReadBit(_outData, 0, 0);
            var mode = _outData[0] & 0b_0011_1000;
            var targetPos = Io.GetOneIndex(Io.ReadShort(_outData, 3));
            if (_k100 && !inhibit && notEStop && notStop) {
                var jogCw = Io.ReadBit(_outData, 0, 1);
                var jogCcw = Io.ReadBit(_outData, 0, 2);

                if (start && mode == (int) Mode.Pos) {
                    var fastMode = Io.ReadBit(_outData, 1, 7);
                    var speed = fastMode ? _speed : (_speed * 0.1);

                    if (targetPos < 0 || targetPos >= 16) return;
                    var targetValue = _posValues[targetPos];

                    if (_actualPos > _leftLimit && _actualPos < _rightLimit &&
                        !(Math.Abs(_actualPos - targetValue) < 0.01)) {
                        if (_actualPos - targetValue > speed) {
                            _actualPos -= speed;
                        }
                        else if (targetValue - _actualPos > speed) {
                            _actualPos += speed;
                        }
                        else {
                            _actualPos = targetValue;
                        }

                        _axisSync = true;
                    }
                    else {
                        return;
                    }
                }
                else if (mode == (int) Mode.Jog) {
                    var speed = _speed * 0.01;
                    if (jogCw) {
                        _actualPos += speed;
                        _axisSync = true;
                    }
                    else if (jogCcw) {
                        _actualPos -= speed;
                        _axisSync = true;
                    }
                }
            }

            var actualValue = (int) Math.Round(_actualPos);

            // FINISH
            _k100K = _k100;
            _nK100K = !_k100;
            _beRef = actualValue > -_offset && actualValue < _offset;
            _qkRxK = false;
            _fqm = true;
            _fqr = true;
            _bbm = true;
            _bbr = true;
            var inPosition = false;
            for (var i = 0; i < 16; i++) {
                _signals[i] = Math.Abs(actualValue - _posValues[i]) <= _window && (targetPos == i || !start);
                _sensors[i] = Math.Abs(actualValue - _posValues[i]) <= _offset;
                _cameras[i] = Math.Abs(actualValue - _camValues[i]) <= _window;
                inPosition = inPosition || _signals[i];
            }

            var invReady = _k100;
            const bool iposRef = true;
            var notBreak = start;
            const bool fault = false;
            var limitCw = actualValue >= _rightLimit;
            var limitCcw = actualValue <= _leftLimit;

            /* --------------------------- Write AMA_E --------------------------- */
            _inData[0] = 0; // EB 0
            Io.WriteBit(_inData, 1, 0, _axisSync); // E1.0
            Io.WriteBit(_inData, 1, 1, invReady); // E1.1
            Io.WriteBit(_inData, 1, 2, iposRef); // E1.2
            Io.WriteBit(_inData, 1, 3, inPosition); // E1.3
            Io.WriteBit(_inData, 1, 4, notBreak); // E1.4
            Io.WriteBit(_inData, 1, 5, fault); // E1.5
            Io.WriteBit(_inData, 1, 6, limitCw); // E1.6
            Io.WriteBit(_inData, 1, 7, limitCcw); // E1.7

            Io.WriteInt(_inData, 2, actualValue); // ED 2
            Io.WriteBits(_inData, 7, _signals); // ED 7
            Io.WriteBits(_inData, 9, _cameras); // ED 9
        }
    }

    internal enum Mode {
        Jog = 0b_0000_0000,
        Pos = 0b_0001_0000,
        Ref = 0b_0010_0000,
        Tech = 0b_0010_1000
    }
}