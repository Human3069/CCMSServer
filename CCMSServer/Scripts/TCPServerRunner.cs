using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace CCMSServer.Scripts
{
    internal class TCPServerRunner
    {
        private const string LOG_FORMAT = "[TCPServerRunner] {0}";

        protected TcpListener listener;
        protected List<TcpClient> clientList;

        protected Thread listenerThread;
        protected Action<TcpClient, string> _onReceivedAction;

        protected bool isDisposed = false;

        protected bool _useLog = false;

        internal TCPServerRunner(int serverPort, Action<TcpClient, string> onReceivedAction, bool useLog = false)
        {
            try
            {
                clientList = new List<TcpClient>();
                _onReceivedAction = onReceivedAction;

                listener = new TcpListener(IPAddress.Any, serverPort);
                listener.Start();

                listenerThread = new Thread(ReceiveFromClientsAsync);
                listenerThread.Start();

                _useLog = useLog;

                if (_useLog == true)
                {
                    Console.WriteLine(LOG_FORMAT, "Server started on " + listener.LocalEndpoint + " : " + ((IPEndPoint)listener.LocalEndpoint).Port);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(LOG_FORMAT, e.Message);
            }
        }

        ~TCPServerRunner()
        {
            Dispose(false);
        }

        internal void SendMessageTo(TcpClient client, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
            }
            catch (Exception exception)
            {
                Console.WriteLine(LOG_FORMAT, "Failed to SendMessageAll : " + exception.Message);
            }

            if (_useLog == true)
            {
                Console.WriteLine(LOG_FORMAT, "SendMessage : " + message);
            }
        }

        internal void SendMessageAll(string message)
        {
            foreach (TcpClient client in clientList)
            {
                SendMessageTo(client, message);
            }
        }

        protected void ReceiveFromClientsAsync()
        {
            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    lock (clientList)
                    {
                        clientList.Add(client);
                    }

                    if (_useLog == true)
                    {
                        Console.WriteLine(LOG_FORMAT, "Client connected : " + client.Client.RemoteEndPoint);
                    }

                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);
                }
                catch (Exception e)
                {
                    Console.WriteLine(LOG_FORMAT, e.Message);
                    break;
                }
            }
        }

        protected void HandleClient(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (_useLog == true)
                    {
                        Console.WriteLine(LOG_FORMAT, "Received message : " + message);
                    }

                    _onReceivedAction.Invoke(client, message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(LOG_FORMAT, "Exception : " + e.Message);
            }
            finally
            {
                lock (clientList)
                {
                    clientList.Remove(client);
                }

                client.Close();
                if (_useLog == true)
                {
                    Console.WriteLine(LOG_FORMAT, "Client disconnected : " + client.Client.RemoteEndPoint);
                }
            }
        }

        internal void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposed == false)
            {
                if (isDisposing == true)
                {
                    if (listenerThread != null)
                    {
                        listenerThread.Abort();
                        listenerThread = null;
                    }

                    if (listener != null)
                    {
                        listener.Stop();
                        listener = null;
                    }

                    lock (clientList)
                    {
                        foreach (var client in clientList)
                        {
                            client.Close();
                        }
                        clientList.Clear();
                    }
                }

                isDisposed = true;
            }
        }
    }
}
