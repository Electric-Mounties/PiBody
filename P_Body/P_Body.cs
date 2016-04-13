/*
 * Project: S.A.S.A.R
 * File: P_Body.cs
 * Programmer: Matthew Thiessen, Frank Taylor, Jordan Poirier, Tylor McLaughlin
 * First Version: Nov.11/2015
 * Description: This file contains a client program which will
 *              connect and communicate with a server by sending
 *              small text messages back and forth. It will act as
 *              an intermediate between the Atlas and P-Body robots.
 *              It will reside in the P-Body robot.
 * Reference: This project is based on a chat program example found
 *            on http://www.codeproject.com/Articles/16023/Multithreaded-Chat-Server
 */
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace P_Body
{
    /*
     * Class: P_Body
     * Description: Connects to the server, sends and receives
     *              messages.
     */
    public class P_Body
    {
        private static PiComm piComm;
        private static bool running = true;

        /*
         * Method: Main()
         * Parameter: Nothing
         * Return: void
         * Description: This is the main function of the p_Body class
         *              and is responsible for connecting to the server
         *              as well as sending messages to the server.
         */
        public static void Main()
        {
            byte[] data = new byte[1024];
            Random rand = new Random();

            try
            {
                piComm = new PiComm();
            }
            catch(Exception ex)
            {
                piComm = null;
            }

            if (piComm != null)
            {
                //create a new StateObject
                StateObject state = new StateObject();
                state.workSocket = piComm.server.Client;
                piComm.server.Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback((new P_Body()).OnReceive), state);
                //A test to ensure TCP connection 
                while (running)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        piComm.SendTCPMessage("test");
                    }
                    catch(Exception ex)
                    {
                        running = false;
                    }
                }
            }
            Console.WriteLine("Closing Client Application");

            if (piComm != null)
            {
                piComm.Dispose();
            }
            //piComm.server.Client.Dispose();
            //piComm.server.Client.Close();
        }

        /*
         * Method: OnReceive()
         * Parameter: IAsyncResult
         * Return: void
         * Description: Upon receiving data(message) from the
         *              server, display it to the console.
         */
        public void OnReceive(IAsyncResult ar)
        {
            String content = String.Empty;
            Console.WriteLine("Begin");
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead;

            if (handler.Connected)
            {

                // Read data from the client socket. 
                try
                {
                    bytesRead = handler.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        // There  might be more data, so store the data received so far.
                        state.sb.Remove(0, state.sb.Length);
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                        content = state.sb.ToString();

                        if (content.Contains("CMD"))
                        {
                            Console.WriteLine("SERVER: " + content);
                            piComm.SendSerialMessage(content);
                            Console.WriteLine("END");
                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnReceive), state);
                        }
                        else if(content.Contains("LOC"))
                        {
                            Console.WriteLine("SERVER: " + content);
                            //TODO: database the coordinates
                            Console.WriteLine("END");
                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnReceive), state);
                        }
                        else if(content.Contains("disconnect"))
                        {
                            Console.WriteLine(content);
                            running = false;
                        }
                    }
                }
                
                catch (SocketException socketException)
                {
                    //server connection closed abruptly
                    if (socketException.ErrorCode == 10054 || ((socketException.ErrorCode != 10004) && (socketException.ErrorCode != 10053)))
                    {
                        handler.Close();
                    }
                }

                //display exception message
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
            Console.WriteLine("End 2.0");
        }
    }
}