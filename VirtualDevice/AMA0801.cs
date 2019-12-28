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
        private int[] _posValues = new int[16];
        private int _window = 0;
        private int _speed = 0;
        private int _leftLimit, _rightLimit;

        private readonly Control _control = new Control();

        // OUT
        private bool _k100K = false;
        private bool _nK100K = false;
        private bool _beRef = false;
        private bool _qkRxK = false;
        private bool _fqm = false;
        private bool _fqr = false;
        private bool _bbm = false;
        private bool _bbr = false;
        
        public Ama0801(Plc plc, int addr) : base(plc)
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

        public override void Init()
        {
        }

        public override void Run()
        {
            Io.BytesToStruct(Plc.ReadBytes(DataType.Output, 0, _addr, 4), _control);
            
            if (_k100 & !_control.inhibit & _control.notEStop & _control.notStop)
            {
                bool posMode = !_control.mode0 && _control.mode1 && !_control.mode2;
                // bool jogMode = !_control.mode0 && !_control.mode1 && !_control.mode2;
                // bool techMode = _control.mode0 && !_control.mode1 && _control.mode2;
                // bool refMode = _control.mode0 && !_control.mode1 && !_control.mode2;

                if (_control.start && posMode)
                {
                    if (!_control.fastMode)
                    {
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Control
    {
        internal readonly bool start; // A0.0
        internal readonly bool jogCw; // A0.1
        internal readonly bool jogCcw; // A0.2
        internal readonly bool mode0; // A0.3
        internal readonly bool mode1; // A0.4
        internal readonly bool mode2; // A0.5
        private readonly bool A06; // A0.6
        private readonly bool A07; // A0.7

        internal readonly bool inhibit; // A1.0
        internal readonly bool notEStop; // A1.1
        internal readonly bool notStop; // A1.2
        private readonly bool A13; // A1.3
        private readonly bool A14; // A1.4
        private readonly bool A15; // A1.5
        private readonly bool A16; // A1.6
        internal readonly bool fastMode; // A1.7

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.Bool)]
        readonly bool[] targetPos; // AB3 - AB4
        
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Status
    {
        int stayus';'
    }
}