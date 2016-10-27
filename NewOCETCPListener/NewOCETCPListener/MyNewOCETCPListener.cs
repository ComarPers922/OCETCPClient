using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace NewOCETCPListener
{
    public class MyNewOCETCPListener
    {
        private TcpListener tcplistener;
        private Thread listening;

        /// <summary>
        /// Handling new connecting requests
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="tcpclient">Incoming Client Request</param>
        public delegate void TCPAcceptRequestArrived(object sender,TcpClient tcpclient);
        public event TCPAcceptRequestArrived TCPAcceptRequestArrivedHandler;
        /// <summary>
        /// Prepare the listener
        /// </summary>
        private void InitializeThread()
        {
            tcplistener.Start();
            listening = new Thread(delegate()
                {
                    while (true)
                    {
                        var temp = tcplistener.AcceptTcpClient();
                        if (TCPAcceptRequestArrivedHandler != null)
                        {
                            TCPAcceptRequestArrivedHandler(this, temp);
                        }
                    }
                });
            listening.IsBackground = true;
            listening.Start();
        }        
        /// <summary>
        /// Initialize a listener using port
        /// </summary>
        /// <param name="port"></param>
        [Obsolete("This method is not recommanded!")]
        public MyNewOCETCPListener(int port)
        {
            this.tcplistener = new TcpListener(port);
        }
        /// <summary>
        /// Initianlize a listener using ipaddress and port
        /// </summary>
        /// <param name="addr">IPAddress</param>
        /// <param name="port">Port</param>
        public MyNewOCETCPListener(IPAddress addr,int port)
        {
            this.tcplistener = new TcpListener(addr,port);
        }
        /// <summary>
        /// Start the listener
        /// </summary>
        public void Start()
        {
            this.InitializeThread();
        }
        /// <summary>
        /// Close the listener
        /// </summary>
        public void Close()
        {
            this.tcplistener.Stop();
            this.listening.Abort();
        }
    }
}
