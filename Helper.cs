using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace serialPortTest
{
    public class Helper
    { 
        public int ConvertHex(char ch)
        {
            if (ch >= '0' && ch <= '9')
            {
                return (int)(ch - '0');
            }
            if (ch >= 'A' && ch <= 'F')
            {
                return (int)(ch - 'A' + '\n');
            }
            if (ch >= 'a' && ch <= 'f')
            {
                return (int)(ch - 'a' + '\n');
            }
            return -1;
        }

        public char Hex_Ascii(byte a)
        {
            char result = '0';
            if (a == 48)
            {
                result = '0';
            }
            else if (a == 49)
            {
                result = '1';
            }
            else if (a == 50)
            {
                result = '2';
            }
            else if (a == 51)
            {
                result = '3';
            }
            else if (a == 52)
            {
                result = '4';
            }
            else if (a == 53)
            {
                result = '5';
            }
            else if (a == 54)
            {
                result = '6';
            }
            else if (a == 55)
            {
                result = '7';
            }
            else if (a == 56)
            {
                result = '8';
            }
            else if (a == 57)
            {
                result = '9';
            }
            else if (a == 46)
            {
                result = '.';
            }
            return result;
        }
    }
}
