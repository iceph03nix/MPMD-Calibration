using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;

namespace MPMD_Calibration
{
    class Program
    {
        static SerialPort _PrinterPort;
        static bool _continue;

        static void Main(string[] args)
        {
            Thread readThread = new Thread(Read);

            _PrinterPort = new SerialPort();

            _PrinterPort.PortName = SetPortName(_PrinterPort.PortName);
            _PrinterPort.BaudRate = 115200;

            _PrinterPort.WriteTimeout = 500;
            _PrinterPort.ReadTimeout = 1000;

            _PrinterPort.Open();
            _continue = true;
            readThread.Start();

            _PrinterPort.WriteLine("G29 P5");
            
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _PrinterPort.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }

        // Display Port values and prompt user to enter a port.
        public static string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }
    }
}
