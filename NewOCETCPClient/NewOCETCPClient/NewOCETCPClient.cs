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
        public delegate void MessageArrived(object sender, byte[] message);
        public event MessageArrived MessageArrivedHandler;
        public MyNewOCETCPClient()
        {
            tcpclient = new TcpClient();
        }
        public MyNewOCETCPClient(TcpClient tcpclient)
        {
            this.tcpclient = tcpclient;
            Initialize();
        }
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
        public void Connect(string addr, int port)
        {
            this.tcpclient.Connect(IPAddress.Parse(addr), port);
            Initialize();
        }
        public void Send(byte[] message)
        {
            ns.Write(message, 0, message.Length);
        }
        public void SendString(string message)
        {
            SendString(message,Encoding.Default);
        }
        public void SendString(string message , Encoding encoding)
        {
            this.Send(encoding.GetBytes(message));
        }
        public void Close()
        {
            receiver.Abort();
            ns.Dispose();
            tcpclient.Close();
        }
    }
}
