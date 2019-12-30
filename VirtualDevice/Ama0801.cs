using S7.Net;
using System;
using System.Runtime.InteropServices;

namespace virtualdevice
{
    class Ama0801 : VirtualDevice
    {
        private readonly string _name;
        private readonly int _addr;
        private bool _k100 = false;
        private readonly int[] _posValues = new int[16];
        private int _window = 0;
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

        private readonly bool[] _sensors = new bool[16];


        private byte[] _outData = new byte[4];
        private byte[] _inData = new byte[4];

        public Ama0801(Plc plc, string name, int addr) : base(plc, name)
        {
            _addr = addr;
        }

        public Ama0801 WithPos(int index, int value)
        {
            if (index >= 0 && index < _posValues.Length)
            {
                _posValues[index] = value;
            }

            return this;
        }

        public Ama0801 WithWindow(int window)
        {
            _window = window;
            return this;
        }

        public Ama0801 WithSpeed(int speed)
        {
            _speed = speed;
            return this;
        }

        public Ama0801 WithLimit(int left, int right)
        {
            _leftLimit = left;
            _rightLimit = right;
            return this;
        }

        public override void Read()
        {
            _outData = Plc.ReadBytes(DataType.Output, 0, _addr, 4);
        }

        public override void Write()
        {
            Plc.WriteBytes(DataType.Input, 0, _addr, _inData);
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
        public override void Run()
        {
            var inhibit = Io.ReadBit(_outData, 1, 0);
            var notEStop = Io.ReadBit(_outData, 1, 1);
            var notStop = Io.ReadBit(_outData, 1, 2);
            if (!(_k100 & !inhibit & notEStop & notStop)) return;
            var start = Io.ReadBit(_outData, 0, 0);
            var jogCw = Io.ReadBit(_outData, 0, 1);
            var jogCcw = Io.ReadBit(_outData, 0, 2);

            var mode = _outData[0] & 0b_0011_1000;

            if (start && mode == (int) Mode.Pos)
            {
                var fastMode = Io.ReadBit(_outData, 1, 7);
                var speed = fastMode ? _speed : (_speed * 0.1);

                var index = Io.GetOneIndex(Io.ReadShort(_outData, 3));
                if (index < 0 || index >= 16) return;
                var targetPos = _posValues[index];

                if (!(_actualPos > _leftLimit) || !(_actualPos < _rightLimit) ||
                    Math.Abs(_actualPos - targetPos) < 0.01) return;
                if (_actualPos - targetPos > speed)
                {
                    _actualPos -= speed;
                }
                else if (targetPos - _actualPos > speed)
                {
                    _actualPos += speed;
                }
                else
                {
                    _actualPos = targetPos;
                }

                _axisSync = true;
            }
            else if (mode == (int) Mode.Jog)
            {
                var speed = _speed * 0.01;
                if (jogCw)
                {
                    _actualPos += speed;
                    _axisSync = true;
                }
                else if (jogCcw)
                {
                    _actualPos -= speed;
                    _axisSync = true;
                }
            }

            // FINISH
            _k100K = _k100;
            _nK100K = !_k100;
            _beRef = _actualPos > -_window && _actualPos < _window;
            _qkRxK = false;
            _fqm = true;
            _fqr = true;
            _bbm = true;
            _bbr = true;
            for (var i = 0; i < 16; i++)
            {
                _sensors[i] = _actualPos >= _posValues[i] - _window && _actualPos <= _posValues[i] + _window;
            }
        }
    }

    internal enum Mode
    {
        Jog = 0b_0000_0000,
        Pos = 0b_0001_0000,
        Ref = 0b_0010_0000,
        Tech = 0b_0010_1000
    }
}