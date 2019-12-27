using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualdevice
{
    class IO
    {
        public static bool readBit(byte[] data,int index,byte bit)
        {
            if(index >=0 && index < data.Length && bit>=0 && bit<8)
            {
                return (data[index] & (0x01 << bit)) != 0;
            }
            return false;
        }
    }
}
