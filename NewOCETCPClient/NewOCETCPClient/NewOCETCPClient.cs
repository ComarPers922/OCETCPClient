using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace NewOCETCPClient
{
    public class MyNewOCETCPClient
    {
        private TcpClient tcpclient;
        private Thread receiver;
        private NetworkStream ns;
        /// <summary>
        /// Handle incoming messgae
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="message">Incoming message</param>
        public delegate void MessageArrived(object sender, byte[] message);
        public event MessageArrived MessageArrivedHandler;
        /// <summary>
        /// Create a new TCPClient
        /// </summary>
        public MyNewOCETCPClient()
        {
            tcpclient = new TcpClient();
        }
        /// <summary>
        /// Create a new TCPClient using copy from other instance
        /// </summary>
        /// <param name="tcpclient">Other instance</param>
        public MyNewOCETCPClient(TcpClient tcpclient)
        {
            this.tcpclient = tcpclient;
            Initialize();
        }
        /// <summary>
        /// Initialize new instance using
        /// </summary>
        private void Initialize()
        {
            ns = tcpclient.GetStream();
            receiver = new Thread(delegate()
            {
                while (true)
                {
                    if (tcpclient.Available < 1)
                    {
                        continue;
                    }
                    byte[] b = new byte[tcpclient.Available];
                    ns.Read(b, 0, b.Length);
                    if (this.MessageArrivedHandler != null)
                    {
                        MessageArrivedHandler(this, b);
                    }
                }
            });
            receiver.IsBackground = true;
            receiver.Start();
        }
        /// <summary>
        /// Connect to Server
        /// </summary>
        /// <param name="addr">IPAddress</param>
        /// <param name="port">Port</param>
        public void Connect(string addr, int port)
        {
            this.tcpclient.Connect(IPAddress.Parse(addr), port);
            Initialize();
        }
        /// <summary>
        /// Send message using bytes
        /// </summary>
        /// <param name="message">Message attempted to send</param>
        public void Send(byte[] message)
        {
            ns.Write(message, 0, message.Length);
        }
        /// <summary>
        /// Send string message using Default encoding directly.
        /// </summary>
        /// <param name="message">Message attempted to send</param>
        public void SendString(string message)
        {
            SendString(message,Encoding.Default);
        }
        /// <summary>
        /// Send string message directly
        /// </summary>
        /// <param name="message">Message attempted to send</param>
        /// <param name="encoding">Encoding</param>
        public void SendString(string message , Encoding encoding)
        {
            this.Send(encoding.GetBytes(message));
        }
        /// <summary>
        /// Close this client
        /// </summary>
        public void Close()
        {
            receiver.Abort();
            ns.Dispose();
            tcpclient.Close();
        }
    }
}
