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
        static bool _waitforprinter;
        static bool _responseReady;
        static List<string> fullResponse = new List<string>();

        static void Main(string[] args)
        {
            Thread readThread = new Thread(Read);
            _continue = true;
            _waitforprinter = false;
            _responseReady = false;
            _PrinterPort = new SerialPort();

            _PrinterPort.PortName = SetPortName(_PrinterPort.PortName);
            _PrinterPort.BaudRate = 115200;

            _PrinterPort.WriteTimeout = 500;
            _PrinterPort.ReadTimeout = 1000;

            _PrinterPort.Open();

            
            while (_continue)
            {            
                if (!(_waitforprinter))
                {
                    readThread.Start();
                    _PrinterPort.WriteLine("G28");
                    _PrinterPort.WriteLine("G29 P5 V4");
                    _PrinterPort.WriteLine("G28");
                    _waitforprinter = true;
                }
                else if (_responseReady)
                {
                    ParseResponse(fullResponse);
                    _continue = false;
                }
            }

            Console.WriteLine("Press any key to stop reading");
            Console.ReadKey();
            _continue = false;

            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
            
            _PrinterPort.Close();
        }

        public static void Read()
        {
            while (!(_responseReady))
            {
                try
                {
                    
                    string message = _PrinterPort.ReadLine();
                    Console.WriteLine(message);
                    if (message.Contains("Bed X:")){
                        fullResponse.Add(message);
                    }
                    else if (message.Contains("ok N0 P15 B15"))
                    {
                        _responseReady = true;
                    }
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

        public static void ParseResponse(List<string> response)
        {
            Console.WriteLine("begin ParseResponse");
            bool first = true;
            int coordinateCount = 0;
            List<CoordinateTest> CoordinateCollection;
            foreach(string line in response)
            {
                String[] splitstrings = { "Bed X:", "Y:", "Z:" };
                string[] coordsstring = line.Split(splitstrings, System.StringSplitOptions.RemoveEmptyEntries);
                
                if (first) {CoordinateCollection.Add(new  }
                else { };

                int x = Convert.ToInt32(coordsstring[0]);
                int y = Convert.ToInt32(coordsstring[1]);
                decimal z = Convert.ToDecimal(coordsstring[2]);

                first = !(first);
            }
            
        }
        
    }
    public class CoordinateTest
    {
        int x;
        int y;
        decimal z1;
        decimal z2;

        public CoordinateTest(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

}
