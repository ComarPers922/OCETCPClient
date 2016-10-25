﻿using System;
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

        public delegate void TCPAcceptRequestArrived(object sender,TcpClient tcpclient);
        public event TCPAcceptRequestArrived TCPAcceptRequestArrivedHandler;

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
        
        [Obsolete("This method is not recommanded!")]
        public MyNewOCETCPListener(int port)
        {
            this.tcplistener = new TcpListener(port);
        }
        public MyNewOCETCPListener(IPAddress addr,int port)
        {
            this.tcplistener = new TcpListener(addr,port);
        }
        public void Start()
        {
            this.InitializeThread();
        }
        public void Close()
        {
            this.tcplistener.Stop();
            this.listening.Abort();
        }
    }
}
