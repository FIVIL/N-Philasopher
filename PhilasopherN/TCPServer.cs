using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PhilasopherN
{
    public delegate void MessageRecive(string s);
    public delegate void MessageReciveByte(byte[] b);
    class TCPServer
    {
        private Socket SocServer;
        private Socket SocClient;
        private IPEndPoint IP;
        public event MessageRecive OnMessageRecive;
        public event MessageReciveByte OnMessageReciveByte;
        public int Port { get => IP.Port; }
        public TCPServer(int port, string ip = "127.0.0.1")
        {
            Connect(port, ip);
        }
        private void Connect(int port, string ip)
        {
            SocServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocClient = null;
            IP = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        public void Start()
        {
            Thread TrStart = new Thread(new ThreadStart(start));
            TrStart.Start();
        }
        private void start()
        {
            SocServer.Bind(IP);
            SocServer.Listen(1);
            SocClient = SocServer.Accept();
            Task.Factory.StartNew(getMessage);
        }
        private void getMessage()
        {
            try
            {
                while (true)
                {
                    byte[] barray = new byte[1024];
                    int RecB = SocClient.Receive(barray);
                    if (RecB > 0)
                    {
                        string mess = Encoding.Unicode.GetString(barray, 0, RecB);
                        OnMessageRecive?.Invoke(mess);
                        OnMessageReciveByte?.Invoke(barray);
                    }
                    Thread.Sleep(1);
                }
            }
            catch
            {
            }
        }
        public void Send(string s)
        {
            byte[] b;
            b = Encoding.Unicode.GetBytes(s);
            Send(b);
        }
        public void Send(byte[] inbyte)
        {
            SocClient.Send(inbyte);
        }
        public void Stop()
        {
            try
            {
                if (SocClient != null)
                {
                    SocClient.Shutdown(SocketShutdown.Both);
                    SocClient.Close();
                    SocClient.Dispose();
                    SocClient = null;
                }
                if (SocServer != null)
                {
                    SocServer.Shutdown(SocketShutdown.Both);
                    SocServer.Close();
                    SocServer.Dispose();
                    SocServer = null;
                }
            }
            catch { }
        }
        public void Restart(int port,string ip = "127.0.0.1")
        {
            Stop();
            Connect(port, ip);
            Start();
        }
    }
}
