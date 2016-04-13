/*
 * Project: Yellow Taxi
 * File: StateObject.cs
 * Programmer: Matthew Thiessen, Frank Taylor, Jordan Poirier, Tylor McLaughlin
 * First Version: Nov.11/2015
 * Description: This file contains the State Object class which
 *              is used to read data from Client.
 * Reference: This project is based on a chat program example found
 *            on http://www.codeproject.com/Articles/16023/Multithreaded-Chat-Server
 */

using System.Net.Sockets;
using System.Text;

namespace P_Body
{
    /*
     * Class: StateObject
     * Description: used to read data from Client
     */
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}