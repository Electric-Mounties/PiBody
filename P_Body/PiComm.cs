using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace P_Body
{
    public class PiComm : IDisposable
    {
        private SerialPort serialPort = null;
        private const int port = 2021;
        private Thread dataThread = null;
        private string ipAddress = "192.168.43.124";
        public TcpClient server = null;
        public NetworkStream ns = null;

        public PiComm(string portName)
        {
                server = ConnectToServer(ipAddress);
                serialPort = ConnectToArduino();
                ns = server.GetStream();

                dataThread = new Thread(new ParameterizedThreadStart(serialDataReceived));
                dataThread.Start();
        }

        public PiComm()
        {
            server = ConnectToServer(ipAddress);
            serialPort = ConnectToArduino();
            ns = server.GetStream();
            
            dataThread = new Thread(new ParameterizedThreadStart(serialDataReceived));
            dataThread.Start();
        }

        public void SendTCPMessage(string message)
        {
            ns.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
            ns.Flush();
        }

        public void SendSerialMessage(string message)
        {
            serialPort.Write(message);
        }

        private SerialPort ConnectToArduino()
        {
            SerialPort newConnection = null;
            
            newConnection = new SerialPort(ListComPorts()[0]);
            newConnection.Open();

            Console.WriteLine("Connected to the Serial Port...");

            return newConnection;
        }

        public void serialDataReceived(object obj)
        {
            string message = "";

            try
            {
                while (true)
                {
                    byte tmpByte;

                    tmpByte = (byte)serialPort.ReadByte();

                    if(tmpByte != '\n')
                    {
                        message += (char)tmpByte;
                    }
                    else
                    {
                        SendTCPMessage("Sending: " + message);
                        Console.WriteLine("PiBody: " + message);
                        message = "";
                    }
                }
            }
            catch (Exception e)
            {
                Dispose();
            }
        }

        /*
         * Method: ConnectToServer()
         * Parameters: nothing
         * Return: TcpClient
         * Description: Will connect to a named machine on a local network
         */
        private static TcpClient ConnectToServer(string ipAddress)
        {
            TcpClient server = null;

            try
            {
                //connect to the server
                server = new TcpClient(ipAddress, port);

                Console.WriteLine("Connected to the Server...");
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Unable to connect to server");
                throw (Ex);
            }

            return server;
        }

        /*
         * Method: ListComPorts()
         * Parameters: nothing
         * Return: string[]
         * Description: returns a list of available serial ports
         */
        string[] ListComPorts()
        {
            // Get a list of serial port names. 
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console. 
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }

            return ports;
        }

        /*
         * Method: Dispose()
         * Parameters: nothing
         * Return: void
         * Description: when the class is closed, makes sure to
         *              close all data connections safely
         */
        public void Dispose()
        {
            Console.WriteLine("Disconnecting from server...");

            if (serialPort != null)
            {
                serialPort.Dispose();
            }

            if (dataThread != null)
            {
                dataThread.Abort();
            }

            if (ns != null)
            {
                ns.Close();
            }

            if (server != null)
            {
                server.Close();
            }
        }
    }
}
