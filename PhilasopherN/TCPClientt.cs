using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PhilasopherN
{
    class TCPClientt
    {
        private Socket SClient;
        private IPEndPoint IP;
        public event MessageRecive OnMessageRecive;
        public event MessageReciveByte OnMessageReciveByte;
        public int Port { get => IP.Port; }
        public TCPClientt(int port, string ip = "127.0.0.1")
        {
            Connect(port, ip);
        }
        public void Connect()
        {
            try
            {
                SClient.Connect(IP);
                Task.Factory.StartNew(getMessage);
            }
            catch
            {
            }
        }
        private void Connect(int port, string ip)
        {
            SClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IP = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        private void getMessage()
        {
            try
            {
                while (true)
                {
                    byte[] barray = new byte[1024];
                    int RecB = SClient.Receive(barray);
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
            SClient.Send(inbyte);
        }
        public void Disconnect()
        {
            try
            {
                if (SClient != null)
                {
                    SClient.Shutdown(SocketShutdown.Both);
                    SClient.Close();
                    SClient.Dispose();
                    SClient = null;
                }
            }
            catch { }
        }
        public void Reconnect(int port, string ip = "127.0.0.1")
        {
            Disconnect();
            Connect(port, ip);
            Connect();
        }
    }
}
