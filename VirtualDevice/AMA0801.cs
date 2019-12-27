using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualdevice
{
    class AMA0801 : VirtualDevice
    {
        private readonly int addr;
        private bool K100 = false;
        private int[] posValues = new int[16];
        private int window = 0;
        private int speed = 0;
        private int leftLimit, rightLimit;

        // OUT
        private bool K100K = false;
        private bool nK100K = false;
        private bool BE_Ref = false;
        private bool QKRxK = false;
        private bool FQM = false;
        private bool FQR = false;
        private bool BBM = false;
        private bool BBR = false;

        // TEMP
        private byte[] status_data;
        private byte[] control_data;
        public AMA0801(Plc plc, int addr) : base(plc)
        {
            this.addr = addr;
        }

        public AMA0801 withPos(int index, int value)
        {
            if (index >= 0 && index < posValues.Length)
            {
                posValues[index] = value;
            }
            return this;
        }

        public AMA0801 withWindow(int window)
        {
            this.window = window;
            return this;
        }

        public AMA0801 withSpeed(int speed)
        {
            this.speed = speed;
            return this;
        }

        public AMA0801 withLimit(int left, int right)
        {
            this.leftLimit = left;
            this.rightLimit = right;
            return this;
        }

        public override void init()
        {
            throw new NotImplementedException();
        }

        public override void run()
        {
            control_data = plc.ReadBytes(DataType.Output, 0, addr, 4);
            bool inhibit = IO.readBit(control_data, 1, 0);// A1.0
            bool notEStop = IO.readBit(control_data, 1, 1);// A1.1
            bool notStop = IO.readBit(control_data, 1, 2);// A1.2

            if (K100 & !inhibit & notEStop & notStop)
            {
                bool fastMode = IO.readBit(control_data, 1, 7);// A1.7
                bool start = IO.readBit(control_data, 0, 0);// A0.0
                bool jogCW = IO.readBit(control_data, 0, 1);// A0.1
                bool jogCCW = IO.readBit(control_data, 0, 2);// A0.2
                bool mode_0 = IO.readBit(control_data, 0, 3);// A0.3
                bool mode_1 = IO.readBit(control_data, 0, 4);// A0.4
                bool mode_2 = IO.readBit(control_data, 0, 5);// A0.5
                bool targetPos1 = IO.readBit(control_data, 3, 0);// A3.0
                bool targetPos2 = IO.readBit(control_data, 3, 1);// A3.1
                bool targetPos3 = IO.readBit(control_data, 3, 2);// A3.2
                bool targetPos4 = IO.readBit(control_data, 3, 3);// A3.3

                bool posMode = !mode_0 && mode_1 && !mode_2;
                bool jogMode = !mode_0 && !mode_1 && !mode_2;
                bool techMode = mode_0 && !mode_1 && mode_2;
                bool refMode = mode_0 && !mode_1 && !mode_2;

                if (start && posMode)
                {
                    if (!fastMode)
                    {

                    }
                }
            }
        }
    }
}
