using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // OUT
        private bool _k100K = false;
        private bool _nK100K = false;
        private bool _beRef = false;
        private bool _qkRxK = false;
        private bool _fqm = false;
        private bool _fqr = false;
        private bool _bbm = false;
        private bool _bbr = false;

        // TEMP
        private byte[] _statusData;
        private byte[] _controlData;
        public Ama0801(Plc plc, int addr) : base(plc)
        {
            this._addr = addr;
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
            this._window = window;
            return this;
        }

        public Ama0801 WithSpeed(int speed)
        {
            this._speed = speed;
            return this;
        }

        public Ama0801 WithLimit(int left, int right)
        {
            this._leftLimit = left;
            this._rightLimit = right;
            return this;
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            _controlData = Plc.ReadBytes(DataType.Output, 0, _addr, 4);
            bool inhibit = Io.ReadBit(_controlData, 1, 0);// A1.0
            bool notEStop = Io.ReadBit(_controlData, 1, 1);// A1.1
            bool notStop = Io.ReadBit(_controlData, 1, 2);// A1.2

            if (_k100 & !inhibit & notEStop & notStop)
            {
                bool fastMode = Io.ReadBit(_controlData, 1, 7);// A1.7
                bool start = Io.ReadBit(_controlData, 0, 0);// A0.0
                bool jogCw = Io.ReadBit(_controlData, 0, 1);// A0.1
                bool jogCcw = Io.ReadBit(_controlData, 0, 2);// A0.2
                bool mode0 = Io.ReadBit(_controlData, 0, 3);// A0.3
                bool mode1 = Io.ReadBit(_controlData, 0, 4);// A0.4
                bool mode2 = Io.ReadBit(_controlData, 0, 5);// A0.5
                bool targetPos1 = Io.ReadBit(_controlData, 3, 0);// A3.0
                bool targetPos2 = Io.ReadBit(_controlData, 3, 1);// A3.1
                bool targetPos3 = Io.ReadBit(_controlData, 3, 2);// A3.2
                bool targetPos4 = Io.ReadBit(_controlData, 3, 3);// A3.3

                bool posMode = !mode0 && mode1 && !mode2;
                bool jogMode = !mode0 && !mode1 && !mode2;
                bool techMode = mode0 && !mode1 && mode2;
                bool refMode = mode0 && !mode1 && !mode2;

                if (start && posMode)
                {
                    if (!fastMode)
                    {

                    }
                }
            }
        }
    }

    struct Control
    {
        bool _fastMode;// A1.7
        bool _start;// A0.0
        bool _jogCw;// A0.1
        bool _jogCcw;// A0.2
        bool _mode0;// A0.3
        bool _mode1;// A0.4
        bool _mode2;// A0.5
        //bool targetPos[];// A3.0
    }
}
