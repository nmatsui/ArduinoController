using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace Arduino
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("起動方法: Arduino.exe COMPORT");
                return;
            }

            string comPort = args[0];
            Console.WriteLine("comPort = " + comPort);
            Console.WriteLine("bit1                : bit1(7bit)をArduinoへ送信する");
            Console.WriteLine("bit1 bit2 wait loop : bit1(7bit)とbit2(7bit)をwait/2を挟みながらloop回Arduinoへ送信する");
            Console.WriteLine("Q                   : 終了する");

            Regex re1 = new Regex(@"^([01]{7})$");
            Regex re2 = new Regex(@"^([01]{7}) ([01]{7}) (\d+) (\d+)$");

            SerialPort port = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            while (true)
            {
                string s = Console.ReadLine();
                if ("Q".Equals(s) || "q".Equals(s)) break;
                else if (re1.IsMatch(s))
                {
                    GroupCollection g = re1.Match(s).Groups;
                    port.Write(new byte[]{ Convert.ToByte(g[1].Value, 2) }, 0, 1);
                }
                else if (re2.IsMatch(s))
                {
                    GroupCollection g = re2.Match(s).Groups;
                    byte tick = Convert.ToByte(g[1].Value, 2);
                    byte tack = Convert.ToByte(g[2].Value, 2);
                    int wait = Convert.ToInt16(g[3].Value);
                    int loop = Convert.ToInt16(g[4].Value);
                    for (int i = 0; i < loop; i++)
                    {
                        port.Write(new byte[] { tick }, 0, 1);
                        System.Threading.Thread.Sleep(wait * 500);
                        port.Write(new byte[] { tack }, 0, 1);
                        System.Threading.Thread.Sleep(wait * 500);
                    }
                }
                
            }
            port.Close();
        }
    }
}
