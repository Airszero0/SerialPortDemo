using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace serialPortTest
{
    public class GloableVirables
    {
        public static string PortName = string.Empty;              //串口号
        public static int BuadRate = 0;                //波特率
        public static int dataBits = 0;               //数据位
        public static Parity parity = 0;             //校验位
        public static StopBits stopbits = 0;         //停止位
    }
}
