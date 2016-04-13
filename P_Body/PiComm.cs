/*
 * Project: S.A.S.A.R.
 * File: PiComm.cs
 * Programmer: Matthew Thiessen, Frank Taylor, Jordan Poirier, Tylor McLaughlin
 * First Version: Nov.11/2015
 * Description: This file handels the piBodys communication
 * Reference: This project is based on a chat program example found
 *            on http://www.codeproject.com/Articles/16023/Multithreaded-Chat-Server
 */

using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;

   
namespace P_Body
{
    /*
     * Class: PiComm 
     * Description: Holds meathods for the Pi's communication
     */
    public class PiComm : IDisposable
    {
        //Decleration of variables
        private SerialPort serialPort = null;
        private const int port = 2021;
        private Thread dataThread = null;
        private string ipAddress = "192.168.43.124";
        public TcpClient server = null;
        public NetworkStream ns = null;



        public PiComm(string portName)
        {
            //Call to connect server setting up IP and Port
            server = ConnectToServer(ipAddress);
            serialPort = ConnectToArduino();
            ns = server.GetStream();
            //Create a new thread
            dataThread = new Thread(new ParameterizedThreadStart(serialDataReceived));
            dataThread.Start();
        }

        public PiComm()
        {
            //Call to connect server setting up IP and Port
            server = ConnectToServer(ipAddress);
            serialPort = ConnectToArduino();
            ns = server.GetStream();
            //Create a new thread
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
            //search for serial ports
            SerialPort newConnection = null;
            
            newConnection = new SerialPort(ListComPorts()[0]);
            newConnection.Open();
            //display serial connected serial port
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
                    //Read bytes in until the terminator is read
                    if(tmpByte != '\n')
                    {
                        message += (char)tmpByte;
                    }
                    //Send the message
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
            //If unable to connect to the server catch exception
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

            //Gracefull shut down on dissconnect
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
